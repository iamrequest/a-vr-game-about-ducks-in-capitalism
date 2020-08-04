using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Delegates button presses to the current QuestDialog
public class DialogDelegator : MonoBehaviour {
    public List<QuestDialog> acts;
    public int activeActIndex;

    public void StartDialog() {
        // No acts exist
        if (acts.Count == 0) {
            Debug.Log("Missing objective!");
            return;
        }

        // Failsafe
        if (activeActIndex > acts.Count) {
            Debug.LogError("Bad count of acts!");
            return;
        }
        if (acts[activeActIndex] == null) {
            Debug.LogError("Null objective!");
            return;
        }

        // -- Test finishing of the quest
        if (acts[activeActIndex].IsComplete()) {
            if (IsFinalObjective()) {
                // Move on to the next objective
                Debug.LogError("Attempted to start dialog after the last act was completed");
                return;
            } else {
                // Move on to the next objective
                activeActIndex++;
                StartDialog();
                return;
            }
        }

        acts[activeActIndex].StartDialog();
    }

    private bool IsFinalObjective() {
        return activeActIndex == acts.Count - 1;
    }
}

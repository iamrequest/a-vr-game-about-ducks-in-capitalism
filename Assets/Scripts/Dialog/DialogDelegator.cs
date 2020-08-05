using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Delegates button presses to the current QuestDialog (act)
public class DialogDelegator : MonoBehaviour {
    public List<QuestDialog> acts;
    public int activeActIndex;
    private bool lastActInvoked;

    [Tooltip("The delay before setting up act 1")]
    public float initialDelay;

    public static DialogDelegator instance;
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        StartCoroutine(SetupActAfterDelay(initialDelay));
    }

    public void StartDialog() {
        // No acts exist
        if (acts.Count == 0) {
            Debug.Log("Missing objective!");
            return;
        }

        // Failsafe
        if (activeActIndex > acts.Count - 1) {
            Debug.LogError("Bad count of acts!");
            return;
        }
        if (acts[activeActIndex] == null) {
            Debug.LogError("Null objective!");
            return;
        }

        acts[activeActIndex].StartDialog();
    }




    private void SetupAct() {
        if (activeActIndex > acts.Count - 1) {
            Debug.LogError("Attempted to setup act " + activeActIndex + ", but there was not enough acts configured");
        } else {
            acts[activeActIndex].onActStart.Invoke();
            StartCoroutine(acts[activeActIndex].StartActAfterInitDelay());
        }
    }

    private IEnumerator SetupActAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        SetupAct();
    }

    public void StartNextActAfterDelay(float delay) {
        acts[activeActIndex].onActEnd.Invoke();

        // TODO: This stuff still triggers after the last act is complete. No damage, but it sends some debug error logs
        activeActIndex++;
        StartCoroutine(SetupActAfterDelay(delay));
    }
}

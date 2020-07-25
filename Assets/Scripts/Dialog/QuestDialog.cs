using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Quick spaghetti that maintains a "quest" system.
//  User has a list of objectives. For each objective, there's start/finish dialog that must be played to advance.
//  During the objective, the NPC will loop through the same convo until the objective is marked as complete
public class QuestDialog : BaseDialog {
    public List<QuestObjective> objectives;
    public int activeObjectiveIndex;

    //public Conversation allObjectivesCompleteDialog;
    public RandomDialog postObjectiveRandomDialog;

    public override void StartDialog() {
        // No quests exist
        if (objectives.Count == 0) {
            postObjectiveRandomDialog.StartDialog();
            //dialogManager.StartDialog(this, allObjectivesCompleteDialog, true);
            return;
        }

        // Failsafe
        if (activeObjectiveIndex > objectives.Count) {
            Debug.LogError("Bad count of objectives!");
            return;
        }
        if (objectives[activeObjectiveIndex] == null) {
            Debug.LogError("Null objective!");
            return;
        }

        QuestObjective o = objectives[activeObjectiveIndex];

        // -- Test finishing of the quest
        if (o.initialDialogComplete && o.finalDialogComplete && o.isComplete) {
            if (IsFinalObjective()) {
                //dialogManager.StartDialog(this, allObjectivesCompleteDialog, true);
                postObjectiveRandomDialog.StartDialog();
                return;
            } else {
                activeObjectiveIndex++;

                StartDialog();
                return;
            }
        }

        // -- Quest is in progress
        if (!o.initialDialogComplete) {
            dialogManager.StartDialog(this, o.initialDialog, true);

            return;
        }

        if (!o.isComplete) {
            dialogManager.StartDialog(this, o.repeatDialog, true);
            return;
        }

        if (!o.finalDialogComplete) {
            dialogManager.StartDialog(this, o.onCompleteDialog, true);
            return;
        }

        Debug.LogError("This shouldn't be reached");
    }

    public override void OnDialogEnd(bool wasDialogFullyCompleted) {
        // -- Only advance objective dialog when the conversation ended completely
        if (wasDialogFullyCompleted) {
            if (objectives[activeObjectiveIndex] == null) {
                Debug.LogError("Objective " + activeObjectiveIndex + " is null!");
                return;
            }
            
            // -- Test the initial dialog for completion
            if (!objectives[activeObjectiveIndex].initialDialogComplete) {
                objectives[activeObjectiveIndex].initialDialogComplete = true;
                objectives[activeObjectiveIndex].onObjectiveStart.Invoke();
                return;
            }

            // If you've finished talking to the NPC, and the quest has been completed, advance to the next quest
            if (objectives[activeObjectiveIndex].isComplete) {
                objectives[activeObjectiveIndex].finalDialogComplete = true;
                objectives[activeObjectiveIndex].onObjectiveComplete.Invoke();
            }
        }
    }

    // Current objective is complete, and the final dialog for this quest has been said
    private bool IsReadyToAdvanceToNextObjective() {
        return objectives[activeObjectiveIndex].isComplete 
            && objectives[activeObjectiveIndex].finalDialogComplete;
    }

    private bool IsFinalObjective() {
        return activeObjectiveIndex == objectives.Count - 1;
    }
}

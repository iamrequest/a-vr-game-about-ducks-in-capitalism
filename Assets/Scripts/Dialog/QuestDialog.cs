using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Slightly better spaghetti that maintains a list of coffee orders
//  User has a list of objectives. For each objective, there's start/finish dialog that must be played to advance.
public class QuestDialog : BaseDialog {
    public List<QuestObjective> objectives;
    public int activeObjectiveIndex;
    public Slottable coffeeMug;
    private CoffeeContainer coffeeContainer;

    private void Start() {
        coffeeContainer = coffeeMug.GetComponent<CoffeeContainer>();
    }

    public override void StartDialog() {
        // No quests exist
        if (objectives.Count == 0) {
            Debug.Log("Missing objective!");
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

        // -- Test finishing of the quest
        if (objectives[activeObjectiveIndex].state == QuestObjectiveState.ALL_DIALOG_COMPLETE) {
            if (IsFinalObjective()) {
                // Move on to the next objective
                Debug.LogError("Attempted to start dialog after the last objective was completed");
                return;
            } else {
                // Move on to the next objective
                activeObjectiveIndex++;
                StartDialog();
                return;
            }
        }


        // -- Quest is in progress
        // TODO: Post order dialog not triggering automatically
        // TODO: Evaluate coffee
        switch (objectives[activeObjectiveIndex].state) {
            case QuestObjectiveState.PRE_ORDER:
                dialogManager.StartDialog(this, objectives[activeObjectiveIndex].preOrderDialog, true);
                break;

            case QuestObjectiveState.POST_ORDER:
                dialogManager.StartDialog(this, objectives[activeObjectiveIndex].postOrderDialog, true);
                break;

            case QuestObjectiveState.WAITING:
                if (!coffeeMug.isSlotted) {
                    // No coffee presented to NPC. Looping dialog
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].waitingDialog, true);
                    break;

                } 

                // Initial evaluation dialog
                dialogManager.StartDialog(this, objectives[activeObjectiveIndex].evaluateCoffeeDialog, true);

                // Test if the coffee is good or not, and react accordingly
                EvaluateCoffee();
                if (objectives[activeObjectiveIndex].isCoffeeOrderValid) {
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].orderReceivedDialog, false);

                    // Advance to "order received"
                    objectives[activeObjectiveIndex].AdvanceDialogState();
                } else {
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].badCoffeeDialog, false);
                }

                break;

            default:
                Debug.LogError("Unexpected QuestObjectiveState met in switch-case");
                break;
        }
    }

    public override void OnDialogEnd(bool wasDialogFullyCompleted) {
        if (objectives[activeObjectiveIndex] == null) {
            Debug.LogError("Objective " + activeObjectiveIndex + " is null!");
            return;
        }

        switch (objectives[activeObjectiveIndex].state) {
            // Start the post-order dialog immediately
            case QuestObjectiveState.PRE_ORDER:
                objectives[activeObjectiveIndex].AdvanceDialogState();
                StartDialog();
                break;

            // Don't start dialog immediately. Invoke a unity event
            case QuestObjectiveState.POST_ORDER:
                objectives[activeObjectiveIndex].AdvanceDialogState();
                objectives[activeObjectiveIndex].onOrderServed.Invoke();
                break;

            // Don't advance until coffee is served
            case QuestObjectiveState.WAITING:
                break;

            // Don't start dialog immediately. Invoke a unity event
            case QuestObjectiveState.COFFEE_RECEIVED:
                objectives[activeObjectiveIndex].AdvanceDialogState();
                objectives[activeObjectiveIndex].onLastDialogComplete.Invoke();
                break;
        }
    }

    private bool IsFinalObjective() {
        return activeObjectiveIndex == objectives.Count - 1;
    }


    public void EvaluateCoffee () {
        if (objectives[activeObjectiveIndex] == null) {
            Debug.LogError("Unable to evaluate coffee: null objective");
            return;
        } 

        objectives[activeObjectiveIndex].isCoffeeOrderValid = false;

        // Capacity
        if (coffeeContainer.currentCoffeeLevel < objectives[activeObjectiveIndex].minCapacityPercentage) {
            return;
        }

        // Sugar content
        if (objectives[activeObjectiveIndex].requiredSugar != coffeeContainer.GetSugarAmount()) {
            return;
        }

        // Has at least the minimum required cream percentage
        if (coffeeContainer.creamPercentage < objectives[activeObjectiveIndex].minCream) {
            return;
        }

        // Has no more than the maximum amount of cream
        if (coffeeContainer.creamPercentage > objectives[activeObjectiveIndex].maxCream) {
            return;
        }

        objectives[activeObjectiveIndex].isCoffeeOrderValid = true;
    }
}

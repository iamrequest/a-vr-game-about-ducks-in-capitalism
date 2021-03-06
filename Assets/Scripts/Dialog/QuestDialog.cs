﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

// Slightly better spaghetti that maintains a list of coffee orders
//  User has a list of objectives. For each objective, there's start/finish dialog that must be played to advance.
public class QuestDialog : BaseDialog {
    public List<QuestObjective> objectives;
    public int activeObjectiveIndex;
    public Slottable coffeeMug;
    private CoffeeContainer coffeeContainer;
    private Animator orderSlipAnimator;
    public TextMeshProUGUI orderSlipText;

    [Header("Setup/teardown")]
    public UnityEvent onActStart;
    public UnityEvent onActEnd;
    public float dialogStartDelay, dialogEndDelay;
    private bool isActInitialized;

    private void Start() {
        coffeeContainer = coffeeMug.GetComponent<CoffeeContainer>();
        orderSlipAnimator = orderSlipText.GetComponentInParent<Animator>();
    }

    public override void StartDialog() {
        if (!isActInitialized) {
            return;
        }

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
                HideCoffee();
                EvaluateCoffee();
                if (objectives[activeObjectiveIndex].isCoffeeOrderValid) {
                    dialogManager.StartDialog(this, objectives[activeObjectiveIndex].orderReceivedDialog, false);
                    coffeeContainer.EmptyCup();

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
                CreateOrderSlip();
                coffeeMug.gameObject.SetActive(true);

                objectives[activeObjectiveIndex].AdvanceDialogState();
                objectives[activeObjectiveIndex].onOrderReceived.Invoke();
                StartDialog();
                break;

            // Don't start dialog immediately. Invoke a unity event
            case QuestObjectiveState.POST_ORDER:
                objectives[activeObjectiveIndex].AdvanceDialogState();
                objectives[activeObjectiveIndex].onOrderServed.Invoke();
                break;

            // Don't advance until coffee is served
            case QuestObjectiveState.WAITING:
                coffeeMug.gameObject.SetActive(true);
                break;

            // Final dialog complete. 
            case QuestObjectiveState.COFFEE_RECEIVED:
                DestroyOrderSlip();

                objectives[activeObjectiveIndex].AdvanceDialogState();
                objectives[activeObjectiveIndex].onLastDialogComplete.Invoke();

                if (IsFinalObjective()) {
                    GetComponentInParent<DialogDelegator>().StartNextActAfterDelay(dialogEndDelay);
                } else {
                    StartDialog();
                }
                break;
        }
    }

    private bool IsFinalObjective() {
        return activeObjectiveIndex == objectives.Count - 1;
    }

    public bool IsComplete() {
        return activeObjectiveIndex == objectives.Count - 1 && 
            objectives[activeObjectiveIndex].state == QuestObjectiveState.ALL_DIALOG_COMPLETE;
    }


    public void EvaluateCoffee() {
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

    private void CreateOrderSlip() {
        orderSlipAnimator.SetBool("isOrderSlipActive", true);

        if (objectives[activeObjectiveIndex].minCream == objectives[activeObjectiveIndex].maxCream) {
            // Exact cream percentage
            orderSlipText.text = "Coffee\n- Sugar: " + objectives[activeObjectiveIndex].requiredSugar +
                "\n- Cream: " + FormatPercentage(objectives[activeObjectiveIndex].minCream);
        } else {
            // Range of cream percentages
            orderSlipText.text = "Coffee\n- Sugar: " + objectives[activeObjectiveIndex].requiredSugar +
                "\n- Cream:\n  " +
                FormatPercentage(objectives[activeObjectiveIndex].minCream) + " - " +
                FormatPercentage(objectives[activeObjectiveIndex].maxCream);
        }
    }

    /// <summary>
    /// Formats a float percentage .5 as "50%"
    /// </summary>
    /// <param name="percentage">A float, from 0 to 1</param>
    private string FormatPercentage(float percentage) {
        return percentage * 100 + "%";
    }

    private void DestroyOrderSlip() {
        orderSlipAnimator.SetBool("isOrderSlipActive", false);
    }

    public IEnumerator StartActAfterInitDelay() {
        // Stop early if we're not actually at the beginning of this act
        if (activeObjectiveIndex != 0) {
            Debug.LogError("Attempted to start dialog after delay, but we're not on the first objective of the quest");
            yield break;
        }

        Debug.Log("Starting dialog after " + dialogStartDelay + " seconds...");
        yield return new WaitForSeconds(dialogStartDelay);
        Debug.Log("Waited for delay - starting dialog.");

        // Begin the initial dialog
        isActInitialized = true;
        StartDialog();
    }

    /// <summary>
    /// Slot the coffee, and set it as inactive
    /// </summary>
    private void HideCoffee() {
        coffeeMug.SnapToSlot();
        coffeeContainer.gameObject.SetActive(false);
    }
}

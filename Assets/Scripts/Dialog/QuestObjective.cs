using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum QuestObjectiveState {
    PRE_ORDER, POST_ORDER, WAITING, COFFEE_RECEIVED, ALL_DIALOG_COMPLETE
}
public class QuestObjective : MonoBehaviour {
    [Header("Coffee order")]
    [Range(0f, 1f)]
    public float minCapacityPercentage = 0.8f;
    [Range(0f, 1f)]
    public float minCream, maxCream;
    public int requiredSugar;

    public bool isCoffeeOrderValid;

    [Header("Dialog")]
    public UnityEvent onOrderReceived;
    public UnityEvent onOrderServed;
    public UnityEvent onLastDialogComplete;

    public QuestObjectiveState state { get; private set; }

    // preOrderDialog: The order is created after this conversation
    // postOrderDialog: Idle chatting that happens while waiting for the order
    // waitingDialog: Dialog that loops after postOrderDialog is complete. Finishes once the order is served
    // postOrderDialog: Dialog that happens after getting the order
    public Conversation preOrderDialog;
    public Conversation postOrderDialog;
    public Conversation waitingDialog;
    public Conversation evaluateCoffeeDialog; // Initial dialog for coffee evaluation
    public Conversation badCoffeeDialog; // Customer isn't happy with the coffee supplied
    public Conversation orderReceivedDialog;

    public void AdvanceDialogState() {
        if (state != QuestObjectiveState.ALL_DIALOG_COMPLETE) {
            state++;
        }
    }
}

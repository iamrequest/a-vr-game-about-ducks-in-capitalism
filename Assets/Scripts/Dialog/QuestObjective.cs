using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class QuestObjective : MonoBehaviour {
    public string title;
    public string description;

    public bool isComplete;
    public bool initialDialogComplete;
    public bool finalDialogComplete;

    public UnityEvent onObjectiveStart, onObjectiveComplete;

    // initialDialog: Dialog that plays once before the player gets the objective
    // repeatDialog: Dialog that loops until the player completes the objective
    public Conversation initialDialog;
    public Conversation repeatDialog;
    public Conversation onCompleteDialog;

    protected virtual void Start() {
        //isComplete = false;
        //initialDialogComplete = false;
        finalDialogComplete = false;
    }

    // Called via unityevent
    public virtual void CompleteObjective() {
        isComplete = true;
    }
}

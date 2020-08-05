using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class DialogInteractor : MonoBehaviour {
    [Header("SteamVR")]
    public SteamVR_Action_Boolean skipSentenceAction;
    public SteamVR_Action_Boolean dialogInteractAction;
    private Hand hand;

    // -- Dialog management
    public DialogDelegator dialogDelegator;
    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start() {
        hand = GetComponentInParent<Hand>();

        skipSentenceAction.AddOnStateUpListener(SkipSentence, hand.handType);
        dialogInteractAction.AddOnStateUpListener(DialogInteract, hand.handType);
    }

    private void SkipSentence(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        ConfigureDialogComponentReferences();

        if (dialogManager != null) {
            dialogManager.SkipCurrentSentence();
        } else {
            Debug.LogError("No dialog manager assigned!");
        }
    }

    private void DialogInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        ConfigureDialogComponentReferences();

        // -- Next dialog line
        if (dialogDelegator != null) {
            if (dialogManager.isDialogActive) {
                dialogManager.DisplayNextSentence();
            } else {
                dialogDelegator.StartDialog();
            }
        } else {
            Debug.LogError("No dialog delegate assigned!");
        }
    }

    // On scene load, dialogManager and dialogDelegator won't be set. Do so here
    private void ConfigureDialogComponentReferences() {
        dialogDelegator = DialogDelegator.instance;
        dialogManager = DialogManager.instance;
    }
}

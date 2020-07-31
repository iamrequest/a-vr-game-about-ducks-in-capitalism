using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

// TODO: This can cause issues where the player walks away mid-dialog.
//  How to test which dialog manager we're looking at? Multiple dialog managers (multi-person convo?)
public class DialogInteractor : MonoBehaviour {
    [Header("SteamVR")]
    public SteamVR_Action_Boolean skipSentenceAction;
    public SteamVR_Action_Boolean dialogInteractAction;
    private Hand hand;

    // -- Dialog management
    public DialogManager activeDialogManager;
    private DialogInteractor otherDialogInteractor;

    // Start is called before the first frame update
    void Start() {
        hand = GetComponentInParent<Hand>();
        otherDialogInteractor = hand.otherHand.GetComponentInChildren<DialogInteractor>();

        skipSentenceAction.AddOnStateUpListener(SkipSentence, hand.handType);
        dialogInteractAction.AddOnStateUpListener(DialogInteract, hand.handType);
    }

    private void SkipSentence(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (activeDialogManager != null) {
            otherDialogInteractor.activeDialogManager = activeDialogManager;
            activeDialogManager.SkipCurrentSentence();
        }
    }

    private void DialogInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        // -- Next dialog line
        if (activeDialogManager != null) {
            otherDialogInteractor.activeDialogManager = activeDialogManager;

            // Start dialog
            if (!activeDialogManager.isDialogActive) {
                activeDialogManager.DisplayNextSentence();
            }
        }
    }
}

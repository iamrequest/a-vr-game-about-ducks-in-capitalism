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
    public DialogDelegator dialogDelegator;
    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start() {
        hand = GetComponentInParent<Hand>();

        skipSentenceAction.AddOnStateUpListener(SkipSentence, hand.handType);
        dialogInteractAction.AddOnStateUpListener(DialogInteract, hand.handType);
    }

    private void SkipSentence(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (dialogManager != null) {
            dialogManager.SkipCurrentSentence();
        } else {
            Debug.LogError("No dialog manager assigned!");
        }
    }

    private void DialogInteract(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
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
}

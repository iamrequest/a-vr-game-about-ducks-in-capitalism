using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDialogInteractor : MonoBehaviour {
    public QuestDialog exampleDialog;
    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start() {
        //exampleDialog = GetComponent<QuestDialog>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.G)) {
            //dialogManager.StartDialog(exampleDialog, 
            exampleDialog.StartDialog();
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            if (dialogManager.isDialogActive) {
                dialogManager.DisplayNextSentence();
            } else {
                exampleDialog.StartDialog();
            }
        }

        if (Input.GetKeyUp(KeyCode.B)) {
            exampleDialog.objectives[exampleDialog.activeObjectiveIndex].isCoffeeOrderValid = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardDialogInteractor : MonoBehaviour {
    public BaseDialog exampleDialog;
    public DialogManager dialogManager;

    // Start is called before the first frame update
    void Start() {
        exampleDialog = GetComponent<BaseDialog>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyUp(KeyCode.G)) {
            //dialogManager.StartDialog(exampleDialog, 
            exampleDialog.StartDialog();
        }

        if (dialogManager.isDialogActive && Input.GetKeyUp(KeyCode.Space)) {
            dialogManager.DisplayNextSentence();
        }
    }
}

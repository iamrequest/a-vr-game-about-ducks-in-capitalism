using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Delegates button presses to the current QuestDialog (act)
public class DialogDelegator : MonoBehaviour {
    public List<QuestDialog> acts;
    public int activeActIndex;

    public Animator openingCinematicAnimator;
    public bool setupInitialActAfterDelay;
    private bool isInitialActInProgress;
    [Tooltip("The delay before setting up act 1")]
    public float initialDelay;

    public static DialogDelegator instance;
    void Awake() {
        if (instance != null && instance != this) {
            Destroy(this.gameObject);
            return;
        }

        instance = this;
    }

    private void Start() {
        if (setupInitialActAfterDelay) {
            StartCoroutine(SetupActAfterDelay(initialDelay));
        }
    }

    public void StartDialog() {
        // Setup initial act via button press, rather than via delay
        if (!isInitialActInProgress) {
            StartCoroutine(SetupActAfterDelay(initialDelay));
        }

        // No acts exist
        if (acts.Count == 0) {
            Debug.Log("Missing objective!");
            return;
        }

        // Failsafe
        if (activeActIndex > acts.Count - 1) {
            Debug.LogError("Bad count of acts!");
            return;
        }
        if (acts[activeActIndex] == null) {
            Debug.LogError("Null objective!");
            return;
        }

        acts[activeActIndex].StartDialog();
    }




    private void SetupAct() {
        if (activeActIndex > acts.Count - 1) {
            Debug.LogError("Attempted to setup act " + activeActIndex + ", but there was not enough acts configured");
        } else {
            acts[activeActIndex].onActStart.Invoke();
            StartCoroutine(acts[activeActIndex].StartActAfterInitDelay());
        }
    }

    private IEnumerator SetupActAfterDelay(float delay) {
        // If there's an opening cinematic in this scene, then play it.
        // This is really only the case for the initial scene
        if (openingCinematicAnimator != null) {
            openingCinematicAnimator.SetTrigger("startOpeningCinematic");
        }

        yield return new WaitForSeconds(delay);
        isInitialActInProgress = true;
        SetupAct();
    }

    public void StartNextActAfterDelay(float delay) {
        acts[activeActIndex].onActEnd.Invoke();

        // TODO: This stuff still triggers after the last act is complete. No damage, but it sends some debug error logs
        activeActIndex++;
        StartCoroutine(SetupActAfterDelay(delay));
    }
}

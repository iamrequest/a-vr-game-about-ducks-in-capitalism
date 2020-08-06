using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class DoorController : MonoBehaviour {
    private Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    // -- Open door
    public void OpenDoor() {
        animator.SetBool("isDoorOpen", true);
    }
    public void OpenDoorAfterDelay(float delay) {
        StartCoroutine(DoOpenDoorAfterDelay(delay));
    }
    private IEnumerator DoOpenDoorAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isDoorOpen", true);
    }


    // -- Close door
    public void CloseDoorAfterDelay(float delay) {
        StartCoroutine(DoCloseDoorAfterDelay(delay));
    }
    private IEnumerator DoCloseDoorAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        animator.SetBool("isDoorOpen", false);
    }
}

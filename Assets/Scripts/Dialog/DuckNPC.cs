using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DuckNPC : MonoBehaviour {
    public Animator animator;

    public bool isTalking {
        set {
            animator.SetBool("isTalking", value);
        }
    }

    [Header("Look At")]
    // lookatTransform: The NPC's head looks at this transform.
    // lookatTarget: lookatTransform lerps towards this position
    public Transform lookatTransform;
    public Transform m_lookatTarget;
    public Transform lookatTarget {
        set {
            // value of null represents LookatTarget.NoChange
            if (value != null) {
                m_lookatTarget = value;
            }
        }
    }
    public float headTurnSpeed;

    private void Start() { }

    private void Update() {
        // Move the head's lookat position towards the current target
        if (m_lookatTarget != null) {
            lookatTransform.position = Vector3.MoveTowards(lookatTransform.position, m_lookatTarget.position, headTurnSpeed * Time.deltaTime);
        }

        if (Input.GetKeyUp(KeyCode.N)) {
            lookatTarget = SpeakerManager.instance.lookatBonsaiTransform;
        }
        if (Input.GetKeyUp(KeyCode.M)) {
            lookatTarget = SpeakerManager.instance.lookatMugTransform;
        }
    }

    public void Enter() {
        animator.SetTrigger("enter");
    }
    public void Exit() {
        animator.SetTrigger("exit");
    }
    public void SetMugVisibility(bool isHoldingMug) {
        animator.SetBool("hasCoffee", isHoldingMug);
    }
    public void SipCoffee() {
        animator.SetTrigger("takeSipOfCoffee");
    }
}

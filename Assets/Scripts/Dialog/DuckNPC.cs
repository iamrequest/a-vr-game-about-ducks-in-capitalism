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
    public bool isShouting {
        set {
            animator.SetBool("isShouting", value);
        }
    }

    [Header("Look At")]
    // lookatTransform: The NPC's head looks at this transform.
    // lookatTarget: lookatTransform lerps towards this position
    public Transform lookatTransform;
    private Transform m_lookatTarget;
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
    }

    public void SetAnimation(NPCAnimation animation) {
        switch (animation) {
            case NPCAnimation.SetCoffee: 
                SetIsDrawing(false);
                SetMugVisibility(true);
                break;
            case NPCAnimation.UnsetCoffee: 
                SetIsDrawing(false);
                SetMugVisibility(false);
                break;
            case NPCAnimation.SipCoffee: 
                SipCoffee();
                break;
            case NPCAnimation.StartDrawing: 
                SetMugVisibility(false);
                SetIsDrawing(true);
                break;
            case NPCAnimation.StopDrawing: 
                SetMugVisibility(false);
                SetIsDrawing(false);
                break;
            case NPCAnimation.Shout:
                isTalking = false;
                isShouting = true;
                break;

            default:
                isShouting = false;
                isTalking = true;
                break;
        }
    }

    public void Enter() {
        lookatTarget = null;
        animator.SetTrigger("enter");
    }
    public void Exit() {
        lookatTarget = null;
        animator.SetTrigger("exit");
    }
    public void SetMugVisibility(bool isHoldingMug) {
        animator.SetBool("hasCoffee", isHoldingMug);
    }
    public void SetIsDrawing(bool isDrawing) {
        animator.SetBool("isDrawing", isDrawing);
    }
    public void SipCoffee() {
        lookatTarget = null;
        animator.SetTrigger("takeSipOfCoffee");
    }
}

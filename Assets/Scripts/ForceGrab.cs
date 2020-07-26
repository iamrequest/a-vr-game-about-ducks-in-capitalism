using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Animator))]
public class ForceGrab : MonoBehaviour {
    // Used for displaying a hint of where the player is force grabbing
    private LineRenderer lineRenderer;
    private Animator animator;

    // SteamVR components
    private Hand sourceHand;
    private SteamVR_Input_Sources controller;
    public SteamVR_Action_Boolean doForceGrab;

    public float forceGrabLength;
    [Tooltip("The set of layers that raycast will collide with")]
    public LayerMask layerMask;
    public float lerpDuration;
    private bool isForceGrabActive;

    // Start is called before the first frame update
    void Start() {
        // -- Configure steamvr hand
        sourceHand = GetComponentInParent<Hand>();
        if (sourceHand == null) {
            Debug.LogError("Unable to find Hand component in parent gameobject!");
        }
        controller = sourceHand.handType;
        doForceGrab.AddOnStateDownListener(DoForceGrab, controller);
        doForceGrab.AddOnStateUpListener(UpdateIsForceGrabActive, controller);

        // Fetch the rest of the necessary components
        lineRenderer = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();
    }

    private void UpdateIsForceGrabActive(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        isForceGrabActive = false;
    }

    // Update is called once per frame
    void Update() { }

    public void DoForceGrab(SteamVR_Action_Boolean triggeringAction, SteamVR_Input_Sources controller) {
        if(sourceHand.currentAttachedObject == null) {
            // TODO: Animate
            lineRenderer.SetPosition(0, sourceHand.transform.position);
            lineRenderer.SetPosition(1, sourceHand.transform.position + sourceHand.transform.forward * forceGrabLength);

            RaycastHit raycastHit;

            // See if we collide with a grabbable object (ie: on layer forceGrabLayer) 
            if(Physics.Raycast(sourceHand.transform.position, 
                               sourceHand.transform.forward, out raycastHit, forceGrabLength, layerMask)) {
                if(!sourceHand.currentAttachedObjectInfo.HasValue) {
                    lineRenderer.SetPosition(1, raycastHit.point);
                    animator.SetTrigger("pulseLineRenderer");

                    Throwable target = raycastHit.collider.gameObject.GetComponentInParent<Throwable>();
                    if(target != null) {
                        isForceGrabActive = true;
                        StartCoroutine(DoLerp(target));
                    }
                }
            }
        }
    }

    private IEnumerator DoLerp(Throwable target) {
        float currentLerpTime = 0f;

        // Release from the hand, if the player is holding onto the object
        //if (hand != null) {
        //    hand.DetachObject(gameObject);
        //}

        Vector3 originalPosition = target.transform.position;
        Quaternion originalRotation = transform.rotation;

        while (currentLerpTime < lerpDuration) {
            // Quit early if the player lets go
            if (!isForceGrabActive) {
                yield break;
            }

            // Quit early if the other hand is holding the object
            if (sourceHand.otherHand.currentAttachedObject == target.gameObject) {
                yield break;
            }

            // Still too far from the target - lerp towards it
            target.transform.position = Vector3.Lerp(originalPosition, sourceHand.transform.position, currentLerpTime / lerpDuration);
            target.transform.rotation = originalRotation;
            //transform.rotation = Quaternion.Lerp(originalRotation, sourceHand.transform.rotation, currentLerpTime / lerpDuration);

            currentLerpTime += Time.deltaTime;
            yield return null;
        }

        isForceGrabActive = false;
        target.transform.rotation = originalRotation;
        sourceHand.AttachObject(target.gameObject, GrabTypes.Grip, target.attachmentFlags);
        yield break;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using UnityEngine.Events;

// Post-add configuration (should probably be auto-done in Reset()):
//  Interactable.onPickUp(): this.ReleaseFromSlot()
//  Interactable.OnDetachFromHand(): rb.isKinematic = false
[RequireComponent(typeof(Rigidbody))]
public class Slottable : MonoBehaviour {
    private Rigidbody rb;
    private Interactable interactable;

    public GameObject slot;
    public UnityEvent onSlotEvent;
    public bool slottedOnStart, isSlotted, followSlotWhenSlotted;
    private bool isLerping;
    public float lerpDuration = 1f;
    [Tooltip("The delay between a grab, and being able to slot this object again")]
    public float postGrabSnapDelay;
    private float timeSinceLastGrab;

    [Header("Release")]
    public bool releaseWhenInverted;
    [Tooltip("How far the container has to tilt to pour its contents out. 0 represents completely upside-down")]
    [Range(0f, 1f)]
    public float minReleasePercentage;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();
        
        if (slottedOnStart) {
            slot.SetActive(false);
            isSlotted = true;

            transform.position = slot.transform.position;
            transform.rotation = slot.transform.rotation;
            CompleteSlotting();
        }
    }
    private void Update() {
        if (timeSinceLastGrab < 999) {
            timeSinceLastGrab += Time.deltaTime;
        }

        // Follow the slot's transform, if the option's enabled. Only do so if we're slotted, and not currently lerping
        //  Likely would be better to do this via rb.MovePosition() and MoveRotation(), and in FixedUpdate(), but there's some jankiness by doing it that way
        //  - rb still has gravity, so this gameobject falls through the floor for some reason
        //  - rb MovePosition() jitters, since it's still doing collisions
        //
        //  Doing it this way is bad because rb calculations are still being made in FixedUpdate(). Good old gamejam jank~
        if (isSlotted && !isLerping && followSlotWhenSlotted) {
            transform.position = slot.transform.position;
            transform.rotation = slot.transform.rotation;
        }
    }

    private void FixedUpdate() {
        if (isSlotted && !isLerping) {
            // Attempt to release the slot
            if (releaseWhenInverted) {
                // Ranges from [-1, 1], where 1 represents fully up
                float upSimilarity = Vector3.Dot(transform.up, Vector3.up);

                // Next, we'll remap it to [0, 1], where 0 means completely downwards
                float upPercentage = (upSimilarity * .5f) + 0.5f;
                if (upPercentage < minReleasePercentage) {
                    rb.isKinematic = false;
                    ReleaseFromSlot();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // TODO: Time duration
        // Only process a slot if it's been long enough since we last picked up the object
        if (timeSinceLastGrab > postGrabSnapDelay && !isSlotted) {
            if(other.gameObject == slot) {
                LerpToSlot();
            }
        }
    }

    /// <summary>
    /// Begins lerp to slot
    /// </summary>
    public void LerpToSlot() {
        isSlotted = true;
        isLerping = true;
        rb.isKinematic = true;

        StartCoroutine(DoSlotLerp());
    }

    /// <summary>
    /// Snaps to slot immediately
    /// </summary>
    public void SnapToSlot() {
        transform.position = slot.transform.position;
        transform.rotation = slot.transform.rotation;
        CompleteSlotting();
    }

    // Hook this up to the interactable OnPickUp() event
    public void ReleaseFromSlot() {
        // If we're releasing the object from its slot (ie: first grab after being slotted)
        if (isSlotted) {
            timeSinceLastGrab = 0f;
        }

        isSlotted = false;
        slot.SetActive(true);
    }

    private IEnumerator DoSlotLerp() {
        slot.SetActive(false);
        float currentLerpTime = 0f;

        // Release from the hand, if the player is holding onto the object
        Hand hand = interactable.attachedToHand;
        if (hand != null) {
            hand.DetachObject(gameObject);
        }

        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;

        while (currentLerpTime < lerpDuration) {
            // Still too far from the target - lerp towards it
            transform.position = Vector3.Lerp(originalPosition, slot.transform.position, currentLerpTime / lerpDuration);
            transform.rotation = Quaternion.Lerp(originalRotation, slot.transform.rotation, currentLerpTime / lerpDuration);

            currentLerpTime += Time.deltaTime;
            yield return null;
        }

        CompleteSlotting();

        isLerping = false;
        yield break;
    }

    private void CompleteSlotting() {
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        onSlotEvent.Invoke();
    }
}

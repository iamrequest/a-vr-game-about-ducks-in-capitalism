using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

// TODO: Slot doesn't become active again until a grab. 
//      User can push this gameobject around via physics to remove the object from the slot (ok), but they can't re-slot until they pick up the object (not ok)
[RequireComponent(typeof(Rigidbody))]
public class Slottable : MonoBehaviour {
    private Rigidbody rb;
    private Interactable interactable;
    public GameObject slot;
    public bool isSlotted;
    public float lerpDuration = 1f;
    [Tooltip("The delay between a grab, and being able to slot this object again")]
    public float postGrabSnapDelay;
    private float timeSinceLastGrab;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<Interactable>();
    }
    private void Update() {
        if (timeSinceLastGrab < 999) {
            timeSinceLastGrab += Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other) {
        // TODO: Time duration
        // Only process a slot if it's been long enough since we last picked up the object
        if (timeSinceLastGrab > postGrabSnapDelay && !isSlotted) {
            if(other.gameObject == slot) {
                isSlotted = true;
                rb.isKinematic = true;

                StartCoroutine(DoSlotLerp());
            }
        }
    }

    // Hook this up to the interactable OnPickUp() event
    public void OnGrab() {
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

        // Reset RB
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        yield break;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On Trigger Enter with a killzone, this gameobject will snap back to its original position/rotation
public class KillzoneListener : MonoBehaviour {
    private const string KILLZONE_LAYER = "Killzone";
    private Rigidbody rb;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start() {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer(KILLZONE_LAYER)) {
            transform.position = originalPosition;
            transform.rotation = originalRotation;

            // If this gameobject has a rigidbody at its root, then reset its positional/angular velocity
            if (rb != null) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}

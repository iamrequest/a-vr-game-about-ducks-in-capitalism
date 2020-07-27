using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Some object that can holds sugar or coffee grounds
//  This would be a spoon, and a coffee filter.
// 
public enum ScoopableContents {
    NONE, COFFEE_GROUNDS_DRY, COFFEE_GROUNDS_WET, SUGAR
}
public class ScoopableContainer : MonoBehaviour {
    private Color sugarColor = new Color(190,190,190);
    private Color dryCoffeeGroundsColor = new Color(178,124,70);
    private Color wetCoffeeGroundsColor = new Color(137,87,5);

    private ScoopableContents m_contents;
    public ScoopableContents contents {
        get {
            return m_contents;
        }
        set {
            // Configure the material
            if (contentRenderer != null) {
                switch (value) {
                    case ScoopableContents.COFFEE_GROUNDS_DRY: 
                        contentRenderer.gameObject.SetActive(true);
                        contentRenderer.material.color = dryCoffeeGroundsColor;
                        break;
                    case ScoopableContents.COFFEE_GROUNDS_WET: 
                        contentRenderer.gameObject.SetActive(true);
                        contentRenderer.material.color = wetCoffeeGroundsColor;
                        break;
                    case ScoopableContents.SUGAR: 
                        contentRenderer.gameObject.SetActive(true);
                        contentRenderer.material.color = sugarColor;
                        break;
                    case ScoopableContents.NONE: 
                        contentRenderer.gameObject.SetActive(false);
                        break;
                    default:
                        Debug.LogError("Unexpected Spoon Content recieved");
                        break;
                }
            }

            m_contents = value;
        }
    }
    public ScoopableContents initialContent;

    public Renderer contentRenderer;
    [Range(0f, 1f)]
    [Tooltip("How far the container has to tilt to pour its contents out. 0 represents completely upside-down")]
    public float minReleasePercentage;

    [Header("Transfer of contents")]
    public int capacity;
    public bool canReceive, canGive, canPour, infiniteCapacity;
    public float maxPourDistance;
    public LayerMask pourLayerMask;

    private void Start() {
        contents = initialContent;
    }

    // -- Destroy the contents if flipped up-side down
    private void FixedUpdate() {
        // Pour logic
        if (canPour) {
            AttemptPour();
        }
    }

    private void OnTriggerEnter(Collider other) {
        // -- Attempt to give
        if (canGive) {
            ScoopableContainer container = GetScoopableContainer(other.gameObject);

            if (container != null) {
                ProcessGive(container);
            }
        }
    }

    // Check the target first, then check the parent
    private ScoopableContainer GetScoopableContainer(GameObject target) {
        if (target.TryGetComponent(out ScoopableContainer container)) {
            return container;
        }

        return target.GetComponentInParent<ScoopableContainer>();
    }

    private void AttemptPour() {
        if (contents == ScoopableContents.NONE) return;

        // -- Detect if the container is up-side down enough
        // Ranges from [-1, 1], where 1 represents fully up
        float upSimilarity = Vector3.Dot(transform.up, Vector3.up);

        // Next, we'll remap it to [0, 1], where 0 means completely downwards
        float upPercentage = (upSimilarity * .5f) + 0.5f;
        if (upPercentage < minReleasePercentage) {

            // Raycast down, and try to find a ScoopableContainer to give our contents to 
            // TODO: Make sure we don't collide with ourselves
            if (Physics.Raycast(contentRenderer.transform.position, Vector3.down, out RaycastHit rayHit, maxPourDistance, pourLayerMask)) {
                ScoopableContainer container = GetScoopableContainer(rayHit.collider.gameObject);

                if (container != null) {
                    ProcessGive(container);
                }
            }

            // Regardless if there was something below, we we should get rid of the contents
            if (!infiniteCapacity) {
                capacity = 0;
                contents = ScoopableContents.NONE;
            }
        }
    }

    private void ProcessGive(ScoopableContainer other) {
        if (other == this) return;
        if (!other.canReceive) return;

        // We must either have the same contents, or the same
        if (other.contents == ScoopableContents.NONE || contents == other.contents) {
            other.contents = contents;
            other.capacity ++;
        }

        // Get rid of whatever's in this container, if applicable
        //  eg: The sugar jar may have infinite capacity, but the spoon can only hold one 
        if (!infiniteCapacity) {
            contents = ScoopableContents.NONE;
        } else {
            capacity = 0;
        }
    }
}

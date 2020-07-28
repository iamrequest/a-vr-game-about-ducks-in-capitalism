using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoffeeContainer))]
public class CoffeePourable : MonoBehaviour {
    private CoffeeContainer coffeeContainer;

    [Header("Pour")]
    public ParticleSystem coffeePourVFX;
    [Range(0f, 1f)]
    public float minPour, currentPour;
    public float maxPourDistance;
    public LayerMask pourLayerMask;

    [Header("Tilt management")]
    // This represents how much the pot is upside-down
    [Range(0f, 1f)]
    public float tiltPercentage;
    public float capacityLossMultiplier;
    public bool infiniteCapacity;

    [Range(0f, 1f)]
    [Tooltip("How far the container has to tilt to enter drain mode")]
    public float tiltPercentageToDrain;
    [Range(0f, 1f)]
    [Tooltip("How fast the container loses capacity in drain mode")]
    public float drainSpeed;

    // Start is called before the first frame update
    void Start() {
        coffeeContainer = GetComponent<CoffeeContainer>();
    }

    // Update is called once per frame
    void Update() {
        // -- Calculate the pour percentage
        // The amount of coffee that pours is proportional to:
        //  1. The amount of coffee in the pot
        //  2. How far the player is tilting the pot upside-down 
        tiltPercentage = GetTiltPercentage();

        // Calculate how much we're tilting the container
        if (tiltPercentage > tiltPercentageToDrain) {
            currentPour = drainSpeed;
        } else {
            currentPour = coffeeContainer.currentCoffeeLevel * tiltPercentage;
        }

        // If we're tilting the container enough to trigger a pour...
        if (currentPour > minPour && coffeeContainer.capacity > 0) {
            // If the animation isn't already playing, then do so
            if (!coffeePourVFX.isPlaying) {
                coffeePourVFX.Play();
            }

            // Calculate how much fluid is being lost, and remove it from the source container
            float capacityLoss = CalculateFluidLoss();
            if (!infiniteCapacity) {
                coffeeContainer.AddCoffee(- capacityLoss);
            }

            // Raycast straight down from the VFX spawn position, to find a container
            if (Physics.Raycast(coffeePourVFX.transform.position, Vector3.down, out RaycastHit rayHit, maxPourDistance, pourLayerMask)) {
                CoffeeContainer otherContainer = rayHit.collider.GetComponentInParent<CoffeeContainer>();
                if(otherContainer != null) {
                    // If we found a otherContainer (that isn't the source otherContainer), then transfer contents to it
                    if (otherContainer != coffeeContainer) {
                        // Calculate the new percentage of cream going to this otherContainer
                        otherContainer.UpdateCreamPercentage(coffeeContainer.creamPercentage, capacityLoss);
                        otherContainer.AddCoffee(capacityLoss);
                    }
                }
            }
        } else {
            coffeePourVFX.Stop();

            // 0 capacity, and we're pouring. Clear the sugar amount
            // We usually do this in coffeeContainer.AddCoffee(), but since the capacity is 0, we need to explicitly check it again
            //  Ex: Mug has 0 fluids, and 3 sugar. Inverting the mug needs to yeet those 3 sugars.
            coffeeContainer.ClearSugar();
        }
    }

    private float GetTiltPercentage() {
        // This represents how much the coffee pot is vertically oriented, on a scale from [-1, 1],
        //  where 1 means completely up
        float upSimilarity = Vector3.Dot(transform.up, Vector3.up);

        // Next, we'll remap it to [0, 1], where 1 means completely downwards
        return (upSimilarity * -.5f) + 0.5f;
    }

    // Calculate how much of this container is being poured this frame
    //  This will range from [0f, 1f].
    private float CalculateFluidLoss() {
        // -- Calculate how much is being poured
        // How much more are we pouring than the minimum amount?
        //  If we're tilting very far, we'll displace much more coffee
        float pourAmount = currentPour - minPour;

        // TODO: This is still too fast, even with a very small multiplier
        float capacityLoss = capacityLossMultiplier *
            coffeeContainer.capacity *
            pourAmount *
            Time.deltaTime;

        // Small boost to get over multipliers approaching 0
        capacityLoss += 1f;
        return Mathf.Clamp(capacityLoss, 0f, capacityLoss);
    }
}

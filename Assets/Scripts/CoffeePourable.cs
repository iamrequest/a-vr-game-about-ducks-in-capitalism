using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CoffeeContainer))]
public class CoffeePourable : MonoBehaviour {
    private CoffeeContainer coffeeContainer;
    public ParticleSystem coffeePourVFX;
    [Range(0f, 1f)]
    public float minPour, currentPour;

    // This represents how much the pot is upside-down
    [Range(0f, 1f)]
    public float tiltPercentage;
    public float capacityLossMultiplier;

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

        if (tiltPercentage > tiltPercentageToDrain) {
            currentPour = drainSpeed;
        } else {
            currentPour = coffeeContainer.currentCoffeeLevel * tiltPercentage;
        }

        if (currentPour > minPour && coffeeContainer.capacity > 0) {
            if (!coffeePourVFX.isPlaying) {
                coffeePourVFX.Play();
            }

            float capacityLoss = CalculateFluidLoss();
            coffeeContainer.SubtractCoffee(capacityLoss);

            // TODO: Raycast down to container below, fill by pourDelta
        } else {
            coffeePourVFX.Stop();
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

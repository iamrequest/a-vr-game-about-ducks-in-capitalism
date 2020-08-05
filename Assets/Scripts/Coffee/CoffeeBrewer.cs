using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR.InteractionSystem;

// TODO: How to avoid reuse coffee filter
// TODO: How to reset the CircularDrive's rotation after brewing? Changing the rotation manually works, but it snaps right back on grab. Same results even if I set LinearMapping.value
public class CoffeeBrewer : MonoBehaviour {
    public TextMeshProUGUI text;
    public ParticleSystem pourVFX;

    // -- Gameobject references used for brewing coffee
    // Coffee pot
    public Slottable coffeePotSlottable;
    private CoffeeContainer coffeeContainer;

    // Filter tray
    public Slottable coffeeFilterTray;

    // Filter 
    //public Slottable coffeeFilterSlottable;
    public ScoopableContainer coffeeFilterContents;

    public bool isBrewing;
    public bool isBrewingComplete;
    public float brewTime;
    private bool isBrewingPossible;
    private float elapsedBrewTime;

    // Start is called before the first frame update
    void Start() {
        coffeeContainer = coffeePotSlottable.GetComponent<CoffeeContainer>();
        //coffeeFilterContents = coffeeFilterSlottable.GetComponent<ScoopableContainer>();
    }

    // Update is called once per frame
    void Update() {
        // This could be done on slot changes or something for efficiency, but this is faster to throw together
        UpdateUI();
    }

    private string GetStatus() {
        isBrewingPossible = false;
        if (isBrewing) {
            float brewPercentage = elapsedBrewTime / brewTime;
            return "Brewing: " + (brewPercentage * 100 ).ToString("F0") + "%";
        }
        if (isBrewingComplete) {
            return "Brewing complete";
        }

        // -- Validation on the coffee pot
        if (!coffeePotSlottable.isSlotted) {
            return "Please insert coffee pot";
        }
        if (coffeeContainer.capacity > 0) {
            return "Error: Coffee pot is not empty";
        }

        // -- Validation on the coffee filter tray
        if (!coffeeFilterTray.isSlotted) {
            return "Please insert coffee filter tray";
        }

        // -- Validation on the coffee filter
        //if (!coffeeFilterSlottable.isSlotted) {
        //    return "Please insert coffee filter";
        //}
        if (coffeeFilterContents.capacity < 1) {
            return "Error: Missing coffee grounds";
        }
        if (coffeeFilterContents.contents == ScoopableContents.COFFEE_GROUNDS_WET) {
            return "Error: Please dispose of old coffee grounds";
        }

        isBrewingPossible = true;
        return "Ready to brew. Pull lever to start.";
    }

    void UpdateUI() {
        text.text = GetStatus();
    }


    public void StartBrew() {
        if (isBrewing) {
            return;
        }

        // Make sure GetStatus() is called before this, so we can update isBrewingPossible
        if (isBrewingPossible) {
            StartCoroutine(DoBrew());
        }
    }

    private IEnumerator DoBrew() {
        isBrewing = true;
        isBrewingComplete = false;

        elapsedBrewTime = 0f;
        pourVFX.Play();

        // Pour VFX
        while (elapsedBrewTime < brewTime) {
            elapsedBrewTime += Time.deltaTime;

            // Validate that none of the slottables were removed
            // We can remove the pot early
            //if (!coffeeFilterTray.isSlotted || !coffeeFilterSlottable.isSlotted) {
            if (!coffeeFilterTray.isSlotted) {
                ResetBrewStatus();
                yield break;
            }

            // If the pot is still slotted, then add fluids to it, relative to how much time has passed
            if (coffeePotSlottable.isSlotted) {
                float capacityPercentage = Time.deltaTime / brewTime;
                coffeeContainer.AddCoffee(capacityPercentage * coffeeContainer.maxCapacity);
            }

            yield return null;
        }

        ResetBrewStatus();
        yield break;
    }

    private void ResetBrewStatus() {
        isBrewing = false;
        isBrewingComplete = true;
        coffeeFilterContents.contents = ScoopableContents.COFFEE_GROUNDS_WET;
        pourVFX.Stop();
    }

    // Hook this up to OnGrab() event
    public void ResetIsBrewingComplete() {
        isBrewingComplete = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: Button press to start
// TODO: Add pourable?
public class CoffeeBrewer : MonoBehaviour {
    public TextMeshProUGUI text;

    // -- Gameobject references used for brewing coffee
    // Coffee pot
    public Slottable coffeePotSlottable;
    private CoffeeContainer coffeeContainer;

    // Filter tray
    public Slottable coffeeFilterTray;

    // Filter 
    public Slottable coffeeFilterSlottable;
    private ScoopableContainer coffeeFilterContents;

    public bool isBrewing;
    public bool isBrewingComplete;
    public float brewTime;
    private float elapsedBrewTime;

    // Start is called before the first frame update
    void Start() {
        coffeeContainer = coffeePotSlottable.GetComponent<CoffeeContainer>();
        coffeeFilterContents = coffeeFilterSlottable.GetComponent<ScoopableContainer>();
    }

    // Update is called once per frame
    void Update() {
        UpdateUI();
    }

    private string GetStatus() {
        if (isBrewing) {
            float brewPercentage = elapsedBrewTime / brewTime;
            return "Brewing: " + (brewPercentage * 100 ).ToString("0F") + "%";
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
        if (!coffeeFilterSlottable.isSlotted) {
            return "Please insert coffee filter";
        }
        if (coffeeFilterContents.capacity < 1) {
            return "Error: Missing coffee grounds";
        }

        return "Ready to brew";
    }

    void UpdateUI() {
        text.text = GetStatus();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MugUI : MonoBehaviour {
    public TextMeshProUGUI text;
    private CoffeeContainer coffeeContainer;
    private ScoopableContainer sugarContainer;

    // TODO:
    //public bool onlyVisibleFromFront;

    // Start is called before the first frame update
    void Start() {
        coffeeContainer = GetComponentInParent<CoffeeContainer>();
        sugarContainer = GetComponentInParent<ScoopableContainer>();
    }

    // Update is called once per frame
    void Update() {
        UpdateGUI();
    }

    private void UpdateGUI() {
        text.text = "Capacity: " + FormatPercentage(coffeeContainer.currentCoffeeLevel)
            + "%\nCream: " + FormatPercentage(coffeeContainer.creamPercentage)
            + "%\nSugar: " + sugarContainer.capacity;
    }

    private string FormatPercentage(float value) {
        return (value * 100).ToString("F0");
    }
}

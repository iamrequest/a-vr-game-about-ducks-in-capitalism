﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

/* Coffee Container
 * 
 * This script handles steam output, and the fill percentage of a coffee container
 * Assumptions:
 *  1. On Start(), the mesh representing the coffee is scaled vertically such that it's at 100% fill capacity
 *  2. The coffee mesh's pivot point is at the bottom of the mesh. By doing so, we can just scale the mesh's z value to reduce fill capacity
 */
public class CoffeeContainer : MonoBehaviour {
    [Header("Steam")]
    public VisualEffect steamVFX;
    private int steamVFXId_spawnRateId;
    public float minSteamOutput, maxSteamOutput;
    [Range(0f, 1f)]
    public float currentSteamOutput;

    [Header("Coffee Level")]
    [Tooltip("Please make sure that the coffee is scaled to 100% when creating your gameobject")]
    public Transform coffeeTransform;
    public float minCoffeeLevel;

    [Tooltip("Capacity of this container, in ML")]
    public float capacity, maxCapacity;
    [Range(0f, 1f)]
    public float currentCoffeeLevel;

    private float maxCoffeeLevel;
    private Vector3 coffeeScale;

    private void Start() {
        steamVFXId_spawnRateId = Shader.PropertyToID("spawnRate");
        coffeeScale = coffeeTransform.localScale;
        maxCoffeeLevel = coffeeTransform.localScale.z;
    }

    private void Update() {
        // Output steam
        float steamSpawnRate = Mathf.Lerp(minSteamOutput, maxSteamOutput, currentSteamOutput);
        steamVFX.SetFloat(steamVFXId_spawnRateId, steamSpawnRate);
    }

    public void SubtractCoffee(float capacitySubtraction) {
        // Subtract coffee
        capacity -= capacitySubtraction;
        capacity = Mathf.Clamp(capacity, 0f, maxCapacity);
        currentCoffeeLevel = capacity / maxCapacity;

        UpdateCoffeeMesh();
    }

    private void UpdateCoffeeMesh() {
        // Hide coffee mesh if at 0
        if (capacity == 0) {
            coffeeTransform.gameObject.SetActive(false);
        } else {
            coffeeTransform.gameObject.SetActive(true);
        }

        // Scale coffee liquids
        coffeeScale.z = Mathf.Lerp(minCoffeeLevel, maxCoffeeLevel, currentCoffeeLevel);
        coffeeTransform.localScale = coffeeScale;
    }
}


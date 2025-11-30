using UnityEngine;
using System;
using System.Collections.Generic;

public enum ItemType {
    manaFruit,
    healthFruit,
    staminaFruit
}

public enum Currency {
    Gold = 0
}

[System.Serializable]
public class ItemDefinition {
    [Header("General")]
    public string name;

    [Header("Rendering")]
    // prefab for displaying item in scene (e.g. shop, loot drops)
    public GameObject itemModel;
    // prefab for displaying item in ui (e.g. hotbar)
    public GameObject itemModelUI;

    [Header("Loot")]
    [Range(0f,1f)]
    public float dropProbability;

    [Header("Shop")]
    [Range(0f,1f)]
    public float shopProbability;
    public int cost;
    public int maxPerShopSlot = 1;

    [Header("Inventory")]
    public float weight;            // owned    items should slow down the player
    public int maxPerInventorySlot = 1;
}

public class ItemDefinitions : MonoBehaviour
{
    // can be accessed as a singleton
    public static ItemDefinitions main = null;

    public ItemDefinitions() {
        main = this;
    }

    public ItemDefinition this[int i] {
        get => definitions[i];
    }

    // 
    public ItemDefinition[] definitions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // normalize probabilities
        var probabilitySum = 0.0f;
        foreach(ItemDefinition def in definitions)
            probabilitySum += def.shopProbability;

        foreach(ItemDefinition def in definitions)
            def.shopProbability /= probabilitySum;

        // normalize probabilities
        probabilitySum = 0.0f;
        foreach(ItemDefinition def in definitions)
            probabilitySum += def.dropProbability;

        foreach(ItemDefinition def in definitions)
            def.dropProbability /= probabilitySum;
    }
}

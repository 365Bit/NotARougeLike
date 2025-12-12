using UnityEngine;
using System;

public class RunDataComponent : MonoBehaviour
{
    public static RunData data;

    public RunDataComponent() {
        if (data == null) {
            data = RunData.Instance;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameSaver.subscribe(this);
    }
}


// class to store run data, allows defining persistence
[System.Serializable]
public class RunData {
    public int level = 0;
    public CurrencyContainer currencies {get; private set;} = null;
    public ItemContainer items {get; private set;} = null;
    public PlayerUpgradeState upgrades {get; private set;} = null;

    // 
    public void InitIfRequired() {
        GameObject defs = GameObject.Find("Definitions");

        if (upgrades == null) {
            upgrades = new();
        }
    
        if (currencies == null) {
            currencies = new();
            currencies[Currency.Gold] += 5;
        }
    
        if (items == null) {
            items = new(defs.GetComponent<Constants>().itemSlots);
            items.AddItem(defs.GetComponent<ItemDefinitions>()[3], 200);
        }
    }

    // 
    public void LevelDone() {
        Debug.Log("Level " + level + " done");
        level += 1;
        currencies[Currency.XP] += 100;
        currencies[Currency.Gold] += 5;

        // for  testing
        foreach (BaseStatKey key in Enum.GetValues(typeof(BaseStatKey))) {
            upgrades.Upgrade(key);
            upgrades.Upgrade(key);
        }
    }

    // reset rundata and keep persistent stuff
    public void ResetNonPersistent() {
        // back to first level
        level = 0;

        // transfer xp to new currency container
        CurrencyContainer newcurrencies = new();
        newcurrencies[Currency.XP] = currencies[Currency.XP];
        currencies = newcurrencies;

        // create new item container
        ItemContainer newitems = new(items.slots.Length);
        items = newitems;

        // player upgrades are kept
    }
    
    public static RunData Instance = new();
}






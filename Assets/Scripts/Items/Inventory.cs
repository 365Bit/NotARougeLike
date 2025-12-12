using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    public int numHotbarSlots;
    public int numItemSlots;
    public ItemContainer container { get; private set;}

    // amount of currencies
    public int numCurrencySlots = Enum.GetValues(typeof(Currency)).Length;
    public CurrencyContainer currency { get; private set; }

    public void Start() {
        RunData.Instance.InitializeItems(numItemSlots);
        RunData.Instance.InitializeCurrencies();

        container = RunData.Instance.items;
        currency = RunData.Instance.currencies;
    }
}

[System.Serializable]
public class ItemSlot {
    public ItemDefinition storedItem = null;
    public int count = 0;
}


[System.Serializable]
public class CurrencyContainer {
    public int[] balances {get; private set;}

    public CurrencyContainer() {
        balances = new int[Enum.GetValues(typeof(Currency)).Length];
        for (int i = 0; i < balances.Length; i++) 
        {
            balances[i] = 0;
        }
    }

    public ref int this[Currency c] {
        get => ref balances[(int)c];
    }
}

[System.Serializable]
public class ItemContainer {
    public ItemSlot[] slots {get; private set;}

    public ItemContainer(int count) {
        slots = new ItemSlot[count];
        for (int i = 0; i < count; i++) 
        {
            slots[i] = new();
        }
    }

    public ItemSlot this[int slot] {
        get => slots[slot];
    }


    private void PrintState() {
        StringBuilder output = new();
        for (int i = 0; i < slots.Length; i++) {
            output.AppendFormat("{0} : {1}({2}x), ", i, (slots[i].storedItem != null) ? slots[i].storedItem.name : "none", slots[i].count);
        }
        Debug.Log("inventory: " + output.ToString());
    }

    public void SwapItems(int first, int second) {
        if (first == second) return;

        var tmp = slots[first];
        slots[first] = slots[second];
        slots[second] = tmp;
    }

    public void ConsumeItem(int slot) {
        if (slots[slot].count > 0)
        {
            slots[slot].count -= 1;
            if(slots[slot].count == 0)
                slots[slot].storedItem = null;
        }
        PrintState();
    }

    // returns the number of items that have been added to the slot
    public int AddItem(int slot, ItemDefinition item, int count) {
        // there was a different item, overwrite it
        if (slots[slot].storedItem != item)
            slots[slot].count = 0;
        slots[slot].storedItem = item;

        int remainingSpace = item.maxPerInventorySlot - slots[slot].count;
        int actualCount = (remainingSpace < count) ? remainingSpace : count;
        slots[slot].count += actualCount;

        PrintState();
        return actualCount;
    }

    // Selects an appropriate slot to add item, i.e. either a slot containing same item type or a free one
    public void AddItem(ItemDefinition item, int count) {
        for  (int i = 0; i < 10 && count > 0; i++) {
        // while (count > 0) {
            int slot = GetSlotContaining(item, 1);
            if (slot < 0)
                slot = GetFreeSlot();

            if (slot >= 0) {
                count -= AddItem(slot, item, count);
            } else {    // no free slots
                count = 0;
            }
        }
    }

    public int GetSlotContaining(ItemDefinition item, int requiredRemainingCapacity = 0) {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].storedItem == item && slots[i].count <= item.maxPerInventorySlot - requiredRemainingCapacity) {
                return i;
            }
        }
        return -1;
    }

    public int GetFreeSlot() {
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].count == 0)
                return i;
        }
        return -1;
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class InventoryUI : MonoBehaviour
{
    private Player player;
    private Inventory inventory;

    [Header("Rendering")]
    public GameObject slotPrefab;

    public Vector2 slotPositionOrigin;
    public Vector2 slotPositionRight;
    public Vector2 slotPositionBottom;
    
    public int  numColumns;
    private int numHotbarSlots;
    private int numSlots;

    // selection state
    public int currentSlot;
    public bool grabbed;

    // instantiated slots
    private GameObject[] slots = null;

    void InitializeSlots() {
        numSlots = inventory.numItemSlots;
        numHotbarSlots = inventory.numHotbarSlots;
        slots = new GameObject[numSlots];

        for (int i = 0; i < numSlots; i++) {
            slots[i] = Instantiate(slotPrefab, transform);
            // left-to-right
            float right = (float)(i % numColumns) / numColumns;
            Vector2 pos = slotPositionOrigin + slotPositionRight * right;
            // top-to-bottom
            float down = (float)(i / numColumns) / (float)Math.Ceiling((double)numSlots / numColumns);
            pos += slotPositionBottom * down;
            slots[i].transform.localPosition = pos;
        }
    }

    void UpdateSlots() {
        for (int i = 0; i < slots.Length; i++) {
            GameObject slot = slots[i];
            var s = slot.GetComponent<ItemHotbarSlot>();
            s.SetItem(inventory.container[i].storedItem, inventory.container[i].count);
            s.SetSelected(i == currentSlot);
            slot.GetComponent<Button>().onClick.AddListener(delegate { OnClick(i); });
        }
    }

    void OnClick(int slot) {
        Debug.Log("slot " + slot + " has been clicked");
    }

    private void SwitchSlot(bool right) {
        int newSlot = right ? ((currentSlot + 1 + numSlots) % numSlots) : (currentSlot - 1 + numSlots) % numSlots;
        if (grabbed)
            inventory.container.SwapItems(currentSlot, newSlot);
        currentSlot = newSlot;
    }

    public void MoveSelection(Vector2 delta) {
        if (numSlots == 0) return;
        int newSlot = (currentSlot + (int)Math.Round(delta.x) + numSlots) % numSlots;
        newSlot = (newSlot + (int)Math.Round(delta.y) * numColumns + numSlots) % numSlots;
        if (grabbed)
            inventory.container.SwapItems(currentSlot, newSlot);
        currentSlot = newSlot;
    }

    public void ToggleItemGrabbed() {
        grabbed = !grabbed;
    }

    void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
        inventory = GameObject.Find("Player").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if slot count has changed
        numSlots = inventory.numItemSlots;
        if (slots == null || numSlots != slots.Length)
            InitializeSlots();

        UpdateSlots();
    }
}

using UnityEngine;

public class CurrencyDisplay : MonoBehaviour
{
    public Player player;

    [Header("Rendering")]
    public GameObject slotPrefab;

    public Vector2 slotPositionLeft;
    public Vector2 slotPositionRight;

    // 
    private GameObject[] slots;

    void InitializeSlots() {
        var numSlots = player.inventory.numCurrencySlots;
        slots = new GameObject[numSlots];

        for (int i = 0; i < numSlots; i++) {
            slots[i] = Instantiate(slotPrefab, transform);
            slots[i].transform.localPosition = Vector2.Lerp(slotPositionLeft, slotPositionRight, (float)i / numSlots);
        }
    }

    void UpdateSlots() {
        var inv = player.inventory;
        for (int i = 0; i < slots.Length; i++) {
            var slot = slots[i];
            var s = slot.GetComponent<ItemHotbarSlot>();
            s.SetCurrency((Currency)i, inv.currency[(Currency)i]);
            s.SetSelected(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if slot count has changed
        if (slots == null || player.inventory.numCurrencySlots != slots.Length)
            InitializeSlots();

        UpdateSlots();
    }
}

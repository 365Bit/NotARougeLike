using UnityEngine;

public class ItemHotbar : MonoBehaviour
{
    private GameObject player;

    [Header("Rendering")]
    public GameObject slotPrefab;

    public Vector2 slotPositionLeft;
    public Vector2 slotPositionRight;

    // 
    private GameObject[] slots;

    public void Awake() {
        player = GameObject.Find("Player");
    }

    void InitializeSlots() {
        
        int numSlots = player.GetComponent<Inventory>().numItemSlots;
        slots = new GameObject[numSlots];

        for (int i = 0; i < numSlots; i++) {
            slots[i] = Instantiate(slotPrefab, transform);
            slots[i].transform.localPosition = Vector2.Lerp(slotPositionLeft, slotPositionRight, (float)i / numSlots);
        }
    }

    void UpdateSlots() {
        var inv = player.GetComponent<Inventory>();
        for (int i = 0; i < slots.Length; i++) {
            GameObject slot = slots[i];
            var s = slot.GetComponent<ItemHotbarSlot>();
            s.SetItem(inv.container[i].storedItem, inv.container[i].count);
            s.SetSelected(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // check if slot count has changed
        var numSlots = player.GetComponent<Inventory>().numItemSlots;
        if (slots == null || numSlots != slots.Length)
            InitializeSlots();

        UpdateSlots();
    }
}

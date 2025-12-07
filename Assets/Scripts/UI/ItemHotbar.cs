using UnityEngine;

public class ItemHotbar : MonoBehaviour
{
    private GameObject player;
    private Inventory playerInventory;

    [Header("Rendering")]
    public GameObject slotPrefab;

    public Vector2 slotPositionLeft;
    public Vector2 slotPositionRight;

    // 
    private GameObject[] slots;

    public void Awake() {
        player = GameObject.Find("Player");
        playerInventory = player.GetComponent<Inventory>();
    }

    void InitializeSlots() {
        int numSlots = playerInventory.numHotbarSlots;
        slots = new GameObject[numSlots];

        for (int i = 0; i < numSlots; i++) {
            slots[i] = Instantiate(slotPrefab, transform);
            slots[i].transform.localPosition = Vector2.Lerp(slotPositionLeft, slotPositionRight, (float)i / numSlots);
        }
    }

    void UpdateSlots() {
        for (int i = 0; i < slots.Length; i++) {
            GameObject slot = slots[i];
            var s = slot.GetComponent<ItemHotbarSlot>();
            s.SetItem(playerInventory.container[i].storedItem, playerInventory.container[i].count);
            s.SetSelected(false);
        }
    }

    void Start() {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if slot count has changed
        var numSlots = playerInventory.numHotbarSlots;
        if (slots == null || numSlots != slots.Length)
            InitializeSlots();

        UpdateSlots();
    }
}

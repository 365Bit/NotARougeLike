using UnityEngine;

using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    [Header("Rendering")]
    public Vector3 modelPosition;
    public Vector3 modelScale;

    public ItemDefinition item {get; private set;}
    public int count {get; private set;}

    public TMP_Text costText;

    public void Init(ShopRenderer renderer, int slot) {
        Index = slot;
        shop = renderer;
        ItemContainer itemContainer = renderer.Items;
        count = itemContainer[slot].count;
        item = itemContainer[slot].storedItem;

        Transform instance = Instantiate(item.itemModel, transform).transform;
        instance.localPosition = modelPosition;
        instance.localScale = modelScale;

        costText.text = item.cost.ToString();
    }

    public ShopRenderer shop;
    public int Index;
    public int Cost { get => item.cost; }
    public int Count { get => count; }
    public string ItemName { get => item.name; }
}

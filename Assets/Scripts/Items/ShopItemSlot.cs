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

    public void SetItem(ItemDefinition item, int count) {
        this.count = count;
        this.item = item;

        Transform instance = Instantiate(item.itemModel, transform).transform;
        instance.localPosition = modelPosition;
        instance.localScale = modelScale;

        costText.text = item.cost.ToString();
    }

    public void Disable() {
        count--;
        if (count == 0)
            Destroy(gameObject); //suicide this gameobject
    }

    public int Cost { get => item.cost; }
    public int Count { get => count; }
    public string ItemName { get => item.name; }
}

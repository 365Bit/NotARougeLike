using UnityEngine;

using TMPro;

public class ShopItemSlot : MonoBehaviour
{
    private int index = -1;
    private ShopRenderer.Item item;

    public TMP_Text costText;

    public void SetItem(int index, ShopRenderer.Item item) {
        this.index = index;
        this.item = item;

        var instance = Instantiate(item.prefab, transform).transform;
        instance.localScale = item.scale;

        costText.text = item.cost.ToString();
    }

    public void Disable() {
        Destroy(gameObject); //suicide this gameobject
    }

    public int Cost { get => item.cost; }
    public string ItemName { get => item.name; }
}

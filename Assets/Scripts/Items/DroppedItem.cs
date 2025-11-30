using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("Rendering")]
    public Vector3 modelPosition;
    public Vector3 modelScale;

    public ItemDefinition item {get; private set;}
    public int count {get; private set;}

    public void SetItem(ItemDefinition item, int count) {
        this.item = item;
        this.count = count;

        var instance = Instantiate(item.itemModel, transform).transform;
        instance.localPosition = modelPosition;
        instance.localScale = modelScale;
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

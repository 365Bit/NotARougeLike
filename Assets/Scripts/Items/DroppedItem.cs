using UnityEngine;
using System;

public class DroppedItem : MonoBehaviour
{
    [Header("Rendering")]
    public Vector3 modelPosition;
    public Vector3 modelScale;
    public Quaternion modelRotation;

    public ItemDefinition item {get; private set;}
    public int count {get; private set;}

    public string interactionHintFormat;

    public void SetItem(ItemDefinition item, int count) {
        this.item = item;
        this.count = count;

        // put item name into interaction hint
        GetComponent<InteractionHint>().Text = String.Format(interactionHintFormat, item.displayName, count);

        // instantiate prefab as child
        Transform instance = Instantiate(item.itemModel, transform).transform;
        // instance.localPosition = modelPosition;
        // instance.localScale = modelScale;
        // instance.localRotation = modelRotation;

        // compute extent of collider (box collider for ease of use)
        BoxCollider myCollider = GetComponent<BoxCollider>();
        if (instance.TryGetComponent<Collider>(out Collider coll)) {
            Bounds bounds = coll.bounds;
            myCollider.center = transform.InverseTransformPoint(bounds.center);
            myCollider.size = bounds.size;
        } else if (instance.TryGetComponent<MeshFilter>(out MeshFilter mesh)) {
            Bounds bounds = mesh.mesh.bounds;
            myCollider.center = bounds.center;
            myCollider.size = bounds.size;
        }
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

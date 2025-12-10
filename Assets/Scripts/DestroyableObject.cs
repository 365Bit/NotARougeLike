using UnityEngine;

public class DestroyableObject : MonoBehaviour
{
    private float currentHealth;

    [Header("Health")]
    public float maxHealth = 50.0f;

    [Header("Dropped Item")]
    public GameObject droppedItemPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // item to drop
    private ItemDefinition item;
    private int count;

    public void SetItem(ItemDefinition item, int count) {
        this.item = item;
        this.count = count;
    }

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
        
        Vector3 position = transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 0.0f, Random.Range(-0.5f, 0.5f));
        Quaternion rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);

        GameObject instance = Instantiate(droppedItemPrefab, position, rotation);
        instance.GetComponent<DroppedItem>().SetItem(item, count);
        instance.name = item.name;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            SelfDestruct();
        }
    }
}

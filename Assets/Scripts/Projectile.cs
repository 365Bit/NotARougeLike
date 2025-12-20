using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Projectile : MonoBehaviour
{
    // Components
    private Rigidbody rigidBody;
    private BoxCollider boxCollider;

    public GameObject droppedItemPrefab;

    private ItemDefinitions itemDefinitions;
    private ItemDefinition bowAmmo;
    Vector3 travelDirection;
    Rigidbody rb;

    private float damage;

    [Header("Properties")]
    public float speed;
    public float lifetime;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = true; // TODO: Make this projectile-specific

        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = false;

        itemDefinitions = GameObject.Find("Definitions").GetComponent<ItemDefinitions>();
        bowAmmo = itemDefinitions.definitions[3];
        rb = GetComponent<Rigidbody>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);

        // Destroy the projectile after its lifetime expires
        Destroy(this.gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        travelDirection = rb.linearVelocity.normalized;
    }

    private void OnBecameInvisible()
    {
        // Destroy the projectile when it goes off-screen
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string name = collision.gameObject.name;
        Debug.Log("Projectile hit: " + name);

        if (name.Contains("chest"))
        {
            Transform parent = collision.gameObject.transform.parent;
            DestroyableObject destroyableObject = parent.GetComponent<DestroyableObject>();

            if (destroyableObject != null)
            {
                Debug.Log("Projectile dealing " + damage + " damage to " + parent.name);
                destroyableObject.TakeDamage(damage);
            }
        } else if (name.Contains("Opponent"))
        {
            Opponent opponent = collision.gameObject.GetComponent<Opponent>();
            Debug.Log("Projectile dealing " + damage + " damage to " + name);
            opponent.TakeDamage(damage);
            Destroy(this.gameObject);
        }
        else
        {
            GameObject instance = Instantiate(droppedItemPrefab, this.transform.position, Quaternion.LookRotation(travelDirection));
            instance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            instance.name = bowAmmo.name;
            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

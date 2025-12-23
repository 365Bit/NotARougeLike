using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    // Components
    private Rigidbody rigidBody;

    public GameObject droppedItemPrefab;

    private ItemDefinitions itemDefinitions;
    private ItemDefinition bowAmmo;
    Vector3 travelDirection;
    Vector3 lastPosition;

    private float damage;

    [Header("Properties")]
    public float speed;
    public float lifetime;
    public Vector3 tipPosition;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = true; // TODO: Make this projectile-specific

        itemDefinitions = GameObject.Find("Definitions").GetComponent<ItemDefinitions>();
        bowAmmo = itemDefinitions.definitions[3];
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidBody.AddForce(transform.forward * speed, ForceMode.Impulse);
        lastPosition = transform.position;

        // Destroy the projectile after its lifetime expires
        Destroy(this.gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        travelDirection = rigidBody.linearVelocity.normalized;
        transform.rotation = Quaternion.LookRotation(travelDirection);

        if (Physics.Raycast(lastPosition, travelDirection, out RaycastHit hit, 1.2f * (transform.position - lastPosition).magnitude)) {
            OnHit(hit);
        }

        lastPosition = transform.position;
    }

    private void OnBecameInvisible()
    {
        // Destroy the projectile when it goes off-screen
        Destroy(this.gameObject);
    }

    private void OnHit(RaycastHit hit)
    {
        GameObject hitObject = hit.transform.gameObject;
        string name = hitObject.name;
        Debug.Log("Projectile hit: " + name);

        GameObject droppedInstance = null;


        if (name.Contains("chest"))
        {
            Transform parent = hit.transform.parent;
            DestroyableObject destroyableObject = parent.GetComponent<DestroyableObject>();

            if (destroyableObject != null)
            {
                Debug.Log("Projectile dealing " + damage + " damage to " + parent.name);
                destroyableObject.TakeDamage(damage);
            }

            // instantiate dropped item and attach to opponent
            droppedInstance = Instantiate(droppedItemPrefab, hit.point - transform.TransformVector(tipPosition) + travelDirection.normalized * 0.5f, transform.GetChild(0).rotation);
            droppedInstance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            droppedInstance.name = bowAmmo.name;

            Destroy(this.gameObject);
        } else if (name.Contains("Opponent"))
        {
            Opponent opponent = hitObject.GetComponent<Opponent>();
            Debug.Log("Projectile dealing " + damage + " damage to " + name);
            opponent.TakeDamage(damage);

            // instantiate dropped item and attach to opponent
            droppedInstance = Instantiate(droppedItemPrefab, hit.point - transform.TransformVector(tipPosition) + travelDirection.normalized * 0.5f, transform.GetChild(0).rotation);
            droppedInstance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            droppedInstance.name = bowAmmo.name;

            Destroy(this.gameObject);
        }
        else
        {
            droppedInstance = Instantiate(droppedItemPrefab, hit.point - transform.TransformVector(tipPosition), transform.GetChild(0).rotation);
            droppedInstance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            droppedInstance.name = bowAmmo.name;

            Destroy(this.gameObject);
        }

        if (droppedInstance != null) {
            if (hitObject.TryGetComponent<AttachedObjects>(out AttachedObjects a)) {
                a.Attach(droppedInstance.transform);
            } else {
                var rb = droppedInstance.GetComponent<Rigidbody>();
                rb.isKinematic = !hitObject.TryGetComponent<Rigidbody>(out Rigidbody _);
            }
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

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

        // move one frame ahead already
        transform.rotation = Quaternion.LookRotation(travelDirection);
        travelDirection = rigidBody.linearVelocity.normalized;
        transform.position += travelDirection * Time.deltaTime;

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

        // if destroyable, deal damage
        if (hit.transform.TryGetComponent<DestroyableObject>(out DestroyableObject destroyableObject)) {
            Debug.Log("Projectile dealing " + damage + " damage to " + name);
            destroyableObject.TakeDamage(damage);
        }

        // deal damage to opponent
        if (hit.transform.TryGetComponent<Opponent>(out Opponent opponent)) {
            Debug.Log("Projectile dealing " + damage + " damage to " + name);
            opponent.TakeDamage(damage);
        }

        // arrow can penetrate opponent, so its position has be computed appropriately
        if (name.Contains("Opponent"))
        {
            float penetrationFactor = 0.5f;
            droppedInstance = Instantiate(droppedItemPrefab, hit.point - transform.TransformVector(tipPosition) + travelDirection.normalized * penetrationFactor, transform.GetChild(0).rotation);
            droppedInstance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            droppedInstance.name = bowAmmo.name;
        }
        else
        {
            droppedInstance = Instantiate(droppedItemPrefab, hit.point - transform.TransformVector(tipPosition), transform.GetChild(0).rotation);
            droppedInstance.GetComponent<DroppedItem>().SetItem(bowAmmo, 1);
            droppedInstance.name = bowAmmo.name;
        }

        if (droppedInstance != null) {
            if (hitObject.TryGetComponent<AttachedObjects>(out AttachedObjects a)) {
                a.Attach(droppedInstance.transform);
            } else {
                // if no AttachedObjects component, stick to objects that are not rigidbodies
                var rb = droppedInstance.GetComponent<Rigidbody>();
                rb.isKinematic = !hitObject.TryGetComponent<Rigidbody>(out Rigidbody _);
            }

            Destroy(this.gameObject);
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

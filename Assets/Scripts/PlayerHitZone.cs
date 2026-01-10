using System.ComponentModel;
using UnityEngine;

public class PlayerHitZone : MonoBehaviour
{
    private float damage;
    private Player player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string name = otherObject.name;

        if (name == "Player")
        {
            // Ignore self triggers
            return;
        }

        Debug.Log("Zone hit: " + name);

        if (otherObject.TryGetComponent<DestroyableObject>(out DestroyableObject destroyableObject))
        {
            Debug.Log("Zone dealing " + damage + " damage to " + other.gameObject.name);
            destroyableObject.TakeDamage(damage);
        }

        if (otherObject.TryGetComponent<Opponent>(out Opponent opponent))
        {
            Debug.Log("Zone dealing " + damage + " damage to " + name);
            if (!player.GetOpponentGotHit())
            {
                opponent.TakeDamage(damage);
                player.SetOpponentGotHit(true);
            }
        }
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

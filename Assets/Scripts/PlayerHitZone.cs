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
        string name = other.gameObject.name;

        if (name == "Player")
        {
            // Ignore self triggers
            return;
        }

        Debug.Log("Zone hit: " + name);

        if (other.gameObject.TryGetComponent<DestroyableObject>(out DestroyableObject destroyableObject)) {
            Debug.Log("Zone dealing " + damage + " damage to " + other.gameObject.name);
            destroyableObject.TakeDamage(damage);
        }

        if (name.Contains("Opponent"))
        {
            Opponent opponent = other.gameObject.GetComponent<Opponent>();
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

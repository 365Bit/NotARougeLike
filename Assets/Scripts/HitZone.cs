using UnityEngine;

public class HitZone : MonoBehaviour
{
    private float damage;
    private Opponent opponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        opponent = GetComponentInParent<Opponent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        string name = otherObject.name;

        if (name == "Opponent")
        {
            // Ignore self triggers
            return;
        }

        Debug.Log("Zone hit: " + name);

        if (otherObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("Zone dealing " + damage + " damage to " + name);
            if (!opponent.playerGotHit)
            {
                player.TakeDamage(damage);
                opponent.playerGotHit = true;
            }
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}

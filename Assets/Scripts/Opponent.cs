using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Opponent : MonoBehaviour
{
    // Components
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;

    private float currentHealth;
    private float wanderTime;

    private Vector3 spawnPosition;

    [Header("Combat")]
    public float attackRange = 2.0f;
    public float detectionRange = 10.0f;

    [Header("Health")]
    public float maxHealth = 100.0f;
    public float healthRegRate = 5.0f;

    [Header("Movement")]
    public float wanderInterval = 3.0f;
    public float wanderRadius = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;

        currentHealth = maxHealth;
        spawnPosition = transform.position;
        wanderTime = wanderInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null)
        {
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (distance <= detectionRange)
        {
            if (distance <= attackRange)
            {
                Attack();
            }
            else
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
        else
        {
            Wander();
            RegenerateHealth();
        }
    }
    void Attack()
    {
        // TODO
        navMeshAgent.isStopped = true;

        transform.LookAt(playerTransform);
    }

    void Die()
    {
        navMeshAgent.isStopped = true;
        Destroy(gameObject, 2.0f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            Die();
        }
    }

    void RegenerateHealth()
    {
        if (currentHealth < maxHealth)
        {
            float health = currentHealth + healthRegRate * Time.deltaTime;
            currentHealth = Mathf.Min(health, maxHealth);
        }
    }

    void Wander()
    {
        wanderTime += Time.deltaTime;

        if (wanderTime >= wanderInterval)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += spawnPosition;

            NavMeshHit navHit;
            NavMesh.SamplePosition(randomDirection, out navHit, wanderRadius, NavMesh.AllAreas);

            navMeshAgent.SetDestination(navHit.position);

            wanderTime = 0.0f;
        }
    }

}

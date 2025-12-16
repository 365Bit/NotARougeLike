using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class Opponent : MonoBehaviour
{
    // Components
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Player player;

    private float currentHealth;
    private float wanderTime;

    private Vector3 spawnPosition;

    [Header("Combat")]
    private Animator animator;
    public HitZone hitZone;
    public float hitRate = 1.0f;
    private float hitTime;
    public float hitCooldown = 0.5f;
    //private Transform rightShoulderTransform;
    public float swingDuration = 0.1f;
    public float strikeDuration = 0.1f;
    public float returnDuration = 0.1f;
    public float attackRange = 3.0f;
    public float detectionRange = 15.0f;
    private Vector3 restRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 swingRotation = new Vector3(-130.0f, 0.0f, 0.0f);
    private Vector3 strikeRotation = new Vector3(-30.0f, 0.0f, 0.0f);
    enum HitState
    {
        Idle,
        Return,
        Swing,
        Strike
    }

    private HitState hitState;

    private Quaternion startRotation;
    private Quaternion endRotation;

    public bool playerGotHit = false;

    [Header("Health")]
    public float maxHealth = 100.0f;
    public float healthRegRate = 5.0f;

    [Header("Movement")]
    public float wanderInterval = 3.0f;
    public float wanderRadius = 5.0f;

    public Node spawnRoom;
    private int nextNavPointID = 0;
    bool wandering = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        //rightShoulderTransform = transform.Find("Opp Right Shoulder");

        currentHealth = maxHealth;
        spawnPosition = transform.position;
        wanderTime = wanderInterval;

        hitState = HitState.Idle;
        hitCooldown = 0.0f;
        hitTime = 0.0f;
        hitZone.gameObject.SetActive(false);

        player = playerTransform.GetComponent<Player>();

        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("attackSpeed", hitRate);
    }

    // Update is called once per frame
    void Update()
    {
        if(navMeshAgent.speed > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }


        if (playerTransform == null)
        {
            return;
        }

        float distance = Vector3.Distance(playerTransform.position, transform.position);

        if (hitCooldown > 0.0f)
        {
            hitCooldown -= Time.deltaTime;
            if (hitCooldown < 0.0f)
            {
                hitCooldown = 0.0f;
            }
        }

        if (distance <= detectionRange && !player.isDead)
        {
            if (distance <= attackRange)
            {
                if(hitState == HitState.Idle)
                    Attack();
            }
            else
            {
                navMeshAgent.SetDestination(playerTransform.position);
            }
        }
        else
        {
            Wander();
            RegenerateHealth();
        }

        if (hitState != HitState.Idle)
        {
            hitTime += Time.deltaTime;

            float duration = hitState == HitState.Swing ? swingDuration :
                             hitState == HitState.Strike ? strikeDuration : returnDuration;

            float ratio = Mathf.Clamp(hitTime / duration, 0.0f, 1.0f);

            //rightShoulderTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, ratio);

            if (ratio >= 1.0f)
            {
                switch (hitState)
                {
                    case HitState.Swing:
                        animator.SetTrigger("swing");
                        hitState = HitState.Strike;
                        hitTime = 0.0f;

                        startRotation = Quaternion.Euler(swingRotation);
                        endRotation = Quaternion.Euler(strikeRotation);
                        break;
                    case HitState.Strike:
                        hitZone.gameObject.SetActive(true);
                        hitState = HitState.Return;
                        hitTime = 0.0f;

                        startRotation = Quaternion.Euler(strikeRotation);
                        endRotation = Quaternion.Euler(restRotation);
                        break;
                    case HitState.Return:
                        hitZone.gameObject.SetActive(false);
                        hitState = HitState.Idle;
                        hitCooldown = 0.5f;
                        playerGotHit = false;
                        navMeshAgent.isStopped = false;

                        break;
                    default:
                        Debug.LogError("Unknown Hit State: " + hitState);
                        return;
                }
            }
        }
    }
    void Attack()
    {
        // TODO
        navMeshAgent.isStopped = true;

        //transform.LookAt(playerTransform);

        if (hitCooldown > 0.0f || hitState != HitState.Idle)
        {
            return;
        }
        hitZone.SetDamage(20.0f);
        hitState = HitState.Swing;
        hitTime = 0.0f;

        //startRotation = rightShoulderTransform.localRotation;
        endRotation = Quaternion.Euler(swingRotation);
    }

    void Die()
    {
        navMeshAgent.isStopped = true;
        animator.SetTrigger("death");
        Destroy(gameObject, 2.0f);
    }

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("gotHit");
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
        navMeshAgent.isStopped = false;

        if (navMeshAgent.remainingDistance < 1)
        {
            wandering = false;
        }

        if (!wandering)
        {
            //Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            //Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f),Random.Range(-1f, 1f)).normalized;
            //Vector3 targetPos = spawnPosition + new Vector3(randomDirection.x, randomDirection.y, 0) * wanderRadius;
            //randomDirection += spawnPosition;

            List<NavPoint> targetList = spawnRoom.navPointList;

            Vector3 targetPos = targetList[nextNavPointID].transform.position;

            NavMeshHit navHit;
            NavMesh.SamplePosition(targetPos, out navHit, wanderRadius, NavMesh.AllAreas);

            navMeshAgent.SetDestination(navHit.position);

            wanderTime = 0.0f;
            wandering = true;
            nextNavPointID++;
            nextNavPointID %= targetList.Count;
        }
    }

}

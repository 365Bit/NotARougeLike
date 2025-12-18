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
    public float hitRate;
    private float hitTime;
    public float hitCooldown;
    public float stunDuration;
    private float stunTimer;
    //private Transform rightShoulderTransform;
    public float swingDuration;
    public float strikeDuration;
    public float returnDuration;
    public float attackRange;
    public float detectionRange;
    public float rotationSpeed;
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

    Vector3 localVelocity;
    private Quaternion lastRotation;
    public float viewAngle;
    private LayerMask obstacleMask;
    private float memoryTimer;
    public float memoryDuration;

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
        obstacleMask = LayerMask.GetMask("Wall");
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetBool("isDead"))
        {
            return;
        }

        localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);

        float moveX = localVelocity.x;
        float moveZ = localVelocity.z;
        animator.SetFloat("MoveX", moveX, 0.25f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveZ, 0.25f, Time.deltaTime);

        if (stunTimer > 0.0f)
        {
            stunTimer -= Time.deltaTime;
            if(stunTimer < 0.0f)
            {
                stunTimer = 0.0f;
            }
            return;
        }

        if(stunTimer == 0.0f) 
        {
            navMeshAgent.isStopped = false;
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

        if (CanSeePlayer() && !player.isDead)
        {
            navMeshAgent.updateRotation = false;
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            direction.y = 0.0f;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                lastRotation = transform.rotation;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
                float signedAngularVelocity = Vector3.SignedAngle(
                    lastRotation * Vector3.forward,
                    transform.rotation * Vector3.forward,
                    Vector3.up
                ) / Time.deltaTime;
                //Debug.Log("Signed Angular Velocity: " + signedAngularVelocity);
                if (Mathf.Abs(signedAngularVelocity) > 170f)
                {
                    animator.SetTrigger("TurnAround180");
                } else if (signedAngularVelocity > 45f) 
                {
                    animator.SetTrigger("TurnRight90");
                } else if (signedAngularVelocity < -45f) 
                { 
                    animator.SetTrigger("TurnLeft90");
                }
            }


            Vector3 playerOppDir = (transform.position - playerTransform.position).normalized;  
            Vector3 targetPosition;
            if (currentHealth < maxHealth * 0.5f)
            {
                targetPosition = playerTransform.position + playerOppDir * 7.5f;
            }
            else
            {
                targetPosition = playerTransform.position + playerOppDir * 2.5f;
            }
            navMeshAgent.SetDestination(targetPosition);

            if (distance <= attackRange && hitState == HitState.Idle && hitCooldown == 0.0f)
            {
                Attack();
            }
        }
        else
        {
            navMeshAgent.updateRotation = true;
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

                        break;
                    default:
                        Debug.LogError("Unknown Hit State: " + hitState);
                        return;
                }
            }
        }
        if (memoryTimer > 0.0f)
        {
            memoryTimer -= Time.deltaTime;
            if (memoryTimer < 0.0f)
            {
                memoryTimer = 0.0f;
            }
        }
    }
    void Attack()
    {
        // TODO
        navMeshAgent.isStopped = true;

        hitZone.SetDamage(20.0f);
        hitState = HitState.Swing;
        hitTime = 0.0f;

        //startRotation = rightShoulderTransform.localRotation;
        endRotation = Quaternion.Euler(swingRotation);
    }

    void Die()
    {
        navMeshAgent.isStopped = true;
        animator.SetBool("isDead", true);
        Destroy(gameObject, 2.0f);
    }

    public void TakeDamage(float damage)
    {
        navMeshAgent.isStopped = true;
        animator.SetTrigger("gotHit");
        stunTimer = stunDuration;

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
        //wanderTime += Time.deltaTime;
        navMeshAgent.isStopped = false;

        if (navMeshAgent.remainingDistance < 1)
        {
            wanderTime -= Time.deltaTime;
            if(wanderTime <= 0.0f) wandering = false;
        }

        if (!wandering)
        {
            //Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            //Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f),Random.Range(-1f, 1f)).normalized;
            //Vector3 targetPos = spawnPosition + new Vector3(randomDirection.x, randomDirection.y, 0) * wanderRadius;
            //randomDirection += spawnPosition;

            if(spawnRoom != null)
            {
                List<NavPoint> targetList = spawnRoom.navPointList;

                Vector3 targetPos = targetList[nextNavPointID].transform.position;

                NavMeshHit navHit;
                NavMesh.SamplePosition(targetPos, out navHit, wanderRadius, NavMesh.AllAreas);

                navMeshAgent.SetDestination(navHit.position);

                wanderTime = Random.Range(wanderInterval * 0.5f, wanderInterval * 2.0f);
                wandering = true;
                nextNavPointID++;
                nextNavPointID %= targetList.Count;
            }
        }
    }
    bool CanSeePlayer()
    {
        if(stunTimer > 0.0f)
        {
            return true;
        }

        Vector3 toPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > detectionRange) return false;
        else if (distanceToPlayer < 5.0f && !player.isSneaking()) return true;

        Vector3 forward = transform.forward;
        toPlayer.Normalize();

        float angleToPlayer = Vector3.Angle(forward, toPlayer);
        if (angleToPlayer > viewAngle) return false;

        if (Physics.Raycast(transform.position, toPlayer, distanceToPlayer, obstacleMask))
            return false;

        return true; // Player is visible
    }

}

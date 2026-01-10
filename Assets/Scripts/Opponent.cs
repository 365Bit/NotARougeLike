using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent)), RequireComponent(typeof(OpponentStats))]
public class Opponent : MonoBehaviour
{
    // Components
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Player player;
    private OpponentStats stats;

    private string className;
    private OpponentType opponentType;

    private float currentHealth;
    private float wanderTime;

    private Vector3 spawnPosition;

    [Header("Combat")]
    private Animator animator;
    public HitZone hitZone;

    private float hitCooldown;
    private float hitTime;
    private float stunTimer;

    enum OpponentType
    {
        SkeletonMelee,
        SkeletonRange
    }

    enum HitState
    {
        Idle,
        Return,
        Swing,
        Strike
    }

    private HitState hitState;

    public bool playerGotHit = false;

    public Node spawnRoom;
    private int nextNavPointID = 0;
    private bool hit, stunned, wandering = false;

    private Vector3 localDirection;
    private Vector3 localVelocity;

    private Quaternion lookRotation;
    private Vector3Int spawnRoomCenter;
    public float viewAngle;
    private LayerMask obstacleMask;
    private float memoryTimer;

    private List<NavPoint> targetList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerTransform = GameObject.Find("Player").transform;
        //rightShoulderTransform = transform.Find("Opp Right Shoulder");

        stats = GetComponent<OpponentStats>();

        className = stats.className;

        switch (className)
        {
            case "MeleeSkeleton":
                opponentType = OpponentType.SkeletonMelee;
                break;
            case "RangedSkeleton":
                opponentType = OpponentType.SkeletonRange;
                break;
            default:
                Debug.LogError("Unknown Opponent Class Name: " + className);
                break;
        }

        currentHealth = stats.maxHealth;
        spawnPosition = transform.position;

        if (spawnRoom != null)
        {
            Vector2Int roomCenter = (spawnRoom.BottomLeftAreaCorner + spawnRoom.TopRightAreaCorner) / 2;
            spawnRoomCenter = new Vector3Int(roomCenter.x, 0, roomCenter.y);

            targetList = spawnRoom.navPointList;
        }

        wanderTime = stats.wanderInterval;

        hitState = HitState.Idle;
        hitCooldown = 0.0f;
        hitTime = 0.0f;
        hitZone.gameObject.SetActive(false);

        player = playerTransform.GetComponent<Player>();

        animator = GetComponentInChildren<Animator>();
        animator.SetFloat("attackSpeed", stats.hitRate);
        obstacleMask = LayerMask.GetMask("Wall");

        navMeshAgent.speed = stats.movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetBool("isDead"))
        {
            return;
        }

        if (hit)
        {
            // Stop movement temporarily and face the direction of the hit
            navMeshAgent.updateRotation = false;

            localDirection.y = 0.0f;

            animator.SetFloat("MoveX", 0.0f, 0.25f, Time.deltaTime);
            animator.SetFloat("MoveZ", 0.0f, 0.25f, Time.deltaTime);

            lookRotation = Quaternion.LookRotation(localDirection);

            if (transform.rotation != lookRotation)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    lookRotation,
                    Time.deltaTime * stats.rotationSpeed
                );

                return;
            }
            
            if (!stunned && transform.rotation == lookRotation)
            {
                hit = false;
                stunned = true;
                stunTimer = stats.stunDuration * 2.0f;
            }
        }

        if (stunned)
        {
            stunTimer -= Time.deltaTime;

            if (stunTimer < 0.0f)
            {
                stunTimer = 0.0f;
                stunned = false;
            }

            return;
        }

        localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);

        float moveX = localVelocity.x;
        float moveZ = localVelocity.z;
        animator.SetFloat("MoveX", moveX, 0.25f, Time.deltaTime);
        animator.SetFloat("MoveZ", moveZ, 0.25f, Time.deltaTime);

        if (stunTimer == 0.0f)
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

            localDirection = (playerTransform.position - transform.position).normalized;
            localDirection.y = 0.0f;

            lookRotation = Quaternion.LookRotation(localDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                Time.deltaTime * stats.rotationSpeed
             );

            Vector3 playerOppDir = (transform.position - playerTransform.position).normalized;
            float distanceModifier = currentHealth < stats.maxHealth * 0.5f ? 7.5f : 2.5f;

            navMeshAgent.SetDestination(playerTransform.position + playerOppDir * distanceModifier);

            if (distance <= stats.attackRange && hitState == HitState.Idle && hitCooldown == 0.0f)
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

            float duration = hitState == HitState.Swing ? stats.swingDuration :
                             hitState == HitState.Strike ? stats.strikeDuration : stats.returnDuration;
            duration /= stats.hitRate;

            float ratio = Mathf.Clamp(hitTime / duration, 0.0f, 1.0f);

            if (ratio >= 1.0f)
            {
                switch (hitState)
                {
                    case HitState.Swing:
                        animator.SetTrigger("swing");
                        hitState = HitState.Strike;
                        hitTime = 0.0f;

                        break;
                    case HitState.Strike:
                        hitZone.gameObject.SetActive(true);
                        hitState = HitState.Return;
                        hitTime = 0.0f;

                        break;
                    case HitState.Return:
                        hitZone.gameObject.SetActive(false);
                        hitState = HitState.Idle;
                        hitCooldown = 1f / stats.hitRate;
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

        hitZone.SetDamage(stats.attackDamage);
        hitState = HitState.Swing;
        hitTime = 0.0f;
    }

    void Die()
    {
        navMeshAgent.isStopped = true;
        animator.SetBool("isDead", true);
        Destroy(gameObject, 2.0f);
    }

    public void TakeDamage(float damage, Vector3? direction = null)
    {
        bool hasDirection = direction.HasValue;

        navMeshAgent.isStopped = true;
        animator.SetTrigger("gotHit");

        if (hasDirection)
        {
            localDirection = direction.Value;
        }
        else
        {
            stunTimer = stats.stunDuration;
            stunned = true;
        }

        currentHealth -= damage;

        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            Die();
        }

        hit = true;
    }

    void RegenerateHealth()
    {
        if (currentHealth < stats.maxHealth)
        {
            float health = currentHealth + stats.healthRegRate * Time.deltaTime;
            currentHealth = Mathf.Min(health, stats.maxHealth);
        }
    }

    void Wander()
    {
        //wanderTime += Time.deltaTime;
        navMeshAgent.isStopped = false;

        if (navMeshAgent.remainingDistance < 1)
        {
            wanderTime -= Time.deltaTime;

            // Look towards the center of the spawn room
            localDirection = spawnRoomCenter - transform.position;
            localDirection.y = 0.0f;

            lookRotation = Quaternion.LookRotation(localDirection);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                lookRotation,
                Time.deltaTime * stats.rotationSpeed
            );

            if (wanderTime <= 0.0f) wandering = false;
        }

        if (!wandering)
        {
            Vector3 targetPos = targetList[nextNavPointID].transform.position;

            NavMeshHit navHit;
            NavMesh.SamplePosition(targetPos, out navHit, stats.wanderRadius, NavMesh.AllAreas);

            navMeshAgent.SetDestination(navHit.position);

            wanderTime = Random.Range(stats.wanderInterval * 0.5f, stats.wanderInterval * 2.0f);
            wandering = true;

            nextNavPointID++;
            nextNavPointID %= targetList.Count;
        }
    }
    bool CanSeePlayer()
    {
        if (stunned)
        {
            return true;
        }

        Vector3 toPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;

        if (distanceToPlayer > stats.detectionRange) return false;
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

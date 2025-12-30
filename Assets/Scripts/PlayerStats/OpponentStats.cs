using UnityEngine;

[DisallowMultipleComponent]
public class OpponentStats : MonoBehaviour
{
    [Header("Combat")]
    public float hitRate;
    public float stunDuration;

    public float swingDuration;
    public float strikeDuration;
    public float returnDuration;
    public float attackRange;

    [Header("Health")]
    public float maxHealth;
    public float healthRegRate;

    [Header("Movement")]
    public float wanderInterval;
    public float wanderRadius;
    public float rotationSpeed;
    public float movementSpeed;

    [Header("Perception")]
    public float memoryDuration;
    public float detectionRange;
}

public enum OpponentStatKey {
    HitRate,
    StunDuration,
    SwingDuration,
    StrikeDuration,
    ReturnDuration,
    AttackRange,
    MaxHealth,
    HealthRegRate,
    WanderInterval,
    WanderRadius,
    RotationSpeed,
    MovementSpeed,
    MemoryDuration,
    DetectionRange
};



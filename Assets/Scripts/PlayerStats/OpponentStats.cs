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

    public void ComputeFrom(OpponentClassDefinition def, int level) {
        hitRate = def[OpponentStatKey.HitRate].ComputeFrom(level);
		stunDuration = def[OpponentStatKey.StunDuration].ComputeFrom(level);

		swingDuration = def[OpponentStatKey.SwingDuration].ComputeFrom(level);
		strikeDuration = def[OpponentStatKey.StrikeDuration].ComputeFrom(level);
		returnDuration = def[OpponentStatKey.ReturnDuration].ComputeFrom(level);
		attackRange = def[OpponentStatKey.AttackRange].ComputeFrom(level);

		maxHealth = def[OpponentStatKey.MaxHealth].ComputeFrom(level);
		healthRegRate = def[OpponentStatKey.HealthRegRate].ComputeFrom(level);

		wanderInterval = def[OpponentStatKey.WanderInterval].ComputeFrom(level);
		wanderRadius = def[OpponentStatKey.WanderRadius].ComputeFrom(level);
		rotationSpeed = def[OpponentStatKey.RotationSpeed].ComputeFrom(level);
		movementSpeed = def[OpponentStatKey.MovementSpeed].ComputeFrom(level);

		memoryDuration = def[OpponentStatKey.MemoryDuration].ComputeFrom(level);
		detectionRange = def[OpponentStatKey.DetectionRange].ComputeFrom(level);
    }
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



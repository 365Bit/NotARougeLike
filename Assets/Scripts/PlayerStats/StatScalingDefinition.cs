using UnityEngine;
using System;

public class StatScalingDefinitions : MonoBehaviour
{
    [System.Serializable]
    public struct Def {
        public StatKey key;
        public ScalingFormula def;
    };

    // for editor
    [SerializeField]
    private Def[] definitions;

    // optimized datastructure
    private ScalingFormula[] _defs;

    // put definitions into an array
    public void Awake() {
        _defs = new ScalingFormula[Enum.GetValues(typeof(StatKey)).Length];
        foreach (Def d in definitions) {
            _defs[(int)d.key] = d.def;
        }

        // check definitions
        for (int i = 0; i < _defs.Length; i++) {
            if (_defs[i] == null) Debug.Log("scaling undefined for " + Enum.GetName(typeof(StatKey), (StatKey) i));
        }
    }

    public ScalingFormula this[StatKey stat] {
        get => _defs[(int)stat];
    }
}


[System.Serializable]
public class ScalingFormula {
    [System.Serializable]
    public struct Operand {
        [Tooltip("The base stat to depend on")]
        public BaseStatKey stat;
        [Tooltip("value for each upgrade level")]
        public float[] value;
    }

    // defines how the values should be combined
    public enum Operation {
        Addition, Multiplication
    };
    public float baseValue;
    public Operation operation;
    public Operand[] scalingInBaseStat;

    public float ComputeFrom(PlayerUpgrades upgradeLevels) {
        float result = baseValue;
        if (scalingInBaseStat != null) {
            if (operation == Operation.Multiplication) {
                foreach (var s in scalingInBaseStat) {
                    result *= s.value[upgradeLevels[s.stat]];
                }
            } else {
                foreach (var s in scalingInBaseStat) {
                    result += s.value[upgradeLevels[s.stat]];
                }
            }
        }
        return result;
    }
}

// base stats that can be upgraded
public enum BaseStatKey {
    Health,
    Armour,
    Strength,
    Agility,
    Stamina,
    Dexterity,
    Perception
}

// stats that are relevant to gameplay and depend on base stats
public enum StatKey {
    FireRate,
    HitRate,
    SwingDuration,
    StrikeDuration,
    ReturnDuration,
    MaxMana,
    ManaRegRate,
    RunSpeed,
    WalkSpeed,
    SneakSpeed,
    MaxSlideTime,
    SlideSpeed,
    MaxHealth,
    HealthRegRate,
    JumpHeight,
    MaxStamina,
    StaminaRegRate,
    RunConsRate,
    SlideConsRate,
    JumpConsRate
}


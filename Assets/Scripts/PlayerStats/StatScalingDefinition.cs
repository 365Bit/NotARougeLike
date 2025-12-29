using UnityEngine;
using System;
using System.Collections.Generic;

/// provides definitions on how to compute gameplay-stats from base-stats
[DisallowMultipleComponent]
public class StatScalingDefinitions : KeyValueStoreComponent<StatKey, ScalingFormula>
{ }


[System.Serializable]
public class KeyValueStoreComponent<Key, Value> : MonoBehaviour
    where Key : System.Enum
{
    public KeyValueStore<Key, Value> data;

    void Awake() { data.Init(); }

    public Value this[Key key] { get => data[key]; }
}

[System.Serializable]
public class KeyValueStore<Key, Value>
    where Key : System.Enum
{
    [System.Serializable]
    public struct Def {
        public Key key;
        public Value def;
    };

    // for editor
    [SerializeField]
    public Def[] definitions;

    // optimized datastructure
    [HideInInspector]
    public Value[] _defs;

    private int ToIndex(Key key) {
        Type underlyingType = Enum.GetUnderlyingType(key.GetType());
        return (int)Convert.ChangeType(key, underlyingType);
    }

    // put definitions into an array
    public void Init() {
        _defs = new Value[Enum.GetValues(typeof(Key)).Length];
        foreach (Def d in definitions) {
            _defs[ToIndex(d.key)] = d.def;
        }

        // check definitions
        for (int i = 0; i < _defs.Length; i++) {
            if (_defs[i] == null) Debug.Log("scaling undefined for " + i);
        }
    }

    public Value this[Key key] {
        get => _defs[ToIndex(key)];
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

    // initial value
    public float baseValue;

    // how to combine operands
    public Operation operation;
    // operand constists of a value per upgrade level of a base stat
    public Operand[] scalingInBaseStat;

    public float ComputeFrom(PlayerUpgradeState upgradeLevels) {
        float result = baseValue;
        if (scalingInBaseStat != null) {
            foreach (Operand s in scalingInBaseStat) {
                int index = (upgradeLevels[s.stat] < s.value.Length) ? upgradeLevels[s.stat] : s.value.Length - 1;
                float value = s.value[index];
                if (operation == Operation.Multiplication) {
                    result *= value;
                } else {
                    result += value;
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


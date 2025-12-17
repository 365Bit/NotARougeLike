using UnityEngine;
using System;

[DisallowMultipleComponent]
public class DungeonDefinitions : MonoBehaviour
{
    [System.Serializable]
    public struct Def {
        public DungeonStatKey key;
        public ScalingFunction def;
    };

    // for editor
    [SerializeField]
    private Def[] definitions;

    // optimized datastructure
    private ScalingFunction[] _defs;

    // put definitions into an array
    public void Awake() {
        _defs = new ScalingFunction[Enum.GetValues(typeof(DungeonStatKey)).Length];
        foreach (Def d in definitions) {
            _defs[(int)d.key] = d.def;
        }

        // check definitions
        for (int i = 0; i < _defs.Length; i++) {
            if (_defs[i] == null) Debug.Log("scaling undefined for " + Enum.GetName(typeof(DungeonStatKey), (DungeonStatKey) i));
        }
    }

    public ScalingFunction this[DungeonStatKey stat] {
        get => _defs[(int)stat];
    }
}


public enum DungeonStatKey {
    Hostility,
    Populatedness,
    BossProbability,
    Size
}


[System.Serializable]
public class ScalingFunction {
    public enum Function {
        // b * a^x + c
        Exponential,
        // b * log_a(x) + c
        Logarithmic,
        // b * x^a + c
        Polynomial,
        // values
        Cyclic
    }

    public Function function;

    public float a;
    public float b;
    public float c;

    [Header("For Cyclic")]
    public float[] values = null;

    public float ComputeFrom(int level) {
        switch (function) {
        case Function.Exponential:
            return (float) (b * Math.Pow(a, level) + c);
        case Function.Logarithmic:
            return (float) (b * Math.Log((double)level, a) + c);
        case Function.Polynomial:
            return (float) (b * Math.Pow((double)level, a) + c);
        case Function.Cyclic: 
            return values[(level >= values.Length) ? values.Length - 1 : level];
        }
        return 0;
    }
}


using UnityEngine;
using System;
using System.Collections.Generic;

public class DungeonPropertyDefinitions : MonoBehaviour
{
    [System.Serializable]
    public struct Def {
        public DungeonPropertyKey key;
        public InterpolationScaling def;
    };

    // for editor
    [SerializeField]
    private Def[] definitions;

    // internal data structure
    private InterpolationScaling[] _defs;

    void Awake() {
        _defs = new InterpolationScaling[Enum.GetValues(typeof(DungeonPropertyKey)).Length];
        foreach (Def d in definitions) {
            _defs[(int)d.key] = d.def;
        }

        // check definitions
        for (int i = 0; i < _defs.Length; i++) {
            if (_defs[i] == null) Debug.Log("scaling undefined for " + Enum.GetName(typeof(DungeonPropertyKey), (DungeonPropertyKey) i));
        }
    }

    // returns the dungeon properties for the given level
    public DungeonProperties
    ComputeFrom(int level) {
        Dictionary<DungeonPropertyKey, float> result = new();
        
        for (int i = 0; i < _defs.Length; i++) {
            result.Add((DungeonPropertyKey) i, _defs[i].ComputeFrom(level));
        }

        return new DungeonProperties(result);
    }
}

public class DungeonProperties : Dictionary<DungeonPropertyKey, float> {
    public DungeonProperties(Dictionary<DungeonPropertyKey, float> b) : base(b) { }
}


[System.Serializable]
public class InterpolationScaling {
    [System.Serializable]
    public struct Point {
        public int   level;
        public float value;
    }

    public float baseValue = 1.0f;

    // interpolation points
    public Point[] points;

    // how to treat level values that are outside the given points
    public bool cyclic;

    public float ComputeFrom(int level) {
        if (points == null || points.Length == 0)
            return baseValue;

        int index = 0;

        if (cyclic) {
            level = level % (points[points.Length - 1].level);
        }

        for (; index + 1 < points.Length && points[index + 1].level <= level; index++);

        // index+1 still has to point into the array
        index = Math.Clamp(index, 0, points.Length - 2);

        // linear interpolation
        float diff = (points[index + 1].value - points[index].value) / (points[index + 1].level - points[index].level);
        float value = points[index].value + (level - points[index].level) * diff;

        return baseValue * value;
    }
};

public enum DungeonPropertyKey {
    // dungeon size (maybe expected number of rooms, or width)
    Size,
    // number of traps (maximum)
    TrapCount,
    // 
    EnemyCount,
    EnemyLevel,
    // probability for having a boss
    HasBoss,
    // allows balancing of (scarce) resources 
    AvailableXP,
    AvailableGold,
    AvailableAmmoPerEnemy
}


using UnityEngine;
using System;

public class UpgradeCostDefinitions : MonoBehaviour
{
    [System.Serializable]
    public struct Def {
        public BaseStatKey stat;
        public int[] xpValues;
    }

    // for editor
    [SerializeField]
    private Def[] cost;

    // internal data strcture
    public int[][] _cost;

    // gets xp cost for the given stat
    public int[] this[BaseStatKey stat] {
        get => _cost[(int)stat];
    }

    public int MaxLevel(BaseStatKey stat) {
        return _cost[(int)stat].Length;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _cost = new int[Enum.GetValues(typeof(BaseStatKey)).Length][];

        foreach(var def in cost) {
            _cost[(int)def.stat] = def.xpValues;
        }
    }
}





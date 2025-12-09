using UnityEngine;
using System;

public class PlayerUpgrades : MonoBehaviour
{
    public PlayerUpgrades()
    {
        GameSaver.subscribe(this);
    }

    [SerializeField]
    private int[] levels;

    public void Awake() {
        levels = new int[Enum.GetValues(typeof(BaseStatKey)).Length];
        foreach (BaseStatKey key in Enum.GetValues(typeof(BaseStatKey))) {
            levels[(int)key] = 0;
        }
        GameSaver.save();
    }

    public int this[BaseStatKey stat] {
        get => levels[(int)stat];
    }

    public void Upgrade(BaseStatKey stat) {
        levels[(int)stat] += 1;
    }
}

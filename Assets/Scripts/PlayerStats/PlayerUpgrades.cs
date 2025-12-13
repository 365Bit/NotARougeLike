using UnityEngine;
using System;

public class PlayerUpgrades : MonoBehaviour
{
    [SerializeField]
    private PlayerUpgradeState levels;

    public PlayerUpgrades() {
        levels = RunData.Instance.upgrades;
    }

    public ref int this[BaseStatKey stat] {
        get => ref levels[stat];
    }

    public void Upgrade(BaseStatKey stat) {
        levels[stat] += 1;
    }
}

public class PlayerUpgradeState {
    [SerializeField]
    private int[] levels;

    public PlayerUpgradeState() {
        levels = new int[Enum.GetValues(typeof(BaseStatKey)).Length];
        foreach (BaseStatKey key in Enum.GetValues(typeof(BaseStatKey))) {
            levels[(int)key] = 0;
        }
    }

    public ref int this[BaseStatKey stat] {
        get => ref levels[(int)stat];
    }

    public void Upgrade(BaseStatKey stat) {
        levels[(int)stat] += 1;
    }
}


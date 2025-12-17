using UnityEngine;
using System;

public class PlayerUpgrades : MonoBehaviour,ISaveable
{
    private PlayerUpgradeState levels;

    public PlayerUpgrades() {
        levels = RunData.Instance.upgrades;
    }

    void Awake() {
        GameSaver.subscribe(levels);
    }

    public ref int this[BaseStatKey stat] {
        get => ref levels[stat];
    }

    public void Upgrade(BaseStatKey stat) {
        levels[stat] += 1;
    }
}

[Serializable]
public class PlayerUpgradeState {
    [SaveAble]
    public int[] levels;

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


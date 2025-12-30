using UnityEngine;
using UnityEngine.Events;
using System;

using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    [Header("Rendering GameObjects")]
    public TMP_Text nameText, levelText, costText;
    public Button upgradeButton;

    public Color background, selectedBackground;

    public void SetStat(BaseStatKey stat, int level, int maxLevel) {
        nameText.text  = Enum.GetName(typeof(BaseStatKey), stat);
        levelText.text = level.ToString() + "/" + maxLevel.ToString();
    }

    public void SetCost(int cost) {
        costText.text = (cost >= 0) ? cost.ToString() : "";
    }

    public void SetSelected(bool selected) {
        GetComponent<Image>().color = selected ? selectedBackground : background;
    }

    public void SetUpgradeable(bool upgradeable) {
        upgradeButton.interactable = upgradeable;
    }

    void Awake()
    {
        GetComponent<Image>().color = background;
    }
}

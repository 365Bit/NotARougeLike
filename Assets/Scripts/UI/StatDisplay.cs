using UnityEngine;
using System;

using UnityEngine.UI;
using TMPro;

public class StatDisplay : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text levelText;
    public Button upgradeButton;

    public Color background, selectedBackground;

    public void SetStat(BaseStatKey stat, int level) {
        nameText.text  = Enum.GetName(typeof(BaseStatKey), stat);
        levelText.text = level.ToString();
    }

    public void SetSelected(bool selected) {
        GetComponent<Image>().color = selected ? selectedBackground : background;
    }

    public void SetUpgradeable(bool upgradeable) {
        upgradeButton.interactable = upgradeable;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        GetComponent<Image>().color = background;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;
using System;

public class UpgradeUI : MonoBehaviour
{
    [Header("Rendering")]
    public GameObject statUIPrefab;
    public Vector2 origin;
    public Vector2 offset;

    private int selectedIndex;
    public BaseStatKey selectedStat { get => (BaseStatKey)stats.GetValue(selectedIndex); }

    private GameObject[] uiInstances;
    private UpgradeCostDefinitions defs;
    private GameObject player;

    // get array of stats
    private Array stats = Enum.GetValues(typeof(BaseStatKey));

    // 
    public void MoveSelection(Vector2 delta) {
        selectedIndex = (selectedIndex - (int)delta.y + stats.Length) % stats.Length;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        defs = GameObject.Find("Definitions").GetComponent<UpgradeCostDefinitions>();
        player = GameObject.Find("Player");

        // clear old ui objects
        if (uiInstances != null) {
            foreach (var i in uiInstances) 
                GameObject.Destroy(i);
        } else {
            uiInstances = new GameObject[stats.Length];
        }

        int index = 0;
        foreach(BaseStatKey key in stats) {
            var obj = Instantiate(statUIPrefab, transform);
            obj.transform.localPosition = origin + offset * index;

            uiInstances[index] = obj;
            index++;
        }
    }

    public void UpdateUpgrades() {
        int index = 0;
        foreach (GameObject i in uiInstances) {
            var disp = i.GetComponent<StatDisplay>();
            var stat = (BaseStatKey)stats.GetValue(index);
            int currentLevel = player.GetComponent<PlayerUpgrades>()[stat];
            int maxLevel = defs.MaxLevel(stat);
            int[] costs = defs[stat];
            int cost = (costs != null && currentLevel < costs.Length) ? costs[currentLevel] : -1;

            disp.SetStat(stat, currentLevel, maxLevel);
            disp.SetSelected(index == selectedIndex);
            disp.SetUpgradeable(currentLevel < maxLevel);
            disp.SetCost(cost);

            index++;
        }
    }

    void Update() {
        UpdateUpgrades();
    }
}

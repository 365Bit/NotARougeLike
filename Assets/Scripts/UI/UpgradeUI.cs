using UnityEngine;
using System;

public class UpgradeUI : MonoBehaviour
{
    [Header("Rendering")]
    public GameObject statUIPrefab;
    public Vector2 origin;
    public Vector2 offset;

    [Header("Stat selection")]
    public int selectedStat {get; private set;}

    private GameObject[] uiInstances;
    private UpgradeCostDefinitions defs;

    // get array of stats
    private Array stats = Enum.GetValues(typeof(BaseStatKey));

    // 
    public void MoveSelection(Vector2 delta) {
        selectedStat = (selectedStat - (int)delta.y + stats.Length) % stats.Length;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        defs = GameObject.Find("Definitions").GetComponent<UpgradeCostDefinitions>();

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
            int currentLevel = 0;
            int maxLevel = 10;
            int[] costs = defs[stat];
            int cost = costs != null ? costs[currentLevel] : 1000;

            disp.SetStat(stat, currentLevel, maxLevel);
            disp.SetSelected(index == selectedStat);
            disp.SetUpgradeable(currentLevel < maxLevel);
            disp.SetCost(cost);

            index++;
        }
    }

    void Update() {
        UpdateUpgrades();
    }
}

using UnityEngine;
using System;

public class UpgradeUI : MonoBehaviour
{
    public GameObject statUIPrefab;

    public Vector2 origin;
    public Vector2 offset;

    private int selectedStat;

    private GameObject[] uiInstances;

    // get array of stats
    private Array stats = Enum.GetValues(typeof(BaseStatKey));

    // 
    public void MoveSelection(Vector2 delta) {
        selectedStat = (selectedStat - (int)delta.y + stats.Length) % stats.Length;
    }

    public bool TryUpgrade() {
        Debug.Log("trying to upgrade " + Enum.GetName(typeof(BaseStatKey), stats.GetValue(selectedStat)));
        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // clear
        if (uiInstances != null) 
            foreach (var i in uiInstances) 
                GameObject.Destroy(i);
        else
            uiInstances = new GameObject[stats.Length];

        int index = 0;
        foreach(BaseStatKey key in stats) {
            var obj = Instantiate(statUIPrefab, transform);
            obj.transform.localPosition = origin + offset * index;

            var disp = obj.GetComponent<StatDisplay>();
            disp.SetStat(key, 0);

            uiInstances[index] = obj;
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int index = 0;
        foreach (GameObject i in uiInstances) {
            var disp = i.GetComponent<StatDisplay>();
            disp.SetStat((BaseStatKey)stats.GetValue(index), 0);
            disp.SetSelected(index == selectedStat);
            disp.SetUpgradeable(false);
            index++;
        }
    }
}

using UnityEngine;

public class RunDataManager : MonoBehaviour
{
    public void InitializeOrLoad()
    {
        if (!RunData.Instance.Initialized) {
            RunData.Instance.NewGame();
        }
        GameSaver.load();
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class FloorClearing : MonoBehaviour
{
    // reduce Find calls
    private GameObject cachedOpponent;

    public bool CheckClearingCondition() {
        if (cachedOpponent != null && cachedOpponent.activeInHierarchy) return false;

        cachedOpponent = GameObject.Find("Opponent(Clone)");
        return cachedOpponent == null;
    }

    void Update()
    {
        if (CheckClearingCondition()) {
            Debug.Log("Floor cleared, going to next level");
            RunData.Instance.LevelDone();
            GameSaver.save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}

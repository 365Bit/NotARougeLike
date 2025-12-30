using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restarting Level");

        RunData.Instance.NewRun();

        //DungeonCreator dungeonCreator = GameObject.Find("DungeonCreator").GetComponent<DungeonCreator>();
        //dungeonCreator.CreateDungeon();

        //UIManager.Instance.HideDeathScreen();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void RestartGame()
    {
        Debug.Log("Restarting Level");

        GameSaver.save();

        //DungeonCreator dungeonCreator = GameObject.Find("DungeonCreator").GetComponent<DungeonCreator>();
        //dungeonCreator.CreateDungeon();

        //UIManager.Instance.HideDeathScreen();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

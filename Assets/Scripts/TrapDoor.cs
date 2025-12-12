using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapDoor : MonoBehaviour
{
    private bool isenabled = false;

    public void Enable() {
        isenabled = true;
    }

    public void Interact()
    {
        if (!isenabled) return;

        Debug.Log("Floor cleared, going to next level");
        RunData.Instance.LevelDone();
        GameSaver.save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class TrapDoor : MonoBehaviour
{
    private bool isEnabled = false;

    public void Enable() {
        isEnabled = true;
    }

    public void Interact()
    {
        if (!isEnabled) return;

        Debug.Log("Floor cleared, going to next level");
        RunData.Instance.LevelDone();
        GameSaver.save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

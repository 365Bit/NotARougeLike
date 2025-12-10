using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject canvas;

    private GameObject deathScreen;

    void Awake()
    {
        Instance = this;
        Transform child = canvas.transform.Find("Death Screen");
        deathScreen = child.gameObject;
        deathScreen.SetActive(false);
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void HideDeathScreen()
    {
        deathScreen.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}


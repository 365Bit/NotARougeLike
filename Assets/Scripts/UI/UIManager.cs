using UnityEngine;
using UnityEngine.InputSystem;

// class to handle ui state:
// can switch between gameplay/inventory/death-screen (/pause-screen)
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // ui elements
    private GameObject playerStats;
    private GameObject hotbar;
    private GameObject deathScreen;

    // set of possible ui states
    public enum UIState {
        Gameplay,
        Inventory,
        Death,
        Pause
    };
    public UIState currentState { get; private set; }


    public void SwitchToGameplay() {
        playerStats.SetActive(true);
        hotbar.SetActive(true);

        currentState = UIState.Gameplay;
    }

    public void SwitchToInventory() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        // TODO

        currentState = UIState.Inventory;
    }

    public void SwitchToDeathScreen() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentState = UIState.Death;
    }

    void Awake() {
        Instance = this;
        playerStats = GameObject.Find("Player Stats");
        hotbar = GameObject.Find("Inventory");
        deathScreen = GameObject.Find("Death Screen");
        deathScreen.SetActive(false);

        SwitchToGameplay();
    }

}

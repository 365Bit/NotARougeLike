using UnityEngine;
using UnityEngine.InputSystem;

// class to handle ui state:
// can switch between gameplay/inventory/death-screen (/pause-screen)
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    // ui elements
    public GameObject playerStats { get; private set; }
    public GameObject hotbar { get; private set; }
    public GameObject inventoryUI { get; private set; }
    public GameObject deathScreen { get; private set; }

    public bool inventoryOpen { get => inventoryUI.active; }

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

        inventoryUI.SetActive(false);

        deathScreen.SetActive(false);

        currentState = UIState.Gameplay;
    }

    public void SwitchToInventory() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        inventoryUI.SetActive(true);

        currentState = UIState.Inventory;
    }

    public void SwitchToDeathScreen() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        inventoryUI.SetActive(false);

        deathScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        currentState = UIState.Death;
    }

    void Awake() {
        Instance = this;
        playerStats = GameObject.Find("Player Stats");
        hotbar = GameObject.Find("Hotbar");

        inventoryUI = GameObject.Find("Inventory");

        deathScreen = GameObject.Find("Death Screen");

        SwitchToGameplay();
    }

}

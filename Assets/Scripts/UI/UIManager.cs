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
    public GameObject upgradesUI { get; private set; }
    public GameObject deathScreen { get; private set; }

    public bool inventoryOpen { get => inventoryUI != null && inventoryUI.activeInHierarchy; }
    public bool upgradesOpen { get => upgradesUI != null && upgradesUI.activeInHierarchy; }

    // set of possible ui states
    public enum UIState {
        Gameplay,
        Inventory,
        Upgrades,
        Death,
        Pause
    };
    public UIState currentState { get; private set; }


    public void SwitchToGameplay() {
        upgradesUI.SetActive(false);
        inventoryUI.SetActive(false);
        deathScreen.SetActive(false);

        playerStats.SetActive(true);
        hotbar.SetActive(true);

        currentState = UIState.Gameplay;
    }

    public void SwitchToInventory() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);
        upgradesUI.SetActive(false);

        inventoryUI.SetActive(true);

        currentState = UIState.Inventory;
    }

    public void SwitchToUpgrades() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);
        inventoryUI.SetActive(false);

        upgradesUI.SetActive(true);

        currentState = UIState.Inventory;
    }


    public void SwitchToDeathScreen() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        inventoryUI.SetActive(false);
        upgradesUI.SetActive(false);

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
        upgradesUI = GameObject.Find("Upgrades");

        deathScreen = GameObject.Find("Death Screen");

        SwitchToGameplay();
    }

}

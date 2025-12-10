using UnityEngine;
using UnityEngine.InputSystem;

// class to handle ui state:
// can switch between gameplay/inventory/death-screen (/pause-screen)
public class UIManager : MonoBehaviour
{
    // ui elements
    private GameObject playerStats;
    private GameObject hotbar;

    // set of possible ui states
    public enum UiState {
        Gameplay,
        Inventory,
        Death,
        Pause
    };
    public UiState currentState { get; private set; }


    public void SwitchToGameplay() {
        playerStats.SetActive(true);
        hotbar.SetActive(true);

        currentState = UiState.Gameplay;
    }

    public void SwitchToInventory() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        // TODO

        currentState = UiState.Inventory;
    }

    public void SwitchToDeathScreen() {
        playerStats.SetActive(false);
        hotbar.SetActive(false);

        // TODO

        currentState = UiState.Death;
    }

    void Awake() {
        playerStats = GameObject.Find("Player Stats");
        hotbar = GameObject.Find("Hotbar");

        SwitchToGameplay();
    }

}

using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public GameObject hotbar;
    public GameObject inventoryUI;

    public bool inventoryOpen;

    void Start() {
        hotbar = GameObject.Find("Hotbar");
        inventoryUI = GameObject.Find("Inventory");

        inventoryOpen = false;
        inventoryUI.SetActive(false);
    }

    public void OpenInventory() {
        Debug.Log("Opening inventory");
        hotbar.SetActive(false);
        inventoryUI.SetActive(true);
        inventoryOpen = true;
    }

    public void CloseInventory() {
        Debug.Log("closing inventory");
        hotbar.SetActive(true);
        inventoryUI.SetActive(false);
        inventoryOpen = false;
    }
}

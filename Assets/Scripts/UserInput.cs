using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    private Player player;
    private PlayerUpgrades playerUpgrades;
    private UIManager uiController;
    private InventoryUI inventoryUI;
    private UpgradeUI upgradeUI;

    private Vector2 direction;
    private Vector2 rotation;

    private bool controlPlayer {
        get => !uiController.upgradesOpen && !uiController.inventoryOpen && !player.isDead;
    }

    private bool aim;
    private bool interact;
    private bool jump;
    private bool run;
    private bool slide;
    private bool sneak;
    private bool escape;

    private int itemID;

    // whether a ui should be toggled in this frame
    private bool toggleUI;
    private bool switchUI;
    private bool inventory;
    private bool upgrades;

    // values for ui
    private Vector2 uiDirection;
    private bool uiSelect;

    public float mouseSensitivity = 0.1f;
    public float stickSensitivity = 150f;

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        playerUpgrades = GameObject.Find("Player").GetComponent<PlayerUpgrades>();
        uiController = GetComponent<UIManager>();
        inventoryUI = uiController.inventoryUI.GetComponent<InventoryUI>();
        upgradeUI = uiController.upgradesUI.GetComponent<UpgradeUI>();

        rotation = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Reset variables
        direction = Vector2.zero;
        uiDirection = Vector2.zero;

        aim = false;
        interact = false;
        jump = false;
        run = false;
        slide = false;
        sneak = false;
        inventory = false;
        upgrades = false;
        toggleUI = false;
        switchUI = false;
        uiSelect = false;

        itemID = -1;

        KeyboardInput();
        GamepadInput();
        MouseInput();

        // Apply inputs
        if (controlPlayer) {
            player.Run(run);
            player.Slide(slide);
            player.Sneak(sneak);

            player.Aim(aim);
            player.Rotate(rotation);
            player.Move(direction);

            if (itemID > -1)
            {
                player.UseItem(itemID);
            }

            if (interact)
            {
                player.Interact();
            }

            if (jump)
            {
                player.Jump();
            }
        }

        // control upgrades ui
        if (uiController.upgradesOpen) {
            upgradeUI.MoveSelection(uiDirection);

            if (uiSelect)
                playerUpgrades.TryUpgrade(upgradeUI.selectedStat);
        }

        // control inventory ui
        if (uiController.inventoryOpen) {
            if (uiDirection != Vector2.zero)
                inventoryUI.MoveSelection(uiDirection);
            
            if (uiSelect)
                inventoryUI.ToggleItemGrabbed();
        }

        // handle switching between UIs
        if (switchUI) {
            if (uiController.upgradesOpen) {
                uiController.SwitchToInventory();
            } else if (uiController.inventoryOpen) {
                uiController.SwitchToUpgrades();
            }
        }

        if (escape) {
            uiController.SwitchToGameplay();
        }

        // handle toggling of ui
        if (toggleUI) {
            if (uiController.upgradesOpen || uiController.inventoryOpen) {
                uiController.SwitchToGameplay();
            } else {
                uiController.SwitchToInventory();
            }
        }

        // toggle upgrade ui
        if (upgrades) {
            if (uiController.upgradesOpen) {
                uiController.SwitchToGameplay();
            } else {
                uiController.SwitchToUpgrades();
            }
        }

        // toggle inventory ui
        if (inventory) { 
            if (uiController.inventoryOpen) {
                uiController.SwitchToGameplay();
            } else {
                uiController.SwitchToInventory();
            }
        }
    }

    void GamepadInput()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
        {
            return;
        }

        bool startButton = gamepad.startButton.isPressed;

        // Movement
        direction += gamepad.leftStick.ReadValue();

        // Camera
        rotation += gamepad.rightStick.ReadValue() * Time.deltaTime * stickSensitivity;
        aim |= gamepad.leftTrigger.IsPressed();

        // Attack
        if (gamepad.rightTrigger.IsPressed())
        {
            player.Attack(Player.AttackType.Shoot);
        }

        if (gamepad.xButton.isPressed)
        {
            player.Attack(Player.AttackType.Hit);
        }

        interact |= gamepad.bButton.wasPressedThisFrame;
        jump |= gamepad.aButton.wasPressedThisFrame;
        run |= gamepad.leftStickButton.isPressed;
        slide |= gamepad.rightShoulder.isPressed;
        sneak |= gamepad.leftShoulder.isPressed;

        // Item selection and ui navigation
        if (gamepad.dpad.left.wasPressedThisFrame)
        {
            uiDirection.x -= 1;
            itemID = 0;
        }
        if (gamepad.dpad.up.wasPressedThisFrame)
        {
            uiDirection.y += 1;
            itemID = 1;
        }
        if (gamepad.dpad.right.wasPressedThisFrame)
        {
            uiDirection.x += 1;
            itemID = 2;
        }
        if (gamepad.dpad.down.wasPressedThisFrame)
        {
            uiDirection.y -= 1;
            itemID = 3;
        }

        // from gameplay, open inventory using y-button
        toggleUI |= gamepad.yButton.wasPressedThisFrame;

        // from inventory/upgrades, switch using shoulder buttons
        switchUI |= gamepad.rightShoulder.wasPressedThisFrame;
        switchUI |= gamepad.leftShoulder.wasPressedThisFrame;

        uiSelect |= gamepad.aButton.wasPressedThisFrame;
    }

    void KeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        escape = keyboard.escapeKey.isPressed;

        // Movement
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            direction.y -= 1;
        }
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            direction.x -= 1;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            direction.x += 1;
        }
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            direction.y += 1;
        }

        interact |= keyboard.fKey.wasPressedThisFrame;
        jump |= keyboard.spaceKey.wasPressedThisFrame;
        run |= keyboard.leftShiftKey.isPressed;
        slide |= keyboard.leftAltKey.isPressed;
        sneak |= keyboard.leftCtrlKey.isPressed;

        // Item selection
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            itemID = 0;
        }
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            itemID = 1;
        }
        if (keyboard.digit3Key.wasPressedThisFrame)
        {
            itemID = 2;
        }
        if (keyboard.digit4Key.wasPressedThisFrame)
        {
            itemID = 3;
        }


        // ui
        if (keyboard.sKey.wasPressedThisFrame || keyboard.downArrowKey.wasPressedThisFrame)
        {
            uiDirection.y -= 1;
        }
        if (keyboard.aKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame)
        {
            uiDirection.x -= 1;
        }
        if (keyboard.dKey.wasPressedThisFrame || keyboard.rightArrowKey.wasPressedThisFrame)
        {
            uiDirection.x += 1;
        }
        if (keyboard.wKey.wasPressedThisFrame || keyboard.upArrowKey.wasPressedThisFrame)
        {
            uiDirection.y += 1;
        }

        // from inventory/upgrades, switch using tab
        switchUI  |= keyboard.tabKey.wasPressedThisFrame;

        upgrades  |= keyboard.uKey.wasPressedThisFrame;
        inventory |= keyboard.eKey.wasPressedThisFrame;

        uiSelect  |= keyboard.spaceKey.wasPressedThisFrame;
    }

    void MouseInput()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            return;
        }

        // Camera
        rotation += mouse.delta.ReadValue() * mouseSensitivity;
        aim |= mouse.rightButton.isPressed;

        // Attack
        if (mouse.leftButton.isPressed)
        {
            player.Attack();
        }

        if (mouse.scroll.down.IsPressed() || mouse.scroll.up.IsPressed())
        {
            player.ChangeAttack();
        }
    }
}

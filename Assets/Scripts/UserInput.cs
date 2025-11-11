using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    private Player player;

    private Vector2 direction;
    private Vector2 rotation;

    private bool aim;
    private bool interact;
    private bool jump;
    private bool run;
    private bool slide;
    private bool sneak;

    private int itemID;

    public float mouseSensitivity = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        rotation = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Reset variables
        direction = Vector2.zero;

        aim = false;
        interact = false;
        jump = false;
        run = false;
        slide = false;
        sneak = false;

        itemID = -1;

        // Get inputs
        GamepadInput();
        KeyboardInput();
        MouseInput();

        // Apply inputs
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

    void GamepadInput()
    {
        Gamepad gamepad = Gamepad.current;

        if (gamepad == null)
        {
            return;
        }

        bool startButton = gamepad.startButton.isPressed;

        // Movement
        direction = gamepad.leftStick.ReadValue();

        // Camera
        rotation += gamepad.rightStick.ReadValue();
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

        interact |= gamepad.bButton.isPressed;
        jump |= gamepad.aButton.isPressed;
        run |= gamepad.leftStickButton.isPressed;
        slide |= gamepad.rightShoulder.isPressed;
        sneak |= gamepad.leftShoulder.isPressed;

        bool yButton = gamepad.yButton.isPressed;

        // Item selection
        if (gamepad.dpad.left.isPressed)
        {
            itemID = 0;
        }
        if (gamepad.dpad.up.isPressed)
        {
            itemID = 1;
        }
        if (gamepad.dpad.right.isPressed)
        {
            itemID = 2;
        }
        if (gamepad.dpad.down.isPressed)
        {
            itemID = 3;
        }
    }

    void KeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        bool escape = keyboard.escapeKey.isPressed;

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

        interact |= keyboard.fKey.isPressed;
        jump |= keyboard.spaceKey.isPressed;
        run |= keyboard.leftShiftKey.isPressed;
        slide |= keyboard.leftAltKey.isPressed;
        sneak |= keyboard.leftCtrlKey.isPressed;

        // Item selection
        if (keyboard.digit1Key.isPressed)
        {
            itemID = 0;
        }
        if (keyboard.digit2Key.isPressed)
        {
            itemID = 1;
        }
        if (keyboard.digit3Key.isPressed)
        {
            itemID = 2;
        }
        if (keyboard.digit4Key.isPressed)
        {
            itemID = 3;
        }
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

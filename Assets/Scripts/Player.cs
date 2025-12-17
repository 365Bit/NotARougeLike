using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(PlayerStats)), DisallowMultipleComponent]
public class Player : MonoBehaviour
{
    // Components
    private Transform cameraTransform;
    private CharacterController characterController;
    private InteractionArea interactArea;
    private Camera playerCamera;

    private Transform leftShoulderTransform;
    private Transform rightShoulderTransform;

    // Enumerations
    public enum AttackType
    {
        Hit,
        Shoot
    }

    enum HitState
    {
        Idle,
        Return,
        Swing,
        Strike
    }

    private AttackType attackType;
    private HitState hitState;

    private float gravity = -9.81f;

    private bool grounded;
    private bool run;
    private bool slide;
    private bool sneak;

    private Vector3 motion;
    private Vector3 scale;

    private float currentFOV;
    private float currentHealth;
    private float currentMana;
    private float currentStamina;

    private float defaultYScale;
    private float fireCooldown;

    private float hitCooldown;
    private float hitTime;

    private Vector3 restRotation = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 swingRotation = new Vector3(-130.0f, 0.0f, 0.0f);
    private Vector3 strikeRotation = new Vector3(-30.0f, 0.0f, 0.0f);

    private Quaternion startRotation;
    private Quaternion endRotation;

    private float slideCooldown;
    private float slideTime;

    [Header("Camera")]
    public float normalFOV = 60.0f;
    public float aimFOV = 30.0f;
    public float zoomSpeed = 10.0f;

    [Header("Combat")]
    public Projectile[] projectiles;
    public Weapon weapon;
    public PlayerHitZone hitZone;

    private Inventory inventory;
    private ItemDefinitions itemDefinitions;
    private PlayerStats stats;

    public bool isDead = false;
    private bool opponentGotHit;

    void Awake()
    {
        stats = GetComponent<PlayerStats>();
        inventory = GetComponent<Inventory>();
        itemDefinitions = GameObject.Find("Definitions").GetComponent<ItemDefinitions>();

        GameObject mainCamera = GameObject.Find("Main Camera");

        playerCamera = mainCamera.GetComponent<Camera>();
        cameraTransform = mainCamera.GetComponent<Transform>();
        characterController = GetComponent<CharacterController>();
        interactArea = GameObject.Find("Interaction Area").GetComponent<InteractionArea>();

        leftShoulderTransform = GameObject.Find("Left Shoulder").GetComponent<Transform>();
        rightShoulderTransform = GameObject.Find("Right Shoulder").GetComponent<Transform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackType = AttackType.Shoot;
        hitState = HitState.Idle;

        currentHealth = stats.maxHealth;
        currentMana = stats.maxMana;
        currentStamina = stats.maxStamina;

        fireCooldown = 0.0f;
        hitCooldown = 0.0f;
        hitTime = 0.0f;

        slideCooldown = 0.0f;
        slideTime = 0.0f;

        defaultYScale = transform.localScale.y;

        weapon.gameObject.SetActive(false);
        hitZone.gameObject.SetActive(false);
        opponentGotHit = false;

        if (!RunData.Instance.Initialized)
            RunData.Instance.NewRun();
    }

    // Update is called once per frame
    void Update()
    {
        grounded = characterController.isGrounded;
        scale = transform.localScale;

        scale.y = slide ? defaultYScale * 0.5f : sneak ? defaultYScale * 0.75f : defaultYScale;
        transform.localScale = scale;

        if (!grounded)
        {
            motion.y += gravity * Time.deltaTime;
        }
        else
        {
            motion.y = 0.0f;
        }

        if (fireCooldown > 0.0f)
        {
            fireCooldown -= Time.deltaTime;
            if (fireCooldown < 0.0f)
            {
                fireCooldown = 0.0f;
            }
        }

        if (hitCooldown > 0.0f)
        {
            hitCooldown -= Time.deltaTime;
            if (hitCooldown < 0.0f)
            {
                hitCooldown = 0.0f;
            }
        }

        if (hitState != HitState.Idle)
        {
            hitTime += Time.deltaTime;

            float duration = hitState == HitState.Swing ? stats.swingDuration :
                             hitState == HitState.Strike ? stats.strikeDuration : stats.returnDuration;

            float ratio = Mathf.Clamp(hitTime / duration, 0.0f, 1.0f);

            rightShoulderTransform.localRotation = Quaternion.Slerp(startRotation, endRotation, ratio);

            if (ratio >= 1.0f)
            {
                switch (hitState)
                {
                    case HitState.Swing:
                        hitState = HitState.Strike;
                        hitTime = 0.0f;

                        startRotation = Quaternion.Euler(swingRotation);
                        endRotation = Quaternion.Euler(strikeRotation);
                        break;
                    case HitState.Strike:
                        hitState = HitState.Return;
                        hitTime = 0.0f;

                        startRotation = Quaternion.Euler(strikeRotation);
                        endRotation = Quaternion.Euler(restRotation);
                        hitZone.gameObject.SetActive(true);
                        hitZone.SetDamage(20.0f);
                        break;
                    case HitState.Return:
                        hitState = HitState.Idle;
                        hitCooldown = 1.0f / stats.hitRate;

                        weapon.gameObject.SetActive(false);
                        hitZone.gameObject.SetActive(false);
                        opponentGotHit = false;
                        break;
                    default:
                        Debug.LogError("Unknown Hit State: " + hitState);
                        return;
                }
            }
        }

        if (slide)
        {
            slideTime -= Time.deltaTime;

            if (slideTime <= 0.0f)
            {
                // End slide
                slide = false;
                slideCooldown = 1.0f;
                slideTime = 0.0f;
            }
        }
        else if (slideCooldown > 0.0f)
        {
            slideCooldown -= Time.deltaTime;

            if (slideCooldown < 0.0f)
            {
                slideCooldown = 0.0f;
            }
        }

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, currentFOV, zoomSpeed * Time.deltaTime);

        if (characterController.enabled)
            characterController.Move(motion * Time.deltaTime);

        RegenerateHealth();
        RegenerateMana();
        RegenrerateStamina();
    }

    public void Aim(bool value)
    {
        if (value)
        {
            currentFOV = aimFOV;
        }
        else
        {
            currentFOV = normalFOV;
        }
    }

    public void Attack(AttackType? type = null)
    {
        // Use current attack type if no type is specified
        attackType = type ?? attackType;

        switch (attackType)
        {
            case AttackType.Hit:
                Hit();
                break;
            case AttackType.Shoot:
                Shoot();
                break;
            default:
                Debug.LogError("Unknown Attack Type: " + attackType);
                break;
        }
    }

    public void ChangeAttack()
    {
        if (attackType == AttackType.Hit)
        {
            attackType = AttackType.Shoot;
        }
        else
        {
            attackType = AttackType.Hit;
        }
    }

    private void Die()
    {
        // TODO
        isDead = true;
        characterController.enabled = false;
        UIManager.Instance.SwitchToDeathScreen();
    }

    public void StartGame()
    {
        isDead = false;
        characterController.enabled = true;
        currentHealth = stats.maxHealth;
        currentMana = stats.maxMana;
        currentStamina = stats.maxStamina;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public float GetMana()
    {
        return currentMana;
    }

    public float GetStamina()
    {
        return currentStamina;
    }

    void Hit()
    {
        if (hitCooldown > 0.0f || hitState != HitState.Idle)
        {
            return;
        }

        weapon.gameObject.SetActive(true);
        weapon.SetDamage(20.0f);

        hitState = HitState.Swing;
        hitTime = 0.0f;

        startRotation = rightShoulderTransform.localRotation;
        endRotation = Quaternion.Euler(swingRotation);
    }


    public bool RaycastInteractible(out RaycastHit hit) {
        Vector3 position = cameraTransform.position + cameraTransform.forward * 0.1f;
        Debug.DrawRay(position, cameraTransform.forward, Color.white, 1.0f);
        float interactionDistance = 10f;
        return Physics.Raycast(position, cameraTransform.forward, out hit, interactionDistance, ~LayerMask.GetMask("Player"));
    }

    // returns true on successfull interaction
    public bool InteractWith(Transform transform) {
        if (transform.gameObject.TryGetComponent<ShopItemSlot>(out ShopItemSlot slot)) {
            if (inventory.currency[Currency.Gold] < slot.item.cost) return false;
            inventory.currency[Currency.Gold] -= slot.item.cost;
            inventory.container.AddItem(slot.item, slot.count);
            slot.shop.RemoveItem(slot.Index);
            return true;
        } else if (transform.gameObject.TryGetComponent<DroppedItem>(out DroppedItem item)) {
            inventory.container.AddItem(item.item, item.count);
            item.Disable();
            return true;
        } else if (transform.gameObject.TryGetComponent<TrapDoor>(out TrapDoor door)){
            door.Interact();
            return true;
        }
        return false;
    }

    public void Interact()
    {
        bool done = false;

        // first check interaction area
        if (interactArea.triggerObject != null)
        {
            done = InteractWith(interactArea.triggerObject.transform);
        }

        // then try raycasting
        if (!done && RaycastInteractible(out RaycastHit hit)) {
            done = InteractWith(hit.transform);
        }
    }

    public bool isGrounded()
    {
        return grounded;
    }

    public bool isRunning()
    {
        return run;
    }

    public bool isSliding()
    {
        return slide;
    }

    public bool isSneaking()
    {
        return sneak;
    }

    public void Jump()
    {
        if (grounded && currentStamina >= stats.jumpConsRate)
        {
            currentStamina -= stats.jumpConsRate;
            motion.y = Mathf.Sqrt(stats.jumpHeight * -2f * gravity);
        }
    }

    public void Move(Vector2 direction)
    {
        Vector3 movement = transform.right * direction.x + transform.forward * direction.y;
        movement.Normalize();

        float speed = stats.walkSpeed;

        if (run && currentStamina >= stats.runConsRate)
        {
            currentStamina -= stats.runConsRate * Time.deltaTime;
            speed = stats.runSpeed;
        }
        else if (sneak)
        {
            speed = stats.sneakSpeed;
        }

        if (slide)
        {
            speed += stats.slideSpeed * (slideTime / stats.maxSlideTime);
        }

        motion.x = movement.x * speed;
        motion.z = movement.z * speed;

        float ratio = Time.deltaTime * 5.0f;

        if (hitState == HitState.Idle && (direction.x != 0.0f || direction.y != 0.0f) && !slide)
        {
            float amplitude = 2.0f * speed;
            float frequency = 5.0f;

            float wave = Mathf.Sin(Time.time * frequency) * amplitude;

            leftShoulderTransform.localRotation = Quaternion.Slerp(leftShoulderTransform.localRotation,
                Quaternion.Euler(wave, 0.0f, 0.0f), ratio);
            rightShoulderTransform.localRotation = Quaternion.Slerp(rightShoulderTransform.localRotation,
                Quaternion.Euler(-wave, 0.0f, 0.0f), ratio);
        }
        else
        {
            leftShoulderTransform.localRotation = Quaternion.Slerp(leftShoulderTransform.localRotation,
                Quaternion.Euler(0.0f, 0.0f, 0.0f), ratio);
            rightShoulderTransform.localRotation = Quaternion.Slerp(rightShoulderTransform.localRotation,
                Quaternion.Euler(0.0f, 0.0f, 0.0f), ratio);
        }
    }

    void RegenerateHealth()
    {
        if (currentHealth < stats.maxHealth)
        {
            float health = currentHealth + stats.healthRegRate * Time.deltaTime;
            currentHealth = Mathf.Min(health, stats.maxHealth);
        }
    }

    void RegenerateMana()
    {
        if (currentMana < stats.maxMana)
        {
            float mana = currentMana + stats.manaRegRate * Time.deltaTime;
            currentMana = Mathf.Min(mana, stats.maxMana);
        }
    }

    void RegenrerateStamina()
    {
        if (currentStamina < stats.maxStamina)
        {
            float rate = stats.staminaRegRate;

            if (motion.x >= -0.1f && motion.x <= 0.1f &&
                motion.y >= -0.1f && motion.y <= 0.1f &&
                motion.z >= -0.1f && motion.z <= 0.1f)
            {
                // Increase regeneration rate when idle
                rate *= 1.5f;
            }

            float stamina = currentStamina + rate * Time.deltaTime;
            currentStamina = Mathf.Min(stamina, stats.maxStamina);
        }
    }

    public void Run(bool value)
    {
        if (value)
        {
            // Disable sneak when running
            sneak = false;
        }

        run = value;
    }

    public void Rotate(Vector2 rotation)
    {
        // Player
        transform.localRotation = Quaternion.AngleAxis(rotation.x, Vector3.up);

        // Camera
        float angle = Mathf.Clamp(rotation.y * 2, -90, 90.0f);
        cameraTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.left);
    }

    void Shoot()
    {
        ItemContainer items = inventory.container;
        int bowAmmoSlot = items.GetSlotContaining(itemDefinitions[3], 1);

        Debug.Log("Bow Ammo Slot: " + bowAmmoSlot);
        items.PrintState();

        if (fireCooldown > 0.0f || projectiles.Length == 0 || bowAmmoSlot == -1)
        {
            return;
        }

        items.ConsumeItem(bowAmmoSlot);

        Vector3 position = cameraTransform.position + transform.forward * 1.0f;
        Quaternion rotation = cameraTransform.rotation;

        Projectile instance = Instantiate(projectiles[0], position, rotation);
        instance.name = projectiles[0].name;
        instance.SetDamage(10.0f);

        fireCooldown = 1.0f / stats.fireRate;
    }

    public void Slide(bool value)
    {
        if (value && currentStamina >= stats.slideConsRate &&
            grounded && !slide && slideCooldown == 0.0f)
        {
            currentStamina -= stats.slideConsRate;

            slide = true;
            slideTime = stats.maxSlideTime;
        }
        else if (!value && slide)
        {
            slide = false;
        }
    }

    public void Sneak(bool value)
    {
        if (value)
        {
            // Disable run when sneaking
            run = false;
        }

        sneak = value;
    }

    public void UseItem(int itemID)
    {
        ItemContainer inv = GetComponent<Inventory>().container;
        ItemSlot slot = inv[itemID];
        ItemDefinition item = slot.storedItem;
        int count = slot.count;

        if (item == null) return;

        switch (item.name)
        {
            case "health_fruit": // Health Fruit
                currentHealth += 30.0f;
                currentHealth = Mathf.Clamp(currentHealth, 0.0f, stats.maxHealth);
                break;
            case "mana_fruit": // Mana Fruit
                currentMana += 10.0f;
                currentMana = Mathf.Clamp(currentMana, 0.0f, stats.maxMana);
                break;
            case "stamina_fruit": // Stamina Fruit
                currentStamina += 20.0f;
                currentStamina = Mathf.Clamp(currentStamina, 0.0f, stats.maxStamina);
                break;
            default:
                break;
        }

        inv.ConsumeItem(itemID);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0.0f)
        {
            currentHealth = 0.0f;
            Die();
        }
    }

    public void SetOpponentGotHit(bool value)
    {
        opponentGotHit = value;
    }

    public bool GetOpponentGotHit()
    {
        return opponentGotHit;
    }
}

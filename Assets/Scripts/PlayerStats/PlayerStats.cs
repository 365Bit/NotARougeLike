using UnityEngine;



/// stores player stats depending on player upgrades and items
[RequireComponent(typeof(Inventory)), RequireComponent(typeof(PlayerUpgrades)), DisallowMultipleComponent]
public class PlayerStats : MonoBehaviour
{
    private StatScalingDefinitions def;
    private PlayerUpgrades upgrades;
    private Inventory inventory;

    public void Start() {
        def = GameObject.Find("Definitions").GetComponent<StatScalingDefinitions>();
        upgrades = GetComponent<PlayerUpgrades>();
        inventory = GetComponent<Inventory>();
        Recompute();
    }

    public void Update() { 
        // TODO: only when necessary
        Recompute();
    }

    // 
    private float Compute(StatKey stat) {
        // TODO: check inventory for items with stat modifiers
        return def[stat].ComputeFrom(upgrades);
    }

    // 
    public void Recompute() {
        fireRate = Compute(StatKey.FireRate); 
        hitRate = Compute(StatKey.HitRate); 
        swingDuration = Compute(StatKey.SwingDuration); 
        strikeDuration = Compute(StatKey.StrikeDuration); 
        returnDuration = Compute(StatKey.ReturnDuration); 

        maxMana = Compute(StatKey.MaxMana); 
        manaRegRate = Compute(StatKey.ManaRegRate); 

        runSpeed = Compute(StatKey.RunSpeed); 
        walkSpeed = Compute(StatKey.WalkSpeed); 
        sneakSpeed = Compute(StatKey.SneakSpeed); 

        maxSlideTime = Compute(StatKey.MaxSlideTime); 
        slideSpeed = Compute(StatKey.SlideSpeed); 

        maxHealth = Compute(StatKey.MaxHealth); 
        healthRegRate = Compute(StatKey.HealthRegRate); 

        jumpHeight = Compute(StatKey.JumpHeight); 

        maxStamina = Compute(StatKey.MaxStamina); 
        staminaRegRate = Compute(StatKey.StaminaRegRate); 
        runConsRate = Compute(StatKey.RunConsRate); 
        slideConsRate = Compute(StatKey.SlideConsRate); 
        jumpConsRate = Compute(StatKey.JumpConsRate); 
    }

    // 
    [Header("Combat")]
    public float fireRate;
    public float hitRate;
    public float swingDuration;
    public float strikeDuration;
    public float returnDuration;

    [Header("Mana")]
    public float maxMana;
    public float manaRegRate;

    [Header("Movement")]
    public float runSpeed;
    public float walkSpeed;
    public float sneakSpeed;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideSpeed;

    [Header("Health")]
    public float maxHealth;
    public float healthRegRate;

    [Header("Jump")]
    public float jumpHeight;

    [Header("Stamina")]
    public float maxStamina;
    public float staminaRegRate;
    public float runConsRate;
    public float slideConsRate;
    public float jumpConsRate;
}

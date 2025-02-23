using Godot;
using System;

public partial class PlayerStatsHandler : Node
{
    private CustomSignals customSignals;
    private PlayerMoveAndLookComponent PlayerMoveComponent;
    [Export] private float MaxHealth;
    [Export] private float MaxStamina;
    private TextureProgressBar HealthBar;
    private TextureProgressBar StaminaBar;
    private Label SpeedLabel;
    private Label AmmoLabel;
    private CharacterBody3D Parent;
    private RayCast3D PlayerRay;
    private Control PlayerUI;
    public float CurrentHealth;
    public float CurrentStamina;
    public int AmmoCount;

    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        PlayerMoveComponent = Parent.GetNode<PlayerMoveAndLookComponent>("PlayerMoveAndLookComponent");
        PlayerUI = Parent.GetNode<Control>("PlayerUI");
        HealthBar = PlayerUI.GetNode<TextureProgressBar>("PlayerHealthBar");
        StaminaBar = PlayerUI.GetNode<TextureProgressBar>("PlayerStaminaBar");
        AmmoLabel = PlayerUI.GetNode<Label>("AmmoLabel");
        SpeedLabel = PlayerUI.GetNode<Label>("SpeedLabel");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        PlayerRay = Parent.GetNode<RayCast3D>("Head/GrabRaycast");
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
        AmmoCount = 0;
        customSignals.AmmoPickup += OnAmmoPickup;
        customSignals.PlayerDamage += OnPlayerDamage;
    }

    public override void _Process(double delta)
    {
        HandleHealth();
        HandleStamina((float)delta);
        HandleAmmo();
        HandleSpeed();
    }

    //Handle player ammo
    private void HandleAmmo()
    {
        //Update UI
        AmmoLabel.Text = "Ammo: " + AmmoCount;
    }

    //Handle player health
    private void HandleHealth()
    {
        //Update UI
        HealthBar.Value = CurrentHealth;
    }

    //Handle player stamina
    private void HandleStamina(float delta)
    {
        //Sprinting stamina drain
        if (PlayerMoveComponent.Sprinting)
        {
            CurrentStamina -= 50 * delta;
        }
        
        //Stamina regen
        if (CurrentStamina < MaxStamina && !PlayerMoveComponent.Sprinting)
        {
            CurrentStamina += 10 * delta;
        }

        //Update UI
        StaminaBar.Value = CurrentStamina;
    }

    //Handle player speed
    private void HandleSpeed()
    {
        //Update UI
        SpeedLabel.Text = "SPEED: " + Mathf.Round(Parent.Velocity.Length());
    }

    //Handle ammo pickup
    private void OnAmmoPickup(int ammoAmount)
    {
        AmmoCount += ammoAmount;
    }

    //Handle player damage
    private void OnPlayerDamage(float damageAmount)
    {
        CurrentHealth -= damageAmount;
    }
}

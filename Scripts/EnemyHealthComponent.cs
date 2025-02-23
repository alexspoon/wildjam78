using Godot;
using System;

public partial class EnemyHealthComponent : Node
{
    private CustomSignals customSignals;
    private CharacterBody3D Parent;
    private Node3D ParticleInstance;
    [Export] private PackedScene BloodExplosion;
    [Export] private float MaxHealth;
    private float CurrentHealth;

    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        customSignals.EnemyDamage += OnEnemyDamage;
        CurrentHealth = MaxHealth;
    }
    
    //Handle taking damage
    private void OnEnemyDamage(float damageAmount, CharacterBody3D enemy)
    {
        //If enemy hit is this enemy, take damage
        if (enemy == Parent)
        {
            CurrentHealth -= damageAmount;
            if (CurrentHealth <= 0)
                Kill();
            //GD.Print("enemy took " + damageAmount + " damage.");
            //GD.Print("enemy has " + CurrentHealth + " health remaining.");
        }
    }

    //Kill enemy
    private void Kill()
    {
        //GD.Print("killed");
        ParticleInstance = BloodExplosion.Instantiate() as Node3D;
        Parent.AddSibling(ParticleInstance);
        ParticleInstance.GlobalPosition = Parent.GlobalPosition;
        Parent.QueueFree();
    }
}

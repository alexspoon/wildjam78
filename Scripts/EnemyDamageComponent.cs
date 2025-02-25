using Godot;
using System;
using System.ComponentModel;

public partial class EnemyDamageComponent : Node
{
    private CharacterBody3D Parent;
    private Timer DamageCooldown;
    private CustomSignals customSignals;
    [Export] private float PlayerDamage;
    
    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        DamageCooldown = GetNode<Timer>("DamageCooldown");
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
    }

    public override void _Process(double delta)
    {
        OnPlayerDamage();
    }

    private void OnPlayerDamage()
    {
        if (Parent.GetSlideCollisionCount() == 0)
            return;
        
        if (Parent.GetSlideCollision(0).GetCollider() is not PlayerCharacter)
            return;
        
        if (DamageCooldown.TimeLeft == 0)
        {
            DamageCooldown.Start();
            customSignals.EmitSignal(nameof(customSignals.PlayerDamage), PlayerDamage);
        }
            
    }
}

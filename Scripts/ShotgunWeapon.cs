using Godot;
using System;
using System.Collections.Generic;

public partial class ShotgunWeapon : Node3D
{
    [Signal] public delegate void FiredEventHandler();
    [Export] private float ShotgunDamage;
    private CustomSignals customSignals;
    private CharacterBody3D Parent;
    private Marker3D MuzzlePosition;
    private AnimationPlayer ShotgunAnimation;
    private RayCast3D MuzzleRaycast;
    private PlayerStatsHandler PlayerStats;
    private Camera3D PlayerCamera;
    private SpringArm3D AimSpringArm;
    private Marker3D AimPosition;
    
    public override void _Ready()
    {
        Parent = GetParent().GetParent().GetParent<CharacterBody3D>();
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        PlayerCamera = Parent.GetNode<Camera3D>("Head/Camera");
        AimSpringArm = Parent.GetNode<SpringArm3D>("Head/AimSpringArm");
        AimPosition = AimSpringArm.GetNode<Marker3D>("AimPosition");
        MuzzlePosition = GetNode<Marker3D>("MuzzlePosition");
        MuzzleRaycast = MuzzlePosition.GetNode<RayCast3D>("MuzzleRaycast");
        ShotgunAnimation = GetNode<AnimationPlayer>("ShotgunAnimation");
        PlayerStats = Parent.GetNode<PlayerStatsHandler>("PlayerStatsHandler");
        Fired += OnShotgunFire;
    }

    public override void _Process(double delta)
    {
        HandleShooting();
        ShotgunLook();
    }
    
    private void ShotgunLook()
    {
       LookAt(AimPosition.GlobalPosition);
    }
    
    private void HandleShooting()
    {
        
        
        if (PlayerStats.AmmoCount == 0)
            return;

        if (Input.IsActionPressed("inputLeftMouse") && !ShotgunAnimation.IsPlaying())
        {
            EmitSignalFired();
        }
    }

    private void OnShotgunFire()
    {
        ShotgunAnimation.Play("ShotgunFire");
        PlayerStats.AmmoCount--;

        if (!MuzzleRaycast.IsColliding())
            return;

        if (MuzzleRaycast.GetCollider() is not CharacterBody3D)
            return;

        //GD.Print("enemy hit");
        var hitEnemy = MuzzleRaycast.GetCollider() as CharacterBody3D;
        customSignals.EmitSignal(nameof(customSignals.EnemyDamage), ShotgunDamage, hitEnemy);
    }
}

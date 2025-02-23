using Godot;
using System;

public partial class ShotgunAmmoBoxPickup : RigidBody3D
{
    private CustomSignals customSignals;
    private Node3D Root;
    private CharacterBody3D Player;
    private RayCast3D PlayerRay;
    private MeshInstance3D CurrentMesh;
    private Material MeshMaterial;
    private Material OutlineShader;
    private Resource ShaderInstance;
    [Export] private int AmmoCount;

    public override void _Ready()
    {
        customSignals = GetNode<CustomSignals>("/root/CustomSignals");
        Root = GetTree().GetRoot().GetNode<Node3D>("Main");
        Player = Root.GetNode<CharacterBody3D>("PlayerCharacter");
        PlayerRay = Player.GetNode<RayCast3D>("Head/GrabRaycast");
        CurrentMesh = GetNode<MeshInstance3D>("shotgun_ammo_22/shotgun_ammo_2");
        MeshMaterial = CurrentMesh.GetSurfaceOverrideMaterial(0);
        OutlineShader = MeshMaterial.GetNextPass();
    }

    public override void _Process(double delta)
    {
        Pickup();
    }

    private void Pickup()
    {
        OutlineShader.Set("shader_parameter/outline_width", 0);
        
        if (!PlayerRay.IsColliding())
            return;
        
        if (PlayerRay.GetCollider() != this)
            return;
        
        OutlineShader.Set("shader_parameter/outline_width", 1);
        
        if (PlayerRay.GetCollider() == this && Input.IsActionJustPressed("inputE"))
        {
            customSignals.EmitSignal(nameof(customSignals.AmmoPickup), AmmoCount);
            QueueFree();
        }
    }
}


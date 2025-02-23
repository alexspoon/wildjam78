using Godot;
using System;

public partial class ShotgunPickup : RigidBody3D
{
    private Node3D Root;
    private CharacterBody3D Player;
    private RayCast3D PlayerRay;
    private Node3D PlayerHandPoint;
    private MeshInstance3D CurrentMesh;
    private Material MeshMaterial;
    private Material OutlineShader;

    public override void _Ready()
    {
        Root = GetTree().GetRoot().GetNode<Node3D>("Main");
        Player = Root.GetNode<CharacterBody3D>("PlayerCharacter");
        PlayerRay = Player.GetNode<RayCast3D>("Head/GrabRaycast");
        PlayerHandPoint = Player.GetNode<Node3D>("Head/HandPoint");
        CurrentMesh = GetNode<MeshInstance3D>("ShotgunMesh/shotgun_1");
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
            var shotgunWeapon = ResourceLoader.Load<PackedScene>("res://Scenes/ShotgunWeapon.tscn").Instantiate();
            PlayerHandPoint.AddChild(shotgunWeapon);
            QueueFree();
        }
    }
}

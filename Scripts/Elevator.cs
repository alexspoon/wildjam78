using Godot;
using System;

public partial class Elevator : Node3D
{
    //Class variables
    private Node3D Root;
    private CharacterBody3D Player;
    private RayCast3D PlayerRay;
    private MeshInstance3D ElevatorButton;
    private AnimationPlayer ElevatorAnimation;
    private Area3D ClickArea;
    
    public override void _Ready()
    {
        Root = GetTree().GetRoot().GetNode<Node3D>("Main");
        Player = Root.GetNode<CharacterBody3D>("PlayerCharacter");
        PlayerRay = Player.GetNode<RayCast3D>("Head/GrabRaycast");
        ElevatorButton = GetNode<MeshInstance3D>("elevator_1/elevator_buttons_1");
        ElevatorAnimation = GetNode<AnimationPlayer>("ElevatorAnimation");
        ClickArea = ElevatorButton.GetNode<Area3D>("Area3D");
    }

    public override void _Process(double delta)
    {
        ButtonInteraction();
    }

    private void ButtonInteraction()
    {
        if (!PlayerRay.IsColliding())
            return;

        if (PlayerRay.GetCollider() != ClickArea)
            return;
        
        if (PlayerRay.GetCollider() == ClickArea && Input.IsActionJustPressed("inputE") && !ElevatorAnimation.IsPlaying())
        {
            ElevatorAnimation.Play("ElevatorOpen");
        }
    }
}

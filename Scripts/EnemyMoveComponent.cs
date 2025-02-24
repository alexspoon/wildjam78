using Godot;
using System;

public partial class EnemyMoveComponent : Node
{
    private CharacterBody3D Player;
    private Node3D PlayerHead;
    private CharacterBody3D Parent;
    private Timer ChaseTimer;
    private PathFollow3D PlayerChasePath;
    private Vector3 ChasePoint;
    private Vector3 ChaseDir;
    private Vector3 TargetVelocity;
    [Export] private float ChaseSpeed;

    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        Player = GetTree().GetRoot().GetNode("Main/PlayerCharacter") as CharacterBody3D;
        PlayerHead = Player.GetNode<Node3D>("Head");
        PlayerChasePath = Player.GetNode<PathFollow3D>("ChasePath/ChasePathFollow");
        ChaseTimer = GetNode<Timer>("ChaseTimer");
        ChaseTimer.Timeout += ChasePointReset;

    }

    public override void _Process(double delta)
    {
        HandleChase(delta);
    }

    private void HandleChase(double delta)
    {
        if (Parent.GlobalPosition.DistanceTo(ChasePoint) < 5)
        {
            ChaseDir = (Player.GlobalPosition - Parent.GlobalPosition).Normalized();
            Parent.LookAt(PlayerHead.GlobalPosition);
            TargetVelocity = ChaseDir * ChaseSpeed * 0.5f * (float)delta;
        }
        else
        {
            ChaseDir = (ChasePoint - Parent.GlobalPosition).Normalized();
            Parent.LookAt(ChasePoint);
            TargetVelocity = ChaseDir * ChaseSpeed * (float)delta;
        }
        
        Parent.Velocity = TargetVelocity;
        
        Parent.MoveAndSlide();
    }

    private void ChasePointReset()
    {
        PlayerChasePath.ProgressRatio = GD.Randf();
        GD.Print(Parent.GlobalPosition.DistanceTo(ChasePoint));
        ChasePoint = PlayerChasePath.GlobalPosition;
    }
}

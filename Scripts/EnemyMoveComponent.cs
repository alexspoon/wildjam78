using Godot;
using System;

public partial class EnemyMoveComponent : Node
{
    private CharacterBody3D Player;
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
        ChaseDir = ChasePoint - Parent.GlobalPosition;
        ChaseDir = ChaseDir.Normalized();
        TargetVelocity = ChaseDir * ChaseSpeed * (float)delta;
        Parent.Velocity = TargetVelocity;
        Parent.MoveAndSlide();
    }

    private void ChasePointReset()
    {
        PlayerChasePath.ProgressRatio = GD.Randf();
        ChasePoint = PlayerChasePath.GlobalPosition;
    }
}

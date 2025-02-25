using Godot;
using System;

public partial class PlayerCharacter : CharacterBody3D
{
    [Export] private PackedScene TestEnemy;
    private Node EnemyPool;
    private Marker3D AimPosition;
    private Node3D Root;
    
    public override void _Ready()
    {
        Root = GetTree().GetRoot().GetNode<Node3D>("Main");
        EnemyPool = Root.GetNode<Node>("EnemyPool");
        AimPosition = GetNode<Marker3D>("Head/AimSpringArm/AimPosition");
    }

    public override void _Process(double delta)
    {
        DebugEnemySpawn();
    }

    private void DebugEnemySpawn()
    {
        if (Input.IsActionJustPressed("inputR"))
        {
            var spawn = TestEnemy.Instantiate() as CharacterBody3D;
            EnemyPool.AddChild(spawn);
            spawn.GlobalPosition = AimPosition.GlobalPosition;
        }
    }

}


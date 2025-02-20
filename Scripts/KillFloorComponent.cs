using Godot;
using System;

public partial class KillFloorComponent : Node
{
    [Export] private PackedScene GameScene;
    
    private void KillEvent(Node3D node)
    {
        if (node is CharacterBody3D)
        {
            GetTree().ChangeSceneToPacked(GameScene);
        }
    }
}

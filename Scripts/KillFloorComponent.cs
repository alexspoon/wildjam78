using Godot;
using System;

public partial class KillFloorComponent : Node
{
    [Export] private SceneTransition TransitionLayer;
    private Node Root;
    private Node3D CurrentLevel;

    public override void _Ready()
    {
        TransitionLayer.Transitioned += SceneTransitionBegin;
        Root = GetTree().GetRoot().GetNode<Node3D>("Main");
        CurrentLevel = Root.GetNode<Node3D>("SpawnLevel");
    }

    private void KillEvent(Node3D node)
    {
        if (node is CharacterBody3D)
        {
            TransitionLayer.TransitionStart();
        }
    }

    private void SceneTransitionBegin()
    {
        var nextLevel = ResourceLoader.Load<PackedScene>("res://Scenes/GameScene.tscn").Instantiate();
        Root.AddChild(nextLevel);
        Root.CallDeferred(Node.MethodName.RemoveChild, CurrentLevel);
    }
}

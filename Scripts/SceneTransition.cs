using Godot;
using System;

public partial class SceneTransition : CanvasLayer
{
    [Signal] public delegate void TransitionedEventHandler();
    
    [Export] private AnimationPlayer TransitionPlayer;
    
    public override void _Ready()
    {
        TransitionPlayer.AnimationFinished += TransitionFinish;
    }

    public void TransitionStart()
    {
        TransitionPlayer.Play("StartFade");
    }

    private void TransitionFinish(StringName stringName)
    {
        if (stringName == "StartFade")
        {
            EmitSignalTransitioned();
            TransitionPlayer.Play("FinishFade");
        }
    }
}

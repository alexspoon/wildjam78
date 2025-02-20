using Godot;
using System;

public partial class PlayerMeleeComponent : Node
{
    private CharacterBody3D Parent;
    private AnimationPlayer PlayerAnimation;
    private bool Swinging;

    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        PlayerAnimation = Parent.GetNode<AnimationPlayer>("PlayerAnimation");
        PlayerAnimation.AnimationFinished += OnAnimationFinished;
        Swinging = false;
    }

    public override void _Process(double delta)
    {
        PlayAnimation();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("inputLeftMouse") && Swinging == false)
        {
            Swinging = true;
        }
    }

    private void PlayAnimation()
    {
        if (Swinging)
        {
            PlayerAnimation.Play("BeamSwing");
        }
    }
    
    private void OnAnimationFinished(StringName stringName)
    {
        Swinging = false;
    }
}

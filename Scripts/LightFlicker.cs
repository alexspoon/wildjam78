using Godot;
using System;

public partial class LightFlicker : OmniLight3D
{
    [Export] private AnimationPlayer ElevatorAnimation;
    [Export] private Area3D ElevatorArea;
    private Timer FlickerTimer;
    private RandomNumberGenerator RNG;
    private float RandValue;
    private bool IsActive;
    private bool AnimFinish;
    private bool Exited;
    
    public override void _Ready()
    {
        IsActive = true;
        RNG = new RandomNumberGenerator();
        RandValue = 0;
        FlickerTimer = GetChild<Timer>(0);
        FlickerTimer.Timeout += RandomizeEnergy;
        ElevatorArea.BodyExited += ElevatorExited;
        ElevatorAnimation.AnimationFinished += AnimationFinish;
        ElevatorAnimation.AnimationStarted += AnimationStart;
    }

    public override void _Process(double delta)
    {
        if (Exited && AnimFinish)
        {
            IsActive = false;
        }
        
        if (IsActive)
        {
            LightEnergy = Mathf.Lerp(LightEnergy, RandValue, 0.1f);
            LightVolumetricFogEnergy = Mathf.Lerp(LightVolumetricFogEnergy, RandValue / 4f, 0.1f);
        }
        else
        {
            LightEnergy = 0;
            LightVolumetricFogEnergy = 0;
            ShadowEnabled = false;
            FlickerTimer.OneShot = true;
        }
        
    }

    private void AnimationStart(StringName stringName)
    {
        AnimFinish = false;
    }
    
    private void AnimationFinish(StringName stringName)
    {
        AnimFinish = true;
    }
    
    private void ElevatorExited(Node body)
    {
        Exited = true;
    }
    
    private void RandomizeEnergy()
    {
        RandValue = RNG.RandfRange(1, 16);
    }
}

using Godot;
using System;

public partial class PlayerMoveAndLookComponent : Node
{
    //Class variables
    private CharacterBody3D Parent;
    private PlayerStatsHandler StatsHandler;
    
    //Move variables
    [Export] private float Gravity;                 //Recommended: 9.82
    [Export] private float JumpForce;               //Recommended: 7.5
    [Export] private float DefaultMoveSpeed;        //Recommended: 10
    [Export] private float MoveSpeed;               //Recommended: 10
    [Export] private float DefaultMoveAcceleration; //Recommended: 100
    [Export] private float MoveAcceleration;        //Recommended: 100
    [Export] private float MoveDrag;                //Recommended: 10
    [Export] private int MaxJumps;           
    private int JumpsLeft;
    private Vector3 InputDirection = Vector3.Zero;
    private Vector3 TargetVelocity = Vector3.Zero;
    
    //Move states
    public bool Walking;
    public bool Sprinting;
    public bool Jumping;
    public bool Crouching;
    
    //Move state active status
    public bool CanWalk;
    public bool CanSprint;
    public bool CanJump;
    public bool CanCrouch;
    
    //FOV variables
    [Export] private float DefaultFov;              //Recommended: 80
    [Export] private float SprintFov;               //Recommended: 10
    [Export] private float CrouchFov;               //Recommended: -10
    [Export] private float JumpFov;                 //Recommended: 5
    
    //Look variables
    [Export] private float MouseSensitivity;        //Recommended: 0.5
    private Node3D Head;
    private Camera3D Camera;
    private Basis ParentBasis;
    private Vector3 ParentBasisRotation;
    
    //Flashlight variables
    private SpotLight3D PlayerFlashlight;
    private bool FlashlightToggle;
    
    //Called at node initialization
    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        StatsHandler = Parent.GetNode<PlayerStatsHandler>("PlayerStatsHandler");
        Head = Parent.GetNode<Node3D>("Head");
        Camera = Head.GetNode<Camera3D>("Camera");
        PlayerFlashlight = Head.GetNode<SpotLight3D>("PlayerFlashlight");

        CanWalk = true;
        CanSprint = true;
        CanJump = true;
        CanCrouch = true;
    }

    //Called every frame
    public override void _Process(double delta)
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Move(delta);
        HandleFov(delta);
        HandleFlashlight();
        StateMachine();
    }

    //Handle player head rotation with mouse input
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var motion = (InputEventMouseMotion)@event;

            Parent.RotateY(-motion.Relative.X * MouseSensitivity * 0.01f);
            Head.RotateX(-motion.Relative.Y * MouseSensitivity * 0.01f);

            Head.Rotation = new Vector3((float)Mathf.Clamp(Head.Rotation.X, -Math.PI / 2, Math.PI / 2),
                Head.Rotation.Y, Head.Rotation.Z);
        }
    }

    //Handles player movement
    private void Move(double delta)
    {
        //Initialize variables
        Walking = false;
        // Jumping = false;
        InputDirection = Vector3.Zero;
        ParentBasis = Parent.GlobalTransform.Basis;
        ParentBasisRotation = ParentBasis.GetEuler();
        
        //Capture input
        if (Input.IsActionPressed("inputD"))
            InputDirection.X += 1.0f;
        
        if (Input.IsActionPressed("inputA"))
            InputDirection.X -= 1.0f;
        
        if (Input.IsActionPressed("inputS"))
            InputDirection.Z += 1.0f;
        
        if (Input.IsActionPressed("inputW"))
            InputDirection.Z -= 1.0f;
        
        //On input, set move direction to head forward facing direction
        if (InputDirection != Vector3.Zero)
        {
            Walking = true;
            ParentBasis = Basis.FromEuler(ParentBasisRotation);
            InputDirection = ParentBasis * InputDirection;
            InputDirection = InputDirection.Normalized();
            Parent.Rotation = ParentBasisRotation;
        }
        else Walking = false;
        
        //Apply sprint
        if (Input.IsActionPressed("inputShift") && InputDirection != Vector3.Zero && !Sprinting && CanSprint)
        {
            Sprinting = true;
        }
        else
        {
            Sprinting = false;
        }
        
        //Apply drag
        Vector3 targetVelocityWithoutY = new Vector3(TargetVelocity.X, 0, TargetVelocity.Z);
        Vector3 dragForce = -targetVelocityWithoutY * MoveDrag * (float)delta;
        TargetVelocity += dragForce;

        //Apply acceleration
        Vector3 deltaVelocity = InputDirection * MoveAcceleration * (float)delta;
        deltaVelocity = deltaVelocity.Normalized() * MathF.Min(deltaVelocity.Length(),
            MathF.Max(0, MoveSpeed - targetVelocityWithoutY.Dot(deltaVelocity.Normalized())));
        TargetVelocity += deltaVelocity;
        
        //If head touches ceiling, stop upward velocity
        if (Parent.IsOnCeiling())
            TargetVelocity.Y = 0;
        
        //Apply gravity
        if (!Parent.IsOnFloor())
            TargetVelocity.Y -= Gravity * (float)delta * 2f;
        
        

        //Reset amount of jumps left
        if (Parent.IsOnFloor())
        {
            Jumping = false;
            JumpsLeft = MaxJumps;
        }
        
        //Apply jump
        if (JumpsLeft > 0 && Input.IsActionJustPressed("inputSpace") && CanJump)
        {
            Jumping = true;
            JumpsLeft--;
            StatsHandler.CurrentStamina -= 15;
            TargetVelocity.Y = JumpForce;
        }
        
        //Apply movement to character
        Parent.Velocity = TargetVelocity;
        Parent.MoveAndSlide();
    }

    //Handles FOV functions
    private void HandleFov(double delta)
    {
        SprintFovUpdate((float)delta);
        JumpFovUpdate((float)delta);
    }
    
    //Handles sprint FOV lerp
    private void SprintFovUpdate(float delta)
    {
            if (Sprinting && InputDirection != Vector3.Zero)
            {
                Camera.Fov = Mathf.Lerp(Camera.Fov, DefaultFov + SprintFov, 5f * delta);
            }
            else Camera.Fov = Mathf.Lerp(Camera.Fov, DefaultFov, 3f * delta);
    }

    //Handles jump FOV lerp
    private void JumpFovUpdate(float delta)
    {
        if (Jumping)
        {
            Camera.Fov = Mathf.Lerp(Camera.Fov, DefaultFov + JumpFov, 3f * delta);
        }
        else Camera.Fov = Mathf.Lerp(Camera.Fov, DefaultFov, 7.5f * delta);
    }
    
    private void HandleFlashlight()
    {
        if (FlashlightToggle)
        {
            PlayerFlashlight.LightEnergy = 16;
            PlayerFlashlight.LightVolumetricFogEnergy = 8;
        }
        else
        {
            PlayerFlashlight.LightEnergy = 0;
            PlayerFlashlight.LightVolumetricFogEnergy = 0;
        }
        
        if (Input.IsActionJustPressed("inputF"))
        {
            FlashlightToggle = !FlashlightToggle;
            GD.Print(FlashlightToggle);
        }
    }

    private void HandleCrouch()
    {
        var crouchPos = Head.GlobalPosition.Y - 0.5f;
        var defaultPos = Head.GlobalPosition.Y + 0.5f;
        
        if (Input.IsActionPressed("inputControl"))
        {
            
        }
    }

    private void StateMachine()
    {
        if (StatsHandler.CurrentStamina <= 0)
        {
            CanSprint = false;
            CanJump = false;
        }
        else
        {
            CanSprint = true;
            CanJump = true;
        }
        
        if (!CanSprint)
            Sprinting = false;

        if (!CanJump)
            Jumping = false;

        if (!CanWalk)
            Walking = false;

        if (!CanCrouch)
            Crouching = false;
        
        if (Sprinting)
        {
            MoveSpeed *= 3f;
            MoveAcceleration *= 3f;
        }
        else
        {
            MoveSpeed = DefaultMoveSpeed;
            MoveAcceleration = DefaultMoveAcceleration;
        }
        
    }
}

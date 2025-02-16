using Godot;
using System;

public partial class PlayerMoveAndLookComponent : Node
{
    //Class variables
    private CharacterBody3D Parent;
    
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
    private bool Walking;
    private bool Sprinting;
    private bool Jumping;
    private bool Crouching;
    
    //FOV variables
    [Export] private float DefaultFov;              //Recommended: 80
    [Export] private float SprintFov;               //Recommended: 10
    [Export] private float CrouchFov;               //Recommended: -10
    [Export] private float JumpFov;                 //Recommended: 5
    
    //Look variables
    [Export] private float MouseSensitivity;        //Recommended: 0.5
    private RayCast3D HeadRaycast;
    private Node3D Head;
    private Camera3D Camera;
    private Basis ParentBasis;
    private Vector3 ParentBasisRotation;
    
    public override void _Ready()
    {
        Parent = GetParent() as CharacterBody3D;
        Head = Parent.GetNode<Node3D>("Head");
        HeadRaycast = Head.GetNode<RayCast3D>("HeadRaycast");
        Camera = Head.GetNode<Camera3D>("Camera");
    }

    public override void _Process(double delta)
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Move(delta);
        HandleFov(delta);
    }

    //Handle player head rotation with mouse input
    public override void _Input(InputEvent @event)
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
        if (Input.IsActionPressed("inputShift") && InputDirection != Vector3.Zero && !Sprinting)
        {
            Sprinting = true;
            MoveSpeed *= 3f;
            MoveAcceleration *= 3f;
        }
        else
        {
            Sprinting = false;
            MoveSpeed = DefaultMoveSpeed;
            MoveAcceleration = DefaultMoveAcceleration;
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
        if (JumpsLeft > 0 && Input.IsActionJustPressed("inputSpace"))
        {
            Jumping = true;
            JumpsLeft--;
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
}

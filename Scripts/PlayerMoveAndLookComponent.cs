using Godot;
using System;

public partial class PlayerMoveAndLookComponent : Node
{
    //Class variables
    private CharacterBody3D Parent;
    
    //Move variables
    [Export] private float Gravity;
    [Export] private float JumpForce;
    [Export] private float MoveSpeed;
    [Export] private float MoveAcceleration;
    [Export] private float MoveDrag;
    private Vector3 InputDirection = Vector3.Zero;
    private Vector3 TargetVelocity = Vector3.Zero;
    
    //Look variables
    [Export] private float MouseSensitivity;
    private RayCast3D HeadRaycast;
    private Node3D Head;
    private Camera3D Camera;
    private Basis HeadBasis;
    private Vector3 HeadBasisRotation;
    
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
    }

    //Handles player mouse input
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            var motion = (InputEventMouseMotion)@event;

            Head.RotateY(-motion.Relative.X * MouseSensitivity * 0.01f);
            Camera.RotateX(-motion.Relative.Y * MouseSensitivity * 0.01f);

            Camera.Rotation = new Vector3((float)Mathf.Clamp(Camera.Rotation.X, -Math.PI / 2, Math.PI / 2),
                Camera.Rotation.Y, Camera.Rotation.Z);
        }
    }

    //Handles player movement
    private void Move(double delta)
    {
        //Set variables
        InputDirection = Vector3.Zero;
        HeadBasis = Head.GlobalTransform.Basis;
        HeadBasisRotation = HeadBasis.GetEuler();
        
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
            HeadBasis = Basis.FromEuler(HeadBasisRotation);
            InputDirection = HeadBasis * InputDirection;
            InputDirection = InputDirection.Normalized();
            Head.Rotation = HeadBasisRotation;
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
        
        //Apply jump force
        if (Input.IsActionJustPressed("inputSpace"))
        {
            TargetVelocity.Y = JumpForce;
        }
        
        //Apply movement to character
        Parent.Velocity = TargetVelocity;
        Parent.MoveAndSlide();
    }
}

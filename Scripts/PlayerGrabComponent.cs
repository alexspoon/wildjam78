using Godot;
using System;

public partial class PlayerGrabComponent : Node
{
    //stuff
    [Export] private float GrabRange;
    [Export] private float GrabSpeed;
    private Node3D Head;
    private Node3D GrabPoint;
    private MeshInstance3D DebugGrabMesh;
    private RayCast3D HeadRaycast;
    private SpringArm3D GrabSpringArm;
    private GodotObject RayCollider;
    private Vector3 RayPoint;
    
    //Debug grab materials
    [Export] private StandardMaterial3D DebugGreen;
    [Export] private StandardMaterial3D DebugRed;
    
    public override void _Ready()
    {
        //Initialize variables
        Head = GetParent().GetNode<Node3D>("Head");
        HeadRaycast = Head.GetNode<RayCast3D>("HeadRaycast");
        GrabSpringArm = Head.GetNode<SpringArm3D>("GrabSpringArm");
        GrabPoint = GrabSpringArm.GetNode<Node3D>("GrabPoint");
        DebugGrabMesh = GrabPoint.GetNode<MeshInstance3D>("DebugMesh");
        HeadRaycast.SetTargetPosition(new Vector3(0, 0, -GrabRange));
        GrabSpringArm.SetLength(-GrabRange);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleGrabbedObject((float)delta);
    }
    
    private void HandleGrabbedObject(float delta)
    {
        DebugGrabMesh.MaterialOverride = DebugRed;
        
        if (!HeadRaycast.IsColliding())
            return;
        
        RayCollider = HeadRaycast.GetCollider();
        
        if (RayCollider is not RigidBody3D)
            return;
        
        DebugGrabMesh.MaterialOverride = DebugGreen;
        
        var hoveredObj = RayCollider as RigidBody3D;

        if (Input.IsActionPressed("inputLeftMouse"))
        {
            return;
        }
    }
}

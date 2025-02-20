using Godot;
using System;

public partial class PlayerGrabComponent : Node
{
    //Class variables
    [Export] private float GrabRange;
    [Export] private float GrabSpeed;
    private Node3D Head;
    private Node3D GrabPoint;
    private MeshInstance3D DebugGrabMesh;
    private RayCast3D GrabRaycast;
    private SpringArm3D GrabSpringArm;
    private GodotObject RayCollider;
    private Vector3 RayPoint;
    private RigidBody3D GrabbedObject;
    
    //Debug grab materials
    [Export] private StandardMaterial3D DebugGreen;
    [Export] private StandardMaterial3D DebugRed;
    
    public override void _Ready()
    {
        //Initialize variables
        Head = GetParent().GetNode<Node3D>("Head");
        GrabRaycast = Head.GetNode<RayCast3D>("GrabRaycast");
        GrabSpringArm = Head.GetNode<SpringArm3D>("GrabSpringArm");
        GrabPoint = GrabSpringArm.GetNode<Node3D>("GrabPoint");
        DebugGrabMesh = GrabPoint.GetNode<MeshInstance3D>("DebugMesh");
        GrabRaycast.SetTargetPosition(new Vector3(0, 0, -GrabRange));
        GrabSpringArm.SetLength(-GrabRange);
    }

    public override void _PhysicsProcess(double delta)
    {
        HandleGrab();
        HoldGrabbedObject();
    }
    
    //Check if grab raycast is colliding and if left mouse is clicked while it is, set grabbed object to raycast collider
    //Also update debug mesh to display whether raycast is colliding with a rigidbody
    private void HandleGrab()
    {
        GrabSpringArm.SetCollisionMask(1);
        DebugGrabMesh.MaterialOverride = DebugRed;
        
        if (!GrabRaycast.IsColliding())
            return;
        
        RayCollider = GrabRaycast.GetCollider();
        
        if (RayCollider is not RigidBody3D)
            return;
        
        DebugGrabMesh.MaterialOverride = DebugGreen;
        
        var hoveredObj = RayCollider as RigidBody3D;

        if (Input.IsActionJustPressed("inputLeftMouse"))
            GrabbedObject = hoveredObj;
    }

    //Update position of grabbed object to move towards grab point, change springarm collision layer to prevent colliding with grabbed object
    private void HoldGrabbedObject()
    {
        if (Input.IsActionJustPressed("inputRightMouse"))
            GrabbedObject = null;
        
        if (GrabbedObject != null)
        {
            GrabbedObject.ApplyCentralForce(GrabbedObject.GlobalPosition.DirectionTo(GrabPoint.GlobalPosition) * 50);
            GrabSpringArm.SetCollisionMask(16);
        }
    }
}

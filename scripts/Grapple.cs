using Godot;
using Godot.Collections;
using System;

public partial class Grapple : PinJoint2D
{
	Player player;
	Line2D line = null;
	PhysicsBody2D attached = null;
	Vector2 atchoffset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		line = GetNode<Line2D>("Line2D");
		player = GetParent<Player>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left) && attached != null) {
			//Position = ToLocal(attached.GlobalPosition + atchoffset);
			Vector2[] points = {ToLocal(player.GlobalPosition), Vector2.Zero};
			line.Points = points;
			//GD.Print("Should be: " + ToLocal(player.GlobalPosition) + ", " + Vector2.Zero);
			//GD.Print("Is:        " + points[0] + ", " + points[1]);
			//if (player.LinearVelocity.Dot(points[1] - points[0]) > 0) {
			//	NodeB = null;
			//} else {
			//	NodeB = attached.GetPath();
			//}
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Check if the event is a mouse button event
		if (@event is InputEventMouseButton mouseEvent)
		{
			// Left mouse button (button 1)
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				if (mouseEvent.Pressed) {
					Vector2 mousepos = GetGlobalMousePosition();
					PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
					// use global coordinates, not local to node
					PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, mousepos);
					query.Exclude = new Array<Rid> { GetRid() };
					Dictionary result = spaceState.IntersectRay(query);
					if (result.Count > 0) {
						attached = (PhysicsBody2D)result["collider"];
						atchoffset = (Vector2)result["position"] - attached.GlobalPosition;
						Reparent(attached);
						GlobalPosition = (Vector2)result["position"];
						GlobalRotation = ((Vector2)result["normal"]).Angle();
						NodeA = player.GetPath();
						NodeB = attached.GetPath();
					}
				} else {
					NodeB = null;
					line.Points = null;
					attached = null;
					Reparent(player);
					GlobalPosition = player.GlobalPosition;
				}
			}
		}
	}
}

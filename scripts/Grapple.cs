using Godot;
using Godot.Collections;
using System;

public partial class Grapple : Node2D
{
	[Export]
	private float maxdist = 1000;
	Player player;
	Line2D line = null;
	public bool attached = false;
	public float length = 0;

	public override void _Ready()
	{
		line = GetNode<Line2D>("Line2D");
		player = GetParent<Player>();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left) && attached) {
			Vector2[] points = {ToLocal(player.GlobalPosition), Vector2.Zero};
			line.Points = points;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent)
		{
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				if (mouseEvent.Pressed) {
					Vector2 mousepos = GetGlobalMousePosition();
					PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
					PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition+(mousepos-GlobalPosition).Normalized()*maxdist);
					query.Exclude = new Array<Rid> { player.GetRid() };
					Dictionary result = spaceState.IntersectRay(query);
					if (result.Count > 0) {
						attached = true;
						Reparent((PhysicsBody2D)result["collider"]);
						GlobalPosition = (Vector2)result["position"];
						GlobalRotation = ((Vector2)result["normal"]).Angle();

						Vector2 dist = GlobalPosition - player.GlobalPosition;
						length = dist.Length();
						//Vector2 dir = (dist).Normalized().Rotated(Mathf.Pi/2);
						//if (player.LinearVelocity.Dot(dir) < 0) {
						//	dir = -dir;
						//}
						//GD.Print("Old vel: " + player.LinearVelocity.Length());
            			//player.LinearVelocity = dir * player.LinearVelocity.Length();
						//GD.Print("New vel: " + player.LinearVelocity.Length());

						//NodeA = player.GetPath();
						//NodeB = GetParent().GetPath();
					}
				} else {
					//GD.Print("Pin vel: " + player.LinearVelocity.Length());
					//NodeB = null;
					line.Points = null;
					attached = false;
					Reparent(player);
					GlobalPosition = player.GlobalPosition;
				}
			}
		}
	}
}

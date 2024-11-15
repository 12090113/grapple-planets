using Godot;
using Godot.Collections;

public partial class Grapple : Node2D
{
	[Export]
	public float maxLength = 1250;
	[Export]
	private float minLength = 80;
	[Export]
	float pullSpeed = 300;
	[Export]
	public float acceleration = 100;
	[Export]
	public float maxSpeed = 1000;
	Player player = null;
	public GrappleRope rope = null;
	public Sprite2D gun = null;
	public bool attached = false;
	public float length = 0;

	public override void _Ready()
	{
		rope = GetNode<GrappleRope>("GrappleRope");
		player = GetParent<Player>();
		gun = GetNode<Sprite2D>("../Body/RightArm/GrappleGun");
	}

    public override void _Process(double delta)
    {
		Vector2[] points = {Vector2.Zero, ToLocal(gun.GlobalPosition)};
		rope.UpdatePoints(points, (float)delta);
    }

    public override void _PhysicsProcess(double delta)
	{
		if (attached) {
			if (Input.IsActionPressed("grapple_pull")) {
				PullPlayer((float)delta);
			}
			if (Input.IsActionPressed("grapple_push")) {
				PushPlayer((float)delta);
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent) {
			if (mouseEvent.ButtonIndex == MouseButton.Left) {
				if (mouseEvent.Pressed) {
					Reparent(player);
					GlobalPosition = player.GlobalPosition;
					Vector2 mousepos = GetGlobalMousePosition();
					Vector2 target = (mousepos-GlobalPosition).Normalized()*maxLength;
					PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
					PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition+target);
					query.Exclude = new Array<Rid> { player.GetRid() };
					Dictionary result = spaceState.IntersectRay(query);
					if (result.Count > 0) {
						attached = true;
						Reparent((PhysicsBody2D)result["collider"]);
						GlobalPosition = (Vector2)result["position"];
						GlobalRotation = (-(Vector2)result["normal"]).Angle();
						Vector2 dist = GlobalPosition - player.GlobalPosition;
						length = dist.Length();
						rope.ExtendSuccess();
					} else {
						GlobalPosition = player.GlobalPosition + target/2f;
						GlobalRotation = target.Angle();
						rope.ExtendFail();
					}
				} else if (attached) {
					rope.Retract();
					attached = false;
				}
			}
		}
	}

	private void PullPlayer(float delta)
	{
		length -= pullSpeed * delta;
		if (length < minLength) {
			length = minLength;
		}
	}

	private void PushPlayer(float delta)
	{
		length += pullSpeed * delta;
		if (length > maxLength) {
			length = maxLength;
		}
	}
}

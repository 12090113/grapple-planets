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
	public Player player = null;
	public GrappleRope rope = null;
	public Sprite2D gun = null;
	public bool attached = false;
	public PhysicsBody2D attachedBody;
	public float length = 0;
	[Export]
	private Area2D cutArea;
	public Vector2[] points;
	public Vector2[] oldPoints;

	public override void _Ready()
	{
		rope = GetNode<GrappleRope>("GrappleRope");
		player = GetParent<Player>();
		gun = GetNode<Sprite2D>("../Body/RightArm/GrappleGun");
		rope.playerHook = gun.GetNode<Sprite2D>("Hook");
		Callable.From(() => Reparent(player.GetParent())).CallDeferred();
	}

	public override void _Process(double delta)
	{
		oldPoints = points;
		points = [Vector2.Zero, ToLocal(gun.GlobalPosition)];
		rope.UpdatePoints(points, (float)delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (attached) {
			if (player.input.IsActionJustReleased("grapple"))
			{
				Retract();
			}
			/*if (Input.IsActionPressed("grapple_pull" + player.input)) {
				PullPlayer((float)delta);
			}
			if (Input.IsActionPressed("grapple_push" + player.input)) {
				PushPlayer((float)delta);
			}*/
		} else if (player.input.IsActionPressed("grapple") && player.grappleDisabled <= 0) {
			Reparent(player.GetParent());
			GlobalPosition = player.GlobalPosition;
			Vector2 target;
			if (player.input.IsKeyboard()) {
				Vector2 mousepos = GetGlobalMousePosition();
				target = (mousepos - GlobalPosition).Normalized() * maxLength;
			} else {
				target = player.input.GetVector("grapple_left", "grapple_right", "grapple_up", "grapple_down").Normalized() * maxLength;
			}
			PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + target);
			query.Exclude = new Array<Rid> { player.GetRid() };
			query.CollisionMask = player.CollisionMask;
			Dictionary result = spaceState.IntersectRay(query);
			if (result.Count > 0)
			{
				attached = true;
				attachedBody = (PhysicsBody2D)result["collider"];
				Reparent(attachedBody);
				GlobalPosition = (Vector2)result["position"];
				GlobalRotation = (-(Vector2)result["normal"]).Angle();
				Vector2 dist = GlobalPosition - player.GlobalPosition;
				length = dist.Length();
				if (attachedBody is RigidBody2D) {
					//player.SetCollisionMaskValue(5, false);
					length = (attachedBody.GlobalPosition - player.GlobalPosition).Length();
				}
				cutArea.Monitoring = true;
				rope.ExtendSuccess();
			}
			// else
			// {
			// 	GlobalPosition = player.GlobalPosition + target / 2f;
			// 	GlobalRotation = target.Angle();
			// 	rope.ExtendFail();
			// }
		}
	}

	public void Retract() {
		//player.SetCollisionMaskValue(5, true);
		cutArea.Monitoring = false;
		rope.Retract();
		attached = false;
		attachedBody = null;
		Reparent(player.GetParent());
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

using Godot;

public partial class Player : RigidBody2D
{
	Grapple grapple;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grapple = GetNode<Grapple>("Grapple");
	}

	[Export]
	private Vector2 _thrust = new Vector2(0, -250);
	[Export]
	private float _torque = 20000;

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (Input.IsActionPressed("ui_up"))
			state.ApplyForce(_thrust.Rotated(Rotation));
		else
			state.ApplyForce(new Vector2());

		var rotationDir = 0;
		if (Input.IsActionPressed("ui_right"))
			rotationDir += 1;
		if (Input.IsActionPressed("ui_left"))
			rotationDir -= 1;
		state.ApplyTorque(rotationDir * _torque);

		if (grapple.attached) {
			Vector2 dist = grapple.GlobalPosition - GlobalPosition;
			Vector2 dir = dist.Normalized();
			Vector2 perpdir = dist.Rotated(Mathf.Pi/2).Normalized();
			if (state.LinearVelocity.Dot(perpdir) < 0) {
				perpdir = -perpdir;
			}
			if (dist.Length() >= grapple.length && state.LinearVelocity.Dot(dir) <= 0) {
				state.LinearVelocity = perpdir * state.LinearVelocity.Length();
                if (dist.Length() > grapple.length) {
                    GlobalPosition += -dir * (grapple.length-dist.Length());
                }
			}
		}
	}
}

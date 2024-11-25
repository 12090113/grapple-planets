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
	private float thrust = 250f;

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (Input.IsActionPressed("ui_up"))
			state.ApplyForce(Vector2.Up*thrust);
		if (Input.IsActionPressed("ui_down"))
			state.ApplyForce(Vector2.Down*thrust);
		if (Input.IsActionPressed("ui_right"))
			state.ApplyForce(Vector2.Right*thrust);
		if (Input.IsActionPressed("ui_left"))
			state.ApplyForce(Vector2.Left*thrust);
		if (Input.IsActionPressed("ui_accept"))
			GlobalPosition = Vector2.Zero;

		if (grapple.attached) {
			Vector2 dist = grapple.GlobalPosition - GlobalPosition;
			Vector2 dir = dist.Normalized();
			Vector2 perpdir = dist.Rotated(Mathf.Pi/2).Normalized();
			if (state.LinearVelocity.Dot(perpdir) < 0) {
				perpdir = -perpdir;
			}
			if (state.LinearVelocity.Length() < grapple.maxSpeed) {
				state.LinearVelocity += perpdir * grapple.acceleration * state.Step;
			}
			if (dist.Length() >= grapple.length && state.LinearVelocity.Dot(dir) <= 0) {
				state.LinearVelocity = perpdir * state.LinearVelocity.Length();
				if (dist.Length() > grapple.length) {
					GlobalPosition += -dir * (grapple.length-dist.Length());
				}
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
    	var collision = MoveAndCollide(LinearVelocity * (float)delta);
    	if (collision != null)
    	{
        	var collider = collision.GetCollider() as Node;
        	if (collider != null && collider.IsInGroup("Enemy"))
       		{
            	GetTree().ChangeSceneToFile("res://scenes/death_menu.tscn");
        	}
    	}
	}
}

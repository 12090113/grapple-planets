using Godot;

public partial class Player : RigidBody2D
{
	[Export]
	private float maxHealth = 100;
	[Export]
	private float invulnTime = 0.2f;
	private float invuln = 0f;
	private float health = 1;
	private Grapple grapple;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grapple = GetNode<Grapple>("Grapple");
		health = maxHealth;
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
        //invul
    }

    public void _on_body_entered(Node2D body) {
		if (body.IsInGroup("enemybullet")) {
			health -= 5;
			body.QueueFree();
		} else if (body.IsInGroup("enemy1")) {
			health -= 15;
		} else if (body.IsInGroup("enemy2")) {
			health -= 10;
		} else {
			health -= 5;
		}
		GD.Print(health);
		if (health < 0) {
			Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/death_menu.tscn")).CallDeferred();
		}
	}
}

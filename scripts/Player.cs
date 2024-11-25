using Godot;

public partial class Player : RigidBody2D
{
	[Export]
	private float maxHealth = 100;
	[Export]
	private float invulnTime = 0.2f;
	[Export]
	private float regenTime = 3f;
	[Export]
	private float regenRate = 4f;
	[Export]
	private ProgressBar healthBar;
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
        invuln -= (float)delta;
		if (invuln < -regenTime && health < maxHealth) {
			health += regenRate * (float)delta;
		}
		healthBar.Value = health;
    }

    public void _on_body_entered(Node2D body) {
		float healthchange = 0;
		if (body.IsInGroup("enemybullet")) {
			healthchange = 5;
			body.QueueFree();
		} else if (body.IsInGroup("enemy1")) {
			healthchange = 15;
		} else if (body.IsInGroup("enemy2")) {
			healthchange = 10;
		} else {
			healthchange = 5;
		}
		if (invuln <= 0) {
			health -= healthchange;
			if (health < 0) {
				Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/death_menu.tscn")).CallDeferred();
			}
			invuln = invulnTime;
		}
	}
}

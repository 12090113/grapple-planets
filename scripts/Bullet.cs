using Godot;

public partial class Bullet : RigidBody2D
{
	[Export]
	private float lerptime = 0.2f;
	private Timer timer;
	private Trail trail;
	private float startrot = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		timer = GetNode<Timer>("Timer");
		trail = GetNode<Trail>("Trail");
		trail.bullet = this;
		trail.Reparent(GetTree().Root);
		trail.GlobalPosition = Vector2.Zero;
		trail.Rotation = 0;
		startrot = Rotation;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		float time = (float)(timer.WaitTime-timer.TimeLeft);
		if (time < lerptime) {
			Rotation = Mathf.LerpAngle(startrot, LinearVelocity.Angle(), 1/lerptime*time);
		}
		else
			Rotation = LinearVelocity.Angle();
	}

	public void _on_timer_timeout() {
		QueueFree();
	}

	public void _on_body_entered(Node2D body) {
		if (body.IsInGroup("other")) {
			QueueFree();
		}
	}
}

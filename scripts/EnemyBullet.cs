using Godot;

public partial class EnemyBullet : RigidBody2D
{
	private Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		timer = GetNode<Timer>("Timer");
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

using Godot;
using System;

public partial class Bullet : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(StaticBody2D body)
	{
		GD.Print("j");
		if (body.IsInGroup("other"))
		{
			QueueFree();
		}
	}

	public override void _PhysicsProcess(double delta)
{
    var collision = MoveAndCollide(LinearVelocity * (float)delta);
    if (collision != null)
    {
        var collider = collision.GetCollider() as Node;
        if (collider != null && collider.IsInGroup("other"))
        {
            QueueFree();
        }
    }
}
	
	public void OnVisibleOnScreenNotifier2DScreenExited()
	{
		QueueFree();
	}
}

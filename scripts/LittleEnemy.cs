using Godot;

public partial class LittleEnemy : RigidBody2D
{
	[Export] 
	public float Speed = 200f;
	private Node2D _player;

	private AnimatedSprite2D outline;
	private AnimatedSprite2D fly;
	
	public override void _Ready()
	{
		_player = GetNode<Node2D>("../../Player");
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_player != null)
		{
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

			state.LinearVelocity = direction * Speed;
		}
	}

	public void _on_body_entered(Node body)
	{
		if (body.IsInGroup("bullet"))
		{
			GetNodeOrNull<Grapple>("Grapple")?.Retract();
			body.QueueFree();
			QueueFree();
		}
	}
}

using Godot;

public partial class Enemy : RigidBody2D
{
	[Export]
	PackedScene tinyEnemyScn;
	[Export] 
	public float Speed = 200f;
	[Export]
	private int childrenCount = 3;
	private Node2D _player;

	private AnimatedSprite2D outline;
	private AnimatedSprite2D fly;
	
	public override void _Ready()
	{
		outline = GetNode<AnimatedSprite2D>("Fly/Outline");
		fly = GetNode<AnimatedSprite2D>("Fly");
		_player = GetNode<Node2D>("../Player");
		playanimation();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_player != null)
		{
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();

			LinearVelocity = direction * Speed;
		}
	}

	public void _on_body_entered(Node body)
	{
		if (body.IsInGroup("bullet"))
		{
			for (int i = 0; i < childrenCount; i++)
			{
				RigidBody2D enemy = tinyEnemyScn.Instantiate<RigidBody2D>();
				enemy.Rotation = GlobalRotation;
				enemy.GlobalPosition = GlobalPosition;

				Callable.From(() => GetParent().AddChild(enemy)).CallDeferred();
			}
			GetNodeOrNull<Grapple>("Grapple")?.Retract();
			body.QueueFree();
			QueueFree();
		}
	}
	public void playanimation()
	{
		outline.CallDeferred("play");
		fly.CallDeferred("play");
	}
}

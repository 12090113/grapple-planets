using Godot;

public partial class Enemy : RigidBody2D
{
	[Export]
	PackedScene bulletScn;
	[Export]
	float bulletSpeed = 1000f;

	[Export]
	PackedScene tinyEnemyScn;
	[Export] 
	public float Speed = 200f;
	[Export]
	private int childrenCount = 3;
	private Node2D _player;

	private AnimatedSprite2D outline;
	private AnimatedSprite2D fly;

	float bps = 0.25f;
	float fireRate;
	float timeUntilFire;

	public Vector2 direction;
	
	public override void _Ready()
	{
		fireRate = 1 / bps;
		outline = GetNode<AnimatedSprite2D>("Fly/Outline");
		fly = GetNode<AnimatedSprite2D>("Fly");
		_player = GetNode<Node2D>("../../Player");
		playanimation();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (_player != null)
		{
			direction = (_player.GlobalPosition - GlobalPosition).Normalized();

			state.LinearVelocity = direction * Speed;
		}
	}

	public override void _Process(double delta)
	{
		if (timeUntilFire > fireRate)
		{
			shoot();
		}
		else
		{
			timeUntilFire += (float)delta;
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

	public void shoot()
	{
		RigidBody2D bullet = bulletScn.Instantiate<RigidBody2D>();

		bullet.GlobalPosition = GlobalPosition;
		direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		bullet.Rotation = direction.Angle();
		bullet.LinearVelocity = direction * bulletSpeed;

		GetTree().Root.AddChild(bullet);
		timeUntilFire = 0f;
	}
}

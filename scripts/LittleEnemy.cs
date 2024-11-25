using Godot;

public partial class LittleEnemy : RigidBody2D
{
	[Export]
	PackedScene bulletScn;
	[Export]
	float bulletSpeed = 2000f;
	[Export] 
	public float Speed = 200f;
	private Node2D _player;

	private AnimatedSprite2D outline;
	private AnimatedSprite2D fly;

	float bps = 1f;
	float fireRate;
	float timeUntilFire;
	
	public override void _Ready()
	{
		fireRate = 1 / bps;
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
			Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
			bullet.Rotation = direction.Angle();
			bullet.LinearVelocity = direction * bulletSpeed;

			GetTree().Root.AddChild(bullet);
			timeUntilFire = 0f;
	}
}

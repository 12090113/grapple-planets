using Godot;
using System;

public partial class PlayerBullet : Node2D
{	
	[Export]
	PackedScene bulletScn;
	[Export]
	float bulletSpeed = 2000f;
	[Export]
	float bps = 3f;
	float fireRate;
	float timeUntilFire;
	private Player player;
	public override void _Ready()
	{
		player = GetNode<Player>("../../..");
		fireRate = 1 / bps;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("click") && timeUntilFire > fireRate)
		{
			RigidBody2D bullet = bulletScn.Instantiate<RigidBody2D>();

			bullet.Rotation = GlobalRotation;
			bullet.GlobalPosition = GlobalPosition;
			bullet.LinearVelocity = bullet.Transform.X * bulletSpeed + player.LinearVelocity;

			GetTree().Root.AddChild(bullet);

			timeUntilFire = 0f;
		}
		else
		{
			timeUntilFire += (float)delta;
		}
	}
}

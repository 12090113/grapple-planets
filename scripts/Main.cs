using Godot;
using System;

public partial class Main : Node2D
{
	[Export]
	PackedScene enemyScn;
	float EPS = 1;
	float timeUntilSpawn;
	float spawnRate;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spawnRate = 1 / EPS;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (timeUntilSpawn > spawnRate)
		{
			spawn();
		}
		else
		{
			timeUntilSpawn += (float)delta;
		}
	}

	public void spawn()
	{
	}
}

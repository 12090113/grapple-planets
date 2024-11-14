using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Swing : AnimatedSprite2D
{
	private Grapple grapple;
	private AnimatedSprite2D outline;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		outline = GetNode<AnimatedSprite2D>("Outline");
		this.Play("swinging");
		grapple = GetParent().GetNode<Grapple>("Grapple");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (grapple.attached)
		{
			this.Play("swinging");
			outline.Play("swinging2");
		}
		else
		{
			this.Play("idle");
			outline.Play("idle2");
		}
	}
}

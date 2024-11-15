using Godot;
using System;

public partial class Hook : Sprite2D
{
	public Node2D parent = null;
	private Vector2 scale;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = GetParent<Node2D>();
		scale = GlobalScale;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (parent.GlobalScale.X >= 0.7f)
			GlobalScale = scale/parent.GlobalScale;
		else
			GlobalScale = parent.GlobalScale/scale;

		GlobalSkew = parent.GlobalSkew;
	}
}

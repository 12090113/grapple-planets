using Godot;

public partial class Planet : StaticBody2D
{
	CircleShape2D collider = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collider = (CircleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
	}

	public override void _Draw()
	{
		DrawCircle(new Vector2(0,0), collider.Radius, new Color(1,1,1));
	}
}

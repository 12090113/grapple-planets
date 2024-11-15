using Godot;

public partial class Swing : AnimatedSprite2D
{
	private Grapple grapple;
	Player player;
	private AnimatedSprite2D outline;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		outline = GetNode<AnimatedSprite2D>("Outline");
		this.Play("swinging");
		grapple = GetParent().GetNode<Grapple>("Grapple");
		player = GetParent<Player>();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player.LinearVelocity < Vector2.Zero)
		{
			this.FlipH = true;
			outline.FlipH = true;
		}
		else
		{
			this.FlipH = false;
			outline.FlipH = false;
		}
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

using Godot;

public partial class GrappleCutDetector : Area2D
{
    [Export]
    private float bufferSize = 100;
    public Grapple grapple {get; private set;}
    private RectangleShape2D collider;

    public override void _Ready()
	{
        grapple = GetParent().GetParent<Grapple>();
        collider = (RectangleShape2D)GetChild<CollisionShape2D>(0).Shape;
	}

    public override void _Process(double delta)
    {
        if (grapple.attached) {
            Position = grapple.points[0].Lerp(grapple.points[1], 0.5f);
            Rotation = grapple.points[0].AngleToPoint(grapple.points[1]);
            collider.Size = new Vector2(grapple.points[0].DistanceTo(grapple.points[1]) + bufferSize, bufferSize);
        }
    }
}

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
        if (grapple.ropePoints != null) {
            Position = grapple.ropePoints[0].Lerp(grapple.ropePoints[1], 0.5f);
            Rotation = grapple.ropePoints[0].AngleToPoint(grapple.ropePoints[1]);
            collider.Size = new Vector2(grapple.ropePoints[0].DistanceTo(grapple.ropePoints[1]) + bufferSize, bufferSize);
        }
    }
}

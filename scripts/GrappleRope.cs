using Godot;

public partial class GrappleRope : Line2D
{
	Vector2[] grapplePoints;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setPoints(Vector2[] points) {
		/*grapplePoints = points;
		if (points != null) {
			Vector2[] sinpoints = new Vector2[100];
			for (int i = 0; i < sinpoints.Length; i++)
			{
				sinpoints[i] = grapplePoints[0].Lerp(grapplePoints[1], (float)i/sinpoints.Length) + (grapplePoints[0]-grapplePoints[1]).Rotated(Mathf.Pi/2)*Mathf.Sin(i)*0.01f;
			}
			Points = sinpoints;
		} else {*/
			Points = points;
		//}
	}
}

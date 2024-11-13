using Godot;

public partial class GrappleRope : Line2D
{
	[Export]
	float reachTime = 0.15f;
	[Export]
	float straightenTime = 0.1f;
	[Export]
	float frequency = 3*Mathf.Pi;
	[Export]
	float amplitude = 0.1f;
	[Export]
	int precision = 100;

	Vector2[] grapplePoints;
	float reach = 0;
	float straighten = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void setPoints(Vector2[] points) {
		if (points != null) {
			if (grapplePoints == null) {
				reach = 1f;
			}
			grapplePoints = points;
			if (reach > 0 || straighten > 0) {
				Vector2[] sinpoints = new Vector2[precision];
				if (reach > 0) {
					for (int i = 0; i < sinpoints.Length; i++)
					{
						sinpoints[i] = grapplePoints[0].Lerp(grapplePoints[1]-(grapplePoints[1]-grapplePoints[0])*reach, (float)i/sinpoints.Length) + (grapplePoints[0]-grapplePoints[1]).Rotated(Mathf.Pi/2)*Mathf.Sin((float)i/sinpoints.Length*frequency)*amplitude*(1-reach);//*(1-(float)i/sinpoints.Length);
					}
					reach -= 1f/reachTime*(float)GetPhysicsProcessDeltaTime();
					if (reach <= 0)
						straighten = 1;
				}
				if (straighten > 0) {
					for (int i = 0; i < sinpoints.Length; i++)
					{
						sinpoints[i] = grapplePoints[0].Lerp(grapplePoints[1], (float)i/sinpoints.Length) + (grapplePoints[0]-grapplePoints[1]).Rotated(Mathf.Pi/2)*Mathf.Sin((float)i/sinpoints.Length*frequency)*amplitude*straighten;//*(1-(float)i/sinpoints.Length);
					}
					straighten -= 1f/straightenTime*(float)GetPhysicsProcessDeltaTime();
				}
				Points = sinpoints;
			} else {
				Points = grapplePoints;
			}
		} else {
			grapplePoints = points;
			Points = points;
			reach = 0;
			straighten = 0;
		}
	}
}

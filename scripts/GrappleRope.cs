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
	float maxAmplitude = 400f;
	[Export]
	int precision = 100;

	Vector2[] grapplePoints;
	float reach = 0;
	float straighten = 0;

	public void setPoints(Vector2[] points) {
		if (points != null) {
			if (grapplePoints == null) {
				reach = 1f;
			}
			grapplePoints = points;
			if (reach > 0 || straighten > 0) {
				Vector2[] sinpoints = new Vector2[precision];
				Vector2 vector = grapplePoints[1]-grapplePoints[0];
				Vector2 offset = vector.Rotated(Mathf.Pi/2).Normalized() * Mathf.Min(vector.Length(), maxAmplitude);
				if (reach > 0) {
					for (int i = 0; i < sinpoints.Length; i++)
					{
						sinpoints[i] = (grapplePoints[0]+vector*reach).Lerp(grapplePoints[1], (float)i/sinpoints.Length) + offset*Mathf.Sin((float)i/sinpoints.Length*frequency)*amplitude*(1-reach);
					}
					reach -= 1f/reachTime*(float)GetPhysicsProcessDeltaTime();
					if (reach <= 0)
						straighten = 1;
				} else if (straighten > 0) {
					for (int i = 0; i < sinpoints.Length; i++)
					{
						sinpoints[i] = grapplePoints[0].Lerp(grapplePoints[1], (float)i/sinpoints.Length) + offset*Mathf.Sin((float)i/sinpoints.Length*frequency)*amplitude*straighten;
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

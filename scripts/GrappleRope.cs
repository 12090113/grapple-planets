using Godot;

public partial class GrappleRope : Line2D
{
	[Export]
	float maxExtendTime = 0.3f;
	[Export]
	float straightenTime = 0.1f;
	[Export]
	float maxRetractTime = 0.3f;
	[Export]
	float frequency = 3*Mathf.Pi;
	[Export]
	float amplitude = 0.1f;
	[Export]
	float maxAmplitude = 400f;
	[Export]
	int precision = 100;
	Line2D outline = null;
	Sprite2D hook = null;
	Grapple grapple = null;
	Vector2[] grapplePoints;
	float extend = 0;
	float straighten = 0;
	public float retract = 0;
	bool fail = false;

	public override void _Ready()
	{
		grapple = GetParent<Grapple>();
		outline = GetNode<Line2D>("Outline");
		hook = GetNode<Sprite2D>("Hook");
	}

	public void ExtendSuccess() {
		hook.Reparent(this);
		grapplePoints = null;
		retract = 0f;
		straighten = 0f;
		extend = 1f;
		fail = false;
	}

	public void ExtendFail() {
		hook.Reparent(this);
		grapplePoints = null;
		retract = 0f;
		straighten = 0f;
		extend = 1f;
		fail = true;
	}

	public void Retract() {
		if (extend > 0f) {
			retract = 1f-extend;
			if (straighten < 0)
				straighten = 0f;
		} else {
			retract = 1f;
			if (straighten <= 0)
				straighten = 1f;
			else
				straighten = 1f-straighten;
		}
		extend = 0f;
	}

	public void UpdatePoints(Vector2[] points, float delta) {
		if (points != null) {
			grapplePoints = points;
		}
		if (grapplePoints == null) {
			Points = null;
			return;
		}
		if (extend > 0 || straighten > 0 || retract > 0) {
			Vector2[] sinpoints = new Vector2[precision];
			Vector2 vector = grapplePoints[1]-grapplePoints[0];
			Vector2 offset = vector.Rotated(Mathf.Pi/2).Normalized() * Mathf.Min(vector.Length(), maxAmplitude);
			if (extend > 0) {
				for (int i = 0; i < sinpoints.Length; i++)
				{
					sinpoints[i] = (grapplePoints[0]+vector*extend).Lerp(grapplePoints[1], i/(sinpoints.Length-1f)) + offset*Mathf.Sin(i/(sinpoints.Length-1f)*frequency)*amplitude*(1-extend);
				}
				extend -= 1f/Mathf.Lerp(0.01f, maxExtendTime, vector.Length()/grapple.maxLength)*delta;
				if (extend <= 0) {
					if (fail)
						retract = 1f;
					else
						straighten = 1;
				}
			} else if (retract > 0) {
				for (int i = 0; i < sinpoints.Length; i++)
				{
					sinpoints[i] = (grapplePoints[0]+vector*(1-retract)).Lerp(grapplePoints[1], i/(sinpoints.Length-1f)) + offset*Mathf.Sin(i/(sinpoints.Length-1f)*frequency)*amplitude*retract*(1-straighten);
				}
				retract -= 1f/Mathf.Lerp(0.01f, maxRetractTime, vector.Length()/grapple.maxLength)*delta;
				if (retract <= 0) {
					grapplePoints = null;
					Points = null;
					straighten = 0;
					hook.Reparent(grapple.gun);
					hook.Position = Vector2.Zero;
					hook.Rotation = 0;
					outline.Points = Points;
					return;
				}
				straighten -= 1f/straightenTime*delta;
			} else if (straighten > 0) {
				for (int i = 0; i < sinpoints.Length; i++)
				{
					sinpoints[i] = grapplePoints[0].Lerp(grapplePoints[1], i/(sinpoints.Length-1f)) + offset*Mathf.Sin(i/(sinpoints.Length-1f)*frequency)*amplitude*straighten;
				}
				straighten -= 1f/straightenTime*delta;
			}
			Points = sinpoints;
			hook.Visible = true;
			hook.Position = Points[0];
		} else if (grapple.attached) {
			Points = grapplePoints;
			hook.Visible = true;
			hook.Position = Points[0];
		} else {
			Points = null;
		}
		outline.Points = Points;
	}
}

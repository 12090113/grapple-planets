using Godot;
using Godot.Collections;

public partial class PlayerAnimation : AnimatedSprite2D
{
	[Export]
	float flipSpeed = 0.05f;
	private Grapple grapple;
	private Player player;
	private AnimatedSprite2D outline;
	private Sprite2D rightArm;
	private Sprite2D leftArm;
	private float rightArmRotationSpeed = 0.2f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		outline = GetNode<AnimatedSprite2D>("Outline");
		this.Play("swinging");
		grapple = GetParent().GetNode<Grapple>("Grapple");
		player = GetParent<Player>();
		rightArm = GetNode<Sprite2D>("RightArm");
		leftArm = GetNode<Sprite2D>("LeftArm");
		if (player.input.IsJoypad()) {
			rightArmRotationSpeed = 1f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		Vector2 mousepos;
		if (player.input.IsKeyboard())
			mousepos = player.ToLocal(GetGlobalMousePosition());
		else
			mousepos = player.input.GetVector("grapple_left", "grapple_right", "grapple_up", "grapple_down").Normalized() * grapple.maxLength;
		if (grapple.attached) {
			this.Play("swinging");
			outline.Play("swinging2");
			rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, (grapple.GlobalPosition-rightArm.GlobalPosition).Angle(), rightArmRotationSpeed);
		} else if (mousepos.LengthSquared() > 0f) {
			this.Play("idle");
			outline.Play("idle2");
			Vector2 target = mousepos.Normalized()*grapple.maxLength;
			PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition+target);
			query.Exclude = new Array<Rid> { player.GetRid() };
			query.CollisionMask = player.CollisionMask;
			Dictionary result = spaceState.IntersectRay(query);
			if (result.Count > 0) {
				rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, ((Vector2)result["position"]-rightArm.GlobalPosition).Angle(), grapple.rope.retract <= 0 ? rightArmRotationSpeed : rightArmRotationSpeed/2);
			} else {
				rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, (player.GlobalPosition+target-rightArm.GlobalPosition).Angle(), grapple.rope.retract <= 0 ? rightArmRotationSpeed : rightArmRotationSpeed/2);
			}
		}
		leftArm.GlobalRotation = Mathf.LerpAngle(leftArm.GlobalRotation, mousepos.Angle(), rightArmRotationSpeed);
		if (Mathf.Cos(rightArm.GlobalRotation) < 0 && Mathf.Cos(leftArm.GlobalRotation) < 0 && player.Scale.X < 0)
			rightArm.ZIndex = 1;
		else
			rightArm.ZIndex = 0;
		
		float targetScaleX;
		if (mousepos.X >= 0) {
			targetScaleX = 0.25f;
		} else {
			targetScaleX = -0.25f;
		}
		Scale = new Vector2(Mathf.Lerp(Scale.X, targetScaleX, flipSpeed), Scale.Y);

		if (player.LinearVelocity.Length() < grapple.maxSpeed-2f) {
			Modulate = new Color(1, 1 - player.LinearVelocity.Length() / grapple.maxSpeed, 1);
		} else {
			Modulate = new Color(1,0,0);
		}
	}
}

using Godot;
using Godot.Collections;

public partial class PlayerAnimation : AnimatedSprite2D
{
	private Grapple grapple;
	private Player player;
	private AnimatedSprite2D outline;
	private Sprite2D rightArm;
	private Sprite2D leftArm;
	private float rightArmRotationSpeed = 0.2f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		outline = GetNode<AnimatedSprite2D>("Outline");
		this.Play("swinging");
		grapple = GetParent().GetNode<Grapple>("Grapple");
		player = GetParent<Player>();
		rightArm = GetNode<Sprite2D>("RightArm");
		leftArm = GetNode<Sprite2D>("LeftArm");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 mousepos = GetGlobalMousePosition();
		if (grapple.attached)
		{
			this.Play("swinging");
			outline.Play("swinging2");
			rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, (grapple.GlobalPosition-rightArm.GlobalPosition).Angle(), rightArmRotationSpeed);
		}
		else
		{
			this.Play("idle");
			outline.Play("idle2");
			Vector2 target = (mousepos-player.GlobalPosition).Normalized()*grapple.maxLength;
			PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition+target);
			query.Exclude = new Array<Rid> { player.GetRid() };
			Dictionary result = spaceState.IntersectRay(query);
			if (result.Count > 0) {
				rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, ((Vector2)result["position"]-rightArm.GlobalPosition).Angle(), grapple.rope.retract <= 0 ? rightArmRotationSpeed : rightArmRotationSpeed/2);
			} else {
				rightArm.GlobalRotation = Mathf.LerpAngle(rightArm.GlobalRotation, (player.GlobalPosition+target-rightArm.GlobalPosition).Angle(), grapple.rope.retract <= 0 ? rightArmRotationSpeed : rightArmRotationSpeed/2);
			}
		}
		leftArm.GlobalRotation = Mathf.LerpAngle(leftArm.GlobalRotation, (mousepos-leftArm.GlobalPosition).Angle(), rightArmRotationSpeed);
		if (Mathf.Cos(rightArm.GlobalRotation) < 0 && Mathf.Cos(leftArm.GlobalRotation) < 0)
			rightArm.ZIndex = 1;
		else
			rightArm.ZIndex = 0;
		
		if (Mathf.Cos(leftArm.GlobalRotation) < 0)
		{
			this.FlipH = true;
			outline.FlipH = true;
		}
		else
		{
			this.FlipH = false;
			outline.FlipH = false;
		}
	}
}

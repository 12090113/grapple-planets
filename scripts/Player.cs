using Godot;
using MultiplayerInputSharp;

public partial class Player : RigidBody2D
{
	[Export]
	private float maxHealth = 100;
	[Export]
	private float invulnTime = 0.2f;
	[Export]
	private float regenTime = 3f;
	[Export]
	private float regenRate = 4f;
	[Export]
	private ProgressBar healthBar;
	private float invuln = 0f;
	private float health = 1;
	private Grapple grapple;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grapple = GetNode<Grapple>("Grapple");
		health = maxHealth;
	}

	[Export]
	private float thrust = 250f;

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (grapple.attached) {
			Vector2 vel = state.LinearVelocity;
			RigidBody2D attachedBody = null;
			float acceleration = grapple.acceleration;
			bool doubleAttached = false;
			Vector2 attatchedPos = grapple.GlobalPosition;
			if (grapple.attachedBody is RigidBody2D) {
				attachedBody = (RigidBody2D)grapple.attachedBody;
				vel -= attachedBody.LinearVelocity;
				acceleration *= Mass / (Mass + attachedBody.Mass);
				if (attachedBody is Player) {
					if (((Player)attachedBody).grapple.attachedBody == this) {
						doubleAttached = true;
						acceleration /= 2;
					}
					attatchedPos = attachedBody.Position;
				}
			}
			Vector2 difference = attatchedPos - GlobalPosition;
			float dist = difference.Length();
			Vector2 dir = difference.Normalized();
			Vector2 perpdir = dir.Rotated(Mathf.Pi/2);
			
			if (vel.Dot(perpdir) < 0) {
				perpdir = -perpdir;
			}
			if (vel.Length() < grapple.maxSpeed) {
				state.LinearVelocity += perpdir * acceleration * state.Step;
				if (attachedBody != null) {
					attachedBody.LinearVelocity += -perpdir * ((grapple.acceleration / (doubleAttached ? 2 : 1))-acceleration) * state.Step;
				}
			}
			if (dist >= grapple.length && vel.Dot(dir) <= 0) {
				state.LinearVelocity = perpdir * state.LinearVelocity.Length();
				if (attachedBody != null) {
					Vector2 otherdir = -perpdir;
					if (vel.Dot(otherdir) > 0)
						otherdir = -otherdir;
					attachedBody.LinearVelocity = otherdir * attachedBody.LinearVelocity.Length();
				}
				if (dist > grapple.length) {
					GlobalPosition += -dir * (grapple.length-dist);
				}
			}
		}
	}
    public override void _PhysicsProcess(double delta)
    {
        invuln -= (float)delta;
		if (invuln < -regenTime && health < maxHealth) {
			health += regenRate * (float)delta;
		}
		healthBar.Value = health;
    }

    public void _on_body_entered(Node2D body) {
		float healthchange = 0;
		if (body.IsInGroup("enemybullet")) {
			healthchange = 5;
			body.QueueFree();
		} else if (body.IsInGroup("enemy1")) {
			healthchange = 15;
		} else if (body.IsInGroup("enemy2")) {
			healthchange = 10;
		} else {
			healthchange = 5;
		}
		if (invuln <= 0) {
			health -= healthchange;
			if (health < 0) {
				Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/death_menu.tscn")).CallDeferred();
			}
			invuln = invulnTime;
		}
	}

	// Multiplayer

	[Signal]
	public delegate void LeaveEventHandler(int player);

	public int playerNum {get; private set;}
	public DeviceInput input {get; private set;}

	public void Init(int player, int device) {
		playerNum = player;

		// in my project, I got the device integer by accessing the singleton autoload PlayerManager
		// but for simplicity, it's not an autoload in this demo.
		// but I recommend making it a singleton so you can access the player data from anywhere.
		// that would look like the following line, instead of the device function parameter above.
		//    device = PlayerManager.GetPlayerDevice(_player);
		input = new DeviceInput(device);
		GD.Print("input: ", input);

		//GetNode<Label>("Player").Text = playerNum.ToString();
	}

	public override void _Process(double delta) {
		// let the player leave by pressing the "join" button
		if (input.IsActionJustPressed("join")) {
			// an alternative to this is just call PlayerManager.leave(player)
			// but that only works if you set up the PlayerManager singleton
			grapple.QueueFree();
			EmitSignal(SignalName.Leave, playerNum);
		}
		QueueRedraw();
	}

	public override void _Draw() {
		DrawLine(Vector2.Zero, input.GetVector("grapple_left", "grapple_right", "grapple_up", "grapple_down").Normalized() * grapple.maxLength, Colors.Red, 5, true);
	}
}

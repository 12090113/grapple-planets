using System.Collections.Generic;
using Godot;
using Godot.Collections;
using MultiplayerInputSharp;

public partial class Player : RigidBody2D
{
	[Export]
	private ProgressBar grappleBar;
	[Export]
	public float grappleCutTime = 5;
	[Export]
	public float grappleBumpTime = 1;
	public float grappleDisabled = 0;
	[Export]
	public float speedEquivalency = 2f;
	public Grapple grapple {get; private set;}
	private Area2D enemyDetector;
	[Export]
	private float playerBounciness = 0.5f;
	private List<Player> colliding = new();
	[Export]
	public float invulnerableSpawnTime = 5;
	public float invulTime = float.MaxValue;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		grapple = GetNode<Grapple>("Grapple");
		enemyDetector = GetNode<Area2D>("EnemyDetector");
	}

	[Export]
	private float thrust = 250f;

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if (invulTime <= 0) {
			Array<Node2D> bodies = enemyDetector.GetOverlappingBodies();
			List<Player> stillColliding = new();
			foreach (Node2D body in bodies) {
				if (body is Player && body != this) {
					Player enemy = (Player)body;
					if (enemy.invulTime > 0) {
						continue;
					} else if (colliding.Contains(enemy)) {
						stillColliding.Add(enemy);
						continue;
					}

					float enemyspeed = enemy.LinearVelocity.Length();
					float speed = state.LinearVelocity.Length();
					
					float enemyInverseMass = 1/enemy.Mass;

					Vector2 axis = GlobalPosition - enemy.GlobalPosition;
					Vector2 relativeVelocity = LinearVelocity - enemy.LinearVelocity;
					Vector2 normal = axis.Normalized();
					float velocityAlongNormal = relativeVelocity.Dot(normal);
					
					if (enemyspeed > speed + speedEquivalency ) {
						GD.Print("Player ", playerNum, " died because its speed ", speed, " was less than ", enemyspeed);
						colliding.Add(this);
						enemy.colliding.Add(this);
						Die();
					} else if (enemyspeed >= speed - speedEquivalency) {
						if (velocityAlongNormal <= 0) {
							GD.Print("Player ", playerNum, " collided at same speed : ", speed, " and ", enemyspeed, " because ", enemyspeed, " >= ", speed - speedEquivalency, " and velnorm = ", velocityAlongNormal);
							float j = -(1 + playerBounciness) * velocityAlongNormal;
							j /= state.InverseMass + enemyInverseMass;
							Vector2 impulse = normal * j;
							state.LinearVelocity += impulse * state.InverseMass;
							enemy.LinearVelocity -= impulse * enemyInverseMass;
							colliding.Add(enemy);
							enemy.colliding.Add(this);
						}

						grappleDisabled = grappleBumpTime;
						grapple.Retract(false);
						enemy.grappleDisabled = grappleBumpTime;
						enemy.grapple.Retract(false);
					}
				}
			}
			for (int i = 0; i < colliding.Count; i++) {
				if (!stillColliding.Contains(colliding[i])) {
					colliding.RemoveAt(i);
					i--;
				}
			}
		}

		if (grapple.attachedBody != null) {
			if (invulTime == float.MaxValue) {
				invulTime = invulnerableSpawnTime;
			}
			Vector2 vel = state.LinearVelocity;
			RigidBody2D attachedBody = null;
			float acceleration = grapple.acceleration;
			bool doubleAttached = false;
			Vector2 attatchedPos = grapple.GlobalPosition;
			if (grapple.attachedBody is RigidBody2D) {
				attachedBody = (RigidBody2D)grapple.attachedBody;
				vel -= attachedBody.LinearVelocity;
				acceleration *= attachedBody.Mass / (Mass + attachedBody.Mass);
				if (attachedBody is Player) {
					if (((Player)attachedBody).grapple.attachedBody == this) {
						doubleAttached = true;
						acceleration /= 2;
					}
				}
				attatchedPos = attachedBody.Position;
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

	public void Die() {
		grapple.Retract(false);
		LinearVelocity = Vector2.Zero;
		GlobalPosition = Vector2.Zero;
		LinearVelocity = Vector2.Zero;
		invulTime = float.MaxValue;
		grappleDisabled = 0;
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

		if (grappleDisabled > 0) {
			grappleBar.Visible = true;
			grappleDisabled -= (float)delta;
			grappleBar.Value = grappleDisabled / grappleCutTime;
		} else {
			grappleBar.Visible = false;
		}
		if (invulTime > 0 && invulTime < float.MaxValue) {
			invulTime -= (float) delta;
		}
	}

	public override void _Draw() {
		DrawLine(Vector2.Zero, input.GetVector("grapple_left", "grapple_right", "grapple_up", "grapple_down").Normalized() * grapple.maxLength, Colors.Red, 5, true);
	}
}

using Godot;
using System;

public partial class Player : RigidBody2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	[Export]
	private Vector2 _thrust = new Vector2(0, -250);
	[Export]
    private float _torque = 20000;

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        if (Input.IsActionPressed("ui_up"))
            state.ApplyForce(_thrust.Rotated(Rotation));
        else
            state.ApplyForce(new Vector2());

        var rotationDir = 0;
        if (Input.IsActionPressed("ui_right"))
            rotationDir += 1;
        if (Input.IsActionPressed("ui_left"))
            rotationDir -= 1;
        state.ApplyTorque(rotationDir * _torque);
    }
}

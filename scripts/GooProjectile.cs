using Godot;
using Godot.Collections;

public partial class GooProjectile : RigidBody2D
{
    [Export]
	PackedScene gooScene;
    [Export]
	float attatchDist = 40;
    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        for (int i = 0; i < state.GetContactCount(); i++) {
            GodotObject obj = state.GetContactColliderObject(i);
            if (!(obj is Player || obj is GooProjectile)) {
                // Array<Node> grapples = FindChildren("Grapple", "Grapple", false);
                // foreach (var grapple in grapples) {
                //     //grapple.Reparent(GetTree().Root);
                //     ((Grapple)grapple).Retract();
                // }
                PhysicsBody2D attachedBody = (PhysicsBody2D)state.GetContactColliderObject(i);
                Node2D goo = gooScene.Instantiate<Node2D>();
                attachedBody.CallDeferred(Node.MethodName.AddChild, goo);
                goo.SetDeferred(Node2D.PropertyName.GlobalPosition, state.GetContactColliderPosition(i) + state.GetContactLocalNormal(i) * attatchDist);
                
                QueueFree();
            }
        }
    }
}

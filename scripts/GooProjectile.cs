using Godot;
using Godot.Collections;

public partial class GooProjectile : RigidBody2D
{
    [Export]
	PackedScene gooScene;
    [Export]
	float attatchDist = 40;
    [Export]
    float lifetime = 20;
    public override void _IntegrateForces(PhysicsDirectBodyState2D state) {
        for (int i = 0; i < state.GetContactCount(); i++) {
            GodotObject obj = state.GetContactColliderObject(i);
            if (!(obj is Player || obj is GooProjectile)) {
                Vector2 placePos = state.GetContactColliderPosition(i) + state.GetContactLocalNormal(i) * attatchDist;

                PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
                PhysicsPointQueryParameters2D query = new PhysicsPointQueryParameters2D();
                query.Position = placePos;
                //query.Exclude = new Array<Rid> { GetRid() };
                query.CollisionMask = CollisionMask;
                Array<Dictionary> result = spaceState.IntersectPoint(query);
                if (result.Count == 0) {
                    PhysicsBody2D attachedBody = (PhysicsBody2D)state.GetContactColliderObject(i);
                    Node2D goo = gooScene.Instantiate<Node2D>();
                    attachedBody.CallDeferred(Node.MethodName.AddChild, goo);
                    goo.SetDeferred(Node2D.PropertyName.GlobalPosition, placePos);

                    uint shapeIndex = (uint)state.GetContactColliderShape(i);
                    var collisionShapeNode = attachedBody.ShapeOwnerGetOwner(shapeIndex) as CollisionShape2D;
                    goo.SetDeferred(Goo.PropertyName.attachedBody, collisionShapeNode);
			    }
                QueueFree();
            }
        }
    }
    public override void _PhysicsProcess(double delta)
    {
        lifetime -= (float)delta;
        if (lifetime <= 0) {
            QueueFree();
        }
    }
}

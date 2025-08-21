using Godot;
using Godot.Collections;

public partial class GooGun : Sprite2D
{
    Player player;
    [Export]
	PackedScene gooScene;
    private float maxRange = 10000f;

    public override void _Ready()
    {
        player = GetNode<Player>("../../..");
    }

    public override void _Process(double delta) {
        if (player.input.IsActionJustPressed("attack")) {
            Vector2 dir;
            if (player.input.IsKeyboard()) {
				Vector2 mousepos = GetGlobalMousePosition();
				dir = (mousepos - GlobalPosition).Normalized();
			} else {
				dir = player.input.GetVector("grapple_left", "grapple_right", "grapple_up", "grapple_down").Normalized();
			}
            PhysicsDirectSpaceState2D spaceState = GetWorld2D().DirectSpaceState;
			PhysicsRayQueryParameters2D query = PhysicsRayQueryParameters2D.Create(player.GlobalPosition, player.GlobalPosition + dir * maxRange);
			query.Exclude = new Array<Rid> { player.GetRid() };
			query.CollisionMask = player.CollisionMask;
			Dictionary result = spaceState.IntersectRay(query);
            if (result.Count > 0) {
                PhysicsBody2D attachedBody = (PhysicsBody2D)result["collider"];
                Node2D goo = gooScene.Instantiate<Node2D>();
                attachedBody.AddChild(goo);
                goo.GlobalPosition = (Vector2)result["position"] - dir * 40;
            }
        }
    }
}

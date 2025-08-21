using Godot;
using Godot.Collections;

public partial class Border : Area2D
{
	GameManager gameManager;

    public override void _Ready()
    {
        gameManager = GetNode<GameManager>("/root/GameManager");
    }

    public override void _PhysicsProcess(double delta)
    {
        Array<Node2D> bodies = GetOverlappingBodies();
		foreach (var player in gameManager.playerNodes) {
            if (!bodies.Contains(player.Value)) {
                player.Value.Die();
            }
        }
    }
}

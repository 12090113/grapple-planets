using Godot;

public partial class Goo : CollisionShape2D
{
    public Node2D attachedBody;
    [Export]
    public float maxHealth = 1;
    [Export]
    public float decayTime = 10f;
    private float health;
    private float decayRate;
    private Sprite2D sprite;

    public override void _Ready()
    {
        health = maxHealth;
        decayRate = maxHealth / decayTime;
        sprite = GetNode<Sprite2D>("PlanetOutline");
    }

    public override void _Process(double delta)
    {
        Modulate = new Color(1, 1, health / maxHealth);
        health -= decayRate * (float)delta;
        if (!IsInstanceValid(attachedBody)) {
            RemoveChild(sprite);
            QueueFree();
        }
        if (health < 0) {
            QueueFree();
        }
    }
}

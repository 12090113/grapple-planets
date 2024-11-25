using Godot;

public partial class EnemySpawner : Timer
{
	[Export]
	private PackedScene enemyScene;
	[Export]
	private float intervalDecayRate = 0.9f;
	[Export]
	private float minInterval = 1f;
	[Export]
	private int maxEnemies = 30;
	private Camera2D camera;
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("../Player/Camera2D");
	}
	public void _on_timeout() {
		if (GetChildCount() < maxEnemies) {
			RigidBody2D enemy = enemyScene.Instantiate<RigidBody2D>();
			Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
			Vector2 pos = new Vector2((float)GD.RandRange(-viewportSize.X-100,viewportSize.X+100), viewportSize.Y + 100f);

			enemy.GlobalPosition = pos + camera.GlobalPosition;
			Callable.From(() => AddChild(enemy)).CallDeferred();
		}
		WaitTime *= intervalDecayRate;
		if (WaitTime < minInterval) {
			WaitTime = minInterval;
		}
	}
}

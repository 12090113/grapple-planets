using Godot;
using Godot.Collections;

public partial class Trail : Line2D
{
	private Array<Vector2> queue = new();
	[Export] public int MAX_LENGTH = 100;
	public Bullet bullet;

	public override void _Process(double delta)
	{
		if (IsInstanceValid(bullet)) {
			queue.Insert(0, bullet.Position);
		} else if (queue.Count < 1) {
			QueueFree();
			return;
		}
		if (queue.Count > MAX_LENGTH || !IsInstanceValid(bullet)) {
			queue.RemoveAt(queue.Count - 1);
		}
		ClearPoints();
		foreach (var point in queue) {
			AddPoint(point);
		}
	}
}
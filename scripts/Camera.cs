using Godot;

public partial class Camera : Camera2D
{
	[Export]
	private float deadZone = 160f;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion mouseEvent)
		{
			Vector2 target = mouseEvent.Position - GetViewport().GetVisibleRect().Size * 0.5f;

			if (target.Length() < deadZone)
			{
				Position = Vector2.Zero;
			}
			else
			{
				Position = target.Normalized() * (target.Length() - deadZone) * 0.5f;
			}
		}
	}
}

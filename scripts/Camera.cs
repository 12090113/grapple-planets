using Godot;
using Godot.Collections;

public partial class Camera : Camera2D
{
	private Dictionary<int, Player> _players;

    [Export] public float PaddingSize { get; set; } = 1000f;
    [Export] public float CameraSpeed { get; set; } = 20f;
    [Export] public float ZoomSpeed { get; set; } = 20f;
    [Export] public float ZoomOutMin { get; set; } = 0.0001f;
    [Export] public float ZoomOutMax { get; set; } = 10f;

    private float _width = 0f;
    private float _height = 0f;

    public override void _Ready()
    {
        // Example of getting players from a GameManager autoload singleton.
        // Adjust this line to how you actually access your player list.
        _players = GetNode<GameManager>("/root/GameManager").playerNodes;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_players.Count == 0)
        {
            return;
        }

        // --- Calculate Bounding Box ---
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (var player in _players)
        {
            Node2D playerNode = player.Value;
            minX = Mathf.Min(minX, playerNode.GlobalPosition.X);
            maxX = Mathf.Max(maxX, playerNode.GlobalPosition.X);
            minY = Mathf.Min(minY, playerNode.GlobalPosition.Y);
            maxY = Mathf.Max(maxY, playerNode.GlobalPosition.Y);
        }

        // --- Update Camera Position ---
        var targetPosition = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
        Vector2 cameraMoveVelocity = targetPosition - GlobalPosition;
        GlobalPosition += cameraMoveVelocity * CameraSpeed * (float)delta;

        // --- Calculate Target Zoom ---
        _width = maxX - minX + PaddingSize;
        _height = maxY - minY + PaddingSize;

        Rect2 viewportRect = GetViewport().GetVisibleRect();
        float targetZoom = Mathf.Min(viewportRect.Size.X / _width, viewportRect.Size.Y / _height);
        targetZoom = Mathf.Clamp(targetZoom, ZoomOutMin, ZoomOutMax);
        
        // --- Apply Zoom ---
        Vector2 newZoom = Zoom;
        newZoom.X = Mathf.Lerp(Zoom.X, targetZoom, ZoomSpeed * (float)delta);
        newZoom.Y = newZoom.X;
        Zoom = newZoom;
        /*
        // --- Update Player Pointers ---
        // This part updates an "arrow" on each player to keep its on-screen
        // size consistent and make it fade in as the camera zooms out.
        foreach (Player person in _players)
        {
            CanvasItem pointer = person.Arrow;
            if (pointer != null)
            {
                pointer.Scale = new Vector2(1 / Zoom.X * 0.2f, 1 / Zoom.Y * 0.2f);
                float newAlpha = Mathf.Clamp((1 / Zoom.X * 0.5f) - 2f, 0f, 0.5f);
                pointer.Modulate = new Color(1, 1, 1, newAlpha);
            }
        }*/
    }
}

using Godot;

public partial class DraggableSprite : Sprite2D
{
    // This flag will be true when we are holding down the mouse button on the sprite.
    private bool _isDragging = false;
    
    // This stores the difference between the mouse position and the sprite's position.
    // It prevents the sprite from "snapping" its center to the mouse cursor.
    private Vector2 _offset;

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// We use this to update the sprite's position while it's being dragged.
    /// </summary>
    public override void _Process(double delta)
    {
        // If the dragging flag is true, update the sprite's position.
        if (_isDragging)
        {
            // Set the sprite's global position to the current mouse position,
            // adjusted by the offset we calculated on click.
            this.GlobalPosition = GetGlobalMousePosition() - _offset;
        }
    }

    /// <summary>
    /// Called when an input event occurs. We use this to detect mouse clicks.
    /// </summary>
    public override void _Input(InputEvent @event)
    {
        // We are only interested in mouse button events.
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            // Case 1: The left mouse button is pressed.
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left && mouseButtonEvent.IsPressed())
            {
                // Check if the mouse click is within the sprite's bounding box.
                // GetRect() returns a Rect2 representing the sprite's boundary in its own local coordinates.
                // HasPoint() checks if a point is inside this rectangle. We must provide the mouse
                // position in the sprite's local coordinate space.
                if (GetRect().HasPoint(ToLocal(GetGlobalMousePosition())))
                {
                    // The click is on the sprite, so we start dragging.
                    _isDragging = true;
                    
                    // Calculate the offset. This is the magic part that makes dragging
                    // feel natural. It's the vector from the sprite's origin to the mouse click position.
                    _offset = GetGlobalMousePosition() - this.GlobalPosition;
                }
            }
            // Case 2: The left mouse button is released.
            else if (mouseButtonEvent.ButtonIndex == MouseButton.Left && !mouseButtonEvent.IsPressed())
            {
                // Stop dragging.
                _isDragging = false;
            }
        }
    }
}
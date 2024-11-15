using Godot;

[Tool]
public partial class Planet : StaticBody2D
{
	Texture2D texture;
	Texture2D outline;

	[Export]
	public Texture2D Texture
	{
		get => texture;
		set
		{
			texture = value;
			Sprite2D sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
			if (sprite != null) sprite.Texture = texture;
		}
	}
	
	[Export]
	public Texture2D Outline
	{
		get => outline;
		set
		{
			outline = value;
			Sprite2D sprite = GetNodeOrNull<Sprite2D>("Sprite2D/Outline");
			if (sprite != null) sprite.Texture = texture;
		}
	}
}

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
			GetNode<Sprite2D>("Sprite2D").Texture = texture;
		}
	}
	
	[Export]
	public Texture2D Outline
	{
		get => outline;
		set
		{
			outline = value;
			GetNode<Sprite2D>("Sprite2D/Outline").Texture = outline;
		}
	}
}

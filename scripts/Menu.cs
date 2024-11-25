using Godot;

public partial class Menu : Control
{
	private TextureRect help;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		help = GetNode<TextureRect>("HelpMenu");
	}	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnStartButtonPressed()
	{
		Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/main.tscn")).CallDeferred();
		GetTree().ReloadCurrentScene();
	}

	private void _on_help_button_pressed()
	{
		help.Visible = true;
	}

	private void _on_close_button_pressed()
	{
		help.Visible = false;
	}
}

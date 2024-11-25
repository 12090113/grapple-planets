using Godot;

public partial class Menu : Control
{
	private TextureRect help;
	private TextureRect credits;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		help = GetNode<TextureRect>("HelpMenu");
		credits = GetNode<TextureRect>("Credits");
	}	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnStartButtonPressed()
	{
		Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/main.tscn")).CallDeferred();
	}

	private void _on_help_button_pressed()
	{
		help.Visible = true;
	}

	private void _on_help_close_button_pressed()
	{
		help.Visible = false;
	}

	private void _on_credits_button_pressed()
	{
		credits.Visible = true;
	}

	private void _on_credits_close_button_pressed()
	{
		credits.Visible = false;
	}
}

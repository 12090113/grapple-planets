using Godot;

public partial class Menu : Control
{
	private Node help;
	private void OnStartButtonPressed()
	{
		Callable.From(() => GetTree().ChangeSceneToFile("res://scenes/main.tscn")).CallDeferred();
		GetTree().ReloadCurrentScene();
	}

	private void OnHelpButtonPressed()
	{
		//help;
	}
}

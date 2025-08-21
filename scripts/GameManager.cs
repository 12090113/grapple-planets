using Godot;
using Godot.Collections;

public partial class GameManager : Node {
	/// <summary>
	/// this is a singleton autoload in my project but for the purposes of this demo,
	/// this is simpler
	/// </summary>
	private PlayerManager _playerManager;

	/// <summary>map from player integer to the player node</summary>
	public Dictionary<int, Player>playerNodes { get; private set; } = new ();

	public override void _Ready() {
		_playerManager = GetNode<PlayerManager>("/root/PlayerManager");
		_playerManager.PlayerJoined += player => SpawnPlayer(player);
		_playerManager.PlayerLeft += player => DeletePlayer(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		_playerManager.HandleJoinInput();
	}

	private void SpawnPlayer(int player) {
		var playerScene = GD.Load<PackedScene>("res://scenes/player.tscn");
		var playerNode = playerScene.Instantiate<Player>();
		playerNode.Leave += (player) => { OnPlayerLeave(player); };
		playerNodes[player] = playerNode;

		// let the player know which device controls it
		var device = _playerManager.GetPlayerDevice(player);
		playerNode.Init(player, device);

		// add the player to the tree
		AddChild(playerNode);

		playerNode.Position = Vector2.Zero;
	}

	private void DeletePlayer(int player) {
		playerNodes[player].QueueFree();
		playerNodes.Remove(player);
	}

	public void OnPlayerLeave(int player) {
		// just let the player manager know this player is leaving
		// this will, through the player manager's "player_left" signal,
		// indirectly call delete_player because it's connected in this file's _ready()
		_playerManager.Leave(player);
	}
}

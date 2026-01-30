using Godot;
using System;

public partial class GameManager : Node
{
	/// <summary>
	/// Reference to the house scene to spawn. Set this in the editor.
	/// </summary>
	[Export] public PackedScene HouseScene { get; set; }

	/// <summary>
	/// Reference to the player scene to spawn. Set this in the editor.
	/// </summary>
	[Export] public PackedScene PlayerScene { get; set; }

	private House _house;
	private Player _player;

	public override void _Ready()
	{
		SpawnHouse();
		SpawnPlayer();
	}

	private void SpawnHouse()
	{
		if (HouseScene == null)
		{
			GD.PrintErr("GameManager: HouseScene is not set! Please assign it in the editor.");
			return;
		}

		_house = HouseScene.Instantiate<House>();
		AddChild(_house);
	}

	private void SpawnPlayer()
	{
		if (PlayerScene == null)
		{
			GD.PrintErr("GameManager: PlayerScene is not set! Please assign it in the editor.");
			return;
		}

		if (_house == null)
		{
			GD.PrintErr("GameManager: Cannot spawn player - house was not spawned!");
			return;
		}

		if (_house.Spawnpoint == null)
		{
			GD.PrintErr("GameManager: Spawnpoint not set on House! Please assign it in the editor.");
			return;
		}

		_player = PlayerScene.Instantiate<Player>();
		AddChild(_player);
		_player.GlobalPosition = _house.Spawnpoint.GlobalPosition;
	}

	public override void _Process(double delta)
	{
	}
}

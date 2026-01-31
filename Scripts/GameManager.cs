using Godot;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Export] public PackedScene HouseScene { get; set; }
	[Export] public PackedScene PlayerScene { get; set; }
	[Export] public Control PickupUIElement { get; set; }
	[Export] public Control SetMaskUIElement { get; set; }

	private House _house;
	private Player _player;

	public override void _Ready()
	{
		Instance = this;
		ShowPickupUI(false);
		ShowSetMaskUI(false);
		SpawnHouse();
		SpawnPlayer();
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void ShowPickupUI(bool show)
	{
		if (PickupUIElement == null)
		{
			return;
		}

		PickupUIElement.Visible = show;
	}

	public void ShowSetMaskUI(bool show)
	{
		if (SetMaskUIElement == null)
		{
			return;
		}

		SetMaskUIElement.Visible = show;
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

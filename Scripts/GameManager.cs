using Godot;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	[Export] public PackedScene HouseScene { get; set; }
	[Export] public PackedScene PlayerScene { get; set; }
	[Export] public Control PickupUIElement { get; set; }
	[Export] public Control SetMaskUIElement { get; set; }
	[Export] public UiSleepBar SleepBarUI { get; set; }
	[Export] public UiScore ScoreUI { get; set; }

	[Export] public float SleepFillRate { get; set; } = 10f;
	[Export] public float SleepDrainRate { get; set; } = 15f;
	[Export] public float ScorePerSecond { get; set; } = 100f;

	public MaskType CurrentEvent { get; private set; } = MaskType.None;
	public float Score { get; private set; }

	private House _house;
	private Player _player;
	private BedArea _bedArea;

	public override void _Ready()
	{
		Instance = this;
		ShowPickupUI(false);
		ShowSetMaskUI(false);
		SpawnHouse();
		SpawnPlayer();
		DebugReferences();
	}

	private void DebugReferences()
	{
		if (SleepBarUI == null)
		{
			GD.PrintErr("GameManager: SleepBarUI is not assigned!");
		}
		else if (SleepBarUI.Slider == null)
		{
			GD.PrintErr("GameManager: SleepBarUI.Slider is not assigned!");
		}
		else
		{
			GD.Print($"GameManager: SleepBarUI OK - Value: {SleepBarUI.GetValue()}");
		}
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
		_bedArea = _house.Bed?.Area;
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
		UpdateSleep((float)delta);
		UpdateScore((float)delta);
	}

	public void SetEvent(MaskType requiredMask)
	{
		CurrentEvent = requiredMask;
		GD.Print($"Event started: requires {requiredMask} mask");
	}

	public void ClearEvent()
	{
		CurrentEvent = MaskType.None;
		GD.Print("Event cleared");
	}

	public bool HasCorrectMask()
	{
		if (_bedArea == null)
		{
			return false;
		}

		return _bedArea.CurrentMask == CurrentEvent;
	}

	private void UpdateSleep(float delta)
	{
		if (SleepBarUI == null)
		{
			return;
		}

		bool shouldFill = CurrentEvent == MaskType.None || HasCorrectMask();

		if (shouldFill)
		{
			SleepBarUI.IncreaseValue(SleepFillRate * delta);
		}
		else
		{
			SleepBarUI.DecreaseValue(SleepDrainRate * delta);
		}
	}

	private void UpdateScore(float delta)
	{
		if (SleepBarUI == null)
		{
			return;
		}

		float sleepPercent = SleepBarUI.GetNormalizedValue();
		Score += ScorePerSecond * sleepPercent * delta;

		ScoreUI?.UpdateScore(Score);
	}
}

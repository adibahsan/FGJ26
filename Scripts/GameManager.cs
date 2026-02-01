using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	public static bool LastGameWon { get; private set; }
	public static int LastFinalScore { get; private set; }

	[Export] public PackedScene HouseScene { get; set; }
	[Export] public PackedScene PlayerScene { get; set; }
	[Export] public PackedScene[] MinigameScenes { get; set; }
	[Export] public PackedScene GameOverScene { get; set; }
	[Export] public Control PickupUIElement { get; set; }
	[Export] public Control SetMaskUIElement { get; set; }
	[Export] public UiSleepBar SleepBarUI { get; set; }
	[Export] public UiScore ScoreUI { get; set; }
	[Export] public EventAudioData[] EventAudioDataList { get; set; }

	[Export] public float SleepFillRate { get; set; } = 10f;
	[Export] public float SleepDrainRate { get; set; } = 15f;
	[Export] public float ScorePerSecond { get; set; } = 100f;

	[Export] public float NightDurationSeconds { get; set; } = 300f;
	[Export] public float NightProgress { get; set; } = 0f;

	[Export] public MaskType CurrentEvent { get; private set; } = MaskType.None;
	public MaskType CurrentPickupMask { get; private set; } = MaskType.None;
	public float Score { get; private set; }

	private House _house;
	private Player _player;
	private BedArea _bedArea;
	private MinigameBase _activeMinigame;
	private List<PackedScene> _shuffledMinigames;
	private int _minigameIndex;
	private bool _gameEnded;

	public override void _Ready()
	{
		Instance = this;
		ValidateExports();
		ShowPickupUI(false);
		ShowSetMaskUI(false);
		SpawnHouse();
		SpawnPlayer();
		ShuffleMinigames();
	}

	private void ValidateExports()
	{
		if (HouseScene == null)
		{
			GD.PrintErr("GameManager: HouseScene is not assigned!");
		}

		if (PlayerScene == null)
		{
			GD.PrintErr("GameManager: PlayerScene is not assigned!");
		}

		if (MinigameScenes == null || MinigameScenes.Length == 0)
		{
			GD.PrintErr("GameManager: MinigameScenes is not assigned!");
		}

		if (GameOverScene == null)
		{
			GD.PrintErr("GameManager: GameOverScene is not assigned!");
		}

		if (PickupUIElement == null)
		{
			GD.PrintErr("GameManager: PickupUIElement is not assigned!");
		}

		if (SetMaskUIElement == null)
		{
			GD.PrintErr("GameManager: SetMaskUIElement is not assigned!");
		}

		if (SleepBarUI == null)
		{
			GD.PrintErr("GameManager: SleepBarUI is not assigned!");
		}

		if (ScoreUI == null)
		{
			GD.PrintErr("GameManager: ScoreUI is not assigned!");
		}
	}

	private void ShuffleMinigames()
	{
		_minigameIndex = 0;
		_shuffledMinigames = ShuffleHelper.ToShuffledList(MinigameScenes);
		GD.Print($"GameManager: Shuffled {_shuffledMinigames.Count} minigames");
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void ShowPickupUI(bool show, MaskType maskType = MaskType.None)
	{
		CurrentPickupMask = show ? maskType : MaskType.None;
		PickupUIElement.Visible = show;
	}

	public void ShowSetMaskUI(bool show)
	{
		SetMaskUIElement.Visible = show;
	}

	private void SpawnHouse()
	{
		_house = HouseScene.Instantiate<House>();
		AddChild(_house);
		_bedArea = _house.Bed?.Area;
	}

	private void SpawnPlayer()
	{
		_player = PlayerScene.Instantiate<Player>();
		AddChild(_player);
		_player.GlobalPosition = _house.Spawnpoint.GlobalPosition;
	}

	public override void _Process(double delta)
	{
		if (_gameEnded)
		{
			return;
		}

		UpdateNightProgress((float)delta);
		UpdateSleep((float)delta);
		UpdateScore((float)delta);
	}

	private void UpdateNightProgress(float delta)
	{
		if (NightDurationSeconds <= 0.0f)
		{
			return;
		}

		NightProgress = Mathf.Clamp(NightProgress + (delta / NightDurationSeconds), 0.0f, 1.0f);

		if (NightProgress >= 1.0f)
		{
			EndGame(true);
		}
	}

	public void SetEvent(MaskType requiredMask)
	{
		CurrentEvent = requiredMask;
		GD.Print($"Event started: requires {requiredMask} mask");

		EventAudioData audioData = GetEventAudioData(requiredMask);
		var audio = global::PlayerAudio.Instance;
		if (audioData != null && audio != null)
		{
			audio.PlayEventAudio(audioData);
		}
	}

	private EventAudioData GetEventAudioData(MaskType maskType)
	{
		if (EventAudioDataList == null)
		{
			return null;
		}

		foreach (EventAudioData data in EventAudioDataList)
		{
			if (data != null && data.EventType == maskType)
			{
				return data;
			}
		}

		return null;
	}

	public void ClearEvent()
	{
		CurrentEvent = MaskType.None;
		GD.Print("Event cleared");
	}

	public void ClearBedMask()
	{
		_bedArea?.ClearMask();
	}

	public bool HasCorrectMask()
	{
		if (_bedArea == null)
		{
			return false;
		}

		return _bedArea.CurrentMask == CurrentEvent;
	}

	public void OnMaskApplied(MaskType maskType)
	{
		// If the correct mask was applied for the current event, stop the looping SFX immediately.
			if (maskType == CurrentEvent && CurrentEvent == MaskType.Cpap)
		{
			var audio = global::PlayerAudio.Instance;
			audio?.StopLoopingAudio();
			GD.Print($"Correct mask applied ({maskType}), stopped looping audio");
		}
		if (maskType == CurrentEvent && CurrentEvent == MaskType.Scary)
		{
			var audio = global::PlayerAudio.Instance;
			audio?.StopLoopingAudio();
			GD.Print($"Correct mask applied ({maskType}), stopped looping audio");
		}
		if (maskType == CurrentEvent && CurrentEvent == MaskType.FakeEyeGlasses)
		{
			var audio = global::PlayerAudio.Instance;
			audio?.StopLoopingAudio();
			GD.Print($"Correct mask applied ({maskType}), stopped looping audio");
		}
	}

	public void StartNextMinigame()
	{
		if (_minigameIndex >= _shuffledMinigames.Count)
		{
			ShuffleMinigames();
		}

		PackedScene scene = _shuffledMinigames[_minigameIndex];
		_minigameIndex++;

		MinigameBase minigame = scene.Instantiate<MinigameBase>();
		AddChild(minigame);
		SetActiveMinigame(minigame);
		minigame.StartMinigame();

		GD.Print($"GameManager: Started minigame {_minigameIndex}/{_shuffledMinigames.Count}");
	}

	public void SetActiveMinigame(MinigameBase minigame)
	{
		_activeMinigame = minigame;
	}

	public void ClearActiveMinigame()
	{
		_activeMinigame = null;
		ApplyCarriedMask();
	}

	private void ApplyCarriedMask()
	{
		if (_player == null || !_player.IsCarryingMask)
		{
			return;
		}

		if (_bedArea == null)
		{
			return;
		}

		Mask mask = _player.CarriedMask;
		_player.DropMask();
		_bedArea.SetMask(mask);
	}

	public bool OverrideInput()
	{
		if (_activeMinigame != null)
		{
			_activeMinigame.HandleInput();
			return true;
		}
			
		return false;
	}

	private void UpdateSleep(float delta)
	{
		bool shouldFill = CurrentEvent == MaskType.None || HasCorrectMask();

		if (shouldFill)
		{
			SleepBarUI.IncreaseValue(SleepFillRate * delta);
		}
		else
		{
			SleepBarUI.DecreaseValue(SleepDrainRate * delta);
		}

		if (SleepBarUI.GetValue() <= 0.0f)
		{
			EndGame(false);
		}
	}

	private void UpdateScore(float delta)
	{
		float sleepPercent = SleepBarUI.GetNormalizedValue();
		Score += ScorePerSecond * sleepPercent * delta;
		ScoreUI.UpdateScore(Score);
	}

	public void EndGame(bool won)
	{
		if (_gameEnded)
		{
			return;
		}

		_gameEnded = true;
		LastGameWon = won;
		LastFinalScore = (int)Score;
		GD.Print($"Game Over! Won={won}, Final Score: {LastFinalScore}");

		// Make sure any looping event SFX doesn't carry over into menus / game over.
		var audio = global::PlayerAudio.Instance;
		audio?.StopLoopingAudio();

		if (GameOverScene == null)
		{
			GD.PrintErr("GameManager: GameOverScene is not assigned, can't switch to game over screen.");
			return;
		}

		GetTree().ChangeSceneToPacked(GameOverScene);
	}

	public void OnGameEnd()
	{
		EndGame(true);
	}
}

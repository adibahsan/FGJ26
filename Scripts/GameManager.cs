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
	[Export] public Control EventGuideUIElement { get; set; }
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

	private bool _wantsPickupUI;
	private bool _wantsSetMaskUI;

	public override void _Ready()
	{
		Instance = this;
		ValidateExports();
		HideAllPromptUI();
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

		if (EventGuideUIElement == null)
		{
			GD.PrintErr("GameManager: EventGuideUIElement is not assigned!");
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
		_wantsPickupUI = show;
		UpdatePromptUI();
	}

	public void ShowSetMaskUI(bool show)
	{
		_wantsSetMaskUI = show;
		UpdatePromptUI();
	}

	private void HideAllPromptUI()
	{
		if (PickupUIElement != null) PickupUIElement.Visible = false;
		if (SetMaskUIElement != null) SetMaskUIElement.Visible = false;
		if (EventGuideUIElement != null) EventGuideUIElement.Visible = false;
	}

	private void UpdatePromptUI()
	{
		HideAllPromptUI();

		// Priority: SetMaskUI > PickupUI > EventGuideUI
		if (_wantsSetMaskUI)
		{
			if (SetMaskUIElement != null) SetMaskUIElement.Visible = true;
		}
		else if (_wantsPickupUI)
		{
			if (PickupUIElement != null) PickupUIElement.Visible = true;
		}
		else if (ShouldShowEventGuide())
		{
			if (EventGuideUIElement != null) EventGuideUIElement.Visible = true;
		}
	}

	private bool ShouldShowEventGuide()
	{
		// Show guide when: event is active, player not carrying mask, sleeper doesn't have correct mask
		if (CurrentEvent == MaskType.None)
		{
			return false;
		}

		if (_player != null && _player.IsCarryingMask)
		{
			return false;
		}

		if (HasCorrectMask())
		{
			return false;
		}

		return true;
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
		UpdatePromptUI();
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

	public EventAudioData GetEventAudioData(MaskType maskType)
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
		if (maskType != CurrentEvent)
		{
			return;
		}

		var audio = global::PlayerAudio.Instance;
		EventAudioData audioData = GetEventAudioData(maskType);

		// Play mask equipped sound if available
		if (audioData != null && audioData.MaskEquippedSound != null && audio != null)
		{
			audio.PlayOneShotSound(audioData.MaskEquippedSound);
			GD.Print($"Correct mask applied ({maskType}), played mask equipped sound");
		}

		// Handle event-specific audio behavior
		if (CurrentEvent == MaskType.Cpap || CurrentEvent == MaskType.Scary)
		{
			audio?.StopLoopingAudio();
			GD.Print($"Correct mask applied ({maskType}), stopped looping audio");
		}
		else if (CurrentEvent == MaskType.FakeEyeGlasses)
		{
			audio?.PlaySecondaryLoopingAudio();
			GD.Print($"Correct mask applied ({maskType}), played secondary looping audio");
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

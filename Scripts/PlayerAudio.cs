using Godot;

public partial class PlayerAudio : Node
{
	public static PlayerAudio Instance { get; private set; }

	[Export] public AudioStreamPlayer[] MusicPlayers { get; set; }
	[Export] public AudioStreamPlayer SFXPlayer { get; set; }
	[Export] public AudioStreamPlayer SFXPlayerLoop { get; set; }
	[Export] public float VolumeTransitionSpeed { get; set; } = 1f;
	[Export] public float MinVolumeDb { get; set; } = -80f;
	[Export] public float MaxVolumeDb { get; set; } = 0f;
	[Export] public float HighSleepThreshold { get; set; } = 0.8f;
	[Export] public float MidSleepThreshold { get; set; } = 0.4f;

	private int _activePlayerIndex;
	private UiSleepBar _sleepBar;

	private EventAudioData currentEventData;

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			GD.PrintErr("PlayerAudio: Multiple instances detected. Music persistence may behave unexpectedly.");
		}

		Instance = this;
		ValidateExports();
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	private void ValidateExports()
	{
		if (MusicPlayers == null || MusicPlayers.Length < 3)
		{
			GD.PrintErr("PlayerAudio: MusicPlayers array must have at least 3 AudioStreamPlayers!");
		}

		if (MusicPlayers != null)
		{
			for (int i = 0; i < MusicPlayers.Length; i++)
			{
				if (MusicPlayers[i] == null)
				{
					GD.PrintErr($"PlayerAudio: MusicPlayers[{i}] is not assigned!");
				}
			}
		}

		if (SFXPlayer == null)
		{
			GD.PrintErr("PlayerAudio: SFXPlayer is not assigned!");
		}

		if (SFXPlayerLoop == null)
		{
			GD.PrintErr("PlayerAudio: SFXPlayerLoop is not assigned!");
		}
	}

	public override void _Process(double delta)
	{
		// If we're not in gameplay (menus/game over), force the default music layer (index 0).
		if (GameManager.Instance == null || GameManager.Instance.SleepBarUI == null)
		{
			_activePlayerIndex = 0;
		}
		else
		{
			UpdateActivePlayerIndex();
		}

		UpdateVolumes((float)delta);
	}

	private void UpdateActivePlayerIndex()
	{
		float sleepPercent = GameManager.Instance.SleepBarUI.GetNormalizedValue();

		if (sleepPercent > HighSleepThreshold)
		{
			_activePlayerIndex = 0;
		}
		else if (sleepPercent > MidSleepThreshold)
		{
			_activePlayerIndex = 1;
		}
		else
		{
			_activePlayerIndex = 2;
		}
	}

	private void UpdateVolumes(float delta)
	{
		float linearStep = VolumeTransitionSpeed * delta;
		float minLinear = Mathf.DbToLinear(MinVolumeDb);
		float maxLinear = Mathf.DbToLinear(MaxVolumeDb);

		for (int i = 0; i < MusicPlayers.Length; i++)
		{
			float targetLinear = i == _activePlayerIndex ? maxLinear : minLinear;
			float currentLinear = Mathf.DbToLinear(MusicPlayers[i].VolumeDb);
			float newLinear = Mathf.MoveToward(currentLinear, targetLinear, linearStep);

			MusicPlayers[i].VolumeDb = Mathf.LinearToDb(newLinear);
		}
	}

	public void PlayEventAudio(EventAudioData eventData)
	{
		if (eventData == null)
		{
			return;
		}
		
		currentEventData = eventData;

		if (eventData.OneShotSound != null && SFXPlayer != null)
		{
			SFXPlayer.Stream = eventData.OneShotSound;
			SFXPlayer.Play();
		}

		if (eventData.LoopingSound != null && SFXPlayerLoop != null)
		{
			SFXPlayerLoop.Stream = eventData.LoopingSound;
			SFXPlayerLoop.Play();
		}
	}

	public void PlaySecondaryLoopingAudio()
	{
		if (currentEventData.LoopingSound != null && SFXPlayerLoop != null)
		{
			SFXPlayerLoop.Stream = currentEventData.SecondaryLoopingSound;
			SFXPlayerLoop.Play();
		}
		else
		{
			StopLoopingAudio();
		}
	}
	
	public void StopLoopingAudio()
	{
		if (SFXPlayerLoop != null)
		{
			SFXPlayerLoop.Stop();
		}
	}
}

using Godot;

public partial class PlayerAudio : Node
{
	[Export] public AudioStreamPlayer[] MusicPlayers { get; set; }
	[Export] public float VolumeTransitionSpeed { get; set; } = 1f;
	[Export] public float MinVolumeDb { get; set; } = -80f;
	[Export] public float MaxVolumeDb { get; set; } = 0f;
	[Export] public float HighSleepThreshold { get; set; } = 0.8f;
	[Export] public float MidSleepThreshold { get; set; } = 0.4f;

	private int _activePlayerIndex;
	private UiSleepBar _sleepBar;

	public override void _Ready()
	{
		ValidateExports();
	}

	private void ValidateExports()
	{
		if (MusicPlayers == null || MusicPlayers.Length < 3)
		{
			GD.PrintErr("PlayerAudio: MusicPlayers array must have at least 3 AudioStreamPlayers!");
			return;
		}

		for (int i = 0; i < MusicPlayers.Length; i++)
		{
			if (MusicPlayers[i] == null)
			{
				GD.PrintErr($"PlayerAudio: MusicPlayers[{i}] is not assigned!");
			}
		}
	}

	public override void _Process(double delta)
	{
		UpdateActivePlayerIndex();
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
}

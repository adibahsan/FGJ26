using Godot;

public partial class AudioSettingsUI : Control
{
	[Export] public Button MuteButton { get; set; }
	[Export] public HSlider VolumeSlider { get; set; }

	public override void _Ready()
	{
		if (MuteButton == null)
		{
			MuteButton = GetNodeOrNull<Button>("MuteButton");
			if (MuteButton == null)
			{
				GD.PrintErr("AudioSettingsUI: MuteButton is not assigned and could not be found by name!");
			}
		}

		if (VolumeSlider == null)
		{
			VolumeSlider = GetNodeOrNull<HSlider>("VolumeSlider");
			if (VolumeSlider == null)
			{
				GD.PrintErr("AudioSettingsUI: VolumeSlider is not assigned and could not be found by name!");
			}
		}

		if (AudioSettings.Instance == null)
		{
			var settings = new AudioSettings();
			GetTree().Root.CallDeferred(Node.MethodName.AddChild, settings);
			CallDeferred(nameof(RefreshAudioUIAfterInit));
		}
		else
		{
			SyncFromSettings();
		}

		if (MuteButton != null)
		{
			MuteButton.Pressed += OnMutePressed;
		}

		if (VolumeSlider != null)
		{
			VolumeSlider.ValueChanged += OnVolumeChanged;
		}
	}

	private void RefreshAudioUIAfterInit()
	{
		SyncFromSettings();
	}

	private void SyncFromSettings()
	{
		if (AudioSettings.Instance == null)
		{
			return;
		}

		UpdateMuteButtonText();
		if (VolumeSlider != null)
		{
			VolumeSlider.Value = AudioSettings.Instance.VolumeLinear * 100f;
		}
	}

	private void UpdateMuteButtonText()
	{
		if (MuteButton == null || AudioSettings.Instance == null)
		{
			return;
		}

		MuteButton.Text = AudioSettings.Instance.IsMuted ? "Unmute" : "Mute";
	}

	private void OnMutePressed()
	{
		if (AudioSettings.Instance == null)
		{
			return;
		}

		AudioSettings.Instance.SetMuted(!AudioSettings.Instance.IsMuted);
		UpdateMuteButtonText();
	}

	private void OnVolumeChanged(double value)
	{
		if (AudioSettings.Instance == null)
		{
			return;
		}

		AudioSettings.Instance.SetVolumeLinear((float)value / 100f);
	}
}

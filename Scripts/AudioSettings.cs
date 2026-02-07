using Godot;

public partial class AudioSettings : Node
{
	public static AudioSettings Instance { get; private set; }

	private const string SavePath = "user://audio_settings.cfg";
	private const string Section = "audio";
	private const string KeyMuted = "muted";
	private const string KeyVolumeLinear = "volume_linear";
	private const float MinVolumeDb = -80f;

	public bool IsMuted { get; private set; }
	public float VolumeLinear { get; private set; } = 1f;

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			GD.PrintErr("AudioSettings: Multiple instances detected.");
			return;
		}

		Instance = this;
		LoadAndApply();
	}

	public override void _ExitTree()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}

	public void SetMuted(bool muted)
	{
		if (IsMuted == muted)
		{
			return;
		}

		IsMuted = muted;
		ApplyToBus();
		Save();
	}

	public void SetVolumeLinear(float linear)
	{
		float clamped = Mathf.Clamp(linear, 0f, 1f);
		if (Mathf.IsEqualApprox(VolumeLinear, clamped))
		{
			return;
		}

		VolumeLinear = clamped;
		ApplyToBus();
		Save();
	}

	private void LoadAndApply()
	{
		var config = new ConfigFile();
		var err = config.Load(SavePath);
		if (err == Error.Ok)
		{
			if (config.HasSectionKey(Section, KeyMuted))
			{
				IsMuted = (bool)config.GetValue(Section, KeyMuted);
			}

			if (config.HasSectionKey(Section, KeyVolumeLinear))
			{
				VolumeLinear = Mathf.Clamp((float)config.GetValue(Section, KeyVolumeLinear), 0f, 1f);
			}
		}

		ApplyToBus();
	}

	private void ApplyToBus()
	{
		int busIndex = AudioServer.GetBusIndex("Master");
		if (busIndex < 0)
		{
			busIndex = 0;
		}

		AudioServer.SetBusMute(busIndex, IsMuted);

		float db = IsMuted || VolumeLinear <= 0f
			? MinVolumeDb
			: Mathf.LinearToDb(Mathf.Max(VolumeLinear, 0.0001f));
		AudioServer.SetBusVolumeDb(busIndex, db);
	}

	private void Save()
	{
		var config = new ConfigFile();
		config.SetValue(Section, KeyMuted, IsMuted);
		config.SetValue(Section, KeyVolumeLinear, VolumeLinear);
		config.Save(SavePath);
	}
}

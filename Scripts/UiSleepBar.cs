using Godot;

public partial class UiSleepBar : Control
{
	[Export] public ProgressBar ProgressBar { get; set; }
	[Export] public float StartValue { get; set; } = 50f;

	private float _sleepValue;

	public override void _Ready()
	{
		if (ProgressBar == null)
		{
			GD.PrintErr("UiSleepBar: ProgressBar is not assigned! Please assign it in the editor.");
		}

		_sleepValue = Mathf.Clamp(StartValue, 0f, 100f);
		UpdateProgressBar();
	}

	public void IncreaseValue(float amount)
	{
		_sleepValue = Mathf.Clamp(_sleepValue + amount, 0f, 100f);
		UpdateProgressBar();
	}

	public void DecreaseValue(float amount)
	{
		_sleepValue = Mathf.Clamp(_sleepValue - amount, 0f, 100f);
		UpdateProgressBar();
	}

	public float GetValue()
	{
		return _sleepValue;
	}

	public float GetNormalizedValue()
	{
		return _sleepValue / 100f;
	}

	private void UpdateProgressBar()
	{
		ProgressBar.Value = _sleepValue / 100f;
		GD.Print("UpdateProgressBar: ", _sleepValue);
		GD.Print("ProgressBar: ", ProgressBar.Value);
	}
}

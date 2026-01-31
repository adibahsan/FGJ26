using Godot;

public partial class UiSleepBar : Control
{
	[Export] public Slider Slider { get; set; }
	[Export] public float StartValue { get; set; } = 50f;

	private float _sleepValue;

	public override void _Ready()
	{
		if (Slider == null)
		{
			GD.PrintErr("UiSleepBar: Slider is not assigned! Please assign it in the editor.");
		}

		_sleepValue = Mathf.Clamp(StartValue, 0f, 100f);
		UpdateSlider();
	}

	public void IncreaseValue(float amount)
	{
		_sleepValue = Mathf.Clamp(_sleepValue + amount, 0f, 100f);
		UpdateSlider();
	}

	public void DecreaseValue(float amount)
	{
		_sleepValue = Mathf.Clamp(_sleepValue - amount, 0f, 100f);
		UpdateSlider();
	}

	public float GetValue()
	{
		return _sleepValue;
	}

	public float GetNormalizedValue()
	{
		return _sleepValue / 100f;
	}

	private void UpdateSlider()
	{
		Slider.Value = _sleepValue;
	}
}

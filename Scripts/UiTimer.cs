using Godot;

public partial class UiTimer : Control
{
	[Export] public Control RotatingControl { get; set; }
	[Export] public float StartAngle { get; set; } = -90f;
	[Export] public float EndAngle { get; set; } = 90f;

	public override void _Ready()
	{
		if (RotatingControl == null)
		{
			GD.PrintErr("UiTimer: RotatingControl is not assigned!");
			return;
		}

		RotatingControl.RotationDegrees = StartAngle;
	}

	public override void _Process(double delta)
	{
		GameManager gm = GameManager.Instance;
		if (gm == null || RotatingControl == null)
		{
			return;
		}

		float t = Mathf.Clamp(gm.NightProgress, 0.0f, 1.0f);
		RotatingControl.RotationDegrees = Mathf.Lerp(StartAngle, EndAngle, t);
	}
}

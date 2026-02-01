using Godot;

public partial class MinigameRotator : MinigameBase
{
	[Export] public ProgressBar FillBar { get; set; }
	[Export] public Control RotatingVisual { get; set; }

	[Export] public float TargetValue { get; set; } = 1.0f;
	[Export] public float MinProgressPerPress { get; set; } = 0.035f;
	[Export] public float MaxProgressPerPress { get; set; } = 0.075f;

	[Export] public float RotationDurationSeconds { get; set; } = 0.12f;
	[Export] public Tween.TransitionType RotationTransition { get; set; } = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType RotationEase { get; set; } = Tween.EaseType.Out;

	// Defaults are mapped to both WASD + arrows in this project via Input Map.
	[Export] public string UpAction { get; set; } = "move_forward";
	[Export] public string RightAction { get; set; } = "move_right";
	[Export] public string DownAction { get; set; } = "move_back";
	[Export] public string LeftAction { get; set; } = "move_left";

	// Direction indices: 0=Up, 1=Right, 2=Down, 3=Left
	private const int DirectionCount = 4;

	private readonly RandomNumberGenerator _rng = new();
	private float _progress;
	private int _currentIndex;
	private int _rotationSteps;
	private Tween _rotateTween;
	private ulong _lastProcessedFrame = ulong.MaxValue;

	public override void _Ready()
	{
		_rng.Randomize();
		SetProcess(false);
		SetPhysicsProcess(false);
		Visible = false;

		if (FillBar == null)
		{
			GD.PrintErr("MinigameRotator: FillBar is not assigned!");
		}

		if (RotatingVisual == null)
		{
			GD.PrintErr("MinigameRotator: RotatingVisual is not assigned!");
		}
	}

	public override void StartMinigame()
	{
		base.StartMinigame();

		_progress = 0.0f;
		_currentIndex = 0;
		_rotationSteps = 0;
		Visible = true;

		if (FillBar != null)
		{
			FillBar.Visible = true;
			FillBar.MaxValue = TargetValue;
			FillBar.Value = 0.0f;
		}

		UpdateVisualRotation(instant: true);
	}

	public override void StopMinigame()
	{
		Visible = false;
		base.StopMinigame();
		QueueFree();
	}

	public override void ResetMinigame()
	{
		_progress = 0.0f;
		_currentIndex = 0;
		_rotationSteps = 0;
		UpdateFillBar();
		UpdateVisualRotation(instant: true);
	}

	public override void HandleInput()
	{
		if (!Visible)
		{
			return;
		}

		// Prevent double-processing the same input in one frame.
		ulong frame = Engine.GetProcessFrames();
		if (_lastProcessedFrame == frame)
		{
			return;
		}
		_lastProcessedFrame = frame;

		int pressedIndex = GetPressedIndex();
		if (pressedIndex == _currentIndex)
		{
			OnCorrectPress();
		}
	}

	private int GetPressedIndex()
	{
		if (Input.IsActionJustPressed(UpAction)) return 0;
		if (Input.IsActionJustPressed(RightAction)) return 1;
		if (Input.IsActionJustPressed(DownAction)) return 2;
		if (Input.IsActionJustPressed(LeftAction)) return 3;
		return -1;
	}

	private void OnCorrectPress()
	{
		// Move to next index
		_currentIndex = (_currentIndex + 1) % DirectionCount;
		_rotationSteps++;

		// Update visual to show new expected direction
		UpdateVisualRotation(instant: false);

		// Add progress
		float inc = _rng.RandfRange(MinProgressPerPress, MaxProgressPerPress);
		_progress = Mathf.Min(_progress + inc, TargetValue);
		UpdateFillBar();

		if (_progress >= TargetValue)
		{
			StopMinigame();
		}
	}

	private void UpdateFillBar()
	{
		if (FillBar == null)
		{
			return;
		}

		FillBar.MaxValue = TargetValue;
		FillBar.Value = _progress;
	}

	private void UpdateVisualRotation(bool instant)
	{
		if (RotatingVisual == null)
		{
			return;
		}

		// Use rotation steps so it always rotates forward (never backwards from 270° to 0°)
		// 90 degrees per step
		float targetRotationDegrees = _rotationSteps * 90.0f;

		if (instant)
		{
			_rotateTween?.Kill();
			RotatingVisual.RotationDegrees = targetRotationDegrees;
		}
		else
		{
			AnimateRotationTo(targetRotationDegrees);
		}
	}

	private void AnimateRotationTo(float targetRotationDegrees)
	{
		if (RotatingVisual == null)
		{
			return;
		}

		_rotateTween?.Kill();
		_rotateTween = CreateTween();
		_rotateTween.SetTrans(RotationTransition);
		_rotateTween.SetEase(RotationEase);
		_rotateTween.TweenProperty(RotatingVisual, "rotation_degrees", targetRotationDegrees, RotationDurationSeconds);
	}
}

using Godot;

public partial class MinigameRotator : MinigameBase
{
	[Export] public ProgressBar FillBar { get; set; }
	[Export] public Control RotatingVisual { get; set; }

	[Export] public float TargetValue { get; set; } = 1.0f;
	[Export] public float MinProgressPerPress { get; set; } = 0.01f;
	[Export] public float MaxProgressPerPress { get; set; } = 0.05f;

	[Export] public float RotationDurationSeconds { get; set; } = 0.12f;
	[Export] public Tween.TransitionType RotationTransition { get; set; } = Tween.TransitionType.Sine;
	[Export] public Tween.EaseType RotationEase { get; set; } = Tween.EaseType.Out;
	[Export] public bool AutoCenterPivot { get; set; } = true;
	[Export] public bool ResetRotationOnFail { get; set; } = true;

	// Defaults are mapped to both WASD + arrows in this project via Input Map.
	[Export] public string UpAction { get; set; } = "move_forward";
	[Export] public string RightAction { get; set; } = "move_right";
	[Export] public string DownAction { get; set; } = "move_back";
	[Export] public string LeftAction { get; set; } = "move_left";

	private enum Direction
	{
		Up = 0,
		Right = 1,
		Down = 2,
		Left = 3,
	}

	private readonly RandomNumberGenerator _rng = new();
	private float _progress;
	private int _expectedIndex;

	private float _baseRotationRadians;
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

		if (AutoCenterPivot && RotatingVisual != null)
		{
			CallDeferred(nameof(SetupPivot));
		}
	}

	public override void StartMinigame()
	{
		base.StartMinigame();

		_progress = 0.0f;
		_expectedIndex = 0;
		_rotationSteps = 0;
		Visible = true;

		if (FillBar != null)
		{
			FillBar.Visible = true;
			FillBar.MaxValue = TargetValue;
			FillBar.Value = 0.0f;
		}

		if (RotatingVisual != null)
		{
			_baseRotationRadians = RotatingVisual.Rotation;

			if (ResetRotationOnFail)
			{
				RotatingVisual.Rotation = _baseRotationRadians;
			}
		}
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
		_expectedIndex = 0;
		_rotationSteps = 0;
		UpdateFillBar();
	}

	public override void HandleInput()
	{
		if (!Visible)
		{
			return;
		}

		// Player calls OverrideInput() from both _Process and _PhysicsProcess paths.
		// This prevents double-processing the same "just pressed" input in one frame.
		ulong frame = Engine.GetProcessFrames();
		if (_lastProcessedFrame == frame)
		{
			return;
		}
		_lastProcessedFrame = frame;

		if (!TryGetPressedDirection(out Direction pressed))
		{
			return;
		}

		Direction expected = (Direction)_expectedIndex;
		if (pressed == expected)
		{
			OnCorrectPress();
		}
		else
		{
			//OnFail();
		}
	}

	private bool TryGetPressedDirection(out Direction pressed)
	{
		pressed = Direction.Up;
		int pressedCount = 0;

		if (Input.IsActionJustPressed(UpAction))
		{
			pressed = Direction.Up;
			pressedCount++;
		}

		if (Input.IsActionJustPressed(RightAction))
		{
			if (pressedCount == 0)
			{
				pressed = Direction.Right;
			}
			pressedCount++;
		}

		if (Input.IsActionJustPressed(DownAction))
		{
			if (pressedCount == 0)
			{
				pressed = Direction.Down;
			}
			pressedCount++;
		}

		if (Input.IsActionJustPressed(LeftAction))
		{
			if (pressedCount == 0)
			{
				pressed = Direction.Left;
			}
			pressedCount++;
		}

		if (pressedCount == 0)
		{
			return false;
		}

		if (pressedCount > 1)
		{
			//OnFail();
			return false;
		}

		return true;
	}

	private void OnCorrectPress()
	{
		_expectedIndex = (_expectedIndex + 1) % 4;
		_rotationSteps++;

		AnimateRotationTo(_baseRotationRadians + _rotationSteps * (Mathf.Pi / 2.0f));

		float inc = _rng.RandfRange(MinProgressPerPress, MaxProgressPerPress);
		_progress = Mathf.Min(_progress + inc, TargetValue);
		UpdateFillBar();

		if (_progress >= TargetValue)
		{
			StopMinigame();
		}
	}

	private void OnFail()
	{
		_expectedIndex = 0;
		_progress = 0.0f;
		UpdateFillBar();

		if (ResetRotationOnFail)
		{
			_rotationSteps = 0;
			AnimateRotationTo(_baseRotationRadians);
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

	private void AnimateRotationTo(float targetRotationRadians)
	{
		if (RotatingVisual == null)
		{
			return;
		}

		_rotateTween?.Kill();
		_rotateTween = CreateTween();
		_rotateTween.SetTrans(RotationTransition);
		_rotateTween.SetEase(RotationEase);
		_rotateTween.TweenProperty(RotatingVisual, "rotation", targetRotationRadians, RotationDurationSeconds);
	}

	private void SetupPivot()
	{
		if (RotatingVisual == null)
		{
			return;
		}

		RotatingVisual.PivotOffset = RotatingVisual.Size / 2.0f;
	}
}


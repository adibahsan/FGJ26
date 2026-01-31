using Godot;

public partial class Player : RigidBody3D
{
	[Export] public float MoveSpeed { get; set; } = 10.0f;
	[Export] public bool UseAcceleration { get; set; } = true;
	[Export] public float Acceleration { get; set; } = 50.0f;
	[Export] public float Deceleration { get; set; } = 30.0f;
	[Export] public float RotationSpeed { get; set; } = 10.0f;
	[Export] public Node3D MaskCarryPosition { get; set; }

	public Mask CarriedMask { get; private set; }
	public bool IsCarryingMask => CarriedMask != null;

	private Mask _nearbyMask;
	private BedArea _nearbyBedArea;
	private GameManager _gameManager;

	public override void _Ready()
	{
		_gameManager = GameManager.Instance;

		if (MaskCarryPosition == null)
		{
			GD.PrintErr("Player: MaskCarryPosition is not assigned! Please assign it in the editor.");
		}
	}

	public void PickupMask(Mask mask)
	{
		// If already carrying a mask, return it to spawn first
		if (IsCarryingMask)
		{
			GD.Print($"Returning {CarriedMask.Type} mask to spawn");
			CarriedMask.ReturnToSpawn();
			CarriedMask = null;
		}

		CarriedMask = mask;
		mask.OnPickedUp(this);
		GD.Print($"Picked up {mask.Type} mask");
	}

	public void DropMask()
	{
		if (!IsCarryingMask)
		{
			return;
		}

		GD.Print($"Dropped {CarriedMask.Type} mask");
		CarriedMask = null;
	}

	public void SetNearbyMask(Mask mask)
	{
		_nearbyMask = mask;
	}

	public void ClearNearbyMask(Mask mask)
	{
		if (_nearbyMask == mask)
		{
			_nearbyMask = null;
		}
	}

	public void SetNearbyBedArea(BedArea bedArea)
	{
		_nearbyBedArea = bedArea;
	}

	public void ClearNearbyBedArea(BedArea bedArea)
	{
		if (_nearbyBedArea == bedArea)
		{
			_nearbyBedArea = null;
		}
	}

	public override void _Process(double delta)
	{
		if (_gameManager.OverrideInput())
		{
			return;
		}
		
		if (Input.IsActionJustPressed("interact"))
		{
			// Bed interaction takes priority when carrying a mask
			if (_nearbyBedArea != null && IsCarryingMask)
			{
				// Store pending mask - will be applied after minigame completes
				// Mask stays on player until minigame finishes
				_gameManager.SetPendingMask(CarriedMask, _nearbyBedArea);
				_gameManager.StartNextMinigame();
			}
			else if (_nearbyMask != null)
			{
				PickupMask(_nearbyMask);
				_nearbyMask = null;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 inputDir = GetInputDirection();
		HandleMovement(inputDir, (float)delta);
		HandleRotation(inputDir, (float)delta);
	}

	private void HandleMovement(Vector3 inputDir, float delta)
	{
		Vector3 targetVelocity = inputDir * MoveSpeed;
		Vector3 currentVelocity = LinearVelocity;
		Vector3 newHorizontalVelocity;

		if (UseAcceleration)
		{
			Vector3 horizontalVelocity = new Vector3(currentVelocity.X, 0, currentVelocity.Z);
			float accel = inputDir.LengthSquared() > 0 ? Acceleration : Deceleration;
			newHorizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, accel * delta);
		}
		else
		{
			newHorizontalVelocity = targetVelocity;
		}

		LinearVelocity = new Vector3(newHorizontalVelocity.X, currentVelocity.Y, newHorizontalVelocity.Z);
	}

	private void HandleRotation(Vector3 inputDir, float delta)
	{
		if (inputDir.LengthSquared() == 0)
		{
			return;
		}

		float targetAngle = Mathf.Atan2(-inputDir.X, -inputDir.Z);
		float currentAngle = Rotation.Y;
		float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, RotationSpeed * delta);

		Rotation = new Vector3(Rotation.X, newAngle, Rotation.Z);
	}

	private Vector3 GetInputDirection()
	{
		if (_gameManager.OverrideInput())
		{
			return Vector3.Zero;
		}

		Vector3 direction = Vector3.Zero;
		
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1;
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction.Z += 1;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1;
		}
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1;
		}

		return direction.Normalized();
	}
}

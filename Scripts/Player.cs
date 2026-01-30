using Godot;

public partial class Player : RigidBody3D
{
	/// <summary>
	/// Movement speed of the player.
	/// </summary>
	[Export] public float MoveSpeed { get; set; } = 10.0f;

	/// <summary>
	/// How quickly the player reaches target velocity.
	/// </summary>
	[Export] public float Acceleration { get; set; } = 50.0f;

	/// <summary>
	/// How quickly the player slows down when not moving.
	/// </summary>
	[Export] public float Deceleration { get; set; } = 30.0f;

	public override void _Ready()
	{
		// Lock rotation so player doesn't tip over
		AxisLockAngularX = true;
		AxisLockAngularY = true;
		AxisLockAngularZ = true;

		// Lock vertical movement (no jumping/falling for now)
		AxisLockLinearY = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		HandleMovement((float)delta);
	}

	private void HandleMovement(float delta)
	{
		Vector3 inputDir = GetInputDirection();
		Vector3 targetVelocity = inputDir * MoveSpeed;

		// Only affect horizontal velocity (X and Z)
		Vector3 currentVelocity = LinearVelocity;
		Vector3 horizontalVelocity = new Vector3(currentVelocity.X, 0, currentVelocity.Z);

		float accel = inputDir.LengthSquared() > 0 ? Acceleration : Deceleration;
		Vector3 newHorizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, accel * delta);

		// Apply the new velocity, preserving any vertical velocity
		LinearVelocity = new Vector3(newHorizontalVelocity.X, currentVelocity.Y, newHorizontalVelocity.Z);
	}

	private Vector3 GetInputDirection()
	{
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

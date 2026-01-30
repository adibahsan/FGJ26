using Godot;

public partial class Player : RigidBody3D
{
	/// <summary>
	/// Movement speed of the player.
	/// </summary>
	[Export] public float MoveSpeed { get; set; } = 10.0f;

	/// <summary>
	/// If true, use acceleration/deceleration. If false, movement is instant.
	/// </summary>
	[Export] public bool UseAcceleration { get; set; } = true;

	/// <summary>
	/// How quickly the player reaches target velocity. Only used when UseAcceleration is true.
	/// </summary>
	[Export] public float Acceleration { get; set; } = 50.0f;

	/// <summary>
	/// How quickly the player slows down when not moving. Only used when UseAcceleration is true.
	/// </summary>
	[Export] public float Deceleration { get; set; } = 30.0f;

	public override void _PhysicsProcess(double delta)
	{
		HandleMovement((float)delta);
	}

	private void HandleMovement(float delta)
	{
		Vector3 inputDir = GetInputDirection();
		Vector3 targetVelocity = inputDir * MoveSpeed;

		Vector3 currentVelocity = LinearVelocity;
		Vector3 newHorizontalVelocity;

		if (UseAcceleration)
		{
			// Movement with acceleration/deceleration
			Vector3 horizontalVelocity = new Vector3(currentVelocity.X, 0, currentVelocity.Z);
			float accel = inputDir.LengthSquared() > 0 ? Acceleration : Deceleration;
			newHorizontalVelocity = horizontalVelocity.MoveToward(targetVelocity, accel * delta);
		}
		else
		{
			// Instant movement
			newHorizontalVelocity = targetVelocity;
		}

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

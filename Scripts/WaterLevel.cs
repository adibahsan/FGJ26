using Godot;

public partial class WaterLevel : Node3D
{
	[Export] public MaskType EventType { get; set; } = MaskType.Snorkel;
	[Export] public float LowY { get; set; } = -1f;
	[Export] public float HighY { get; set; } = 1f;
	[Export] public float MoveSpeed { get; set; } = 2f;

	private float _targetY;

	public override void _Ready()
	{
		_targetY = LowY;
		Position = new Vector3(Position.X, LowY, Position.Z);
	}

	public override void _Process(double delta)
	{
		if (GameManager.Instance == null)
		{
			return;
		}

		_targetY = GameManager.Instance.CurrentEvent == EventType ? HighY : LowY;

		float newY = Mathf.MoveToward(Position.Y, _targetY, MoveSpeed * (float)delta);
		Position = new Vector3(Position.X, newY, Position.Z);
	}
}

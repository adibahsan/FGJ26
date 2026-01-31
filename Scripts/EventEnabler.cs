using Godot;

public partial class EventEnabler : Node3D
{
	[Export] public MaskType EventType { get; set; } = MaskType.Gas;
	[Export] public Node3D VisualEffect { get; set; }

	public override void _Ready()
	{
		if (VisualEffect == null)
		{
			GD.PrintErr("EventEnabler: VisualEffect is not assigned!");
			return;
		}

		VisualEffect.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (VisualEffect == null || GameManager.Instance == null)
		{
			return;
		}

		VisualEffect.Visible = GameManager.Instance.CurrentEvent == EventType;
	}
}

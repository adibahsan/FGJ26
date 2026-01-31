using Godot;

public partial class Lamp : Node3D
{
	[Export] public Light3D Light { get; set; }

	public override void _Ready()
	{
		if (Light == null)
		{
			GD.PrintErr("Lamp: Light is not assigned!");
			return;
		}

		Light.Visible = false;
	}

	public override void _Process(double delta)
	{
		if (Light == null || GameManager.Instance == null)
		{
			return;
		}

		Light.Visible = GameManager.Instance.CurrentEvent == MaskType.Sleep;
	}
}

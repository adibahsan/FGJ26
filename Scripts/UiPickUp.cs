using Godot;

public partial class UiPickUp : Control
{
	[Export] public Label MaskNameLabel { get; set; }

	public override void _Ready()
	{
		if (MaskNameLabel == null)
		{
			GD.PrintErr("UiPickUp: MaskNameLabel is not assigned!");
		}
	}

	public override void _Process(double delta)
	{
		GameManager gm = GameManager.Instance;
		if (gm == null || MaskNameLabel == null)
		{
			return;
		}

		MaskNameLabel.Text = GetMaskName(gm.CurrentPickupMask);
	}

	private static string GetMaskName(MaskType maskType)
	{
		return maskType switch
		{
			MaskType.None => "",
			MaskType.Cooling => "Cooling Mask",
			MaskType.Sleep => "Sleep Mask",
			MaskType.Scary => "Scary Mask",
			MaskType.FakeEyeGlasses => "Fake Eye Glasses",
			MaskType.Snorkel => "Snorkel Mask",
			MaskType.Gas => "Gas Mask",
			MaskType.Cpap => "CPAP Mask",
			MaskType.Welding => "Welding Mask",
			_ => "Unknown Mask"
		};
	}
}

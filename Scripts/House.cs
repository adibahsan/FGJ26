using Godot;

public partial class House : Node3D
{
	[Export] public Marker3D Spawnpoint { get; set; }
	[Export] public Bed Bed { get; set; }

	public override void _Ready()
	{
		if (Spawnpoint == null)
		{
			GD.PrintErr("House: Spawnpoint is not assigned! Please assign it in the editor.");
		}
		if (Bed == null)
		{
			GD.PrintErr("House: Bed is not assigned! Please assign it in the editor.");
		}
	}
}

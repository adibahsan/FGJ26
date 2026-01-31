using Godot;

public partial class Bed : StaticBody3D
{
	[Export] public BedArea Area { get; set; }

	public override void _Ready()
	{
		if (Area == null)
		{
			GD.PrintErr("Bed: Area is not assigned! Please assign it in the editor.");
		}
	}
}

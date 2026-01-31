using Godot;

public partial class WallDisable : Area3D
{
	[Export] public Node3D[] NodesToToggle { get; set; }

	public override void _Ready()
	{
		var TriggerArea = this as Area3D;
		
		if (TriggerArea == null)
		{
			GD.PrintErr("WallDisable: Area3D child node not found!");
			return;
		}

		TriggerArea.BodyEntered += OnBodyEntered;
		TriggerArea.BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Player player)
		{
			// GD.Print("Player entered wall trigger area");

			if (NodesToToggle != null)
			{
				foreach (var node in NodesToToggle)
				{
					if (node != null)
					{
						node.Visible = false;

						if (node is CollisionObject3D collisionObject)
						{
							collisionObject.SetDeferred("disable_mode", (int)CollisionObject3D.DisableModeEnum.Remove);
						}
					}
				}
			}
		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body is Player player)
		{
			// GD.Print("Player exited wall trigger area");

			if (NodesToToggle != null)
			{
				foreach (var node in NodesToToggle)
				{
					if (node != null)
					{
						node.Visible = true;

						if (node is CollisionObject3D collisionObject)
						{
							collisionObject.SetDeferred("disable_mode", (int)CollisionObject3D.DisableModeEnum.MakeStatic);
						}
					}
				}
			}
		}
	}
}

using Godot;

public partial class Mask : Area3D
{
	[Export] public MaskType Type { get; set; } = MaskType.Sleep;

	/// <summary>
	/// Node3D that will be reparented to player when picked up.
	/// </summary>
	[Export] public Node3D MaskRoot { get; set; }

	private Player _playerInZone;

	public override void _Ready()
	{
		if (MaskRoot == null)
		{
			GD.PrintErr("Mask: MaskRoot is not assigned! Please assign it in the editor.");
		}

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Player player)
		{
			_playerInZone = player;
			player.SetNearbyMask(this);
			if (!player.IsCarryingMask)
			{
				GameManager.Instance?.ShowPickupUI(true);
			}
		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body is Player player && player == _playerInZone)
		{
			_playerInZone = null;
			player.ClearNearbyMask(this);
			GameManager.Instance?.ShowPickupUI(false);
		}
	}

	public void OnPickedUp(Player player)
	{
		GameManager.Instance?.ShowPickupUI(false);
		
		Node3D carryPosition = player.MaskCarryPosition;
		if (carryPosition != null && MaskRoot != null)
		{
			MaskRoot.GetParent()?.RemoveChild(MaskRoot);
			carryPosition.AddChild(MaskRoot);
			MaskRoot.Position = Vector3.Zero;
			MaskRoot.Rotation = Vector3.Zero;
		}
		else if (MaskRoot == null)
		{
			GD.PrintErr("Mask: MaskRoot is not set! Please assign it in the editor.");
		}
		
		SetDeferred("monitoring", false);
		SetDeferred("monitorable", false);
	}
}

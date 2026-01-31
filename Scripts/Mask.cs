using Godot;

public partial class Mask : Node3D
{
	[Export] public MaskType Type { get; set; } = MaskType.Sleep;
	[Export] public Area3D PickupArea { get; set; }

	private Player _playerInZone;

	public override void _Ready()
	{
		if (PickupArea == null)
		{
			GD.PrintErr("Mask: PickupArea is not assigned! Please assign it in the editor.");
			return;
		}

		PickupArea.BodyEntered += OnBodyEntered;
		PickupArea.BodyExited += OnBodyExited;
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
		if (carryPosition != null)
		{
			GetParent()?.RemoveChild(this);
			carryPosition.AddChild(this);
			Position = Vector3.Zero;
			Rotation = Vector3.Zero;
		}
		
		if (PickupArea != null)
		{
			PickupArea.SetDeferred("monitoring", false);
			PickupArea.SetDeferred("monitorable", false);
		}
	}
}

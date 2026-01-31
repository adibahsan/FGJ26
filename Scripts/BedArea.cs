using Godot;

public partial class BedArea : Area3D
{
	[Export] public Node3D MaskPosition { get; set; }

	public MaskType CurrentMask { get; private set; } = MaskType.None;

	private Player _playerInZone;

	public override void _Ready()
	{
		if (MaskPosition == null)
		{
			GD.PrintErr("BedArea: MaskPosition is not assigned! Please assign it in the editor.");
		}

		BodyEntered += OnBodyEntered;
		BodyExited += OnBodyExited;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Player player)
		{
			_playerInZone = player;
			player.SetNearbyBedArea(this);
			UpdateSetMaskUI();
		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body is Player player && player == _playerInZone)
		{
			_playerInZone = null;
			player.ClearNearbyBedArea(this);
			GameManager.Instance?.ShowSetMaskUI(false);
		}
	}

	public void UpdateSetMaskUI()
	{
		if (_playerInZone != null && _playerInZone.IsCarryingMask)
		{
			GameManager.Instance?.ShowSetMaskUI(true);
		}
		else
		{
			GameManager.Instance?.ShowSetMaskUI(false);
		}
	}

	public void SetMask(Mask mask)
	{
		if (MaskPosition == null)
		{
			GD.PrintErr("BedArea: MaskPosition is not set! Please assign it in the editor.");
			return;
		}

		Node3D maskRoot = mask.MaskRoot;
		if (maskRoot == null)
		{
			GD.PrintErr("BedArea: Mask has no MaskRoot set!");
			return;
		}

		maskRoot.GetParent()?.RemoveChild(maskRoot);
		MaskPosition.AddChild(maskRoot);
		maskRoot.Position = Vector3.Zero;
		maskRoot.Rotation = Vector3.Zero;

		CurrentMask = mask.Type;
		GameManager.Instance?.ShowSetMaskUI(false);
		GD.Print($"Set {mask.Type} mask on bed");
	}

	public void ClearMask()
	{
		CurrentMask = MaskType.None;
	}
}

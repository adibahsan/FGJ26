using Godot;
using System;
using System.Collections.Generic;

public partial class RadiatorRoot : Node3D
{
	private static RadiatorRoot _instance;
	public static RadiatorRoot Instance => _instance;

	[Export] public Material RadiatorMaterial { get; set; } = null;
	[Export] public string MaterialPropertyName { get; set; } = "emission_energy_multiplier";
	[Export] public float AnimationSpeed { get; set; } = 1.0f;
	[Export] public float MinValue { get; set; } = 0.5f;
	[Export] public float MaxValue { get; set; } = 2.0f;
	[Export] public Color redAlbedoColor { get; set; } = new Color(1, 0.4f, 0.4f, 1);
	[Export] public Color whiteAlbedoColor { get; set; } = new Color(1, 1, 1, 1);

	private float _time = 0.0f;
	private bool _hasSetAlbedo = false;
	private bool animate = false;

	public override void _EnterTree()
	{
		if (_instance != null && _instance != this)
		{
			GD.PrintErr("RadiatorRoot: Multiple instances detected! Removing duplicate.");
			QueueFree();
			return;
		}
		_instance = this;
	}

	public override void _ExitTree()
	{
		if (_instance == this)
		{
			_instance = null;
		}
	}

	public void EnableAnimation(bool value)
	{
		animate = value;
	}
	
	public override void _Process(double delta)
	{
		if (RadiatorMaterial == null)
			return;
		if (animate == false)
		{
			SetInitialAlbedo(whiteAlbedoColor);
			
			if (RadiatorMaterial is ShaderMaterial shaderMaterial2)
			{
				shaderMaterial2.SetShaderParameter(MaterialPropertyName, 0);
			}
			else if (RadiatorMaterial is StandardMaterial3D standardMaterial2)
			{
				standardMaterial2.EmissionEnergyMultiplier = 0;
			}
			
			return;
		}

	SetInitialAlbedo(redAlbedoColor);

	_time += (float)delta * AnimationSpeed;
	float value = Mathf.Lerp(MinValue, MaxValue, (Mathf.Sin(_time) + 1.0f) / 2.0f);

	if (RadiatorMaterial is ShaderMaterial shaderMaterial)
	{
		shaderMaterial.SetShaderParameter(MaterialPropertyName, value);
	}
	else if (RadiatorMaterial is StandardMaterial3D standardMaterial)
	{
		standardMaterial.EmissionEnergyMultiplier = value;
	}
}

private void SetInitialAlbedo(Color color)
{
	if (_hasSetAlbedo)
		return;

	if (RadiatorMaterial is ShaderMaterial shaderMaterial)
	{
		shaderMaterial.SetShaderParameter("albedo", color);
	}
	else if (RadiatorMaterial is StandardMaterial3D standardMaterial)
	{
		standardMaterial.AlbedoColor = color;
	}

	_hasSetAlbedo = true;
	}

	/// <summary>
	/// Gets all direct children of this Node3D.
	/// </summary>
	public List<Node> GetAllChildren()
	{
		List<Node> children = new List<Node>();
		int childCount = GetChildCount();

		for (int i = 0; i < childCount; i++)
		{
			children.Add(GetChild(i));
		}

		return children;
	}

	/// <summary>
	/// Gets all children recursively, including nested children.
	/// </summary>
	public List<Node> GetAllChildrenRecursive()
	{
		List<Node> allChildren = new List<Node>();
		GetChildrenRecursiveHelper(this, allChildren);
		return allChildren;
	}

	private void GetChildrenRecursiveHelper(Node parent, List<Node> result)
	{
		int childCount = parent.GetChildCount();

		for (int i = 0; i < childCount; i++)
		{
			Node child = parent.GetChild(i);
			result.Add(child);
			GetChildrenRecursiveHelper(child, result);
		}
	}
}

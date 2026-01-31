using Godot;
using System;

public partial class MinigameBase : Control
{
	/// <summary>
	/// Called when the minigame should become active (show UI, start timers, enable input, etc).
	/// </summary>
	public virtual void StartMinigame()
	{
		SetProcess(true);
		SetPhysicsProcess(true);
	}

	/// <summary>
	/// Called when the minigame should stop (hide UI, stop timers, disable input, etc).
	/// </summary>
	public virtual void StopMinigame()
	{
		GameManager.Instance.ClearActiveMinigame();
	}

	/// <summary>
	/// Optional: reset internal state so the minigame can be replayed.
	/// </summary>
	public virtual void ResetMinigame()
	{
		// Intentionally empty: override in child classes.
	}

	/// <summary>
	/// Optional: report whether the minigame is currently allowed to start.
	/// </summary>
	public virtual bool CanStart()
	{
		return true;
	}

	/// <summary>
	/// Optional: called when the minigame is considered "completed".
	/// You can override to trigger rewards, notify GameManager, etc.
	/// </summary>
	public virtual void CompleteMinigame()
	{
		GameManager.Instance.ClearActiveMinigame();
	}

	public virtual void HandleInput()
	{
		
	}
}

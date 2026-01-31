using Godot;
using System;
using System.Collections.Generic;

public partial class EventSystem : Node
{
	[Export] public float MinEventDuration { get; set; } = 10f;
	[Export] public float MaxEventDuration { get; set; } = 30f;
	[Export] public float InitialDelay { get; set; } = 5f;

	private readonly MaskType[] _availableEvents = 
	{
		MaskType.Cooling,        // Heater malfunction
		MaskType.Sleep,          // Lights go on
		MaskType.Scary,          // Vacuum cleaner merchant
		MaskType.FakeEyeGlasses, // Teams call from boss
		MaskType.Snorkel,        // Water leak
		MaskType.Gas,            // Gas leak
		MaskType.Cpap,           // CPAP failure
		MaskType.Welding         // Electric malfunction
	};

	private Queue<MaskType> _eventQueue;
	private float _eventTimer;
	private float _currentEventDuration;
	private bool _eventActive;
	private bool _gameEnded;
	private MaskType _currentEventType = MaskType.None;
	private Random _random;

	public override void _Ready()
	{
		_random = new Random();
		_gameEnded = false;
		_eventActive = false;
		_eventTimer = InitialDelay;
		
		CreateShuffledEventQueue();
		GD.Print($"EventSystem: Created queue with {_eventQueue.Count} events");
	}

	private void CreateShuffledEventQueue()
	{
		var shuffled = ShuffleHelper.ToShuffledList(_availableEvents);
		_eventQueue = new Queue<MaskType>(shuffled);
	}

	public override void _Process(double delta)
	{
		if (_gameEnded)
		{
			return;
		}

		_eventTimer -= (float)delta;

		if (_eventTimer <= 0)
		{
			if (_eventActive)
			{
				EndCurrentEvent();
			}

			StartNextEvent();
		}
	}

	private void StartNextEvent()
	{
		if (_eventQueue.Count == 0)
		{
			OnAllEventsCompleted();
			return;
		}

		_currentEventType = _eventQueue.Dequeue();
		_currentEventDuration = GetRandomDuration();
		_eventTimer = _currentEventDuration;
		_eventActive = true;
		
		if(_currentEventType == MaskType.Cooling)
		{
			RadiatorRoot.Instance.EnableAnimation(true);
		}

		GameManager.Instance?.SetEvent(_currentEventType);
		GD.Print($"EventSystem: Started {_currentEventType} event (duration: {_currentEventDuration:F1}s, {_eventQueue.Count} remaining)");
	}

	private void EndCurrentEvent()
	{
		GD.Print($"EventSystem: Ended {_currentEventType} event");
		
		if(_currentEventType == MaskType.Cooling)
		{
			RadiatorRoot.Instance.EnableAnimation(false);
		}
		
		GameManager.Instance?.ClearEvent();
		_currentEventType = MaskType.None;
		_eventActive = false;
	}

	private void OnAllEventsCompleted()
	{
		// Events are not a win condition. When we run out, reshuffle and keep going
		// until the GameManager ends the game (night ends or sleep hits zero).
		GD.Print("EventSystem: All events completed - reshuffling queue");
		CreateShuffledEventQueue();
		_eventActive = false;
		_currentEventType = MaskType.None;
		_eventTimer = 0f;
	}

	private float GetRandomDuration()
	{
		return (float)(_random.NextDouble() * (MaxEventDuration - MinEventDuration) + MinEventDuration);
	}
}

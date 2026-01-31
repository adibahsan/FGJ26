using Godot;
using System;

public partial class MinigameTyper : MinigameBase
{
	[Export] public Label WordLabel { get; set; }
	[Export] public Label TypedLabel { get; set; }

	private Random _random;
	private string _currentWord;
	private int _currentIndex;

	public override void _Ready()
	{
		_random = new Random();
		SetProcess(false);

		if (WordLabel == null)
		{
			GD.PrintErr("MinigameTyper: WordLabel is not assigned!");
		}

		if (TypedLabel == null)
		{
			GD.PrintErr("MinigameTyper: TypedLabel is not assigned!");
		}
	}

	public override void StartMinigame()
	{
		base.StartMinigame();
		
		_currentWord = SleepWords.Words[_random.Next(SleepWords.Words.Length)];
		_currentIndex = 0;

		Visible = true;
		WordLabel.Text = _currentWord;
		UpdateTypedLabel();

		GD.Print($"MinigameTyper: Type '{_currentWord}'");
	}

	public override void StopMinigame()
	{
		Visible = false;
		base.StopMinigame();
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			HandleKeyInput(keyEvent);
		}
	}

	private void HandleKeyInput(InputEventKey keyEvent)
	{
		if (string.IsNullOrEmpty(_currentWord) || _currentIndex >= _currentWord.Length)
		{
			return;
		}

		char expectedChar = _currentWord[_currentIndex];
		char typedChar = (char)keyEvent.Unicode;

		if (char.ToLower(typedChar) == expectedChar)
		{
			_currentIndex++;
			UpdateTypedLabel();

			if (_currentIndex >= _currentWord.Length)
			{
				OnWordCompleted();
			}
		}
		else if (typedChar != '\0')
		{
			OnWrongKey();
		}
	}

	private void OnWordCompleted()
	{
		GD.Print("MinigameTyper: Word completed!");
		StopMinigame();
	}

	private void OnWrongKey()
	{
		_currentIndex = 0;
		UpdateTypedLabel();
		GD.Print("MinigameTyper: Wrong key, reset!");
	}

	private void UpdateTypedLabel()
	{
		string typed = _currentWord.Substring(0, _currentIndex);
		string spaces = new string(' ', _currentWord.Length - _currentIndex);
		TypedLabel.Text = typed + spaces;
	}

	public override void ResetMinigame()
	{
		_currentWord = null;
		_currentIndex = 0;
	}
}

using Godot;

public partial class UiScore : Control
{
	[Export] public Label ScoreLabel { get; set; }

	private int _displayedScore = -1;

	public override void _Ready()
	{
		if (ScoreLabel == null)
		{
			GD.PrintErr("UiScore: ScoreLabel is not assigned! Please assign it in the editor.");
		}

		UpdateScore(0);
	}

	public void UpdateScore(float score)
	{
		int intScore = (int)score;
		if (intScore == _displayedScore)
		{
			return;
		}

		_displayedScore = intScore;
		ScoreLabel.Text = intScore.ToString();
	}
}

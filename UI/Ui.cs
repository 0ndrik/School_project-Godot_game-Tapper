using Godot;

public partial class Ui : CanvasLayer
{
	[Export] private Label ScoreLabel { get; set; }
	
	[Export] public Godot.Collections.Array<TextureRect> LivesTextures { get; set; }
	
	private int _numOfLives = Global.NumOfLives;

	public override void _Process(double delta)
	{
		ScoreLabel.Text = Global.Score.ToString();
		
		if (_numOfLives != Global.NumOfLives)
		{
			_numOfLives = Global.NumOfLives;
			UpdateLives(_numOfLives);
		}
	}
	
	public void UpdateLives(int currentLives)
	{
		for (int i = 0; i < LivesTextures.Count; i++)
		{
			LivesTextures[i].Visible = i < currentLives;
		}
		if (_numOfLives == 0)
		{
			GetTree().ChangeSceneToFile("res://UI/end_screen.tscn");	
		}
	}
}

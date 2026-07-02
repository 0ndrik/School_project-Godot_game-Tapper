using Godot;

public partial class EndScreen : Control
{
	private Label _score;
	private Label _bestScore;
	private Label _record;
	
	public override void _Ready()
	{
		_score = GetNode<Label>("Score");
		_bestScore = GetNode<Label>("BestScore");
		_record = GetNode<Label>("NewRecord");
		
		if (Global.Score > Global.BestScore)
		{
			Global.BestScore = Global.Score;
			_record.Visible = true;
			SaveSystem.SaveInt(Global.BestScore);
		}
		else
		{
			_record.Visible = false;
		}
		_score.Text = $"Score: {Global.Score}";
		_bestScore.Text = $"Best Score: {Global.BestScore}";
	}

	public void _on_again_pressed()
	{
		GetTree().ChangeSceneToFile("res://Levels/level.tscn");
	}

	public void _on_exit_pressed()
	{
		GetTree().ChangeSceneToFile("res://UI/menu.tscn");	
	}
}

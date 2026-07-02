using Godot;

public partial class Level : Node2D
{
	public override void _Ready()
	{
		Global.NumOfLives = 3;
		Global.Score = 0;
	}

	
}

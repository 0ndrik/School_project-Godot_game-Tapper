using Godot;
using System;

public partial class Menu : Control
{

	public void _on_Play_pressed()
	{
		GetTree().ChangeSceneToFile("res://Levels/level.tscn");
	}

	public void _on_Exit_pressed()
	{
		GetTree().Quit();
	}
}

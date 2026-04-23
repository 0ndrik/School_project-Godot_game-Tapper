using Godot;
using System;

public partial class Ui : CanvasLayer
{
	[Export] private Label ScoreLabel { get; set; }
	[Export] public CharacterBody2D player;
	public override void _Process(double delta)
	{
		
	}
}

using Godot;
using System;

public partial class Tap : Node2D
{

	public Sprite2D Pipe;
	
	public override void _Ready()
	{
		Pipe = GetNode<Sprite2D>("Pipe");
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if (body is Player player)
		{
			player.CurrentTap = this;
		}
	}

	private void _on_area_2d_body_exited(Node2D body)
	{
		if (body is Player player && player.CurrentTap == this)
		{
			player.CurrentTap = null;
		}
	}
}

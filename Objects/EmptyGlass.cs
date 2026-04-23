using Godot;
using System;

public partial class EmptyGlass : Area2D
{
	[Export] public float Speed { get; set; } = 100.0f;
	public Vector2 Direction { get; set; } = Vector2.Right;
	
	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}

	public void GetCaught()
	{
		QueueFree();
	}

	public void Break()
	{
		QueueFree();
	}

	public void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			QueueFree();
		}
	}
}

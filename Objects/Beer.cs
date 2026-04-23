using Godot;
using System;

public partial class Beer : Area2D
{
	[Export] public float Speed { get; set; } = 300.0f;
	public Vector2 Direction { get; set; } = Vector2.Left;
	
	
	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}

	public void GetCaught()
	{
		QueueFree();
	}

	public void End()
	{
		QueueFree();
	}
}

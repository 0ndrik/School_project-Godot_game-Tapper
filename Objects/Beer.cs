using Godot;
using System;

public partial class Beer : Area2D
{
	[Export] public float Speed { get; set; } = 300.0f;
	public Vector2 Direction { get; set; } = Vector2.Left;

	public override void _Ready()
	{
		ZIndex = 3;
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}

	private bool _isCaught = false;
	public bool GetCaught()
	{
		if (_isCaught)
			return false;

		_isCaught = true;
		QueueFree();
		return true;
	}

	public void End()
	{
		QueueFree();
		Global.NumOfLives--;
	}
}

using Godot;

public partial class EmptyGlass : Area2D
{
	[Export] public float Speed { get; set; } = 100.0f;
	public Vector2 Direction { get; set; } = Vector2.Right;
	
	private AnimationPlayer _animationPlayer;

	private bool _broke = false;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		ZIndex = 3;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_broke)
		{
			Position += Direction * Speed * (float)delta;
		}
		
	}

	private void GetCaught()
	{
		Global.Score++;
		QueueFree();
	}

	public async void Break()
	{
		_broke = true;
		_animationPlayer.Play("break");
		await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);

		Global.NumOfLives--;
		
		QueueFree();
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			GetCaught();
		}
	}
}

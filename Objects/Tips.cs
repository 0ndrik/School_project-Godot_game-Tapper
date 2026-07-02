using Godot;

public partial class Tips : Area2D
{
	public override void _Ready()
	{
		ZIndex = 3;
	}

	public void _on_timer_timeout()
	{
		QueueFree();
	}

	private void _on_body_entered(Node2D body)
	{
		if (body is Player)
		{
			Global.Score += 3;
			QueueFree();
		}
	}

}

using Godot;
using System;

public partial class Customer : Area2D
{
	[Export] public float WalkSpeed { get; set; } = 30.0f; 
	[Export] public float SlideBackSpeed { get; set; } = 150.0f;
	[Export] public float SlideBackSec { get; set; } = 0.40f;
	[Export] public PackedScene EmptyGlassScene { get; set; }

	public Vector2 Direction { get; set; } = Vector2.Right;
	
	private AnimationPlayer _animationPlayer;
	private Marker2D _emptyGlassSpawnMarker;
	
	private enum CustomerState
	{
		Walking,
		SlidingBack,
		Drinking
	}

	private CustomerState _currentState = CustomerState.Walking;
	
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_emptyGlassSpawnMarker = GetNode<Marker2D>("Marker2D");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (_currentState == CustomerState.Walking)
		{
			_animationPlayer.Play("walk");
			Position += Direction * WalkSpeed * (float)delta;
		}
		else if (_currentState == CustomerState.SlidingBack)
		{
			_animationPlayer.Play("slide_back");
			Position -= Direction * SlideBackSpeed * (float)delta;
		}
		else if (_currentState == CustomerState.Drinking)
		{
			 _animationPlayer.Play("drink");
			 
		}
	}

	private void _on_area_entered(Area2D area)
	{
		if (area is Beer beer)
		{
			beer.GetCaught();
			ReactToBeer();
		}
	}

	private async void  ReactToBeer()
	{
		_currentState = CustomerState.SlidingBack;

		await ToSignal(GetTree().CreateTimer(SlideBackSec), SceneTreeTimer.SignalName.Timeout);

		_currentState = CustomerState.Drinking;

	}

	public void GotOut()
	{
		QueueFree();
	}
	
	private void ThrowEmptyGlass()
	{
		EmptyGlass newEmptyGlass = EmptyGlassScene.Instantiate<EmptyGlass>();
		newEmptyGlass.GlobalPosition = _emptyGlassSpawnMarker.GlobalPosition;
		GetTree().Root.AddChild(newEmptyGlass);
	}

	private void _on_animation_player_animation_finished(StringName animName)
	{
		if (animName == "drink")
		{
			_currentState = CustomerState.Walking;
		}
		
	}
}

using Godot;
using System;
using System.Collections.Generic;

public partial class Customer : Area2D
{
    [Export] public float WalkSpeed { get; set; } = 30.0f; 
    [Export] public float SlideBackSpeed { get; set; } = 150.0f;
    [Export] public float GoingDownSpeed { get; set; } = 20.0f;
    [Export] public float SlideBackSec { get; set; } = 0.6f;
    
    [Export] public PackedScene EmptyGlassScene { get; set; }
    [Export] public PackedScene TipScene { get; set; }

    public Vector2 Direction { get; set; } = Vector2.Right;
    
    private AnimationPlayer _animationPlayer;
    private Marker2D _emptyGlassSpawnMarker;
    
    // --- Spoločný zoznam pre striedanie zákazníkov ---
    private static List<int> _availableSkins = new List<int>();
    private static RandomNumberGenerator _rng = new RandomNumberGenerator();
    
    private int _mySkinIndex = 0;
    private bool _canGoOut = false;
    
    [Export] public float MaxTipsSec = 8.0f;
    private Timer _timer = new();
    
    private enum CustomerState
    {
        Walking,
        SlidingBack,
        Drinking,
        End,
        GoingDown
    }

    private CustomerState _currentState = CustomerState.Walking;
    
    public override void _Ready()
    {
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _emptyGlassSpawnMarker = GetNode<Marker2D>("Marker2D");
        
        SetupRandomSkin();

        _timer.WaitTime = MaxTipsSec;
        _timer.OneShot = true;
        AddChild(_timer);
        _timer.Start();
    }
    
    private void SetupRandomSkin()
    {
        // 1. Ak došli zákazníci, znova naplníme zoznam (0, 1, 2, 3, 4, 5)
        if (_availableSkins.Count == 0)
        {
            for (int i = 0; i < 6; i++)
            {
                _availableSkins.Add(i);
            }
        }

        // 2. Vytiahneme náhodného zákazníka
        int randomIndex = _rng.RandiRange(0, _availableSkins.Count - 1);
        _mySkinIndex = _availableSkins[randomIndex];
        
        // 3. Zmažeme ho zo zoznamu, aby ďalší dostal iného
        _availableSkins.RemoveAt(randomIndex); 
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (_currentState == CustomerState.Walking)
        {
            _animationPlayer.Play($"walk_{_mySkinIndex}");
            Position += Direction * WalkSpeed * (float)delta;
        }
        else if (_currentState == CustomerState.SlidingBack)
        {
            _animationPlayer.Play($"slide_back_{_mySkinIndex}");
            Position -= Direction * SlideBackSpeed * (float)delta;
        }
        else if (_currentState == CustomerState.Drinking)
        {
             _animationPlayer.Play($"drink_{_mySkinIndex}");
        }
        else if (_currentState == CustomerState.End)
        {
            _animationPlayer.Play($"shout_{_mySkinIndex}");
            
        }
        else if (_currentState == CustomerState.GoingDown)
        {
            Direction = Vector2.Down;
            Position += Direction * GoingDownSpeed * (float)delta;
        }
    }

    private void _on_area_entered(Area2D area)
    {
        if (area is Beer beer)
        {
            if (beer.GetCaught())
                ReactToBeer();
        }
    }

    private async void ReactToBeer()
    {
        _canGoOut = true;
        _currentState = CustomerState.SlidingBack;

        await ToSignal(GetTree().CreateTimer(SlideBackSec), SceneTreeTimer.SignalName.Timeout);

        _currentState = CustomerState.Drinking;
    }

    public void GotOut()
    {
        if (_canGoOut)
            QueueFree();
    }

    public void GotToTheStart()
    {
        _currentState = CustomerState.End;
    }

    public void GoDown()
    {
        _currentState = CustomerState.GoingDown;
        Global.NumOfLives--;
    }
    
    private void ThrowEmptyGlass()
    {
        EmptyGlass newEmptyGlass = EmptyGlassScene.Instantiate<EmptyGlass>();
        newEmptyGlass.GlobalPosition = _emptyGlassSpawnMarker.GlobalPosition;
        GetTree().CurrentScene.AddChild(newEmptyGlass);
    }

    private void LeaveTip()
    {
        Tips newTip = TipScene.Instantiate<Tips>();
        newTip.GlobalPosition = _emptyGlassSpawnMarker.GlobalPosition;
        GetTree().CurrentScene.AddChild(newTip);
    }

    private void _on_animation_player_animation_finished(StringName animName)
    {
        if (animName == $"drink_{_mySkinIndex}")
        {
            _currentState = CustomerState.Walking;
            
            if ((MaxTipsSec - _timer.TimeLeft) < MaxTipsSec)
            {
                if (GD.Randf() < 0.5) 
                {
                    LeaveTip();
                }
            }
        }
        else if (animName == $"shout_{_mySkinIndex}")
        {
            QueueFree(); 
            
        }
    }
}
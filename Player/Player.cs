using Godot;
using System;

public partial class Player : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 200.0f;
    [Export] public PackedScene BeerScene { get; set; }
    [Export] public Marker2D[] BarPositions { get; set; }
    
    public Tap CurrentTap { get; set; }
    private bool _isPouring = false, _isVerticallyMoving = false;
    private int _currentBarIndex = 0;
    [Signal] public delegate void MoveEventHandler();
    
    private Sprite2D _sprite;
    private AnimationPlayer _animationPlayer;
    private Marker2D _beerSpawnMarker;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _beerSpawnMarker = GetNode<Marker2D>("Marker2D");
        
        if (BarPositions != null && BarPositions.Length > 0)
        {
            GlobalPosition = BarPositions[_currentBarIndex].GlobalPosition;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isPouring || _isVerticallyMoving) return;

        if (Input.IsActionJustPressed("interact") && CurrentTap != null)
        {
            StartPouring();
            return;
        }
        
        HandleVerticalMovement();
        if (_isVerticallyMoving) return;
        
        HandleHorizontalMovement();
        
        MoveAndSlide();
    }

    private void HandleHorizontalMovement()
    {
         
        float direction = Input.GetAxis("ui_left", "ui_right");

        Vector2 vel = Velocity;
        vel.X = direction * Speed;
        Velocity = vel;

        if (CurrentTap != null)
        {
            _animationPlayer.Play("tap_start");
        }
        else if (direction != 0)
        {
            _animationPlayer.Play("run");
            _sprite.FlipH = (direction > 0);
        }
        else
        {
            _animationPlayer.Play("idle");
        }
    }
    
    private async void HandleVerticalMovement()
    {
        if (BarPositions == null || BarPositions.Length == 0) return;

        bool moved = false;
        
        if (Input.IsActionJustPressed("ui_up") && _currentBarIndex > 0)
        {
            _isVerticallyMoving = true;
            _animationPlayer.Play("move");
            await ToSignal(this, SignalName.Move);
            _currentBarIndex--;
            moved = true;
        }
 
        else if (Input.IsActionJustPressed("ui_down") && _currentBarIndex < BarPositions.Length - 1)
        {
            _isVerticallyMoving = true;
            _animationPlayer.Play("move");
            await ToSignal(this, SignalName.Move);
            _currentBarIndex++;
            moved = true;
        }

        // Ak hráč stlačil šípku a mohol sa pohnúť, aktualizujeme jeho Y pozíciu
        if (moved)
        {
            _isVerticallyMoving = false;
            GlobalPosition = BarPositions[_currentBarIndex].GlobalPosition;
            _sprite.FlipH = true;
            
            // Voliteľné: Zrušenie načapovaného piva pri zmene radu (aby sa odpojil od Tapu)
            CurrentTap = null; 
        }
    }

    private async void StartPouring()
    {
        _isPouring = true;
        CurrentTap.Pipe.Visible = false;
        _sprite.FlipH = true;
        _animationPlayer.Play("tap");
        
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        
        CurrentTap.Pipe.Visible = true;
        _animationPlayer.Play("throw");
        
        await ToSignal(_animationPlayer, AnimationPlayer.SignalName.AnimationFinished);
        
        _animationPlayer.Play("idle");
        //_sprite.FlipH = false;
        
        _isPouring = false;
        
        
    }

    private void ThrowBeer()
    {
        Beer newBeer = BeerScene.Instantiate<Beer>();
        newBeer.GlobalPosition = _beerSpawnMarker.GlobalPosition;
        GetTree().CurrentScene.AddChild(newBeer);
    }
}
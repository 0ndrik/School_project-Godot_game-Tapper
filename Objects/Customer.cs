using Godot;
using System;
using System.Collections.Generic;

public partial class Customer : Area2D
{
    [Export] public float WalkSpeed { get; set; } = 30.0f; 
    [Export] public float SlideBackSpeed { get; set; } = 150.0f;
    [Export] public float SlideBackSec { get; set; } = 0.6f;
    [Export] public PackedScene EmptyGlassScene { get; set; }

    public Vector2 Direction { get; set; } = Vector2.Right;
    
    private AnimationPlayer _animationPlayer;
    private Marker2D _emptyGlassSpawnMarker;
    
    // --- Spoločný zoznam pre striedanie zákazníkov ---
    private static List<int> _availableSkins = new List<int>();
    private static RandomNumberGenerator _rng = new RandomNumberGenerator();
    
    private int _mySkinIndex = 0;
    private bool _canGoOut = false;
    
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
        
        SetupRandomSkin();
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
            // --- UPRAVENÉ: Dynamicky lepíme názov animácie ---
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
    
    private void ThrowEmptyGlass()
    {
        EmptyGlass newEmptyGlass = EmptyGlassScene.Instantiate<EmptyGlass>();
        newEmptyGlass.GlobalPosition = _emptyGlassSpawnMarker.GlobalPosition;
        GetTree().Root.AddChild(newEmptyGlass);
    }

    private void _on_animation_player_animation_finished(StringName animName)
    {
        // --- UPRAVENÉ: Kontrolujeme, či skončila animácia pitia konkrétneho zákazníka ---
        if (animName == $"drink_{_mySkinIndex}")
        {
            _currentState = CustomerState.Walking;
        }
    }
}
using Godot;

public partial class CustomerSpawner : Node
{
    [Export] public PackedScene CustomerScene { get; set; }
    [Export] public Godot.Collections.Array<Marker2D> SpawnPoints { get; set; }

    // Základný čas medzi spawnami 
    [Export] public float BaseBetweenTime { get; set; } = 3.0f;

    // O koľko sekúnd sa môže čas náhodne líšiť 
    [Export] public float TimeVariance { get; set; } = 1.5f;
    
    // Či je spawner aktívny
    [Export] public bool IsSpawning { get; set; } = true;

    private Timer _spawnTimer;
    private RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        _rng.Randomize();
        
        _spawnTimer = new Timer();
        _spawnTimer.OneShot = true;
        _spawnTimer.Timeout += OnSpawnTimerTimeout;
        AddChild(_spawnTimer);

        if (IsSpawning)
        {
            ScheduleNextSpawn();
        }
    }

    private void ScheduleNextSpawn()
    {
        // Vypočíta náhodný čas. Ak je Base = 3 a Variance = 1.5, čas bude náhodne medzi 1.5 a 4.5 sekundami.
        float nextTime = BaseBetweenTime + _rng.RandfRange(-TimeVariance, TimeVariance);

        _spawnTimer.Start(nextTime);
    }

    private void OnSpawnTimerTimeout()
    {
        if (IsSpawning)
        {
            SpawnCustomer();
        }
        
        // Naplánujeme ďalšieho zákazníka
        ScheduleNextSpawn();
    }

    private void SpawnCustomer()
    {
        // Vyberieme náhodný pult (bar), kde sa zákazník objaví
        int randomIndex = _rng.RandiRange(0, SpawnPoints.Count - 1);
        Marker2D spawnPoint = SpawnPoints[randomIndex];
        
        Node2D customer = CustomerScene.Instantiate<Node2D>();
        customer.GlobalPosition = spawnPoint.GlobalPosition;
        customer.ZIndex = 1;
        GetTree().CurrentScene.AddChild(customer);
        
        // Voliteľné: Ak tvoj zákazník potrebuje vedieť, na ktorom je pulte (napr. výška Y), 
        // môžeš mu tu zavolať nejakú metódu, napr:
        // (customer as Customer).InitializeLane(spawnPoint.GlobalPosition.Y);
    }
}
# Ludo Events System

The Ludo Events system provides two complementary communication patterns for Unity applications: **EventHub** for global communication and **Signals** for local component communication.

## Overview

- **EventHub**: Global event system for application-wide communication (level started, score changed, etc.)
- **Signals**: Local event system for component-to-component communication and connected systems

## EventHub (Global Events)

The EventHub is a centralized event system designed for global application events that need to be communicated across different systems and scenes.

### Key Features

- **Global Scope**: Events are accessible throughout the entire application
- **Type-Safe**: Uses generic constraints to ensure type safety
- **Decoupled**: Publishers and subscribers don't need direct references
- **Service Integration**: Registered as a service through the ServiceLocator

### Core Components

#### IEventHub Interface
```csharp
public interface IEventHub 
{
    void Subscribe<T>(Action<T> h) where T : IEvent;
    void Unsubscribe<T>(Action<T> h) where T : IEvent;
    void Publish<T>(in T e) where T : IEvent;
}
```

#### IEvent Interface
All events must implement the `IEvent` interface:
```csharp
public interface IEvent
{
    // Marker interface for event types
}
```

### Usage Examples

#### Creating an Event
```csharp
public struct LevelStartedEvent : IEvent
{
    public int LevelNumber { get; }
    public string LevelName { get; }
    
    public LevelStartedEvent(int levelNumber, string levelName)
    {
        LevelNumber = levelNumber;
        LevelName = levelName;
    }
}

public struct ScoreChangedEvent : IEvent
{
    public int NewScore { get; }
    public int PreviousScore { get; }
    
    public ScoreChangedEvent(int newScore, int previousScore)
    {
        NewScore = newScore;
        PreviousScore = previousScore;
    }
}
```

#### Publishing Events
```csharp
// Get the EventHub from ServiceLocator
var eventHub = ServiceLocator.Get<IEventHub>();

// Publish a level started event
eventHub.Publish(new LevelStartedEvent(1, "Forest Level"));

// Publish a score changed event
eventHub.Publish(new ScoreChangedEvent(1500, 1000));
```

#### Subscribing to Events
```csharp
public class GameManager : MonoBehaviour
{
    private IEventHub _eventHub;
    
    private void Awake()
    {
        _eventHub = ServiceLocator.Get<IEventHub>();
        
        // Subscribe to events
        _eventHub.Subscribe<LevelStartedEvent>(OnLevelStarted);
        _eventHub.Subscribe<ScoreChangedEvent>(OnScoreChanged);
    }
    
    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        _eventHub.Unsubscribe<LevelStartedEvent>(OnLevelStarted);
        _eventHub.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
    }
    
    private void OnLevelStarted(LevelStartedEvent evt)
    {
        Debug.Log($"Level {evt.LevelNumber} started: {evt.LevelName}");
    }
    
    private void OnScoreChanged(ScoreChangedEvent evt)
    {
        Debug.Log($"Score changed from {evt.PreviousScore} to {evt.NewScore}");
    }
}
```

### Best Practices for EventHub

- Use for **global application events** (level transitions, score updates, game state changes)
- Always unsubscribe in `OnDestroy()` to prevent memory leaks
- Use structs for events when possible for better performance
- Keep event data immutable
- Use descriptive event names that clearly indicate what happened

## Signals (Local Events)

Signals provide a lightweight, type-safe event system for local communication between components and connected systems.

### Key Features

- **Local Scope**: Designed for component-to-component communication
- **Type-Safe**: Strongly typed parameters with compile-time checking
- **Flexible Parameters**: Support for 0-3 parameters
- **Memory Efficient**: Uses HashSet for listener management
- **Signal Binding**: Optional SignalBinder for automatic cleanup

### Core Components

#### Signal Types

- **Signal**: Parameterless signals
- **Signal\<T>**: Single parameter signals
- **Signal\<T1, T2>**: Two parameter signals  
- **Signal\<T1, T2, T3>**: Three parameter signals

#### BaseSignal
Abstract base class providing core functionality:
- Listener management (Add/Remove/Clear)
- Thread-safe invocation
- Memory leak prevention

### Usage Examples

#### Basic Signal Usage
```csharp
public class PlayerController : MonoBehaviour
{
    // Parameterless signal
    public Signal OnPlayerDied = new Signal();
    
    // Single parameter signal
    public Signal<int> OnHealthChanged = new Signal<int>();
    
    // Multiple parameter signal
    public Signal<Vector3, float> OnPlayerMoved = new Signal<Vector3, float>();
    
    private void TakeDamage(int damage)
    {
        health -= damage;
        OnHealthChanged.Invoke(health);
        
        if (health <= 0)
        {
            OnPlayerDied.Invoke();
        }
    }
    
    private void Update()
    {
        if (transform.hasChanged)
        {
            OnPlayerMoved.Invoke(transform.position, Time.deltaTime);
            transform.hasChanged = false;
        }
    }
}
```

#### Listening to Signals
```csharp
public class UIHealthBar : MonoBehaviour
{
    private PlayerController _player;
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        
        // Subscribe to signals
        _player.OnHealthChanged.AddListener(UpdateHealthBar);
        _player.OnPlayerDied.AddListener(ShowGameOverScreen);
    }
    
    private void OnDestroy()
    {
        // Clean up listeners
        if (_player != null)
        {
            _player.OnHealthChanged.RemoveListener(UpdateHealthBar);
            _player.OnPlayerDied.RemoveListener(ShowGameOverScreen);
        }
    }
    
    private void UpdateHealthBar(int newHealth)
    {
        // Update UI health bar
    }
    
    private void ShowGameOverScreen()
    {
        // Show game over UI
    }
}
```

#### Using SignalBinder for Automatic Cleanup
```csharp
public class EnemyAI : MonoBehaviour
{
    private SignalBinder _signalBinder = new SignalBinder();
    private PlayerController _player;
    
    private void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        
        // Bind signals - automatic cleanup on Unbind()
        _signalBinder.Bind(_player.OnPlayerDied, OnPlayerDied);
        _signalBinder.Bind(_player.OnPlayerMoved, OnPlayerMoved);
    }
    
    private void OnDestroy()
    {
        // Automatically unbinds all signals
        _signalBinder.Unbind();
    }
    
    private void OnPlayerDied()
    {
        // Stop chasing player
    }
    
    private void OnPlayerMoved(Vector3 position, float deltaTime)
    {
        // Update AI behavior based on player movement
    }
}
```

### Best Practices for Signals

- Use for **local component communication** (UI updates, gameplay interactions)
- Always remove listeners in `OnDestroy()` or use SignalBinder
- Use SignalBinder for complex objects with many signal subscriptions
- Keep signal names descriptive and action-oriented (OnHealthChanged, OnPlayerDied)
- Prefer signals over UnityEvents for performance-critical code

## When to Use Each System

### Use EventHub When:
- Communicating across different scenes or systems
- Broadcasting global game state changes
- Implementing application-wide features (analytics, achievements)
- Events need to persist across object lifecycles

### Use Signals When:
- Communicating between closely related components
- Implementing UI interactions and updates
- Creating reusable component behaviors
- Performance is critical (signals have less overhead)

## Integration with ServiceLocator

The EventHub is automatically registered as a service in the `CoreInstaller`:

```csharp
protected override void Install()
{
    ServiceLocator.Register<IEventHub>(new EventHub());
    // ... other services
}
```

Access it anywhere in your code:
```csharp
var eventHub = ServiceLocator.Get<IEventHub>();
```

## Performance Considerations

- **EventHub**: Slightly higher overhead due to type lookups and service resolution
- **Signals**: Minimal overhead, direct method calls
- Both systems use efficient collection types (Dictionary/HashSet)
- Always unsubscribe to prevent memory leaks
- Consider using structs for event data to reduce GC pressure

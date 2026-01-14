using Raylib_cs;

namespace RedOwl;

public interface IButtonState
{
    float Value { get; }
    bool Pressed { get; }
    bool Released { get; }
    bool WasPressedThisFrame { get; }
    bool WasReleasedThisFrame { get; }
    bool ChangedThisFrame { get; }
}

public class ButtonState(int key) : IButtonState
{
    public int Key { get; init; } = key;
    public bool State { get; internal set; }
    public bool LastState { get; internal set; }
    
    public float Value => State ? 1f : 0f;
    
    public void Set(bool value)
    {
        LastState = State;
        State = value;
    }

    public bool Pressed => State;
    public bool Released => !State;
    public bool WasPressedThisFrame => State && !LastState;
    public bool WasReleasedThisFrame => !State && LastState;
    public bool ChangedThisFrame => WasPressedThisFrame || WasReleasedThisFrame;
    
    public ButtonState(MouseButton key) : this((int)key) {}
    public ButtonState(KeyboardKey key) : this((int)key) {}
    public ButtonState(GamepadButton key) : this((int)key) {}
}

public class ButtonAxis(ButtonState positive, ButtonState negative)
{
    public ButtonState Positive { get; init; } = positive;
    public ButtonState Negative { get; init; } = negative;
    
    public float Value => Positive.Value + -Negative.Value;
}

public class ButtonControl : IControl
{
    private bool _disposed = false;
    public bool Enabled { get; set; }

    private readonly List<IButtonState> _bindings = [];
    private readonly List<IButtonInteraction> _interactions = [];
    
    // Cached interaction results for querying
    private DoubleTapInteraction? _doubleTapInteraction;
    private HoldInteraction? _holdInteraction;

    public ButtonControl()
    {
        Input._controls.Add(this);
    }

    ~ButtonControl()
    {
        Dispose();
    }
    
    public void Dispose()
    {
        if (_disposed) return;
        Input._controls.Remove(this);
        _disposed = true;
    }

    public void Update(float dt)
    {
        if (!Enabled)
        {
            foreach (var interaction in _interactions) interaction.Reset();
            return;
        }
        
        var context = new ButtonInteractionContext(
            PressedInternal,
            WasPressedThisFrameInternal,
            ReleasedInternal,
            WasReleasedThisFrameInternal,
            Enabled,
            dt
        );
        foreach (var interaction in _interactions)
        {
            interaction.Process(context);
        }
    }

    public ButtonControl Bind(IButtonState source)
    {
        _bindings.Add(source);
        return this;
    }
    
    public ButtonControl Enable()
    {
        Enabled = true;
        return this;
    }

    /// <summary>
    /// Adds an interaction to this control. Interactions are processed each frame during Update().
    /// </summary>
    public ButtonControl With(IButtonInteraction interaction)
    {
        _interactions.Add(interaction);

        switch (interaction)
        {
            case DoubleTapInteraction doubleTap:
                _doubleTapInteraction = doubleTap;
                break;
            case HoldInteraction hold:
                _holdInteraction = hold;
                break;
        }
        
        return this;
    }
    
    private bool PressedInternal => _bindings.Any(binding => binding.Pressed);
    private bool WasPressedThisFrameInternal => _bindings.Any(binding => binding.WasPressedThisFrame);
    private bool ReleasedInternal => _bindings.All(binding => binding.Released);
    private bool WasReleasedThisFrameInternal => _bindings.Any(binding => binding.WasReleasedThisFrame);
    
    public float Value => Enabled ? Math.Clamp(_bindings.Sum(b => b.Value), 0, 1) : 0f;

    /// <summary>
    /// Returns true if any binding is held down.
    /// </summary>
    public bool Pressed => Enabled && PressedInternal;
    
    /// <summary>
    /// Returns true if any binding was pressed this frame update.
    /// </summary>
    public bool WasPressedThisFrame => Enabled && WasPressedThisFrameInternal;
    
    /// <summary>
    /// Returns true if all bindings are not held down.
    /// </summary>
    public bool Released => Enabled && ReleasedInternal;
    
    /// <summary>
    /// Returns true if any binding was released this frame update.
    /// </summary>
    public bool WasReleasedThisFrame => Enabled && WasReleasedThisFrameInternal;

    /// <summary>
    /// Returns true if a double-tap was detected this frame.
    /// Returns false if DoubleTapInteraction is not configured for this control.
    /// </summary>
    public bool WasDoubleTappedThisFrame => _doubleTapInteraction?.WasDoubleTappedThisFrame ?? false;

    /// <summary>
    /// Returns the number of times the hold interaction should trigger this frame (0, 1, or >1 if frame time is large).
    /// Returns 0 if HoldInteraction is not configured for this control.
    /// </summary>
    public int HeldCountThisFrame => _holdInteraction?.HeldCountThisFrame ?? 0;
}
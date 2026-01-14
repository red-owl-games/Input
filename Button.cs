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
    
    /// <summary>
    /// If true, requires all bindings to be active simultaneously (combo/chord input).
    /// If false (default), any binding being active will trigger the control.
    /// </summary>
    public bool RequireAllBindings { get; set; } = false;

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

    /// <summary>
    /// Configures this control to require all bindings to be active simultaneously (combo/chord input).
    /// By default, any binding being active will trigger the control.
    /// </summary>
    public ButtonControl RequireAll()
    {
        RequireAllBindings = true;
        return this;
    }
    
    private bool PressedInternal => RequireAllBindings 
        ? _bindings.Count > 0 && _bindings.All(binding => binding.Pressed)
        : _bindings.Any(binding => binding.Pressed);
    
    private bool WasPressedThisFrameInternal
    {
        get
        {
            if (RequireAllBindings)
            {
                // For combo inputs: all bindings must be pressed, and the combo activates
                // when the last required binding is pressed while all others are already pressed
                if (_bindings.Count == 0) return false;
                
                // Check if all bindings are currently pressed
                if (!_bindings.All(binding => binding.Pressed)) return false;
                
                // Check if at least one binding was pressed this frame
                // This ensures we detect when the combo becomes complete
                return _bindings.Any(binding => binding.WasPressedThisFrame);
            }
            else
            {
                return _bindings.Any(binding => binding.WasPressedThisFrame);
            }
        }
    }
    
    private bool ReleasedInternal => RequireAllBindings
        ? _bindings.Any(binding => binding.Released)  // Combo broken if any binding released
        : _bindings.All(binding => binding.Released);
    
    private bool WasReleasedThisFrameInternal => _bindings.Any(binding => binding.WasReleasedThisFrame);
    
    public float Value => Enabled ? Math.Clamp(_bindings.Sum(b => b.Value), 0, 1) : 0f;

    /// <summary>
    /// Returns true if the control is active.
    /// - If RequireAllBindings is false: returns true if any binding is held down.
    /// - If RequireAllBindings is true: returns true if all bindings are held down simultaneously.
    /// </summary>
    public bool Pressed => Enabled && PressedInternal;
    
    /// <summary>
    /// Returns true if the control was activated this frame.
    /// - If RequireAllBindings is false: returns true if any binding was pressed this frame.
    /// - If RequireAllBindings is true: returns true when all bindings become pressed simultaneously (combo activated).
    /// </summary>
    public bool WasPressedThisFrame => Enabled && WasPressedThisFrameInternal;
    
    /// <summary>
    /// Returns true if the control is not active.
    /// - If RequireAllBindings is false: returns true if all bindings are not held down.
    /// - If RequireAllBindings is true: returns true if any binding is released (combo broken).
    /// </summary>
    public bool Released => Enabled && ReleasedInternal;
    
    /// <summary>
    /// Returns true if the control was deactivated this frame.
    /// - If RequireAllBindings is false: returns true if any binding was released this frame.
    /// - If RequireAllBindings is true: returns true if any binding was released this frame (combo broken).
    /// </summary>
    public bool WasReleasedThisFrame => Enabled && WasReleasedThisFrameInternal;

    /// <summary>
    /// Returns true if a double-tap was detected this frame.
    /// Returns false if DoubleTapInteraction is not configured for this control.
    /// </summary>
    public bool WasDoubleTappedThisFrame => _doubleTapInteraction?.WasDoubleTappedThisFrame ?? false;



    /// <summary>
    /// Returns true if the button has been held past the long press threshold and is still being held.
    /// Returns false if HoldInteraction is not configured or LongPressMode is disabled.
    /// </summary>
    public bool IsHeld => _holdInteraction?.IsHeld ?? false;

    /// <summary>
    /// Returns true if the long press threshold was crossed this frame (edge detection).
    /// Returns false if HoldInteraction is not configured or LongPressMode is disabled.
    /// </summary>
    public bool WasHeldThisFrame => _holdInteraction?.WasHeldThisFrame ?? false;
    
    /// <summary>
    /// Returns the number of times the hold interaction should trigger this frame (0, 1, or >1 if frame time is large).
    /// Returns 0 if HoldInteraction is not configured or RepeatMode is disabled.
    /// </summary>
    public int HoldCountThisFrame => _holdInteraction?.HoldCountThisFrame ?? 0;

    /// <summary>
    /// Returns the progress of the long press action as a normalized value between 0.0 and 1.0.
    /// Returns 0.0 when the button is not being held, just pressed, or if HoldInteraction is not configured or LongPressMode is disabled.
    /// Returns a value between 0.0 and 1.0 while the button is held (1.0 = threshold reached or exceeded).
    /// Useful for driving UI progress bars. Check if the button is pressed before using this value.
    /// </summary>
    public float HoldProgress => _holdInteraction?.LongPressProgress ?? 0f;
}
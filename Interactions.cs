namespace RedOwl;

/// <summary>
/// Context passed to interactions during Update to process button state.
/// </summary>
public readonly struct ButtonInteractionContext
{
    public readonly bool Pressed;
    public readonly bool WasPressedThisFrame;
    public readonly bool Released;
    public readonly bool WasReleasedThisFrame;
    public readonly bool Enabled;
    public readonly float DeltaTime;

    public ButtonInteractionContext(bool pressed, bool wasPressedThisFrame, bool released, bool wasReleasedThisFrame, bool enabled, float dt)
    {
        Pressed = pressed;
        WasPressedThisFrame = wasPressedThisFrame;
        Released = released;
        WasReleasedThisFrame = wasReleasedThisFrame;
        Enabled = enabled;
        DeltaTime = dt;
    }
}

/// <summary>
/// Base interface for all button interactions.
/// Interactions process button state each frame and produce results that can be queried.
/// </summary>
public interface IButtonInteraction
{
    /// <summary>
    /// Called once per frame during ButtonControl.Update() to process the current button state.
    /// </summary>
    void Process(ButtonInteractionContext context);
    
    /// <summary>
    /// Resets the interaction state. Called when the control is disabled or reset.
    /// </summary>
    void Reset();
}

/// <summary>
/// Interaction that detects double-tap gestures.
/// </summary>
public class DoubleTapInteraction : IButtonInteraction
{
    /// <summary>
    /// Maximum time (in seconds) between two taps to be considered a double-tap.
    /// </summary>
    public float Threshold { get; set; } = 0.3f;
    
    private float _timeSincePressed = -1f;
    private bool _wasActivatedThisFrame;

    public void Process(ButtonInteractionContext context)
    {
        _wasActivatedThisFrame = false;
        
        if (!context.Enabled)
        {
            Reset();
            return;
        }

        if (context.WasPressedThisFrame)
        {
            if (_timeSincePressed >= 0f && _timeSincePressed < Threshold)
            {
                // Double tap detected!
                _wasActivatedThisFrame = true;
                _timeSincePressed = -1f; // Reset to prevent triple-tap
            }
            else
            {
                // First tap - start timing
                _timeSincePressed = 0f;
            }
        }
        
        // Increment time tracking if we're waiting for a potential second tap
        if (_timeSincePressed >= 0f)
        {
            _timeSincePressed += context.DeltaTime;
            // Clear if threshold exceeded (too much time passed)
            if (_timeSincePressed >= Threshold)
            {
                _timeSincePressed = -1f;
            }
        }
    }

    public void Reset()
    {
        _timeSincePressed = -1f;
        _wasActivatedThisFrame = false;
    }

    public bool WasDoubleTappedThisFrame => _wasActivatedThisFrame;
}

/// <summary>
/// Interaction that handles hold/repeat logic for button presses.
/// </summary>
public class HoldInteraction : IButtonInteraction
{
    /// <summary>
    /// Initial delay (in seconds) before repeat starts.
    /// </summary>
    public float InitialRepeatDelay { get; set; } = 0.3f;
    
    /// <summary>
    /// Interval (in seconds) between repeat triggers after initial delay.
    /// </summary>
    public float RepeatInterval { get; set; } = 0.15f;
    
    /// <summary>
    /// If true, triggers once immediately on press, then repeats after delay.
    /// If false, only triggers after initial delay, then repeats.
    /// </summary>
    public bool FireOnPress { get; set; } = true;

    private bool _pressedLast;
    private float _repeatTimer;
    private int _heldCountThisFrame;

    public void Process(ButtonInteractionContext context)
    {
        _heldCountThisFrame = 0;
        
        if (!context.Enabled)
        {
            Reset();
            return;
        }

        var down = context.Pressed;

        if (!down)
        {
            _repeatTimer = 0f;
        }
        else
        {
            if (!_pressedLast)
            {
                // Rising edge - start repeat timer
                _repeatTimer = InitialRepeatDelay;
                if (FireOnPress)
                {
                    _heldCountThisFrame = 1;
                }
            }
            else
            {
                // Still held - process repeat
                _repeatTimer -= context.DeltaTime;
                while (_repeatTimer <= 0f)
                {
                    _heldCountThisFrame++;
                    _repeatTimer += RepeatInterval; // Keep remainder for stable cadence
                }
            }
        }

        _pressedLast = down;
    }

    public void Reset()
    {
        _pressedLast = false;
        _repeatTimer = 0f;
        _heldCountThisFrame = 0;
    }

    public int HeldCountThisFrame => _heldCountThisFrame;
}

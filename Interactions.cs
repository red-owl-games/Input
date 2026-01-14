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
/// Interaction that handles hold behavior for button presses.
/// Supports two modes: repeat (triggers repeatedly while held) and long press (triggers once after duration).
/// Both modes can be enabled simultaneously.
/// </summary>
public class HoldInteraction : IButtonInteraction
{
    /// <summary>
    /// If true, enables repeat mode - triggers repeatedly while the button is held.
    /// </summary>
    public bool RepeatMode { get; set; } = true;
    
    /// <summary>
    /// If true, enables long press mode - detects when button is held for a specified duration.
    /// </summary>
    public bool LongPressMode { get; set; } = false;
    
    #region Repeat Mode Configuration
    
    /// <summary>
    /// Initial delay (in seconds) before repeat starts. Only used when RepeatMode is true.
    /// </summary>
    public float InitialRepeatDelay { get; set; } = 0.3f;
    
    /// <summary>
    /// Interval (in seconds) between repeat triggers after initial delay. Only used when RepeatMode is true.
    /// </summary>
    public float RepeatInterval { get; set; } = 0.15f;
    
    /// <summary>
    /// If true, triggers once immediately on press, then repeats after delay.
    /// If false, only triggers after initial delay, then repeats.
    /// Only used when RepeatMode is true.
    /// </summary>
    public bool FireOnPress { get; set; } = true;
    
    #endregion
    
    #region Long Press Mode Configuration
    
    /// <summary>
    /// Duration (in seconds) the button must be held before triggering. Only used when LongPressMode is true.
    /// </summary>
    public float LongPressThreshold { get; set; } = 3.0f;
    
    #endregion

    // Repeat mode state
    private bool _pressedLast;
    private float _repeatTimer;
    private int _heldCountThisFrame;
    
    // Long press mode state
    private float _holdTime;
    private bool _wasHeldPastThresholdThisFrame;
    private bool _isHeldPastThreshold;

    public void Process(ButtonInteractionContext context)
    {
        // Reset frame-specific values
        _heldCountThisFrame = 0;
        _wasHeldPastThresholdThisFrame = false;
        
        if (!context.Enabled)
        {
            Reset();
            return;
        }

        var pressed = context.Pressed;

        if (!pressed)
        {
            // Button released - reset state
            if (_pressedLast)
            {
                if (RepeatMode)
                {
                    _repeatTimer = 0f;
                    _heldCountThisFrame = 0;
                }
                if (LongPressMode)
                {
                    _holdTime = 0f;
                    _isHeldPastThreshold = false;
                    _wasHeldPastThresholdThisFrame = false;
                }
            }
        }
        else
        {
            // Button is pressed
            if (!_pressedLast)
            {
                // Rising edge - initialize both modes
                if (RepeatMode)
                {
                    _repeatTimer = InitialRepeatDelay;
                    if (FireOnPress)
                    {
                        _heldCountThisFrame = 1;
                    }
                }
                
                if (LongPressMode)
                {
                    _holdTime = 0f;
                    _isHeldPastThreshold = false;
                }
            }
            else
            {
                // Still held - process both modes
                if (RepeatMode)
                {
                    // Process repeat
                    _repeatTimer -= context.DeltaTime;
                    while (_repeatTimer <= 0f)
                    {
                        _heldCountThisFrame++;
                        _repeatTimer += RepeatInterval; // Keep remainder for stable cadence
                    }
                }
                
                if (LongPressMode)
                {
                    // Process long press
                    _holdTime += context.DeltaTime;
                    
                    // Check if threshold crossed this frame
                    if (!_isHeldPastThreshold && _holdTime >= LongPressThreshold)
                    {
                        _isHeldPastThreshold = true;
                        _wasHeldPastThresholdThisFrame = true;
                    }
                }
            }
        }

        _pressedLast = pressed;
    }

    public void Reset()
    {
        _pressedLast = false;
        
        if (RepeatMode)
        {
            _repeatTimer = 0f;
            _heldCountThisFrame = 0;
        }
        
        if (LongPressMode)
        {
            _holdTime = 0f;
            _isHeldPastThreshold = false;
            _wasHeldPastThresholdThisFrame = false;
        }
    }

    #region Repeat Mode Properties
    
    /// <summary>
    /// Returns the number of times the repeat should trigger this frame (0, 1, or >1 if frame time is large).
    /// Only meaningful when RepeatMode is true.
    /// </summary>
    public int HoldCountThisFrame => RepeatMode ? _heldCountThisFrame : 0;
    
    #endregion
    
    #region Long Press Mode Properties
    
    /// <summary>
    /// Returns true if the button has been held past the threshold and is still being held.
    /// Only meaningful when LongPressMode is true.
    /// </summary>
    public bool IsHeld => LongPressMode && _isHeldPastThreshold;

    /// <summary>
    /// Returns true if the threshold was crossed this frame (edge detection).
    /// Only meaningful when LongPressMode is true.
    /// </summary>
    public bool WasHeldThisFrame => LongPressMode && _wasHeldPastThresholdThisFrame;

    /// <summary>
    /// Returns the progress of the long press action as a normalized value between 0.0 and 1.0.
    /// Returns 0.0 when the button is not being held, just pressed, or LongPressMode is false.
    /// Returns a value between 0.0 and 1.0 while the button is held (1.0 = threshold reached or exceeded).
    /// Useful for driving UI progress bars. Check if the button is pressed before using this value.
    /// Only meaningful when LongPressMode is true.
    /// </summary>
    public float LongPressProgress => LongPressMode && LongPressThreshold > 0f 
        ? Math.Clamp(_holdTime / LongPressThreshold, 0f, 1f) 
        : 0f;
    
    #endregion
}

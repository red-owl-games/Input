using Raylib_cs;

namespace RedOwl;

public class ButtonState(int key)
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

public class ButtonControl
{
    public bool Enabled;
    public float InitialRepeatDelay = 0.3f;
    public float RepeatInterval = 0.15f;
    public bool  FireOnPress = true;
    private bool _pressedLast = false;
    private float _repeatTimer = 0;

    private readonly List<ButtonState> _bindings = [];

    public ButtonControl Bind(ButtonState source)
    {
        _bindings.Add(source);
        return this;
    }
    
    public ButtonControl Enable()
    {
        Enabled = true;
        return this;
    }
    
    public float Value => Enabled ? Math.Clamp(_bindings.Sum(b => b.Value), 0, 1) : 0f;

    public bool Pressed => Enabled && _bindings.Any(binding => binding.Pressed);
    public bool WasPressedThisFrame => Enabled && _bindings.Any(binding => binding.WasPressedThisFrame);
    public bool Released => Enabled && _bindings.Any(binding => binding.Released);
    public bool WasReleasedThisFrame => Enabled && _bindings.Any(binding => binding.WasReleasedThisFrame);
    

    /// <summary>
    /// Returns how many times to trigger this frame (0,1, or >1 if frame time is large).
    /// Call this once per frame with dt.
    /// </summary>
    public int IsHeld(float dt)
    {
        if (!Enabled)
        {
            _pressedLast = false;
            _repeatTimer = 0;
            return 0;
        }

        var down = _bindings.Any(binding => binding.Pressed);
        var count = 0;

        if (!down)
        {
            _repeatTimer = 0f;
            if (_pressedLast) { /* release edge if you need it elsewhere */ }
        }
        else
        {
            if (!_pressedLast)
            {
                // rising edge
                _repeatTimer = InitialRepeatDelay;
                if (FireOnPress) count = 1;
            }
            else
            {
                _repeatTimer -= dt;
                while (_repeatTimer <= 0f)
                {
                    count++;
                    _repeatTimer += RepeatInterval; // keep remainder for stable cadence
                }
            }
        }

        _pressedLast = down;
        return count;
    }
}
using System.Runtime.CompilerServices;

namespace RedOwl;

public class OneAxisPositiveButtonState(OneAxisState state) : IButtonState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPressed(float value) => value > Input.AxisDeadzone;
    
    public float Value => Math.Max(state.Value, 0f);
    public bool Pressed => IsPressed(state.Value);
    public bool Released => !IsPressed(state.Value);
    
    public bool WasPressedThisFrame => 
        state.ChangedThisFrame && IsPressed(state.Value) && !IsPressed(state.LastValue);

    public bool WasReleasedThisFrame => 
        state.ChangedThisFrame && !IsPressed(state.Value) && IsPressed(state.LastValue);

    public bool ChangedThisFrame => 
        state.ChangedThisFrame && IsPressed(state.Value) != IsPressed(state.LastValue);
}

public class OneAxisNegativeButtonState(OneAxisState state) : IButtonState
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsPressed(float value) => value < -Input.AxisDeadzone;
    
    public float Value => Math.Max(-state.Value, 0f); // Return positive value for consistency with IButtonState
    public bool Pressed => IsPressed(state.Value);
    public bool Released => !IsPressed(state.Value);
    
    public bool WasPressedThisFrame => 
        state.ChangedThisFrame && IsPressed(state.Value) && !IsPressed(state.LastValue);

    public bool WasReleasedThisFrame => 
        state.ChangedThisFrame && !IsPressed(state.Value) && IsPressed(state.LastValue);

    public bool ChangedThisFrame => 
        state.ChangedThisFrame && IsPressed(state.Value) != IsPressed(state.LastValue);
}

public class OneAxisState
{
    private float _value;
    
    public string Name { get; }
    public IButtonState Pos { get; }
    public IButtonState Neg { get; }
    
    public float LastValue { get; private set; }
    
    public float Value
    {
        get  => _value;
        set
        {
            LastValue = _value;
            ChangedThisFrame = float.Abs(_value - value) > Input.AxisChangeThreshold;
            _value = value;
        }
    }

    public OneAxisState(string name)
    {
        Name = name;
        Pos = new OneAxisPositiveButtonState(this);
        Neg = new OneAxisNegativeButtonState(this);
    }
    
    public bool ChangedThisFrame { get; private set; }
}

public class OneAxisControl
{
    public bool Enabled { get; set; }

    private readonly List<OneAxisState> _bindings = [];
    private readonly List<ButtonAxis> _composites = [];

    public OneAxisControl Enable()
    {
        Enabled = true;
        return this;
    }

    public OneAxisControl Bind(OneAxisState source)
    {
        _bindings.Add(source);
        return this;
    }

    public OneAxisControl Bind(ButtonState pos, ButtonState neg)
    {
        _composites.Add(new ButtonAxis(pos, neg));
        return this;
    }

    public float Value => Enabled ? Math.Clamp(_bindings.Sum(b => b.Value) + _composites.Sum(c => c.Value), -1, 1) : 0;
}
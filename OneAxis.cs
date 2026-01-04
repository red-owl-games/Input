namespace RedOwl;

public class OneAxisState(string name)
{
    public string Name { get; init; } = name;

    private float _value;
    public float Value
    {
        get  => _value;
        set
        {
            ChangedThisFrame = float.Abs(_value - value) > Input.AxisChangeThreshold;
            _value = value;
        }
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
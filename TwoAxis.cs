using System.Numerics;

namespace RedOwl;

public class TwoAxisState(string name)
{
    public string Name { get; init; } = name;
    public OneAxisState X { get; init; } = new($"{name}X");
    public OneAxisState Y { get; init; } = new($"{name}Y");
    
    public Vector2 Value => new(X.Value, Y.Value);
    
    public bool ChangedThisFrame => X.ChangedThisFrame || Y.ChangedThisFrame;
}

public class TwoAxisControl
{
    public OneAxisControl X { get; init; } = new();
    public OneAxisControl Y { get; init; } = new();

    public TwoAxisControl Enable()
    {
        X.Enable();
        Y.Enable();
        return this;
    }

    public TwoAxisControl Bind(TwoAxisState source)
    {
        X.Bind(source.X);
        Y.Bind(source.Y);
        return this;
    }

    public TwoAxisControl Bind(OneAxisState x, OneAxisState y)
    {
        X.Bind(x);
        Y.Bind(y);
        return this;
    }

    public TwoAxisControl Bind(ButtonState xPos, ButtonState xNeg, ButtonState yPos, ButtonState yNeg)
    {
        X.Bind(xPos, xNeg);
        Y.Bind(yPos, yNeg);
        return this;
    }
    
    public Vector2 Value => new(X.Value, Y.Value);
}
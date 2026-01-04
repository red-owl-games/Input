using Raylib_cs;

namespace RedOwl;

public class Mouse
{
    public bool Enabled { get; set; } = true;
    public TwoAxisState ScrollDelta { get; } = new("MouseScrollDelta");
    public TwoAxisState ScrollDeltaClamped { get; } = new("MouseScrollDeltaClamped");
    public TwoAxisState Position  { get; } = new("MousePosition");
    public TwoAxisState PositionDelta  { get; } = new("MousePositionDelta");
    public TwoAxisState PositionDeltaClamped { get; } = new("MousePositionDeltaClamped");

    
    public ButtonState LeftButton { get; } = new(MouseButton.Left);
    public ButtonState RightButton { get; } = new(MouseButton.Right);
    public ButtonState MiddleButton { get; } = new(MouseButton.Middle);
    public ButtonState SideButton { get; } = new(MouseButton.Side);
    public ButtonState ExtraButton { get; } = new(MouseButton.Extra);
    public ButtonState ForwardButton { get; } = new(MouseButton.Forward);
    public ButtonState BackButton { get; } = new(MouseButton.Back);

    public TwoAxisState[] Axis;
    public ButtonState[] Buttons;

    public bool AnyButton => Buttons.Any(b => b.WasPressedThisFrame);

    public Mouse()
    {
        Axis =
        [
            ScrollDelta, ScrollDeltaClamped, Position, PositionDelta, PositionDeltaClamped,
        ];
        Buttons =
        [
            LeftButton, RightButton, MiddleButton, SideButton, ExtraButton, ForwardButton, BackButton,
        ];
    }

    public void MoveTo(int x, int y) => Raylib.SetMousePosition(x, y);

    public void Read()
    {
        if (!Enabled) return;
        
        var pos = Raylib.GetMousePosition();
        Position.X.Value = pos.X;
        Position.Y.Value = pos.Y;
        
        var delta = Raylib.GetMouseDelta();
        PositionDelta.X.Value = delta.X;
        PositionDelta.Y.Value = delta.Y;
        PositionDeltaClamped.X.Value = Math.Clamp(delta.X, -1, 1);
        PositionDeltaClamped.Y.Value = Math.Clamp(delta.Y, -1, 1);

        var scroll = Raylib.GetMouseWheelMoveV();
        ScrollDelta.X.Value = scroll.X;
        ScrollDelta.Y.Value = scroll.Y;
        ScrollDeltaClamped.X.Value = Math.Clamp(scroll.X, -1, 1);
        ScrollDeltaClamped.Y.Value = Math.Clamp(scroll.Y, -1, 1);

        foreach (var state in Buttons)
        {
            state.Set(Raylib.IsMouseButtonDown((MouseButton)state.Key));
        }
    }

    public void ReadFrom(Mouse other)
    {
        var buttons = other.Buttons.Length;
        for (var i = 0; i < buttons; i++)
        {
            Buttons[i].State = other.Buttons[i].State;
            Buttons[i].LastState = other.Buttons[i].LastState;
        }
        
        var axis = other.Axis.Length;
        for (var i = 0; i < axis; i++)
        {
            Axis[i].X.Value = other.Axis[i].X.Value;
            Axis[i].Y.Value = other.Axis[i].Y.Value;
        }
    }
}
using Raylib_cs;

namespace RedOwl;

public class Gamepad
{
    private int _id = -1;

    public int Id
    {
        get => _id;
        set
        {
            _initialized = false;
            _id = value;
        }
    }
    
    public bool Enabled { get; set; } = true;

    public TwoAxisState LeftStick { get; } = new("GamepadLeftStick");
    public TwoAxisState RightStick { get; } = new("GamepadRightStick");
    
    public ButtonState ButtonNorth { get; } = new(GamepadButton.RightFaceUp);
    public ButtonState ButtonSouth { get; } = new(GamepadButton.RightFaceDown);
    public ButtonState ButtonWest { get; } = new(GamepadButton.RightFaceLeft);
    public ButtonState ButtonEast { get; } = new(GamepadButton.RightFaceRight);
    public ButtonState LeftShoulder { get; } = new(GamepadButton.LeftTrigger1);
    public ButtonState RightShoulder { get; } = new(GamepadButton.RightTrigger1);
    public ButtonState LeftTrigger { get; } = new(GamepadButton.LeftTrigger2);
    public ButtonState RightTrigger { get; } = new(GamepadButton.RightTrigger2);
    public ButtonState DPadUp { get; } = new(GamepadButton.LeftFaceUp);
    public ButtonState DPadDown { get; } = new(GamepadButton.LeftFaceDown);
    public ButtonState DPadLeft { get; } = new(GamepadButton.LeftFaceLeft);
    public ButtonState DPadRight { get; } = new(GamepadButton.LeftFaceRight);
    public ButtonState Select { get; } = new(GamepadButton.MiddleLeft);
    public ButtonState Start { get; } = new(GamepadButton.MiddleRight);
    
    public ButtonState LeftStickPress { get; } = new(GamepadButton.LeftThumb);
    public ButtonState RightStickPress { get; } = new(GamepadButton.RightThumb);

    public TwoAxisState[] Sticks;
    public ButtonState[] Buttons;
    
    public bool AnyButton => Buttons.Any(b => b.WasPressedThisFrame);

    private bool _initialized;
    private bool _isXbox;

    public Gamepad(int id)
    {
        Id = id;
        Sticks =
        [
            LeftStick, RightStick
        ];
        Buttons =
        [
            ButtonNorth, ButtonSouth, ButtonWest, ButtonEast,
            LeftShoulder, RightShoulder,
            LeftTrigger, RightTrigger,
            LeftStickPress, RightStickPress,
            DPadUp,DPadDown,DPadLeft,DPadRight,
            Select, Start,
        ];
    }

    private void Init()
    {
        _isXbox = Raylib.GetGamepadName_(Id).Contains("Xbox");
        _initialized = true;
    }

    public void Rumble(float left, float right, float duration)
    {
        Raylib.SetGamepadVibration(Id, left, right, duration);
    }

    public void Read()
    {
        if (!Enabled) return;
        if (!Raylib.IsGamepadAvailable(Id)) return;
        if (!_initialized) Init();
        if (_isXbox)
        {
            (LeftStick.X.Value, LeftStick.Y.Value) = Input.ApplyDeadzone(
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.LeftX),
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.LeftY)
            );

            (RightStick.X.Value, RightStick.Y.Value) = Input.ApplyDeadzone(
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.RightX),
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.RightY)
            );
        }
        else
        {
            (LeftStick.X.Value, LeftStick.Y.Value) = Input.ApplyDeadzone(
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.LeftX) * -1,
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.LeftY) * -1
            );

            (RightStick.X.Value, RightStick.Y.Value) = Input.ApplyDeadzone(
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.RightX),
                Raylib.GetGamepadAxisMovement(Id, GamepadAxis.RightY)
            );
        }

        foreach (var state in Buttons)
        {
            state.Set(Raylib.IsGamepadButtonDown(Id, (GamepadButton)state.Key));
        }
    }

    public void ReadFrom(Gamepad other)
    {
        Id = other.Id;
        var sticks = other.Sticks.Length;
        for (var i = 0; i < sticks; i++)
        {
            Sticks[i].X.Value = other.Sticks[i].X.Value;
            Sticks[i].Y.Value = other.Sticks[i].Y.Value;
        }
        
        var buttons = other.Buttons.Length;
        for (var i = 0; i < buttons; i++)
        {
            Buttons[i].State = other.Buttons[i].State;
            Buttons[i].LastState = other.Buttons[i].LastState;
        }
    }
}
namespace RedOwl;

public class InputState()
{
    public InputState(int gamepad = 0)
    {
        Mouse = new Mouse();
        Keyboard = new Keyboard();
        Gamepad = new Gamepad(gamepad);
        Input._states.Add(this);
    }
    
    public bool Enabled
    {
        get {
            return Mouse.Enabled && Keyboard.Enabled & Gamepad.Enabled;
        }; 
        set {
            Mouse.Enabled = value;
            Keyboard.Enabled = value;
            Gamepad.Enabled = value;
        };
    }

    public Mouse Mouse = new();
    public Keyboard Keyboard = new();
    public Gamepad Gamepad = new(0);

    public bool AnyButton => Mouse.AnyButton || Keyboard.AnyButton || Gamepad.AnyButton;

    public void Read(float dt)
    { 
        Mouse.Read();
        Keyboard.Read();
        Gamepad.Read();
    }

    public void ReadFrom(InputState other)
    {
        Mouse.ReadFrom(Mouse.Enabled ? other.Mouse : Input.Empty.Mouse);
        Keyboard.ReadFrom(Keyboard.Enabled ? other.Keyboard : Input.Empty.Keyboard);
        Gamepad.ReadFrom(Gamepad.Enabled ? other.Gamepad : Input.Empty.Gamepad);
    }
}

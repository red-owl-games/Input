namespace RedOwl;

public class InputState()
{
    public bool Enabled { get; set; } = true;
    public Mouse Mouse = new();
    public Keyboard Keyboard = new();
    public Gamepad Gamepad = new(0);

    public bool AnyButton => Mouse.AnyButton || Keyboard.AnyButton || Gamepad.AnyButton;

    public void Read(float dt)
    {
        if (!Enabled) return;
        Mouse.Read();
        Keyboard.Read();
        Gamepad.Read();
    }

    public void ReadFrom(InputState other)
    {
        if (!Enabled) return;
        Mouse.ReadFrom(other.Mouse);
        Keyboard.ReadFrom(other.Keyboard);
        Gamepad.ReadFrom(other.Gamepad);
    }
}

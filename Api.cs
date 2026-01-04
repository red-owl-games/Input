using Raylib_cs;

namespace RedOwl;

public static partial class Input
{
    public static InputState State { get; private set; } = new();
    public static Mouse Mouse => State.Mouse;
    public static Keyboard Keyboard => State.Keyboard;
    public static Gamepad Gamepad => State.Gamepad;
    
    #region Settings
    
    public static float AxisDeadzone = 0.15f;
    public static float AxisChangeThreshold = 0.0001f;
    
    #endregion
    
    public static void Update()
    {
        State.Read();
        
        CollectGamepadEvents();
        CollectMouseEvents();
        CollectKeyboardEvents();
        CollectTextInputEvents();
    }

    private static void CollectGamepadEvents()
    {
        foreach (var state in Gamepad.Sticks)
        {
            if (state.ChangedThisFrame)
                Emit(new GamepadAxisEvent(state.Value));
        }
        
        foreach (var state in Gamepad.Buttons)
        {
            if (state.WasPressedThisFrame)
                Emit(new GamepadButtonEvent((GamepadButton)state.Key, true));

            if (state.WasReleasedThisFrame)
                Emit(new GamepadButtonEvent((GamepadButton)state.Key, false));
        }
    }

    private static void CollectMouseEvents()
    {
        var mousePosition = Mouse.Position.Value;
        var mouseDelta = Mouse.PositionDelta.Value;
        if (Mouse.Position.ChangedThisFrame) 
            Emit(new MouseEvent(mousePosition, mouseDelta));

        var wheel = Mouse.ScrollDelta.Value;
        if (Mouse.ScrollDelta.ChangedThisFrame) 
            Emit(new MouseWheelEvent(wheel));
        
        foreach (var state in Mouse.Buttons)
        {
            if (state.WasPressedThisFrame)
                Emit(new MouseButtonEvent((MouseButton)state.Key, true, mousePosition, Keyboard.Modifiers));

            if (state.WasReleasedThisFrame)
                Emit(new MouseButtonEvent((MouseButton)state.Key, false, mousePosition, Keyboard.Modifiers));
        }
    }

    private static void CollectKeyboardEvents()
    {
        foreach (var state in Keyboard.Keys)
        {
            if (state.WasPressedThisFrame)
                Emit(new KeyboardEvent((KeyboardKey)state.Key, true, Keyboard.Modifiers));
                
            if (state.WasReleasedThisFrame)
                Emit(new KeyboardEvent((KeyboardKey)state.Key, false, Keyboard.Modifiers));
        }
    }

    private static void CollectTextInputEvents()
    {
        int ch;
        while ((ch = Raylib.GetCharPressed()) != 0)
        {
            Emit(new TextInputEvent((char)ch, Keyboard.Modifiers));
        }
    }
    
    #region Helpers
    
    /// <summary>
    /// Applies a radial deadzone to two axis input. If the magnitude is less than the deadzone, returns (0, 0).
    /// </summary>
    internal static (float x, float y) ApplyDeadzone(float x, float y)
    {
        var magnitude = MathF.Sqrt(x * x + y * y);
        return magnitude < AxisDeadzone ? (0f, 0f) : (x, y);
    }
    
    #endregion
}

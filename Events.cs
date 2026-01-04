using System.Numerics;
using Raylib_cs;

namespace RedOwl;

public readonly struct GamepadAxisEvent(Vector2 Value);

public readonly struct GamepadButtonEvent(GamepadButton Button, bool IsDown);

public readonly struct MouseEvent(Vector2 Position, Vector2 Delta);
public readonly struct MouseWheelEvent(Vector2 Delta);
public readonly struct MouseButtonEvent(MouseButton Button, bool IsDown, Vector2 MousePosition, Keyboard.ModifierState Modifiers);

public readonly struct KeyboardEvent(KeyboardKey Key, bool IsDown, Keyboard.ModifierState Modifiers);

public readonly struct TextInputEvent(char Character, Keyboard.ModifierState Modifiers);



public static partial class Input
{
    public readonly struct Event
    {
        public enum Types 
        {
            GamepadAxis,
            GamepadButton,
            MouseMove,
            MouseWheel,
            MouseButton,
            Keyboard,
            TextInput
        }
    
        public Types Type { get; }
        public GamepadAxisEvent GamepadAxis { get; }
        public GamepadButtonEvent GamepadButton { get; }
        public MouseEvent Mouse { get; }
        public MouseWheelEvent MouseWheel { get; }
        public MouseButtonEvent MouseButton { get; }
        public KeyboardEvent Keyboard { get; }
        public TextInputEvent TextInput { get; }
    
        private Event(Types type,
            GamepadAxisEvent gamepadAxis = default,
            GamepadButtonEvent gamepadButton = default,
            MouseEvent mouse = default,
            MouseWheelEvent wheel = default,
            MouseButtonEvent button = default,
            KeyboardEvent key = default,
            TextInputEvent text = default)
        {
            Type = type;
            GamepadAxis = gamepadAxis;
            GamepadButton = gamepadButton;
            Mouse = mouse;
            MouseWheel = wheel;
            MouseButton = button;
            Keyboard = key;
            TextInput = text;
        }
    
        public Event(GamepadAxisEvent e) : this(Types.GamepadAxis, gamepadAxis: e) { }
        public Event(GamepadButtonEvent e) : this(Types.GamepadButton, gamepadButton: e) { }
        public Event(MouseEvent e) : this(Types.MouseMove, mouse: e) { }
        public Event(MouseWheelEvent e) : this(Types.MouseWheel, wheel: e) { }
        public Event(MouseButtonEvent e) : this(Types.MouseButton, button: e) { }
        public Event(KeyboardEvent e) : this(Types.Keyboard, key: e) { }
        public Event(TextInputEvent e) : this(Types.TextInput, text: e) { }
    }
    
    public static event Action<GamepadAxisEvent>? OnGamepadAxis;
    public static event Action<GamepadButtonEvent>? OnGamepadButton;
    public static event Action<MouseEvent>? OnMouse;
    public static event Action<MouseWheelEvent>? OnMouseWheel;
    public static event Action<MouseButtonEvent>? OnMouseButton;
    public static event Action<KeyboardEvent>? OnKeyboard;
    public static event Action<TextInputEvent>? OnTextInput;
    public static event Action<Event>? OnAny;
    
    private static void Emit(GamepadAxisEvent e)
    {
        OnGamepadAxis?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(GamepadButtonEvent e)
    {
        OnGamepadButton?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(MouseEvent e)
    {
        OnMouse?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(MouseWheelEvent e)
    {
        OnMouseWheel?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(MouseButtonEvent e)
    {
        OnMouseButton?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(KeyboardEvent e)
    {
        OnKeyboard?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
    
    private static void Emit(TextInputEvent e)
    {
        OnTextInput?.Invoke(e);
        OnAny?.Invoke(new Event(e));
    }
}
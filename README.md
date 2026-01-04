# RedOwl.Input

A robust input system for Raylib games built on .NET. Provides both state access and event-driven input handling with powerful action mapping capabilities.

<p align="center">
<img alt="RedOwl.Input" src="icon.png" width="200">
</p>

## Features

- ⚡ **High Performance** - Efficient single-poll-per-frame architecture
- 🎯 **Action Mapping** - Bind multiple input sources to a single action (e.g., WASD + Arrow Keys + Gamepad)
- 📡 **Event-Driven** - Subscribe to input events for reactive programming
- 🎛️ **State-Based** - Query input state directly for fast prototyping
- 🎨 **Composable** - Build complex input schemes with simple bindings
- 🚀 **Simple** - Designed to have a simple and straight forward API implementation

## Installation

```sh
dotnet add package RedOwl.Input
```

## Quick Start

You only need to call `Input.Update()` once per frame as this will poll for the entire input state this frame and store it
so that input state queries are fast.  This will also trigger any input events to be fired at this time if the input
control has changed its value.

```csharp
using RedOwl;
using Raylib_cs;

// Use Reactive Pattern - Will be triggered when Input.Update is called
// Will only trigger if key was pressed or released this frame
Input.OnKeyboard += (e) => 
{
    if (e.Key == KeyboardKey.Space && e.Pressed)
        Jump();
};

// Or use action mappings - library has several predefined action mappings
var move = Input.StandardMoveControl();

// Or define your own mappings
var move = new TwoAxisControl()
    .Bind(Input.Gamepad.LeftStick)
    .Bind(
        Input.Keyboard.KeyA, Input.Keyboard.KeyD,    // A/D for X axis
        Input.Keyboard.KeyW, Input.Keyboard.KeyS     // W/S for Y axis
    )
    .Bind(
        Input.Keyboard.KeyLeftArrow, Input.Keyboard.KeyRightArrow,  // Arrows X
        Input.Keyboard.KeyUpArrow,   Input.Keyboard.KeyDownArrow    // Arrows Y
    )
    .Enable();

while (!Raylib.WindowShouldClose())
{
    // Poll and store input state for this frame and emit change events
    Input.Update();
    
    // Query input state directly
    if (Input.Keyboard.KeyW.Pressed)
    {
        MoveForward();
    }

    // Query input state using bound controls
    MoveCharacter(move.Value);
    
    // ... rest of game loop
}
```

## Core Concepts

### State vs Events

RedOwl.Input provides two complementary ways to handle input:

1. **State Query** - Check input state stored this frame directly
2. **Event Handlers** - React to input changes this frame

Both can be used together if needed.

### Action Mapping

The library's powerful action mapping system lets you combine multiple input sources into unified actions. For example, a "Move" action can combine:
- WASD keyboard keys
- Arrow keys
- Gamepad left stick

All inputs are automatically normalized and combined.

### Accessing Input State

Use the convenient shortcuts:

```csharp
Input.Keyboard  // Access keyboard state
Input.Mouse     // Access mouse state
Input.Gamepad   // Access gamepad state
```

### State Properties

Most keys are a `ButtonState` with these properties:

```csharp
bool Pressed              // Key is currently down
bool Released             // Key is currently up
bool WasPressedThisFrame  // Key was pressed this frame (edge trigger)
bool WasReleasedThisFrame // Key was released this frame (edge trigger)
bool ChangedThisFrame     // Key state changed this frame
float Value               // 1.0 if pressed, 0.0 if not pressed
```

Some keys are a `TwoAxisState` with these properties:

```csharp
OneAxisState X 
OneAxisState Y 
bool ChangedThisFrame     // X or Y state changed this frame
Vector2 Value             // 0.0 -> 1.0 per axis
```

`TwoAxisState` are made up of `OneAxisState` with these properties

```csharp
bool ChangedThisFrame     // state changed this frame
float Value               // 0.0 - 1.0
```

## Keyboard Input

```csharp
// Check if a key is currently pressed
if (Input.Keyboard.KeyW.Pressed)
{
    // Moving forward
}

// Check if key was pressed this frame
if (Input.Keyboard.KeySpace.WasPressedThisFrame)
{
    // Jump!
}

// Check if key was released this frame
if (Input.Keyboard.KeyEscape.WasReleasedThisFrame)
{
    // Pause game
}

// Check modifier keys directly
if (Input.Keyboard.Control && Input.Keyboard.KeyS.Pressed)
{
    SaveGame();
}
```

## Mouse Input

```csharp
// Current mouse position
var pos = Input.Mouse.Position.Value;  // Vector2

// Mouse movement delta (this frame)
var delta = Input.Mouse.PositionDelta.Value;  // Vector2

// Clamped delta (between -1 and 1)
var clamped = Input.Mouse.PositionDeltaClamped.Value;

// Check button state
if (Input.Mouse.LeftButton.Pressed)
{
    // Left mouse button held
}

if (Input.Mouse.RightButton.WasPressedThisFrame)
{
    // Right mouse button clicked
}

// Scroll delta
var scroll = Input.Mouse.ScrollDelta.Value;  // Vector2 (X and Y scrolling)

// Clamped scroll (between -1 and 1)
var clamped = Input.Mouse.ScrollDeltaClamped.Value;

// Move mouse cursor to screen position
Input.Mouse.MoveTo(x, y);  
```

## Gamepad Input

Deadzone is automatically applied (default 0.15). Adjust via `Input.AxisDeadzone`.

```csharp
// Left stick (normalized to -1 to 1, with deadzone applied)
var leftStick = Input.Gamepad.LeftStick.Value;  // Vector2

// Right stick
var rightStick = Input.Gamepad.RightStick.Value;

// Face buttons (PlayStation: Triangle/Circle/Cross/Square, Xbox: Y/B/A/X)
if (Input.Gamepad.ButtonNorth.Pressed) { }  // Top face button
if (Input.Gamepad.ButtonSouth.Pressed) { }  // Bottom face button
if (Input.Gamepad.ButtonEast.Pressed)  { }  // Right face button
if (Input.Gamepad.ButtonWest.Pressed)  { }  // Left face button

// Shoulders and triggers
if (Input.Gamepad.LeftShoulder.Pressed)  { }
if (Input.Gamepad.RightShoulder.Pressed) { }
if (Input.Gamepad.LeftTrigger.Pressed)   { }
if (Input.Gamepad.RightTrigger.Pressed)  { }

// D-Pad
if (Input.Gamepad.DPadUp.Pressed)    { }
if (Input.Gamepad.DPadDown.Pressed)  { }
if (Input.Gamepad.DPadLeft.Pressed)  { }
if (Input.Gamepad.DPadRight.Pressed) { }

// Special buttons
if (Input.Gamepad.Select.Pressed) { }  // Back/Select button
if (Input.Gamepad.Start.Pressed)  { }  // Start/Menu button

// Stick press buttons
if (Input.Gamepad.LeftStickPress.Pressed)  { }
if (Input.Gamepad.RightStickPress.Pressed) { }

// Rumble: left motor, right motor, duration (seconds)
Input.Gamepad.Rumble(0.5f, 0.5f, 0.3f);
```

## Event System

Subscribe to input events for reactive programming:

```csharp
// Keyboard events
Input.OnKeyboard += (e) =>
{
    Console.WriteLine($"Key: {e.Key}, Down: {e.Pressed}, Modifiers: {e.Modifiers}");
};

// Mouse events
Input.OnMouse += (e) =>
{
    Console.WriteLine($"Mouse moved to {e.Position}, delta: {e.Delta}");
};

Input.OnMouseButton += (e) =>
{
    Console.WriteLine($"Mouse button {e.Button} {(e.Pressed ? "pressed" : "released")}");
};

Input.OnMouseWheel += (e) =>
{
    Console.WriteLine($"Wheel scrolled: {e.Delta}");
};

// Gamepad events
Input.OnGamepadButton += (e) =>
{
    Console.WriteLine($"Gamepad button {e.Button} {(e.Pressed ? "pressed" : "released")}");
};

Input.OnGamepadAxis += (e) =>
{
    Console.WriteLine($"Gamepad axis: {e.Value}");
};

// Text input (character typing)
Input.OnTextInput += (e) =>
{
    Console.WriteLine($"Character typed: {e.Character}");
};

// Catch-all event handler
Input.OnAny += (e) =>
{
    switch (e.Type)
    {
        case Input.Event.Types.Keyboard:
            // Handle keyboard event
            break;
        // ... other types
    }
};
```

### Event Types

- `GamepadAxisEvent` - Gamepad stick movement
- `GamepadButtonEvent` - Gamepad button press/release
- `MouseEvent` - Mouse movement
- `MouseWheelEvent` - Scroll wheel movement
- `MouseButtonEvent` - Mouse button press/release
- `KeyboardEvent` - Keyboard key press/release
- `TextInputEvent` - Character input (for text entry)

## Action Mapping

The action mapping system lets you combine multiple input sources into unified actions.

### ButtonControl

Combine multiple buttons into a single action:

```csharp
var jumpAction = new ButtonControl()
    .Bind(Input.Keyboard.KeySpace)      // Space key
    .Bind(Input.Gamepad.ButtonSouth)    // A/X button
    .Enable();

// Check the action
if (jumpAction.Pressed)
{
    Jump();
}

if (jumpAction.WasPressedThisFrame)
{
    StartJumpAnimation();
}
```

### OneAxisControl

Combine single-axis inputs:

```csharp
var zoomAction = new OneAxisControl()
    .Bind(Input.Mouse.ScrollDeltaClamped.Y)  // Mouse scroll wheel
    .Enable();

var zoomAmount = zoomAction.Value;  // -1 to 1
ZoomCamera(zoomAmount);
```

### TwoAxisControl

Combine multiple 2D inputs (keyboard + gamepad + mouse):

```csharp
var moveAction = new TwoAxisControl()
    .Bind(Input.Gamepad.LeftStick)                    // Gamepad left stick
    .Bind(
        Input.Keyboard.KeyA, Input.Keyboard.KeyD,    // A/D for X axis
        Input.Keyboard.KeyW, Input.Keyboard.KeyS     // W/S for Y axis
    )
    .Bind(
        Input.Keyboard.KeyLeftArrow, Input.Keyboard.KeyRightArrow,  // Arrows X
        Input.Keyboard.KeyUpArrow,   Input.Keyboard.KeyDownArrow    // Arrows Y
    )
    .Enable();

// Get combined input value
var moveDirection = moveAction.Value;  // Vector2, normalized to -1 to 1
MoveCharacter(moveDirection);
```

### ButtonAxis

Create an axis from two buttons:

```csharp
var horizontalAxis = new ButtonAxis(
    Input.Keyboard.KeyD,  // Positive
    Input.Keyboard.KeyA   // Negative
);

var value = horizontalAxis.Value;  // 1.0 if D pressed, -1.0 if A pressed, 0.0 if neither/both
```

### ButtonControl Repeat Behavior

`ButtonControl` supports key repeat for held buttons:

```csharp
var action = new ButtonControl()
{
    InitialRepeatDelay = 0.3f,  // Delay before first repeat (seconds)
    RepeatInterval = 0.15f,     // Time between repeats (seconds)
    FireOnPress = true          // Fire immediately on press
};

// In your update loop
var count = action.IsHeld(deltaTime);  // Returns number of times to trigger this frame
for (int i = 0; i < count; i++)
{
    ProcessAction();
}
```

## Standard Action Mappings

The library includes helper methods for common input schemes:

```csharp
var input = Input.State;

// Standard movement (WASD + Arrows + Gamepad Left Stick)
var move = input.StandardMoveControl();

// Standard look (Mouse Delta + Gamepad Right Stick)
var look = input.StandardLookControl();

// Standard zoom (Mouse Scroll Wheel)
var zoom = input.StandardZoomControl();

// Standard back/cancel (Escape + Gamepad B/Circle)
var back = input.StandardBackControl();

// Standard use/interact (Space + Gamepad A/Cross)
var use = input.StandardUseControl();

// Standard action (Left Click + F + Gamepad X/Square)
var action = input.StandardActionControl();

// Standard special (Right Click + R + Gamepad Y/Triangle)
var special = input.StandardSpecialControl();
```

## Settings

Configure global input settings:

```csharp
// Gamepad stick deadzone (default: 0.15)
Input.AxisDeadzone = 0.2f;

// Threshold for axis change detection (default: 0.0001)
Input.AxisChangeThreshold = 0.001f;
```

## Disabling Input

Disable the entire input system:

```csharp
Input.State.Enabled = false;  // Prevents all input polling
Input.Keyboard.Enabled = false; // Just Disable Keyboard
Input.Mouse.Enabled = false; // Just Disable Mouse
Input.Gamepad.Enabled = false; // Just Disable Gamepad
```

### Custom Action Mapping

```csharp
var attack = new ButtonControl()
    .Bind(Input.Mouse.LeftButton)
    .Bind(Input.Keyboard.KeyF)
    .Bind(Input.Gamepad.ButtonEast)
    .Enable();

var block = new ButtonControl()
    .Bind(Input.Mouse.RightButton)
    .Bind(Input.Gamepad.LeftShoulder)
    .Enable();

while (running)
{
    Input.Update();
    
    if (attack.Pressed && !block.Pressed)
    {
        Attack();
    }
    else if (block.Pressed)
    {
        Block();
    }
}
```

## License

See [LICENSE](LICENSE) file for details.

## Credits

Built on top of [Raylib-cs](https://github.com/ChrisDill/Raylib-cs) using [Raylib](https://www.raylib.com/).

---

Made with ❤️ by Red Owl Games

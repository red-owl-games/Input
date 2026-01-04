namespace RedOwl;

public static partial class Input
{
    public static TwoAxisControl StandardMoveControl(this InputState input)
    {
        return new TwoAxisControl()
            .Bind(input.Gamepad.LeftStick)
            .Bind(
                input.Keyboard.KeyA, input.Keyboard.KeyD,
                input.Keyboard.KeyW, input.Keyboard.KeyS
            )
            .Bind(
                input.Keyboard.KeyRightArrow, input.Keyboard.KeyLeftArrow,
                input.Keyboard.KeyUpArrow, input.Keyboard.KeyDownArrow
            )
            .Enable();
    }

    public static TwoAxisControl StandardLookControl(this InputState input)
    {
        return new TwoAxisControl()
            .Bind(input.Gamepad.RightStick)
            .Bind(input.Mouse.PositionDeltaClamped.X, input.Mouse.PositionDeltaClamped.Y)
            .Enable();
    }

    public static OneAxisControl StandardZoomControl(this InputState input)
    {
        return new OneAxisControl()
            //.Bind(input.Gamepad.RightStick) // TODO: need "combo" binds, RightStick pressed + RightStick Axis Value
            .Bind(input.Mouse.ScrollDeltaClamped.Y)
            .Enable();
    }
    
    public static ButtonControl StandardBackControl(this InputState input)
    {
        return new ButtonControl()
            .Bind(input.Keyboard.KeyEscape)
            .Bind(input.Gamepad.ButtonSouth)
            .Enable();
    }

    public static ButtonControl StandardUseControl(this InputState input)
    {
        return new ButtonControl()
            .Bind(input.Keyboard.KeySpace)
            .Bind(input.Gamepad.ButtonSouth)
            .Enable();
    }
    
    public static ButtonControl StandardActionControl(this InputState input)
    {
        return new ButtonControl()
            .Bind(input.Mouse.LeftButton)
            .Bind(input.Keyboard.KeyF)
            .Bind(input.Gamepad.ButtonEast)
            .Enable();
    }
    
    public static ButtonControl StandardSpecialControl(this InputState input)
    {
        return new ButtonControl()
            .Bind(input.Mouse.RightButton)
            .Bind(input.Keyboard.KeyR)
            .Bind(input.Gamepad.ButtonWest)
            .Enable();
    }
}
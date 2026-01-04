using Raylib_cs;

namespace RedOwl;


public class Keyboard
{
	public readonly record struct ModifierState(bool Control, bool Alt, bool Shift, bool Super);
	
	public bool Enabled { get; set; } = true;
    public ButtonState Key0 { get; } = new (KeyboardKey.Zero);
	public ButtonState Key1 { get; } = new (KeyboardKey.One);
	public ButtonState Key2 { get; } = new (KeyboardKey.Two);
	public ButtonState Key3 { get; } = new (KeyboardKey.Three);
	public ButtonState Key4 { get; } = new (KeyboardKey.Four);
	public ButtonState Key5 { get; } = new (KeyboardKey.Five);
	public ButtonState Key6 { get; } = new (KeyboardKey.Six);
	public ButtonState Key7 { get; } = new (KeyboardKey.Seven);
	public ButtonState Key8 { get; } = new (KeyboardKey.Eight);
	public ButtonState Key9 { get; } = new (KeyboardKey.Nine);
	
	public ButtonState KeyNum0 { get; } = new (KeyboardKey.Kp0);
	public ButtonState KeyNum1 { get; } = new (KeyboardKey.Kp1);
	public ButtonState KeyNum2 { get; } = new (KeyboardKey.Kp2);
	public ButtonState KeyNum3 { get; } = new (KeyboardKey.Kp3);
	public ButtonState KeyNum4 { get; } = new (KeyboardKey.Kp4);
	public ButtonState KeyNum5 { get; } = new (KeyboardKey.Kp5);
	public ButtonState KeyNum6 { get; } = new (KeyboardKey.Kp6);
	public ButtonState KeyNum7 { get; } = new (KeyboardKey.Kp7);
	public ButtonState KeyNum8 { get; } = new (KeyboardKey.Kp8);
	public ButtonState KeyNum9 { get; } = new (KeyboardKey.Kp9);

	public ButtonState KeyA { get; } = new (KeyboardKey.A);
	public ButtonState KeyB { get; } = new (KeyboardKey.B);
	public ButtonState KeyC { get; } = new (KeyboardKey.C);
	public ButtonState KeyD { get; } = new (KeyboardKey.D);
	public ButtonState KeyE { get; } = new (KeyboardKey.E);
	public ButtonState KeyF { get; } = new (KeyboardKey.F);
	public ButtonState KeyG { get; } = new (KeyboardKey.G);
	public ButtonState KeyH { get; } = new (KeyboardKey.H);
	public ButtonState KeyI { get; } = new (KeyboardKey.I);
	public ButtonState KeyJ { get; } = new (KeyboardKey.J);
	public ButtonState KeyK { get; } = new (KeyboardKey.K);
	public ButtonState KeyL { get; } = new (KeyboardKey.L);
	public ButtonState KeyM { get; } = new (KeyboardKey.M);
	public ButtonState KeyN { get; } = new (KeyboardKey.N);
	public ButtonState KeyO { get; } = new (KeyboardKey.O);
	public ButtonState KeyP { get; } = new (KeyboardKey.P);
	public ButtonState KeyQ { get; } = new (KeyboardKey.Q);
	public ButtonState KeyR { get; } = new (KeyboardKey.R);
	public ButtonState KeyS { get; } = new (KeyboardKey.S);
	public ButtonState KeyT { get; } = new (KeyboardKey.T);
	public ButtonState KeyU { get; } = new (KeyboardKey.U);
	public ButtonState KeyV { get; } = new (KeyboardKey.V);
	public ButtonState KeyW { get; } = new (KeyboardKey.W);
	public ButtonState KeyX { get; } = new (KeyboardKey.X);
	public ButtonState KeyY { get; } = new (KeyboardKey.Y);
	public ButtonState KeyZ { get; } = new (KeyboardKey.Z);

	public ButtonState KeyF1  { get; } = new (KeyboardKey.F1);
	public ButtonState KeyF2  { get; } = new (KeyboardKey.F2);
	public ButtonState KeyF3  { get; } = new (KeyboardKey.F3);
	public ButtonState KeyF4  { get; } = new (KeyboardKey.F4);
	public ButtonState KeyF5  { get; } = new (KeyboardKey.F5);
	public ButtonState KeyF6  { get; } = new (KeyboardKey.F6);
	public ButtonState KeyF7  { get; } = new (KeyboardKey.F7);
	public ButtonState KeyF8  { get; } = new (KeyboardKey.F8);
	public ButtonState KeyF9  { get; } = new (KeyboardKey.F9);
	public ButtonState KeyF10 { get; } = new (KeyboardKey.F10);
	public ButtonState KeyF11 { get; } = new (KeyboardKey.F11);
	public ButtonState KeyF12 { get; } = new (KeyboardKey.F12);

	public ButtonState KeySpace        { get; } = new (KeyboardKey.Space);
	public ButtonState KeyApostrophe   { get; } = new (KeyboardKey.Apostrophe);
	public ButtonState KeyComma        { get; } = new (KeyboardKey.Comma);
	public ButtonState KeyMinus        { get; } = new (KeyboardKey.Minus);
	public ButtonState KeyPeriod       { get; } = new (KeyboardKey.Period);
	public ButtonState KeySlash        { get; } = new (KeyboardKey.Slash);
	public ButtonState KeySemicolon    { get; } = new (KeyboardKey.Semicolon);
	public ButtonState KeyEqual        { get; } = new (KeyboardKey.Equal);
	public ButtonState KeyLeftBracket  { get; } = new (KeyboardKey.LeftBracket);
	public ButtonState KeyBackslash    { get; } = new (KeyboardKey.Backslash);
	public ButtonState KeyRightBracket { get; } = new (KeyboardKey.RightBracket);
	public ButtonState KeyTilde        { get; } = new (KeyboardKey.Grave);
	public ButtonState KeyEscape       { get; } = new (KeyboardKey.Escape);
	public ButtonState KeyEnter        { get; } = new (KeyboardKey.Enter);
	public ButtonState KeyTab          { get; } = new (KeyboardKey.Tab);
	public ButtonState KeyBackspace    { get; } = new (KeyboardKey.Backspace);
	public ButtonState KeyInsert       { get; } = new (KeyboardKey.Insert);
	public ButtonState KeyDelete       { get; } = new (KeyboardKey.Delete);
	public ButtonState KeyPageUp       { get; } = new (KeyboardKey.PageUp);
	public ButtonState KeyPageDown     { get; } = new (KeyboardKey.PageDown);
	public ButtonState KeyHome         { get; } = new (KeyboardKey.Home);
	public ButtonState KeyEnd          { get; } = new (KeyboardKey.End);
	public ButtonState KeyCapsLock     { get; } = new (KeyboardKey.CapsLock);
	public ButtonState KeyScrollLock   { get; } = new (KeyboardKey.ScrollLock);
	public ButtonState KeyNumLock      { get; } = new (KeyboardKey.NumLock);
	public ButtonState KeyPrintScreen  { get; } = new (KeyboardKey.PrintScreen);
	public ButtonState KeyPause        { get; } = new (KeyboardKey.Pause);

	public ButtonState KeyUpArrow    { get; } = new (KeyboardKey.Up);
	public ButtonState KeyDownArrow  { get; } = new (KeyboardKey.Down);
	public ButtonState KeyLeftArrow  { get; } = new (KeyboardKey.Left);
	public ButtonState KeyRightArrow { get; } = new (KeyboardKey.Right);

	public ButtonState KeyLeftShift    { get; } = new (KeyboardKey.LeftShift);
	public ButtonState KeyRightShift   { get; } = new (KeyboardKey.RightShift);
	public ButtonState KeyLeftControl  { get; } = new (KeyboardKey.LeftControl);
	public ButtonState KeyRightControl { get; } = new (KeyboardKey.RightControl);
	public ButtonState KeyLeftAlt      { get; } = new (KeyboardKey.LeftAlt);
	public ButtonState KeyRightAlt     { get; } = new (KeyboardKey.RightAlt);
	public ButtonState KeyLeftSuper      { get; } = new (KeyboardKey.LeftSuper);
	public ButtonState KeyRightSuper     { get; } = new (KeyboardKey.RightSuper);

	public ButtonState[] Keys;
	
	public bool AnyButton => Keys.Any(b => b.WasPressedThisFrame);

	public Keyboard()
	{
		Keys =
		[
			Key0, Key1, Key2, Key3, Key4, Key5, Key6, Key7, Key8, Key9,
			KeyNum0, KeyNum1, KeyNum2, KeyNum3, KeyNum4, KeyNum5, KeyNum6, KeyNum7, KeyNum8, KeyNum9,
			
			KeyA, KeyB, KeyC, KeyD, KeyE, KeyF, KeyG, KeyH, KeyI, KeyJ,
			KeyK, KeyL, KeyM, KeyN, KeyO, KeyP, KeyQ, KeyR, KeyS, KeyT,
			KeyU, KeyV, KeyW, KeyX, KeyY, KeyZ,

			KeyF1, KeyF2, KeyF3, KeyF4, KeyF5, KeyF6, KeyF7, KeyF8, KeyF9, KeyF10, KeyF11, KeyF12,

			KeySpace, KeyApostrophe, KeyComma, KeyMinus, KeyPeriod, KeySlash, KeySemicolon, KeyEqual,
			KeyLeftBracket, KeyBackslash, KeyRightBracket, KeyTilde, KeyEscape, KeyEnter, KeyTab, KeyBackspace,
			KeyInsert, KeyDelete, KeyPageUp, KeyPageDown, KeyHome, KeyEnd, KeyCapsLock, KeyScrollLock,
			KeyNumLock, KeyPrintScreen, KeyPause,

			KeyUpArrow, KeyDownArrow, KeyLeftArrow, KeyRightArrow,

			KeyLeftShift, KeyRightShift, KeyLeftControl, KeyRightControl, KeyLeftAlt, KeyRightAlt, KeyLeftSuper, KeyRightSuper,
		];
	}
	
	public bool Alt => KeyLeftAlt.Pressed || KeyRightAlt.Pressed;
	public bool Shift => KeyLeftShift.Pressed || KeyRightShift.Pressed;
	public bool Control => KeyLeftControl.Pressed || KeyRightControl.Pressed;
	public bool Super => KeyLeftSuper.Pressed || KeyRightSuper.Pressed;
	public ModifierState Modifiers { get; private set; }
	
	public void Read()
	{
		if (!Enabled) return;
		
		foreach (var state in Keys)
		{
			state.Set(Raylib.IsKeyDown((KeyboardKey)state.Key));
		}
		Modifiers = new ModifierState(Control, Alt, Shift, Super);
	}

	public void ReadFrom(Keyboard other)
	{
		var source = other.Keys;
		var dest = Keys;
		var count = source.Length;
		for (var i = 0; i < count; i++)
		{
			dest[i].State = source[i].State;
			dest[i].LastState = source[i].LastState;
		}
	}
}
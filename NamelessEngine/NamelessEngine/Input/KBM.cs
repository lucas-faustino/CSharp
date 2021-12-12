using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NamelessEngine.Input
{
    public enum ScrollWheelState : sbyte
    {
        None = 0,
        Up = 1,
        Down = -1
    }

    public sealed class KBM
    {
        KeyboardState _previousKeyboardState;
        KeyboardState _currentKeyboardState;
        MouseState _previousMouseState;
        MouseState _currentMouseState;
        int _previousScrollValue;

        public Point PreviousMousePoint => new Point(_previousMouseState.X, _previousMouseState.Y);

        public Point MousePoint
        {
            get
            {
                var state = Mouse.GetState();
                return new Point(state.X, state.Y);
            }
        }

        public ScrollWheelState ScrollWheel { get; private set; }

        internal void Update()
        {
            // update keyboard
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();
            // update mouse
            _previousMouseState = _currentMouseState;
            _currentMouseState = Mouse.GetState();
            // update scroll wheel
            if (_currentMouseState.ScrollWheelValue < _previousScrollValue)
                ScrollWheel = ScrollWheelState.Down;

            else if (_currentMouseState.ScrollWheelValue > _previousScrollValue)
                ScrollWheel = ScrollWheelState.Up;

            else
                ScrollWheel = ScrollWheelState.None;

            _previousScrollValue = _currentMouseState.ScrollWheelValue;
        }

        public bool IsHoldingUp => _currentKeyboardState.IsKeyDown(Keys.W);

        public bool IsHoldingDown => _currentKeyboardState.IsKeyDown(Keys.S);

        public bool IsHoldingRight => _currentKeyboardState.IsKeyDown(Keys.D);

        public bool IsHoldingLeft => _currentKeyboardState.IsKeyDown(Keys.A);

        public bool WasLeftMouseClick => _previousMouseState.LeftButton != ButtonState.Pressed && _currentMouseState.LeftButton == ButtonState.Pressed;

        public bool WasRightMouseClick => _previousMouseState.RightButton != ButtonState.Pressed && _currentMouseState.RightButton == ButtonState.Pressed;

        public bool WasKeyReleased(Keys key) => !_previousKeyboardState.IsKeyUp(key) && _currentKeyboardState.IsKeyUp(key);

        public bool WasKeyPressed(Keys key) => !_previousKeyboardState.IsKeyDown(key) && _currentKeyboardState.IsKeyDown(key);

        public bool IsKeyHeldDown(Keys key) => _currentKeyboardState.IsKeyDown(key);

        public bool IsKeyLetter(Keys key) => (int)key >= (int)Keys.A && (int)key <= (int)Keys.Z;
        // if the states are different it means a key was pressed this frame
        public bool IsAnyKey => _previousKeyboardState != _currentKeyboardState;

        public int GetScrollWheelValue => _currentMouseState.ScrollWheelValue;
       
        public bool IsUsingMouse
        {
            get
            {
                return PreviousMousePoint != MousePoint ||
                _currentMouseState.LeftButton == ButtonState.Pressed ||
                _currentMouseState.RightButton == ButtonState.Pressed ||
                ScrollWheel != ScrollWheelState.None;
            }
        }
    }
}

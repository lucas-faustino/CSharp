using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace NamelessEngine.Input
{
    public sealed class Controller
    {
        sealed class GamePadStates
        {
            public GamePadState[] Previous;
            public GamePadState[] Current;
        }

        const float MARGIN = 0.1f;

        public readonly int MaxControllers;

        readonly Buttons[] _buttonList;
        readonly GamePadStates _states;

        public Controller(int maxControllers)
        {
            MaxControllers = maxControllers;
            _buttonList = Helpers.GetEnumValues<Buttons>();
            _states = new GamePadStates
            {
                Previous = new GamePadState[MaxControllers],
                Current = new GamePadState[MaxControllers]
            };
        }

        internal void Update()
        {
            for (int i = 0; i < MaxControllers; i++)
            {
                _states.Previous[i] = _states.Current[i];
                _states.Current[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        public bool IsConnected(int index) => GamePad.GetState((PlayerIndex)index).IsConnected;

        public bool IsMovingUp(int playerIndex)
        {
            return _states.Current[playerIndex].ThumbSticks.Left.Y > MARGIN ||
                _states.Current[playerIndex].DPad.Up == ButtonState.Pressed;
        }

        public bool IsMovingDown(int playerIndex)
        {
            return _states.Current[playerIndex].ThumbSticks.Left.Y < -MARGIN ||
                _states.Current[playerIndex].DPad.Down == ButtonState.Pressed;
        }

        public bool IsMovingRight(int playerIndex)
        {
            return _states.Current[playerIndex].ThumbSticks.Left.X > MARGIN ||
                _states.Current[playerIndex].DPad.Right == ButtonState.Pressed;
        }

        public bool IsMovingLeft(int playerIndex)
        {
            return _states.Current[playerIndex].ThumbSticks.Left.X < -MARGIN ||
                _states.Current[playerIndex].DPad.Left == ButtonState.Pressed;
        }

        public bool IsPressingUp(int playerIndex)
        {
            return (_states.Previous[playerIndex].DPad.Up != ButtonState.Pressed && _states.Current[playerIndex].DPad.Up == ButtonState.Pressed) ||
                (_states.Previous[playerIndex].ThumbSticks.Left.Y < MARGIN && _states.Current[playerIndex].ThumbSticks.Left.Y > MARGIN);
        }

        public bool IsPressingDown(int playerIndex)
        {
            return (_states.Previous[playerIndex].DPad.Down != ButtonState.Pressed && _states.Current[playerIndex].DPad.Down == ButtonState.Pressed) ||
                (_states.Previous[playerIndex].ThumbSticks.Left.Y > -MARGIN && _states.Current[playerIndex].ThumbSticks.Left.Y < -MARGIN); ;
        }

        public bool IsPressingRight(int playerIndex)
        {
            return (_states.Previous[playerIndex].DPad.Right != ButtonState.Pressed && _states.Current[playerIndex].DPad.Right == ButtonState.Pressed) ||
                (_states.Previous[playerIndex].ThumbSticks.Left.X < MARGIN && _states.Current[playerIndex].ThumbSticks.Left.X > MARGIN);
        }

        public bool IsPressingLeft(int playerIndex)
        {
            return (_states.Previous[playerIndex].DPad.Left != ButtonState.Pressed && _states.Current[playerIndex].DPad.Left == ButtonState.Pressed) ||
                (_states.Previous[playerIndex].ThumbSticks.Left.X > -MARGIN && _states.Current[playerIndex].ThumbSticks.Left.X < -MARGIN);
        }

        public bool IsPressing(int playerIndex, Buttons button)
        {
            return _states.Previous[playerIndex].IsButtonDown(button) == false && _states.Current[playerIndex].IsButtonDown(button) == true;
        }

        public bool IsHolding(int playerIndex, Buttons button)
        {
            return _states.Current[playerIndex].IsButtonDown(button) == true;
        }

        public bool IsAnyButton(int playerIndex)
        {
            var count = _buttonList.Length;
            for (int i = 0; i < count; i++)
            {
                if (_states.Current[playerIndex].IsButtonDown(_buttonList[i]))
                    return true;
            }

            return false;
        }
    }
}

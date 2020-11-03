using UserTCQ.Engine.Rendering;
using UserTCQ.Engine.Types;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace UserTCQ.Engine.Managers
{
    public enum Axis
    {
        Horizontal = 0,
        Vertical = 1
    }

    public class InputManager : Behaviour
    {
        private static KeyboardState keyState;
        private static MouseState mouseState;

        protected override void Start()
        {
            base.Start();
        }

        protected override void EarlyUpdate()
        {
            base.EarlyUpdate();
            keyState = MainWindow.instance.KeyboardState;
            mouseState = MainWindow.instance.MouseState;
        }
        public static bool GetKeyDown(Keys key)
        {
            if (!MainWindow.instance.IsFocused)
                return false;
            if (!keyState.WasKeyDown(key) && keyState.IsKeyDown(key))
                return true;
            return false;
        }
        public static bool GetKey(Keys key)
        {
            if (!MainWindow.instance.IsFocused)
                return false;
            if (keyState.IsKeyDown(key))
                return true;
            return false;
        }
        public static float GetAxis(Axis axis)
        {
            switch (axis)
            {
                case Axis.Horizontal:
                    if (GetKey(Keys.D) || GetKey(Keys.Right))
                        return 1;
                    else if (GetKey(Keys.A) || GetKey(Keys.Left))
                        return -1;
                    break;
                case Axis.Vertical:
                    if (GetKey(Keys.W) || GetKey(Keys.Up))
                        return 1;
                    else if (GetKey(Keys.S) || GetKey(Keys.Down))
                        return -1;
                    break;
            }
            return 0;
        }
        public static bool GetKeyUp(Keys key)
        {
            if (!MainWindow.instance.IsFocused)
                return false;
            if (!keyState.IsKeyDown(key) && keyState.WasKeyDown(key))
                return true;
            return false;
        }
    }
}

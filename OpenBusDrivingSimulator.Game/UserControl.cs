using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Engine;
using OpenBusDrivingSimulator.Engine.Controls;

namespace OpenBusDrivingSimulator.Game
{
    public enum ControlCommand
    {
        INVALID,
        CAMERA_MOVE_LEFT,
        CAMERA_MOVE_RIGHT,
        CAMERA_MOVE_FRONT,
        CAMERA_MOVE_BACK,
        CAMERA_MOVE_UP,
        CAMERA_MOVE_DOWN,
        CAMERA_ROTATE_Y_LEFT,
        CAMERA_ROTATE_Y_RIGHT,
        CAMERA_ROTATE_X_UP,
        CAMERA_ROTATE_X_DOWN,
        TOGGLE_FPS
    }

    public class UserControl
    {
        public ControlCommand Command;
        public Control ConfiguredControl;

        public UserControl()
        {
            this.Command = ControlCommand.INVALID;
            this.ConfiguredControl = new Control();
        }

        public UserControl(ControlCommand command, Control configuredControl)
        {
            this.Command = command;
            this.ConfiguredControl = configuredControl;
        }
    }

    public static class ControlHandler
    {
        private static UserControl[] userControls;

        public static void LoadControls()
        {
            // TODO: load from a config file
            userControls = new UserControl[]
            {
                new UserControl(ControlCommand.CAMERA_MOVE_LEFT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_A)),
                new UserControl(ControlCommand.CAMERA_MOVE_RIGHT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_D)),
                new UserControl(ControlCommand.CAMERA_MOVE_FRONT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_W)),
                new UserControl(ControlCommand.CAMERA_MOVE_BACK, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_S)),
                new UserControl(ControlCommand.CAMERA_MOVE_UP, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_Q)),
                new UserControl(ControlCommand.CAMERA_MOVE_DOWN, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_E)),
                new UserControl(ControlCommand.CAMERA_ROTATE_Y_LEFT,
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_LEFT)),
                new UserControl(ControlCommand.CAMERA_ROTATE_Y_RIGHT,
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_RIGHT)),
                new UserControl(ControlCommand.CAMERA_ROTATE_X_DOWN,
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_DOWN)),
                new UserControl(ControlCommand.CAMERA_ROTATE_X_UP,
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCode.KEY_UP)),
                new UserControl(ControlCommand.TOGGLE_FPS, 
                    new Control(ControlType.DISCRETE, ControlSource.KEYBOARD, KeyCode.KEY_F))
            };
        }

        public static void ProcessControls()
        {
            foreach (Control control in Controller.ControlSequence)
            {
                UserControl userControl = FindUserControlByControl(control);
                Control configuredControl = userControl.ConfiguredControl;
                control.Type = configuredControl.Type;

                if (control.Type == ControlType.DISCRETE)
                {
                    if (control.DiscreteState == ControlState.PRESSED)
                        control.DiscreteState = ControlState.ALREADY_PRESSED;
                    else
                        continue;
                }

                switch (userControl.Command)
                {
                    case ControlCommand.INVALID:
                        break;
                    case ControlCommand.CAMERA_MOVE_LEFT:
                        Camera.MoveBy(-0.5f, 0, 0);
                        break;
                    case ControlCommand.CAMERA_MOVE_RIGHT:
                        Camera.MoveBy(0.5f, 0, 0);
                        break;
                    case ControlCommand.CAMERA_MOVE_FRONT:
                        Camera.MoveBy(0, 0, 0.5f);
                        break;
                    case ControlCommand.CAMERA_MOVE_BACK:
                        Camera.MoveBy(0, 0, -0.5f);
                        break;
                    case ControlCommand.CAMERA_MOVE_UP:
                        Camera.MoveBy(0, 0.5f, 0);
                        break;
                    case ControlCommand.CAMERA_MOVE_DOWN:
                        Camera.MoveBy(0, -0.5f, 0);
                        break;
                    case ControlCommand.CAMERA_ROTATE_Y_LEFT:
                        Camera.RotateYBy(0.5f);
                        break;
                    case ControlCommand.CAMERA_ROTATE_Y_RIGHT:
                        Camera.RotateYBy(-0.5f);
                        break;
                    case ControlCommand.CAMERA_ROTATE_X_DOWN:
                        break;
                    case ControlCommand.CAMERA_ROTATE_X_UP:
                        break;
                    case ControlCommand.TOGGLE_FPS:
                        Game.ShowFrameRate = !Game.ShowFrameRate;
                        break;
                }
            }
        }

        private static UserControl FindUserControlByControl(Control control)
        {
            for (int i = 0; i < userControls.Length; i++)
                if (userControls[i].ConfiguredControl.Equals(control))
                    return userControls[i];

            return new UserControl();
        }
    }
}

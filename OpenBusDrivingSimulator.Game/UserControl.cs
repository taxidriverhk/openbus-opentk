using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
{
    public enum ControlCommand
    {
        INVALID,
        CAMERA_MOVE_LEFT,
        CAMERA_MOVE_RIGHT,
        CAMERA_MOVE_FRONT,
        CAMERA_MOVE_BACK
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
            userControls = new UserControl[]
            {
                new UserControl(ControlCommand.CAMERA_MOVE_LEFT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCodeConvert.GetKeyCode("LEFT"))),
                new UserControl(ControlCommand.CAMERA_MOVE_RIGHT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCodeConvert.GetKeyCode("RIGHT"))),
                new UserControl(ControlCommand.CAMERA_MOVE_FRONT, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCodeConvert.GetKeyCode("UP"))),
                new UserControl(ControlCommand.CAMERA_MOVE_BACK, 
                    new Control(ControlType.CONTINUOUS, ControlSource.KEYBOARD, KeyCodeConvert.GetKeyCode("DOWN")))
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
                    if (control.State == ControlState.PRESSED)
                        control.State = ControlState.ALREADY_PRESSED;
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
                        Camera.MoveBy(0, 0, -0.5f);
                        break;
                    case ControlCommand.CAMERA_MOVE_BACK:
                        Camera.MoveBy(0, 0, 0.5f);
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

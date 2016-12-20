﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Engine;
using OpenBus.Engine.Controls;

namespace OpenBus.Game.Controls
{
    public enum ControlCommand
    {
        Invalid,
        CameraMoveLeft,
        CameraMoveRight,
        CameraMoveFront,
        CameraMoveBack,
        CameraMoveUp,
        CameraMoveDown,
        CameraRotateYawLeft,
        CameraRotateYawRight,
        CameraRotatePitchUp,
        CameraRotatePitchDown,
        CameraZoomIn,
        CameraZoomOut,
        ToggleFPSDisplay
    }

    public class UserControl
    {
        public ControlCommand Command;
        public Control ConfiguredControl;

        public UserControl()
        {
            this.Command = ControlCommand.Invalid;
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
                new UserControl(ControlCommand.CameraMoveLeft, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyA)),
                new UserControl(ControlCommand.CameraMoveRight, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyD)),
                new UserControl(ControlCommand.CameraMoveFront, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyW)),
                new UserControl(ControlCommand.CameraMoveBack, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyS)),
                new UserControl(ControlCommand.CameraMoveUp, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyQ)),
                new UserControl(ControlCommand.CameraMoveDown, 
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyE)),
                new UserControl(ControlCommand.CameraRotateYawLeft,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyLeft)),
                new UserControl(ControlCommand.CameraRotateYawRight,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyRight)),
                new UserControl(ControlCommand.CameraRotatePitchDown,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyDown)),
                new UserControl(ControlCommand.CameraRotatePitchUp,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyUp)),
                new UserControl(ControlCommand.CameraZoomIn,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyNumpadPlus)),
                new UserControl(ControlCommand.CameraZoomOut,
                    new Control(ControlType.Continuous, ControlSource.Keyboard, KeyCode.KeyNumpadMinus)),
                new UserControl(ControlCommand.ToggleFPSDisplay, 
                    new Control(ControlType.Discrete, ControlSource.Keyboard, KeyCode.KeyF))
            };
        }

        public static void ProcessControls()
        {
            foreach (Control control in Controller.ControlSequence)
            {
                UserControl userControl = FindUserControlByControl(control);
                Control configuredControl = userControl.ConfiguredControl;
                control.Type = configuredControl.Type;

                if (control.Type == ControlType.Discrete)
                {
                    if (control.DiscreteState == ControlState.Pressed)
                        control.DiscreteState = ControlState.AlreadyPressed;
                    else
                        continue;
                }

                switch (userControl.Command)
                {
                    case ControlCommand.Invalid:
                        break;
                    case ControlCommand.CameraMoveLeft:
                        Game.CurrentView.MoveBy(-0.5f, 0, 0);
                        break;
                    case ControlCommand.CameraMoveRight:
                        Game.CurrentView.MoveBy(0.5f, 0, 0);
                        break;
                    case ControlCommand.CameraMoveFront:
                        Game.CurrentView.MoveBy(0, 0, 0.5f);
                        break;
                    case ControlCommand.CameraMoveBack:
                        Game.CurrentView.MoveBy(0, 0, -0.5f);
                        break;
                    case ControlCommand.CameraMoveUp:
                        Game.CurrentView.MoveBy(0, 0.5f, 0);
                        break;
                    case ControlCommand.CameraMoveDown:
                        Game.CurrentView.MoveBy(0, -0.5f, 0);
                        break;
                    case ControlCommand.CameraRotateYawLeft:
                        Game.CurrentView.ChangeYawAngleBy(1.0f);
                        break;
                    case ControlCommand.CameraRotateYawRight:
                        Game.CurrentView.ChangeYawAngleBy(-1.0f);
                        break;
                    case ControlCommand.CameraRotatePitchDown:
                        break;
                    case ControlCommand.CameraRotatePitchUp:
                        break;
                    case ControlCommand.CameraZoomIn:
                        Game.CurrentView.ZoomBy(0.1f);
                        break;
                    case ControlCommand.CameraZoomOut:
                        Game.CurrentView.ZoomBy(-0.1f);
                        break;
                    case ControlCommand.ToggleFPSDisplay:
                        Game.Settings.ScreenDisplaySettings.ToggleFrameRateDisplay();
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

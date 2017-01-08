using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBus.Engine.Controls
{
    /// <summary>
    /// The type of control made by the user.
    /// </summary>
    public enum ControlType
    {
        /// <summary>
        /// The control will be effective until the user releases the control 
        /// (ex. releases the key).
        /// </summary>
        Continuous,
        /// <summary>
        /// The control will not be effective once the command associated to it has been done.
        /// </summary>
        Discrete
    }

    /// <summary>
    /// The source of the control.
    /// </summary>
    public enum ControlSource
    {
        /// <summary>
        /// To be used as an default value, or when the source cannot be detected.
        /// </summary>
        Invalid,
        /// <summary>
        /// The control comes from a keyboard (ex. a key press).
        /// </summary>
        Keyboard,
        /// <summary>
        /// The control comes from a mouse (ex. a mouse click, a mouse scroll, etc.).
        /// </summary>
        Mouse
    }

    /// <summary>
    /// The state of the control
    /// </summary>
    public enum ControlState
    {
        /// <summary>
        /// The user has already pressed the key or button.
        /// </summary>
        Pressed,
        /// <summary>
        /// The key has already been pressed and its command has already been handled.
        /// This is usually used for a discrete control so the command won't be invoked again.
        /// </summary>
        PressHandled,
        /// <summary>
        /// The user has already released the key or button.
        /// </summary>
        Released,
        /// <summary>
        /// The key has already been released and its command has already been handled.
        /// </summary>
        ReleaseHandled
    }

    /// <summary>
    /// The SDL numeric key code that represents a key
    /// </summary>
    public enum KeyCode
    {
        /// <summary>
        /// An invalid key code, this is to be used as the default value
        /// </summary>
        Invalid = -1,
        /// <summary>
        /// "A"
        /// </summary>
        KeyA = 4,
        /// <summary>
        /// "B"
        /// </summary>
        KeyB = 5,
        /// <summary>
        /// "C"
        /// </summary>
        KeyC = 6,
        /// <summary>
        /// "D"
        /// </summary>
        KeyD = 7,
        /// <summary>
        /// "E"
        /// </summary>
        KeyE = 8,
        KeyF = 9,
        KeyG = 10,
        KeyH = 11,
        KeyI = 12,
        KeyJ = 13,
        KeyK = 14,
        KeyL = 15,
        KeyM = 16,
        KeyN = 17,
        KeyO = 18,
        KeyP = 19,
        KeyQ = 20,
        KeyR = 21,
        KeyS = 22,
        KeyT = 23,
        KeyU = 24,
        KeyV = 25,
        KeyW = 26,
        KeyX = 27,
        KeyY = 28,
        KeyZ = 29,
        KeyRight = 79,
        KeyLeft = 80,
        KeyUp = 81,
        KeyDown = 82,
        KeyNumpadMinus = 86,
        KeyNumpadPlus = 87
    }

    /// <summary>
    /// Used to determine whether Ctrl/Alt/Shift (regardless of the left one or the right one) 
    /// is/are pressed or required for a control.
    /// </summary>
    [Flags]
    public enum KeyModifier
    {
        /// <summary>
        /// None of the Ctrl, Alt or Shift keys are pressed or required.
        /// </summary>
        None = 0,
        /// <summary>
        /// Ctrl key is pressed or required.
        /// </summary>
        Ctrl = 1,
        /// <summary>
        /// Alt key is pressed or required.
        /// </summary>
        Alt = 2,
        /// <summary>
        /// Shift key is pressed or required.
        /// </summary>
        Shift = 4
    }

    public class Control
    {
        public ControlSource Source;
        public KeyCode KeyCode;
        public KeyModifier KeyModifierMask;

        public ControlType Type;
        public ControlState DiscreteState;
        public double ContinuousState;
        public double RepeatInterval;

        public Control()
        {
            // This creates an invalid default control
            this.Source = ControlSource.Invalid;
            this.KeyCode = KeyCode.Invalid;
            this.KeyModifierMask = KeyModifier.None;
            this.Type = ControlType.Discrete;
            this.DiscreteState = ControlState.Released;
            this.ContinuousState = 0.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlSource source, KeyCode keyCode)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            // Default attributes, will be changed when this is being used
            this.KeyModifierMask = KeyModifier.None;
            this.Type = ControlType.Discrete;
            this.DiscreteState = ControlState.Pressed;
            this.ContinuousState = 1.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlType type, ControlSource source, KeyCode keyCode)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            this.KeyModifierMask = KeyModifier.None;
            this.Type = type;
            this.DiscreteState = ControlState.Pressed;
            this.ContinuousState = 1.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlType type, ControlSource source, KeyCode keyCode, KeyModifier modifierMask)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            this.KeyModifierMask = modifierMask;
            this.Type = type;
            this.DiscreteState = ControlState.Pressed;
            this.ContinuousState = 1.0;
            this.RepeatInterval = 0.0;
        }

        public override bool Equals(object obj)
        {
            Control other = (Control)obj;
            if (other == null)
                return false;
            return this.Source == other.Source &&
                this.KeyCode == other.KeyCode;
        }

        public override int GetHashCode()
        {
            return (int)this.KeyCode;
        }
    }

    public static class Controller
    {
        private static HashSet<Control> controlSequence = new HashSet<Control>();
        public static List<Control> ControlSequence
        {
            get { return controlSequence.ToList(); }
        }

        public static void AddControlToSequence(KeyCode keyCode)
        {
            controlSequence.Add(new Control(
                ControlSource.Keyboard,
                keyCode));
        }

        public static void RemoveControlFromSequence(KeyCode keyCode)
        {
            controlSequence.Remove(new Control(
                ControlSource.Keyboard,
                keyCode));
        }

        public static void RemoveAllControls()
        {
            controlSequence.Clear();
        }
    }
}

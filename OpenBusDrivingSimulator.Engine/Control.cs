using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBusDrivingSimulator.Engine
{
    public enum ControlType
    {
        CONTINUOUS,
        DISCRETE
    }

    public enum ControlSource
    {
        INVALID,
        KEYBOARD,
        MOUSE
    }

    public enum ControlState
    {
        PRESSED,
        ALREADY_PRESSED,
        RELEASED,
        ALREADY_RELEASED
    }
    public enum KeyCode
    {
        INVALID = -1,
        KEY_RIGHT = 79,
        KEY_LEFT = 80,
        KEY_UP = 81,
        KEY_DOWN = 82
    }

    public class Control
    {
        public ControlSource Source;
        public KeyCode KeyCode;

        public ControlType Type;
        public ControlState State;
        public double RepeatInterval;

        public Control()
        {
            this.Type = ControlType.DISCRETE;
            this.Source = ControlSource.INVALID;
            this.KeyCode = KeyCode.INVALID;
            this.State = ControlState.RELEASED;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlSource source, KeyCode keyCode)
        {
            this.Type = ControlType.DISCRETE;
            this.Source = source;
            this.KeyCode = keyCode;
            this.State = ControlState.PRESSED;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlType type, ControlSource source, KeyCode keyCode)
        {
            this.Type = type;
            this.Source = source;
            this.KeyCode = keyCode;
            this.State = ControlState.PRESSED;
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
            return 0;
        }
    }

    public class KeyInfo
    {
        public string Name;
        public KeyCode KeyCode;

        public KeyInfo()
        {
            this.Name = string.Empty;
            this.KeyCode = KeyCode.INVALID;
        }

        public KeyInfo(string name, KeyCode keyCode)
        {
            this.Name = name;
            this.KeyCode = keyCode;
        }
    }

    public static class KeyCodeConvert
    {
        private static List<KeyInfo> keyInfoList;

        static KeyCodeConvert()
        {
            keyInfoList = new List<KeyInfo>()
            {
                new KeyInfo("LEFT", KeyCode.KEY_LEFT),
                new KeyInfo("RIGHT", KeyCode.KEY_RIGHT),
                new KeyInfo("UP", KeyCode.KEY_UP),
                new KeyInfo("DOWN", KeyCode.KEY_DOWN)
            };
        }

        public static KeyCode GetKeyCode(string name)
        {
            foreach (KeyInfo keyInfo in keyInfoList)
                if (keyInfo.Name == name)
                    return keyInfo.KeyCode;
            return KeyCode.INVALID;
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
                ControlSource.KEYBOARD,
                keyCode));
        }

        public static void RemoveControlFromSequence(KeyCode keyCode)
        {
            controlSequence.Remove(new Control(
                ControlSource.KEYBOARD,
                keyCode));
        }

        public static void RemoveAllControls()
        {
            controlSequence.Clear();
        }
    }
}

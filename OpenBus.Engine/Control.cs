﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBus.Engine.Controls
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
        KEY_A = 4,
        KEY_B = 5,
        KEY_C = 6,
        KEY_D = 7,
        KEY_E = 8,
        KEY_F = 9,
        KEY_G = 10,
        KEY_H = 11,
        KEY_I = 12,
        KEY_J = 13,
        KEY_K = 14,
        KEY_L = 15,
        KEY_M = 16,
        KEY_N = 17,
        KEY_O = 18,
        KEY_P = 19,
        KEY_Q = 20,
        KEY_R = 21,
        KEY_S = 22,
        KEY_T = 23,
        KEY_U = 24,
        KEY_V = 25,
        KEY_W = 26,
        KEY_X = 27,
        KEY_Y = 28,
        KEY_Z = 29,
        KEY_RIGHT = 79,
        KEY_LEFT = 80,
        KEY_UP = 81,
        KEY_DOWN = 82
    }

    [Flags]
    public enum KeyModifier
    {
        NONE = 0,
        CTRL = 1,
        ALT = 2,
        SHIFT = 4
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
            this.Source = ControlSource.INVALID;
            this.KeyCode = KeyCode.INVALID;
            this.KeyModifierMask = KeyModifier.NONE;
            this.Type = ControlType.DISCRETE;
            this.DiscreteState = ControlState.RELEASED;
            this.ContinuousState = 0.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlSource source, KeyCode keyCode)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            // Default attributes, will be changed when this is being used
            this.KeyModifierMask = KeyModifier.NONE;
            this.Type = ControlType.DISCRETE;
            this.DiscreteState = ControlState.PRESSED;
            this.ContinuousState = 1.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlType type, ControlSource source, KeyCode keyCode)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            this.KeyModifierMask = KeyModifier.NONE;
            this.Type = type;
            this.DiscreteState = ControlState.PRESSED;
            this.ContinuousState = 1.0;
            this.RepeatInterval = 0.0;
        }

        public Control(ControlType type, ControlSource source, KeyCode keyCode, KeyModifier modifierMask)
        {
            this.Source = source;
            this.KeyCode = keyCode;
            this.KeyModifierMask = modifierMask;
            this.Type = type;
            this.DiscreteState = ControlState.PRESSED;
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
            return 0;
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
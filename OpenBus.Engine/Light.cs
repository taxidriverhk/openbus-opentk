using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenBus.Engine
{
    public enum LightType
    {
        Directional = 1,
        Point = 2,
        Spotlight = 3
    }

    public struct Light
    {
        public Vector3 Position;
        public Vector3 Color;
        public LightType Type;

        public Light(Vector3 position, Vector3 color, LightType type)
        {
            Position = position;
            Color = color;
            Type = type;
        }
    }
}

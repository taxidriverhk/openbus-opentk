using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace OpenBusDrivingSimulator.Engine
{
    public struct PhongShading
    {
        public Vector4 Emission;
        public Vector4 Ambient;
        public Vector4 Diffuse;
        public Vector4 Specular;
        public float Shininess;
        public float IndexOfRefraction;
    }

    public enum LightType
    {
        DIRECTIONAL = 1,
        POINT = 2,
        SPOTLIGHT = 3
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

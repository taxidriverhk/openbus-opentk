﻿using System;
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
}
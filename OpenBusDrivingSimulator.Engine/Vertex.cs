using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using OpenTK;

namespace OpenBusDrivingSimulator.Engine
{
    public struct Vertex
    {
        public static readonly int Size = Marshal.SizeOf(default(Vertex));

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public Vertex(Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        public Vertex(float px, float py, float pz,
            float nx, float ny, float nz,
            float s, float t)
        {
            this.Position = new Vector3(px, py, pz);
            this.Normal = new Vector3(nx, ny, nz);
            this.UV = new Vector2(s, t);
        }

        public void MoveBy(float x, float y, float z)
        {
            this.Position.X += x;
            this.Position.Y += y;
            this.Position.Z += z;
        }
    }
}

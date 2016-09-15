using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Renderer
    {
        private static double counter = -10;

        public static void Initialize()
        {
            
        }

        public static void RenderTest()
        {
            GL.ClearColor(Color.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-10.0, 10.0, -10.0, 10.0, 0.0, 4.0);

            GL.Begin(PrimitiveType.Triangles);

            GL.Color3(Color.MidnightBlue);
            GL.Vertex2(-1.0f + counter, 1.0f);
            GL.Color3(Color.SpringGreen);
            GL.Vertex2(0.0f + counter, -1.0f);
            GL.Color3(Color.Ivory);
            GL.Vertex2(1.0f + counter, 1.0f);

            GL.End();

            counter += 0.1;
            if (counter > 10)
                counter = -10;
        }

    }
}

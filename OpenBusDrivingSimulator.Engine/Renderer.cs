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
            GL.Enable(EnableCap.Texture2D);
        }

        public static void RenderTest()
        {
            GL.ClearColor(Color.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            Matrix4 lookAt = Matrix4.LookAt(0.0f, 0.0f, 10.0f,
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1280/720, 1.0f, 10.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(counter - 1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); 
            GL.Vertex2(counter + 1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); 
            GL.Vertex2(counter + 1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(counter - 1.0f, 1.0f);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[1]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(counter - 4.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(counter - 3.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(counter - 3.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(counter - 4.0f, 1.0f);
            GL.End();

            counter += 0.1;
            if (counter > 10)
                counter = -10;
        }

    }
}

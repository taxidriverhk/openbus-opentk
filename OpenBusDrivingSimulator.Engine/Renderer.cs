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
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Renderer
    {
        private static float speed = 2.0f;
        private static float displacement = 0.0f;

        private static Bitmap textBmp;

        public static void Initialize()
        {
            textBmp = new Bitmap(Screen.Width, Screen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            InitailizeBuffer();
        }

        public static void DrawText(string text, int x, int y, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Brush brush = new SolidBrush(color);
            Font font = new Font(FontFamily.GenericSansSerif, 20.0f);
            using(Graphics gfx = Graphics.FromImage(textBmp))
            {
                gfx.Clear(Color.Transparent);
                gfx.DrawString(text, font, brush, new PointF(0, Screen.Height - 30));
            }

            int textTextureId = Texture.LoadTextureFromBitmap(textBmp);

            GL.Disable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0.0f, 0.0f, Screen.Width, Screen.Height, -1.0f, 1.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textTextureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex2(-1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex2(1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex2(1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex2(-1.0f, 1.0f);
            GL.End();
            Texture.UnloadTexture(textTextureId);

            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
            GL.Disable(EnableCap.Blend);
        }

        public static void DrawText(string text, int x, int y)
        {
            DrawText(text, x, y, Color.White);
        }

        #region Test Functions
        public static void RenderTestScene(double timeElapsed)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            displacement += (float)timeElapsed * speed;
            DrawTestCubeWithBuffer();
            if (displacement >= 5 || displacement <= -5)
            {
                displacement = displacement < 0 ? -5 : 5;
                speed *= -1;
            }
        }

        public static void DrawTestCube()
        {
            float zPosition = 10.0f;
            Vector3[] vertices =
            {
                new Vector3(displacement - 2.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement + 2.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement + 2.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement - 2.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement - 2.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement + 2.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement + 2.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement - 2.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
            };

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.Begin(PrimitiveType.Quads);

            // Front
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(vertices[0]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(vertices[1]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(vertices[2]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(vertices[3]);
            GL.Normal3(0.0f, 0.0f, 1.0f);

            GL.End();
        }
        #endregion

        #region Vertex Buffer Object
        // Buffer related fields
        private static Vertex[] bufferVertices;
        private static ushort[] bufferVertexIndices;
        private static uint vertexBufferId;
        private static uint indexBufferId;

        private static void InitailizeBuffer()
        {
            float zPosition = 10.0f;
            bufferVertices = new Vertex[]
            {
                new Vertex(-1, 1, zPosition, 0, 0, 1, 0, 0),
                new Vertex(-1, -1, zPosition, 0, 0, 1, 0, 1),
                new Vertex(1, -1, zPosition, 0, 0, 1, 1, 1),
                new Vertex(1, 1, zPosition, 0, 0, 1, 1, 0)
            };

            GL.Enable(EnableCap.Normalize);

            GL.GenBuffers(1, out vertexBufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(bufferVertices.Length * Vertex.Size), bufferVertices, BufferUsageHint.StaticDraw);

            bufferVertexIndices = new ushort[] { 0, 1, 2, 3, 0, 2 };
            GL.GenBuffers(1, out indexBufferId);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(bufferVertexIndices.Length * sizeof(ushort)), bufferVertexIndices, BufferUsageHint.StaticDraw);
        }

        public static void ClearBuffer()
        {
            GL.DeleteBuffers(1, ref indexBufferId);
            GL.DeleteBuffers(1, ref vertexBufferId);
        }

        public static void DrawTestCubeWithBuffer()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
            GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Size, Vector3.SizeInBytes * 2);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.DrawElements(BeginMode.Triangles, bufferVertexIndices.Length, DrawElementsType.UnsignedShort, 0);

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
        #endregion
    }
}

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
    /// <summary>
    /// Responsible for drawing objects and texts for the screen to show
    /// </summary>
    public static class Renderer
    {
        private static float speed = 2.0f;
        private static float displacement = 0.0f;

        /// <summary>
        /// Bitmap used for converting strings into a picture for display
        /// </summary>
        private static Bitmap textBmp;

        /// <summary>
        /// Initialize the components of the renderer, should be called before the main loop
        /// Cleanup function must also be called after the main loop to cleanup everything
        /// </summary>
        public static void Initialize()
        {
            textBmp = new Bitmap(Screen.Width, Screen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            InitailizeBuffer();
            InitializeMirrorBuffer();
        }

        /// <summary>
        /// Cleans up the memory and components associated to this renderer
        /// Must be called after the main loop to cleanup everything
        /// </summary>
        public static void Cleanup()
        {
            ClearBuffer();
            ClearMirrorBuffer();
        }

        /// <summary>
        /// Renders 2D text on the screen
        /// </summary>
        /// <param name="text">Text to be printed</param>
        /// <param name="left">Left position percentage of the text relative to the screen, from 0 to 100</param>
        /// <param name="top">Top position percentage of the text relative to the screen, from 0 to 100</param>
        /// <param name="color">Foreground of the text to be printed</param>
        public static void DrawText(string text, int left, int top, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            float screenLeft = 0.01f * left * Screen.Width,
                  screenTop = 0.01f * top * Screen.Height;
            Brush brush = new SolidBrush(color);
            Font font = new Font(FontFamily.GenericSansSerif, 20.0f);
            using(Graphics gfx = Graphics.FromImage(textBmp))
            {
                gfx.Clear(Color.Transparent);
                gfx.DrawString(text, font, brush, new PointF(screenLeft, screenTop));
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

        /// <summary>
        /// Renders 2D text in white color on the screen
        /// </summary>
        public static void DrawText(string text, int left, int bottom)
        {
            DrawText(text, left, bottom, Color.White);
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
            DrawMirror();
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

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[1]);
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

        #region Mirror
        private static uint frameBufferId;
        private static uint rendererBufferId;
        private static int mirrorTextureId;

        public static void InitializeMirrorBuffer()
        {
            GL.GenFramebuffers(1, out frameBufferId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            GL.GenTextures(1, out mirrorTextureId);
            GL.BindTexture(TextureTarget.Texture2D, mirrorTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                1280, 720, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            GL.Viewport(0, 0, 1280, 720);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, mirrorTextureId, 0);

            GL.GenRenderbuffers(1, out rendererBufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rendererBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.Depth24Stencil8, 1280, 720);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, rendererBufferId);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public static void ClearMirrorBuffer()
        {
            GL.DeleteRenderbuffers(1, ref rendererBufferId);
            GL.DeleteTexture(mirrorTextureId);
            GL.DeleteFramebuffers(1, ref frameBufferId);
        }

        public static void DrawMirror()
        {
            // Render the scene to texture
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
            GL.ClearColor(Color.Yellow);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)1280/720, 0.025f, 20);
            GL.LoadMatrix(ref projection);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            Matrix4 lookAt = Matrix4.LookAt(new Vector3(0, 0, 0), new Vector3(0, 0, 20), Vector3.UnitY);
            GL.LoadMatrix(ref lookAt);
            DrawTestCubeWithBuffer();
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            Vector3[] vertices =
            {
                new Vector3(3.0f, 0.0f, 8),
                new Vector3(4.0f, 0.0f, 6),
                new Vector3(4.0f, 2.0f, 6),
                new Vector3(3.0f, 2.0f, 8)
            };
            GL.BindTexture(TextureTarget.Texture2D, mirrorTextureId);
            GL.Begin(PrimitiveType.Quads);
            // Front
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(vertices[0]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(vertices[1]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(vertices[2]);
            GL.Normal3(0.0f, 0.0f, 1.0f);
            GL.TexCoord2(0.0f, 1.0f);
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
                new Vertex(-1.5f, 1, zPosition, 0, 0, 1, 0, 0),
                new Vertex(-1.5f, -1, zPosition, 0, 0, 1, 0, 1),
                new Vertex(1.5f, -1, zPosition, 0, 0, 1, 1, 1),
                new Vertex(1.5f, 1, zPosition, 0, 0, 1, 1, 0)
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
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(displacement, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.PushAttrib(AttribMask.CurrentBit);
            GL.Color4(1, 0, 0, 0.5);
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
            GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Size, Vector3.SizeInBytes * 2);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.DrawElements(BeginMode.Triangles, bufferVertexIndices.Length, DrawElementsType.UnsignedShort, 0);

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.PopAttrib();

            GL.PopMatrix();
        }
        #endregion
    }
}

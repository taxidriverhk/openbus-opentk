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
    /// <summary>
    /// Responsible for drawing objects and texts for the screen to show.
    /// </summary>
    public static class Renderer
    {
        /// <summary>
        /// Bitmap used for converting strings into a picture for display.
        /// </summary>
        private static Bitmap textBmp;

        /// <summary>
        /// Initialize the components of the renderer, should be called before the main loop.
        /// Cleanup function must also be called after the main loop to cleanup everything.
        /// </summary>
        public static void Initialize()
        {
            textBmp = new Bitmap(Screen.Width, Screen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            InitializeMirrorBuffer();
        }

        /// <summary>
        /// Cleans up the memory and components associated to this renderer.
        /// Must be called after the main loop to cleanup everything.
        /// </summary>
        public static void Cleanup()
        {
            ClearMirrorBuffer();
            RemoveAllStaticMeshes();
        }

        /// <summary>
        /// Renders 2D text on the screen
        /// </summary>
        /// <param name="text">Text to be printed</param>
        /// <param name="left">Left position percentage of the text relative to the screen, from 0 to 100.</param>
        /// <param name="top">Top position percentage of the text relative to the screen, from 0 to 100.</param>
        /// <param name="color">Foreground of the text to be printed.</param>
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
        /// Renders 2D text with white foreground and black thin border on the screen.
        /// </summary>
        public static void DrawText(string text, int left, int bottom)
        {
            DrawText(text, left, bottom, Color.White);
        }

        private static List<uint> staticMeshesBufferIds = new List<uint>();
        private static List<uint> staticMeshesIndexIds = new List<uint>();
        private static Dictionary<uint, List<int>> staticBufferTextureIdMapping = new Dictionary<uint, List<int>>();
        private static Dictionary<int, KeyValuePair<int, int>> textureIndexArrayMapping = new Dictionary<int, KeyValuePair<int, int>>();

        public static int LoadStaticMeshesToScene(List<Mesh> meshes)
        {
            int meshesLoaded = 0;
            uint bufferId = 0,
                 indexBufferId = 0;

            GL.Enable(EnableCap.Normalize);
            GL.GenBuffers(1, out bufferId);
            GL.GenBuffers(1, out indexBufferId);

            staticMeshesBufferIds.Add(bufferId);
            staticMeshesIndexIds.Add(indexBufferId);

            uint maxArraySize = UInt32.MaxValue;
            List<int> textureIdsLoaded = new List<int>();
            List<Vertex> verticesToBeLoaded = new List<Vertex>();
            List<uint> indicesToBeLoaded = new List<uint>();

            foreach (Mesh mesh in meshes)
            {
                for (int i = 0; i < mesh.Vertices.Length; i++)
                {
                    uint startIndex = (uint)verticesToBeLoaded.Count;
                    // Increment the indices to align with the index of the vertices
                    for (int j = 0; j < mesh.Indices[i].Length; j++)
                    {
                        uint currentIndex = startIndex + mesh.Indices[i][j];
                        // If the index array exceeds the max allowable size
                        // Then return with the current mesh unloaded
                        if (currentIndex >= maxArraySize)
                            return meshesLoaded;
                        indicesToBeLoaded.Add(currentIndex);
                    }
                    verticesToBeLoaded.AddRange(mesh.Vertices[i].ToList());
                    textureIndexArrayMapping.Add(mesh.Materials[i].TextureId, 
                        new KeyValuePair<int, int>((int)startIndex, mesh.Indices[i].Length));
                    textureIdsLoaded.Add(mesh.Materials[i].TextureId);
                }
                meshesLoaded++;
            }

            // Finally load everything into the buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(verticesToBeLoaded.Count * Vertex.Size),
                verticesToBeLoaded.ToArray(), BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indicesToBeLoaded.Count * sizeof(uint)),
                indicesToBeLoaded.ToArray(), BufferUsageHint.StaticDraw);

            staticBufferTextureIdMapping.Add(bufferId, textureIdsLoaded);
            return meshesLoaded;
        }

        public static void DrawStaticMeshes()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.SkyBlue);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            for (int i = 0; i < staticMeshesBufferIds.Count; i++)
            {
                uint currentBufferId = staticMeshesBufferIds[i],
                     currentIndexId = staticMeshesIndexIds[i];
                List<int> bufferTextureIds = staticBufferTextureIdMapping[currentBufferId];
                foreach (int bufferTextureId in bufferTextureIds)
                {
                    int indexArrayOffset = textureIndexArrayMapping[bufferTextureId].Key,
                        indexArrayLength = textureIndexArrayMapping[bufferTextureId].Value;

                    GL.MatrixMode(MatrixMode.Modelview);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, currentBufferId);
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.EnableClientState(ArrayCap.TextureCoordArray);

                    GL.BindTexture(TextureTarget.Texture2D, bufferTextureId);
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
                    GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Size, Vector3.SizeInBytes * 2);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, currentIndexId);
                    GL.DrawElements(BeginMode.Triangles, indexArrayLength, DrawElementsType.UnsignedInt, 
                        indexArrayOffset * sizeof(uint));

                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                    GL.DisableClientState(ArrayCap.NormalArray);
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
            }

            GL.Disable(EnableCap.CullFace);
        }


        /// <summary>
        /// Removes all static meshes loaded to the world block.
        /// </summary>
        public static void RemoveAllStaticMeshes()
        {
            for (int i = 0; i < staticMeshesBufferIds.Count; i++)
            {
                uint targetBufferId = staticMeshesBufferIds[i],
                     targetIndexId = staticMeshesIndexIds[i];
                GL.DeleteBuffers(1, ref targetBufferId);
                GL.DeleteBuffers(1, ref targetIndexId);
            }

            staticMeshesBufferIds.Clear();
            staticMeshesIndexIds.Clear();
            staticBufferTextureIdMapping.Clear();
            textureIndexArrayMapping.Clear();
        }

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
            //DrawMeshes();
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
    }
}

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
            RemoveAllMeshes();
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

        /// <summary>
        /// Number of meshes (not the number of vertex buffers) loaded.
        /// </summary>
        private static uint meshesLoaded = 0;
        /// <summary>
        /// List of vertex buffer IDs that are allocated to vertex buffers.
        /// </summary>
        private static List<uint> modelBufferIds = new List<uint>();
        /// <summary>
        /// Mapping of a mesh ID to its list of vertex buffer IDs.
        /// </summary>
        private static Dictionary<uint, List<uint>> meshIdMapping = new Dictionary<uint, List<uint>>();
        /// <summary>
        /// Mapping of a vertex buffer ID to its texture ID.
        /// </summary>
        private static Dictionary<uint, int> textureIdMapping = new Dictionary<uint, int>();
        /// <summary>
        /// Mapping of a vertex buffer ID to its index array buffer ID.
        /// </summary>
        private static Dictionary<uint, uint> indexBufferIdMapping = new Dictionary<uint, uint>();
        /// <summary>
        /// Mapping of an index array ID to its length (i.e. number of elements in the array).
        /// </summary>
        private static Dictionary<uint, int> indexBufferLengths = new Dictionary<uint, int>();

        /// <summary>
        /// Loads a mesh into the scene with its position in the world coordinates.
        /// </summary>
        /// <param name="mesh">The mesh to be loaded.</param>
        /// <returns>
        /// The mesh ID if the mesh was loaded successfully.
        /// 0 if the mesh is null or is invalid.
        /// </returns>
        public static uint LoadMeshToScene(Mesh mesh)
        {
            if (mesh == null || !mesh.Validate())
                return 0;

            // Load vertices into the buffer for each material
            List<uint> bufferIdList = new List<uint>();
            for (int i = 0; i < mesh.Vertices.Length; i++)
            {
                uint bufferId = 0,
                     indexBufferId = 0;

                GL.Enable(EnableCap.Normalize);

                GL.GenBuffers(1, out bufferId);
                GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(mesh.Vertices[i].Length * Vertex.Size), mesh.Vertices[i], BufferUsageHint.StaticDraw);

                GL.GenBuffers(1, out indexBufferId);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mesh.Indices[i].Length * sizeof(ushort)), mesh.Indices[i], BufferUsageHint.StaticDraw);

                modelBufferIds.Add(bufferId);
                bufferIdList.Add(bufferId);
                textureIdMapping.Add(bufferId, mesh.Materials[i].TextureId);
                indexBufferIdMapping.Add(bufferId, indexBufferId);
                indexBufferLengths.Add(indexBufferId, mesh.Indices[i].Length);
            }

            meshesLoaded++;
            meshIdMapping.Add(meshesLoaded, bufferIdList);

            return meshesLoaded;
        }

        /// <summary>
        /// Draws all meshes loaded to the scene.
        /// </summary>
        public static void DrawMeshes()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            foreach (uint meshId in meshIdMapping.Keys)
            {
                foreach (uint bufferId in meshIdMapping[meshId])
                {
                    int textureId = textureIdMapping[bufferId];
                    uint indexArrayId = indexBufferIdMapping[bufferId];
                    int indexArrayLength = indexBufferLengths[indexArrayId];

                    GL.MatrixMode(MatrixMode.Modelview);

                    GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
                    GL.EnableClientState(ArrayCap.VertexArray);
                    GL.EnableClientState(ArrayCap.NormalArray);
                    GL.EnableClientState(ArrayCap.TextureCoordArray);

                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
                    GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Size, Vector3.SizeInBytes * 2);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexArrayId);
                    GL.DrawElements(BeginMode.Triangles, indexArrayLength, DrawElementsType.UnsignedShort, 0);

                    GL.DisableClientState(ArrayCap.TextureCoordArray);
                    GL.DisableClientState(ArrayCap.NormalArray);
                    GL.DisableClientState(ArrayCap.VertexArray);
                }
            }
        }

        /// <summary>
        /// Removes a mesh by its mesh ID.
        /// </summary>
        /// <param name="meshId">The identification number of the mesh loaded to the scene.</param>
        /// <returns>
        /// True if the mesh was removed successfully.
        /// False if either of the following occurs:
        /// 1. Mesh ID does not exist.
        /// 2. Any of the vertex or index array buffers could not be cleared.
        /// 3. Any of the textures could not be unloaded.
        /// </returns>
        public static bool RemoveMesh(uint meshId)
        {
            if (!meshIdMapping.ContainsKey(meshId))
                return false;

            foreach(uint bufferId in meshIdMapping[meshId])
            {
                uint targetBufferId = bufferId,
                     targetIndexArrayId = indexBufferIdMapping[bufferId];
                int textureId = textureIdMapping[bufferId];
                GL.DeleteBuffers(1, ref targetBufferId);
                GL.DeleteBuffers(1, ref targetIndexArrayId);
                Texture.UnloadTexture(textureId);

                indexBufferIdMapping.Remove(bufferId);
                textureIdMapping.Remove(bufferId);
                modelBufferIds.Remove(bufferId);
            }
            meshIdMapping.Remove(meshId);

            return true;
        }

        /// <summary>
        /// Removes all meshes loaded to the scene.
        /// </summary>
        public static void RemoveAllMeshes()
        {
            List<uint> targetMeshIds = meshIdMapping.Keys.ToList();
            foreach (uint meshId in targetMeshIds)
                RemoveMesh(meshId);
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
            DrawMeshes();
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

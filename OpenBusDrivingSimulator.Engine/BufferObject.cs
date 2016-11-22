using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenBusDrivingSimulator.Engine
{
    /// <summary>
    /// 
    /// </summary>
    internal static class BufferObjectHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="indexBufferId"></param>
        internal static void CreateBuffers(out uint bufferId, out uint indexBufferId)
        {
            GL.Enable(EnableCap.Normalize);
            GL.GenBuffers(1, out bufferId);
            GL.GenBuffers(1, out indexBufferId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="indexBufferId"></param>
        internal static void DeleteBuffers(uint bufferId, uint indexBufferId)
        {
            GL.DeleteBuffers(1, ref bufferId);
            GL.DeleteBuffers(1, ref indexBufferId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bufferId"></param>
        /// <param name="indexId"></param>
        /// <param name="textureId"></param>
        /// <param name="indexArrayStart"></param>
        /// <param name="indexArrayLength"></param>
        internal static void BindAndDrawBuffer(uint bufferId, uint indexId, int textureId, int indexArrayStart, int indexArrayLength)
        {
            GL.MatrixMode(MatrixMode.Modelview);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
            GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vertex.Size, Vector3.SizeInBytes * 2);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexId);
            GL.DrawElements(BeginMode.Triangles, indexArrayLength, DrawElementsType.UnsignedInt,
                indexArrayStart * sizeof(uint));

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class StaticBufferObject
    {
        private List<uint> bufferIds;
        private List<uint> indexIds;
        private Dictionary<uint, List<int>> bufferTextureIdMapping;
        private Dictionary<int, KeyValuePair<int, int>> textureIndexMapping;

        internal StaticBufferObject()
        {
            bufferIds = new List<uint>();
            indexIds = new List<uint>();
            bufferTextureIdMapping = new Dictionary<uint, List<int>>();
            textureIndexMapping = new Dictionary<int, KeyValuePair<int, int>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        internal int LoadMeshes(List<Mesh> meshes)
        {
            int meshesLoaded = 0;
            uint bufferId = 0,
                 indexBufferId = 0;

            BufferObjectHelper.CreateBuffers(out bufferId, out indexBufferId);
            bufferIds.Add(bufferId);
            indexIds.Add(indexBufferId);

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
                    textureIndexMapping.Add(mesh.Materials[i].TextureId,
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

            bufferTextureIdMapping.Add(bufferId, textureIdsLoaded);
            return meshesLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void DrawMeshes()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            for (int i = 0; i < bufferIds.Count; i++)
            {
                uint currentBufferId = bufferIds[i],
                     currentIndexId = indexIds[i];
                List<int> bufferTextureIds = bufferTextureIdMapping[currentBufferId];
                foreach (int bufferTextureId in bufferTextureIds)
                {
                    int indexArrayOffset = textureIndexMapping[bufferTextureId].Key,
                        indexArrayLength = textureIndexMapping[bufferTextureId].Value;
                    BufferObjectHelper.BindAndDrawBuffer(currentBufferId, currentIndexId, 
                        bufferTextureId, indexArrayOffset, indexArrayLength);
                }
            }
            GL.Disable(EnableCap.CullFace);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Cleanup()
        {
            for (int i = 0; i < bufferIds.Count; i++)
            {
                uint targetBufferId = bufferIds[i],
                     targetIndexId = indexIds[i];
                BufferObjectHelper.DeleteBuffers(targetBufferId, targetIndexId);
            }

            bufferIds.Clear();
            indexIds.Clear();
            bufferTextureIdMapping.Clear();
            textureIndexMapping.Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class DynamicBufferObject
    {

    }

    /// <summary>
    /// 
    /// </summary>
    internal class OverlayTextObject
    {
        /// <summary>
        /// Bitmap used for converting strings into a picture for display.
        /// </summary>
        private Bitmap textBmp;

        internal OverlayTextObject()
        {
            textBmp = new Bitmap(Screen.Width, Screen.Height, 
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        /// <summary>
        /// Renders 2D text on the screen
        /// </summary>
        /// <param name="text">Text to be printed</param>
        /// <param name="left">Left position percentage of the text relative to the screen, from 0 to 100.</param>
        /// <param name="top">Top position percentage of the text relative to the screen, from 0 to 100.</param>
        /// <param name="color">Foreground of the text to be printed.</param>
        internal void DrawText(string text, int left, int top, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            float screenLeft = 0.01f * left * Screen.Width,
                  screenTop = 0.01f * top * Screen.Height;
            Brush brush = new SolidBrush(color);
            Font font = new Font(FontFamily.GenericSansSerif, 20.0f);
            using (Graphics gfx = Graphics.FromImage(textBmp))
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
    }

    /// <summary>
    /// 
    /// </summary>
    internal class MirrorBufferObject
    {
        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        private float viewableDistance;

        private StaticBufferObject staticScene;
        private uint mirrorBufferId;
        private uint mirrorIndexId;
        private int mirrorIndexArrayLength;

        private uint frameBufferId;
        private uint rendererBufferId;
        private int mirrorTextureId;

        /// <summary>
        /// 
        /// </summary>
        internal MirrorBufferObject()
        {
            viewableDistance = 20.0f;
            position = new Vector3(0, 5, -20.0f);
            target = new Vector3(0, 5, viewableDistance);
            up = Vector3.UnitY; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mirrorMesh"></param>
        /// <param name="scene"></param>
        internal void InitializeMirror(Mesh mirrorMesh, StaticBufferObject scene)
        {
            // Create the frame buffer and the texture
            GL.GenFramebuffers(1, out frameBufferId);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);

            GL.GenTextures(1, out mirrorTextureId);
            GL.BindTexture(TextureTarget.Texture2D, mirrorTextureId);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb,
                Screen.Width, Screen.Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            // Bind the texture with the framebuffer
            GL.Viewport(0, 0, Screen.Width, Screen.Height);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D, mirrorTextureId, 0);

            // Bind the renderer buffer with the frame buffer
            GL.GenRenderbuffers(1, out rendererBufferId);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rendererBufferId);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                RenderbufferStorage.Depth24Stencil8, Screen.Width, Screen.Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthStencilAttachment,
                RenderbufferTarget.Renderbuffer, rendererBufferId);

            // Create the buffers used for the mirror mesh
            BufferObjectHelper.CreateBuffers(out mirrorBufferId, out mirrorIndexId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mirrorBufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(mirrorMesh.Vertices[0].Length * Vertex.Size),
                mirrorMesh.Vertices[0], BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mirrorIndexId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(mirrorMesh.Indices[0].Length * sizeof(uint)),
                mirrorMesh.Indices[0], BufferUsageHint.StaticDraw);
            mirrorIndexArrayLength = mirrorMesh.Indices[0].Length;
            staticScene = scene;

            // Set the current frame back to the screen
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void DrawMirror()
        {
            // Render the scene to texture
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBufferId);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 
                (float)Screen.Width / Screen.Height, 0.025f, viewableDistance);
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            Matrix4 lookAt = Matrix4.LookAt(position, target, up);
            GL.LoadMatrix(ref lookAt);
            // Draw the scene
            staticScene.DrawMeshes();
            GL.PopMatrix(); // Pops the current modelview matrix
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            // Set the current frame back to the screen
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            // Draw the mirror mesh with the rendered texture
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            BufferObjectHelper.BindAndDrawBuffer(mirrorBufferId, mirrorIndexId,
                mirrorTextureId, 0, mirrorIndexArrayLength);
            GL.Disable(EnableCap.CullFace);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Cleanup()
        {
            BufferObjectHelper.DeleteBuffers(mirrorBufferId, mirrorIndexId);
            GL.DeleteRenderbuffers(1, ref rendererBufferId);
            GL.DeleteTexture(mirrorTextureId);
            GL.DeleteFramebuffers(1, ref frameBufferId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SkyBoxObject
    {
        private int textureId;
        private uint bufferId;
        private uint indexArrayId;
        private int indexArrayLength;

        internal void LoadSkyBox(Mesh skyBoxMesh, Vector3 scale, string textureFile)
        {
            // Enlarge the skybox
            for (int i = 0; i < skyBoxMesh.Vertices[0].Length; i++)
            {
                skyBoxMesh.Vertices[0][i].Position.X *= scale.X;
                skyBoxMesh.Vertices[0][i].Position.Y *= scale.Y;
                skyBoxMesh.Vertices[0][i].Position.Z *= scale.Z;
            }

            // Create the buffer and texture neededfor the skybox
            textureId = Texture.LoadTextureFromFile(textureFile);
            BufferObjectHelper.CreateBuffers(out bufferId, out indexArrayId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(skyBoxMesh.Vertices[0].Length * Vertex.Size),
                skyBoxMesh.Vertices[0], BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexArrayId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(skyBoxMesh.Indices[0].Length * sizeof(uint)),
                skyBoxMesh.Indices[0], BufferUsageHint.StaticDraw);
            indexArrayLength = skyBoxMesh.Indices[0].Length;
        }

        internal void ReplaceTextures(string textureFile)
        {
            Texture.UnloadTexture(textureId);
            textureId = Texture.LoadTextureFromFile(textureFile);
        }

        internal void DrawSkyBox()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(Camera.Eye);
            BufferObjectHelper.BindAndDrawBuffer(bufferId, indexArrayId, textureId, 0, indexArrayLength);
            GL.PopMatrix();
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        internal void Cleanup()
        {
            Texture.UnloadTexture(textureId);
            BufferObjectHelper.DeleteBuffers(bufferId, indexArrayId);
        }
    }

    internal class TerrainBufferObject
    {

    }
}

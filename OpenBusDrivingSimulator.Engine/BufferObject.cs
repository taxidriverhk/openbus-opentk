using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenBusDrivingSimulator.Engine
{
    internal enum BufferObjectDrawingMode
    {
        ALL = 0,
        COLOR_ONLY = 1,
        ALPHA_ONLY = 2,
        NON_ALPHA_ONLY = 3
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class BufferObjectHelper
    {
        internal static void CreateBuffers(out uint bufferId)
        {
            GL.Enable(EnableCap.Normalize);
            GL.GenBuffers(1, out bufferId);
        }

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

        internal static void LoadDataIntoBuffers(uint bufferId, Vertex[] vertices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vertex.Size),
                vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        internal static void LoadDataIntoBuffers(uint bufferId, Vertex[] vertices, uint indexBufferId, uint[] indices)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertices.Length * Vertex.Size),
                vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)),
                indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        internal static void DeleteBuffers(uint bufferId)
        {
            GL.DeleteBuffers(1, ref bufferId);
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

        internal static void BindAndDrawBuffer(uint bufferId, int textureId, int startIndex, int length)
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

            GL.DrawArrays(PrimitiveType.Triangles, startIndex * Vertex.Size, length);

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        internal static void BindAndDrawBuffer(uint bufferId, int textureId, int startIndex, int length,
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            // Scaling -> Rotation (both about the origin) -> Translation
            // (so the matrix would be translate * rotation * scaling)
            GL.Translate(translation);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Scale(scale);
            BindAndDrawBuffer(bufferId, textureId, startIndex, length);
            GL.PopMatrix();
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

        internal static void BindAndDrawBuffer(uint bufferId, uint indexId, int textureId, int indexArrayStart, int indexArrayLength,
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            // Scaling -> Rotation (both about the origin) -> Translation
            // (so the matrix would be translate * rotation * scaling)
            GL.Translate(translation);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Scale(scale);
            BindAndDrawBuffer(bufferId, indexId, textureId, indexArrayStart, indexArrayLength);
            GL.PopMatrix();
        }

        internal static void BindAndDrawBuffer(uint bufferId, uint indexId, int indexArrayStart, int indexArrayLength, Vector3 color)
        {
            GL.MatrixMode(MatrixMode.Modelview);

            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.VertexPointer(3, VertexPointerType.Float, Vertex.Size, 0);
            GL.NormalPointer(NormalPointerType.Float, Vertex.Size, Vector3.SizeInBytes);
            GL.Color3(color);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexId);
            GL.DrawElements(BeginMode.Triangles, indexArrayLength, DrawElementsType.UnsignedInt,
                indexArrayStart * sizeof(uint));
            GL.Color3(Vector3.One);

            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.VertexArray);
        }

        internal static void BindAndDrawBuffer(uint bufferId, uint indexId, int indexArrayStart, int indexArrayLength,
            Vector3 color, Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.Translate(translation);
            GL.Rotate(rotation.Y, Vector3.UnitY);
            GL.Scale(scale);
            BindAndDrawBuffer(bufferId, indexId, indexArrayStart, indexArrayLength, color);
            GL.PopMatrix();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class StaticBufferObject
    {
        private List<Entity> entities;
        private List<uint> bufferIds;
        private Dictionary<uint, uint> bufferIndexIdMapping;
        private Dictionary<int, uint> textureIdBufferMapping;
        private Dictionary<string, List<int>> meshTextureIdMapping;
        private Dictionary<int, KeyValuePair<int, int>> textureIndexMapping;

        internal StaticBufferObject()
        {
            bufferIds = new List<uint>();
            bufferIndexIdMapping = new Dictionary<uint, uint>();
            textureIdBufferMapping = new Dictionary<int, uint>();
            meshTextureIdMapping = new Dictionary<string, List<int>>();
            textureIndexMapping = new Dictionary<int, KeyValuePair<int, int>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        internal int LoadEntities(List<Entity> entitiesToBeLoaded, ISet<Mesh> meshes)
        {
            int meshesLoaded = 0;
            uint bufferId = 0,
                 indexBufferId = 0;

            BufferObjectHelper.CreateBuffers(out bufferId, out indexBufferId);
            bufferIds.Add(bufferId);
            bufferIndexIdMapping.Add(bufferId, indexBufferId);

            uint maxArraySize = UInt32.MaxValue;
            List<int> textureIdsLoaded = new List<int>();
            List<Vertex> verticesToBeLoaded = new List<Vertex>();
            List<uint> indicesToBeLoaded = new List<uint>();

            entities = entitiesToBeLoaded;
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
                    textureIdBufferMapping.Add(mesh.Materials[i].TextureId, bufferId);
                    textureIdsLoaded.Add(mesh.Materials[i].TextureId);
                }
                meshTextureIdMapping.Add(mesh.Name, textureIdsLoaded);
                meshesLoaded++;
                textureIdsLoaded = new List<int>();
            }

            // Finally load everything into the buffer
            BufferObjectHelper.LoadDataIntoBuffers(bufferId, verticesToBeLoaded.ToArray(),
                indexBufferId, indicesToBeLoaded.ToArray());

            return meshesLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        internal void DrawEntities()
        {
            DrawEntities(BufferObjectDrawingMode.ALL);   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        internal void DrawEntities(BufferObjectDrawingMode mode)
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            foreach (Entity entity in entities)
            {
                List<int> textureIds = meshTextureIdMapping[entity.MeshName];
                foreach (int textureId in textureIds)
                {
                    if (mode == BufferObjectDrawingMode.ALPHA_ONLY
                        && !Texture.AlphaTextureIds.Contains(textureId))
                        continue;
                    else if (mode == BufferObjectDrawingMode.NON_ALPHA_ONLY
                        && Texture.AlphaTextureIds.Contains(textureId))
                        continue;

                    uint bufferId = textureIdBufferMapping[textureId],
                         indexArrayId = bufferIndexIdMapping[bufferId];
                    int indexArrayOffset = textureIndexMapping[textureId].Key,
                        indexArrayLength = textureIndexMapping[textureId].Value;
                    if (mode == BufferObjectDrawingMode.COLOR_ONLY)
                        BufferObjectHelper.BindAndDrawBuffer(bufferId, indexArrayId,
                            indexArrayOffset, indexArrayLength, entity.Color,
                            entity.Translation, entity.Rotation, Vector3.One);
                    else
                        BufferObjectHelper.BindAndDrawBuffer(bufferId, indexArrayId,
                            textureId, indexArrayOffset, indexArrayLength,
                            entity.Translation, entity.Rotation, Vector3.One);
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
                     targetIndexId = bufferIndexIdMapping[targetBufferId];
                BufferObjectHelper.DeleteBuffers(targetBufferId, targetIndexId);
            }

            bufferIds.Clear();
            bufferIndexIdMapping.Clear();
            entities.Clear();
            textureIdBufferMapping.Clear();
            meshTextureIdMapping.Clear();
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

            int textTextureId = Texture.LoadTexture(textBmp);

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
            staticScene.DrawEntities();
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
        private int vertexCount;

        internal void LoadSkyBox(Mesh skyBoxMesh, Vector3 scale, int skyBoxTextureId)
        {
            // Enlarge the skybox
            for (int i = 0; i < skyBoxMesh.Vertices[0].Length; i++)
            {
                skyBoxMesh.Vertices[0][i].Position.X *= scale.X;
                skyBoxMesh.Vertices[0][i].Position.Y *= scale.Y;
                skyBoxMesh.Vertices[0][i].Position.Z *= scale.Z;
            }

            // Create the buffer and texture needed for the skybox
            textureId = skyBoxTextureId;
            BufferObjectHelper.CreateBuffers(out bufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(skyBoxMesh.Vertices[0].Length * Vertex.Size),
                skyBoxMesh.Vertices[0], BufferUsageHint.StaticDraw);
            vertexCount = skyBoxMesh.Vertices[0].Length;
        }

        internal void ReplaceTextures(string textureFile)
        {
            Texture.UnloadTexture(textureId);
            textureId = Texture.LoadTexture(textureFile);
        }

        internal void DrawSkyBox()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            BufferObjectHelper.BindAndDrawBuffer(bufferId, textureId, 0, vertexCount, 
                Camera.Eye, Vector3.Zero, Vector3.One);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        internal void Cleanup()
        {
            Texture.UnloadTexture(textureId);
            BufferObjectHelper.DeleteBuffers(bufferId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class TerrainBufferObject
    {
        private Vector2 position;
        private uint bufferId;
        private uint indexArrayId;
        private int terrainTextureId;
        private int indexArrayLength;

        internal void InitializeTerrain(Vector2 grid, int size, float[][] heights, int textureId, Vector2 uv)
        {
            position = grid;
            terrainTextureId = textureId;
            // Generate the vertices for the terrain based on the inputs
            float sliceU = uv.X / size,
                  sliceV = uv.Y / size;
            Vertex[] vertices = new Vertex[size * size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                {
                    int currentIndex = i * size + j;
                    vertices[currentIndex] = new Vertex(
                        new Vector3(i, heights[i][j], -j),
                        new Vector3(0.0f, 1.0f, 0.0f),
                        new Vector2(i * sliceU, j * sliceV));
                }

            uint[] indices = new uint[6 * (size-1) * (size-1)];
            int pointIndex = 0;
            for (uint i = 0; i < size - 1; i++)
                for (uint j = 0; j < size - 1; j++)
                {
                    uint topLeft = (i + 1) * (uint)size + j,
                         topRight = topLeft + 1,
                         bottomLeft = i * (uint)size + j,
                         bottomRight = bottomLeft + 1;
                    indices[pointIndex++] = bottomLeft;
                    indices[pointIndex++] = topLeft;
                    indices[pointIndex++] = bottomRight;
                    indices[pointIndex++] = bottomRight;
                    indices[pointIndex++] = topLeft;
                    indices[pointIndex++] = topRight;
                }

            indexArrayLength = indices.Length;

            // Load the data into the buffer
            BufferObjectHelper.CreateBuffers(out bufferId, out indexArrayId);
            BufferObjectHelper.LoadDataIntoBuffers(bufferId, vertices, indexArrayId, indices);
        }

        internal void DrawTerrain()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            BufferObjectHelper.BindAndDrawBuffer(bufferId, indexArrayId, terrainTextureId, 0, indexArrayLength,
                new Vector3(position.X, 0, position.Y), Vector3.Zero, Vector3.One);
            GL.Disable(EnableCap.CullFace);
        }

        internal void Cleanup()
        {
            Texture.UnloadTexture(terrainTextureId);
            BufferObjectHelper.DeleteBuffers(bufferId);
        }
    }
}

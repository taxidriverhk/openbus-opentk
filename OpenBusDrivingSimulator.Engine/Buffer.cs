using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    internal enum BufferObjectDrawingMode
    {
        ALL = 0,
        COLOR_ONLY = 1,
        ALPHA_ONLY = 2,
        NON_ALPHA_ONLY = 3
    }

    internal delegate void ShaderProgramSetUniformsDelegate();

    /// <summary>
    /// 
    /// </summary>
    internal static class BufferHelper
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

    internal static class ShaderProgramHelper
    {
        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, int vertexCount, int startIndex, int textureId)
        {
            UseAndDrawBuffer(shader, bufferId, vertexCount, startIndex, new int[] { textureId }, 
                Matrix4.Identity, Camera.ProjectionMatrix, Matrix4.Identity, () => { });
        }

        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, int vertexCount, int startIndex, int[] textureIds,
            Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, ShaderProgramSetUniformsDelegate setUniforms)
        {
            shader.UseProgram();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);

            shader.BindVertexAttribPointer(ShaderAttribute.POSITION);
            shader.BindVertexAttribPointer(ShaderAttribute.NORMAL);
            shader.BindVertexAttribPointer(ShaderAttribute.TEXCOORD);

            shader.SetUniform(ShaderUniform.VIEW_MATRIX, viewMatrix);
            shader.SetUniform(ShaderUniform.PROJECTION_MATRIX, projectionMatrix);
            shader.SetUniform(ShaderUniform.MODEL_MATRIX, modelMatrix);
            setUniforms();

            for (int i = 0; i < textureIds.Length && i < 32; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, textureIds[i]);
            }
            GL.DrawArrays(PrimitiveType.Triangles, startIndex * Vertex.Size, vertexCount);

            shader.DisableAllVertexAttribArrays();
            shader.UnUseProgram();
        }

        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, uint indexArrayId, int indexArrayLength, int indexOffset, int textureId)
        {
            UseAndDrawBuffer(shader, bufferId, indexArrayId, indexArrayLength, indexOffset, new int[] { textureId },
                Camera.ViewMatrix, Camera.ProjectionMatrix, Matrix4.Identity, () => { });
        }

        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, uint indexArrayId, int indexArrayLength, int indexOffset, int textureId,
            Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 modelMatrix = GraphicsHelper.CreateModelMatrix(translation, rotation, scale);
            UseAndDrawBuffer(shader, bufferId, indexArrayId, indexArrayLength, indexOffset, new int[] { textureId },
                Camera.ViewMatrix, Camera.ProjectionMatrix, modelMatrix, () => { });
        }

        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, uint indexArrayId, int indexArrayLength, int indexOffset, int textureId,
            Vector3 translation, Vector3 rotation, Vector3 scale, ShaderProgramSetUniformsDelegate setUniforms)
        {
            Matrix4 modelMatrix = GraphicsHelper.CreateModelMatrix(translation, rotation, scale);
            UseAndDrawBuffer(shader, bufferId, indexArrayId, indexArrayLength, indexOffset, new int[] { textureId },
                Camera.ViewMatrix, Camera.ProjectionMatrix, modelMatrix, setUniforms);
        }

        internal static void UseAndDrawBuffer(ShaderProgram shader, uint bufferId, uint indexArrayId, int indexArrayLength, int indexOffset, int[] textureIds,
            Matrix4 viewMatrix, Matrix4 projectionMatrix, Matrix4 modelMatrix, ShaderProgramSetUniformsDelegate setUniforms)
        {
            shader.UseProgram();
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);

            shader.BindVertexAttribPointer(ShaderAttribute.POSITION);
            shader.BindVertexAttribPointer(ShaderAttribute.NORMAL);
            shader.BindVertexAttribPointer(ShaderAttribute.TEXCOORD);

            shader.SetUniform(ShaderUniform.VIEW_MATRIX, viewMatrix);
            shader.SetUniform(ShaderUniform.PROJECTION_MATRIX, projectionMatrix);
            shader.SetUniform(ShaderUniform.MODEL_MATRIX, modelMatrix);
            setUniforms();

            for (int i = 0; i < textureIds.Length && i < 32; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.BindTexture(TextureTarget.Texture2D, textureIds[i]);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexArrayId);
            GL.DrawElements(BeginMode.Triangles, indexArrayLength, DrawElementsType.UnsignedInt, indexOffset * sizeof(uint));

            shader.DisableAllVertexAttribArrays();
            shader.UnUseProgram();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class StaticVertexBuffer
    {
        private static readonly string VERTEX_SHADER_PATH = GameEnvironment.RootPath + @"shaders\staticVertexShader.glsl";
        private static readonly string FRAGMENT_SHADER_PATH = GameEnvironment.RootPath + @"shaders\staticFragmentShader.glsl";

        private ShaderProgram shader;
        private Light sun;

        private bool renderWithinBounds;

        private List<Entity> entities;
        private List<uint> bufferIds;
        private Dictionary<uint, uint> bufferIndexIdMapping;
        private Dictionary<Texture, uint> textureBufferMapping;
        private Dictionary<string, List<Texture>> meshTextureMapping;
        private Dictionary<Texture, KeyValuePair<int, int>> textureIndexMapping;

        internal StaticVertexBuffer()
        {
            entities = new List<Entity>();
            bufferIds = new List<uint>();
            bufferIndexIdMapping = new Dictionary<uint, uint>();
            textureBufferMapping = new Dictionary<Texture, uint>();
            meshTextureMapping = new Dictionary<string, List<Texture>>();
            textureIndexMapping = new Dictionary<Texture, KeyValuePair<int, int>>();

            shader = new ShaderProgram();
            shader.LoadShaderCodes(VERTEX_SHADER_PATH, FRAGMENT_SHADER_PATH);

            // Only render the entities where their bounds are within the bounding box
            // TODO: this member should be configurable
            renderWithinBounds = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitiesToBeLoaded"></param>
        /// <param name="meshes"></param>
        /// <param name="sunLight"></param>
        /// <returns></returns>
        internal int LoadEntities(List<Entity> entitiesToBeLoaded, ISet<Mesh> meshes, Light sunLight)
        {
            int meshesLoaded = 0;
            uint bufferId = 0,
                 indexBufferId = 0;

            BufferHelper.CreateBuffers(out bufferId, out indexBufferId);
            bufferIds.Add(bufferId);
            bufferIndexIdMapping.Add(bufferId, indexBufferId);

            uint maxArraySize = UInt32.MaxValue;
            List<Texture> texturesLoaded = new List<Texture>();
            List<Vertex> verticesToBeLoaded = new List<Vertex>();
            List<uint> indicesToBeLoaded = new List<uint>();

            entities = new List<Entity>(entitiesToBeLoaded);
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

                    Texture texture = mesh.Materials[i].Texture;
                    if (texture.TextureId == 0)
                        texture.TextureId = TextureManager.GetTextureId(texture);
                    textureIndexMapping.Add(texture,
                        new KeyValuePair<int, int>((int)startIndex, mesh.Indices[i].Length));
                    textureBufferMapping.Add(texture, bufferId);
                    texturesLoaded.Add(texture);
                }
                meshTextureMapping.Add(mesh.Name, texturesLoaded);
                meshesLoaded++;
                texturesLoaded = new List<Texture>();
            }

            // And also the shaders
            sun = sunLight;
            // Finally load everything into the buffer
            BufferHelper.LoadDataIntoBuffers(bufferId, verticesToBeLoaded.ToArray(),
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
                if (renderWithinBounds && !EntityInBoundingBox(entity))
                    continue;

                List<Texture> textures = meshTextureMapping[entity.MeshName];
                foreach (Texture texture in textures)
                {
                    if (mode == BufferObjectDrawingMode.ALPHA_ONLY
                        && !texture.HasAlpha)
                        continue;
                    else if (mode == BufferObjectDrawingMode.NON_ALPHA_ONLY
                        && texture.HasAlpha)
                        continue;

                    uint bufferId = textureBufferMapping[texture],
                         indexArrayId = bufferIndexIdMapping[bufferId];
                    int indexArrayOffset = textureIndexMapping[texture].Key,
                        indexArrayLength = textureIndexMapping[texture].Value;
                    if (mode == BufferObjectDrawingMode.COLOR_ONLY)
                        BufferHelper.BindAndDrawBuffer(bufferId, indexArrayId,
                            indexArrayOffset, indexArrayLength, entity.Color,
                            entity.Translation, entity.Rotation, Vector3.One);
                    else
                    {
                        ShaderProgramHelper.UseAndDrawBuffer(shader, bufferId, indexArrayId, indexArrayLength,
                            indexArrayOffset, texture.TextureId, entity.Translation, entity.Rotation, Vector3.One,
                            () =>
                            {
                                shader.SetLight("lightPos", "lightColor", "lightType", sun);
                            });
                    }
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
                BufferHelper.DeleteBuffers(targetBufferId, targetIndexId);
            }

            bufferIds.Clear();
            bufferIndexIdMapping.Clear();
            entities.Clear();
            textureBufferMapping.Clear();
            meshTextureMapping.Clear();
            textureIndexMapping.Clear();
        }

        private bool EntityInBoundingBox(Entity entity)
        {
            // TODO: should check the bounding box rather than just the origin
            // If all the 8 points of the bounding box is not within the plane
            // Then the entity is considered not within the bound
            Vector3 origin = entity.Translation;
            foreach (Vector4 plane in Camera.BoundingPlanes)
                if (plane.X * origin.X + plane.Y * origin.Y 
                    + plane.Z * origin.Z - plane.W < 0)
                    return false;
            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class DynamicVertexBuffer
    {

    }

    /// <summary>
    /// 
    /// </summary>
    internal class OverlayTextBuffer
    {
        private Bitmap textBmp;

        internal OverlayTextBuffer()
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
            int textTextureId = TextureManager.LoadTexture(textBmp, false);

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
            TextureManager.UnloadTexture(textTextureId);

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
    internal class MirrorBuffer
    {
        private Vector3 position;
        private Vector3 target;
        private Vector3 up;
        private float viewableDistance;

        private StaticVertexBuffer staticScene;
        private uint mirrorBufferId;
        private uint mirrorIndexId;
        private int mirrorIndexArrayLength;

        private uint frameBufferId;
        private uint rendererBufferId;
        private int mirrorTextureId;

        /// <summary>
        /// 
        /// </summary>
        internal MirrorBuffer()
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
        internal void InitializeMirror(Mesh mirrorMesh, StaticVertexBuffer scene)
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
            BufferHelper.CreateBuffers(out mirrorBufferId, out mirrorIndexId);
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
            BufferHelper.BindAndDrawBuffer(mirrorBufferId, mirrorIndexId,
                mirrorTextureId, 0, mirrorIndexArrayLength);
            GL.Disable(EnableCap.CullFace);
        }

        /// <summary>
        /// 
        /// </summary>
        internal void Cleanup()
        {
            BufferHelper.DeleteBuffers(mirrorBufferId, mirrorIndexId);
            GL.DeleteRenderbuffers(1, ref rendererBufferId);
            GL.DeleteTexture(mirrorTextureId);
            GL.DeleteFramebuffers(1, ref frameBufferId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class SkyBoxBuffer
    {
        private static readonly string VERTEX_SHADER_PATH = GameEnvironment.RootPath + @"shaders\skyBoxVertexShader.glsl";
        private static readonly string FRAGMENT_SHADER_PATH = GameEnvironment.RootPath + @"shaders\skyBoxFragmentShader.glsl";

        private ShaderProgram shader;
        private Texture texture;
        private uint bufferId;
        private int vertexCount;

        internal SkyBoxBuffer()
        {
            shader = new ShaderProgram();
            shader.LoadShaderCodes(VERTEX_SHADER_PATH, FRAGMENT_SHADER_PATH);
        }

        internal void LoadSkyBox(Mesh skyBoxMesh, Vector3 scale)
        {
            // Enlarge the skybox
            for (int i = 0; i < skyBoxMesh.Vertices[0].Length; i++)
            {
                skyBoxMesh.Vertices[0][i].Position.X *= scale.X;
                skyBoxMesh.Vertices[0][i].Position.Y *= scale.Y;
                skyBoxMesh.Vertices[0][i].Position.Z *= scale.Z;
            }

            // Create the buffer and texture needed for the skybox
            texture = skyBoxMesh.Materials[0].Texture;
            int textureId = TextureManager.LoadTexture(texture);
            texture.TextureId = textureId;
            BufferHelper.CreateBuffers(out bufferId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(skyBoxMesh.Vertices[0].Length * Vertex.Size),
                skyBoxMesh.Vertices[0], BufferUsageHint.StaticDraw);
            vertexCount = skyBoxMesh.Vertices[0].Length;
        }

        internal void ReplaceTextures(string textureFile)
        {
            TextureManager.UnloadTexture(texture.TextureId);
            int textureId = TextureManager.LoadTexture(textureFile);
            texture = new Texture(textureId, false, textureFile);
        }

        internal void DrawSkyBox()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            ShaderProgramHelper.UseAndDrawBuffer(shader, bufferId, vertexCount, 0, texture.TextureId);
            GL.Disable(EnableCap.CullFace);
            GL.Enable(EnableCap.DepthTest);
        }

        internal void Cleanup()
        {
            TextureManager.UnloadTexture(texture.TextureId);
            BufferHelper.DeleteBuffers(bufferId);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class TerrainBuffer
    {
        private static readonly string VERTEX_SHADER_PATH = GameEnvironment.RootPath + @"shaders\terrainVertexShader.glsl";
        private static readonly string FRAGMENT_SHADER_PATH = GameEnvironment.RootPath + @"shaders\terrainFragmentShader.glsl";

        private ShaderProgram shader;
        private Light sun;

        private Vector2 position;
        private int size;

        private uint bufferId;
        private uint indexArrayId;
        private int terrainTextureId;
        private int indexArrayLength;

        internal TerrainBuffer()
        {
            // Initialize the shader
            shader = new ShaderProgram();
            shader.LoadShaderCodes(VERTEX_SHADER_PATH, FRAGMENT_SHADER_PATH);
        }

        internal Vector2 Position
        {
            get { return position; }
        }

        internal int Size
        {
            get { return size; }
        }

        internal void InitializeTerrain(Vector2 grid, int terrainSize, float[][] heights, int textureId, Vector2 uv, Light sunLight)
        {
            sun = sunLight;
            position = grid;
            size = terrainSize;
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
            BufferHelper.CreateBuffers(out bufferId, out indexArrayId);
            BufferHelper.LoadDataIntoBuffers(bufferId, vertices, indexArrayId, indices);
        }

        internal void DrawTerrain()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            ShaderProgramHelper.UseAndDrawBuffer(shader, bufferId, indexArrayId, indexArrayLength, 
                0, terrainTextureId, new Vector3(position.X, 0, position.Y), Vector3.Zero, Vector3.One,
                () => 
                {
                    shader.SetLight("lightPos", "lightColor", "lightType", sun);
                });
            GL.Disable(EnableCap.CullFace);
        }

        internal void Cleanup()
        {
            TextureManager.UnloadTexture(terrainTextureId);
            BufferHelper.DeleteBuffers(bufferId);
        }
    }
}

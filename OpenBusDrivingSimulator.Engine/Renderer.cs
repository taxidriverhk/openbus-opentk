using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenBusDrivingSimulator.Engine
{
    /// <summary>
    /// Responsible for drawing objects and texts for the screen to show.
    /// </summary>
    public static class Renderer
    {
        // TODO: the sun color and position should be based on the game's time
        private static readonly Vector3 SUN_POSITION = new Vector3(500, 500, 500);
        private static readonly Vector3 SUN_COLOR = new Vector3(1.0f, 1.0f, 1.0f);
        private static Light sun;

        private static List<Entity> loadedEntities;
        private static StaticBufferObject staticBuffer;
        private static List<MirrorBufferObject> mirrorBuffers;
        private static OverlayTextObject overlayText;
        private static SkyBoxObject skyBox;
        private static TerrainBufferObject terrain;

        /// <summary>
        /// Initialize the components of the renderer, should be called before the main loop.
        /// Cleanup function must also be called after the main loop to cleanup everything.
        /// </summary>
        public static void Initialize()
        {
            loadedEntities = new List<Entity>();
            staticBuffer = new StaticBufferObject();
            overlayText = new OverlayTextObject();
            mirrorBuffers = new List<MirrorBufferObject>();
            skyBox = new SkyBoxObject();
            terrain = new TerrainBufferObject();
            sun = new Light(SUN_POSITION, SUN_COLOR, LightType.DIRECTIONAL);
        }

        /// <summary>
        /// Cleans up the memory and components associated to this renderer.
        /// Must be called after the main loop to cleanup everything.
        /// </summary>
        public static void Cleanup()
        {
            loadedEntities.Clear();
            staticBuffer.Cleanup();
            foreach (MirrorBufferObject mirrorBuffer in mirrorBuffers)
                mirrorBuffer.Cleanup();
            skyBox.Cleanup();
            terrain.Cleanup();
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
            overlayText.DrawText(text, left, top, color);
        }

        /// <summary>
        /// Renders 2D text with white foreground and black thin border on the screen.
        /// </summary>
        public static void DrawText(string text, int left, int bottom)
        {
            DrawText(text, left, bottom, Color.White);
        }

        /// <summary>
        /// Draws a red line from a point to another point
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="startZ"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        /// <param name="endZ"></param>
        public static void DrawLine(float startX, float startY, float startZ,
            float endX, float endY, float endZ)
        {
            GL.LineWidth(2.0f);
            GL.Color3(Vector3.UnitX);
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(startX, startY, startZ);
            GL.Vertex3(endX, endY, endZ);
            GL.End();
            GL.Color3(Vector3.One);
        }

        /// <summary>
        /// Draws the mouse ray created by a mouse click
        /// (This function should be used for debugging purpose only)
        /// </summary>
        public static void DrawMouseRay()
        {
            Ray mouseRay = Screen.MouseRay;
            if (mouseRay != null)
                DrawLine(mouseRay.StartPoint.X, mouseRay.StartPoint.Y, mouseRay.StartPoint.Z, 
                    mouseRay.EndPoint.X, mouseRay.EndPoint.Y, mouseRay.EndPoint.Z);
        }

        /// <summary>
        /// Gets the entity that was hit by the mouse cursor position
        /// </summary>
        /// <param name="mouseLocation"></param>
        /// <returns></returns>
        public static Entity GetHitEntity(Vector2 mouseLocation)
        {
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(EnableCap.DepthTest);
            staticBuffer.DrawEntities(BufferObjectDrawingMode.COLOR_ONLY);
            Vector3 color = GraphicsHelper.GetColorOfScreen(new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f));
            GL.Enable(EnableCap.DepthTest);
            return loadedEntities.Find(e => e.Color == color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static int LoadStaticEntitiesToScene(List<Entity> entities, ISet<Mesh> meshes)
        {
            loadedEntities = new List<Entity>(entities);
            return staticBuffer.LoadEntities(entities, meshes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mirrorMesh"></param>
        public static void LoadMirror(Mesh mirrorMesh)
        {
            MirrorBufferObject mirrorBuffer = new MirrorBufferObject();
            mirrorBuffer.InitializeMirror(mirrorMesh, staticBuffer);
            mirrorBuffers.Add(mirrorBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skyBoxMesh"></param>
        /// <param name="scale"></param>
        /// <param name="textureFile"></param>
        public static void LoadSkyBox(Mesh skyBoxMesh, float scale)
        {
            skyBox.LoadSkyBox(skyBoxMesh, new Vector3(scale), skyBoxMesh.Materials[0].TextureId);
        }

        public static void ReplaceSkyBox(string textureFile)
        {
            skyBox.ReplaceTextures(textureFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size"></param>
        /// <param name="heights"></param>
        /// <param name="textureFile"></param>
        /// <param name="u"></param>
        /// <param name="v"></param>
        public static void LoadTerrain(int x, int y, int size, float[][] heights, string textureFile, float u, float v)
        {
            int textureId = Texture.LoadTexture(textureFile);
            terrain.InitializeTerrain(new Vector2(x, y), size, heights, textureId, new Vector2(u, v), sun);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void DrawScene()
        {
            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawSkyBox();
            DrawStaticScene();
            DrawMirrors();
            DrawTerrain();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawStaticScene()
        {
            // !!WARNING!!
            // Although the code below is able to draw entities with transparent texture
            // while retaining the depth buffer, such operation is expensive.
            // Should find a better way to draw.
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            // Draw the scene with opaque entities contribute to the depth buffer
            GL.Disable(EnableCap.Blend);
            GL.AlphaFunc(AlphaFunction.Equal, 1.0f);
            staticBuffer.DrawEntities();

            // Draw the scene again with depth buffer read only, so the transparent entites cannot
            // contribute to the depth buffer
            GL.Enable(EnableCap.Blend);
            GL.DepthMask(false);
            GL.AlphaFunc(AlphaFunction.Less, 1.0f);
            staticBuffer.DrawEntities(BufferObjectDrawingMode.ALPHA_ONLY);

            // Set the states back
            GL.DepthMask(true);
            GL.Disable(EnableCap.AlphaTest);
            GL.Disable(EnableCap.Blend);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawMirrors()
        {
            foreach (MirrorBufferObject mirrorBuffer in mirrorBuffers)
                mirrorBuffer.DrawMirror();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawSkyBox()
        {
            skyBox.DrawSkyBox();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawTerrain()
        {
            terrain.DrawTerrain();
        }
    }
}
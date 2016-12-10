﻿using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenBus.Engine
{
    /// <summary>
    /// Responsible for drawing objects and texts for the screen to show.
    /// </summary>
    public static class Renderer
    {
        // TODO: the sun color and position should be based on the game's time
        private static readonly Vector3 SUN_POSITION = new Vector3(-5000, 5000, 5000);
        private static readonly Vector3 SUN_COLOR = new Vector3(1.0f, 0.99f, 0.95f);

        private static Light sun;

        private static List<Entity> currentEntityList;
        private static List<List<Entity>> loadedEntities;
        private static StaticVertexBuffer currentStaticBuffer;
        private static List<StaticVertexBuffer> staticBuffers;
        private static TerrainBuffer currentTerrainBuffer;
        private static List<TerrainBuffer> terrainBuffers;

        private static List<MirrorBuffer> mirrorBuffers;
        private static OverlayTextBuffer overlayText;
        private static SkyBoxBuffer skyBox;

        /// <summary>
        /// Initialize the components of the renderer, should be called before the main loop.
        /// Cleanup function must also be called after the main loop to cleanup everything.
        /// </summary>
        public static void Initialize()
        {
            loadedEntities = new List<List<Entity>>();
            staticBuffers = new List<StaticVertexBuffer>();
            terrainBuffers = new List<TerrainBuffer>();

            overlayText = new OverlayTextBuffer();
            mirrorBuffers = new List<MirrorBuffer>();
            skyBox = new SkyBoxBuffer();
            
            sun = new Light(SUN_POSITION, SUN_COLOR, LightType.DIRECTIONAL);
        }

        /// <summary>
        /// Cleans up the memory and components associated to this renderer.
        /// Must be called after the main loop to cleanup everything.
        /// </summary>
        public static void Cleanup()
        {
            foreach (List<Entity> entityList in loadedEntities)
                entityList.Clear();
            loadedEntities.Clear();
            foreach (StaticVertexBuffer staticBuffer in staticBuffers)
                staticBuffer.Cleanup();
            foreach (TerrainBuffer terrainBuffer in terrainBuffers)
                terrainBuffer.Cleanup();
            foreach (MirrorBuffer mirrorBuffer in mirrorBuffers)
                mirrorBuffer.Cleanup();
            skyBox.Cleanup();
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
            currentStaticBuffer.DrawEntities(BufferObjectDrawingMode.COLOR_ONLY);
            Vector3 color = GraphicsHelper.GetColorOfScreen(new Vector3(mouseLocation.X, mouseLocation.Y, 0.0f));
            GL.Enable(EnableCap.DepthTest);
            return currentEntityList.Find(e => e.Color == color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static int LoadStaticEntitiesToScene(List<Entity> entities, ISet<Mesh> meshes)
        {
            List<Entity> entityList = new List<Entity>(entities);
            if (currentEntityList == null)
                currentEntityList = entityList;
            loadedEntities.Add(entityList);
            StaticVertexBuffer staticBuffer = new StaticVertexBuffer();
            int meshesLoaded = staticBuffer.LoadEntities(entities, meshes, sun);
            staticBuffers.Add(staticBuffer);
            if (currentStaticBuffer == null)
                currentStaticBuffer = staticBuffer;
            return meshesLoaded;
        }

        /// <summary>
        /// 
        /// </summary>
        public static void UnloadStaticEntitiesFromScene()
        {
            // TODO: how to know which slot to unload
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mirrorMesh"></param>
        public static void LoadMirror(Mesh mirrorMesh)
        {
            MirrorBuffer mirrorBuffer = new MirrorBuffer();
            mirrorBuffer.InitializeMirror(mirrorMesh, currentStaticBuffer);
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
            skyBox.LoadSkyBox(skyBoxMesh, new Vector3(scale));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textureFile"></param>
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
            int textureId = TextureManager.LoadTexture(textureFile);
            TerrainBuffer terrainBuffer = new TerrainBuffer();
            terrainBuffer.InitializeTerrain(new Vector2(x, y), size, heights, textureId, new Vector2(u, v), sun);
            terrainBuffers.Add(terrainBuffer);
            if (currentTerrainBuffer == null)
                currentTerrainBuffer = terrainBuffer;
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
        internal static void UpdateCurrentBuffer()
        {
            int bufferIndex = 0;
            Vector3 currentPosition = Camera.Eye;
            foreach (TerrainBuffer terrainBuffer in terrainBuffers)
            {
                int size = terrainBuffer.Size;
                Vector2 terrainPosition = terrainBuffer.Position;
                if (currentPosition.X >= terrainPosition.X && currentPosition.X <= terrainPosition.X + size
                    && -currentPosition.Z >= terrainPosition.Y && -currentPosition.Z <= terrainPosition.Y + size)
                {
                    currentTerrainBuffer = terrainBuffer;
                    currentStaticBuffer = staticBuffers[bufferIndex];
                    currentEntityList = loadedEntities[bufferIndex];
                    return;
                }
                bufferIndex++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawStaticScene()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);
            foreach (StaticVertexBuffer staticBuffer in staticBuffers)
                if (staticBuffer != null)
                    staticBuffer.DrawEntities();
            GL.Disable(EnableCap.Blend);
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawMirrors()
        {
            foreach (MirrorBuffer mirrorBuffer in mirrorBuffers)
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
            foreach (TerrainBuffer terrainBuffer in terrainBuffers)
                if (terrainBuffer != null)
                    terrainBuffer.DrawTerrain();
        }
    }
}
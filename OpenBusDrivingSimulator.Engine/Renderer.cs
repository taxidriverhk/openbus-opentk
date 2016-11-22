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
        private static StaticBufferObject staticBuffer;
        private static List<MirrorBufferObject> mirrorBuffers;
        private static OverlayTextObject overlayText;
        private static SkyBoxObject skyBox;

        /// <summary>
        /// Initialize the components of the renderer, should be called before the main loop.
        /// Cleanup function must also be called after the main loop to cleanup everything.
        /// </summary>
        public static void Initialize()
        {
            staticBuffer = new StaticBufferObject();
            overlayText = new OverlayTextObject();
            mirrorBuffers = new List<MirrorBufferObject>();
            skyBox = new SkyBoxObject();
        }

        /// <summary>
        /// Cleans up the memory and components associated to this renderer.
        /// Must be called after the main loop to cleanup everything.
        /// </summary>
        public static void Cleanup()
        {
            staticBuffer.Cleanup();
            foreach (MirrorBufferObject mirrorBuffer in mirrorBuffers)
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
        /// 
        /// </summary>
        /// <param name="meshes"></param>
        /// <returns></returns>
        public static int LoadStaticMeshesToScene(List<Mesh> meshes)
        {
            return staticBuffer.LoadMeshes(meshes);
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

        public static void LoadSkyBox(Mesh skyBoxMesh, float scale, string textureFile)
        {
            skyBox.LoadSkyBox(skyBoxMesh, new Vector3(scale), textureFile);
        }

        public static void DrawScene()
        {
            GL.ClearColor(Color.Black);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawSkyBox();
            DrawStaticScene();
            DrawMirrors();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawStaticScene()
        {
            staticBuffer.DrawMeshes();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DrawMirrors()
        {
            foreach (MirrorBufferObject mirrorBuffer in mirrorBuffers)
                mirrorBuffer.DrawMirror();
        }

        private static void DrawSkyBox()
        {
            skyBox.DrawSkyBox();
        }
    }
}
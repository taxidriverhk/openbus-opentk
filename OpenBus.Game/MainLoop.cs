using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenBus.Common;
using OpenBus.Engine;

namespace OpenBus.Game
{
    /// <summary>
    /// Controls the main loop of a game.
    /// This includes receiving inputs from the player, updating the game state, and drawing the scene.
    /// </summary>
    public static class MainLoop
    {
        /// <summary>
        /// Starts the game loop. 
        /// </summary>
        public static void Start()
        {
            double totalTimeElapsedForHud = 0.0;

            Initialize();
            #region Test Calls
            Renderer.LoadSkyBox(
                Mesh.LoadFromCollada(GameEnvironment.RootPath + @"objects\test\models\sky.dae"), 450f);
            Game.LoadMap(GameEnvironment.RootPath + @"maps\Test Map\test.map");
            #endregion

            Screen.Show();
            while (true)
            {
                // Timing calculation
                double timeElapsed = Timer.TimeElapsed;
                totalTimeElapsedForHud += timeElapsed;
                if (totalTimeElapsedForHud >= 0.2)
                {
                    Game.FrameRate = 1 / timeElapsed;
                    totalTimeElapsedForHud = 0;
                }

                // Process inputs and update states
                Screen.HandleEvents();
                if (Screen.Closed)
                    break;
                UpdateState();
                
                // Render the state
                UpdateBlocks();
                DrawScene();

                // Update the screen with the new drawing
                Screen.SwapBuffers();
            }
            Cleanup();
        }

        private static void Cleanup()
        {
            Game.SaveAndClean();
            Renderer.Cleanup();
            TextureManager.UnloadAllTextures();
            Screen.Destroy();
        }

        private static void DrawScene()
        {
            Renderer.DrawScene();
            if (Game.ShowFrameRate)
                Renderer.DrawText(string.Format("{0:0.00} fps", Game.FrameRate), 0, 95);
        }

        private static void Initialize()
        {
            string iconPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                + Constants.PATH_DELIM + Constants.APPLICATION_ICON;
            if (!File.Exists(iconPath))
                iconPath = string.Empty;

            Game.Initialize();
            Screen.Initialize(Constants.DEFAULT_SCREEN_WIDTH,
                Constants.DEFAULT_SCREEN_HEIGHT, Constants.APPLICATION_NAME, iconPath);
            Renderer.Initialize();
            Camera.Initialize();
            ControlHandler.LoadControls();
            Timer.Start();
        }

        private static void UpdateBlocks()
        {
            Game.LoadOrUnloadBlocks();
            Game.LoadIntoBuffers();
        }

        private static void UpdateState()
        {
            ControlHandler.ProcessControls();
            Game.CurrentView.UpdateCamera();
        }
    }
}
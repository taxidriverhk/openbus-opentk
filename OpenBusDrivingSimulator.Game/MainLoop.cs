using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
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
            string iconPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) 
                +"\\" + Constants.APPLICATION_ICON;
            if (!File.Exists(iconPath))
                iconPath = string.Empty;

            Screen.Initialize(Constants.DEFAULT_SCREEN_WIDTH,
                Constants.DEFAULT_SCREEN_HEIGHT, Constants.APPLICATION_NAME, iconPath);
            Renderer.Initialize();
            Camera.Initialize();

            ControlHandler.LoadControls();

            #region Test Calls
            Renderer.LoadSkyBox(
                Mesh.LoadFromCollada(GameEnvironment.RootPath + @"objects\sky.dae"), 450f);
            Game.LoadMap(GameEnvironment.RootPath + @"map\test.map");
            #endregion

            Screen.Show();
            Timer.Start();
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

                // Update inputs

                // Process inputs and update states
                Screen.HandleEvents();
                if (Screen.Closed)
                    break;
                ControlHandler.ProcessControls();
                Camera.UpdateCamera();
                // Render the state
                Renderer.DrawScene();

                if (Game.ShowFrameRate)
                    Renderer.DrawText(string.Format("{0:0.00} fps", Game.FrameRate), 0, 95);
                Screen.SwapBuffers();
            }
            Renderer.Cleanup();
            Texture.UnloadAllTextures();
            Screen.Destroy();
        }
    }
}

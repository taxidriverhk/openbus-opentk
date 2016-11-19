using System;
using System.Collections.Generic;
using System.Linq;
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

            Screen.Initialize(Constants.DEFAULT_SCREEN_WIDTH, 
                Constants.DEFAULT_SCREEN_HEIGHT, Constants.APPLICATION_NAME);
            Renderer.Initialize();
            Camera.Initialize();

            ControlHandler.LoadControls();

            #region Test Calls
            Renderer.LoadStaticMeshesToScene(new List<Mesh>()
            {
                Mesh.LoadFromCollada(@"D:\Downloads\grandwaterfront.dae"),
                Mesh.LoadFromCollada(@"D:\Downloads\wuhu.dae")
            });
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
                Renderer.DrawStaticScene();
                //Renderer.DrawMirror();

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

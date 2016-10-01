// GameLoop.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
{
    /// <summary>
    /// Object that controls the game loop
    /// </summary>
    public static class GameLoop
    {
        /// <summary>
        /// Starts the game loop. 
        /// </summary>
        public static void Start()
        {
            double totalTimeElapsedForHud = 0.0;

            Screen.Initialize();
            Screen.Show();
            Texture.LoadTestTextures();
            Timer.Start();
            Renderer.Initialize();
            Camera.Initialize();
            while(true)
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
                Camera.RotateYBy(0.5f);
                Camera.UpdateCamera();

                // Render the state
                Renderer.RenderTestScene(timeElapsed);
                Renderer.DrawText(string.Format("{0:0.00} fps", Game.FrameRate), 0, 0);
                Screen.SwapBuffers();
            }
            Renderer.ClearBuffer();
            Texture.UnloadAllTextures();
            Screen.Destroy();
        }
    }
}

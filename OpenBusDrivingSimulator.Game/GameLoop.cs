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
            Screen.Initialize();
            Screen.Show();
            Texture.LoadTestTextures();
            Renderer.Initialize();
            Timer testTimer = Timer.Initialize();
            while(true)
            {
                if (Screen.Closed)
                    break;

                // Update inputs

                // Process inputs and update states
                Screen.HandleEvents();
                // Render the state
                Renderer.RenderTest(testTimer);
                Screen.SwapBuffers();
            }
            Texture.UnloadAllTextures();
            Screen.Destroy();
        }
    }
}

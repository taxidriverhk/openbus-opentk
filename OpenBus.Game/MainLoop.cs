using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenBus.Common;
using OpenBus.Engine;
using OpenBus.Game.Controls;

namespace OpenBus.Game
{
    /// <summary>
    /// Defines the start parameters of the main game loop.
    /// </summary>
    public class MainLoopStartParameter
    {
        /// <summary>
        /// Full path to the map's main directory.
        /// </summary>
        public string MapPath;

        /// <summary>
        /// 
        /// </summary>
        public static MainLoopStartParameter ReadProgramArgs(string[] args)
        {
            MainLoopStartParameter parameter = new MainLoopStartParameter();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i].ToLower();
                switch (arg)
                {
                    case "map":

                        break;
                }
            }

            return parameter;
        }
    }

    /// <summary>
    /// Controls the main loop of a game.
    /// This includes receiving inputs from the player, updating the game state, and drawing the scene.
    /// </summary>
    public static class MainLoop
    {
        private static string currentMapPath;
        private static double frameRate;
        private static double deltaTime;
        private static MainLoopStartParameter startParameter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapPath"></param>
        public static void SetParameters(MainLoopStartParameter parameter)
        {
            // TODO: add more parameters as we can
            currentMapPath = parameter.MapPath;
        }

        /// <summary>
        /// Starts the game loop. 
        /// </summary>
        public static void Start()
        {
            double deltaTimeForHud = 0.0;

            Initialize();

            Log.Write(LogLevel.Info, "Start to load map {0}.", currentMapPath);
            Game.LoadMap(currentMapPath);

            Screen.Show();
            while (true)
            {
                // Timing calculation
                deltaTime = Timer.DeltaTime;
                deltaTimeForHud += deltaTime;
                if (deltaTimeForHud >= 0.2)
                {
                    frameRate = 1 / deltaTime;
                    deltaTimeForHud = 0;
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

            CleanUp();
            Log.Write(LogLevel.Info, "Session on map {0} is ended.", true, currentMapPath);
        }

        private static void CleanUp()
        {
            Game.SaveAndClean();
            Renderer.CleanUp();
            TextureManager.UnloadAllTextures();
            Screen.Destroy();
        }

        private static void DrawScene()
        {
            Renderer.DrawScene();
            if (Game.Settings.ScreenDisplaySettings.ShowFrameRate)
                Renderer.DrawText(string.Format("{0:0.00} fps", frameRate), 0, 95);
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
            ControlHandler.ProcessControls(deltaTime);
            Game.UpdateState();
        }
    }
}
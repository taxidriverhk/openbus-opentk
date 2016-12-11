using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using OpenBus.Common;
using OpenBus.Config;
using OpenBus.Engine;
using OpenBus.Game.Objects;
using OpenBus.Game.Controls;

namespace OpenBus.Game
{
    public class ScreenDisplaySettings
    {
        public bool ShowFrameRate = true;

        public void ToggleFrameRateDisplay()
        {
            ShowFrameRate = !ShowFrameRate;
        }
    }

    public class MapDisplaySettings
    {
        public int MaxSimuBlocks = 9;
        public int BlockSize = 250;
    }

    public class GameSettings
    {
        private MapDisplaySettings mapDisplaySettings;
        private ScreenDisplaySettings screenDisplaySettings;

        public MapDisplaySettings MapDisplaySettings
        {
            get { return mapDisplaySettings; }
        }

        public ScreenDisplaySettings ScreenDisplaySettings
        {
            get { return screenDisplaySettings; }
        }

        public GameSettings()
        {
            // Load the default settings
            mapDisplaySettings = new MapDisplaySettings();
            screenDisplaySettings = new ScreenDisplaySettings();
        }

        public void LoadMapDisplaySettings()
        {

        }
    }

    public static class Game
    {
        private static GameSettings gameSettings;
        private static Map world;
        private static List<Bus> buses;
        private static View currentView;
        private static List<View> views;

        public static View CurrentView
        {
            get { return currentView; }
        }

        public static Map World
        {
            get { return world; }
        }

        public static GameSettings Settings
        {
            get { return gameSettings; }
        }

        static Game()
        {
            gameSettings = new GameSettings();
            buses = new List<Bus>();
            views = new List<View>();
        }

        public static void Initialize()
        {
            
        }

        /// <summary>
        /// Checks to see if the object configurations loaded can be loaded into
        /// the graphics buffers
        /// </summary>
        public static void LoadIntoBuffers()
        {
            MapBlockLoader.LoadIntoBuffer();
        }

        /// <summary>
        /// Loads or unloads blocks based on the player's current position
        /// </summary>
        public static void LoadOrUnloadBlocks()
        {

            // If the current block position is to be changed, then determine which
            // blocks should be loaded

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void LoadMap(string path)
        {
            world = ConfigLoader.LoadMap(path);
            if (world != null)
            {
                #region Test Code
                // TODO: load the starting block according to the config
                string mapDirectory = Path.GetDirectoryName(path);
                MapBlockInfo blockInfo = world.BlockInfoList[0];
                MapBlock block = ConfigLoader.LoadMapBlock(mapDirectory 
                    + Constants.PATH_DELIM + blockInfo.MapBlockToLoad, blockInfo.Position);
                Terrain terrain = ConfigLoader.LoadTerrain(mapDirectory 
                    + Constants.PATH_DELIM + blockInfo.TerrainToLoad, blockInfo.Position);
                if (block != null && terrain != null)
                    world.LoadBlock(blockInfo.Position.X, blockInfo.Position.Y, block, terrain);
                #endregion
                world.LoadCurrentSky();
            }

            // Load the free camera by default
            currentView = new View(ViewType.FREE);
            views.Add(currentView);
            Camera.UpdateCamera();
        }

        public static void SaveAndClean()
        {
            buses.Clear();
            views.Clear();
        }

        public static void UpdateCamera()
        {

        }

        private static void UnloadMap()
        {

        }
    }
}

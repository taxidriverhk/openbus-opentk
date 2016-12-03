using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;
using OpenBus.Config;
using OpenBus.Engine;

namespace OpenBus.Game
{
    public static class Game
    {
        private static Map world;
        private static List<Bus> buses;
        private static View currentView;
        private static List<View> views;

        public static bool ShowFrameRate = true;
        public static double FrameRate = 0.0;

        public static View CurrentView
        {
            get { return currentView; }
        }

        public static Map World
        {
            get { return world; }
        }

        static Game()
        {
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
                MapBlockInfo blockInfo = world.BlockInfoList[0];
                MapBlock block = ConfigLoader.LoadMapBlock(GameEnvironment.RootPath 
                    + "maps\\Test Map\\" + blockInfo.MapBlockToLoad, blockInfo.Position);
                Terrain terrain = ConfigLoader.LoadTerrain(GameEnvironment.RootPath 
                    + "maps\\Test Map\\" + blockInfo.TerrainToLoad, blockInfo.Position);
                if (block != null && terrain != null)
                    world.LoadBlock(blockInfo.Position.X, blockInfo.Position.Y, block, terrain);
                #endregion
            }

            // Loads the free camera by default
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

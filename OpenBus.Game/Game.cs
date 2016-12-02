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
        private static List<Bus> buses = new List<Bus>();

        public static bool ShowFrameRate = true;
        public static double FrameRate = 0.0;

        public static Map World
        {
            get { return world; }
        }

        public static List<Bus> Buses
        {
            get { return buses; }
        }

        public static void LoadIntoBuffers()
        {
            MapBlockLoader.LoadIntoBuffer();
        }

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
        }

        private static void UnloadMap()
        {

        }
    }
}

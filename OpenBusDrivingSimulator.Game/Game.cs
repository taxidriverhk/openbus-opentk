using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Config;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
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
            MapBlockLoader.LoadInfoBuffer();
        }

        public static void LoadMap(string filename)
        {
            world = new Map();
            MapEx mapLoaded = XmlDeserializeHelper<MapEx>.DeserializeFromFile(filename);
            if (mapLoaded != null)
            {
                #region Test Code
                // TODO: load the starting block according to the config
                MapEx.BlockInfo blockInfo = mapLoaded.Blocks[0];
                MapBlockEx block = XmlDeserializeHelper<MapBlockEx>.DeserializeFromFile(
                    GameEnvironment.RootPath + "map\\" + blockInfo.Path);
                TerrainEx terrain = XmlDeserializeHelper<TerrainEx>.DeserializeFromFile(
                    GameEnvironment.RootPath + "map\\" + blockInfo.TerrainPath);
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

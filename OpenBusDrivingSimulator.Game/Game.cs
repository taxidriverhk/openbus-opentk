using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Config;

namespace OpenBusDrivingSimulator.Game
{
    public static class GameEnvironment
    {
        public static string RootPath = @"D:\Downloads\OpenBDS\";
    }

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

        public static void LoadMap(string filename)
        {
            world = new Map();
            MapEx mapLoaded = XmlDeserializeHelper<MapEx>.DeserializeFromFile(filename);
            if (mapLoaded != null)
            {
                #region Test Code
                // TODO: load the starting block according to the config
                MapEx.BlockInfo blockInfo = mapLoaded.Blocks[0];
                MapBlockEx block = XmlDeserializeHelper<MapBlockEx>.DeserializeFromFile(GameEnvironment.RootPath + "map\\" + blockInfo.Path);
                if (block != null)
                    world.LoadBlock(block);
                #endregion
            }
        }

        private static void UnloadMap()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using OpenBus.Common;
using OpenBus.Game.Controls;
using OpenBus.Game.Objects;

namespace OpenBus.WPF.Model
{
    public class MainWindowModel
    {
        public class MapListItem
        {
            public string Name { get; set; }
            public string Path { get; set; }

            public MapListItem(string name, string path)
            {
                this.Name = name;
                this.Path = path;
            }
        }

        private List<MapListItem> mapList;

        public MainWindowModel()
        {
            GetMapList();
        }

        public List<MapListItem> MapList
        {
            get { return mapList; }
        }

        private void GetMapList()
        {
            // Get all sub-folders under map folder
            string[] mapFolders = Directory.GetDirectories(EnvironmentVariables.MapPath);
            List<string> mapConfigsToShow = new List<string>();
            foreach (string mapFolder in mapFolders)
            {
                // Add the .map file to the list
                string[] mapConfigFiles = Directory.GetFiles(mapFolder, "*.map");
                if (mapConfigFiles.Length > 0)
                    mapConfigsToShow.Add(mapConfigFiles[0]);
            }

            // Read the map files and then load their info
            mapList = new List<MapListItem>();
            foreach (string mapConfig in mapConfigsToShow)
            {
                MapInfo mapInfo = ConfigLoader.LoadMapInfo(mapConfig);
                mapList.Add(new MapListItem(mapInfo.Name, mapInfo.Path));
            }
        }
    }
}

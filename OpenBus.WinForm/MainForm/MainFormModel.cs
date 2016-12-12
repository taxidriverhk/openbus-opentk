using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenBus.Common;
using OpenBus.Game.Controls;
using OpenBus.Game.Objects;

namespace OpenBus.WinForm
{
    public class MainFormModel
    {
        private List<MapInfo> mapList;

        public string[] MapNameList
        {
            get
            {
                string[] mapNameList = new string[mapList.Count];
                for (int i = 0; i < mapNameList.Length; i++)
                    mapNameList[i] = mapList[i].Name;
                return mapNameList;
            }
        }

        public MainFormModel()
        {
            LoadMapList();
        }

        public string GetMapPath(string mapName)
        {
            return mapList.Find(m => m.Name == mapName).Path;
        }

        private void LoadMapList()
        {
            mapList = new List<MapInfo>();
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
            foreach (string mapConfig in mapConfigsToShow)
            {
                MapInfo mapInfo = ConfigLoader.LoadMapInfo(mapConfig);
                mapList.Add(mapInfo);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace OpenBusDrivingSimulator.Config
{
    [XmlRoot("block")]
    public class MapBlockEx
    {
        [XmlElement("position")]
        public MapBlockPosition Position;
        [XmlArray("objects")]
        [XmlArrayItem("object")]
        public ObjectInfo[] Objects;
        // TODO: add splines, terrain and other things
    }

    public class MapBlockPosition
    {
        [XmlAttribute("x")]
        public int X;
        [XmlAttribute("y")]
        public int Y;
    }

    [XmlRoot("map")]
    public class MapEx
    {
        public class Terrain
        {
            [XmlElement("position")]
            public MapBlockPosition Block;
            // TODO: add terrain data
        }

        public class BlockInfo
        {
            [XmlElement("path")]
            public string Path;
            [XmlElement("terrain")]
            public string TerrainFileName;
            [XmlElement("position")]
            public MapBlockPosition Position;
        }

        public class MapInfo
        {
            [XmlElement("name")]
            public string Name;
        }

        [XmlElement("info")]
        public MapInfo Info;
        [XmlArray("blocks")]
        [XmlArrayItem("block")]
        public BlockInfo[] Blocks;
    }
}

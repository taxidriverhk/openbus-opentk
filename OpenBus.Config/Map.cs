using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenBus.Common;

namespace OpenBus.Config
{
    [XmlRoot("terrain")]
    public class TerrainEx
    {
        public class TerrainDisplacement
        {
            [XmlAttribute("x")]
            public int X;
            [XmlAttribute("y")]
            public int Y;
            [XmlAttribute("displacement")]
            public int Displacement;
        }

        public class TerrainTexture
        {
            [XmlElement("path")]
            public string Path;
            [XmlElement("uv")]
            public Vector2f UV;
        }

        [XmlElement("texture")]
        public TerrainTexture Texture;
        [XmlArray("displacements")]
        [XmlArrayItem("displacement")]
        public TerrainDisplacement[] Displacements;
    }

    [XmlRoot("block")]
    public class MapBlockEx
    {
        [XmlArray("objects")]
        [XmlArrayItem("object")]
        public ObjectInfo[] Objects;
        // TODO: add splines, terrain and other things
    }

    [XmlRoot("map")]
    public class MapEx
    {
        public class BlockInfo
        {
            public class MapBlockPosition
            {
                [XmlAttribute("x")]
                public int X;
                [XmlAttribute("y")]
                public int Y;
            }

            [XmlElement("path")]
            public string Path;
            [XmlElement("terrain")]
            public string TerrainPath;
            [XmlElement("position")]
            public MapBlockPosition Position;
        }

        public class MapInfo
        {
            [XmlElement("name")]
            public string Name;
        }

        public class SkyInfo
        {
            public enum SkyTextureMode
            {
                [XmlEnum("day")]
                Day,
                [XmlEnum("night")]
                Night,
                [XmlEnum("dawn")]
                Dawn,
                [XmlEnum("sunset")]
                Sunset
            }

            public class SkyTexture
            {
                [XmlAttribute("mode")]
                public SkyTextureMode Mode;
                [XmlAttribute("path")]
                public string Path;
            }

            [XmlElement("size")]
            public int Size;
            [XmlArray("textures")]
            [XmlArrayItem("texture")]
            public SkyTexture[] Textures;
        }

        [XmlElement("info")]
        public MapInfo Info;
        [XmlElement("sky")]
        public SkyInfo Sky;
        [XmlArray("blocks")]
        [XmlArrayItem("block")]
        public BlockInfo[] Blocks;
    }
}

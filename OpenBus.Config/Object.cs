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
    [XmlRoot("object")]
    public class ObjectConfig
    {
        public class ObjectInfo
        {
            [XmlElement("name")]
            public string Name;
            [XmlElement("model_dir")]
            public string ModelDirectory;
            [XmlElement("texture_dir")]
            public string TextureDirectory;
        }

        public class AlphaTextureInfo
        {
            public enum AlphaTextureMode
            {
                [XmlEnum("full")]
                Full = 0,
                [XmlEnum("alpha")]
                Alpha = 1
            }

            [XmlAttribute("path")]
            public string Path;
            [XmlAttribute("mode")]
            public AlphaTextureMode Mode;
        }

        public class MeshInfo
        {
            [XmlAttribute("path")]
            public string Path;
        }

        [XmlElement("info")]
        public ObjectInfo Info;
        [XmlArray("meshes")]
        [XmlArrayItem("mesh")]
        public MeshInfo[] Meshes;
        [XmlArray("alpha_textures")]
        [XmlArrayItem("alpha_texture")]
        public AlphaTextureInfo[] AlphaTextures;
    }

    public class ObjectInfo
    {
        [XmlElement("path")]
        public string Path;
        [XmlElement("position")]
        public Vector3f Position;
        [XmlElement("rotations")]
        public Vector3f Rotations;
    }
}

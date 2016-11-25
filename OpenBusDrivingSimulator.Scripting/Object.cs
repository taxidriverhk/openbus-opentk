using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Config
{
    [XmlRoot("object")]
    public class ObjectEx
    {
        public class AlphaTextureInfo
        {
            public enum AlphaTextureMode
            {
                [XmlEnum("0")]
                FULL = 0,
                [XmlEnum("1")]
                ALPHA = 1
            }

            [XmlElement("path")]
            public string Path;
            [XmlElement("mode")]
            public AlphaTextureMode Mode;
        }

        public class MeshInfo
        {
            [XmlElement("path")]
            public string Path;
        }

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

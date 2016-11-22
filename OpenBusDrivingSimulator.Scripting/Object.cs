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
        public class MeshInfo
        {
            [XmlElement("path")]
            public string Path;
        }

        [XmlArray("meshes")]
        [XmlArrayItem("mesh")]
        public MeshInfo[] Meshes;
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

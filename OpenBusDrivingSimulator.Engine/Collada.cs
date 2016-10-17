using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OpenTK;

namespace OpenBusDrivingSimulator.Engine
{
    public static class ColladaXmlSerializer
    {
        public static Collada DeserializeFromXml(string path)
        {
            // Serialize part of the file with XmlSerializer first
            XmlSerializer serializer = new XmlSerializer(typeof(Collada));
            Collada collada = serializer.Deserialize(new StreamReader(path)) as Collada;

            // Then read the rest with xpath
            XmlDocument colladaXmlDoc = new XmlDocument();
            colladaXmlDoc.Load(path);
            XmlNode root = colladaXmlDoc.DocumentElement;

            XmlNamespaceManager xmlNsMgr = new XmlNamespaceManager(colladaXmlDoc.NameTable);
            xmlNsMgr.AddNamespace("collada", Collada.COLLADA_SCHEMA);

            // Get the shading for each effect
            int effectIndex = 0;
            XmlNodeList effectNodes = root.SelectNodes("collada:library_effects/collada:effect", xmlNsMgr);
            foreach (XmlNode effectNode in effectNodes)
            {
                XmlNodeList phongNodes = effectNode.SelectNodes("*/collada:technique/collada:phong/*", xmlNsMgr);
                PhongShading phongShading = new PhongShading();
                foreach (XmlNode phongNode in phongNodes)
                {
                    switch (phongNode.Name)
                    {
                        case "emission":
                        case "ambient":
                        case "diffuse":
                        case "specular":
                            string[] colorStr = phongNode.SelectSingleNode("collada:color", xmlNsMgr).
                                InnerXml.Split(' ');
                            Vector4 color = new Vector4(Convert.ToSingle(colorStr[0]),
                                Convert.ToSingle(colorStr[1]),
                                Convert.ToSingle(colorStr[2]),
                                Convert.ToSingle(colorStr[3]));
                            switch (phongNode.Name)
                            {
                                case "emission":
                                    phongShading.Emission = color;
                                    break;
                                case "ambient":
                                    phongShading.Ambient = color;
                                    break;
                                case "diffuse":
                                    phongShading.Diffuse = color;
                                    break;
                                case "specular":
                                    phongShading.Specular = color;
                                    break;
                            }
                            break;
                        case "shininess":
                        case "index_of_refraction":
                            float floatValue = Convert.ToSingle(phongNode.
                                SelectSingleNode("collada:float", xmlNsMgr).InnerText);
                            switch(phongNode.Name)
                            {
                                case "shininess":
                                    phongShading.Shininess = floatValue;
                                    break;
                                case "index_of_refraction":
                                    phongShading.IndexOfRefraction = floatValue;
                                    break;
                            }
                            break;
                    }
                }
                collada.Effects[effectIndex].Shading = phongShading;
                effectIndex++;
            }

            // Finally get the geometries
            int geometryIndex = 0;
            XmlNodeList geometryNodes = root.SelectNodes("collada:library_geometries/collada:geometry", xmlNsMgr);
            collada.Geometries = new ColladaGeometry[geometryNodes.Count];
            foreach(XmlNode geometryNode in geometryNodes)
            {
                collada.Geometries[geometryIndex] = new ColladaGeometry();
                collada.Geometries[geometryIndex].Id = geometryNode.Attributes[0].Value;
                collada.Geometries[geometryIndex].Name = geometryNode.Attributes[1].Value;
                // Process float arrays
                XmlNodeList floatArrayNodes = geometryNode.SelectNodes("//collada:float_array", xmlNsMgr);
                foreach(XmlNode floatArrayNode in floatArrayNodes)
                {
                    string arrayId = floatArrayNode.Attributes[0].Value;
                    int arraySize = Convert.ToInt32(floatArrayNode.Attributes[1].Value);
                    string[] floatStrArray = floatArrayNode.InnerText.Split(ColladaGeometry.ARRAY_DELIM);
                    if(arrayId.Contains("map"))
                    {
                        Vector2[] vectorArray = new Vector2[arraySize / ColladaGeometry.TWOD_ARRAY_SIZE];
                        for (int i = 0, j = 0;
                            i < vectorArray.Length && j < floatStrArray.Length; 
                            i++, j += ColladaGeometry.TWOD_ARRAY_SIZE)
                            vectorArray[i] = new Vector2(Convert.ToSingle(floatStrArray[j]),
                                Convert.ToSingle(floatStrArray[j + 1]));
                        collada.Geometries[geometryIndex].Map = vectorArray;
                    }
                    else
                    {
                        Vector3[] vectorArray = new Vector3[arraySize / ColladaGeometry.THREED_ARRAY_SIZE];
                        for (int i = 0, j = 0; 
                            i < vectorArray.Length && j < floatStrArray.Length; 
                            i++, j += ColladaGeometry.THREED_ARRAY_SIZE)
                            vectorArray[i] = new Vector3(Convert.ToSingle(floatStrArray[j]),
                                Convert.ToSingle(floatStrArray[j + 1]),
                                Convert.ToSingle(floatStrArray[j + 2]));
                        if(arrayId.Contains("position"))
                            collada.Geometries[geometryIndex].Positions = vectorArray;
                        else if(arrayId.Contains("normal"))
                            collada.Geometries[geometryIndex].Normals = vectorArray;
                    }
                }

                // Process indices
                XmlNodeList polyNodes = geometryNode.SelectNodes("//collada:polylist", xmlNsMgr);
                collada.Geometries[geometryIndex].Indices = new Dictionary<string,int[]>();
                foreach(XmlNode polyNode in polyNodes)
                {
                    string materialId = polyNode.Attributes[0].Value;
                    string[] indexStrArray = polyNode.SelectSingleNode("collada:p", xmlNsMgr)
                        .InnerText.Split(ColladaGeometry.ARRAY_DELIM);
                    collada.Geometries[geometryIndex].Indices.Add(materialId, new int[indexStrArray.Length]);
                    for (int i = 0; i < indexStrArray.Length; i++)
                        collada.Geometries[geometryIndex].Indices[materialId][i] = Convert.ToInt32(indexStrArray[i]);
                }

                geometryIndex++;
            }

            return collada;
        }
    }

    [XmlRootAttribute("COLLADA", 
        Namespace = Collada.COLLADA_SCHEMA)]
    public class Collada
    {
        public const string COLLADA_SCHEMA = "http://www.collada.org/2005/11/COLLADASchema";

        [XmlElement("asset")]
        public ColladaAsset Asset;
        [XmlArray("library_effects")]
        [XmlArrayItem("effect")]
        public ColladaEffect[] Effects;
        [XmlArray("library_materials")]
        [XmlArrayItem("material")]
        public ColladaMaterial[] Materials;
        [XmlIgnore]
        public ColladaGeometry[] Geometries;
    }

    public class ColladaAsset
    {
        [XmlElement("unit")]
        public ColladaUnit Unit;
        [XmlElement("up_axis")]
        public string UpAxis;
    }

    public class ColladaUnit
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlAttribute("meter")]
        public int Meter;
    }

    public class ColladaEffect
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlIgnore]
        public PhongShading Shading;
    }

    public class ColladaMaterial
    {
        [XmlAttribute("id")]
        public string Id;
        [XmlAttribute("name")]
        public string Name;
    }

    public class ColladaGeometry
    {
        public const char ARRAY_DELIM = ' ';
        public const int TWOD_ARRAY_SIZE = 2;
        public const int THREED_ARRAY_SIZE = 3;

        public string Id;
        public string Name;
        public Vector3[] Positions;
        public Vector3[] Normals;
        public Vector2[] Map;
        public Dictionary<string, int[]> Indices;
    }
}
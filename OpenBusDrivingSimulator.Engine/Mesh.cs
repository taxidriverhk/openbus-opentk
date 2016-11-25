using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine.Assets;

namespace OpenBusDrivingSimulator.Engine
{
    public struct Entity
    {
        private static int currentId = 0;

        public string MeshName;
        public Vector3 Translation;
        public Vector3 Rotation;

        public int Id;
        public Vector3 Color;

        public Entity(string meshName, float tx, float ty, float tz, float rx, float ry, float rz)
        {
            MeshName = meshName;
            Translation = new Vector3(tx, ty, tz);
            Rotation = new Vector3(rx, ry, rz);

            Id = currentId;
            int r = (Id & 0x000000FF) >> 0,
                g = (Id & 0x0000FF00) >> 8,
                b = (Id & 0x00FF0000) >> 16;
            Color = new Vector3(r/255f, g/255f, b/255f);
            currentId++;
        }
    }

    public class Mesh
    {
        // TODO: make the path of the texture file configurable
        private string texturePath = @"D:\Downloads\OpenBDS\objects\texture";

        public string Name;
        public Material[] Materials;
        public Vertex[][] Vertices; 
        public uint[][] Indices;

        public static Mesh LoadFromCollada(string path)
        {
            return LoadFromCollada(path, null);
        }

        public static Mesh LoadFromCollada(string path, ISet<string> alphaTextures)
        {
            Collada collada = null;
            try
            {
                collada = ColladaXmlSerializer.DeserializeFromXml(path);
            }
            catch (Exception ex)
            {
                Log.Write(LogLevel.ERROR, "Unable to read Collada file {0}.", path);
                Log.Write(LogLevel.DEBUG, "The exception thrown was {0}:\n {1}", ex.Message, ex.StackTrace);
                return null;
            }
            
            Mesh resultMesh = new Mesh();
            // Get the name of the mesh
            resultMesh.Name = path.Replace(GameEnvironment.RootPath, "");
            // Used for mapping the material id to the array index
            Dictionary<string, int> materialMapping = new Dictionary<string, int>();
            resultMesh.Materials = new Material[collada.Materials.Length];
            for (int i = 0; i < collada.Materials.Length; i++)
            {
                resultMesh.Materials[i] = new Material();
                resultMesh.Materials[i].Name = collada.Materials[i].Name;
                resultMesh.Materials[i].Shading = collada.Effects[i].Shading;
                materialMapping.Add(collada.Materials[i].Id, i);
            }

            for (int i = 0; i < collada.Images.Length && i < collada.Materials.Length; i++)
            {
                string fullPath = resultMesh.texturePath
                    + Constants.PATH_DELIM + collada.Images[i].ImageFile;
                if (alphaTextures != null && alphaTextures.Contains(collada.Images[i].ImageFile))
                    resultMesh.Materials[i].TextureId = Texture.LoadTexture(fullPath, true);
                else
                    resultMesh.Materials[i].TextureId = Texture.LoadTexture(fullPath);
            }
                

            resultMesh.Vertices = new Vertex[resultMesh.Materials.Length][];
            for (int i = 0; i < collada.Geometries.Length; i++)
            {
                foreach (string materialId in collada.Geometries[i].Indices.Keys)
                {
                    int materialIndex = materialMapping[materialId];
                    List<Vertex> vertexList = new List<Vertex>();
                    int[] currentIndices = collada.Geometries[i].Indices[materialId];
                    for (int j = 0; j < currentIndices.Length; j += 3)
                    {
                        vertexList.Add(new Vertex(
                            collada.Geometries[i].Positions[currentIndices[j]],
                            collada.Geometries[i].Normals[currentIndices[j + 1]],
                            collada.Geometries[i].Map[currentIndices[j + 2]]));
                    }
                    resultMesh.Vertices[materialIndex] = vertexList.ToArray();
                }
            }

            resultMesh.Indices = new uint[resultMesh.Vertices.Length][];
            for (int i = 0; i < resultMesh.Vertices.Length; i++)
            {
                List<uint> indexList = new List<uint>();
                for (uint j = 0; j < resultMesh.Vertices[i].Length; j++)
                    indexList.Add(j);
                resultMesh.Indices[i] = indexList.ToArray();
            }

            return resultMesh;
        }

        public bool Validate()
        {
            // TODO: implement this function
            return true;
        }

        public override bool Equals(object obj)
        {
            Mesh other = obj as Mesh;
            if (other == null)
                return false;
            else
                return this.Name == other.Name;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private Mesh()
        {

        }
    }
}

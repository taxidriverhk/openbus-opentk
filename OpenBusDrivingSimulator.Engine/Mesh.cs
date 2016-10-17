using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    public class Mesh
    {
        public Material[] Materials;
        public List<Vertex>[] Vertices; 
        public List<int>[] Indices;

        public static Mesh LoadFromCollada(string path)
        {
            Collada collada = null;
            try
            {
                collada = ColladaXmlSerializer.DeserializeFromXml(path);
            }
            catch(Exception ex)
            {
                Log.Write(LogLevel.ERROR, "Unable to read Collada file {0}.", path);
                Log.Write(LogLevel.DEBUG, "The exception thrown was {0}:\n {1}", ex.Message, ex.StackTrace);
            }
            
            Mesh resultMesh = new Mesh();

            // Used for mapping the material id to the array index
            Dictionary<string, int> materialMapping = new Dictionary<string, int>();
            resultMesh.Materials = new Material[collada.Materials.Length];
            for (int i = 0; i < collada.Materials.Length; i++)
            {
                resultMesh.Materials[i] = new Material();
                resultMesh.Materials[i].Name = collada.Materials[i].Name;
                // TODO: make the path of the texture file configurable
                resultMesh.Materials[i].TextureId = Texture.LoadTextureFromFile(@"texture\" + resultMesh.Materials[i].Name + ".bmp");
                resultMesh.Materials[i].Shading = collada.Effects[i].Shading;
                materialMapping.Add(collada.Materials[i].Id, i);
            }

            resultMesh.Vertices = new List<Vertex>[resultMesh.Materials.Length];
            for(int i = 0; i < collada.Geometries.Length; i++)
            {
                foreach(string materialId in collada.Geometries[i].Indices.Keys)
                {
                    int materialIndex = materialMapping[materialId];
                    resultMesh.Vertices[materialIndex] = new List<Vertex>();
                    int[] currentIndices = collada.Geometries[i].Indices[materialId];
                    for (int j = 0; j < currentIndices.Length; j += 3)
                    {
                        resultMesh.Vertices[materialIndex].Add(new Vertex(
                            collada.Geometries[i].Positions[currentIndices[j]],
                            collada.Geometries[i].Normals[currentIndices[j + 1]],
                            collada.Geometries[i].Map[currentIndices[j + 2]]));
                    }
                }
            }

            resultMesh.Indices = new List<int>[resultMesh.Vertices.Length];
            for (int i = 0; i < resultMesh.Vertices.Length; i++)
            {
                resultMesh.Indices[i] = new List<int>();
                for (int j = 0; j < resultMesh.Vertices[i].Count; j++)
                    resultMesh.Indices[i].Add(j);
            }

            return resultMesh;
        }

        private Mesh()
        {

        }
    }
}

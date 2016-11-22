using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine.Assets;

namespace OpenBusDrivingSimulator.Engine
{
    public class Mesh
    {
        // TODO: make the path of the texture file configurable
        private string texturePath = @"D:\Downloads\OpenBDS\objects\texture";

        public Material[] Materials;
        public Vertex[][] Vertices; 
        public uint[][] Indices;

        public static Mesh LoadFromCollada(string path)
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
                resultMesh.Materials[i].TextureId = Texture.LoadTextureFromFile(resultMesh.texturePath
                    + Constants.PATH_DELIM + collada.Images[i].ImageFile);

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

        public void Translate(float x, float y, float z)
        {
            for (int i = 0; i < Vertices.Length; i++)
                for (int j = 0; j < Vertices[i].Length; j++)
                {
                    Vertices[i][j].Position.X += x;
                    Vertices[i][j].Position.Y += y;
                    Vertices[i][j].Position.Z += z;
                }
        }

        public void RotateY(float degrees)
        {
            float radians = MathHelper.DegreesToRadians(degrees),
                  cosTheta = (float)Math.Cos(radians),
                  sinTheta = (float)Math.Sin(radians);
            for (int i = 0; i < Vertices.Length; i++)
                for (int j = 0; j < Vertices[i].Length; j++)
                {
                    float x = Vertices[i][j].Position.X,
                          z = Vertices[i][j].Position.Z;
                    Vertices[i][j].Position.X = x * cosTheta - z * sinTheta;
                    Vertices[i][j].Position.Z = z * cosTheta + x * sinTheta;
                }
        }

        public bool Validate()
        {
            // TODO: implement this function
            return true;
        }

        private Mesh()
        {

        }
    }
}

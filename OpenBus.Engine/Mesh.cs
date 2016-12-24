using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK;
using OpenBus.Common;
using OpenBus.Engine.Assets;

namespace OpenBus.Engine
{
    public enum MeshFormat
    {
        Collada,
        Wavefront,
        DirectX,
        Autodesk3ds
    }

    public struct Entity
    {
        private static int currentId = 0;

        public string MeshName;
        public Vector3 Translation;
        public Vector3 Rotation;

        public int Id;
        public Vector3 Color;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="meshName">A unique name that identifies the mesh</param>
        /// <param name="tx"></param>
        /// <param name="ty"></param>
        /// <param name="tz"></param>
        /// <param name="rx">Rotation about x-axis in degrees.</param>
        /// <param name="ry">Rotation about y-axis in degrees.</param>
        /// <param name="rz">Rotation about z-axis in degrees.</param>
        public Entity(string meshName, float tx, float ty, float tz, float rx, float ry, float rz)
        {
            MeshName = meshName;
            Translation = new Vector3(tx, ty, tz);
            // Conver the angles into radians
            float rxRadians = Common.MathHelper.DegreesToRadians(rx),
                  ryRadians = Common.MathHelper.DegreesToRadians(ry),
                  rzRadians = Common.MathHelper.DegreesToRadians(rz);
            Rotation = new Vector3(rxRadians, ryRadians, rzRadians);

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
        public string Name;
        public Material[] Materials;
        public Vertex[][] Vertices; 
        public uint[][] Indices;

        public static Mesh Load(MeshFormat format, string path)
        {
            return Load(format, path, Path.GetDirectoryName(path) + @"\..\textures", null);
        }

        public static Mesh Load(MeshFormat format, string path, string textureDir, ISet<string> alphaTextures)
        {
            switch (format)
            {
                case MeshFormat.Autodesk3ds:
                    throw new NotImplementedException();
                case MeshFormat.Collada:
                    return LoadFromCollada(path, textureDir, alphaTextures);
                case MeshFormat.DirectX:
                    throw new NotImplementedException();
                case MeshFormat.Wavefront:
                    throw new NotImplementedException();
                default:
                    return null;
            }
        }

        private static Mesh LoadFromCollada(string path, string textureDir, ISet<string> alphaTextures)
        {
            Collada collada = null;
            try
            {
                collada = ColladaXmlSerializer.DeserializeFromXml(path);
            }
            catch (Exception ex)
            {
                Log.Write(LogLevel.Error, "Unable to read Collada file {0}.", path);
                Log.Write(LogLevel.Debug, "The exception thrown was {0}:\n {1}", ex.Message, ex.StackTrace);
                return null;
            }
            
            Mesh resultMesh = new Mesh();
            // Get the name of the mesh (which is the path in this case)
            resultMesh.Name = path;
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
                string fullPath = textureDir
                    + Constants.PATH_DELIM + collada.Images[i].ImageFile;
                // TODO: separate image file paths reading from material info reading
                // and should not assume that material's index == image's index
                if (alphaTextures != null && alphaTextures.Contains(collada.Images[i].ImageFile))
                    resultMesh.Materials[i].Texture = TextureManager.PutIntoLoadQueue(fullPath, true);
                else
                    resultMesh.Materials[i].Texture = TextureManager.PutIntoLoadQueue(fullPath, false);
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
            return this.Name.GetHashCode();
        }

        private Mesh()
        {
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    public class Mesh
    {
        // TODO: make the path of the texture file configurable
        private string texturePath = @"D:\下載\texture";

        public Material[] Materials;
        public Vertex[][] Vertices; 
        public ushort[][] Indices;

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

                resultMesh.Materials[i].TextureId = Texture.LoadTextureFromFile(resultMesh.texturePath 
                    + Constants.PATH_DELIM + resultMesh.Materials[i].Name);
                resultMesh.Materials[i].Shading = collada.Effects[i].Shading;
                materialMapping.Add(collada.Materials[i].Id, i);
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

            resultMesh.Indices = new ushort[resultMesh.Vertices.Length][];
            for (int i = 0; i < resultMesh.Vertices.Length; i++)
            {
                List<ushort> indexList = new List<ushort>();
                for (ushort j = 0; j < resultMesh.Vertices[i].Length; j++)
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

        private Mesh()
        {

        }
    }
}
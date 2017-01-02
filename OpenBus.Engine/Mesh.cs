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

    /// <summary>
    /// Defines a mesh with an identifier, translation and rotation applied.
    /// </summary>
    public struct Entity
    {
        private static int currentId = 0;

        public string MeshName;
        public Vector3 Translation;
        public Vector3 Rotation;

        public int Id;
        public Vector3 Color;

        /// <summary>
        /// Creates an entity that uses the specified mesh name, translation and rotation.
        /// </summary>
        /// <param name="meshName">A unique name that identifies the mesh</param>
        /// <param name="tx">Meters to translate along the x-axis (left or right).</param>
        /// <param name="ty">Meters to translate along the y-axis (up or down).</param>
        /// <param name="tz">Meters to translate along the z-axis (forward or backward).</param>
        /// <param name="rx">Rotation about x-axis in degrees.</param>
        /// <param name="ry">Rotation about y-axis in degrees.</param>
        /// <param name="rz">Rotation about z-axis in degrees.</param>
        public Entity(string meshName, float tx, float ty, float tz, float rx, float ry, float rz)
        {
            MeshName = meshName;
            Translation = new Vector3(tx, ty, -tz);
            // Conver the angles into radians
            float rxRadians = Common.MathHelper.DegreesToRadians(rx),
                  ryRadians = Common.MathHelper.DegreesToRadians(ry),
                  rzRadians = Common.MathHelper.DegreesToRadians(rz);
            Rotation = new Vector3(rxRadians, ryRadians, rzRadians);

            // The color is determined by getting the three least significant bytes of the ID.
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

        public static Mesh CreateSpline(SplineInputParameters input)
        {
            // Calculate the parameters needed to construct the spline
            int sign, segments;
            float centralAngle, chord, radiansPerSegment;
            sign = Math.Sign(input.Radius);
            if (input.Radius == 0.0)
            {
                centralAngle = float.PositiveInfinity;
                chord = input.Length;
                segments = 1;
                radiansPerSegment = 0;
            }
            else
            {
                centralAngle = input.Length / input.Radius;
                chord = 2 * input.Radius * (float)Math.Sin(centralAngle / 2);
                segments = (int)Math.Ceiling(
                    Common.MathHelper.RadiansToDegrees(centralAngle) 
                    / SplineInputParameters.DEGREES_PER_SEGMENT);
                // Actual and accurate radians per segment to use
                radiansPerSegment = centralAngle / segments;
            }

            int materialIndex = 0;
            Mesh mesh = new Mesh();
            mesh.Name = input.Name;
            // Load the textures
            mesh.Materials = new Material[input.TextureHeightMapping.Count];
            foreach (string texturePath in input.TextureHeightMapping.Keys)
            {
                mesh.Materials[materialIndex] = new Material();
                mesh.Materials[materialIndex].Name = Path.GetFileNameWithoutExtension(texturePath);
                mesh.Materials[materialIndex].Texture = TextureManager.PutIntoLoadQueue(texturePath, false);
                materialIndex++;
            }

            // Get every two cross-sectional vertices for each material, and construct the curve segment
            mesh.Vertices = new Vertex[mesh.Materials.Length][];
            mesh.Indices = new uint[mesh.Materials.Length][];
            materialIndex = 0;
            // For each list of cross-sectional vertices of a texture
            foreach (string texturePath in input.TextureHeightMapping.Keys)
            {
                List<Vertex> vertexList = new List<Vertex>();
                List<uint> indexList = new List<uint>();

                Vertex[] vertices = input.TextureHeightMapping[texturePath].ToArray();
                Vertex startVertexA = vertices[0],
                       startVertexB = vertices[1];
                // For every two cross-sectional vertices
                for (uint vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex += 2)
                {
                    float segmentLength = SplineInputParameters.TEXTURE_V_COORDINATE_PER_METER * 
                        (input.Radius == 0 ? input.Length : SplineInputParameters.RADIANS_PER_SEGMENT * input.Radius);
                    float textureVPerSegmentA = segmentLength * startVertexA.UV.Y,
                          textureVPerSegmentB = segmentLength * startVertexB.UV.Y;
                    Vertex currentVertexA = vertices[vertexIndex],
                           nextVertexA = vertices[vertexIndex],
                           currentVertexB = vertices[vertexIndex + 1],
                           nextVertexB = vertices[vertexIndex + 1];
                    // For each segment of every two cross-sectional vertices
                    for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++)
                    {
                        // Special logic for straight line
                        if (input.Radius == 0)
                        {
                            nextVertexA.Position.Z = -input.Length;
                            nextVertexB.Position.Z = -input.Length;
                        }
                        // Use the current angle to calculate the positions of the next vertices
                        else
                        {
                            float theta = (segmentIndex + 1) * radiansPerSegment,
                                  cosTheta = (float)Math.Cos(theta),
                                  sinTheta = (float)Math.Sin(theta);
                            nextVertexA.Position.X = (input.Radius + startVertexA.Position.X) * cosTheta - input.Radius;
                            // TODO: calculate the y-coordinate with given elevation
                            // nextVertexA.Position.Y = 
                            nextVertexA.Position.Z = -(input.Radius + startVertexA.Position.X) * sinTheta;
                            nextVertexB.Position.X = (input.Radius + startVertexB.Position.X) * cosTheta - input.Radius;
                            // TODO: calculate the y-coordinate with given elevation
                            // nextVertexB.Position.Y = 
                            nextVertexB.Position.Z = -(input.Radius + startVertexB.Position.X) * sinTheta;
                        }

                        // Ensure that the normal is constant along the entire spline, so the order to draw the vertices matters
                        currentVertexA.Normal = Vector3.UnitY;
                        currentVertexB.Normal = Vector3.UnitY;
                        nextVertexA.Normal = Vector3.UnitY;
                        nextVertexB.Normal = Vector3.UnitY;
                        // Add the V-coordinate by a constant, U-coordinate remains the same along the entire spline
                        nextVertexA.UV.Y = currentVertexA.UV.Y + textureVPerSegmentA;
                        nextVertexB.UV.Y = currentVertexB.UV.Y + textureVPerSegmentB;

                        // Add everything to the result list
                        vertexList.Add(currentVertexA);
                        vertexList.Add(currentVertexB);
                        vertexList.Add(nextVertexA);
                        vertexList.Add(nextVertexB);

                        indexList.Add(vertexIndex);
                        indexList.Add(vertexIndex + 2);
                        indexList.Add(vertexIndex + 1);
                        indexList.Add(vertexIndex + 1);
                        indexList.Add(vertexIndex + 2);
                        indexList.Add(vertexIndex + 3);

                        // Use the next vertices as the next current vertices in the next iteration
                        currentVertexA = nextVertexA;
                        currentVertexB = nextVertexB;
                    }
                }
                mesh.Vertices[materialIndex] = vertexList.ToArray();
                mesh.Indices[materialIndex] = indexList.ToArray();

                materialIndex++;
            }

            return mesh;
        }

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

        private Mesh()
        {
        }
    }

    public struct SplineInputParameters
    {
        internal const float DEGREES_PER_SEGMENT = 0.5f;
        internal const float RADIANS_PER_SEGMENT = DEGREES_PER_SEGMENT * Common.MathHelper.Pi / 180f;
        /// <summary>
        /// The v-coordinate (along the z-axis) of the texture coordinates applied 
        /// for each vertex of each meter of the spline's surface.
        /// </summary>
        /// <example>
        /// Draw a figure to explain how this works.
        /// </example>
        public const float TEXTURE_V_COORDINATE_PER_METER = 0.1f;

        public string Name;
        public Dictionary<string, List<Vertex>> TextureHeightMapping;
        public float Radius;
        public float Length;
        public KeyValuePair<float, float> Elevations;
        public KeyValuePair<float, float> Cants;

        public SplineInputParameters(string name, Dictionary<string, List<Vertex>> textureHeights, float radius, float length, float startElevation,
            float endElevation, float startCant, float endCant)
        {
            this.Name = name;
            this.TextureHeightMapping = textureHeights;
            this.Radius = radius;
            this.Length = length;
            this.Elevations = new KeyValuePair<float, float>(startElevation, endElevation);
            this.Cants = new KeyValuePair<float, float>(startCant, endCant);
        }
    }
}

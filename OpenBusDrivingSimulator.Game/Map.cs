using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Config;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
{
    public class MapObject
    {
        private Vector3f position;
        private Vector3f rotations;
        private string[] meshes;
        private string path;

        public string Path
        {
            get { return path; }
        }

        public Vector3f Position
        {
            get { return position; }
        }

        public Vector3f Rotations
        {
            get { return rotations; }
        }

        public string[] Meshes
        {
            get { return meshes; }
        }

        public MapObject(Vector3f position, Vector3f rotations, string[] meshes, string path)
        {
            this.position = position;
            this.rotations = rotations;
            this.meshes = meshes;
            this.path = path;
        }
    }

    public class Terrain
    {
        public struct TerrainDisplacement
        {
            public int X;
            public int Y;
            public float Displacement;
        }

        private const int TERRAIN_GRID_SIZE = 250;

        private List<TerrainDisplacement> displacements;
    }

    public class MapBlock
    {
        private const int MAP_BLOCK_SIZE = 250;

        private bool loaded;
        private Vector2f position;
        private Terrain terrain;
        private List<MapObject> objects;

        public bool Loaded
        {
            get { return loaded; }
        }

        public List<MapObject> Objects
        {
            get { return objects; }
        }

        public Terrain Terrain
        {
            get { return terrain; }
        }

        public static implicit operator MapBlock(MapBlockEx blockEx)
        {
            MapBlock block = new MapBlock();
            block.loaded = false;
            block.position = new Vector2f(blockEx.Position.X,
                blockEx.Position.Y);
            // TODO: read the terrain data as well
            block.objects = new List<MapObject>();
            foreach (ObjectInfo objectInfo in blockEx.Objects)
            {
                ObjectEx objectEx = XmlDeserializeHelper<ObjectEx>.DeserializeFromFile(GameEnvironment.RootPath + objectInfo.Path);
                string[] meshPaths = new string[objectEx.Meshes.Length];
                for (int i = 0; i < objectEx.Meshes.Length; i++)
                    meshPaths[i] = objectEx.Meshes[i].Path;
                block.objects.Add(new MapObject(objectInfo.Position,
                    objectInfo.Rotations, meshPaths, objectInfo.Path));
            }
            return block;
        }
    }

    public class Map
    {
        private MapBlock currentBlock;

        public void LoadBlock(MapBlock block)
        {
            // Load the static objects to buffer
            List<Mesh> staticMeshes = new List<Mesh>();
            foreach (MapObject mapObject in block.Objects)
            {
                foreach (string meshPath in mapObject.Meshes)
                {
                    Mesh staticMesh = Mesh.LoadFromCollada(GameEnvironment.RootPath + "objects\\" + meshPath);
                    staticMesh.RotateY(mapObject.Rotations.Y);
                    staticMesh.Translate(mapObject.Position.X, mapObject.Position.Y, mapObject.Position.Z);
                    staticMeshes.Add(staticMesh);
                }
            }
            Renderer.LoadStaticMeshesToScene(staticMeshes);

            // TODO: load the terrain data into buffer
            // For now, just draw a 250 x 250 plane at y = 0

            currentBlock = block;
        }
    }
}

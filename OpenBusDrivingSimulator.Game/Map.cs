using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBusDrivingSimulator.Config;
using OpenBusDrivingSimulator.Core;
using OpenBusDrivingSimulator.Engine;

namespace OpenBusDrivingSimulator.Game
{
    public class Terrain
    {
        public class TerrainDisplacement
        {
            public int X;
            public int Y;
            public float Displacement;
        }

        public const int TERRAIN_GRID_SIZE = 250;

        private string texture;
        private Vector2f textureUV;
        private Vector2f position;
        private List<TerrainDisplacement> displacements;

        public string TexturePath
        {
            get { return texture; }
        }

        public Vector2f TextureUV
        {
            get { return textureUV; }
        }

        public List<TerrainDisplacement> Displacements
        {
            get { return displacements;}
        }

        public static implicit operator Terrain(TerrainEx terrainEx)
        {
            Terrain terrain = new Terrain();
            terrain.texture = terrainEx.Texture.Path;
            terrain.textureUV = terrainEx.Texture.UV;
            terrain.displacements = new List<TerrainDisplacement>();
            foreach (TerrainEx.TerrainDisplacement displacementEx in terrainEx.Displacements)
            {
                TerrainDisplacement displacement = new TerrainDisplacement();
                displacement.X = displacementEx.X;
                displacement.Y = displacementEx.Y;
                displacement.Displacement = displacementEx.Displacement;
                terrain.displacements.Add(displacement);
            }
            return terrain;
        }
    }

    public class MapBlock
    {
        public const int MAP_BLOCK_SIZE = 250;

        private bool loaded;
        private Vector2f position;
        private List<Object> objects;

        public bool Loaded
        {
            get { return loaded; }
        }

        public List<Object> Objects
        {
            get { return objects; }
        }

        public Vector2f Position
        {
            get { return position; }
            set { position = value; }
        }

        public static implicit operator MapBlock(MapBlockEx blockEx)
        {
            MapBlock block = new MapBlock();
            block.loaded = false;
            block.position = new Vector2f();
            block.objects = new List<Object>();
            foreach (ObjectInfo objectInfo in blockEx.Objects)
            {
                ObjectEx objectEx = XmlDeserializeHelper<ObjectEx>.DeserializeFromFile(GameEnvironment.RootPath + objectInfo.Path);
                string[] meshPaths = new string[objectEx.Meshes.Length];
                ObjectTexture[] alphaTextures = null;
                if (objectEx.AlphaTextures != null)
                    alphaTextures = new ObjectTexture[objectEx.AlphaTextures.Length];
                for (int i = 0; i < objectEx.Meshes.Length; i++)
                    meshPaths[i] = objectEx.Meshes[i].Path;
                if (alphaTextures != null)
                    for (int i = 0; i < objectEx.AlphaTextures.Length; i++)
                        alphaTextures[i] = new ObjectTexture(objectEx.AlphaTextures[i].Path,
                            (ObjectTextureAlphaMode)objectEx.AlphaTextures[i].Mode);
                block.objects.Add(new Object(objectInfo.Position,
                    objectInfo.Rotations, meshPaths, objectInfo.Path, alphaTextures));
            }
            return block;
        }
    }

    public class Map
    {
        private MapBlock currentBlock;

        public void LoadBlock(int x, int y, MapBlock block, Terrain terrain)
        {
            // Load the static objects to buffer
            block.Position = new Vector2f(x, y);
            List<Entity> staticEntities = new List<Entity>();
            HashSet<Mesh> staticMeshes = new HashSet<Mesh>();
            foreach (Object mapObject in block.Objects)
            {
                HashSet<string> alphaTextures = new HashSet<string>();
                if (mapObject.AlphaTextures != null)
                    foreach (ObjectTexture objTexture in mapObject.AlphaTextures)
                        alphaTextures.Add(objTexture.Path);

                foreach (string meshPath in mapObject.Meshes)
                {
                    Mesh staticMesh = Mesh.LoadFromCollada(GameEnvironment.RootPath + "objects\\" + meshPath, alphaTextures);
                    Entity entity = new Entity(staticMesh.Name, mapObject.Position.X + block.Position.X, 
                        mapObject.Position.Y, -mapObject.Position.Z - block.Position.Y,
                        mapObject.Rotations.X, mapObject.Rotations.Y, mapObject.Rotations.Z);
                    staticEntities.Add(entity);
                    staticMeshes.Add(staticMesh);
                }
            }
            Renderer.LoadStaticEntitiesToScene(staticEntities, staticMeshes);

            // Load the terrain data to buffer as well
            int terrainSize = Terrain.TERRAIN_GRID_SIZE;
            float[][] heights = new float[terrainSize][];
            for (int i = 0; i < terrainSize; i++)
            {
                heights[i] = new float[terrainSize];
                for (int j = 0; j < terrainSize; j++)
                {
                    Terrain.TerrainDisplacement displacement = terrain.Displacements
                        .Find(d => d.X == i && d.Y == j);
                    if (displacement != null)
                        heights[i][j] = displacement.Displacement;
                    else
                        heights[i][j] = 0;
                }
            }
                
            Renderer.LoadTerrain(x, y, Terrain.TERRAIN_GRID_SIZE,
                heights, GameEnvironment.RootPath + terrain.TexturePath, terrain.TextureUV.X, terrain.TextureUV.Y);
            currentBlock = block;
        }
    }
}

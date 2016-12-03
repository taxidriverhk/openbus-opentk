using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OpenBus.Config;
using OpenBus.Common;
using OpenBus.Engine;

namespace OpenBus.Game
{
    public class Terrain
    {
        public class TerrainDisplacement
        {
            public int X;
            public int Y;
            public float Displacement;
        }

        private string texture;
        private Vector2f textureUV;
        private MapBlockPosition position;
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

        public Terrain(string texture, Vector2f uv, MapBlockPosition position, List<TerrainDisplacement> displacements)
        {
            this.texture = texture;
            this.textureUV = uv;
            this.position = position;
            this.displacements = displacements;
        }
    }

    public class MapBlock
    {
        public const int MAP_BLOCK_SIZE = 250;

        private bool loaded;
        private MapBlockPosition position;
        private List<Object> objects;

        public bool Loaded
        {
            get { return loaded; }
        }

        public List<Object> Objects
        {
            get { return objects; }
        }

        public MapBlockPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        public MapBlock()
        {
            objects = new List<Object>();
            loaded = false;
        }

        public void AddObject(Object objectItem)
        {
            objects.Add(objectItem);
        }
    }

    public struct MapBlockPosition
    {
        public int X;
        public int Y;

        public MapBlockPosition(int x, int y)
        {
            X = x; Y = y;
        }

        public static MapBlockPosition Zero
        {
            get { return new MapBlockPosition(0, 0); }
        }

        public static bool operator ==(MapBlockPosition left, MapBlockPosition right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(MapBlockPosition left, MapBlockPosition right)
        {
            return left.X != right.X || left.Y != right.Y;
        }
    }

    public struct MapBlockInfo
    {
        public MapBlockPosition Position;
        public string MapBlockToLoad;
        public string TerrainToLoad;

        public MapBlockInfo(MapBlockPosition position, string mapBlockToLoad, string terrainToLoad)
        {
            Position = position;
            MapBlockToLoad = mapBlockToLoad;
            TerrainToLoad = terrainToLoad;
        }
    }

    public class Map
    {
        private const int MAX_SIMU_BLOCKS = 9;

        private List<MapBlockInfo> blockInfoList;
        private List<MapBlock> loadedBlocks;
        private MapBlock currentBlock;

        public List<MapBlockInfo> BlockInfoList
        {
            get { return blockInfoList; }
        }

        public Map()
        {
            blockInfoList = new List<MapBlockInfo>();
            loadedBlocks = new List<MapBlock>();
        }

        public void AddBlockInfo(MapBlockInfo mapBlockInfo)
        {
            blockInfoList.Add(mapBlockInfo);
        }

        public bool BlockExists(MapBlockPosition position)
        {
            return loadedBlocks.Find(b => b.Position == position) != null;
        }

        public MapBlockPosition GetBlockPosition(Vector3f position)
        {
            int blockX = (int)(position.X / MapBlock.MAP_BLOCK_SIZE),
                blockY = (int)(position.Z / MapBlock.MAP_BLOCK_SIZE);
            if (position.X < 0)
                blockX -= 1;
            if (position.Z < 0)
                blockY -= 1;
            return new MapBlockPosition(blockX, blockY);
        }

        public bool IsInMap(Vector3f position)
        {
            return BlockExists(GetBlockPosition(position));
        }

        public void LoadBlock(int x, int y, MapBlock block, Terrain terrain)
        {
            int blockSize = MapBlock.MAP_BLOCK_SIZE;
            // Load the static objects to buffer in a separate thread
            Thread loadBlockThread = new Thread(delegate ()
            {
                MapBlockLoader.StartLoadBlockThread(block, terrain, blockSize);
            });
            loadBlockThread.IsBackground = true;
            loadBlockThread.Start();

            if (currentBlock == null)
                currentBlock = block;
            loadedBlocks.Add(block);
        }
    }

    public static class MapBlockLoader
    {
        private static bool loadedIntoBuffer;
        private static bool loadCompleted;

        private static double progress;

        private static MapBlockPosition blockPosition;
        private static Terrain terrain;
        private static float[][] terrainHeights;
        private static List<Entity> entities;
        private static HashSet<Mesh> meshes;

        static MapBlockLoader()
        {
            loadedIntoBuffer = false;
            loadCompleted = false;
            entities = new List<Entity>();
            meshes = new HashSet<Mesh>();
        }

        public static bool LoadedInfoBuffer
        {
            get { return loadedIntoBuffer; }
        }

        public static bool LoadCompleted
        {
            get { return loadCompleted; }
        }

        public static double Progress
        {
            get { return progress; }
        }

        public static void StartLoadBlockThread(MapBlock block, Terrain terrainToLoad, int blockSize)
        {
            double numOfObjects = block.Objects.Count;
            loadedIntoBuffer = false;

            // Static entity loading
            blockPosition = block.Position;
            foreach (Object mapObject in block.Objects)
            {
                HashSet<string> alphaTextures = new HashSet<string>();
                if (mapObject.AlphaTextures != null)
                    foreach (ObjectTexture objTexture in mapObject.AlphaTextures)
                        alphaTextures.Add(objTexture.Path);

                foreach (string meshPath in mapObject.Meshes)
                {
                    Mesh staticMesh = Mesh.LoadFromCollada(GameEnvironment.RootPath + 
                        mapObject.ModelDirectory + Constants.PATH_DELIM + meshPath,
                        GameEnvironment.RootPath +
                        mapObject.TextureDirectory, alphaTextures);
                    Entity entity = new Entity(staticMesh.Name, mapObject.Position.X + block.Position.X * MapBlock.MAP_BLOCK_SIZE,
                        mapObject.Position.Y, -mapObject.Position.Z - block.Position.Y * MapBlock.MAP_BLOCK_SIZE,
                        mapObject.Rotations.X, mapObject.Rotations.Y, mapObject.Rotations.Z);
                    entities.Add(entity);
                    meshes.Add(staticMesh);
                }
                progress += (1 / numOfObjects) * 0.9;
            }

            // Terrain loading
            terrain = terrainToLoad;
            terrainHeights = new float[blockSize][];
            for (int i = 0; i < blockSize; i++)
            {
                terrainHeights[i] = new float[blockSize];
                for (int j = 0; j < blockSize; j++)
                {
                    Terrain.TerrainDisplacement displacement = terrain.Displacements
                        .Find(d => d.X == i && d.Y == j);
                    if (displacement != null)
                        terrainHeights[i][j] = displacement.Displacement;
                    else
                        terrainHeights[i][j] = 0;
                }
            }

            progress = 1.0;
            loadCompleted = true;
        }

        public static void LoadIntoBuffer()
        {
            if (loadedIntoBuffer || !loadCompleted)
                return;

            TextureManager.LoadAllTexturesInQueue();
            Renderer.LoadStaticEntitiesToScene(entities, meshes);
            Renderer.LoadTerrain(blockPosition.X, blockPosition.Y,
                MapBlock.MAP_BLOCK_SIZE, terrainHeights, GameEnvironment.RootPath + terrain.TexturePath,
                terrain.TextureUV.X, terrain.TextureUV.Y);

            blockPosition = MapBlockPosition.Zero;
            terrain = null;
            terrainHeights = null;
            entities.Clear();
            meshes.Clear();

            loadedIntoBuffer = true;
            loadCompleted = false;
            progress = 0;
        }
    }
}

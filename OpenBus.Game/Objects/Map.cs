using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OpenBus.Common;
using OpenBus.Engine;

namespace OpenBus.Game.Objects
{
    public enum SkyMode
    {
        DAY = 0,
        NIGHT = 1,
        DAWN = 2,
        SUNSET = 3
    }

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

    public class Sky
    {
        private int size;
        private SkyMode mode;
        private string texture;

        public int Size
        {
            get { return size; }
        }

        public SkyMode Mode
        {
            get { return mode; }
        }

        public string Texture
        {
            get { return texture; }
        }

        public Sky(int size, SkyMode mode, string texture)
        {
            this.size = size;
            this.mode = mode;
            this.texture = texture;
        }

        public Sky(int size, string mode, string texture)
        {
            this.size = size;
            if (!Enum.TryParse(mode, out this.mode))
                this.mode = SkyMode.DAY;
            this.texture = texture;
        }

        public Sky(string texture)
        {
            this.size = 450;
            this.mode = SkyMode.DAY;
            this.texture = texture;
        }
    }

    public class MapBlock
    {
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
        private static readonly int blockSize = Game.Settings.MapDisplaySettings.BlockSize;
        private static readonly string skyBoxMeshPath = EnvironmentVariables.RootPath + @"objects\skies\models\sky.dae";

        private List<Sky> skies;
        private Sky currentSky;
        private List<MapBlockInfo> blockInfoList;
        private List<MapBlock> loadedBlocks;
        private MapBlock currentBlock;

        public static int BlockSize
        {
            get { return blockSize; }
        }

        public List<MapBlockInfo> BlockInfoList
        {
            get { return blockInfoList; }
        }

        public Map()
        {
            skies = new List<Sky>();
            blockInfoList = new List<MapBlockInfo>();
            loadedBlocks = new List<MapBlock>();
        }

        public void AddBlockInfo(MapBlockInfo mapBlockInfo)
        {
            blockInfoList.Add(mapBlockInfo);
        }

        public void AddSky(Sky sky)
        {
            skies.Add(sky);
            if (currentSky == null)
                currentSky = sky;
        }

        public bool BlockExists(MapBlockPosition position)
        {
            return loadedBlocks.Find(b => b.Position == position) != null;
        }

        public bool ChangeSky(SkyMode mode)
        {
            Sky sky = skies.Find(s => s.Mode == mode);
            if (sky != null)
            {
                currentSky = sky;
                return true;
            }
            else
                return false;
        }

        public MapBlockPosition GetBlockPosition(Vector3f position)
        {
            int blockX = (int)(position.X / blockSize),
                blockY = (int)(position.Z / blockSize);
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

        public void LoadCurrentSky()
        {
            if (currentSky == null)
                return;

            Mesh skyBoxMesh = Mesh.LoadFromCollada(skyBoxMeshPath);
            if (skyBoxMesh == null)
            {
                Log.Write(LogLevel.ERROR, "Failed to load the sky box from {0}", skyBoxMeshPath);
                return;
            }

            skyBoxMesh.Materials[0].Texture.Path = currentSky.Texture;
            TextureManager.UnloadTexture(skyBoxMesh.Materials[0].Texture.TextureId);
            skyBoxMesh.Materials[0].Texture.TextureId = 
                TextureManager.LoadTexture(EnvironmentVariables.RootPath + currentSky.Texture);
            Renderer.LoadSkyBox(skyBoxMesh, currentSky.Size);
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
                    Mesh staticMesh = Mesh.LoadFromCollada(EnvironmentVariables.RootPath + 
                        mapObject.ModelDirectory + Constants.PATH_DELIM + meshPath,
                        EnvironmentVariables.RootPath +
                        mapObject.TextureDirectory, alphaTextures);
                    Entity entity = new Entity(staticMesh.Name, mapObject.Position.X + block.Position.X * Map.BlockSize,
                        mapObject.Position.Y, -mapObject.Position.Z - block.Position.Y * Map.BlockSize,
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
                Map.BlockSize, terrainHeights, EnvironmentVariables.RootPath + terrain.TexturePath,
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using OpenBus.Common;
using OpenBus.Engine;
using OpenBus.Game.Controls;

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
        private string texture;
        private Vector2f textureUV;
        private MapBlockPosition position;
        private float[][] heights;

        public string TexturePath
        {
            get { return texture; }
        }

        public Vector2f TextureUV
        {
            get { return textureUV; }
        }

        public MapBlockPosition Position
        {
            get { return position; }
        }

        public float[][] Heights
        {
            get { return heights; }
        }

        public Terrain(string texture, Vector2f uv, MapBlockPosition position, float[][] heights)
        {
            this.texture = texture;
            this.textureUV = uv;
            this.position = position;
            this.heights = heights;
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

        public override bool Equals(object obj)
        {
            MapBlockPosition other = (MapBlockPosition)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return this.X * 10000 + this.Y;
        }
    }

    /// <summary>
    /// Contains information about a map block including its position and corresponding paths to the 
    /// block and terrain configuration files.
    /// </summary>
    public struct MapBlockInfo
    {
        /// <summary>
        /// Position of the map block
        /// </summary>
        public MapBlockPosition Position;
        /// <summary>
        /// Path to the .block file to be loaded, relative to the map directory.
        /// </summary>
        public string MapBlockToLoad;
        /// <summary>
        /// Path to the .terrain file to be loaded, relative to the map directory.
        /// </summary>
        public string TerrainToLoad;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="position">Map block position.</param>
        /// <param name="mapBlockToLoad">Path to the .block file, relative to the map directory.</param>
        /// <param name="terrainToLoad">Path to the .terrain file, relative to the map directory.</param>
        public MapBlockInfo(MapBlockPosition position, string mapBlockToLoad, string terrainToLoad)
        {
            Position = position;
            MapBlockToLoad = mapBlockToLoad;
            TerrainToLoad = terrainToLoad;
        }
    }

    public class MapInfo
    {
        public string Path;
        public string Name;
    }

    public class Map
    {
        private static readonly int blockSize = Game.Settings.MapDisplaySettings.BlockSize;
        private static readonly string skyBoxMeshPath = EnvironmentVariables.RootPath + @"objects\skies\models\sky.dae";

        private string mapDirectory;
        private List<MapBlockInfo> blockInfoList;
        private List<MapBlockInfo> blocksToLoad;

        private Sky currentSky;
        private List<Sky> loadedSkies;

        private MapBlock currentBlock;
        private List<MapBlock> loadedBlocks;
        private Terrain currentTerrain;
        private List<Terrain> loadedTerrains;

        public static int BlockSize
        {
            get { return blockSize; }
        }

        public List<MapBlockInfo> BlockInfoList
        {
            get { return blockInfoList; }
        }

        public int NumberOfBlocksLoaded
        {
            get
            {
                if (loadedBlocks == null)
                    return 0;
                else
                    return loadedBlocks.Count;
            }
        }

        public Map(string path)
        {
            mapDirectory = Path.GetDirectoryName(path);
            loadedSkies = new List<Sky>();
            blockInfoList = new List<MapBlockInfo>();
            blocksToLoad = new List<MapBlockInfo>();
            loadedBlocks = new List<MapBlock>();
            loadedTerrains = new List<Terrain>();
        }

        public void AddBlockAndTerrain(MapBlock block, Terrain terrain)
        {
            if (currentBlock == null)
                currentBlock = block;
            loadedBlocks.Add(block);

            if (currentTerrain == null)
                currentTerrain = terrain;
            loadedTerrains.Add(terrain);
        }

        public void AddBlockInfo(MapBlockInfo mapBlockInfo)
        {
            blockInfoList.Add(mapBlockInfo);
        }

        public void AddBlockToLoad(MapBlockInfo blockInfo)
        {
            if (!BlockExists(blockInfo.Position))
                blocksToLoad.Add(blockInfo);
        }

        public void AddSky(Sky sky)
        {
            loadedSkies.Add(sky);
            if (currentSky == null)
                currentSky = sky;
        }

        public bool BlockExists(MapBlockPosition position)
        {
            return loadedBlocks.Find(b => b.Position == position) != null;
        }

        public bool ChangeSky(SkyMode mode)
        {
            Sky sky = loadedSkies.Find(s => s.Mode == mode);
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

        public float GetTerrainHeight(MapBlockPosition blockPosition, Vector3f position)
        {
            Terrain terrain = loadedTerrains.Find(t => t.Position == blockPosition);
            if (terrain != null)
                // TODO: implement a more accurate calculation
                return terrain.Heights[(int)position.X % BlockSize]
                    [(int)position.Z % BlockSize];
            else
                return 0;
        }

        public bool IsInMap(Vector3f position)
        {
            MapBlockPosition blockPosition = GetBlockPosition(position);
            float terrainHeight = GetTerrainHeight(blockPosition, position);
            return BlockExists(blockPosition) && position.Y >= terrainHeight;
        }

        public void LoadBlocksInQueue()
        {
            List<MapBlockInfo> blocksToRemove = new List<MapBlockInfo>();
            foreach (MapBlockInfo blockInfo in blocksToLoad)
            {
                // If the loader is in use by other thread, then check again in the next main loop
                if (MapBlockLoader.LoaderInUse)
                    continue;
                // Load the static objects to buffer in a separate thread
                MapBlockLoader.LoaderInUse = true;
                Thread loadBlockThread = new Thread(delegate ()
                {
                    MapBlockLoader.StartLoadBlockThread(mapDirectory, blockInfo);
                });
                loadBlockThread.IsBackground = true;
                loadBlockThread.Start();
                blocksToRemove.Add(blockInfo);
            }

            // If the thread for loading the block has started, then remove it from the to-load list
            foreach (MapBlockInfo blockInfo in blocksToRemove)
                blocksToLoad.Remove(blockInfo);
        }

        public void LoadCurrentSky()
        {
            if (currentSky == null)
                return;

            Mesh skyBoxMesh = Mesh.Load(MeshFormat.Collada, skyBoxMeshPath);
            if (skyBoxMesh == null)
            {
                Log.Write(LogLevel.Error, "Failed to load the sky box from {0}", skyBoxMeshPath);
                return;
            }

            skyBoxMesh.Materials[0].Texture.Path = currentSky.Texture;
            TextureManager.UnloadTexture(skyBoxMesh.Materials[0].Texture.TextureId);
            skyBoxMesh.Materials[0].Texture.TextureId = 
                TextureManager.LoadTexture(EnvironmentVariables.RootPath + currentSky.Texture);
            Renderer.LoadSkyBox(skyBoxMesh, currentSky.Size);
        }

        public void UnloadBlock(MapBlockInfo blockInfo)
        {

        }
    }

    public static class MapBlockLoader
    {
        private static bool loaderInUse;
        private static bool loadedIntoBuffer;
        private static bool loadCompleted;

        private static double progress;

        private static MapBlockPosition blockPosition;
        private static MapBlock block;
        private static Terrain terrain;
        private static List<Entity> entities;
        private static HashSet<Mesh> meshes;

        static MapBlockLoader()
        {
            loaderInUse = false;
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

        public static bool LoaderInUse
        {
            get { return loaderInUse; }
            set { loaderInUse = value; }
        }

        public static double Progress
        {
            get { return progress; }
        }

        public static void StartLoadBlockThread(string mapDirectory, MapBlockInfo blockInfo)
        {
            loadedIntoBuffer = false;
            MapBlock blockToLoad = ConfigLoader.LoadMapBlock(mapDirectory
                    + Constants.PATH_DELIM + blockInfo.MapBlockToLoad, blockInfo.Position);
            Terrain terrainToLoad = ConfigLoader.LoadTerrain(mapDirectory
                + Constants.PATH_DELIM + blockInfo.TerrainToLoad, blockInfo.Position);
            if (blockToLoad == null || terrainToLoad == null)
                return;

            double numOfObjects = blockToLoad.Objects.Count;
            // Static entity loading
            blockPosition = blockToLoad.Position;
            foreach (Object mapObject in blockToLoad.Objects)
            {
                HashSet<string> alphaTextures = new HashSet<string>();
                if (mapObject.AlphaTextures != null)
                    foreach (ObjectTexture objTexture in mapObject.AlphaTextures)
                        alphaTextures.Add(objTexture.Path);

                foreach (string meshPath in mapObject.Meshes)
                {
                    Mesh staticMesh = Mesh.Load(MeshFormat.Collada, EnvironmentVariables.RootPath + 
                        mapObject.ModelDirectory + Constants.PATH_DELIM + meshPath,
                        EnvironmentVariables.RootPath +
                        mapObject.TextureDirectory, alphaTextures);
                    if (staticMesh == null || !staticMesh.Validate())
                    {
                        Log.Write(LogLevel.Error, "{0} is not a valid mesh.", meshPath);
                        continue;
                    }
                    Entity entity = new Entity(staticMesh.Name, mapObject.Position.X + blockToLoad.Position.X * Map.BlockSize,
                        mapObject.Position.Y, -mapObject.Position.Z - blockToLoad.Position.Y * Map.BlockSize,
                        mapObject.Rotations.X, mapObject.Rotations.Y, mapObject.Rotations.Z);
                    entities.Add(entity);
                    meshes.Add(staticMesh);
                }
                progress += (1 / numOfObjects) * 0.9;
            }

            // Save the loaded block and terrain
            block = blockToLoad;
            terrain = terrainToLoad;

            progress = 1.0;
            loadCompleted = true;
        }

        public static void LoadIntoBuffer()
        {
            if (loadedIntoBuffer || !loadCompleted)
                return;

            int identifier = Game.World.NumberOfBlocksLoaded + 1;
            TextureManager.LoadAllTexturesInQueue();
            Renderer.LoadStaticEntitiesToScene(identifier, entities, meshes);
            Game.World.AddBlockAndTerrain(block, terrain);
            Renderer.LoadTerrain(identifier, blockPosition.X, blockPosition.Y,
                Map.BlockSize, terrain.Heights, EnvironmentVariables.RootPath + terrain.TexturePath,
                terrain.TextureUV.X, terrain.TextureUV.Y);

            blockPosition = MapBlockPosition.Zero;
            block = null;
            terrain = null;
            entities.Clear();
            meshes.Clear();

            loadedIntoBuffer = true;
            loadCompleted = false;
            loaderInUse = false;
            progress = 0;
        }
    }
}

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
    /// <summary>
    /// Defines when should a sky object be displayed
    /// (during daytime or night, etc.).
    /// </summary>
    public enum SkyMode
    {
        /// <summary>
        /// The sky will be displayed during daytime.
        /// This is also the default value used for a Sky object.
        /// </summary>
        Day = 0,
        /// <summary>
        /// The sky will be displayed during night.
        /// </summary>
        Night = 1,
        /// <summary>
        /// The sky will be displayed during dawn (or sunrise).
        /// </summary>
        Dawn = 2,
        /// <summary>
        /// The sky will be displayed during sunset (or evening).
        /// </summary>
        Sunset = 3
    }

    /// <summary>
    /// Contains the terrain information of a map block.
    /// </summary>
    public class Terrain
    {
        private string texture;
        private Vector2f textureUV;
        private MapBlockPosition position;
        private float[][] heights;

        /// <summary>
        /// Path to the texture used for the surface of the terrain, relative to the game's root path.
        /// </summary>
        public string TexturePath
        {
            get { return texture; }
        }

        /// <summary>
        /// Defines the texture coordinates (i.e. UV) applied to the surface of the terrain with the texture.
        /// </summary>
        public Vector2f TextureUV
        {
            get { return textureUV; }
        }

        /// <summary>
        /// Defines the map block position, 
        /// for which there should be a map block with the same map block position in the same map.
        /// </summary>
        public MapBlockPosition Position
        {
            get { return position; }
        }

        /// <summary>
        /// Defines the heights for each vertex of the terrain.
        /// For example, Heights[x][y] defines the height at (x, 0, y), relative to the starting position of the map block.
        /// The dimension is expected to be (BlockSize + 1) x (BlockSize + 1), where BlockSize of the size of a map block.
        /// </summary>
        public float[][] Heights
        {
            get { return heights; }
        }

        /// <summary>
        /// Initializes a Terrain object that uses the specified texture, its coordinates, 
        /// map block position and heights.
        /// </summary>
        /// <param name="texture">Path to the texture used, relative to the game's root path.</param>
        /// <param name="uv">The texture coordinates being applied to the terrain.</param>
        /// <param name="position">Map block position.</param>
        /// <param name="heights">Heights for each vertex of the terrain.</param>
        public Terrain(string texture, Vector2f uv, MapBlockPosition position, float[][] heights)
        {
            this.texture = texture;
            this.textureUV = uv;
            this.position = position;
            this.heights = heights;
        }
    }

    /// <summary>
    /// Defines the sky box of a map.
    /// </summary>
    public class Sky
    {
        private int size;
        private SkyMode mode;
        private string texture;

        /// <summary>
        /// Defines how large the sky box should be.
        /// So the volume would be Size ^ 3.
        /// </summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        /// Defines when should this sky box be used.
        /// </summary>
        public SkyMode Mode
        {
            get { return mode; }
        }

        /// <summary>
        /// Defines the path to the texture being used, relative to the game's root path.
        /// </summary>
        public string Texture
        {
            get { return texture; }
        }

        /// <summary>
        /// Constructs a Sky object that uses the specified size, mode and texture path.
        /// </summary>
        /// <param name="size">Dimensions of the sky box.</param>
        /// <param name="mode">When should this sky box be used.</param>
        /// <param name="texture">Path to the texture being used for this skybox, relative to the game's root path.</param>
        public Sky(int size, SkyMode mode, string texture)
        {
            this.size = size;
            this.mode = mode;
            this.texture = texture;
        }

        /// <summary>
        /// Constructs a Sky object that uses the specified size, mode (in string format) and texture path.
        /// If the mode in string format passed in is invalid, then DAY mode will be used, so the sky will
        /// be displayed during daytime.
        /// </summary>
        /// <param name="size">Dimensions of the sky box.</param>
        /// <param name="mode">When should this sky box be used, in string format.</param>
        /// <param name="texture">Path to the texture being used for this skybox, relative to the game's root path.</param>
        public Sky(int size, string mode, string texture)
        {
            this.size = size;
            if (!Enum.TryParse(mode, out this.mode))
                this.mode = SkyMode.Day;
            this.texture = texture;
        }

        /// <summary>
        /// Constructs a Sky object that uses the specified texture path, with daylight mode and size of 450m.
        /// </summary>
        /// <param name="texture">Path to the texture being used for this skybox, relative to the game's root path.</param>
        public Sky(string texture)
        {
            this.size = 450;
            this.mode = SkyMode.Day;
            this.texture = texture;
        }
    }

    /// <summary>
    /// Defines a map block that contains the information about the objects and splines within the block.
    /// And also the position and whether it has been loaded to the graphics buffer.
    /// </summary>
    public class MapBlock
    {
        private bool loaded;
        private MapBlockPosition position;
        private List<Object> objects;
        private List<Spline> splines;

        /// <summary>
        /// Gets whether this map block has been loaded to the graphics buffer.
        /// </summary>
        public bool Loaded
        {
            get { return loaded; }
        }

        /// <summary>
        /// Gets the list of objects that this map block contains.
        /// </summary>
        public List<Object> Objects
        {
            get { return objects; }
        }

        /// <summary>
        /// Gets the position of the map block so its starting world position would be
        /// (Position.X * BlockSize, 0, Position.Y * BlockSize).
        /// </summary>
        public MapBlockPosition Position
        {
            get { return position; }
        }

        /// <summary>
        /// Constructs a MapBlock object that has the specified position.
        /// </summary>
        /// <param name="position"></param>
        public MapBlock(MapBlockPosition position)
        {
            this.position = position;
            this.objects = new List<Object>();
            this.loaded = false;
        }
    }

    /// <summary>
    /// Represents the starting position of a map block.
    /// So its X and Y would make a map block's start position (X * BlockSize, 0, Y * BlockSize).
    /// </summary>
    public struct MapBlockPosition
    {
        /// <summary>
        /// X-coordinate.
        /// </summary>
        public int X;
        /// <summary>
        /// Y-coordinate, which will be the z-coordinates in world position.
        /// </summary>
        public int Y;

        /// <summary>
        /// Constructs a MapBlockPosition object that uses the specified x- and y-coordinates.
        /// </summary>
        /// <param name="x">X-coordinate of the map block position.</param>
        /// <param name="y">Y-coordinate of the map block position.</param>
        public MapBlockPosition(int x, int y)
        {
            X = x; Y = y;
        }

        /// <summary>
        /// Constructs an invalid MapBlockPosition object.
        /// </summary>
        public static MapBlockPosition Invalid
        {
            get { return new MapBlockPosition(int.MaxValue, int.MaxValue); }
        }

        /// <summary>
        /// Constructs a MapBlockPosition object with both x- and y-coordinates zeros.
        /// </summary>
        public static MapBlockPosition Zero
        {
            get { return new MapBlockPosition(0, 0); }
        }

        /// <summary>
        /// An overloadeded operator to compare the equality of two MapBlockPosition objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(MapBlockPosition left, MapBlockPosition right)
        {
            return left.X == right.X && left.Y == right.Y;
        }

        /// <summary>
        /// An overloaded operator to compare the inequality of two MapBlockPosition objects.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(MapBlockPosition left, MapBlockPosition right)
        {
            return left.X != right.X || left.Y != right.Y;
        }

        /// <summary>
        /// Checks if the current MapBlockPosition object is equal to the other passed in.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            MapBlockPosition other = (MapBlockPosition)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        /// <summary>
        /// Gets the hash code of the current MapBlockPosition object.
        /// </summary>
        /// <returns></returns>
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

    /// <summary>
    /// Contains the information of a map, that could be used to display on the GUI.
    /// </summary>
    public class MapInfo
    {
        /// <summary>
        /// Path to the map configuration file, relative to the game's root path.
        /// </summary>
        public string Path;
        /// <summary>
        /// Name of the map defined in the configuration file.
        /// </summary>
        public string Name;
    }

    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// Gets the dimension of a map block.
        /// </summary>
        /// <remarks>
        /// Although this can be changed at map display settings configuration.
        /// Changing this could cause issues with displaying the map.
        /// </remarks>
        public static int BlockSize
        {
            get { return blockSize; }
        }

        /// <summary>
        /// Gets a list of info (ex. position) of each map block.
        /// </summary>
        public List<MapBlockInfo> BlockInfoList
        {
            get { return blockInfoList; }
        }

        /// <summary>
        /// Gets the number of map blocks loaded to the graphics buffer.
        /// </summary>
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

        /// <summary>
        /// Initializes a map that uses the specified map path.
        /// </summary>
        /// <param name="path">Absolute path to the map's main configuration file, which should have .map extension.</param>
        public Map(string path)
        {
            mapDirectory = Path.GetDirectoryName(path);
            loadedSkies = new List<Sky>();
            blockInfoList = new List<MapBlockInfo>();
            blocksToLoad = new List<MapBlockInfo>();
            loadedBlocks = new List<MapBlock>();
            loadedTerrains = new List<Terrain>();
        }

        /// <summary>
        /// Checks if a map block of the specified position exists in the map.
        /// </summary>
        /// <param name="position">Map block position to check.</param>
        /// <returns></returns>
        public bool BlockExists(MapBlockPosition position)
        {
            return loadedBlocks.Find(b => b.Position == position) != null;
        }

        /// <summary>
        /// Gets the map block position of the map block a world position belongs to.
        /// </summary>
        /// <param name="position">World position of the map.</param>
        /// <returns>
        /// The corresponding map block position if one is found.
        /// If not, then an invalid position will be returned.
        /// </returns>
        public MapBlockPosition GetBlockPosition(Vector3f position)
        {
            int blockX = (int)(position.X / blockSize),
                blockY = (int)(position.Z / blockSize);
            if (position.X < 0)
                blockX -= 1;
            if (position.Z < 0)
                blockY -= 1;
            MapBlockPosition result = new MapBlockPosition(blockX, blockY);
            if (BlockExists(result))
                return result;
            else
                return MapBlockPosition.Invalid;
        }

        /// <summary>
        /// Gets the height of the terrain at specified map block position and position.
        /// </summary>
        /// <param name="blockPosition">Map block position of the map block.</param>
        /// <param name="position">Position, relative to the map block's starting position.</param>
        /// <returns>
        /// The terrain's height at the specified point if one is found.
        /// If not, a zero will be returned.
        /// </returns>
        public float GetTerrainHeight(MapBlockPosition blockPosition, Vector3f position)
        {
            Terrain terrain = loadedTerrains.Find(t => t.Position == blockPosition);
            int x = (int)position.X % BlockSize,
                y = (int)position.Z % BlockSize;
            if (terrain != null && x >= 0 && x <= BlockSize && y >= 0 && y <= BlockSize)
                // TODO: implement a more accurate calculation
                return terrain.Heights[x][y];
            else
                return 0;
        }

        /// <summary>
        /// Checks if a world position specified falls within the map.
        /// </summary>
        /// <param name="position">World position to check.</param>
        /// <returns></returns>
        public bool IsInMap(Vector3f position)
        {
            MapBlockPosition blockPosition = GetBlockPosition(position);
            if (blockPosition == MapBlockPosition.Invalid)
                return false;
            float terrainHeight = GetTerrainHeight(blockPosition, position);
            return position.Y >= terrainHeight;
        }

        /// <summary>
        /// Adds a map block and a terrain to the map, both should have the same map block position.
        /// If they don't, then none of them will be added to the map.
        /// </summary>
        /// <param name="block">Map block that contains the defintions of the objects.</param>
        /// <param name="terrain">Terrain that contains the definitions of the heights at each vertex.</param>
        /// <returns></returns>
        internal bool AddBlockAndTerrain(MapBlock block, Terrain terrain)
        {
            if (block.Position != terrain.Position)
                return false;

            if (currentBlock == null)
                currentBlock = block;
            loadedBlocks.Add(block);

            if (currentTerrain == null)
                currentTerrain = terrain;
            loadedTerrains.Add(terrain);

            return true;
        }

        /// <summary>
        /// Adds a map block to the load queue. So the map loading thread will be able to pick it up,
        /// and then loads the block to the graphics buffer.
        /// </summary>
        /// <param name="blockInfo">Map block's information, including the map block position and paths to the configuration files.</param>
        internal void AddBlockToLoad(MapBlockInfo blockInfo)
        {
            if (!BlockExists(blockInfo.Position))
                blocksToLoad.Add(blockInfo);
        }

        /// <summary>
        /// Adds a sky object to the map. This will also set the current sky to this sky if none has been specified yet.
        /// </summary>
        /// <param name="sky">Sky object to add.</param>
        internal void AddSky(Sky sky)
        {
            loadedSkies.Add(sky);
            if (currentSky == null)
                currentSky = sky;
        }

        /// <summary>
        /// Changes the sky to a different one, with the mode specified.
        /// </summary>
        /// <param name="mode">Mode of the sky to switch to.</param>
        /// <returns>True if the switch was successful. False if not, because the sky with specified mode does not exist.</returns>
        internal bool ChangeSky(SkyMode mode)
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

        /// <summary>
        /// Calls the map block loading thread to load the blocks in queue.
        /// Only one block will be loaded by the thread at a time.
        /// The blocks will not be loaded until the thread has completed the loading job.
        /// If the block has been successfully put to the thread for loading, 
        /// then it will be removed from the queue.
        /// </summary>
        /// <notes>
        /// This method should be called only if the current block position is changed, and
        /// blocks are going to be added and/or removed.
        /// </notes>
        internal void LoadBlocksInQueue()
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

        /// <summary>
        /// Loads the current sky into the graphics buffer. If no Sky object exists, then it will simply do nothing.
        /// </summary>
        internal void LoadCurrentSky()
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

        /// <summary>
        /// Unloads a block from the graphics buffer.
        /// </summary>
        /// <param name="blockInfo">Map block's info with map block position, which is the target block to be unloaded.</param>
        internal void UnloadBlock(MapBlockInfo blockInfo)
        {

        }
    }

    /// <summary>
    /// Represents an interface used by a separate thread to load the objects and the terrain of a map block.
    /// </summary>
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

        /// <summary>
        /// Gets whether the current map block has been loaded into the graphics buffer or not.
        /// </summary>
        public static bool LoadedInfoBuffer
        {
            get { return loadedIntoBuffer; }
        }

        /// <summary>
        /// Gets whether loading of the objects and terrain of the map block has been completed or not.
        /// </summary>
        public static bool LoadCompleted
        {
            get { return loadCompleted; }
        }

        /// <summary>
        /// Gets or sets whether this loader is in use. So the current loading job won't be interfered.
        /// </summary>
        public static bool LoaderInUse
        {
            get { return loaderInUse; }
            set { loaderInUse = value; }
        }

        /// <summary>
        /// Gets the progress of the current loading job.
        /// </summary>
        public static double Progress
        {
            get { return progress; }
        }

        /// <summary>
        /// Starts to load the objects and terrain of the specified map block.
        /// </summary>
        /// <param name="mapDirectory">The directory where the map configuration files are stored.</param>
        /// <param name="blockInfo">The info of the map block to be loaded, including the position and paths to the configuration files.</param>
        /// <notes>
        /// This method must be called by a separate thread, or the main thread will wait for it that makes the UI frozen.
        /// </notes>
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
                        mapObject.Position.Y, mapObject.Position.Z + blockToLoad.Position.Y * Map.BlockSize,
                        mapObject.Rotations.X, mapObject.Rotations.Y, mapObject.Rotations.Z);
                    entities.Add(entity);
                    meshes.Add(staticMesh);
                }
                progress += (1 / numOfObjects) * 0.9;
            }

            // Spline loading as well


            // Save the loaded block and terrain
            block = blockToLoad;
            terrain = terrainToLoad;

            progress = 1.0;
            loadCompleted = true;
        }

        /// <summary>
        /// Loads the loaded objects and terrain into the graphics buffer, only if the loading has been completed.
        /// </summary>
        /// <notes>
        /// This must be called by the main thread, or the thread that contains the graphics context.
        /// If not, then the objects and textures will not be able to load into the graphics buffers.
        /// </notes>
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

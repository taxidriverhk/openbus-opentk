using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;
using OpenBus.Config;
using OpenBus.Game.Objects;

namespace OpenBus.Game.Controls
{
    public static class ConfigLoader
    {
        public static Map LoadMap(string path)
        {
            Map map = new Map();
            MapEx mapEx = XmlDeserializeHelper<MapEx>.DeserializeFromFile(path);
            if (mapEx != null)
            {
                foreach (MapEx.BlockInfo blockInfoEx in mapEx.Blocks)
                {
                    MapBlockInfo blockInfo = new MapBlockInfo(
                        new MapBlockPosition(blockInfoEx.Position.X, blockInfoEx.Position.Y),
                        blockInfoEx.Path, blockInfoEx.TerrainPath);
                    map.AddBlockInfo(blockInfo);
                }

                MapEx.SkyInfo skyInfoEx = mapEx.Sky;
                foreach (MapEx.SkyInfo.SkyTexture texture in skyInfoEx.Textures)
                {
                    switch (texture.Mode)
                    {
                        case MapEx.SkyInfo.SkyTextureMode.Day:
                            map.AddSky(new Sky(skyInfoEx.Size, SkyMode.DAY, texture.Path));
                            break;
                        case MapEx.SkyInfo.SkyTextureMode.Night:
                            map.AddSky(new Sky(skyInfoEx.Size, SkyMode.NIGHT, texture.Path));
                            break;
                        case MapEx.SkyInfo.SkyTextureMode.Dawn:
                            map.AddSky(new Sky(skyInfoEx.Size, SkyMode.DAWN, texture.Path));
                            break;
                        case MapEx.SkyInfo.SkyTextureMode.Sunset:
                            map.AddSky(new Sky(skyInfoEx.Size, SkyMode.SUNSET, texture.Path));
                            break;
                    }
                }
                return map;
            }
            else
            {
                Log.Write(LogLevel.Error, "Unable to load the map config file {0}.", path);
                return null;
            }
        }

        public static MapBlock LoadMapBlock(string path, MapBlockPosition position)
        {
            MapBlockEx blockEx = XmlDeserializeHelper<MapBlockEx>.DeserializeFromFile(path);
            if (blockEx != null)
            {
                MapBlock block = new MapBlock();
                foreach (ObjectInfo objectInfo in blockEx.Objects)
                {
                    ObjectEx objectEx = XmlDeserializeHelper<ObjectEx>
                        .DeserializeFromFile(EnvironmentVariables.RootPath + objectInfo.Path);
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
                    block.AddObject(new Objects.Object(objectInfo.Position,
                        objectInfo.Rotations, meshPaths, objectInfo.Path,
                        objectEx.Info.ModelDirectory,
                        objectEx.Info.TextureDirectory, alphaTextures));
                }
                return block;
            }
            else
            {
                Log.Write(LogLevel.Error, "Unable to load the map block config file {0}.", path);
                return null;
            }
        }

        public static MapInfo LoadMapInfo(string path)
        {
            MapInfo mapInfo = new MapInfo();
            MapEx mapEx = XmlDeserializeHelper<MapEx>.DeserializeFromFile(path);
            if (mapEx != null)
            {
                mapInfo.Name = mapEx.Info.Name;
                mapInfo.Path = path;
            }
            return mapInfo;
        }

        public static Terrain LoadTerrain(string path, MapBlockPosition position)
        {
            TerrainEx terrainEx = XmlDeserializeHelper<TerrainEx>.DeserializeFromFile(path);
            if (terrainEx != null)
            {
                float[][] heights = new float[Map.BlockSize][];
                for (int i = 0; i < Map.BlockSize; i++)
                    heights[i] = new float[Map.BlockSize];
                foreach (TerrainEx.TerrainDisplacement displacementEx in terrainEx.Displacements)
                    heights[displacementEx.X][displacementEx.Y] = displacementEx.Displacement;
                return new Terrain(terrainEx.Texture.Path, terrainEx.Texture.UV,
                    position, heights);
            }
            else
            {
                Log.Write(LogLevel.Error, "Unable to load the terrain config file {0}.", path);
                return null;
            }
        }

        public static Objects.Object LoadObject(string path, Vector3f position, Vector3f rotations)
        {
            ObjectEx objectEx = XmlDeserializeHelper<ObjectEx>.DeserializeFromFile(path);
            if (objectEx != null)
            {
                ObjectTexture[] textures = new ObjectTexture[objectEx.AlphaTextures.Length];
                string[] meshPaths = new string[objectEx.Meshes.Length];

                for (int i = 0; i < textures.Length; i++)
                    textures[i] = new ObjectTexture(objectEx.AlphaTextures[i].Path, 
                        (ObjectTextureAlphaMode)objectEx.AlphaTextures[i].Mode);
                for (int i = 0; i < meshPaths.Length; i++)
                    meshPaths[i] = objectEx.Meshes[i].Path;

                return new Objects.Object(position, rotations, meshPaths, path, objectEx.Info.ModelDirectory,
                    objectEx.Info.TextureDirectory, textures);
            }
            else
            {
                Log.Write(LogLevel.Error, "Unable to load the object config file {0}.", path);
                return null;
            }
        }
    }
}

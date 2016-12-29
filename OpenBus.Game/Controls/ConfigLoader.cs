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
            Map map = new Map(path);
            MapConfig mapConfig = XmlDeserializeHelper<MapConfig>.DeserializeFromFile(path);
            if (mapConfig != null)
            {
                foreach (MapConfig.BlockInfo blockInfoConfig in mapConfig.Blocks)
                {
                    MapBlockInfo blockInfo = new MapBlockInfo(
                        new MapBlockPosition(blockInfoConfig.Position.X, blockInfoConfig.Position.Y),
                        blockInfoConfig.Path, blockInfoConfig.TerrainPath);
                    map.BlockInfoList.Add(blockInfo);
                }

                MapConfig.SkyInfo skyInfoConfig = mapConfig.Sky;
                foreach (MapConfig.SkyInfo.SkyTexture texture in skyInfoConfig.Textures)
                {
                    switch (texture.Mode)
                    {
                        case MapConfig.SkyInfo.SkyTextureMode.Day:
                            map.AddSky(new Sky(skyInfoConfig.Size, SkyMode.DAY, texture.Path));
                            break;
                        case MapConfig.SkyInfo.SkyTextureMode.Night:
                            map.AddSky(new Sky(skyInfoConfig.Size, SkyMode.NIGHT, texture.Path));
                            break;
                        case MapConfig.SkyInfo.SkyTextureMode.Dawn:
                            map.AddSky(new Sky(skyInfoConfig.Size, SkyMode.DAWN, texture.Path));
                            break;
                        case MapConfig.SkyInfo.SkyTextureMode.Sunset:
                            map.AddSky(new Sky(skyInfoConfig.Size, SkyMode.SUNSET, texture.Path));
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
            MapBlockConfig blockConfig = XmlDeserializeHelper<MapBlockConfig>.DeserializeFromFile(path);
            if (blockConfig != null)
            {
                MapBlock block = new MapBlock(position);
                foreach (ObjectInfo objectInfo in blockConfig.Objects)
                {
                    ObjectConfig objectConfig = XmlDeserializeHelper<ObjectConfig>
                        .DeserializeFromFile(EnvironmentVariables.RootPath + objectInfo.Path);
                    string[] meshPaths = new string[objectConfig.Meshes.Length];
                    ObjectTexture[] alphaTextures = null;
                    if (objectConfig.AlphaTextures != null)
                        alphaTextures = new ObjectTexture[objectConfig.AlphaTextures.Length];
                    for (int i = 0; i < objectConfig.Meshes.Length; i++)
                        meshPaths[i] = objectConfig.Meshes[i].Path;
                    if (alphaTextures != null)
                        for (int i = 0; i < objectConfig.AlphaTextures.Length; i++)
                            alphaTextures[i] = new ObjectTexture(objectConfig.AlphaTextures[i].Path,
                                (ObjectTextureAlphaMode)objectConfig.AlphaTextures[i].Mode);
                    block.Objects.Add(new Objects.Object(objectInfo.Position,
                        objectInfo.Rotations, meshPaths, objectInfo.Path,
                        objectConfig.Info.ModelDirectory,
                        objectConfig.Info.TextureDirectory, alphaTextures));
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
            MapConfig mapConfig = XmlDeserializeHelper<MapConfig>.DeserializeFromFile(path);
            if (mapConfig != null)
            {
                mapInfo.Name = mapConfig.Info.Name;
                mapInfo.Path = path;
            }
            return mapInfo;
        }

        public static Terrain LoadTerrain(string path, MapBlockPosition position)
        {
            TerrainConfig terrainConfig = XmlDeserializeHelper<TerrainConfig>.DeserializeFromFile(path);
            if (terrainConfig != null)
            {
                float[][] heights = new float[Map.BlockSize + 1][];
                for (int i = 0; i <= Map.BlockSize; i++)
                    heights[i] = new float[Map.BlockSize + 1];
                foreach (TerrainConfig.TerrainDisplacement displacementConfig in terrainConfig.Displacements)
                    heights[displacementConfig.X][displacementConfig.Y] = displacementConfig.Displacement;
                return new Terrain(terrainConfig.Texture.Path, terrainConfig.Texture.UV,
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
            ObjectConfig objectConfig = XmlDeserializeHelper<ObjectConfig>.DeserializeFromFile(path);
            if (objectConfig != null)
            {
                ObjectTexture[] textures = new ObjectTexture[objectConfig.AlphaTextures.Length];
                string[] meshPaths = new string[objectConfig.Meshes.Length];

                for (int i = 0; i < textures.Length; i++)
                    textures[i] = new ObjectTexture(objectConfig.AlphaTextures[i].Path, 
                        (ObjectTextureAlphaMode)objectConfig.AlphaTextures[i].Mode);
                for (int i = 0; i < meshPaths.Length; i++)
                    meshPaths[i] = objectConfig.Meshes[i].Path;

                return new Objects.Object(position, rotations, meshPaths, path, objectConfig.Info.ModelDirectory,
                    objectConfig.Info.TextureDirectory, textures);
            }
            else
            {
                Log.Write(LogLevel.Error, "Unable to load the object config file {0}.", path);
                return null;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;

namespace OpenBus.Game.Objects
{
    public enum ObjectTextureAlphaMode
    {
        FULL = 0,
        ALPHA = 1
    }

    public class ObjectTexture
    {
        private string path;
        private ObjectTextureAlphaMode mode;

        public string Path
        {
            get { return path; }
        }

        public ObjectTextureAlphaMode Mode
        {
            get { return mode; }
        }

        public ObjectTexture(string path, ObjectTextureAlphaMode mode)
        {
            this.path = path;
            this.mode = mode;
        }
    }

    public class Object
    {
        private Vector3f position;
        private Vector3f rotations;
        private string[] meshes;
        private ObjectTexture[] alphaTextures;
        private string modelDirectory;
        private string textureDirectory;
        private string path;

        public string Path
        {
            get { return path; }
        }

        public string ModelDirectory
        {
            get { return modelDirectory; }
        }

        public string TextureDirectory
        {
            get { return textureDirectory; }
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

        public ObjectTexture[] AlphaTextures
        {
            get { return alphaTextures; }
        }

        public Object(Vector3f position, Vector3f rotations, string[] meshes, string path, 
            string modelDirectory, string textureDirectory, ObjectTexture[] alphaTextures)
        {
            this.position = position;
            this.rotations = rotations;
            this.meshes = meshes;
            this.path = path;
            this.modelDirectory = modelDirectory;
            this.textureDirectory = textureDirectory;
            this.alphaTextures = alphaTextures;
        }
    }
}

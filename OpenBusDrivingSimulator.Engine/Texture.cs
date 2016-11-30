using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace OpenBusDrivingSimulator.Engine
{
    public struct Texture
    {
        public int TextureId;
        public bool HasAlpha;
        public string Path;

        public Texture(int textureId)
        {
            TextureId = textureId;
            HasAlpha = false;
            Path = string.Empty;
        }

        public Texture(string path, bool hasAlpha)
        {
            TextureId = 0;
            HasAlpha = hasAlpha;
            Path = path;
        }

        public Texture(int textureId, bool hasAlpha)
        {
            TextureId = textureId;
            HasAlpha = hasAlpha;
            Path = string.Empty;
        }

        public Texture(int textureId, bool hasAlpha, string path)
        {
            TextureId = textureId;
            HasAlpha = hasAlpha;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            Texture other = (Texture)obj;
            return this.TextureId == other.TextureId;
        }

        public override int GetHashCode()
        {
            return this.TextureId;
        }
    }

    /// <summary>
    /// Manages all the texture data loaded into the memory.
    /// </summary>
    public static class TextureManager
    {
        /// <summary>
        /// List of supported image format to load into.
        /// </summary>
        private static readonly string[] supportedFormats = { ".bmp" };

        private static HashSet<Texture> textures;
        private static List<Texture> textureLoadQueue;


        /// <summary>
        /// List of IDs where each of them points to the data allocated to the graphics memory.
        /// </summary>
        public static HashSet<Texture> Textures
        {
            get { return textures; }
        }

        static TextureManager()
        {
            textures = new HashSet<Texture>();
            textureLoadQueue = new List<Texture>();
        }

        public static int GetTextureId(Texture target)
        {
            foreach (Texture texture in textures)
                if (target.Path == texture.Path)
                    return texture.TextureId;
            return 0;
        }

        /// <summary>
        /// Loads a bitmap into the graphics memory.
        /// </summary>
        /// <param name="bitmap">The bitmap that contains the data of the image.</param>
        /// <returns>
        /// The texture ID allocated to the graphics memory.
        /// -1 if the bitmap is null.
        /// </returns>
        public static int LoadTexture(Bitmap bitmap, bool hasAlpha, bool addToList)
        {
            if (bitmap == null)
                return -1;

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0,
                bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            Texture texture = new Texture(id, hasAlpha);
            if (addToList)
                textures.Add(texture);
            return id;
        }

        public static int LoadTexture(Bitmap bitmap)
        {
            return LoadTexture(bitmap, false, true);
        }

        /// <summary>
        /// Loads an image file into the graphics memory.
        /// </summary>
        /// <param name="path">
        /// The full absolute path of the image, without extension. This method will append the extension of every supported format and will use the first one that could be loaded.
        /// </param>
        /// <param name="hasAlpha">
        /// </param>
        /// <returns>
        /// The texture ID allocated to the graphics memory.
        /// -1 if the bitmap is null.
        /// </returns>
        public static int LoadTexture(string path, bool hasAlpha)
        {
            string fullPath = path;
            if (!fullPath.Contains("."))
                foreach (string supportedFormat in supportedFormats)
                {
                    fullPath = path + supportedFormat;
                    if (string.IsNullOrEmpty(fullPath)
                        || !File.Exists(fullPath))
                        return -1;
                }
            else if (!File.Exists(fullPath))
                return -1;

            Bitmap bitmap = new Bitmap(fullPath);
            if (hasAlpha && bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                Bitmap bitmapWithAlpha = new Bitmap(bitmap.Width, bitmap.Height, 
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics gfx = Graphics.FromImage(bitmapWithAlpha))
                    gfx.DrawImage(bitmap, rect, rect, GraphicsUnit.Pixel);
                bitmapWithAlpha.MakeTransparent();
                bitmap = bitmapWithAlpha;
            }
            int textureId = LoadTexture(bitmap, hasAlpha, false);
            textures.Add(new Texture(textureId, hasAlpha, path));
            return textureId;
        }

        public static int LoadTexture(string path)
        {
            return LoadTexture(path, false);
        }

        public static int LoadTexture(Texture texture)
        {
            int textureId = LoadTexture(texture.Path, texture.HasAlpha);
            texture.TextureId = textureId;
            return textureId;
        }

        public static void LoadAllTexturesInQueue()
        {
            foreach (Texture texture in textureLoadQueue)
                LoadTexture(texture);
            textureLoadQueue.Clear();
        }

        public static Texture PutIntoLoadQueue(string path, bool hasAlpha)
        {
            Texture texture = new Texture(path, hasAlpha);
            textureLoadQueue.Add(texture);
            return texture;
        }

        /// <summary>
        /// Unloads a texture data from the graphics memory.
        /// </summary>
        /// <param name="textureId">The texture ID allocated to the graphics memory.</param>
        public static void UnloadTexture(int textureId)
        {
            foreach (Texture texture in textures)
                if (texture.TextureId == textureId)
                {
                    GL.DeleteTexture(texture.TextureId);
                    textures.Remove(texture);
                    return;
                }
        }

        /// <summary>
        /// Unloads all textures from the graphics memory.
        /// </summary>
        public static void UnloadAllTextures()
        {
            foreach (Texture texture in textures)
                GL.DeleteTexture(texture.TextureId);
        }
    }
}

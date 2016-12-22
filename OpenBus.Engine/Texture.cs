using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace OpenBus.Engine
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
        private struct TextureLoadQueueItem
        {
            public Texture Texture;
            public Bitmap Bitmap;

            public override bool Equals(object obj)
            {
                TextureLoadQueueItem other = (TextureLoadQueueItem)obj;
                return this.Texture.Path == other.Texture.Path;
            }

            public override int GetHashCode()
            {
                return this.Texture.Path.GetHashCode();
            }
        }

        /// <summary>
        /// List of supported image format to load into.
        /// </summary>
        private static readonly string[] supportedFormats = { ".bmp" };
        private static HashSet<Texture> textures;
        private static HashSet<TextureLoadQueueItem> textureLoadQueue;

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
            textureLoadQueue = new HashSet<TextureLoadQueueItem>();
        }

        public static int GetTextureId(Texture target)
        {
            foreach (Texture texture in textures)
                if (target.Path == texture.Path)
                    return texture.TextureId;
            return 0;
        }

        public static int LoadTexture(Bitmap bitmap)
        {
            int textureId = LoadTexture(bitmap, true);
            return textureId;
        }

        public static int LoadTexture(Bitmap bitmap, bool disposeBitmap)
        {
            int textureId = LoadTexture(bitmap, false, true, disposeBitmap);
            return textureId;
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
            int textureId = LoadTexture(bitmap, hasAlpha, false, true);
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
            foreach (TextureLoadQueueItem item in textureLoadQueue)
            {
                int textureId = LoadTexture(item.Bitmap, item.Texture.HasAlpha, false, true);
                Texture loadedTexture = new Texture(item.Texture.Path, item.Texture.HasAlpha);
                loadedTexture.TextureId = textureId;
                textures.Add(loadedTexture);
            }
            textureLoadQueue.Clear();
        }

        public static Texture PutIntoLoadQueue(string path, bool hasAlpha)
        {
            TextureLoadQueueItem item = new TextureLoadQueueItem();
            item.Bitmap = new Bitmap(path);
            if (item.Bitmap != null)
            {
                item.Texture = new Texture(path, hasAlpha);
                textureLoadQueue.Add(item);
                return item.Texture;
            }
            return new Texture();
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
            textures.Clear();
        }

        private static Bitmap GetAlphaBitmap(Bitmap bitmap)
        {
            if (bitmap != null)
            {
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                Bitmap bitmapWithAlpha = new Bitmap(bitmap.Width, bitmap.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics gfx = Graphics.FromImage(bitmapWithAlpha))
                    gfx.DrawImage(bitmap, rect, rect, GraphicsUnit.Pixel);
                bitmapWithAlpha.MakeTransparent();
                bitmap.Dispose();
                return bitmapWithAlpha;
            }
            else
                return bitmap;
        }

        private static int LoadTexture(Bitmap bitmap, bool hasAlpha, bool addToList, bool disposeBitmap)
        {
            if (bitmap == null)
                return -1;

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);
            if (hasAlpha && bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                bitmap = GetAlphaBitmap(bitmap);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0,
                bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bitmapData.Width, bitmapData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);
            if (disposeBitmap)
                bitmap.Dispose();

            Texture texture = new Texture(id, hasAlpha);
            // Either let this function add the texture to the hash set
            // Or let the caller (which should be a TextureManager method too) do it
            if (addToList)
                textures.Add(texture);
            return id;
        }
    }
}

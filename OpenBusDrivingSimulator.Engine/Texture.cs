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
    /// <summary>
    /// Manages all the texture data loaded into the memory.
    /// </summary>
    public static class Texture
    {
        /// <summary>
        /// List of supported image format to load into.
        /// </summary>
        private static readonly string[] supportedFormats = { ".bmp" };

        /// <summary>
        /// List of IDs where each of them points to the data allocated to the graphics memory.
        /// </summary>
        private static List<int> textureIds = new List<int>();
        public static List<int> TextureIds
        {
            get { return textureIds; }
        }

        /// <summary>
        /// Loads a bitmap into the graphics memory.
        /// </summary>
        /// <param name="bitmap">The bitmap that contains the data of the image.</param>
        /// <returns>
        /// The texture ID allocated to the graphics memory.
        /// -1 if the bitmap is null.
        /// </returns>
        public static int LoadTextureFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                return -1;

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0,
                bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            textureIds.Add(id);
            return id;
        }

        /// <summary>
        /// Loads an image file into the graphics memory.
        /// </summary>
        /// <param name="filename">
        /// The full absolute path of the image, without extension. This method will append the extension of every supported format and will use the first one that could be loaded.
        /// </param>
        /// <returns>
        /// The texture ID allocated to the graphics memory.
        /// -1 if the bitmap is null.
        /// </returns>
        public static int LoadTextureFromFile(string filename)
        {
            string fullPath = filename;
            if (!fullPath.Contains("."))
                foreach (string supportedFormat in supportedFormats)
                {
                    fullPath = filename + supportedFormat;
                    if (string.IsNullOrEmpty(fullPath)
                        || !File.Exists(fullPath))
                        return -1;
                }
            else if (!File.Exists(fullPath))
                return -1;

            Bitmap bitmap = new Bitmap(fullPath);
            return LoadTextureFromBitmap(bitmap);
        }

        /// <summary>
        /// Unloads a texture data from the graphics memory.
        /// </summary>
        /// <param name="textureId">The texture ID allocated to the graphics memory.</param>
        public static void UnloadTexture(int textureId)
        {
            if (textureIds.Contains(textureId))
            {
                GL.DeleteTexture(textureId);
                textureIds.Remove(textureId);
            }
        }

        /// <summary>
        /// Unloads all textures from the graphics memory.
        /// </summary>
        public static void UnloadAllTextures()
        {
            foreach (int texture in textureIds)
                GL.DeleteTexture(texture);
        }
    }
}

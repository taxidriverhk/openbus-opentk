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
    public static class Texture
    {
        private static List<int> textureIds = new List<int>();

        public static List<int> TextureIds
        {
            get { return textureIds; }
        }

        public static int LoadTextureFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename)
                || !File.Exists(filename))
                return -1;

            int id = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, id);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                (int)TextureMagFilter.Linear);

            Bitmap bitmap = new Bitmap(filename);

            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0,
                bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, bitmapData.Scan0);
            bitmap.UnlockBits(bitmapData);

            textureIds.Add(id);
            return id;
        }

        public static void LoadTestTextures()
        {
            LoadTextureFromFile(@"D:\Games\OMSI 2\Sceneryobjects\taxidriverhk_meifoo\texture\mf_mfmtr_1.bmp");
            LoadTextureFromFile(@"D:\Games\OMSI 2\Sceneryobjects\taxidriverhk_meifoo\texture\mf_phase6_10.bmp");
        }

        public static void UnloadAllTextures()
        {
            foreach (int texture in textureIds)
                GL.DeleteTexture(texture);
        }
    }
}

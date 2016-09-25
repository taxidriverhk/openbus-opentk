using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using OpenBusDrivingSimulator.Core;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Renderer
    {
        private static float speed = 2.0f;
        private static float displacement = 0.0f;

        private static Bitmap textBmp;

        public static void Initialize()
        {
            GL.Enable(EnableCap.Texture2D);
            textBmp = new Bitmap(Screen.Width, Screen.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        public static void DrawText(string text, int x, int y, Color color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            Brush brush = new SolidBrush(color);
            Font font = new Font(FontFamily.GenericSansSerif, 20.0f);
            using(Graphics gfx = Graphics.FromImage(textBmp))
            {
                gfx.Clear(Color.Transparent);
                gfx.DrawString(text, font, brush, new PointF(0, Screen.Height - 30));
            }

            int textTextureId = Texture.LoadTextureFromBitmap(textBmp);

            GL.Disable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.BindTexture(TextureTarget.Texture2D, textTextureId);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(1.0f, 1.0f);
            GL.Vertex3(-Camera.PlaneWidth/2 + Camera.Target.X, -Camera.PlaneHeight/2 + Camera.Target.Y, Camera.PlaneZPosition);
            GL.TexCoord2(0.0f, 1.0f); 
            GL.Vertex3(Camera.PlaneWidth/2 + Camera.Target.X, -Camera.PlaneHeight/2 + Camera.Target.Y, Camera.PlaneZPosition);
            GL.TexCoord2(0.0f, 0.0f); 
            GL.Vertex3(Camera.PlaneWidth/2 + Camera.Target.X, Camera.PlaneHeight/2 + Camera.Target.Y, Camera.PlaneZPosition);
            GL.TexCoord2(1.0f, 0.0f); 
            GL.Vertex3(-Camera.PlaneWidth/2 + Camera.Target.X, Camera.PlaneHeight/2 + Camera.Target.Y, Camera.PlaneZPosition);
            GL.End();

            Texture.UnloadTexture(textTextureId);
        }

        public static void DrawText(string text, int x, int y)
        {
            DrawText(text, x, y, Color.White);
        }

        #region Test Functions
        public static void RenderTest(double timeElapsed)
        {
            GL.ClearColor(Color.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            displacement += (float)timeElapsed * speed;
            DrawTestCube();
            if (displacement >= 5 || displacement <= -5)
            {
                displacement = displacement < 0 ? -5 : 5;
                speed *= -1;
            }
        }

        public static void DrawTestCube()
        {
            float zPosition = 10.0f;
            Vector3[] vertices =
            {
                new Vector3(displacement - 1.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement + 1.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement + 1.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement - 1.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition - Camera.Eye.Z),
                new Vector3(displacement - 1.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement + 1.0f - Camera.Eye.X, -1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement + 1.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
                new Vector3(displacement - 1.0f - Camera.Eye.X, 1.0f - Camera.Eye.Y, zPosition + 2.0f - Camera.Eye.Z),
            };

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.Begin(PrimitiveType.Quads);

            // Front
            GL.TexCoord2(1.0f, 1.0f); 
            GL.Vertex3(vertices[0]);
            GL.TexCoord2(0.0f, 1.0f);
            GL.Vertex3(vertices[1]);
            GL.TexCoord2(0.0f, 0.0f);
            GL.Vertex3(vertices[2]);
            GL.TexCoord2(1.0f, 0.0f);
            GL.Vertex3(vertices[3]);

            // Back

            // Top

            // Bottom

            // Left

            // Right

            GL.End();
        }
        #endregion
    }
}

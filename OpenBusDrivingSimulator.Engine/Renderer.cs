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
        private static double speed = 2.0;
        private static double displacement = 0;

        private static float fieldOfAngle = MathHelper.PiOver4;
        private static float zNear = 1, zFar = 10;
        private static float aspect = 1;

        private static Vector2 screenVect2;
        private static Bitmap textBmp;

        public static void Initialize()
        {
            GL.Enable(EnableCap.Texture2D);

            aspect = Screen.Width / Screen.Height;

            float widthOver2 = zFar * (float)System.Math.Tan(fieldOfAngle / 2),
                  heightOver2 = widthOver2 / aspect;
            screenVect2 = new Vector2(widthOver2, heightOver2);

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
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(-screenVect2.X, -screenVect2.Y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(screenVect2.X, -screenVect2.Y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(screenVect2.X, screenVect2.Y);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(-screenVect2.X, screenVect2.Y);
            GL.End();

            Texture.UnloadTexture(textTextureId);
        }

        public static void DrawText(string text, int x, int y)
        {
            DrawText(text, x, y, Color.White);
        }

        #region Test Functions
        public static void MoveCameraTest()
        {
            Matrix4 lookAt = Matrix4.LookAt(0.0f, 0.0f, 10.0f,
                0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref lookAt);

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(fieldOfAngle, aspect, zNear, zFar);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        public static void RenderTest(double timeElapsed)
        {
            GL.ClearColor(Color.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            displacement += timeElapsed * speed;
            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[0]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(displacement - 1.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(displacement + 1.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(displacement + 1.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(displacement - 1.0f, 1.0f);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, Texture.TextureIds[1]);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(displacement - 4.0f, -1.0f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(displacement - 3.0f, -1.0f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(displacement - 3.0f, 1.0f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(displacement - 4.0f, 1.0f);
            GL.End();

            if (displacement >= 5 || displacement <= -5)
            {
                displacement = displacement < 0 ? -5 : 5;
                speed *= -1;
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenBus.Engine
{
    public class Ray
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;

        public Ray(Vector3 startPoint, Vector3 endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }

    internal class GraphicsHelper
    {
        private struct Color
        {
            public byte R;
            public byte G;
            public byte B;
            public byte A;
        }

        internal static Matrix4 CreateModelMatrix(Vector3 translation, Vector3 rotation, Vector3 scale)
        {
            Matrix4 modelMatrix = Matrix4.Identity;
            // Scale about the origin -> rotate about the origin -> translate
            modelMatrix *= Matrix4.CreateScale(scale);
            modelMatrix *= Matrix4.CreateRotationX(rotation.X);
            modelMatrix *= Matrix4.CreateRotationY(rotation.Y);
            modelMatrix *= Matrix4.CreateRotationZ(rotation.Z);
            modelMatrix *= Matrix4.CreateTranslation(translation);
            return modelMatrix;
        }

        internal static Vector3 UnProject(Vector3 window)
        {
            // Algorithm inspired by https://capnramses.github.io//opengl/raycasting.html
            // Get viewport coordinates
            int[] viewPort = new int[4];
            GL.GetInteger(GetPName.Viewport, viewPort);

            // Get normalized device coordinates
            Vector4 device = new Vector4();
            device.X = (window.X - viewPort[0]) / viewPort[2] * 2.0f - 1.0f;
            device.Y = 1 - (window.Y - viewPort[1]) / viewPort[3] * 2.0f;
            device.Z = window.Z * 2.0f - 1.0f;
            device.W = 1.0f;

            // Get homogeneous clip coordinates
            Vector4 clip = Vector4.Transform(device, 
                Matrix4.Invert(Camera.ViewMatrix * Camera.ProjectionMatrix));

            // Get eye coordinates
            Vector3 eye = new Vector3(clip.X, clip.Y, clip.Z);

            // Finally the world coordinates
            return eye / clip.W;
        }

        internal static Vector3 GetColorOfScreen(Vector3 window)
        {
            Color color = new Color();
            GL.ReadPixels((int)window.X, (int)(Screen.Height - window.Y), 1, 1, PixelFormat.Rgba, PixelType.UnsignedByte, ref color);
            return new Vector3(color.R/255f, color.G/255f, color.B/255f);
        }
    }
}

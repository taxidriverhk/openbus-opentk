using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenBusDrivingSimulator.Engine
{
    public static class Camera
    {
        // Projection related members
        private static float zNear;
        private static float zFar;
        private static float aspect;
        private static float fieldOfView;
        private static float planeWidth;
        private static float planeHeight;

        private static Vector3 eye;
        private static Vector3 target;
        private static Vector3 up;
        private static float zoomFactor;

        public static float PlaneWidth
        {
            get { return planeWidth; }
        }

        public static float PlaneHeight
        {
            get { return planeHeight; }
        }

        public static float PlaneZPosition
        {
            get { return zNear; }
        }

        public static Vector3 Eye
        {
            get { return eye; }
        }

        public static Vector3 Target
        {
            get { return target; }
        }

        public static void Initialize()
        {
            InitializeWithDefault();
            ModifyCamera();
        }

        public static void MoveTo(float x, float y, float z)
        {
            eye.X = x; eye.Y = y; eye.Z = z;
        }

        public static void MoveBy(float x, float y, float z)
        {
            eye.X += x; eye.Y += y; eye.Z += z;
        }

        public static void Zoom(float zoomMultiplier)
        {
            zoomFactor = zoomMultiplier;
            fieldOfView *= zoomFactor;
            // TODO: setup zoom limit (both min and max)
            ModifyCamera();
        }

        private static void InitializeWithDefault()
        {
            zNear = 0.025f;
            zFar = 20.0f;
            aspect = (float)Screen.Width / Screen.Height;
            zoomFactor = 1.0f;
            fieldOfView = zoomFactor * MathHelper.PiOver4;

            eye = Vector3.Zero;
            target = new Vector3(0, 0, zFar);
            up = Vector3.UnitY;
        }

        private static void ModifyCamera()
        {
            planeHeight = 2 * zNear * (float)System.Math.Tan(fieldOfView / 2);
            planeWidth = planeHeight * aspect;

            GL.MatrixMode(MatrixMode.Projection);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, zNear, zFar);
            GL.LoadMatrix(ref projection);

            GL.MatrixMode(MatrixMode.Modelview);
            Matrix4 lookAt = Matrix4.LookAt(Vector3.Zero, target, up);
            GL.LoadMatrix(ref lookAt);
        }
    }
}

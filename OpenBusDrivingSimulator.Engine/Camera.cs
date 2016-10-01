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
        private const float MAX_ROTATION_RADIANS = 2 * MathHelper.Pi;
        
        // Projection related members
        private static float zNear;
        private static float zFar;
        private static float aspect;
        private static float fieldOfView;

        private static Vector3 eye;
        private static Vector3 target;
        private static Vector3 up;
        private static Vector3 angles;
        private static float zoomFactor;

        public static Vector3 Eye
        {
            get { return eye; }
        }

        public static Vector3 Angles
        {
            get { return angles; }
        }

        public static void Initialize()
        {
            InitializeWithDefaults();
        }

        public static void MoveTo(float x, float y, float z)
        {
            eye.X = x; eye.Y = y; eye.Z = z;
        }

        public static void MoveBy(float x, float y, float z)
        {
            eye.X += x; eye.Y += y; eye.Z += z;
        }

        public static void RotateYTo(float degrees)
        {
            angles.Y = MathHelper.DegreesToRadians(degrees);
            if (angles.Y >= MAX_ROTATION_RADIANS)
                angles.Y = angles.Y % MAX_ROTATION_RADIANS;
        }

        public static void RotateYBy(float degrees)
        {
            angles.Y += MathHelper.DegreesToRadians(degrees);
            if (angles.Y >= MAX_ROTATION_RADIANS)
                angles.Y = angles.Y % MAX_ROTATION_RADIANS;
        }

        public static void UpdateCamera()
        {
            GL.PushMatrix();
            GL.Viewport(0, 0, Screen.Width, Screen.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, zNear, zFar);
            GL.LoadMatrix(ref projection);
            GL.Scale(-1.0f, 1.0f, 1.0f);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            
            float sinTheta = (float)Math.Sin(angles.Y),
                  cosTheta = (float)Math.Cos(angles.Y);
            target.X = zFar * sinTheta;
            target.Z = zFar * cosTheta;
            Matrix4 lookAt = Matrix4.LookAt(Vector3.Zero, target, up);
            GL.LoadMatrix(ref lookAt);
        }

        public static void Zoom(float zoomMultiplier)
        {
            zoomFactor = zoomMultiplier;
            if (zoomFactor >= 4.0f)
                zoomFactor = 3.99f;
            else if (zoomFactor <= 0.0f)
                zoomFactor = 0.01f;

            fieldOfView = (1 / zoomFactor) * MathHelper.PiOver4;
            UpdateCamera();
        }

        private static void InitializeWithDefaults()
        {
            zNear = 0.025f;
            zFar = 20.0f;
            aspect = (float)Screen.Width / Screen.Height;
            zoomFactor = 1.0f;
            fieldOfView = zoomFactor * MathHelper.PiOver4;

            eye = Vector3.Zero;
            target = new Vector3(0, 0, zFar);
            up = Vector3.UnitY;
            angles = Vector3.Zero;
        }
    }
}

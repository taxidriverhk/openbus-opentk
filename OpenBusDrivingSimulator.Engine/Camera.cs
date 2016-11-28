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

        private static Vector3 eye;
        private static Vector3 front;
        private static Vector3 right;
        private static Vector3 up;
        private static Vector3 angles;
        private static float zoomFactor;

        private static Matrix4 projectionMatrix;
        private static Matrix4 viewMatrix;

        internal static Matrix4 ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        internal static Matrix4 ViewMatrix
        {
            get { return viewMatrix; }
        }

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
            eye += right * x;
            eye += front * z;
            eye.Y += y;
        }

        public static void RotateYTo(float degrees)
        {
            angles.Y = MathHelper.DegreesToRadians(degrees);
            if (angles.Y >= MathHelper.TwoPi)
                angles.Y = angles.Y % MathHelper.TwoPi;
        }

        public static void RotateYBy(float degrees)
        {
            angles.Y += MathHelper.DegreesToRadians(degrees);
            if (angles.Y >= MathHelper.TwoPi)
                angles.Y = angles.Y % MathHelper.TwoPi;
        }

        public static void UpdateCamera()
        {
            // Projection
            GL.Viewport(0, 0, Screen.Width, Screen.Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, zNear, zFar);
            GL.LoadMatrix(ref projectionMatrix);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            // Apply yaw and pitch angles
            float cosYaw = (float)Math.Cos(angles.Y),
                  sinYaw = (float)Math.Sin(angles.Y);
            float cosPitch = (float)Math.Cos(angles.X),
                  sinPitch = (float)Math.Sin(angles.X);
            front.X = cosPitch * sinYaw;
            front.Y = sinPitch;
            front.Z = cosPitch * cosYaw;
            right.X = -cosYaw;
            right.Z = sinYaw;
            up = Vector3.Cross(right, front);

            viewMatrix = Matrix4.LookAt(eye, eye + front, up);
            GL.LoadMatrix(ref viewMatrix);
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
            zFar = 500.0f;
            aspect = (float)Screen.Width / Screen.Height;
            zoomFactor = 1.0f;
            fieldOfView = zoomFactor * MathHelper.PiOver4;

            eye = Vector3.Zero;
            front = new Vector3(0, 0, -1.0f);
            right = Vector3.Zero;
            up = Vector3.UnitY;
            angles = new Vector3(0, MathHelper.Pi, 0);

            projectionMatrix = Matrix4.Identity;
            viewMatrix = Matrix4.Identity;
        }
    }
}

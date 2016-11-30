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
        private static Vector3[] boundingBox;
        private static Vector4[] boundingPlanes;

        internal static Matrix4 ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        internal static Matrix4 ViewMatrix
        {
            get { return viewMatrix; }
        }

        internal static Vector3[] BoundingBox
        {
            get { return boundingBox; }
        }

        internal static Vector4[] BoundingPlanes
        {
            get { return boundingPlanes; }
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

            // Also update the bounding box as well
            CalculateBoundingBox();
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

        private static void CalculateBoundingBox()
        {
            // Get the dimensions of the far plane
            float farHeight = 2 * (float)Math.Tan(fieldOfView / 2) * zFar,
                  farWidth = farHeight * aspect;
            // Then get the four points of the far plane
            Vector3 nFront = Vector3.Normalize(front),
                    nUp = Vector3.Normalize(up),
                    nRight = Vector3.Normalize(right),
                    farCenter = eye + nFront * zFar,
                    nearCenter = eye + nFront * zNear;
            Vector3 tempX = nUp * farHeight / 2,
                    tempY = nRight * farWidth / 2;
            Vector3 farRightTop = farCenter + tempX + tempY,
                    farLeftTop = farCenter + tempX - tempY,
                    farRightBottom = farCenter - tempX + tempY,
                    farLeftBottom = farCenter - tempX - tempY;
            // Also the four points of the near plane
            Vector3 nearRightTop = nearCenter + tempX + tempY,
                    nearLeftTop = nearCenter + tempX - tempY,
                    nearRightBottom = nearCenter - tempX + tempY,
                    nearLeftBottom = nearCenter - tempX - tempY;
            boundingBox = new Vector3[8]
            {
                farLeftBottom, farRightBottom, farRightTop, farLeftTop,
                nearLeftBottom, nearRightBottom, nearRightTop, nearLeftTop
            };
            // Finally, compute the six planes
            boundingPlanes = new Vector4[6]
            {
                // The points given should in the direction so the normal of 
                // the plane points into the box (instead of outside of the box)
                CalculatePlaneEquation(nearRightTop, nearLeftTop, farLeftTop),
                CalculatePlaneEquation(nearLeftBottom, nearRightBottom, farRightBottom),
                CalculatePlaneEquation(nearLeftTop, nearLeftBottom, farLeftBottom),
                CalculatePlaneEquation(nearRightBottom, nearRightTop, farRightBottom),
                CalculatePlaneEquation(nearLeftTop, nearRightTop, nearRightBottom),
                CalculatePlaneEquation(farRightTop, farLeftTop, farLeftBottom)
            };
        }

        /// <summary>
        /// Calculates the plane equation given the three points
        /// </summary>
        /// <param name="p1">The first point.</param>
        /// <param name="p2">The second point.</param>
        /// <param name="p3">The third point.</param>
        /// <returns>
        /// The plane equation in form ax + by + cz = d
        /// where they are stored into the 4d vector in form (a, b, c, d).
        /// </returns>
        private static Vector4 CalculatePlaneEquation(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 p2p1 = p2 - p1,
                    p3p1 = p3 - p1;
            Vector3 pNormal = Vector3.Cross(p2p1, p3p1);
            float d = pNormal.X * p1.X + pNormal.Y * p1.Y + pNormal.Z * p1.Z;
            return new Vector4(pNormal, d);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;
using OpenBus.Engine;

namespace OpenBus.Game.Controls
{
    /// <summary>
    /// The type of the view
    /// </summary>
    public enum ViewType
    {
        /// <summary>
        /// Free moving mode, the camera moves along all of the yaw, pitch and roll angles
        /// </summary>
        FREE = 0,
        /// <summary>
        /// Rotates about the object (ex. the bus). The camera moves along the direction of the object,
        /// with pitch angle and the rotation about the object applied to the view.
        /// </summary>
        ROTATE_ABOUT_OBJECT = 1
    }

    public class View
    {
        private ViewType viewType;
        private Vector3f position;
        private Vector3f positionOffsets;
        private Vector3f angles;
        private Vector3f angleOffsets;
        private Vector3f frontDirection;
        private Vector3f rightDirection;
        private float zoom;

        public Vector3f Position
        {
            get { return position; }
        }

        public View(ViewType type)
        {
            viewType = type;
            position = Vector3f.Zero;
            positionOffsets = Vector3f.Zero;
            angles = Vector3f.Zero;
            angleOffsets = Vector3f.Zero;
            frontDirection = Vector3f.UnitZ;
            rightDirection = Vector3f.UnitX;
            zoom = 1.0f;
        }

        public void ChangeYawAngleBy(float degrees)
        {
            angleOffsets.Y = Trigonometry.DegreesToRadians(degrees);
            angles.Y += angleOffsets.Y;
            if (angles.Y >= Trigonometry.TWO_PI)
                angles.Y = angles.Y % Trigonometry.TWO_PI;

            Vector3f newFront = Vector3f.Zero,
                     newRight = Vector3f.Zero;
            UpdateDirections(ref newFront, ref newRight);
            frontDirection = newFront;
            rightDirection = newRight;
        }

        public void MoveBy(float x, float y, float z)
        {
            // Calculate yaw and pitch angles
            Vector3f newPosition = position;
            newPosition += rightDirection * x;
            newPosition += frontDirection * z;
            newPosition.Y += y;
            // If the move will cause it get out of the map, then don't move
            if (viewType == ViewType.FREE && !Game.World.IsInMap(newPosition))
                positionOffsets = Vector3f.Zero;
            else
            {
                positionOffsets = new Vector3f(x, y, z);
                position = newPosition;
            }
        }

        public void UpdateCamera()
        {
            if (positionOffsets == Vector3f.Zero &&
                angleOffsets == Vector3f.Zero)
                return;

            Camera.SetCamera(position, frontDirection, rightDirection);
            Camera.UpdateCamera();
            positionOffsets = Vector3f.Zero;
            angleOffsets = Vector3f.Zero;
        }

        public void ZoomBy(float factor)
        {
            zoom += factor;
            if (zoom >= 4.0f)
                zoom = 3.99f;
            else if (zoom <= 0.5f)
                zoom = 0.51f;
            Camera.Zoom(zoom);
        }

        private void UpdateDirections(ref Vector3f newFront, ref Vector3f newRight)
        {
            float cosYaw = (float)Math.Cos(-angles.Y),
                  sinYaw = (float)Math.Sin(-angles.Y);
            float cosPitch = (float)Math.Cos(-angles.X),
                  sinPitch = (float)Math.Sin(-angles.X);
            newFront = frontDirection;
            newFront.X = cosPitch * sinYaw;
            newFront.Y = sinPitch;
            newFront.Z = cosPitch * cosYaw;
            newRight = rightDirection;
            newRight.X = cosYaw;
            newRight.Z = -sinYaw;
        }
    }
}

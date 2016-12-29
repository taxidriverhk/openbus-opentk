using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;

namespace OpenBus.Game.Objects
{
    /// <summary>
    /// Defines a curve that could be used to construct roads or tracks on a map.
    /// </summary>
    public struct Spline
    {
        private const float DEGREES_PER_SEGMENT = 0.5f;

        /// <summary>
        /// Starting position of the curve.
        /// </summary>
        public Vector3f StartPosition;
        /// <summary>
        /// Y-Direction of the curve, in radians.
        /// </summary>
        public float Direction;
        /// <summary>
        /// Radius of the curve, in meters.
        /// </summary>
        public float CurveRadius;
        /// <summary>
        /// Length of the curve, in meters;
        /// </summary>
        public float Length;
        // TODO: Add vertices and texture coordinates fields to the spline
        // Width is used for now
        #region Temporary Code
        public float Width;
        #endregion

        private int sign;
        private float centralAngle;
        private float chord;
        private int segments;

        /// <summary>
        /// Whether the curve goes to the left, or to the right.
        /// </summary>
        public int Sign
        {
            get { return sign; }
        }

        /// <summary>
        /// Central angle of the arc (or the curve).
        /// </summary>
        public float CentralAngle
        {
            get { return centralAngle; }
        }

        /// <summary>
        /// Chord of the curve.
        /// </summary>
        public float Chord
        {
            get { return chord; }
        }

        /// <summary>
        /// Number of straight line segments used to construct the curve.
        /// </summary>
        public int Segments
        {
            get { return segments; }
        }

        /// <summary>
        /// Initializes a Spline object that uses the specified starting position, direction (in radians), curve radius,
        /// length and width (all in meters). If a straight is to be constructed, then specify the radius with zero.
        /// </summary>
        /// <param name="startPosition">Starting world position.</param>
        /// <param name="rotation">Rotation applied, in radians.</param>
        /// <param name="radius">Curve radius, in meters. A negative radius means the spline goes to the left.</param>
        /// <param name="length">Length of the spline, in meters.</param>
        /// <param name="width">Width of the spline, in meters.</param>
        public Spline(Vector3f startPosition, float rotation, float radius, float length, float width)
        {
            this.StartPosition = startPosition;
            this.Direction = rotation;
            this.CurveRadius = radius;
            this.Length = length;
            this.Width = width;

            // Calculate other needed info
            this.sign = Math.Sign(radius);
            if (radius == 0.0)
            {
                this.centralAngle = float.PositiveInfinity;
                this.chord = length;
                this.segments = 1;
            }
            else
            {
                this.centralAngle = length / radius;
                this.chord = 2 * radius * (float)Math.Sin(centralAngle / 2);
                this.segments = (int)Math.Ceiling(
                    MathHelper.RadiansToDegrees(centralAngle) / DEGREES_PER_SEGMENT);
            }
            
        }
    }
}

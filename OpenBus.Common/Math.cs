using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenBus.Common
{
    public static class Trigonometry
    {
        public const float PI = (float)Math.PI;
        public const float TWO_PI = 2 * (float)Math.PI;
        public const int HALF_CIRCLE_DEGREES = 180;
        public const int CIRCLE_DEGREES = 360;

        public static float DegreesToRadians(float degrees)
        {
            return degrees * PI / HALF_CIRCLE_DEGREES;
        }
    }

    /// <summary>
    /// Represents a three-dimensional vector with single precision.
    /// This should not be used on the engine module, which depends on OpenTK. 
    /// The vector structures included in the OpenTK framework should be used instead.
    /// </summary>
    public struct Vector3f
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
        [XmlAttribute("z")]
        public float Z;

        public Vector3f(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3f(Vector3f other)
        {
            this.X = other.X;
            this.Y = other.Y;
            this.Z = other.Z;
        }

        public static Vector3f Zero
        {
            get { return new Vector3f(0.0f, 0.0f, 0.0f); }
        }

        public static Vector3f UnitX
        {
            get { return new Vector3f(1.0f, 0.0f, 0.0f); }
        }

        public static Vector3f UnitY
        {
            get { return new Vector3f(0.0f, 1.0f, 0.0f); }
        }

        public static Vector3f UnitZ
        {
            get { return new Vector3f(0.0f, 0.0f, 1.0f); }
        }

        public static Vector3f Cross(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        public static Vector3f operator *(Vector3f left, float right)
        {
            return new Vector3f(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vector3f operator *(float left, Vector3f right)
        {
            return new Vector3f(left * right.X, left * right.Y, left * right.Z);
        }

        public static Vector3f operator +(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static bool operator ==(Vector3f left, Vector3f right)
        {
            return left.X == right.X && left.Y == right.Y
                && left.Z == right.Z;
        }

        public static bool operator !=(Vector3f left, Vector3f right)
        {
            return left.X != right.X || left.Y != right.Y
                || left.Z != right.Z;
        }
    }

    public struct Vector2f
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;

        public Vector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2f(Vector2f other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        public static Vector2f Zero
        {
            get { return new Vector2f(0.0f, 0.0f); }
        }

        public static Vector2f UnitX
        {
            get { return new Vector2f(1.0f, 0.0f); }
        }

        public static Vector2f UnitY
        {
            get { return new Vector2f(0.0f, 1.0f); }
        }
    }
}

using System;
using System.Xml.Serialization;

namespace OpenBus.Common
{
    public static class MathHelper
    {
        public const float PI_OVER_4 = PI / 4;
        public const float PI_OVER_2 = PI / 2;
        public const float PI = 3.1415926535897931f;
        public const float TWO_PI = 2 * PI;
        public const int HALF_CIRCLE_DEGREES = 180;
        public const int CIRCLE_DEGREES = 360;

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
        public static Vector4f CalculatePlaneEquation(Vector3f p1, Vector3f p2, Vector3f p3)
        {
            Vector3f p2p1 = p2 - p1,
                     p3p1 = p3 - p1;
            Vector3f pNormal = Vector3f.Cross(p2p1, p3p1);
            float d = pNormal.X * p1.X + pNormal.Y * p1.Y + pNormal.Z * p1.Z;
            return new Vector4f(pNormal, d);
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * PI / HALF_CIRCLE_DEGREES;
        }
    }

    public struct Matrix3f
    {
        public Vector3f Row1;
        public Vector3f Row2;
        public Vector3f Row3;

        public Matrix3f(Vector3f row1, Vector3f row2, Vector3f row3)
        {
            this.Row1 = row1;
            this.Row2 = row2;
            this.Row3 = row3;
        }

        public Matrix3f(float a11, float a12, float a13,
            float a21, float a22, float a23,
            float a31, float a32, float a33)
        {
            this.Row1 = new Vector3f(a11, a12, a13);
            this.Row2 = new Vector3f(a21, a22, a23);
            this.Row3 = new Vector3f(a31, a32, a33);
        }

        public float A11
        {
            get { return Row1.X; }
            set { Row1.X = value; }
        }

        public float A12
        {
            get { return Row1.Y; }
            set { Row1.Y = value; }
        }

        public float A13
        {
            get { return Row1.Z; }
            set { Row1.Z = value; }
        }

        public float A21
        {
            get { return Row2.X; }
            set { Row2.X = value; }
        }

        public float A22
        {
            get { return Row2.Y; }
            set { Row2.Y = value; }
        }

        public float A23
        {
            get { return Row2.Z; }
            set { Row2.Z = value; }
        }

        public float A31
        {
            get { return Row3.X; }
            set { Row3.X = value; }
        }

        public float A32
        {
            get { return Row3.Y; }
            set { Row3.Y = value; }
        }

        public float A33
        {
            get { return Row3.Z; }
            set { Row3.Z = value; }
        }

        public float[] Array
        {
            get
            {
                return new float[9]
                {
                    A11, A12, A13,
                    A21, A22, A23,
                    A31, A32, A33
                };
            }
        }
    }

    public struct Matrix4f
    {
        public Vector4f Row1;
        public Vector4f Row2;
        public Vector4f Row3;
        public Vector4f Row4;

        public Matrix4f(Vector4f row1, Vector4f row2, Vector4f row3, Vector4f row4)
        {
            this.Row1 = row1;
            this.Row2 = row2;
            this.Row3 = row3;
            this.Row4 = row4;
        }

        public Matrix4f(float a11, float a12, float a13, float a14,
            float a21, float a22, float a23, float a24,
            float a31, float a32, float a33, float a34,
            float a41, float a42, float a43, float a44)
        {
            this.Row1 = new Vector4f(a11, a12, a13, a14);
            this.Row2 = new Vector4f(a21, a22, a23, a24);
            this.Row3 = new Vector4f(a31, a32, a33, a34);
            this.Row4 = new Vector4f(a41, a42, a43, a44);
        }

        public float A11
        {
            get { return Row1.X; }
            set { Row1.X = value; }
        }

        public float A12
        {
            get { return Row1.Y; }
            set { Row1.Y = value; }
        }

        public float A13
        {
            get { return Row1.Z; }
            set { Row1.Z = value; }
        }

        public float A14
        {
            get { return Row1.W; }
            set { Row1.W = value; }
        }

        public float A21
        {
            get { return Row2.X; }
            set { Row2.X = value; }
        }

        public float A22
        {
            get { return Row2.Y; }
            set { Row2.Y = value; }
        }

        public float A23
        {
            get { return Row2.Z; }
            set { Row2.Z = value; }
        }

        public float A24
        {
            get { return Row2.W; }
            set { Row2.W = value; }
        }

        public float A31
        {
            get { return Row3.X; }
            set { Row3.X = value; }
        }

        public float A32
        {
            get { return Row3.Y; }
            set { Row3.Y = value; }
        }

        public float A33
        {
            get { return Row3.Z; }
            set { Row3.Z = value; }
        }

        public float A34
        {
            get { return Row3.W; }
            set { Row3.W = value; }
        }

        public float A41
        {
            get { return Row4.X; }
            set { Row4.X = value; }
        }

        public float A42
        {
            get { return Row4.Y; }
            set { Row4.Y = value; }
        }

        public float A43
        {
            get { return Row4.Z; }
            set { Row4.Z = value; }
        }

        public float A44
        {
            get { return Row4.W; }
            set { Row4.W = value; }
        }

        public float[] Array
        {
            get
            {
                return new float[16]
                {
                    A11, A12, A13, A14,
                    A21, A22, A23, A24,
                    A31, A32, A33, A34,
                    A41, A42, A43, A44
                };
            }
        }

        public float Determinant
        {
            get
            {
                return A11 * (A22 * A33 * A44 + A23 * A34 * A42 + A24 * A32 * A43
                    - A42 * A33 * A24 - A43 * A34 * A22 - A44 * A32 * A23)
                    - A12 * (A21 * A33 * A44 + A23 * A34 * A41 + A24 * A31 * A43
                    - A41 * A23 * A24 - A43 * A34 * A21 - A44 * A31 * A23)
                    + A13 * (A21 * A32 * A44 + A22 * A34 * A41 + A24 * A31 * A42
                    - A41 * A32 * A24 - A42 * A34 * A21 - A44 * A31 * A22)
                    - A14 * (A21 * A32 * A43 + A22 * A33 * A41 + A23 * A31 * A41
                    - A41 * A32 * A23 - A42 * A33 * A21 - A43 * A31 * A22);
            }
        }

        public static Matrix4f Identity
        {
            get
            {
                return new Matrix4f(Vector4f.UnitX, Vector4f.UnitY,
                    Vector4f.UnitZ, Vector4f.UnitW);
            }
        }

        public static Matrix4f Inverse(Matrix4f matrix)
        {
            // TODO: implement me
            return Identity;
        }

        public static Matrix4f operator *(Matrix4f left, Matrix4f right)
        {
            float a11 = left.A11, a12 = left.A12, a13 = left.A13, a14 = left.A14,
                  a21 = left.A21, a22 = left.A22, a23 = left.A23, a24 = left.A24,
                  a31 = left.A31, a32 = left.A32, a33 = left.A33, a34 = left.A34,
                  a41 = left.A41, a42 = left.A42, a43 = left.A43, a44 = left.A44;
            float b11 = right.A11, b12 = right.A12, b13 = right.A13, b14 = right.A14,
                  b21 = right.A21, b22 = right.A22, b23 = right.A23, b24 = right.A24,
                  b31 = right.A31, b32 = right.A32, b33 = right.A33, b34 = right.A34,
                  b41 = right.A41, b42 = right.A42, b43 = right.A43, b44 = right.A44;
            return new Matrix4f(
                a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41,
                a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42,
                a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43,
                a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44,
                a21 * b11 + a22 * b22 + a23 * b31 + a24 * b41,
                a21 * b11 + a22 * b23 + a23 * b32 + a24 * b42,
                a21 * b11 + a22 * b24 + a23 * b33 + a24 * b43,
                a21 * b11 + a22 * b21 + a23 * b34 + a24 * b44,
                a31 * b11 + a32 * b22 + a33 * b31 + a34 * b41,
                a31 * b11 + a32 * b23 + a33 * b32 + a34 * b42,
                a31 * b11 + a32 * b24 + a33 * b33 + a34 * b43,
                a31 * b11 + a32 * b21 + a33 * b34 + a34 * b44,
                a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41,
                a41 * b11 + a42 * b22 + a43 * b32 + a44 * b42,
                a41 * b11 + a42 * b23 + a43 * b33 + a44 * b43,
                a41 * b11 + a42 * b24 + a43 * b34 + a44 * b44);
        }

        public static Matrix4f operator *(float left, Matrix4f right)
        {
            return new Matrix4f(left * right.Row1,
                left * right.Row2,
                left * right.Row3,
                left * right.Row4);
        }
    }

    /// <summary>
    /// 
    /// </summary>
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

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y); }
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

        public static Vector2f One
        {
            get { return new Vector2f(1.0f, 1.0f); }
        }
    }

    /// <summary>
    /// Represents a three-dimensional vector with single precision.
    /// </summary>
    public struct Vector3f
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
        [XmlAttribute("z")]
        public float Z;

        public static readonly int Size = 12;

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

        public Vector3f(float s)
        {
            this.X = s;
            this.Y = s;
            this.Z = s;
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z); }
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

        public static Vector3f One
        {
            get { return new Vector3f(1.0f, 1.0f, 1.0f); }
        }

        public static Vector3f Cross(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.Y * right.Z - left.Z * right.Y,
                left.Z * right.X - left.X * right.Z,
                left.X * right.Y - left.Y * right.X);
        }

        public static Vector3f Normalize(Vector3f vector)
        {
            return (1 / vector.Magnitude) * vector;
        }

        public static Vector3f operator +(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3f operator -(Vector3f left, Vector3f right)
        {
            return new Vector3f(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3f operator *(Vector3f left, float right)
        {
            return new Vector3f(left.X * right, left.Y * right, left.Z * right);
        }

        public static Vector3f operator *(float left, Vector3f right)
        {
            return new Vector3f(left * right.X, left * right.Y, left * right.Z);
        }

        public static Vector3f operator /(Vector3f left, float right)
        {
            return new Vector3f(left.X / right, left.Y / right, left.Z / right);
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

    /// <summary>
    /// 
    /// </summary>
    public struct Vector4f
    {
        [XmlAttribute("x")]
        public float X;
        [XmlAttribute("y")]
        public float Y;
        [XmlAttribute("z")]
        public float Z;
        [XmlAttribute("w")]
        public float W;

        public Vector4f(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public Vector4f(Vector3f xyz, float w)
        {
            this.X = xyz.X;
            this.Y = xyz.Y;
            this.Z = xyz.Z;
            this.W = w;
        }

        public float Magnitude
        {
            get { return (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
        }

        public static Vector4f Zero
        {
            get { return new Vector4f(0.0f, 0.0f, 0.0f, 0.0f); }
        }

        public static Vector4f UnitX
        {
            get { return new Vector4f(1.0f, 0.0f, 0.0f, 0.0f); }
        }

        public static Vector4f UnitY
        {
            get { return new Vector4f(0.0f, 1.0f, 0.0f, 0.0f); }
        }

        public static Vector4f UnitZ
        {
            get { return new Vector4f(0.0f, 0.0f, 1.0f, 0.0f); }
        }

        public static Vector4f UnitW
        {
            get { return new Vector4f(0.0f, 0.0f, 0.0f, 1.0f); }
        }

        public static Vector4f One
        {
            get { return new Vector4f(1.0f, 1.0f, 1.0f, 1.0f); }
        }

        public static Vector4f Transform(Vector4f vector, Matrix4f matrix)
        {
            return new Vector4f(
                vector.X * matrix.Row1.X + vector.Y * matrix.Row2.X + vector.Z * matrix.Row3.X + vector.W * matrix.Row4.X,
                vector.X * matrix.Row1.Y + vector.Y * matrix.Row2.Y + vector.Z * matrix.Row3.Y + vector.W * matrix.Row4.Y,
                vector.X * matrix.Row1.Z + vector.Y * matrix.Row2.Z + vector.Z * matrix.Row3.Z + vector.W * matrix.Row4.Z,
                vector.X * matrix.Row1.W + vector.Y * matrix.Row2.W + vector.Z * matrix.Row3.W + vector.W * matrix.Row4.W);
        }

        public static Vector4f operator *(float left, Vector4f right)
        {
            return new Vector4f(left * right.X, left * right.Y, left * right.Z, left * right.W);
        }
    }
}

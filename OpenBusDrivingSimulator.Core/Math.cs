using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenBusDrivingSimulator.Core
{
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
            get { return new Vector3f(0.0f, 1.0f, 0.0f); }
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

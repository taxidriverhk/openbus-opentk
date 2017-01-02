using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenBus.Common;

namespace OpenBus.Game.Objects
{
    /// <summary>
    /// Defines the cross section of a spline, which contains the vertices of the cross-sectional shape, and
    /// also their texture coordinates to use.
    /// </summary>
    public struct SplineCrossSection
    {
        /// <summary>
        /// The texture paths used, relative to the game's root path.
        /// </summary>
        public string[] Textures;
        /// <summary>
        /// Vertices of the cross-sectional shape.
        /// </summary>
        public Vector3f[] Vertices;
        /// <summary>
        /// The corresponding array of texture u-coordinate (along the x-axis) used for each vertex,
        /// the v-coordinate (along the z-axis) is a constant defined in TEXTURE_V_COORDINATE_PER_METER.
        /// </summary>
        public Vector2f[] TextureUCoords;
        /// <summary>
        /// The corresponding index to Textures array, that indicates which texture to use for the vertex.
        /// </summary>
        public int[] TextureIndices;

        /// <summary>
        /// Creates a SplineCrossSection object that uses the specified texture paths, vertices, texture u-coorindates,
        /// and the index mapping.
        /// </summary>
        /// <param name="textures">The texture paths used.</param>
        /// <param name="vertices">Vertices of the cross-sectional shape.</param>
        /// <param name="textureUCoords">Array of U-Coordinates of the corresponding vertices.</param>
        /// <param name="textureIndices">Index to the texture array, of each vertex.</param>
        public SplineCrossSection(string[] textures, Vector3f[] vertices, Vector2f[] textureUCoords, int[] textureIndices)
        {
            this.Textures = textures;
            this.Vertices = vertices;
            this.TextureUCoords = textureUCoords;
            this.TextureIndices = textureIndices;
        }
    }

    /// <summary>
    /// Defines a curve that could be used to construct roads or tracks on a map.
    /// </summary>
    public struct Spline
    {
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
        /// <summary>
        /// Cross section definition of the curve.
        /// </summary>
        public SplineCrossSection CrossSection;

        /// <summary>
        /// Initializes a Spline object that uses the specified starting position, direction (in radians), curve radius,
        /// length and width (all in meters). If a straight is to be constructed, then specify the radius with zero.
        /// </summary>
        /// <param name="crossSection">Cross section definition used for this spline.</param>
        /// <param name="startPosition">Starting world position.</param>
        /// <param name="rotation">Rotation applied, in radians.</param>
        /// <param name="radius">Curve radius, in meters. A negative radius means the spline goes to the left.</param>
        /// <param name="length">Length of the spline, in meters.</param>
        public Spline(SplineCrossSection crossSection, Vector3f startPosition, float rotation, float radius, float length)
        {
            this.StartPosition = startPosition;
            this.Direction = rotation;
            this.CurveRadius = radius;
            this.Length = length;
            this.CrossSection = crossSection;
        }
    }
}

// AForge Math Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge.Math.Geometry
{
    using System;

    /// <summary>
    /// Enumeration of some basic shape types.
    /// </summary>
    public enum ShapeType
    {
        /// <summary>
        /// Unknown shape type.
        /// </summary>
        Unknown,

        /// <summary>
        /// Circle shape.
        /// </summary>
        Circle,

        /// <summary>
        /// Triangle shape.
        /// </summary>
        Triangle,

        /// <summary>
        /// Quadrilateral shape.
        /// </summary>
        Quadrilateral,
    }

    /// <summary>
    /// Some common sub types of some basic shapes.
    /// </summary>
    public enum PolygonSubType
    {
        /// <summary>
        /// Unrecognized sub type of a shape (generic shape which does not have
        /// any specific sub type).
        /// </summary>
        Unknown,

        /// <summary>
        /// Quadrilateral with one pair of parallel sides.
        /// </summary>
        Trapezoid,

        /// <summary>
        /// Quadrilateral with two pairs of parallel sides.
        /// </summary>
        Parallelogram,

        /// <summary>
        /// Parallelogram with perpendicular adjacent sides.
        /// </summary>
        Rectangle,

        /// <summary>
        /// Parallelogram with all sides equal.
        /// </summary>
        Rhombus,

        /// <summary>
        /// Rectangle with all sides equal.
        /// </summary>
        Square,

        /// <summary>
        /// Triangle with all sides/angles equal.
        /// </summary>
        EquilateralTriangle,

        /// <summary>
        /// Triangle with two sides/angles equal.
        /// </summary>
        IsoscelesTriangle,

        /// <summary>
        /// Triangle with a 90 degrees angle.
        /// </summary>
        RectangledTriangle,

        /// <summary>
        /// Triangle with a 90 degrees angle and other two angles are equal.
        /// </summary>
        RectangledIsoscelesTriangle
    }
}

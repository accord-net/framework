// Accord Vision Library
// The Accord.NET Framework (LGPL)
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Vision.Detection
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    ///   Haar Cascade Feature Tree Node.
    /// </summary>
    /// 
    /// <remarks>
    ///   The Feature Node is a node belonging to a feature tree,
    ///   containing information about child nodes and an associated 
    ///   <see cref="HaarFeature"/>.
    /// </remarks>
    /// 
    [Serializable]
    public class HaarFeatureNode : ICloneable
    {
        private int rightNodeIndex = -1;
        private int leftNodeIndex = -1;

        /// <summary>
        ///   Gets the threshold for this feature.
        /// </summary>
        /// 
        [XmlElement("threshold")]
        public double Threshold { get; set; }

        /// <summary>
        ///   Gets the left value for this feature.
        /// </summary>
        /// 
        [XmlElement("left_val")]
        public double LeftValue { get; set; }

        /// <summary>
        ///   Gets the right value for this feature.
        /// </summary>
        /// 
        [XmlElement("right_val")]
        public double RightValue { get; set; }

        /// <summary>
        ///   Gets the left node index for this feature.
        /// </summary>
        /// 
        [XmlElement("left_node")]
        public int LeftNodeIndex
        {
            get { return leftNodeIndex; }
            set { leftNodeIndex = value; }
        }

        /// <summary>
        ///   Gets the right node index for this feature.
        /// </summary>
        /// 
        [XmlElement("right_node")]
        public int RightNodeIndex
        {
            get { return rightNodeIndex; }
            set { rightNodeIndex = value; }
        }

        /// <summary>
        ///   Gets the feature associated with this node.
        /// </summary>
        /// 
        [XmlElement("feature", IsNullable = false)]
        public HaarFeature Feature { get; set; }

        /// <summary>
        ///   Constructs a new feature tree node.
        /// </summary>
        public HaarFeatureNode()
        {
        }

        /// <summary>
        ///   Constructs a new feature tree node.
        /// </summary>
        /// 
        public HaarFeatureNode(double threshold, double leftValue, double rightValue, params int[][] rectangles)
            : this(threshold, leftValue, rightValue, false, rectangles) { }

        /// <summary>
        ///   Constructs a new feature tree node.
        /// </summary>
        /// 
        public HaarFeatureNode(double threshold, double leftValue, double rightValue, bool tilted, params int[][] rectangles)
        {
            this.Feature = new HaarFeature(tilted, rectangles);
            this.Threshold = threshold;
            this.LeftValue = leftValue;
            this.RightValue = rightValue;
        }


        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            HaarFeatureNode r = new HaarFeatureNode();

            r.Feature = (HaarFeature)Feature.Clone();
            r.Threshold = Threshold;

            r.RightValue = RightValue;
            r.LeftValue = LeftValue;

            r.LeftNodeIndex = leftNodeIndex;
            r.RightNodeIndex = rightNodeIndex;

            return r;
        }

    }
}

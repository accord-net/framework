// Accord Statistics Controls Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

namespace Accord.Controls
{
    using System.Windows.Forms;
    using Accord.MachineLearning.DecisionTrees;
    using Accord.Statistics.Filters;

    /// <summary>
    ///   Decision Tree (DT) Viewer.
    /// </summary>
    /// 
    public partial class DecisionTreeView : UserControl
    {
        private DecisionTree treeSource;
        private Codification codebook;

        /// <summary>
        ///   Initializes a new instance of the <see cref="DecisionTreeView"/> class.
        /// </summary>
        /// 
        public DecisionTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Gets or sets the currently displayed
        ///   <see cref="DecisionTree">Decision Tree</see>.
        /// </summary>
        /// 
        /// <value>The decision tree being displayed.</value>
        /// 
        public DecisionTree TreeSource
        {
            get { return treeSource; }
            set
            {
                if (treeSource != value)
                {
                    treeSource = value;
                    update();
                }
            }
        }

        /// <summary>
        ///   Gets or sets the codebook to be used when
        ///   displaying the tree. Using a codebook avoids
        ///   showing integer labels which may be difficult
        ///   to interpret.
        /// </summary>
        /// 
        public Codification Codebook
        {
            get { return codebook; }
            set
            {
                if (codebook != value)
                {
                    codebook = value;
                    update();
                }
            }
        }

        private void update()
        {
            treeView1.Nodes.Clear();

            if (treeSource != null && treeSource.Root != null)
                treeView1.Nodes.Add(convert(TreeSource.Root));
        }

        private TreeNode convert(DecisionNode node)
        {
            TreeNode treeNode = (codebook == null) ?
                new TreeNode(node.ToString()) :
                new TreeNode(node.ToString(codebook));


            if (!node.IsLeaf)
            {
                foreach (var child in node.Branches)
                    treeNode.Nodes.Add(convert(child));

                return treeNode;
            }


            if (codebook == null || !node.Output.HasValue)
            {
                treeNode.Nodes.Add(new TreeNode(node.Output.ToString()));
                return treeNode;
            }

            int index = node.Parent.Branches.AttributeIndex;
            var attrib = treeSource.Attributes[index];

            if (attrib.Nature != DecisionVariableKind.Discrete)
            {
                treeNode.Nodes.Add(new TreeNode(node.Output.ToString()));
                return treeNode;
            }

            string value = codebook.Revert(attrib.Name, node.Output.Value);
            treeNode.Nodes.Add(new TreeNode(value));
            return treeNode;
        }
    }
}

// Accord Math Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © Rémy Dispagne, 2013
// cramer at libertysurf.fr
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

namespace Accord.Math.Kinematics
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    ///   Denavit Hartenberg Model Combinator class to make combination
    ///   of models to create a complex model composed of multiple chains.
    /// </summary>
    ///
    /// <example>
    /// <para>
    ///   The following example shows the creation and animation of a
    ///   2-link planar manipulator with a dual 2-link planar gripper.
    ///   </para>
    /// 
    /// <code>
    ///   // Create the DH-model at (0, 0, 0) location
    ///	  DenavitHartenbergModel model = new DenavitHartenbergModel();
    ///	  
    ///	  // Add the first joint
    ///	  model.Joints.Add(alpha: 0, theta: Math.PI / 4, radius: 35, offset: 0);
    ///	  
    ///	  // Add the second joint
    ///	  model.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 35, offset: 0);
    ///	
    ///	  // Create the top finger
    ///	  DenavitHartenbergModel model_tgripper = new DenavitHartenbergModel();
    ///	  model_tgripper.Joints.Add(alpha: 0, theta:  Math.PI / 4, radius: 20, offset: 0);
    ///	  model_tgripper.Joints.Add(alpha: 0, theta: -Math.PI / 3, radius: 20, offset: 0);
    ///	  
    ///	  // Create the bottom finger
    ///	  DenavitHartenbergModel model_bgripper = new DenavitHartenbergModel();
    ///	  model_bgripper.Joints.Add(0, -Math.PI / 4, 20, 0);
    ///	  model_bgripper.Joints.Add(0,  Math.PI / 3, 20, 0);
    ///	  
    ///	  // Create the model combinator from the parent model
    ///	  DenavitHartenbergModelCombinator arm = new DenavitHartenbergModelCombinator(model);
    ///	  
    ///	  // Add the top finger
    ///	  arm.Children.Add(model_tgripper);
    ///	  
    ///	  // Add the bottom finger
    ///	  arm.Children.Add(model_bgripper);
    ///	  
    ///	  // Calculate the whole model (parent model + children models)
    ///	  arm.Compute();
    ///	</code>
    /// </example>
    /// 
    [Serializable]
    public class DenavitHartenbergNode
    {

        /// <summary>
        ///   Gets the parent of this node.
        /// </summary>
        /// 
        public DenavitHartenbergNode Parent { get; internal set; }

        /// <summary>
        ///   Gets the model contained at this node.
        /// </summary>
        /// 
        public DenavitHartenbergModel Model { get; set; }


        /// <summary>
        ///   Gets the collection of models attached to this node.
        /// </summary>
        /// 
        public DenavitHartenbergNodeCollection Children { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="DenavitHartenbergNode"/> class.
        /// </summary>
        /// 
        /// <param name="model">The inner model contained at this node.</param>
        /// 
        public DenavitHartenbergNode(DenavitHartenbergModel model)
        {
            // Initialize the child list
            Children = new DenavitHartenbergNodeCollection(this);

            // No parent model at first
            this.Parent = null;

            // Set the reference of the model given as argument
            this.Model = model;
        }


        /// <summary>
        ///   Calculates the whole combined model (this model plus all its 
        ///   children plus all the children of the children and so on)
        /// </summary>
        /// 
        public void Compute()
        {
            if (Parent == null)
            {
                Model.Compute();
            }

            else
            {
                Model.Compute(Parent.Model);
            }


            foreach (DenavitHartenbergNode child in Children)
            {
                child.Compute();
            }
        }
    }

    /// <summary>
    ///   Collection of Denavit-Hartenberg model nodes.
    /// </summary>
    /// 
    public class DenavitHartenbergNodeCollection : Collection<DenavitHartenbergNode>
    {
        /// <summary>
        ///   Gets the owner of this collection (i.e. the parent
        ///   <see cref="DenavitHartenbergNode"/> which owns the
        ///   children contained at this collection.
        /// </summary>
        /// 
        public DenavitHartenbergNode Owner { get; private set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DenavitHartenbergNodeCollection"/> class.
        /// </summary>
        /// 
        /// <param name="owner">The <see cref="DenavitHartenbergNode"/> owner.</param>
        /// 
        public DenavitHartenbergNodeCollection(DenavitHartenbergNode owner)
        {
            Owner = owner;
        }

        /// <summary>
        ///   Adds a children model to the end of this <see cref="DenavitHartenbergNodeCollection"/>.
        /// </summary>
        /// 
        public void Add(DenavitHartenbergModel child)
        {
            Add(new DenavitHartenbergNode(child));
        }

        /// <summary>
        ///    Inserts an element into the Collection&lt;T> at the specified index.
        /// </summary>
        /// 
        protected override void InsertItem(int index, DenavitHartenbergNode item)
        {
            if (item.Parent != null)
                throw new ArgumentException("The node already belongs to another model.", "item");

            item.Parent = Owner;
            base.InsertItem(index, item);
        }
    }

}

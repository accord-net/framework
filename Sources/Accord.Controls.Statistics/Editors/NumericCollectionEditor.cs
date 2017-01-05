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
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Reflection;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;

    /// <summary>
    ///   Type editor for numeric collections.
    /// </summary>
    /// 
    /// <remarks>
    ///   This class can be used to edit numeric collections
    ///   more easily in property grids.
    /// </remarks>
    /// 
    public class NumericCollectionEditor : UITypeEditor
    {
        private IWindowsFormsEditorService editorService = null;

        /// <summary>
        ///   Gets the type of the items contained in the collection.
        /// </summary>
        /// 
        public Type CollectionItemType { get; private set; }

        /// <summary>
        ///   Gets the type of the collection.
        /// </summary>
        /// 
        public Type CollectionType { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="NumericCollectionEditor"/> class.
        /// </summary>
        /// 
        public NumericCollectionEditor()
        {
        }

        /// <summary>
        ///   Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> method.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// <param name="provider">An <see cref="T:System.IServiceProvider"/> that this editor can use to obtain services.</param>
        /// <param name="value">The object to edit.</param>
        /// 
        /// <returns>
        ///   The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
        /// </returns>
        /// 
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null && context.Instance != null && provider != null)
            {
                this.editorService = (IWindowsFormsEditorService)provider
                    .GetService(typeof(IWindowsFormsEditorService));

                this.CollectionType = context.PropertyDescriptor.ComponentType;
                this.CollectionItemType = detectCollectionType();

                if (editorService != null)
                {
                    NumericCollectionEditorForm form = new NumericCollectionEditorForm(this, value);

                    context.OnComponentChanging();

                    if (editorService.ShowDialog(form) == DialogResult.OK)
                    {
                        context.OnComponentChanged();
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///   Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method.
        /// </summary>
        /// 
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that can be used to gain additional context information.</param>
        /// 
        /// <returns>
        /// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"/> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"/> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"/> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"/> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"/>.
        /// </returns>
        /// 
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            if (context != null && context.Instance != null)
            {
                return UITypeEditorEditStyle.Modal;
            }
            return base.GetEditStyle(context);
        }


        private Type detectCollectionType()
        {
            PropertyInfo[] properties = TypeDescriptor.GetReflectionType(this.CollectionType)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Name == "Item" || properties[i].Name == "Items")
                    return properties[i].PropertyType;
            }

            return typeof(object);
        }

        /// <summary>
        ///   Gets the items in the collection as a <see cref="T:object[]"/>.
        /// </summary>
        /// 
        /// <param name="editValue">The collection object being edited.</param>
        /// 
        /// <returns>The items contained in <paramref name="editValue"/>.</returns>
        /// 
        public virtual object[] GetItems(object editValue)
        {
            ICollection collection = editValue as ICollection;
            Array array = editValue as Array;

            if (collection != null)
            {
                ArrayList list = new ArrayList();
                foreach (var o in collection)
                    list.Add(o);
                return list.ToArray();
            }
            else if (array != null)
            {
                object[] newArray = new object[array.Length];
                array.CopyTo(newArray, 0);
                return newArray;
            }

            return new object[0];
        }

        /// <summary>
        ///   Sets the items in the collection.
        /// </summary>
        /// 
        /// <param name="editValue">The collection object being edited.</param>
        /// <param name="values">The objects to be added in the collection.</param>
        /// 
        public virtual void SetItems(object editValue, object[] values)
        {
            IList list = editValue as IList;
            Array array = editValue as Array;

            if (list != null)
            {
                list.Clear();
                foreach (var o in values)
                    list.Add(o);
            }
            else if (array != null)
            {
                Array.Copy(values, array, values.Length);
            }
        }
    }
}

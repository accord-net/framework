using System.Collections.Generic;
using System.Windows.Forms;

namespace Accord.Controls.Cyotek.Demo
{
  // Cyotek ImageBox
  // Copyright (c) 2010-2015 Cyotek Ltd.
  // http://cyotek.com
  // http://cyotek.com/blog/tag/imagebox

  // Licensed under the MIT License. See license.txt for the full text.

  // If you use this control in your applications, attribution, donations or contributions are welcome.

  internal class PropertyGrid : System.Windows.Forms.PropertyGrid
  {
    #region Public Members

    public GridItem FindItem(string itemLabel)
    {
      // http://www.vb-helper.com/howto_net_select_propertygrid_item.html

      GridItem rootItem;
      GridItem matchingItem;
      Queue<GridItem> searchItems;

      matchingItem = null;

      // Find the GridItem root.
      rootItem = this.SelectedGridItem;
      while (rootItem.Parent != null)
      {
        rootItem = rootItem.Parent;
      }
      
      // Search the tree.
      searchItems = new Queue<GridItem>();
      
      searchItems.Enqueue(rootItem);

      while (searchItems.Count != 0 && matchingItem == null)
      {
        GridItem checkItem;

        checkItem = searchItems.Dequeue();
        
        if (checkItem.Label == itemLabel)
        {
          matchingItem = checkItem;
        }

        foreach (GridItem item in checkItem.GridItems)
        {
          searchItems.Enqueue(item);
        }
      }

      return matchingItem;
    }

    public void SelectItem(string itemLabel)
    {
      GridItem selection;

      selection = this.FindItem(itemLabel);
      if (selection != null)
      {
        try
        {
          this.SelectedGridItem = selection;
        }
        catch
        {
          // ignore
        }
      }
    }

    #endregion
  }
}

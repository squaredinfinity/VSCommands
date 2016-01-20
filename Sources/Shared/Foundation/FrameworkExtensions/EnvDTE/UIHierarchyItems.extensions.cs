using EnvDTE;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.VSCommands
{
    public static class UIHierarchyItemsExtensions
    {
        public static UIHierarchyItem GetHierarchyItem(this UIHierarchyItems uiHierarchyItems, int zeroBasedIndex)
        {
            if (uiHierarchyItems.Count == 0)
                return null;

            return uiHierarchyItems.Item(zeroBasedIndex + 1) as UIHierarchyItem;
        }

        public static IEnumerable<UIHierarchyItem> TreeTraversal(this UIHierarchyItems me)
        {
            return me.GetHierarchyItem(0).TreeTraversal();
        }
    }
}

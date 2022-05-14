using System.Collections.Generic;

namespace SnapMarket.ViewModels.Category
{
    public class TreeViewCategory
    {
        public TreeViewCategory()
        {
            subs = new List<TreeViewCategory>();
        }

        public int id { get; set; }
        public string title { get; set; }
        public List<TreeViewCategory> subs { get; set; }
    }
}

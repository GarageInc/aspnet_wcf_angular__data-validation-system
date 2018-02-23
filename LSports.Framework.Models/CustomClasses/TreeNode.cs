using System;
using System.Collections.Generic;

namespace LSports.Framework.Models.CustomClasses
{
    [Serializable]
    public class TreeNode
    {
        public string text { get; set; }

        public List<TreeNode> children { get; set; }
    }

    [Serializable]
    public class TreeData
    {
        public TreeNode data { get; set; }
    }

    [Serializable]
    public class TreeCore
    {
        public TreeData core { get; set; }
    }
}

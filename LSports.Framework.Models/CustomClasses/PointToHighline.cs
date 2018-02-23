namespace LSports.Framework.Models.CustomClasses
{
    public class PointToHighline
    {
        public string ParentNodeName { get; set; }
        public string ParentNodeIdentifier { get; set; }

        public bool isFalseIdentificator { get; set; }

        protected string groupingParam = "";

        public string ParentNodeNameAndParentNodeIdentifier
        {
            get
            {
                if (groupingParam == "")
                {
                    groupingParam = ParentNodeName + "@" + ParentNodeIdentifier;
                }

                return groupingParam;
            }
        }
        
        public string Point { get; set; }

        public override string ToString()
        {
            return ParentNodeName + "@" + (isFalseIdentificator ? "" : ParentNodeIdentifier) + "@" + Point;
        }
        /*
        public string DistinctField
        {
            get { return this.ParentNodeName + this.ParentNodeIdentifier; }   
        }
        */
    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LSports.DVS.Framework.DataAccess.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class dvs_selectiontreeitem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<long> ExternalId { get; set; }
        public Nullable<long> ParentId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> TypeId { get; set; }
        public Nullable<int> ProductId { get; set; }
    }
}

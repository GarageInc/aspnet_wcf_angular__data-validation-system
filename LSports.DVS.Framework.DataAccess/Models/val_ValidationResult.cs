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
    
    public partial class val_validationresult
    {
        public int Id { get; set; }
        public int ValidationSettingId { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string SportName { get; set; }
        public string LeagueName { get; set; }
        public string LocationName { get; set; }
        public string Market { get; set; }
        public string Provider { get; set; }
        public string Status { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public int ProductId { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public long LeagueId { get; set; }
        public long SportId { get; set; }
        public long LocationId { get; set; }
        public string PointsToHighline { get; set; }
        public string HashIndexByEventProduct { get; set; }
        public string HashUniqueIndex { get; set; }
        public int Counter { get; set; }
        public string HashValidationIndex { get; set; }
    }
}

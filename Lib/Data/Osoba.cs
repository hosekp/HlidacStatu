//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HlidacStatu.Lib.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Osoba
    {
        public int InternalId { get; set; }
        public string TitulPred { get; set; }
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
        public string TitulPo { get; set; }
        public string Pohlavi { get; set; }
        public Nullable<System.DateTime> Narozeni { get; set; }
        public Nullable<System.DateTime> Umrti { get; set; }
        public string Ulice { get; set; }
        public string Mesto { get; set; }
        public string PSC { get; set; }
        public string CountryCode { get; set; }
        public bool OnRadar { get; set; }
        public int Status { get; set; }
        public System.DateTime LastUpdate { get; set; }
        public string NameId { get; set; }
        public string PuvodniPrijmeni { get; set; }
        public string JmenoAscii { get; set; }
        public string PrijmeniAscii { get; set; }
        public string PuvodniPrijmeniAscii { get; set; }
        public Nullable<System.DateTime> ManuallyUpdated { get; set; }
        public string ManuallyUpdatedBy { get; set; }
    }
}

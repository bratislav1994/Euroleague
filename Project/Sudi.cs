//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project
{
    using System;
    using System.Collections.Generic;
    
    public partial class Sudi
    {
        public string Sudija_LICBR_SUD { get; set; }
        public string Utakmica_OZN_UTK { get; set; }
        public string ID_SUDI { get; set; }
    
        public virtual Sudija Sudija { get; set; }
        public virtual Utakmica Utakmica { get; set; }
    }
}

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
    
    public partial class IgracIgra
    {
        public int POENI_IGRACIGRA { get; set; }
        public int AS_IGRACIGRA { get; set; }
        public int SK_IGRACIGRA { get; set; }
        public string Utakmica_OZN_UTK { get; set; }
        public string Igrac_LICBR_IGR { get; set; }
    
        public virtual Igrac Igrac { get; set; }
        public virtual Utakmica Utakmica { get; set; }
    }
}

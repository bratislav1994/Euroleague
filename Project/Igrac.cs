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
    using System.ComponentModel;
    public partial class Igrac : INotifyPropertyChanged
    {
        private string licbr;
        private string ime;
        private string prz;
        private DateTime pDat;
        private DateTime kDat;
        private string idKlb;
        private string poz;
        private float vis;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Igrac()
        {
            this.IgracIgras = new HashSet<IgracIgra>();
        }

        public string LICBR_IGR
        {
            get
            {
                return this.licbr;
            }
            set
            {
                this.licbr = value;
                this.RaisePropertyChanged("LICBR_IGR");
            }
        }
        public string IME_IGR
        {
            get
            {
                return this.ime;
            }
            set
            {
                this.ime = value;
                this.RaisePropertyChanged("IME_IGR");
            }
        }
        public string PRZ_IGR
        {
            get
            {
                return this.prz;
            }
            set
            {
                this.prz = value;
                this.RaisePropertyChanged("PRZ_IGR");
            }
        }
        public System.DateTime PDATUG_IGR
        {
            get
            {
                return this.pDat;
            }
            set
            {
                this.pDat = value;
                this.RaisePropertyChanged("PDATUG_IGR");
            }
        }
        public System.DateTime KDATUG_IGR
        {
            get
            {
                return this.kDat;
            }
            set
            {
                this.kDat = value;
                this.RaisePropertyChanged("KDATUG_IGR");
            }
        }
        public string POZ_IGR
        {
            get
            {
                return this.poz;
            }
            set
            {
                this.poz = value;
                this.RaisePropertyChanged("POZ_IGR");
            }
        }
        public float VIS_IGR
        {
            get
            {
                return this.vis;
            }
            set
            {
                this.vis = value;
                this.RaisePropertyChanged("VIS_IGR");
            }
        }
        public string Klub_ID_KLB
        {
            get
            {
                return this.idKlb;
            }
            set
            {
                this.idKlb = value;
                this.RaisePropertyChanged("Klub_ID_KLB");
            }
        }

        public byte[] SLIKA_IGR { get; set; }

        public virtual Klub Klub { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IgracIgra> IgracIgras { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Igrac))
                return false;
            bool ret = ((Igrac)obj).LICBR_IGR.Equals(this.LICBR_IGR);
            return ret;
        }
    }
}

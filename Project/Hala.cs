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
    public partial class Hala : INotifyPropertyChanged
    {
        private string ozn;
        private string naz;
        private string drz;
        private string grad;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hala()
        {
            this.Rezervacijas = new HashSet<Rezervacija>();
        }

        public string OZN_HALA
        {
            get
            {
                return this.ozn;
            }
            set
            {
                this.ozn = value;
                this.RaisePropertyChanged("OZN_HALA");
            }
        }
        public string NAZ_HALA
        {
            get
            {
                return this.naz;
            }
            set
            {
                this.naz = value;
                this.RaisePropertyChanged("NAZ_HALA");
            }
        }
        public string DRZ_HALA
        {
            get
            {
                return this.drz;
            }
            set
            {
                this.drz = value;
                this.RaisePropertyChanged("DRZ_HALA");
            }
        }
        public string GRD_HALA
        {
            get
            {
                return this.grad;
            }
            set
            {
                this.grad = value;
                this.RaisePropertyChanged("GRD_HALA");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rezervacija> Rezervacijas { get; set; }

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
            if (obj == null || !(obj is Hala))
                return false;
            bool ret = ((Hala)obj).OZN_HALA.Equals(this.OZN_HALA);
            return ret;
        }
    }
}

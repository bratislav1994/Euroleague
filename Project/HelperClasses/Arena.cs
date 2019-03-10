using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperClasses
{
    public class Arena : INotifyPropertyChanged
    {
        private string idArena;
        private string name;
        private string country;
        private string place;

        public Arena()
        {

        }

        public string IdArena
        {
            get
            {
                return idArena;
            }

            set
            {
                idArena = value;
                this.RaisePropertyChanged("IdArena");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                this.RaisePropertyChanged("Country");
            }
        }

        public string Place
        {
            get
            {
                return place;
            }

            set
            {
                place = value;
                this.RaisePropertyChanged("Place");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperClasses
{
    public class Selected : INotifyPropertyChanged
    {
        public string LICBR { get; set; }
        private string name { get; set; }
        private string surname { get; set; }
        private bool isSelected;
        public bool IsSelected {
            get
            {
                return this.isSelected;
            }
            set
            {
                isSelected = value;
                RaisePropertyChanged("IsSelected");
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        public string Surname
        {
            get { return this.surname; }
            set
            {
                surname = value;
                RaisePropertyChanged("Surname");
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

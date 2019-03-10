using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperClasses
{
    public class Standings : INotifyPropertyChanged
    { 
        private int number;
        private string clubId;
        private string clubName;
        private int winNumber;
        private int lossNumber;
        private int diff;
        private int points;
        
        public Standings()
        {

        }
        
        public int Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
                RaisePropertyChanged("Number");
            }
        }
        
        public string ClubName
        {
            get
            {
                return clubName;
            }

            set
            {
                clubName = value;
                RaisePropertyChanged("ClubName");
            }
        }
        
        public int WinNumber
        {
            get
            {
                return winNumber;
            }

            set
            {
                winNumber = value;
                RaisePropertyChanged("WinNumber");
            }
        }
        
        public int LossNumber
        {
            get
            {
                return lossNumber;
            }

            set
            {
                lossNumber = value;
                RaisePropertyChanged("LossNumber");
            }
        }

        public int Diff
        {
            get
            {
                return diff;
            }

            set
            {
                diff = value;
                RaisePropertyChanged("Diff");
            }
        }

        public int Points
        {
            get
            {
                return points;
            }

            set
            {
                points = value;
            }
        }

        public string ClubId
        {
            get
            {
                return clubId;
            }

            set
            {
                clubId = value;
                RaisePropertyChanged("ClubId");
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

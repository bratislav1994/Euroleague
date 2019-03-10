using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperClasses
{
    public class Game : INotifyPropertyChanged
    {
        private string homePts;
        private string awayPts;
        private string homeName;
        private string awayName;
        private int rbr;

        public Game()
        {

        }

        public string HomePts
        {
            get
            {
                return this.homePts;
            }
            set
            {
                this.homePts = value;
                this.RaisePropertyChanged("HomePts");
            }
        }
        public string AwayPts
        {
            get
            {
                return this.awayPts;
            }
            set
            {
                this.awayPts = value;
                this.RaisePropertyChanged("AwayPts");
            }
        }

        public string HomeName
        {
            get
            {
                return homeName;
            }

            set
            {
                homeName = value;
                this.RaisePropertyChanged("HomeName");
            }
        }

        public string AwayName
        {
            get
            {
                return awayName;
            }

            set
            {
                awayName = value;
                this.RaisePropertyChanged("AwayName");
            }
        }

        public int Rbr
        {
            get
            {
                return rbr;
            }

            set
            {
                rbr = value;
                this.RaisePropertyChanged("Rbr");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}

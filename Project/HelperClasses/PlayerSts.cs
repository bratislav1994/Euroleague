using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Design;

namespace Project.HelperClasses
{
    public class PlayerSts : INotifyPropertyChanged
    {
        private string idPlayer;
        private string name;
        private int games;
        private double pts;
        private double assist;
        private double reb;

        public event PropertyChangedEventHandler PropertyChanged;

        public PlayerSts()
        {

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
            }
        }

        public double Pts
        {
            get
            {
                return pts;
            }

            set
            {
                pts = value;
                this.RaisePropertyChanged("Pts");
            }
        }

        public double Assist
        {
            get
            {
                return assist;
            }

            set
            {
                assist = value;
                this.RaisePropertyChanged("Assist");
            }
        }

        public double Reb
        {
            get
            {
                return reb;
            }

            set
            {
                reb = value;
                this.RaisePropertyChanged("Reb");
            }
        }

        public string IdPlayer
        {
            get
            {
                return idPlayer;
            }

            set
            {
                idPlayer = value;
            }
        }

        public int Games
        {
            get
            {
                return games;
            }

            set
            {
                games = value;
                this.RaisePropertyChanged("Games");
            }
        }

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}

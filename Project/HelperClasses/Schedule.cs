using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.HelperClasses
{
    public class Schedule : INotifyPropertyChanged, IDataErrorInfo
    {
        private string idTeam1;
        private string idTeam2;
        private string idGame;
        private string team1;
        private string team2;
        private int? pointsTeam1;
        private int? pointsTeam2;

        public Schedule()
        {

        }

        public string Team1
        {
            get
            {
                return team1;
            }

            set
            {
                team1 = value;
                this.RaisePropertyChanged("Team1");
            }
        }

        public string Team2
        {
            get
            {
                return team2;
            }

            set
            {
                team2 = value;
                this.RaisePropertyChanged("Team2");
            }
        }

        public int? PointsTeam1
        {
            get
            {
                return pointsTeam1;
            }

            set
            {
                pointsTeam1 = value;
                this.RaisePropertyChanged("PointsTeam1");
            }
        }

        public int? PointsTeam2
        {
            get
            {
                return pointsTeam2;
            }

            set
            {
                pointsTeam2 = value;
                this.RaisePropertyChanged("PointsTeam2");
            }
        }

        public string IdTeam1
        {
            get
            {
                return idTeam1;
            }

            set
            {
                idTeam1 = value;
                this.RaisePropertyChanged("IdTeam1");
            }
        }

        public string IdTeam2
        {
            get
            {
                return idTeam2;
            }

            set
            {
                idTeam2 = value;
                this.RaisePropertyChanged("IdTeam2");
            }
        }

        public string IdGame
        {
            get
            {
                return idGame;
            }

            set
            {
                idGame = value;
                this.RaisePropertyChanged("IdGame");
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

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
        }

        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName.Equals("PointsTeam1"))
                {
                    if (this.PointsTeam1 == null || this.PointsTeam1 < 0)
                    {
                        error = "Pts must be positive number!";
                    }
                    else
                    {
                        if (this.PointsTeam1 == this.PointsTeam2)
                        {
                            error = "The result can't be draw!";
                        }
                    }
                }
                else if (columnName.Equals("PointsTeam2"))
                {
                    if (this.PointsTeam2 == null || this.PointsTeam2 < 0)
                    {
                        error = "Pts must be positive number!";
                    }
                    else
                    {
                        if (this.PointsTeam1 == this.PointsTeam2)
                        {
                            error = "The result can't be draw!";
                        }
                    }
                }

                return error;
            }
        }
    }
}

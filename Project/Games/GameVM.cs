using Project.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Games
{
    public abstract class GameVM : INotifyPropertyChanged
    {
        private string firstTeam;
        private string secondTeam;
        private int firstPts;
        private int secondPts;
        public string idLeague = "evroliga01";

        private string idTeam1;
        private string idTeam2;
        private string idGame;
        private BindingList<Player> playersHome = null;
        private BindingList<Player> players = null;
        private Visibility addRemoveFromHomeTeamVisibility;
        private Visibility addRemoveFromAwayTeamVisibility;
        private Visibility homeTableAdminVisibility;
        private Visibility awayTableAdminVisibility;
        private Visibility homeTableUserVisibility;
        private Visibility awayTableUserVisibility;

        public GameVM()
        {
            this.Players = new BindingList<Player>();
            this.PlayersHome = new BindingList<Player>();
        }

        public string FirstTeam
        {
            get
            {
                return firstTeam;
            }

            set
            {
                firstTeam = value;
                this.RaisePropertyChanged("FirstTeam");
            }
        }

        public string SecondTeam
        {
            get
            {
                return secondTeam;
            }

            set
            {
                secondTeam = value;
                this.RaisePropertyChanged("SecondTeam");
            }
        }

        public int FirstPts
        {
            get
            {
                return firstPts;
            }

            set
            {
                firstPts = value;
                this.RaisePropertyChanged("FirstPts");
            }
        }

        public int SecondPts
        {
            get
            {
                return secondPts;
            }

            set
            {
                secondPts = value;
                this.RaisePropertyChanged("SecondPts");
            }
        }

        public BindingList<Player> PlayersHome
        {
            get
            {
                return playersHome;
            }

            set
            {
                playersHome = value;
            }
        }

        public BindingList<Player> Players
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
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

        public Visibility AddRemoveFromHomeTeamVisibility
        {
            get
            {
                return addRemoveFromHomeTeamVisibility;
            }

            set
            {
                addRemoveFromHomeTeamVisibility = value;
            }
        }

        public Visibility AddRemoveFromAwayTeamVisibility
        {
            get
            {
                return addRemoveFromAwayTeamVisibility;
            }

            set
            {
                addRemoveFromAwayTeamVisibility = value;
            }
        }

        public Visibility HomeTableAdminVisibility
        {
            get
            {
                return homeTableAdminVisibility;
            }

            set
            {
                homeTableAdminVisibility = value;
            }
        }

        public Visibility AwayTableAdminVisibility
        {
            get
            {
                return awayTableAdminVisibility;
            }

            set
            {
                awayTableAdminVisibility = value;
            }
        }

        public Visibility HomeTableUserVisibility
        {
            get
            {
                return homeTableUserVisibility;
            }

            set
            {
                homeTableUserVisibility = value;
            }
        }

        public Visibility AwayTableUserVisibility
        {
            get
            {
                return awayTableUserVisibility;
            }

            set
            {
                awayTableUserVisibility = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Init()
        {
            this.FillPlayersForTeam();
            this.InitializeView();
        }

        protected abstract void InitializeView();

        public void FillPlayersForTeam()
        {
            using (var db = new EuroleagueEntities3())
            {
                var playersFromBothTeams = (from t1 in db.IgracIgras
                                            join t2 in db.Igracs on t1.Igrac_LICBR_IGR equals t2.LICBR_IGR
                                            where t1.Utakmica_OZN_UTK.Equals(this.IdGame)
                                            select new { t1, t2 }).ToList();

                foreach (var t in playersFromBothTeams)
                {
                    IgracIgra sts = t.t1;
                    Igrac i = t.t2;

                    if (i.Klub_ID_KLB.Equals(this.IdTeam1))
                    {
                        this.PlayersHome.Add(new Player()
                        {
                            Pts = sts.POENI_IGRACIGRA,
                            Assist = sts.AS_IGRACIGRA,
                            Reb = sts.SK_IGRACIGRA,
                            Name = i.IME_IGR + " " + i.PRZ_IGR,
                            IdPlayer = i.LICBR_IGR,
                            IdClub = this.IdTeam1
                        });
                    }
                    else if (i.Klub_ID_KLB.Equals(this.IdTeam2))
                    {
                        this.Players.Add(new Player()
                        {
                            Pts = sts.POENI_IGRACIGRA,
                            Assist = sts.AS_IGRACIGRA,
                            Reb = sts.SK_IGRACIGRA,
                            Name = i.IME_IGR + " " + i.PRZ_IGR,
                            IdPlayer = i.LICBR_IGR,
                            IdClub = this.IdTeam2
                        });
                    }
                }
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

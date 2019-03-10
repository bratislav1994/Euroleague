using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Players
{
    public class PlayerModel : INotifyPropertyChanged
    {
        private ObservableCollection<Igrac> playersList = null;
        private ObservableCollection<Klub> clubs = null;
        private BindingList<string> positions;

        public PlayerModel()
        {
            this.playersList = this.GetAllPlayers();
            this.clubs = this.GetAllClubs();
            this.positions = this.GetAllPositions();
        }
        
        public ObservableCollection<Igrac> PlayersList
        {
            get
            {
                return playersList;
            }

            set
            {
                playersList = value;
                this.RaisePropertyChanged("PlayersList");
            }
        }

        public ObservableCollection<Klub> Clubs
        {
            get
            {
                return clubs;
            }

            set
            {
                clubs = value;
            }
        }

        public BindingList<string> Positions
        {
            get
            {
                return positions;
            }

            set
            {
                positions = value;
            }
        }

        public bool DateValid(string date)
        {
            try
            {
                DateTime.Parse(date);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public bool IsStartBeforeEnd(string start, string end)
        {
            return DateTime.Parse(start) < DateTime.Parse(end);
        }

        //public ObservableCollection<Igrac> GetAllPlayersById(string clubId)
        //{
        //    using (var db = new EuroleagueEntities4())
        //    {
        //        return new ObservableCollection<Igrac>(db.Igracs.Where(x => x.Klub_ID_KLB.Equals(clubId)).ToList());
        //    }
        //}

        public ObservableCollection<Igrac> GetAllPlayers()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Igrac>(db.Igracs.Include("Klub").ToList());
            }
        }

        public Igrac GetPlayerById(string id)
        {
            using (var db = new EuroleagueEntities3())
            {
                return db.Igracs.Include("Klub").Where(x => x.LICBR_IGR.Equals(id)).FirstOrDefault();
            }
        }

        public ObservableCollection<Klub> GetAllClubs()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Klub>(db.Klubs.ToList());
            }
        }

        public Klub GetClubById(string id)
        {
            return this.Clubs.Where(x => x.ID_KLB.Equals(id)).FirstOrDefault();
        }

        private BindingList<string> GetAllPositions()
        {
            return new BindingList<string>(Enum.GetValues(typeof(PositionEnum)).Cast<PositionEnum>()
                                            .Select(v => v.ToFriendlyString()).ToList());
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

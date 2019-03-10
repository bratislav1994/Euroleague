using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Project.Roster
{
    public class RosterModel : INotifyPropertyChanged
    {
        private BindingList<Klub> clubs = null;
        private ObservableCollection<Igrac> playersList = null;

        public RosterModel()
        {
            this.Clubs = this.GetAllClubs();
        }

        public BindingList<Klub> Clubs
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

        //public ObservableCollection<Igrac> PlayersList
        //{
        //    get
        //    {
        //        return playersList;
        //    }

        //    set
        //    {
        //        playersList = value;
        //        this.RaisePropertyChanged("Players");
        //    }
        //}

        public BindingList<Klub> GetAllClubs()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new BindingList<Klub>(db.Klubs.ToList());
            }
        }

        public Igrac GetPlayerById(string id)
        {
            using (var db = new EuroleagueEntities3())
            {
                return db.Igracs.Include("Klub").Where(x => x.LICBR_IGR.Equals(id)).FirstOrDefault();
            }
        }

        public ObservableCollection<Igrac> GetAllPlayers()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Igrac>(db.Igracs.Include("Klub").ToList());
            }
        }

        public ObservableCollection<Igrac> GetAllPlayersByClubId(string clubId)
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Igrac>(db.Igracs.Where(x => x.Klub_ID_KLB.Equals(clubId)).ToList());
            }
        }

        public Klub GetClubById(string id)
        {
            return this.Clubs.Where(x => x.ID_KLB.Equals(id)).FirstOrDefault();
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

using Project.HelperClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.GamesByClub
{
    public class GamesByClubModel : INotifyPropertyChanged
    {
        private BindingList<Klub> clubs = null;
        private ObservableCollection<Utakmica> gamesList = null;

        public GamesByClubModel()
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

        public ObservableCollection<Utakmica> GamesList
        {
            get
            {
                return gamesList;
            }

            set
            {
                gamesList = value;
                this.RaisePropertyChanged("GamesList");
            }
        }

        public BindingList<Klub> GetAllClubs()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new BindingList<Klub>(db.Klubs.ToList());
            }
        }

        public void GetAllGamesByClubId(string clubId)
        {
            using (var db = new EuroleagueEntities3())
            {
                 this.GamesList = new ObservableCollection<Utakmica>(db.Utakmicas.Include("Klub").Include("Klub1").Where(x => x.Klub_ID_KLB.Equals(clubId) || x.Klub_ID_KLB1.Equals(clubId)).OrderBy(x => x.RBRKOLA_UTK).ToList());
                  
            //this.Games = new BindingList<Game>();
                //g.ForEach(x => this.Games.Add(new Game()
                //{
                //    Rbr = x.RBRKOLA_UTK,
                //    HomeName = x.Klub.NAZ_KLB,
                //    AwayName = x.Klub1.NAZ_KLB,
                //    HomePts = x.DOMPOENI_UTK != null ? x.DOMPOENI_UTK.ToString() : string.Empty,
                //    AwayPts = x.GOSTPOENI_UTK != null ? x.GOSTPOENI_UTK.ToString() : string.Empty
                //}));
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

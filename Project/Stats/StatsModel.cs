using Project.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Stats
{
    public class StatsModel : INotifyPropertyChanged
    {
        private BindingList<Klub> clubs = null;
        private BindingList<PlayerSts> playersList = null;

        public StatsModel()
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

        public BindingList<PlayerSts> PlayersList
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

        public BindingList<Klub> GetAllClubs()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new BindingList<Klub>(db.Klubs.ToList());
            }
        }

        public void GetAllPlayersByClubId(string clubId)
        {
            using (var db = new EuroleagueEntities3())
            {
                List<Igrac> players = new List<Igrac>(db.Igracs.Where(x => x.Klub_ID_KLB.Equals(clubId)).ToList());
                this.PlayersList = new BindingList<PlayerSts>();
                players.ForEach(x => this.PlayersList.Add(this.GetPlayerSts(x.LICBR_IGR)));
            }
        }

        private PlayerSts GetPlayerSts(string playerId)
        {
            ObjectParameter name = new ObjectParameter("name", "");
            ObjectParameter games = new ObjectParameter("games", 0);
            ObjectParameter pts = new ObjectParameter("pts", 0);
            ObjectParameter assist = new ObjectParameter("as", 0);
            ObjectParameter reb = new ObjectParameter("reb", 0);
            using (var db = new EuroleagueEntities3())
            {
                db.Stats(playerId, name, games, pts, assist, reb);
                return new PlayerSts()
                {
                    Pts = Math.Round(float.Parse(pts.Value.ToString()), 2),
                    Assist = Math.Round(float.Parse(assist.Value.ToString()), 2),
                    Games = int.Parse(games.Value.ToString()),
                    Name = name.Value.ToString(),
                    Reb = Math.Round(float.Parse(reb.Value.ToString()), 2),
                    IdPlayer = playerId
                };

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

using Project.GamesByClub;
using Project.Roster;
using Project.Stats;
using Project.Table;
using System.Windows;

namespace Project.ViewModel
{
    public abstract class MasterVM
    {
        private StandingsVM standingsVM = null;
        private Visibility tabVisibility;
        private RosterVM rosterVM = null;
        private GamesByClubVM gamesByClubVM = null;
        private StatsVM statsVM = null;
        private Visibility competitionVisibility;

        public MasterVM()
        {
            this.StandingsVM = new StandingsVM();
            this.RosterVM = new RosterVM();
            this.GamesByClubVM = new GamesByClubVM();
            this.StatsVM = new StatsVM(); 
        }

        public StandingsVM StandingsVM
        {
            get
            {
                return standingsVM;
            }

            set
            {
                standingsVM = value;
            }
        }
        
        public Visibility TabVisibility
        {
            get
            {
                return tabVisibility;
            }

            set
            {
                tabVisibility = value;
            }
        }

        public RosterVM RosterVM
        {
            get
            {
                return rosterVM;
            }

            set
            {
                rosterVM = value;
            }
        }

        public GamesByClubVM GamesByClubVM
        {
            get
            {
                return gamesByClubVM;
            }

            set
            {
                gamesByClubVM = value;
            }
        }

        public StatsVM StatsVM
        {
            get
            {
                return statsVM;
            }

            set
            {
                statsVM = value;
            }
        }

        public Visibility CompetitionVisibility
        {
            get
            {
                return competitionVisibility;
            }

            set
            {
                competitionVisibility = value;
            }
        }

        protected virtual void InitializeView()
        {
        }
    }
}

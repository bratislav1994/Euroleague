using Project.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Project.Games
{
    public class UserGameVM : GameVM
    {

        public UserGameVM(Schedule game) : base()
        {
            this.FirstTeam = game.Team1;
            this.SecondTeam = game.Team2;
            this.FirstPts = (int)game.PointsTeam1;
            this.SecondPts = (int)game.PointsTeam2;
            this.IdGame = game.IdGame;
            this.IdTeam1 = game.IdTeam1;
            this.IdTeam2 = game.IdTeam2;

            this.Init();
        }

        protected override void InitializeView()
        {
            this.AddRemoveFromAwayTeamVisibility = Visibility.Hidden;
            this.AddRemoveFromHomeTeamVisibility = Visibility.Hidden;
            this.AwayTableAdminVisibility = Visibility.Hidden;
            this.AwayTableUserVisibility = Visibility.Visible;
            this.HomeTableAdminVisibility = Visibility.Hidden;
            this.HomeTableUserVisibility = Visibility.Visible;
        }
    }
}

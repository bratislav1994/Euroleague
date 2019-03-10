using Prism.Commands;
using Project.Arena;
using Project.Coach;
using Project.Games;
using Project.Login;
using Project.Players;
using Project.Referee;
using Project.Schedules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.ViewModel
{
    public class AdminMasterVM : MasterVM
    {
        private ArenaVM arenaVM = null;
        private RefereeVM refereeVM = null;
        private CoachVM coachVM = null;
        private PlayerVM playerVM = null;
        private GameVM gameVM = null;
        private ScheduleVM scheduleVM = null;
        private DelegateCommand generateCommand;

        public AdminMasterVM() : base()
        {
            this.ArenaVM = new ArenaVM();
            this.RefereeVM = new RefereeVM();
            this.CoachVM = new CoachVM();
            this.PlayerVM = new PlayerVM();
            this.ScheduleVM = new ScheduleVM(Roles.Admin);
            this.InitializeView();
        }

        public GameVM GameVM
        {
            get
            {
                return gameVM;
            }

            set
            {
                gameVM = value;
            }
        }

        public ScheduleVM ScheduleVM
        {
            get
            {
                return scheduleVM;
            }

            set
            {
                scheduleVM = value;
            }
        }
        
        public ArenaVM ArenaVM
        {
            get
            {
                return arenaVM;
            }

            set
            {
                arenaVM = value;
            }
        }

        public RefereeVM RefereeVM
        {
            get
            {
                return refereeVM;
            }

            set
            {
                refereeVM = value;
            }
        }

        public CoachVM CoachVM
        {
            get
            {
                return coachVM;
            }

            set
            {
                coachVM = value;
            }
        }

        public PlayerVM PlayerVM
        {
            get
            {
                return playerVM;
            }

            set
            {
                playerVM = value;
            }
        }

        public DelegateCommand GenerateCommand
        {
            get
            {
                if (this.generateCommand == null)
                {
                    this.generateCommand = new DelegateCommand(this.Generate, this.CanGenerate);
                }

                return this.generateCommand;
            }
        }

        private bool CanGenerate()
        {
            using (var db = new EuroleagueEntities3())
            {
                int clubs = db.Klubs.Count();
                return !db.Utakmicas.Any() && clubs > 0 && clubs%2 == 0;
            }
        }

        private void Generate()
        {
            this.ScheduleVM.Model.RoundRobinSchedule();
        }

        protected override void InitializeView()
        {

        }
    }
}

using Project.Games;
using Project.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Project.Schedules;

namespace Project.ViewModel
{
    public class UserMasterVM : MasterVM
    {
        private ScheduleVM scheduleVM = null;

        public UserMasterVM() : base()
        {
            this.scheduleVM = new ScheduleVM(Roles.User);
            this.InitializeView();
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

        protected override void InitializeView()
        {
            this.TabVisibility = Visibility.Hidden;
            this.CompetitionVisibility = Visibility.Hidden;
        }
    }
}

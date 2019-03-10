using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Project.Table
{
    public class StandingsVM
    {
        private ClientStandingsModel client = null;
        private ICommand clickStandingsCommand;

        public StandingsVM()
        {
            this.Client = new ClientStandingsModel();
        }

        public ClientStandingsModel Client
        {
            get
            {
                return this.client;
            }

            set
            {
                this.client = value;
            }
        }
        
        public ICommand ClickStandingsCommand
        {
            get { return this.clickStandingsCommand ?? (clickStandingsCommand = new DelegateCommand(TableView)); }
        }

        private void TableView()
        {
            this.Client.SortTable();
        }
    }
}

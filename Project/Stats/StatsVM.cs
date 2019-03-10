using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Stats
{
    public class StatsVM : INotifyPropertyChanged
    {
        private StatsModel model;
        private Klub club;
        private ICommand clickStatsCommand;
        private ICollectionView players;

        public StatsVM()
        {
            this.Model = new StatsModel();
            this.Players = new CollectionViewSource { Source = this.Model.PlayersList }.View;
        }

        public ICollectionView Players
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
                this.RaisePropertyChanged("Players");
            }
        }

        public StatsModel Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;
            }
        }

        public Klub Club
        {
            get
            {
                return club;
            }

            set
            {
                club = value;
                this.RaisePropertyChanged("Club");
            }
        }

        public ICommand ClickStatsCommand
        {
            get { return this.clickStatsCommand ?? (clickStatsCommand = new DelegateCommand(Refresh)); }
        }
        
        private void Refresh()
        {
            if (this.Club != null)
            {
                this.Model.GetAllPlayersByClubId(this.Club.ID_KLB);
                this.Players = CollectionViewSource.GetDefaultView(this.Model.PlayersList);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                if (propName.Equals("Club"))
                {
                    if (this.Club != null)
                    {
                        this.Model.GetAllPlayersByClubId(this.Club.ID_KLB);
                        this.Players = CollectionViewSource.GetDefaultView(this.Model.PlayersList);
                    }
                }
            }
        }
    }
}

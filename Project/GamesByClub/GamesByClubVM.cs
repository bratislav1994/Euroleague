using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.GamesByClub
{
    public class GamesByClubVM : INotifyPropertyChanged
    {
        private GamesByClubModel model;
        private Klub club;
        private ICommand clickGamesCommand;
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string clubFilter = string.Empty;
        private string club1Filter = string.Empty;
        private ICollectionView games;

        public GamesByClubVM()
        {
            this.Model = new GamesByClubModel();
            this.Games = new CollectionViewSource { Source = this.Model.GamesList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Games = CollectionViewSource.GetDefaultView(this.Model.GamesList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[GamesHeaderDG.Klub.ToString()] = string.Empty;
            columnFilters[GamesHeaderDG.Klub1.ToString()] = string.Empty;
        }

        public string ClubFilter
        {
            get
            {
                return clubFilter;
            }

            set
            {
                clubFilter = value;
                this.RaisePropertyChanged("ClubFilter");
                columnFilters[GamesHeaderDG.Klub.ToString()] = this.clubFilter;
                this.OnFilterApply();
            }
        }

        public string Club1Filter
        {
            get
            {
                return club1Filter;
            }

            set
            {
                club1Filter = value;
                this.RaisePropertyChanged("Club1Filter");
                columnFilters[GamesHeaderDG.Klub.ToString()] = this.club1Filter;
                this.OnFilterApply();
            }
        }

        public ICollectionView Games
        {
            get
            {
                return games;
            }

            set
            {
                games = value;
                this.RaisePropertyChanged("Games");
            }
        }

        public GamesByClubModel Model
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

        public ICommand ClickGamesCommand
        {
            get { return this.clickGamesCommand ?? (clickGamesCommand = new DelegateCommand(Refresh)); }
        }
        
        private void Refresh()
        {
            if (this.Club != null)
            {
                this.Model.GetAllGamesByClubId(this.Club.ID_KLB);
                this.Games = CollectionViewSource.GetDefaultView(this.Model.GamesList);
            }
        }

        #region filter

        public void OnFilterApply()
        {
            this.Games = CollectionViewSource.GetDefaultView(this.Model.GamesList);
            if (this.Games != null)
            {
                this.Games.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(GamesHeaderDG.Klub.ToString()))
                            {
                                containsFilter = ((Utakmica)item).Klub.NAZ_KLB.IndexOf(ClubFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(GamesHeaderDG.Klub1.ToString()))
                            {
                                containsFilter = ((Utakmica)item).Klub1.NAZ_KLB.IndexOf(Club1Filter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }

                            if (!containsFilter)
                            {
                                show = false;
                                break;
                            }
                        }
                    }

                    return show;
                };
            }
        }

        private object GetPropertyValue(object item, string property)
        {
            object value = null;

            PropertyInfo pi = null;
            if (propertyCache.ContainsKey(property))
            {
                pi = propertyCache[property];
            }
            else
            {
                pi = item.GetType().GetProperty(property);
                propertyCache.Add(property, pi);
            }

            if (pi != null)
            {
                value = pi.GetValue(item, null);
            }

            return value;
        }

        #endregion

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
                        this.Model.GetAllGamesByClubId(this.Club.ID_KLB);
                        this.Games = CollectionViewSource.GetDefaultView(this.Model.GamesList);
                    }
                }
            }
        }
    }
}

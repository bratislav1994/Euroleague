using Prism.Commands;
using Project.Players;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Project.Roster
{
    public class RosterVM : INotifyPropertyChanged
    {
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private RosterModel model;
        private Klub club;
        private ICommand clickRosterCommand;
        private BitmapImage playerImage;
        private Igrac selPlayer = null;
        private ICollectionView players;
        private Visibility playerCanvasVis = Visibility.Hidden;
        private Visibility rectangleVis = Visibility.Hidden;
        private string height = string.Empty;
        private string name = string.Empty;
        private string position = string.Empty;
        private string contract = string.Empty;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;
        private string clubFilter = string.Empty;
        private ObservableCollection<Igrac> playersList;

        public RosterVM()
        {
            this.Model = new RosterModel();
            this.PlayersList = this.Model.GetAllPlayers();
            this.Players = new CollectionViewSource { Source = this.PlayersList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Players = CollectionViewSource.GetDefaultView(this.PlayersList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[PlayerHeaderDG.IME_IGR.ToString()] = string.Empty;
            columnFilters[PlayerHeaderDG.PRZ_IGR.ToString()] = string.Empty;
            columnFilters[PlayerHeaderDG.Klub.ToString()] = string.Empty;
        }

        public ObservableCollection<Igrac> PlayersList
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

        public string NameFilter
        {
            get
            {
                return nameFilter;
            }

            set
            {
                nameFilter = value;
                this.RaisePropertyChanged("NameFilter");
                columnFilters[PlayerHeaderDG.IME_IGR.ToString()] = this.nameFilter;
                this.OnFilterApply();
            }
        }

        public string SurnameFilter
        {
            get
            {
                return surnameFilter;
            }

            set
            {
                surnameFilter = value;
                this.RaisePropertyChanged("SurnameFilter");
                columnFilters[PlayerHeaderDG.PRZ_IGR.ToString()] = this.surnameFilter;
                this.OnFilterApply();
            }
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
                columnFilters[PlayerHeaderDG.Klub.ToString()] = this.clubFilter;
                this.OnFilterApply();
            }
        }

        public RosterModel Model
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
                if (this.club != null)
                {
                    columnFilters[PlayerHeaderDG.Klub.ToString()] = this.club.NAZ_KLB;
                    this.OnFilterApply();
                }
            }
        }

        public ICommand ClickRosterCommand
        {
            get { return this.clickRosterCommand ?? (clickRosterCommand = new DelegateCommand(Refresh)); }
        }

        public BitmapImage PlayerImage
        {
            get
            {
                return playerImage;
            }

            set
            {
                playerImage = value;
                this.RaisePropertyChanged("PlayerImage");
            }
        }

        public Igrac SelPlayer
        {
            get
            {
                return this.selPlayer;
            }

            set
            {
                this.selPlayer = value;
                this.RaisePropertyChanged("SelPlayer");
            }
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

        public Visibility PlayerCanvasVis
        {
            get
            {
                return playerCanvasVis;
            }

            set
            {
                playerCanvasVis = value;
                this.RaisePropertyChanged("PlayerCanvasVis");
            }
        }

        public string Height
        {
            get
            {
                return height;
            }

            set
            {
                height = value;
                this.RaisePropertyChanged("Height");
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                this.RaisePropertyChanged("Name");
            }
        }

        public string Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
                this.RaisePropertyChanged("Position");
            }
        }

        public string Contract
        {
            get
            {
                return contract;
            }

            set
            {
                contract = value;
                this.RaisePropertyChanged("Contract");
            }
        }

        public Visibility RectangleVis
        {
            get
            {
                return rectangleVis;
            }

            set
            {
                rectangleVis = value;
                this.RaisePropertyChanged("RectangleVis");
            }
        }

        private void Refresh()
        {
            // if (this.Club != null)
            // {
            this.PlayersList.Clear();
            List<Igrac> temp = this.Model.GetAllPlayers().ToList();
            temp.ForEach(x => this.PlayersList.Add(x));
            this.PlayersList = this.Model.GetAllPlayers();// (this.Club.ID_KLB);
            
            //   this.Players = CollectionViewSource.GetDefaultView(this.PlayersList);
            // }
        }

        #region filter

        public void OnFilterApply()
        {
            this.Players = CollectionViewSource.GetDefaultView(this.PlayersList);
            this.SelPlayer = null;
            if (this.Players != null)
            {
                this.Players.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(PlayerHeaderDG.IME_IGR.ToString()))
                            {
                                containsFilter = ((Igrac)item).IME_IGR.IndexOf(NameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(PlayerHeaderDG.PRZ_IGR.ToString()))
                            {
                                containsFilter = ((Igrac)item).PRZ_IGR.IndexOf(SurnameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(PlayerHeaderDG.Klub.ToString()))
                            {
                                containsFilter = ((Igrac)item).Klub.NAZ_KLB.IndexOf(ClubFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

                //if (propName.Equals("Club"))
                //{

                //}
                if (propName.Equals("SelPlayer"))
                {
                    if (this.SelPlayer != null)
                    {
                        this.PlayerCanvasVis = Visibility.Visible;
                        this.Name = this.SelPlayer.IME_IGR + " " + this.SelPlayer.PRZ_IGR;
                        this.Position = "Position: " + this.SelPlayer.POZ_IGR;
                        this.Contract = "Contract: " + this.SelPlayer.PDATUG_IGR.ToString("d") + " - " + this.SelPlayer.KDATUG_IGR.ToString("d");
                        this.Height = "Height: " + this.SelPlayer.VIS_IGR + "m";

                        try
                        {
                            this.PlayerImage = this.ImageFromBuffer(this.SelPlayer.SLIKA_IGR);
                            this.RectangleVis = Visibility.Hidden;
                        }
                        catch (FileNotFoundException)
                        {
                            this.RectangleVis = Visibility.Visible;
                        }
                        catch (ArgumentNullException)
                        {
                            this.RectangleVis = Visibility.Visible;
                        }
                    }
                }
            }
        }

        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }
    }
}

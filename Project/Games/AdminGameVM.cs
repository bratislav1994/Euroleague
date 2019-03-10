using Prism.Commands;
using Project.HelperClasses;
using Project.Schedules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Games
{
    public class AdminGameVM : GameVM, INotifyPropertyChanged, IDataErrorInfo
    {
        private DelegateCommand<string> addRemovePlayerPlay;
        private DelegateCommand<string> stsCommand;
        private DelegateCommand editCommand;
        private DelegateCommand cancelCommand;
        private DelegateCommand applyCommand;
        private ListOfPlayersWindow win = null;
        private StsWindow stsWin = null;
        private Player playerSts;
        private int? pts;
        private int? assist;
        private int? reb;
        private ClientGame client;
        private string applyClubId;
        private string stsPlayerName;
        private ICollectionView players = null;
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;

        public AdminGameVM(Schedule game) : base()
        {
            this.FirstTeam = game.Team1;
            this.SecondTeam = game.Team2;
            this.FirstPts = (int)game.PointsTeam1;
            this.SecondPts = (int)game.PointsTeam2;
            this.IdGame = game.IdGame;
            this.IdTeam1 = game.IdTeam1;
            this.IdTeam2 = game.IdTeam2;
            this.client = new ClientGame();

            this.Init();
            this.SelPlayers = new CollectionViewSource { Source = this.ListOfPlayers }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[SelectedDGHeader.Name.ToString()] = string.Empty;
            columnFilters[SelectedDGHeader.Surname.ToString()] = string.Empty;
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
                columnFilters[SelectedDGHeader.Name.ToString()] = this.nameFilter;
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
                columnFilters[SelectedDGHeader.Surname.ToString()] = this.surnameFilter;
                this.OnFilterApply();
            }
        }

        public ICollectionView SelPlayers
        {
            get
            {
                return players;
            }

            set
            {
                players = value;
                this.RaisePropertyChanged("SelPlayers");
            }
        }

        public Player PlayerSts
        {
            get
            {
                return this.playerSts;
            }

            set
            {
                this.playerSts = value;
            }
        }

        private ObservableCollection<Selected> listOfPlayers;
        public ObservableCollection<Selected> ListOfPlayers {
            get
            {
                return this.listOfPlayers;
            }
            set
            {
                this.listOfPlayers = value;
                this.RaisePropertyChanged("ListOfPlayers");
            }
        }

        public StsWindow StsWin
        {
            get
            {
                return stsWin;
            }

            set
            {
                stsWin = value;
            }
        }

        public int? Pts
        {
            get
            {
                return pts;
            }

            set
            {
                pts = value;
                this.RaisePropertyChanged("Pts");
            }
        }

        public int? Assist
        {
            get
            {
                return assist;
            }

            set
            {
                assist = value;
                this.RaisePropertyChanged("Assist");
            }
        }

        public int? Reb
        {
            get
            {
                return reb;
            }

            set
            {
                reb = value;
                this.RaisePropertyChanged("Reb");
            }
        }

        public string StsPlayerName
        {
            get
            {
                return stsPlayerName;
            }

            set
            {
                stsPlayerName = value;
                this.RaisePropertyChanged("StsPlayerName");
            }
        }

        public ClientGame Client
        {
            get
            {
                return client;
            }

            set
            {
                client = value;
            }
        }

        protected override void InitializeView()
        {
            this.AwayTableAdminVisibility = Visibility.Visible;
            this.AwayTableUserVisibility = Visibility.Hidden;
            this.HomeTableAdminVisibility = Visibility.Visible;
            this.HomeTableUserVisibility = Visibility.Hidden;
        }

        #region filter

        public void OnFilterApply()
        {
            this.SelPlayers = CollectionViewSource.GetDefaultView(this.ListOfPlayers);
            if (this.SelPlayers != null)
            {
                this.SelPlayers.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(SelectedDGHeader.Name.ToString()))
                            {
                                containsFilter = ((Selected)item).Name.IndexOf(NameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(SelectedDGHeader.Surname.ToString()))
                            {
                                containsFilter = ((Selected)item).Surname.IndexOf(SurnameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

        #region commands

        public ICommand StsCommand
        {
            get
            {
                if (this.stsCommand == null)
                {
                    this.stsCommand = new DelegateCommand<string>(this.ShowSts);
                }

                return this.stsCommand;
            }
        }

        private void ShowSts(string id)
        {
            this.PlayerSts = this.Players.Where(x => x.IdPlayer.Equals(id)).FirstOrDefault();
            if (PlayerSts != null)
            {
                this.Pts = this.PlayerSts.Pts;
                this.Assist = this.PlayerSts.Assist;
                this.Reb = this.PlayerSts.Reb;
            }
            else
            {
                this.PlayerSts = this.PlayersHome.Where(x => x.IdPlayer.Equals(id)).FirstOrDefault();
                this.Pts = this.PlayerSts.Pts;
                this.Assist = this.PlayerSts.Assist;
                this.Reb = this.PlayerSts.Reb;
            }

            this.StsPlayerName = this.PlayerSts.Name;
            this.StsWin = new StsWindow(this);
            this.StsWin.ShowDialog();
        }

        public DelegateCommand EditCommand
        {
            get
            {
                if (this.editCommand == null)
                {
                    this.editCommand = new DelegateCommand(this.Edit, this.CanExecuteEdit);
                }

                return this.editCommand;
            }
        }

        private bool CanExecuteEdit()
        {
            if (this.Pts == null || this.Pts < 0 || this.Assist == null || this.Assist < 0 || this.Reb == null || this.Reb < 0)
            {
                return false;
            }

            if (this.isPtsOver)
            {
                return false;
            }

            if (this.isAsOver)
            {
                return false;
            }

            return true;
        }

        private void Edit()
        {
            using (var db = new EuroleagueEntities3())
            {
                IgracIgra sts = db.IgracIgras.Where(x => x.Igrac_LICBR_IGR.Equals(this.PlayerSts.IdPlayer) && x.Utakmica_OZN_UTK.Equals(this.IdGame)).FirstOrDefault();

                sts.POENI_IGRACIGRA = (int)this.Pts;
                sts.AS_IGRACIGRA = (int)this.Assist;
                sts.SK_IGRACIGRA = (int)this.Reb;

                db.Entry(sts).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                Player p = this.Players.Where(x => x.IdPlayer.Equals(this.PlayerSts.IdPlayer)).FirstOrDefault();
                if (p != null)
                {
                    p.Pts = (int)this.Pts;
                    p.Assist = (int)this.Assist;
                    p.Reb = (int)this.Reb;
                }
                else
                {
                    p = this.PlayersHome.Where(x => x.IdPlayer.Equals(this.PlayerSts.IdPlayer)).FirstOrDefault();
                    p.Pts = (int)this.Pts;
                    p.Assist = (int)this.Assist;
                    p.Reb = (int)this.Reb;
                }
            }

            this.StsWin.Close();
        }

        public DelegateCommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                {
                    this.cancelCommand = new DelegateCommand(this.Cancel);
                }

                return this.cancelCommand;
            }
        }

        private void Cancel()
        {
            if (this.StsWin != null)
            {
                this.StsWin.Close();
            }
            else if (this.win != null)
            {
                this.win.Close();
            }
        }

        public DelegateCommand ApplyCommand
        {
            get
            {
                if (this.applyCommand == null)
                {
                    this.applyCommand = new DelegateCommand(this.Apply, this.CanApplyExecute);
                }

                return this.applyCommand;
            }
        }

        private bool CanApplyExecute()
        {
            return this.ListOfPlayers.Where(x => x.IsSelected == true).Count() < 13;
        }

        private void Apply()
        {
            if (this.applyClubId.Equals(this.IdTeam1))
            {
                this.AddOrRemoveSelectedFromHomeTeam();
            }
            else if (this.applyClubId.Equals(this.IdTeam2))
            {
                this.AddOrRemoveSelectedFromAwayTeam();
            }

            this.win.Close();
        }

        private void AddOrRemoveSelectedFromHomeTeam()
        {
            foreach (Selected selPlayer in ListOfPlayers)
            {
                if (selPlayer.IsSelected)
                {
                    if (this.PlayersHome.Where(x => x.IdPlayer.Equals(selPlayer.LICBR)).FirstOrDefault() == null)
                    {
                        this.PlayersHome.Add(new Player() { IdPlayer = selPlayer.LICBR, Name = selPlayer.Name + " " + selPlayer.Surname, IdClub = this.IdTeam1 });
                        AddToDbStsForPlayer(selPlayer);
                    }
                }
                else
                {
                    Player p = this.PlayersHome.Where(x => x.IdPlayer.Equals(selPlayer.LICBR)).FirstOrDefault();
                    if (p != null)
                    {
                        this.PlayersHome.Remove(p);
                        RemoveFromDbPreviousSelected(p);
                    }
                }
            }
        }

        private void AddOrRemoveSelectedFromAwayTeam()
        {
            foreach (Selected selPlayer in ListOfPlayers)
            {
                if (selPlayer.IsSelected)
                {
                    if (this.Players.Where(x => x.IdPlayer.Equals(selPlayer.LICBR)).FirstOrDefault() == null)
                    {
                        this.Players.Add(new Player() { IdPlayer = selPlayer.LICBR, Name = selPlayer.Name + " " + selPlayer.Surname, IdClub = this.IdTeam2 });
                        AddToDbStsForPlayer(selPlayer);
                    }
                }
                else
                {
                    Player p = this.Players.Where(x => x.IdPlayer.Equals(selPlayer.LICBR)).FirstOrDefault();
                    if (p != null)
                    {
                        this.Players.Remove(p);
                        RemoveFromDbPreviousSelected(p);
                    }
                }
            }
        }

        public ICommand AddRemovePlayerPlay
        {
            get
            {
                if (this.addRemovePlayerPlay == null)
                {
                    this.addRemovePlayerPlay = new DelegateCommand<string>(this.AddRemovePlayerPlayAction);
                }

                return this.addRemovePlayerPlay;
            }
        }

        private void AddRemovePlayerPlayAction(string clubId)
        {
            this.applyClubId = clubId;
            this.ListOfPlayers = new ObservableCollection<Selected>();

            using (var db = new EuroleagueEntities3())
            {
                var selectedPlayers = (from t1 in db.Igracs
                                       join t2 in db.Klubs on t1.Klub_ID_KLB equals t2.ID_KLB
                                       join t3 in db.IgracIgras on t1.LICBR_IGR equals t3.Igrac_LICBR_IGR
                                       where t3.Utakmica_OZN_UTK == this.IdGame && t2.ID_KLB == clubId
                                       select new { Igrac = t1 }).ToList();

                var allPlayers = (from t1 in db.Igracs
                                  join t2 in db.Klubs on t1.Klub_ID_KLB equals t2.ID_KLB
                                  where t2.ID_KLB == clubId
                                  select new { Igrac = t1 }).ToList();

                foreach (var player in selectedPlayers)
                {
                    Selected sel = new Selected()
                    {
                        Name = player.Igrac.IME_IGR,
                        Surname = player.Igrac.PRZ_IGR,
                        LICBR = player.Igrac.LICBR_IGR,
                        IsSelected = true
                    };
                    sel.PropertyChanged += new PropertyChangedEventHandler(SelectedPlayersPropertyChanged);
                    this.ListOfPlayers.Add(sel);
                   
                }

                foreach (var player in allPlayers)
                {
                    if (!selectedPlayers.Any(x => x.Igrac.LICBR_IGR.Equals(player.Igrac.LICBR_IGR)))
                    {
                        Selected sel = new Selected()
                        {
                            Name = player.Igrac.IME_IGR,
                            Surname = player.Igrac.PRZ_IGR,
                            LICBR = player.Igrac.LICBR_IGR,
                            IsSelected = false
                        };
                        sel.PropertyChanged += new PropertyChangedEventHandler(SelectedPlayersPropertyChanged);
                        this.ListOfPlayers.Add(sel);
                        
                    }
                }
            }

            this.SelPlayers = CollectionViewSource.GetDefaultView(this.ListOfPlayers);
            this.win = new ListOfPlayersWindow(this);
            this.win.ShowDialog();
        }

        public void SelectedPlayersPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ApplyCommand.RaiseCanExecuteChanged();
            this.RaisePropertyChanged("ListOfPlayers");
        }

        private void RemoveFromDbPreviousSelected(Player p)
        {
            using (var db = new EuroleagueEntities3())
            {
                IgracIgra player = db.IgracIgras.Where(x => x.Igrac_LICBR_IGR.Equals(p.IdPlayer)).FirstOrDefault();
                db.Entry(player).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
        }

        private void AddToDbStsForPlayer(Selected selPlayer)
        {
            using (var db = new EuroleagueEntities3())
            {
                db.IgracIgras.Add(new IgracIgra()
                {
                    Igrac_LICBR_IGR = selPlayer.LICBR,
                    Utakmica_OZN_UTK = this.IdGame,
                });

                db.SaveChanges();
            }
        }

        #endregion

        #region data error

        private string error = string.Empty;
        private bool isPtsOver = false;
        private bool isAsOver = false;
        public string Error
        {
            get { return error; }
        }
        
        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName.Equals("ListOfPlayers"))
                {
                    if (this.ListOfPlayers.Where(x => x.IsSelected == true).Count() > 12)
                    {
                        error = "You can check only 12 players per game!";
                    }
                }
                if (columnName.Equals("Pts"))
                {
                    if (this.Pts == null || this.Pts < 0)
                    {
                        error = "Pts must be positive number!";
                    }
                    else if (!this.Client.CheckPtsConstraints((int)this.Pts, this.PlayerSts.IdPlayer, IdGame, this.PlayerSts.IdClub))
                    {
                        error = "Points of players must be less than team made!";
                        this.isPtsOver = true;
                    }
                    else
                    {
                        this.isPtsOver = false;
                    }
                }
                if (columnName.Equals("Assist"))
                {
                    if (this.Assist == null || this.Assist < 0)
                    {
                        error = "Assist must be positive number!";
                    }
                    else if (!this.Client.CheckAsConstraints((int)this.Assist, this.PlayerSts.IdPlayer, IdGame, this.PlayerSts.IdClub))
                    {
                        error = "Assist of players must be less than points team made divide by 2!";
                        this.isAsOver = true;
                    }
                    else
                    {
                        this.isAsOver = false;
                    }
                }
                if (columnName.Equals("Reb"))
                {
                    if (this.Reb == null || this.Reb < 0)
                    {
                        error = "Reb must be positive number!";
                    }
                }

                this.ApplyCommand.RaiseCanExecuteChanged();
                this.EditCommand.RaiseCanExecuteChanged();

                return error;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                if (propName.Equals("Pts") || propName.Equals("Assist") || propName.Equals("Reb"))
                {
                    this.EditCommand.RaiseCanExecuteChanged();
                }

                this.ApplyCommand.RaiseCanExecuteChanged();
            }
        }

    }
}

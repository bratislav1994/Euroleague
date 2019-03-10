using Prism.Commands;
using Project.Games;
using Project.HelperClasses;
using Project.Login;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Schedules
{
    public class ScheduleVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private int roundCmb;
        private Schedule selectedGame = null;
        private GameWindow gameWin = null;
        private AddEditReservationWindow reservationWin;
        private DelegateCommand showDetail;
        private GameVM gameVM = null;
        private Roles role;
        private Visibility changeResultVisibility;
        private Visibility addRefereesVisibility;
        private Visibility labelVisibility;
        private Visibility conferenceVisibility;
        private Visibility clickAddReservationVisibility;
        private DelegateCommand clickAddReservationCommand;
        private Visibility clickAddRefereesVisibility;
        private DelegateCommand clickChangeResultCommand;
        private DelegateCommand clickAddRefereesCommand;
        private DelegateCommand addRefereesCommand;
        private DelegateCommand changeResultCommand;
        private DelegateCommand cancelCommand;
        private DelegateCommand applyReservationCommand;
        private bool conference = false;
        private bool chBoxEnabled = false;
        private ScheduleModel model;
        private ResultWindow resWin;
        private AddRefereeWindow addRefWin;
        private string refereesAndReservations;
        private Visibility txbVisibility = Visibility.Hidden;
        private int oldPtsHome = -1;
        private int oldPtsAway = -1;
        private Hala hall = null;
        private string homeTeam;
        private string startDate;
        private string endDate;
        private ObjectParameter objPar = new ObjectParameter("isOk", "");
        private bool isModify = false;

        public ScheduleVM(Roles role)
        {
            this.Model = new ScheduleModel();
            this.role = role;
            this.InitializeView();
        }

        #region properties

        public int RoundCmb
        {
            get
            {
                return this.roundCmb;
            }

            set
            {
                this.roundCmb = value;
                this.RaisePropertyChanged("RoundCmb");
            }
        }

        public Schedule SelectedGame
        {
            get
            {
                return selectedGame;
            }

            set
            {
                selectedGame = value;
                this.RaisePropertyChanged("SelectedGame");
            }
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

        public Roles Role
        {
            get
            {
                return role;
            }

            set
            {
                role = value;
            }
        }

        public Visibility ChangeResultVisibility
        {
            get
            {
                return changeResultVisibility;
            }

            set
            {
                changeResultVisibility = value;
            }
        }

        public Visibility AddRefereesVisibility
        {
            get
            {
                return addRefereesVisibility;
            }

            set
            {
                addRefereesVisibility = value;
            }
        }

        public Visibility LabelVisibility
        {
            get
            {
                return labelVisibility;
            }

            set
            {
                labelVisibility = value;
            }
        }

        public Visibility ConferenceVisibility
        {
            get
            {
                return conferenceVisibility;
            }

            set
            {
                conferenceVisibility = value;
            }
        }

        public Visibility ClickAddRefereesVisibility
        {
            get
            {
                return clickAddRefereesVisibility;
            }

            set
            {
                clickAddRefereesVisibility = value;
            }
        }

        public bool Conference
        {
            get
            {
                return conference;
            }

            set
            {
                conference = value;
                this.RaisePropertyChanged("Conference");
            }
        }

        public bool ChBoxEnabled
        {
            get
            {
                return chBoxEnabled;
            }

            set
            {
                chBoxEnabled = value;
                this.RaisePropertyChanged("ChBoxEnabled");
            }
        }

        public ScheduleModel Model
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

        public string RefereesAndReservations
        {
            get
            {
                return refereesAndReservations;
            }

            set
            {
                refereesAndReservations = value;
                this.RaisePropertyChanged("RefereesAndReservations");
            }
        }

        public Visibility TxbVisibility
        {
            get
            {
                return txbVisibility;
            }

            set
            {
                txbVisibility = value;
                this.RaisePropertyChanged("TxbVisibility");
            }
        }

        public Visibility ClickAddReservationVisibility
        {
            get
            {
                return clickAddReservationVisibility;
            }

            set
            {
                clickAddReservationVisibility = value;
                this.RaisePropertyChanged("ClickAddReservationVisibility");
            }
        }

        public Hala Hall
        {
            get
            {
                return this.hall;
            }

            set
            {
                this.hall = value;
                this.RaisePropertyChanged("Hall");
            }
        }

        public string HomeTeam
        {
            get
            {
                return homeTeam;
            }

            set
            {
                homeTeam = value;
                this.RaisePropertyChanged("HomeTeam");
            }
        }

        public string StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.startDate = value;
                this.RaisePropertyChanged("StartDate");
            }
        }

        public string EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.endDate = value;
                this.RaisePropertyChanged("EndDate");
            }
        }

        #endregion

        private void GetRefereesAndReservations()
        {
            using (var db = new EuroleagueEntities3())
            {
                var selectedReferees = (from t1 in db.Sudijas
                                        join t2 in db.Sudis on t1.LICBR_SUD equals t2.Sudija_LICBR_SUD
                                        where t2.Utakmica_OZN_UTK == this.SelectedGame.IdGame
                                        select new { Sudija = t1 }).ToList();

                this.RefereesAndReservations = "Referees:\n";
                foreach (var s in selectedReferees)
                {
                    this.RefereesAndReservations += s.Sudija.IME_SUD + " " + s.Sudija.PRZ_SUD + " (" + s.Sudija.DRZ_SUD + ")\n";
                }

                this.RefereesAndReservations += "\nHall:\n";
                Rezervacija r = this.Model.GetHallById(this.SelectedGame.IdGame);
                if (r != null)
                {
                    this.RefereesAndReservations += r.Hala.NAZ_HALA;
                }
            }
        }

        private void ResetFields()
        {

        }

        #region cancel command

        public DelegateCommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                {
                    this.cancelCommand = new DelegateCommand(this.CloseWindow);
                }

                return this.cancelCommand;
            }
        }

        protected virtual void CloseWindow()
        {
            if (this.resWin != null)
            {
                this.resWin.Close();
            }
            if (this.addRefWin != null)
            {
                this.addRefWin.Close();
            }
            if (this.reservationWin != null)
            {
                this.reservationWin.Close();
            }

            if (oldPtsHome != -1 && oldPtsAway != -1)
            {
                this.GetOldPtsWhenCancelChange();
            }
           // this.SelectedGame = null;
            // this.ResetFields();
        }

        #endregion

        #region click change result command

        public DelegateCommand ClickChangeResultCommand
        {
            get
            {
                if (this.clickChangeResultCommand == null)
                {
                    this.clickChangeResultCommand = new DelegateCommand(this.ClickChangeResult, this.CanExecute);
                }

                return this.clickChangeResultCommand;
            }
        }

        private void ClickChangeResult()
        {
            this.resWin = new ResultWindow(this);
            this.resWin.ShowDialog();
            if (this.SelectedGame != null && oldPtsHome != -1 && oldPtsAway != -1)
            {
                this.GetOldPtsWhenCancelChange();
            }
           // this.SelectedGame = null;
        }

        #endregion

        private void GetOldPtsWhenCancelChange()
        {
            this.SelectedGame.PointsTeam1 = oldPtsHome;
            this.SelectedGame.PointsTeam2 = oldPtsAway;
        }

        #region change result command

        public DelegateCommand ChangeResultCommand
        {
            get
            {
                if (this.changeResultCommand == null)
                {
                    this.changeResultCommand = new DelegateCommand(this.ChangeResult, this.CanChange);
                }

                return this.changeResultCommand;
            }
        }

        void SelectedGamePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.ChangeResultCommand.RaiseCanExecuteChanged();
        }

        private bool CanChange()
        {
            return this.SelectedGame != null && this.SelectedGame.PointsTeam1 != null &&
                   this.SelectedGame.PointsTeam1 > -1 && this.SelectedGame.PointsTeam2 != null &&
                   this.SelectedGame.PointsTeam2 > -1 && this.SelectedGame.PointsTeam1 != this.SelectedGame.PointsTeam2;
        }

        private void ChangeResult()
        {
            this.Model.ChangeGameResult(this.SelectedGame);
            this.oldPtsHome = -1;
            this.oldPtsAway = -1;
            this.resWin.Close();
        }

        #endregion

        #region reservation commands

        public DelegateCommand ClickAddReservationCommand
        {
            get
            {
                if (this.clickAddReservationCommand == null)
                {
                    this.clickAddReservationCommand = new DelegateCommand(this.ClickAddReservation, this.CanExecute);
                }

                return this.clickAddReservationCommand;
            }
        }

        private void ResetFocusFields()
        {
            this.isHallFocus = false;
            this.isEndFocus = false;
            this.isStartFocus = false;
            this.isStartOk = false;
            this.isEndOk = false;
        }

        private void ClickAddReservation()
        {
            this.ResetFocusFields();
            this.Model.Halls = this.Model.GetAllHalls();
            Rezervacija r = this.Model.CheckIfReservationExists(this.SelectedGame.IdGame);
            this.isModify = r != null ? true : false;

            if (r != null)
            {
                this.StartDate = r.PVRMREZ_HALA.ToString();
                this.EndDate = r.KVRMREZ_HALA.ToString();
                this.Hall = this.Model.Halls.Where(x => x.OZN_HALA.Equals(r.Hala_OZN_HALA)).FirstOrDefault();
            }

            this.HomeTeam = this.Model.GetClubById(this.SelectedGame.IdTeam1);
            this.reservationWin = new AddEditReservationWindow(this);
            this.reservationWin.ShowDialog();
           // this.SelectedGame = null;
        }

        public DelegateCommand ApplyReservationCommand
        {
            get
            {
                if (this.applyReservationCommand == null)
                {
                    this.applyReservationCommand = new DelegateCommand(this.ApplyReservation, this.CanExecuteReservation);
                }

                return this.applyReservationCommand;
            }
        }

        private bool CanExecuteReservation()
        {
            if (string.IsNullOrEmpty(this.StartDate) || string.IsNullOrEmpty(this.EndDate) || this.Hall == null)
            {
                return false;
            }

            using (var db = new EuroleagueEntities3())
            {
                if (this.isModify && this.SelectedGame != null)
                {
                    db.DateValidationReservationModify(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.SelectedGame.IdGame, this.Hall.OZN_HALA, objPar);
                }
                else
                {
                    db.DateValidationReservation(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.Hall.OZN_HALA, objPar);
                }

                return objPar.Value.Equals("ok") ? true : false;
            }
        }

        private void ApplyReservation()
        {
            if (this.isModify)
            {
                using (var db = new EuroleagueEntities3())
                {
                    Rezervacija r = db.Rezervacijas.Where(x => x.SFR_REZ.Equals(this.SelectedGame.IdGame)).FirstOrDefault();
                    if (r != null)
                    {
                        r.PVRMREZ_HALA = DateTime.Parse(this.StartDate);
                        r.KVRMREZ_HALA = DateTime.Parse(this.EndDate);
                        db.Entry(r).State = System.Data.Entity.EntityState.Deleted;
                        db.SaveChanges();
                        r.Hala_OZN_HALA = this.Hall.OZN_HALA;
                        
                        db.Rezervacijas.Add(r);
                        // db.Entry(r).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                using (var db = new EuroleagueEntities3())
                {
                    Rezervacija r = new Rezervacija()
                    {
                        Hala_OZN_HALA = this.Hall.OZN_HALA,
                        Klub_ID_KLB = this.SelectedGame.IdTeam1,
                        SFR_REZ = this.SelectedGame.IdGame,
                        PVRMREZ_HALA = DateTime.Parse(this.StartDate),
                        KVRMREZ_HALA = DateTime.Parse(this.EndDate)
                    };
                    db.Rezervacijas.Add(r);
                    db.SaveChanges();
                }
            }


            this.reservationWin.Close();
            this.ResetFocusFields();
            this.GetRefereesAndReservations();
        }

        #endregion

        #region click add referees command

        public DelegateCommand ClickAddRefereesCommand
        {
            get
            {
                if (this.clickAddRefereesCommand == null)
                {
                    this.clickAddRefereesCommand = new DelegateCommand(this.ClickAddReferee, this.CanExecute);
                }

                return this.clickAddRefereesCommand;
            }
        }

        private void ClickAddReferee()
        {
            this.Model.ListOfReferees = new ObservableCollection<Selected>();

            using (var db = new EuroleagueEntities3())
            {
                var selectedReferees = (from t1 in db.Sudijas
                                        join t2 in db.Sudis on t1.LICBR_SUD equals t2.Sudija_LICBR_SUD
                                        where t2.Utakmica_OZN_UTK == this.SelectedGame.IdGame
                                        select new { Sudija = t1 }).ToList();

                foreach (var referee in selectedReferees)
                {
                    Selected sel = new Selected()
                    {
                        Name = referee.Sudija.IME_SUD,
                        Surname = referee.Sudija.PRZ_SUD,
                        LICBR = referee.Sudija.LICBR_SUD,
                        IsSelected = true
                    };
                    this.Model.ListOfReferees.Add(sel);
                    sel.PropertyChanged += new PropertyChangedEventHandler(this.SelectedRefereesPropertyChanged);

                }

                List<Sudija> allReferees = db.Sudijas.ToList();

                foreach (var referee in allReferees)
                {
                    if (!selectedReferees.Any(x => x.Sudija.LICBR_SUD.Equals(referee.LICBR_SUD)))
                    {
                        Selected sel = new Selected()
                        {
                            Name = referee.IME_SUD,
                            Surname = referee.PRZ_SUD,
                            LICBR = referee.LICBR_SUD,
                            IsSelected = false
                        };
                        this.Model.ListOfReferees.Add(sel);
                        sel.PropertyChanged += new PropertyChangedEventHandler(this.SelectedRefereesPropertyChanged);
                    }
                }
            }

            this.Model.Referees = CollectionViewSource.GetDefaultView(this.Model.ListOfReferees);
            this.addRefWin = new AddRefereeWindow(this);
            this.addRefWin.ShowDialog();
           // this.SelectedGame = null;
        }

        void SelectedRefereesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.AddRefereesCommand.RaiseCanExecuteChanged();
            this.Model.RaisePropertyChanged("Referees");
            this.AddRefereesCommand.RaiseCanExecuteChanged();
        }

        #endregion

        #region add referees ref

        public DelegateCommand AddRefereesCommand
        {
            get
            {
                //  if (this.addRefereesCommand == null)
                // {
                return this.addRefereesCommand ?? (this.addRefereesCommand = new DelegateCommand(this.ChangeReferees, this.CanChangeReferees));
                //  return this.addRefereesCommand;
            }
        }

        private bool CanChangeReferees()
        {
            return this.Model.ListOfReferees.Count > 0 && this.Model.ListOfReferees.Where(x => x.IsSelected == true).Count() < 4;
        }

        private void ChangeReferees()
        {
            this.Model.AddOrRemoveRefereesFromGame(this.Model.ListOfReferees, this.SelectedGame.IdGame);
            this.addRefWin.Close();
            this.GetRefereesAndReservations();
        }

        #endregion

        #region show detail

        public DelegateCommand ShowDetail
        {
            get
            {
                if (this.showDetail == null)
                {
                    this.showDetail = new DelegateCommand(this.ShowGameDetail, this.CanExecute);
                }

                return this.showDetail;
            }
        }

        private bool CanExecute()
        {
            return this.SelectedGame != null;
        }

        protected virtual void ShowGameDetail()
        {
            if (this.Role == Roles.Admin)
            {
                this.GameVM = new AdminGameVM(this.SelectedGame);
            }
            else if (this.Role == Roles.User)
            {
                this.GameVM = new UserGameVM(this.SelectedGame);
            }

            this.gameWin = new GameWindow(this.GameVM);
            this.gameWin.ShowDialog();
           // this.SelectedGame = null;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region init
        public void InitializeView()
        {
            if (this.Role == Roles.User)
            {
                this.AddRefereesVisibility = Visibility.Hidden;
                this.ChangeResultVisibility = Visibility.Hidden;
                this.ConferenceVisibility = Visibility.Hidden;
                this.LabelVisibility = Visibility.Hidden;
                this.ClickAddRefereesVisibility = Visibility.Hidden;
                this.ClickAddReservationVisibility = Visibility.Hidden;
            }

            this.Model.GetGamesForSelectedRound(0);
        }

        #endregion

        #region data error

        private ICommand lostFocusCommand;

        public ICommand LostFocusCommand
        {
            get { return lostFocusCommand ?? (lostFocusCommand = new DelegateCommand<string>(ChangeFocusName)); }
        }

        private void ChangeFocusName(string element)
        {
            if (element.Equals("Hall"))
            {
                this.isHallFocus = true;
                RaisePropertyChanged("Hall");
            }
            else if (element.Equals("Start"))
            {
                this.isStartFocus = true;
                RaisePropertyChanged("StartDate");
            }
            else if (element.Equals("End"))
            {
                this.isEndFocus = true;
                RaisePropertyChanged("EndDate");
            }
        }

        private bool isHallFocus = false;
        private bool isStartFocus = false;
        private bool isEndFocus = false;
        private bool isStartOk = false;
        private bool isEndOk = false;

        private string error = string.Empty;
        public string Error
        {
            get { return error; }
        }

        public string this[string columnName]
        {
            get
            {
                error = string.Empty;
                if (columnName.Equals("ListOfReferees"))
                {
                    if (this.Model.ListOfReferees.Where(x => x.IsSelected == true).Count() > 3)
                    {
                        error = "You can check only 3 referees per game!";
                    }
                }
                else if (columnName.Equals("Hall") && this.Hall == null && isHallFocus)
                {
                    error = "Hall must be selected!";
                }
                else if (columnName.Equals("StartDate") && isStartFocus)
                {
                    using (var db = new EuroleagueEntities3())
                    {
                        if (string.IsNullOrEmpty(this.StartDate))
                        {
                            error = "Start date must be selected!";
                        }
                        else if (!string.IsNullOrEmpty(this.EndDate) && this.Hall != null)
                        {
                            if (this.Model.DateValid(this.StartDate) || this.Model.DateValid(this.EndDate))
                            {
                                if (this.isModify && this.SelectedGame != null)
                                {
                                    db.DateValidationReservationModify(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.SelectedGame.IdGame, this.Hall.OZN_HALA, objPar);
                                }
                                else
                                {
                                    db.DateValidationReservation(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.Hall.OZN_HALA, objPar);
                                }

                                if (!objPar.Value.Equals("ok"))
                                {
                                    error = objPar.Value.ToString();
                                    this.isStartOk = false;
                                }
                                else
                                {
                                    this.isStartOk = true;
                                    if (!this.isEndOk)
                                    {
                                        this.EndDate = this.EndDate;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (columnName == "EndDate" && isEndFocus)
                {
                    using (var db = new EuroleagueEntities3())
                    {
                        if (string.IsNullOrEmpty(this.EndDate))
                        {
                            error = "End date must be selected!";
                        }
                        else if (!string.IsNullOrEmpty(this.StartDate) && this.Hall != null)
                        {
                            if (this.Model.DateValid(this.StartDate) || this.Model.DateValid(this.EndDate))
                            {
                                if (this.isModify && this.SelectedGame != null)
                                {
                                    db.DateValidationReservationModify(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.SelectedGame.IdGame, this.Hall.OZN_HALA, objPar);
                                }
                                else
                                {
                                    db.DateValidationReservation(DateTime.Parse(this.StartDate), DateTime.Parse(this.EndDate), this.Hall.OZN_HALA, objPar);
                                }

                                if (!objPar.Value.Equals("ok"))
                                {
                                    error = objPar.Value.ToString();
                                    this.isEndOk = false;
                                }
                                else
                                {
                                    this.isEndOk = true;
                                    if (!this.isStartOk)
                                    {
                                        this.StartDate = this.StartDate;
                                    }
                                }
                            }
                        }
                    }
                }

                this.AddRefereesCommand.RaiseCanExecuteChanged();
                this.ClickAddReservationCommand.RaiseCanExecuteChanged();
                this.ApplyReservationCommand.RaiseCanExecuteChanged();

                return error;
            }
        }

        #endregion

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                if (propName.Equals("RoundCmb"))
                {
                    this.Model.GetGamesForSelectedRound(this.RoundCmb);
                }
                else if (propName.Equals("SelectedGame"))
                {
                    if (this.SelectedGame != null)
                    {
                        this.SelectedGame.PropertyChanged += new PropertyChangedEventHandler(SelectedGamePropertyChanged);
                        this.TxbVisibility = Visibility.Visible;
                        this.GetRefereesAndReservations();
                        oldPtsHome = (int)this.SelectedGame.PointsTeam1;
                        oldPtsAway = (int)this.SelectedGame.PointsTeam2;

                        using (var db = new EuroleagueEntities3())
                        {
                            Utakmica u = db.Utakmicas.Where(x => x.OZN_UTK.Equals(this.SelectedGame.IdGame)).FirstOrDefault();

                            if (u != null)
                            {
                                this.Conference = u.KONFMDJ_UTK == true ? true : false;
                            }
                        }

                        this.ChBoxEnabled = true;
                    }
                    else
                    {
                        this.ChBoxEnabled = false;
                        this.TxbVisibility = Visibility.Hidden;
                    }

                    this.ShowDetail.RaiseCanExecuteChanged();
                    this.ClickAddRefereesCommand.RaiseCanExecuteChanged();
                    this.ClickChangeResultCommand.RaiseCanExecuteChanged();
                    this.ClickAddReservationCommand.RaiseCanExecuteChanged();
                }
                else if (propName.Equals("Conference"))
                {
                    this.Model.ChangeConference(this.SelectedGame.IdGame, this.Conference);
                }
            }
        }
    }
}

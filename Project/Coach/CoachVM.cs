using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Coach
{
    public class CoachVM : DataGrid, INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string name;
        private string surname;
        private Klub club;
        private string startDate;
        private string endDate;
        private Visibility addButtonVisibility = Visibility.Hidden;
        private Visibility editButtonVisibility = Visibility.Hidden;
        private Visibility addOrEditCoachCanvasVis = Visibility.Hidden;
        private DelegateCommand clickAddCoachCommand;
        private DelegateCommand editCoachCommand;
        private DelegateCommand removeCoachCommand;
        private DelegateCommand addCoachCommand;
        private DelegateCommand clickEditCoachCommand;
        private CoachModel model = null;
        private ICollectionView coaches;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;
        private string clubFilter = string.Empty;
        private Trener selectedCoach = null;

        public CoachVM()
        {
            this.model = new CoachModel();
            this.Model.CoachesList = Model.GetAllCoaches();
            this.Coaches = new CollectionViewSource { Source = this.Model.CoachesList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Coaches = CollectionViewSource.GetDefaultView(this.Model.CoachesList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[CoachHeaderDG.IME_TRN.ToString()] = string.Empty;
            columnFilters[CoachHeaderDG.PRZ_TRN.ToString()] = string.Empty;
            columnFilters[CoachHeaderDG.Klub.ToString()] = string.Empty;
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
                columnFilters[CoachHeaderDG.IME_TRN.ToString()] = this.nameFilter;
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
                columnFilters[CoachHeaderDG.PRZ_TRN.ToString()] = this.surnameFilter;
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
                columnFilters[CoachHeaderDG.Klub.ToString()] = this.clubFilter;
                this.OnFilterApply();
            }
        }

        public ICollectionView Coaches
        {
            get
            {
                return this.coaches;
            }

            set
            {
                this.coaches = value;
                RaisePropertyChanged("Coaches");
            }
        }

        public void OnFilterApply()
        {
           this.Coaches = CollectionViewSource.GetDefaultView(this.Model.CoachesList);
            if (this.Coaches != null)
            {
                this.Coaches.Filter = delegate (object item)
                {
                    bool show = true;
                    
                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(CoachHeaderDG.IME_TRN.ToString()))
                            {
                                containsFilter = ((Trener)item).IME_TRN.IndexOf(NameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(CoachHeaderDG.PRZ_TRN.ToString()))
                            {
                                containsFilter = ((Trener)item).PRZ_TRN.IndexOf(SurnameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(CoachHeaderDG.Klub.ToString()))
                            {
                                containsFilter = ((Trener)item).Klub.NAZ_KLB.IndexOf(ClubFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

        public string NameCoach
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                this.RaisePropertyChanged("NameCoach");
            }
        }

        public string Surname
        {
            get
            {
                return surname;
            }

            set
            {
                surname = value;
                this.RaisePropertyChanged("Surname");
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

        public Trener SelectedCoach
        {
            get
            {
                return selectedCoach;
            }

            set
            {
                selectedCoach = value;
                this.RaisePropertyChanged("SelectedCoach");
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

        public Visibility AddButtonVisibility
        {
            get
            {
                return addButtonVisibility;
            }

            set
            {
                addButtonVisibility = value;
                this.RaisePropertyChanged("AddButtonVisibility");
            }
        }

        public Visibility EditButtonVisibility
        {
            get
            {
                return editButtonVisibility;
            }

            set
            {
                editButtonVisibility = value;
                this.RaisePropertyChanged("EditButtonVisibility");
            }
        }

        public Visibility AddOrEditCoachCanvasVis
        {
            get
            {
                return addOrEditCoachCanvasVis;
            }

            set
            {
                addOrEditCoachCanvasVis = value;
                this.RaisePropertyChanged("AddOrEditCoachCanvasVis");
            }
        }

        public CoachModel Model
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

        private void ResetFields()
        {
            this.Surname = string.Empty;
            this.Club = null;
            this.NameCoach = string.Empty;
        }

        #region click add edit/remove command

        public DelegateCommand ClickAddCoachCommand
        {
            get
            {
                if (this.clickAddCoachCommand == null)
                {
                    this.clickAddCoachCommand = new DelegateCommand(this.ClickAddCoach);
                }

                return this.clickAddCoachCommand;
            }
        }

        private void ClickAddCoach()
        {
            this.ResetFocusFields();
            this.AddOrEditCoachCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Visible;
            this.EditButtonVisibility = Visibility.Hidden;
            this.ResetFields();
        }

        private void ResetFocusFields()
        {
            this.isClubFocus = false;
            this.isEndFocus = false;
            this.isNameFocus = false;
            this.isStartFocus = false;
            this.isSurnameFocus = false;
        }

        #endregion

        #region click edit command

        public DelegateCommand ClickEditCoachCommand
        {
            get
            {
                if (this.clickEditCoachCommand == null)
                {
                    this.clickEditCoachCommand = new DelegateCommand(this.ClickEdit, this.IsSelected);
                }

                return this.clickEditCoachCommand;
            }
        }

        private void ClickEdit()
        {
            this.ResetFocusFields();
            this.AddOrEditCoachCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Hidden;
            this.EditButtonVisibility = Visibility.Visible;
            this.Model.Clubs = this.Model.GetAllClubs();

            using (var db = new EuroleagueEntities3())
            {
                Trener t = db.Treners.Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault();
                this.NameCoach = t.IME_TRN;
                this.Surname = t.PRZ_TRN;
                this.Club = this.Model.GetClubById(t.Klub_ID_KLB);
                this.StartDate = t.PDATUG_TRN.ToString();
                this.EndDate = t.KDATUG_TRN.ToString();
            }
        }

        #endregion

        #region edit command

        public DelegateCommand EditCoachCommand
        {
            get
            {
                if (this.editCoachCommand == null)
                {
                    this.editCoachCommand = new DelegateCommand(this.EditCoach, this.CanAddCoach);
                }

                return this.editCoachCommand;
            }
        }

        private void EditCoach()
        {
            using (var db = new EuroleagueEntities3())
            {
                Trener t = db.Treners.Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault();
                t.IME_TRN = this.NameCoach;
                t.PRZ_TRN = this.Surname;
                t.Klub_ID_KLB = this.Club.ID_KLB;
                t.PDATUG_TRN = DateTime.Parse(this.StartDate);
                t.KDATUG_TRN = DateTime.Parse(EndDate);
                
                db.Entry(t).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //t = this.Coaches.SourceCollection.Cast<Trener>().Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault();
                t = this.Model.CoachesList.Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault();
                t.IME_TRN = this.NameCoach;
                t.PRZ_TRN = this.Surname;
                t.Klub = this.Club;
                t.Klub_ID_KLB = this.Club.ID_KLB;
                t.PDATUG_TRN = DateTime.Parse(this.StartDate);
                t.KDATUG_TRN = DateTime.Parse(EndDate);

                this.Coaches.Refresh();
            }

            this.AddOrEditCoachCanvasVis = Visibility.Hidden;
            this.ResetFields();
        }

        #endregion

        #region remove command

        public DelegateCommand RemoveCoachCommand
        {
            get
            {
                if (this.removeCoachCommand == null)
                {
                    this.removeCoachCommand = new DelegateCommand(this.RemoveCoach, this.IsSelected);
                }

                return this.removeCoachCommand;
            }
        }

        private void RemoveCoach()
        {
            using (var db = new EuroleagueEntities3())
            {
                Trener t = db.Treners.Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault();
                db.Entry(t).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                this.Model.CoachesList.Remove(this.Model.CoachesList.Where(x => x.LICBR_TRN.Equals(this.SelectedCoach.LICBR_TRN)).FirstOrDefault());
            }

            this.SelectedCoach = null;
        }

        private bool IsSelected()
        {
            return this.SelectedCoach != null;
        }

        #endregion

        #region add arena command

        public DelegateCommand AddCoachCommand
        {
            get
            {
                if (this.addCoachCommand == null)
                {
                    this.addCoachCommand = new DelegateCommand(this.AddCoach, this.CanAddCoach);
                }

                return this.addCoachCommand;
            }
        }

        private bool CanAddCoach()
        {
            if (string.IsNullOrEmpty(this.NameCoach) || string.IsNullOrEmpty(this.Surname) ||
                this.Club == null || string.IsNullOrEmpty(this.StartDate) || string.IsNullOrEmpty(this.EndDate))
            {
                return false;
            }

            if (!this.Model.DateValid(this.StartDate) || !this.Model.DateValid(this.EndDate))
            {
                return false;
            }

            if (!this.Model.IsStartBeforeEnd(this.StartDate, this.EndDate))
            {
                return false;
            }

            return true;
        }

        private void AddCoach()
        {
            using (var db = new EuroleagueEntities3())
            {
                Trener t = new Trener()
                {
                    IME_TRN = this.NameCoach,
                    PRZ_TRN = this.Surname,
                    KDATUG_TRN = DateTime.Parse(this.EndDate),
                    Klub_ID_KLB = this.Club.ID_KLB,
                    PDATUG_TRN = DateTime.Parse(this.StartDate),
                    LICBR_TRN = Guid.NewGuid().ToString().Substring(0, 15)
                };

                db.Treners.Add(t);
                db.SaveChanges();

                this.Model.CoachesList.Add(new Trener()
                {
                    IME_TRN = t.IME_TRN,
                    PRZ_TRN = t.PRZ_TRN,
                    KDATUG_TRN = t.KDATUG_TRN,
                    Klub_ID_KLB = t.Klub_ID_KLB,
                    Klub = this.Model.GetClubById(t.Klub_ID_KLB),
                    PDATUG_TRN = t.PDATUG_TRN,
                    LICBR_TRN = t.LICBR_TRN
                });
            }

            this.AddOrEditCoachCanvasVis = Visibility.Hidden;
            this.ResetFields();
        }

        #endregion

        #region IDataErrorInfo


        private ICommand lostFocusCommand;

        public ICommand LostFocusCommand
        {
            get { return lostFocusCommand ?? (lostFocusCommand = new DelegateCommand<string>(ChangeFocusName)); }
        }

        private void ChangeFocusName(string element)
        {
            if (element.Equals("Name"))
            {
                this.isNameFocus = true;
                RaisePropertyChanged(this.NameCoach);
            }
            else if (element.Equals("Surname"))
            {
                this.isSurnameFocus = true;
                RaisePropertyChanged(this.Surname);
            }
            else if (element.Equals("Club"))
            {
                this.isClubFocus = true;
                RaisePropertyChanged("Club");
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

        private bool isNameFocus = false;
        private bool isSurnameFocus = false;
        private bool isClubFocus = false;
        private bool isStartFocus = false;
        private bool isEndFocus = false;

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
                if (columnName.Equals("Name") && string.IsNullOrWhiteSpace(this.NameCoach) && this.isNameFocus)
                {
                    error = "Name is required!";
                }
                else if (columnName.Equals("Surname") && string.IsNullOrWhiteSpace(this.Surname) && this.isSurnameFocus)
                {
                    error = "Surname is required!";
                }
                else if (columnName.Equals("Club") && this.Club == null && isClubFocus)
                {
                    error = "Club must be selected!";
                }
                else if (columnName.Equals("StartDate") && isStartFocus)
                {
                    if (!this.Model.DateValid(this.StartDate))
                    {
                        error = "Start date isn't valid!";
                    }
                    else
                    {
                        if (isEndFocus && this.Model.DateValid(this.EndDate))
                        {
                            if (!this.Model.IsStartBeforeEnd(this.StartDate, this.EndDate))
                            {
                                error = "Start date must be before end date!";
                            }
                        }
                    }
                }
                else if (columnName == "EndDate" && isEndFocus)
                {
                    if (!this.Model.DateValid(this.EndDate))
                    {
                        error = "End date isn't valid!";
                    }
                    else
                    {
                        if (isStartFocus && this.Model.DateValid(this.StartDate))
                        {
                            if (!this.Model.IsStartBeforeEnd(this.StartDate, this.EndDate))
                            {
                                error = "End date must be after start date!";
                            }
                        }
                    }
                }

                this.AddCoachCommand.RaiseCanExecuteChanged();
                this.EditCoachCommand.RaiseCanExecuteChanged();

                return error;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                if (propName.Equals("SelectedCoach"))
                {
                    this.ClickEditCoachCommand.RaiseCanExecuteChanged();
                    this.RemoveCoachCommand.RaiseCanExecuteChanged();

                    if (this.SelectedCoach != null)
                    {
                        this.AddOrEditCoachCanvasVis = Visibility.Hidden;
                    }
                }
            }
        }
    }
}

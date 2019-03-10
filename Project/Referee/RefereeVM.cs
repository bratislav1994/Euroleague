using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Referee
{
    public class RefereeVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string name;
        private string surname;
        private string country;
        private Visibility addButtonVisibility = Visibility.Hidden;
        private Visibility editButtonVisibility = Visibility.Hidden;
        private Visibility addOrEditRefereeCanvasVis = Visibility.Hidden;
        private DelegateCommand clickAddRefereeCommand;
        private DelegateCommand editRefereeCommand;
        private DelegateCommand removeRefereeCommand;
        private DelegateCommand addRefereeCommand;
        private DelegateCommand clickEditRefereeCommand;
        private RefereeModel model = null;
        private Sudija selectedReferee = null;
        private ICollectionView referees;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;
        private string countryFilter = string.Empty;

        public RefereeVM()
        {
            this.model = new RefereeModel();
            this.Model.RefereesList = this.Model.GetAllReferees();
            this.Referees = new CollectionViewSource { Source = this.Model.RefereesList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Referees = CollectionViewSource.GetDefaultView(this.Model.RefereesList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[RefereeHeaderDG.IME_SUD.ToString()] = string.Empty;
            columnFilters[RefereeHeaderDG.PRZ_SUD.ToString()] = string.Empty;
            columnFilters[RefereeHeaderDG.DRZ_SUD.ToString()] = string.Empty;
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
                columnFilters[RefereeHeaderDG.IME_SUD.ToString()] = this.nameFilter;
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
                columnFilters[RefereeHeaderDG.PRZ_SUD.ToString()] = this.surnameFilter;
                this.OnFilterApply();
            }
        }
        
        public string CountryFilter
        {
            get
            {
                return countryFilter;
            }

            set
            {
                countryFilter = value;
                this.RaisePropertyChanged("CountryFilter");
                columnFilters[RefereeHeaderDG.DRZ_SUD.ToString()] = this.countryFilter;
                this.OnFilterApply();
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

        public Visibility AddOrEditRefereeCanvasVis
        {
            get
            {
                return addOrEditRefereeCanvasVis;
            }

            set
            {
                addOrEditRefereeCanvasVis = value;
                this.RaisePropertyChanged("AddOrEditRefereeCanvasVis");
            }
        }

        public Sudija SelectedReferee
        {
            get
            {
                return selectedReferee;
            }

            set
            {
                selectedReferee = value;
                this.RaisePropertyChanged("SelectedReferee");
            }
        }

        public ICollectionView Referees
        {
            get
            {
                return this.referees;
            }

            set
            {
                this.referees = value;
                this.RaisePropertyChanged("Referees");
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                country = value;
                this.RaisePropertyChanged("Country");
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

        public RefereeModel Model
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
            this.Country = string.Empty;
            this.Surname = string.Empty;
            this.Name = string.Empty;
        }

        #region filter

        public void OnFilterApply()
        {
            this.Referees = CollectionViewSource.GetDefaultView(this.Model.RefereesList);
            if (this.Referees != null)
            {
                this.Referees.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(RefereeHeaderDG.IME_SUD.ToString()))
                            {
                                containsFilter = ((Sudija)item).IME_SUD.IndexOf(NameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(RefereeHeaderDG.PRZ_SUD.ToString()))
                            {
                                containsFilter = ((Sudija)item).PRZ_SUD.IndexOf(SurnameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(RefereeHeaderDG.DRZ_SUD.ToString()))
                            {
                                containsFilter = ((Sudija)item).DRZ_SUD.IndexOf(CountryFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

        #region click add command

        public DelegateCommand ClickAddRefereeCommand
        {
            get
            {
                if (this.clickAddRefereeCommand == null)
                {
                    this.clickAddRefereeCommand = new DelegateCommand(this.ClickAddReferee);
                }

                return this.clickAddRefereeCommand;
            }
        }

        private void ClickAddReferee()
        {
            this.ResetFocusFields();
            this.AddOrEditRefereeCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Visible;
            this.EditButtonVisibility = Visibility.Hidden;
            this.ResetFields();
        }

        private void ResetFocusFields()
        {
            this.isCountryFocus = false;
            this.isNameFocus = false;
            this.isSurnameFocus = false;
        }

        #endregion

        #region click edit command

        public DelegateCommand ClickEditRefereeCommand
        {
            get
            {
                if (this.clickEditRefereeCommand == null)
                {
                    this.clickEditRefereeCommand = new DelegateCommand(this.ClickEdit, this.IsSelected);
                }

                return this.clickEditRefereeCommand;
            }
        }

        private void ClickEdit()
        {
            this.ResetFocusFields();
            this.AddOrEditRefereeCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Hidden;
            this.EditButtonVisibility = Visibility.Visible;

            using (var db = new EuroleagueEntities3())
            {
                Sudija s = db.Sudijas.Where(x => x.LICBR_SUD.Equals(this.SelectedReferee.LICBR_SUD)).FirstOrDefault();
                this.Country = s.DRZ_SUD;
                this.Surname = s.PRZ_SUD;
                this.Name = s.IME_SUD;
            }
        }

        #endregion

        #region edit command

        public DelegateCommand EditRefereeCommand
        {
            get
            {
                if (this.editRefereeCommand == null)
                {
                    this.editRefereeCommand = new DelegateCommand(this.EditReferee, this.CanAddReferee);
                }

                return this.editRefereeCommand;
            }
        }

        private void EditReferee()
        {
            using (var db = new EuroleagueEntities3())
            {
                Sudija s = db.Sudijas.Where(x => x.LICBR_SUD.Equals(this.SelectedReferee.LICBR_SUD)).FirstOrDefault();
                s.DRZ_SUD = this.Country;
                s.PRZ_SUD = this.Surname;
                s.IME_SUD = this.Name;
                db.Entry(s).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                s = this.Model.RefereesList.Where(x => x.LICBR_SUD.Equals(this.SelectedReferee.LICBR_SUD)).FirstOrDefault();
                s.DRZ_SUD = this.Country;
                s.PRZ_SUD = this.Surname;
                s.IME_SUD = this.Name;
            }

            this.AddOrEditRefereeCanvasVis = Visibility.Hidden;
            this.ResetFields();
        }

        #endregion

        #region remove command

        public DelegateCommand RemoveRefereeCommand
        {
            get
            {
                if (this.removeRefereeCommand == null)
                {
                    this.removeRefereeCommand = new DelegateCommand(this.RemoveReferee, this.IsSelected);
                }

                return this.removeRefereeCommand;
            }
        }

        private void RemoveReferee()
        {
            using (var db = new EuroleagueEntities3())
            {
                Sudija s = db.Sudijas.Where(x => x.LICBR_SUD.Equals(this.SelectedReferee.LICBR_SUD)).FirstOrDefault();
                List<Sudi> refereeRef = db.Sudis.Where(x => x.Sudija_LICBR_SUD.Equals(s.LICBR_SUD)).ToList();
                refereeRef.ForEach(x => db.Entry(x).State = System.Data.Entity.EntityState.Deleted);
                db.Entry(s).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                this.Model.RefereesList.Remove(this.Model.RefereesList.Where(x => x.LICBR_SUD.Equals(this.SelectedReferee.LICBR_SUD)).FirstOrDefault());
            }
        }

        private bool IsSelected()
        {
            return this.SelectedReferee != null;
        }

        #endregion

        #region add referee command

        public DelegateCommand AddRefereeCommand
        {
            get
            {
                if (this.addRefereeCommand == null)
                {
                    this.addRefereeCommand = new DelegateCommand(this.AddReferee, this.CanAddReferee);
                }

                return this.addRefereeCommand;
            }
        }

        private bool CanAddReferee()
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Country) ||
                string.IsNullOrEmpty(this.Surname))
            {
                return false;
            }

            return true;
        }

        private void AddReferee()
        {
            using (var db = new EuroleagueEntities3())
            {
                Sudija s = new Sudija()
                {
                    DRZ_SUD = this.Country,
                    IME_SUD = this.Name,
                    PRZ_SUD = this.Surname,
                    LICBR_SUD = Guid.NewGuid().ToString().Substring(0, 15),
                    Evroliga_OZN_LIG = "evroliga01"
                };

                db.Sudijas.Add(s);
                db.SaveChanges();

                this.Model.RefereesList.Add(new Sudija()
                {
                    DRZ_SUD = s.DRZ_SUD,
                    IME_SUD = s.IME_SUD,
                    PRZ_SUD = s.PRZ_SUD,
                    LICBR_SUD = s.LICBR_SUD,
                });
            }

            this.AddOrEditRefereeCanvasVis = Visibility.Hidden;
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
                RaisePropertyChanged(this.Name);
            }
            else if (element.Equals("Country"))
            {
                this.isCountryFocus = true;
                RaisePropertyChanged(this.Country);
            }
            else if (element.Equals("Surname"))
            {
                this.isSurnameFocus = true;
                RaisePropertyChanged(this.Surname);
            }
        }

        private bool isNameFocus = false;
        private bool isCountryFocus = false;
        private bool isSurnameFocus = false;

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
                if (columnName == "Name" && string.IsNullOrWhiteSpace(this.Name) && this.isNameFocus)
                {
                    error = "Name is required!";
                }
                else if (columnName == "Country" && string.IsNullOrWhiteSpace(this.Country) && this.isCountryFocus)
                {
                    error = "Country name is required!";
                }
                else if (columnName == "Surname" && string.IsNullOrWhiteSpace(this.Surname) && isSurnameFocus)
                {
                    error = "Surname is required!";
                }

                this.AddRefereeCommand.RaiseCanExecuteChanged();
                this.EditRefereeCommand.RaiseCanExecuteChanged();

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

                if (propName.Equals("SelectedReferee"))
                {
                    this.ClickEditRefereeCommand.RaiseCanExecuteChanged();
                    this.RemoveRefereeCommand.RaiseCanExecuteChanged();

                    if (this.SelectedReferee != null)
                    {
                        this.AddOrEditRefereeCanvasVis = Visibility.Hidden;
                    }
                }
            }
        }
    }
}

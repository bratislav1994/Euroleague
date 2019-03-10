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

namespace Project.Arena
{
    public class ArenaVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string name;
        private string country;
        private string place;
        private Visibility addButtonVisibility = Visibility.Hidden;
        private Visibility editButtonVisibility = Visibility.Hidden;
        private Visibility addOrEditHallCanvasVis = Visibility.Hidden;
        private DelegateCommand clickAddArenaCommand;
        private DelegateCommand editArenaCommand;
        private DelegateCommand removeArenaCommand;
        private DelegateCommand addArenaCommand;
        private DelegateCommand clickEditArenaCommand;
        private ArenaModel model = null;
        private Hala selectedHall = null;
        private ICollectionView halls;
        private string nameFilter = string.Empty;
        private string placeFilter = string.Empty;
        private string countryFilter = string.Empty;

        public ArenaVM()
        {
            this.model = new ArenaModel();
            this.Model.HallsList = this.Model.GetAllHalls();
            this.Halls = new CollectionViewSource { Source = this.Model.HallsList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Halls = CollectionViewSource.GetDefaultView(this.Model.HallsList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[ArenaHeaderDG.NAZ_HALA.ToString()] = string.Empty;
            columnFilters[ArenaHeaderDG.GRD_HALA.ToString()] = string.Empty;
            columnFilters[ArenaHeaderDG.DRZ_HALA.ToString()] = string.Empty;
        }

        public Hala SelectedHall
        {
            get
            {
                return selectedHall;
            }

            set
            {
                selectedHall = value;
                this.RaisePropertyChanged("SelectedHall");
            }
        }

        public ICollectionView Halls
        {
            get
            {
                return halls;
            }

            set
            {
                halls = value;
                this.RaisePropertyChanged("Halls");
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
                columnFilters[ArenaHeaderDG.NAZ_HALA.ToString()] = this.nameFilter;
                this.OnFilterApply();
            }
        }

        public string PlaceFilter
        {
            get
            {
                return placeFilter;
            }

            set
            {
                placeFilter = value;
                this.RaisePropertyChanged("PlaceFilter");
                columnFilters[ArenaHeaderDG.GRD_HALA.ToString()] = this.placeFilter;
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
                columnFilters[ArenaHeaderDG.DRZ_HALA.ToString()] = this.countryFilter;
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

        public Visibility AddOrEditHallCanvasVis
        {
            get
            {
                return addOrEditHallCanvasVis;
            }

            set
            {
                addOrEditHallCanvasVis = value;
                this.RaisePropertyChanged("AddOrEditHallCanvasVis");
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

        public string Place
        {
            get
            {
                return place;
            }

            set
            {
                place = value;
                this.RaisePropertyChanged("Place");
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

        public ArenaModel Model
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
            this.Place = string.Empty;
            this.Name = string.Empty;
        }

        #region filter

        public void OnFilterApply()
        {
            this.Halls = CollectionViewSource.GetDefaultView(this.Model.HallsList);
            if (this.Halls != null)
            {
                this.Halls.Filter = delegate (object item)
                {
                    bool show = true;

                    foreach (KeyValuePair<string, string> filter in columnFilters)
                    {
                        object property = GetPropertyValue(item, filter.Key);
                        if (property != null)
                        {
                            bool containsFilter = false;

                            if (filter.Key.Equals(ArenaHeaderDG.NAZ_HALA.ToString()))
                            {
                                containsFilter = ((Hala)item).NAZ_HALA.IndexOf(NameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(ArenaHeaderDG.GRD_HALA.ToString()))
                            {
                                containsFilter = ((Hala)item).GRD_HALA.IndexOf(PlaceFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
                            }
                            else if (filter.Key.Equals(ArenaHeaderDG.DRZ_HALA.ToString()))
                            {
                                containsFilter = ((Hala)item).DRZ_HALA.IndexOf(CountryFilter, StringComparison.InvariantCultureIgnoreCase) >= 0;
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

        public DelegateCommand ClickAddArenaCommand
        {
            get
            {
                if (this.clickAddArenaCommand == null)
                {
                    this.clickAddArenaCommand = new DelegateCommand(this.ClickAddArena);
                }

                return this.clickAddArenaCommand;
            }
        }

        private void ClickAddArena()
        {
            this.ResetFocusFields();
            this.AddOrEditHallCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Visible;
            this.EditButtonVisibility = Visibility.Hidden;
            this.ResetFields();
        }

        private void ResetFocusFields()
        {
            this.isCountryFocus = false;
            this.isNameFocus = false;
            this.isPlaceFocus = false;
        }
        
        #endregion

        #region click edit command

        public DelegateCommand ClickEditArenaCommand
        {
            get
            {
                if (this.clickEditArenaCommand == null)
                {
                    this.clickEditArenaCommand = new DelegateCommand(this.ClickEdit, this.IsSelected);
                }

                return this.clickEditArenaCommand;
            }
        }

        private void ClickEdit()
        {
            this.ResetFocusFields();
            this.AddOrEditHallCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Hidden;
            this.EditButtonVisibility = Visibility.Visible;
            using (var db = new EuroleagueEntities3())
            {
                Hala h = db.Halas.Where(x => x.OZN_HALA.Equals(this.SelectedHall.OZN_HALA)).FirstOrDefault();
                this.Country = h.DRZ_HALA;
                this.Place = h.GRD_HALA;
                this.Name = h.NAZ_HALA;
            }
        }

        #endregion

        #region edit command

        public DelegateCommand EditArenaCommand
        {
            get
            {
                if (this.editArenaCommand == null)
                {
                    this.editArenaCommand = new DelegateCommand(this.EditArena, this.CanAddArena);
                }

                return this.editArenaCommand;
            }
        }

        private void EditArena()
        {
            using (var db = new EuroleagueEntities3())
            {
                Hala h = db.Halas.Where(x => x.OZN_HALA.Equals(this.SelectedHall.OZN_HALA)).FirstOrDefault();
                h.DRZ_HALA = this.Country;
                h.GRD_HALA = this.Place;
                h.NAZ_HALA = this.Name;
                db.Entry(h).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                h = this.Model.HallsList.Where(x => x.OZN_HALA.Equals(this.SelectedHall.OZN_HALA)).FirstOrDefault();
                h.DRZ_HALA = this.Country;
                h.GRD_HALA = this.Place;
                h.NAZ_HALA = this.Name;
            }

            this.AddOrEditHallCanvasVis = Visibility.Hidden;
            this.ResetFields();
        }

        #endregion

        #region remove command

        public DelegateCommand RemoveArenaCommand
        {
            get
            {
                if (this.removeArenaCommand == null)
                {
                    this.removeArenaCommand = new DelegateCommand(this.RemoveArena, this.IsSelected);
                }

                return this.removeArenaCommand;
            }
        }

        private void RemoveArena()
        {
            using (var db = new EuroleagueEntities3())
            {
                Hala h = db.Halas.Where(x => x.OZN_HALA.Equals(this.SelectedHall.OZN_HALA)).FirstOrDefault();
                List<Rezervacija> reservations = db.Rezervacijas.Where(x => x.Hala_OZN_HALA.Equals(h.OZN_HALA)).ToList();
                reservations.ForEach(x => db.Entry(x).State = System.Data.Entity.EntityState.Deleted);
                db.Entry(h).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                this.Model.HallsList.Remove(this.Model.HallsList.Where(x => x.OZN_HALA.Equals(this.SelectedHall.OZN_HALA)).FirstOrDefault());
            }
        }

        private bool IsSelected()
        {
            return this.SelectedHall != null;
        }

        #endregion

        #region add arena command

        public DelegateCommand AddArenaCommand
        {
            get
            {
                if (this.addArenaCommand == null)
                {
                    this.addArenaCommand = new DelegateCommand(this.AddArena, this.CanAddArena);
                }

                return this.addArenaCommand;
            }
        }
        
        private bool CanAddArena()
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Country) ||
                string.IsNullOrEmpty(this.Place))
            {
                return false;
            }

            return true;
        }

        private void AddArena()
        {
            using (var db = new EuroleagueEntities3())
            {
                Hala h = new Hala()
                {
                    DRZ_HALA = this.Country,
                    GRD_HALA = this.Place,
                    NAZ_HALA = this.Name,
                    OZN_HALA = Guid.NewGuid().ToString().Substring(0, 15)
                };

                db.Halas.Add(h);
                db.SaveChanges();

                this.Model.HallsList.Add(h);
            }

            this.AddOrEditHallCanvasVis = Visibility.Hidden;
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
            else if (element.Equals("Place"))
            {
                this.isPlaceFocus = true;
                RaisePropertyChanged(this.Place);
            }
        }

        private bool isNameFocus = false;
        private bool isCountryFocus = false;
        private bool isPlaceFocus = false;

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
                else if (columnName == "Place" && string.IsNullOrWhiteSpace(this.Place) && isPlaceFocus)
                {
                    error = "Place name is required!";
                }
                
                this.AddArenaCommand.RaiseCanExecuteChanged();
                this.EditArenaCommand.RaiseCanExecuteChanged();

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

                if (propName.Equals("SelectedHall"))
                {
                    this.ClickEditArenaCommand.RaiseCanExecuteChanged();
                    this.RemoveArenaCommand.RaiseCanExecuteChanged();

                    if (this.SelectedHall != null)
                    {
                        this.AddOrEditHallCanvasVis = Visibility.Hidden;
                    }
                }
            }
        }
    }
}

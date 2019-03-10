using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.Entity.Validation;

namespace Project.Players
{
    public class PlayerVM : INotifyPropertyChanged, IDataErrorInfo
    {
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string name;
        private string surname;
        private Klub club;
        private string startDate;
        private string endDate;
        private string position;
        private float height;
        private Visibility addButtonVisibility = Visibility.Hidden;
        private Visibility editButtonVisibility = Visibility.Hidden;
        private Visibility addOrEditPlayerCanvasVis = Visibility.Hidden;
        private DelegateCommand clickAddPlayerCommand;
        private DelegateCommand editPlayerCommand;
        private DelegateCommand removePlayerCommand;
        private DelegateCommand addPlayerCommand;
        private DelegateCommand clickEditPlayerCommand;
        private PlayerModel model = null;
        private string clubName;
        private Igrac selectedPlayer = null;
        private ICollectionView players;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;
        private string clubFilter = string.Empty;
        private DelegateCommand browseImage;
        private BitmapImage image;
        private string fileName = string.Empty;

        public PlayerVM()
        {
            this.model = new PlayerModel();
            this.Model.PlayersList = this.Model.GetAllPlayers();
            this.Players = new CollectionViewSource { Source = this.Model.PlayersList }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Players = CollectionViewSource.GetDefaultView(this.Model.PlayersList);
        }

        private void FillColumnNames()
        {
            columnFilters = new Dictionary<string, string>();
            columnFilters[PlayerHeaderDG.IME_IGR.ToString()] = string.Empty;
            columnFilters[PlayerHeaderDG.PRZ_IGR.ToString()] = string.Empty;
            columnFilters[PlayerHeaderDG.Klub.ToString()] = string.Empty;
        }

        public Igrac SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }

            set
            {
                selectedPlayer = value;
                this.RaisePropertyChanged("SelectedPlayer");
            }
        }

        public BitmapImage Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
                this.RaisePropertyChanged("Image");
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

        public Visibility AddOrEditPlayerCanvasVis
        {
            get
            {
                return addOrEditPlayerCanvasVis;
            }

            set
            {
                addOrEditPlayerCanvasVis = value;
                this.RaisePropertyChanged("AddOrEditPlayerCanvasVis");
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
                if (this.club != null)
                {
                    this.ClubName = this.club.NAZ_KLB;
                    this.RaisePropertyChanged("ClubName");
                }
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

        public float Height
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

        public string ClubName
        {
            get
            {
                return clubName;
            }

            set
            {
                clubName = value;
                this.RaisePropertyChanged("ClubName");
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

        public PlayerModel Model
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
            this.Name = string.Empty;
            this.Position = string.Empty;
            this.Height = 0;
        }

        #region filter

        public void OnFilterApply()
        {
            this.Players = CollectionViewSource.GetDefaultView(this.Model.PlayersList);
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

        #region click add command

        public DelegateCommand ClickAddPlayerCommand
        {
            get
            {
                if (this.clickAddPlayerCommand == null)
                {
                    this.clickAddPlayerCommand = new DelegateCommand(this.ClickAddPlayer);
                }

                return this.clickAddPlayerCommand;
            }
        }

        private void ClickAddPlayer()
        {
            this.ResetFocusFields();
            this.AddOrEditPlayerCanvasVis = Visibility.Visible;
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
            this.isPositionFocus = false;
            this.isHeightFocus = false;
        }

        #endregion

        #region browse image command

        public DelegateCommand BrowseImage
        {
            get
            {
                if (this.browseImage == null)
                {
                    this.browseImage = new DelegateCommand(this.OpenFileImage);
                }

                return this.browseImage;
            }
        }
        
        private void OpenFileImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg";
            Nullable<bool> result = dlg.ShowDialog();
            //Igrac i = new Igrac() { SLIKA_IGR = }
            if (result == true)
            {
                this.fileName = dlg.FileName;
                BitmapImage b = new BitmapImage();
                b.BeginInit();
                b.UriSource = new Uri(this.fileName);
                b.EndInit();
                this.Image = b;//.ToString();
            }
        }

        #endregion

        #region click edit command

        public DelegateCommand ClickEditPlayerCommand
        {
            get
            {
                if (this.clickEditPlayerCommand == null)
                {
                    this.clickEditPlayerCommand = new DelegateCommand(this.ClickEdit, this.IsSelected);
                }

                return this.clickEditPlayerCommand;
            }
        }

        private void ClickEdit()
        {
            this.ResetFocusFields();
            this.AddOrEditPlayerCanvasVis = Visibility.Visible;
            this.AddButtonVisibility = Visibility.Hidden;
            this.EditButtonVisibility = Visibility.Visible;
            this.Model.Clubs = this.Model.GetAllClubs();
            Igrac i;

            using (var db = new EuroleagueEntities3())
            {
                i = db.Igracs.Where(x => x.LICBR_IGR.Equals(this.SelectedPlayer.LICBR_IGR)).FirstOrDefault();
                this.Name = i.IME_IGR;
                this.Surname = i.PRZ_IGR;
                this.Club = this.Model.GetClubById(i.Klub_ID_KLB);
                this.StartDate = i.PDATUG_IGR.ToString();
                this.EndDate = i.KDATUG_IGR.ToString();
                this.Position = i.POZ_IGR;
                this.Height = i.VIS_IGR;
            }

            this.Image = i.SLIKA_IGR == null ? null : this.ImageFromBuffer(i.SLIKA_IGR);
           // BitmapImage a = ImageFromBuffer(i.SLIKA_IGR);
           // this.Image = a;
        }

        #endregion

        #region convert image-byte array

        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private byte[] BitmapSourceToByteArray(BitmapSource image)
        {
            using (var stream = new MemoryStream())
            {
                var encoder = new JpegBitmapEncoder(); // or some other encoder
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        #endregion

        #region edit command

        public DelegateCommand EditPlayerCommand
        {
            get
            {
                if (this.editPlayerCommand == null)
                {
                    this.editPlayerCommand = new DelegateCommand(this.EditPlayer, this.CanAddPlayer);
                }

                return this.editPlayerCommand;
            }
        }

        private void EditPlayer()
        {
            using (var db = new EuroleagueEntities3())
            {
                Igrac i = db.Igracs.Where(x => x.LICBR_IGR.Equals(this.SelectedPlayer.LICBR_IGR)).FirstOrDefault();
                i.IME_IGR = this.Name;
                i.PRZ_IGR = this.Surname;
                i.Klub_ID_KLB = this.Club.ID_KLB;
                i.PDATUG_IGR = DateTime.Parse(this.StartDate);
                i.KDATUG_IGR = DateTime.Parse(EndDate);
                i.POZ_IGR = this.Position;
                i.VIS_IGR = this.Height;
                BitmapImage b = new BitmapImage();
                if (!string.IsNullOrEmpty(this.fileName))
                {
                    b.BeginInit();
                    b.UriSource = new Uri(this.fileName);
                    b.EndInit();
                    this.Image = b;
                }

                i.SLIKA_IGR = string.IsNullOrEmpty(this.fileName) ? null : BitmapSourceToByteArray(b);

                db.Entry(i).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                i = this.Model.PlayersList.Where(x => x.LICBR_IGR.Equals(this.SelectedPlayer.LICBR_IGR)).FirstOrDefault();
                i.IME_IGR = this.Name;
                i.PRZ_IGR = this.Surname;
                i.Klub_ID_KLB = this.Club.ID_KLB;
                i.Klub = this.Model.GetClubById(i.Klub_ID_KLB);
                i.PDATUG_IGR = DateTime.Parse(this.StartDate);
                i.KDATUG_IGR = DateTime.Parse(EndDate);
                i.POZ_IGR = this.Position;
                i.VIS_IGR = this.Height;

                this.Players.Refresh();
            }
            
            if (!string.IsNullOrWhiteSpace(this.fileName))
            {
                Bitmap img = new Bitmap(this.fileName);
                img.Save(Path.GetFullPath(@"..\..\imagesOfPlayers\" + this.SelectedPlayer.LICBR_IGR + ".jpg"), ImageFormat.Jpeg);

            }

            this.AddOrEditPlayerCanvasVis = Visibility.Hidden;
            this.ResetFields();
            this.ResetImageFields();
        }
        
        #endregion

        #region remove command

        public DelegateCommand RemovePlayerCommand
        {
            get
            {
                if (this.removePlayerCommand == null)
                {
                    this.removePlayerCommand = new DelegateCommand(this.RemovePlayer, this.IsSelected);
                }

                return this.removePlayerCommand;
            }
        }

        private void RemovePlayer()
        {
            using (var db = new EuroleagueEntities3())
            {
                Igrac i = db.Igracs.Where(x => x.LICBR_IGR.Equals(this.SelectedPlayer.LICBR_IGR)).FirstOrDefault();
                List<IgracIgra> playersPlay = db.IgracIgras.Where(x => x.Igrac_LICBR_IGR.Equals(i.LICBR_IGR)).ToList();
                playersPlay.ForEach(x => db.Entry(x).State = System.Data.Entity.EntityState.Deleted);
                db.Entry(i).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                string path = Path.GetFullPath(@"..\..\imagesOfPlayers\" + i.LICBR_IGR + ".jpg");
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                this.Model.PlayersList.Remove(this.Model.PlayersList.Where(x => x.LICBR_IGR.Equals(this.SelectedPlayer.LICBR_IGR)).FirstOrDefault());
            }
        }

        private bool IsSelected()
        {
            return this.SelectedPlayer != null;
        }

        #endregion

        #region add player command

        public DelegateCommand AddPlayerCommand
        {
            get
            {
                if (this.addPlayerCommand == null)
                {
                    this.addPlayerCommand = new DelegateCommand(this.AddPlayer, this.CanAddPlayer);
                }

                return this.addPlayerCommand;
            }
        }

        private bool CanAddPlayer()
        {
            if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.Surname) ||
                this.Club == null || string.IsNullOrEmpty(this.StartDate) || string.IsNullOrEmpty(this.EndDate) ||
                string.IsNullOrEmpty(this.Position) || (this.Height < 1.50 || this.Height > 2.50))
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

        private void AddPlayer()
        {
            using (var db = new EuroleagueEntities3())
            {
                BitmapImage b = new BitmapImage();
                if (!string.IsNullOrEmpty(this.fileName))
                {
                    b.BeginInit();
                    b.UriSource = new Uri(this.fileName);
                    b.EndInit();
                    this.Image = b;
                }
                
                //.ToString();
               // var imageBuffer = BitmapSourceToByteArray((BitmapSource)b);
                Igrac i = new Igrac()
                {
                    IME_IGR = this.Name,
                    PRZ_IGR = this.Surname,
                    KDATUG_IGR = DateTime.Parse(this.EndDate),
                    Klub_ID_KLB = this.Club.ID_KLB,
                    PDATUG_IGR = DateTime.Parse(this.StartDate),
                    LICBR_IGR = Guid.NewGuid().ToString().Substring(0, 15),
                    POZ_IGR = this.Position,
                    VIS_IGR = this.Height,
                    //SLIKA_IGR = BitmapSourceToByteArray((BitmapSource)b)
                    SLIKA_IGR = string.IsNullOrEmpty(this.fileName) ? null : BitmapSourceToByteArray(b)
                };

                db.Igracs.Add(i);
                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    string a = e.Message;
                }
                
                // Bitmap img = new Bitmap(this.fileName);
                // img.Save(Path.GetFullPath(@"..\..\imagesOfPlayers\" + i.LICBR_IGR + ".jpg"), ImageFormat.Jpeg);
                
                this.Model.PlayersList.Add(this.Model.GetPlayerById(i.LICBR_IGR));
            }

            this.AddOrEditPlayerCanvasVis = Visibility.Hidden;
            this.ResetFields();
            this.ResetImageFields();
        }
        
        private void ResetImageFields()
        {
            this.fileName = string.Empty;
           // this.Image = string.Empty;
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
            else if (element.Equals("Position"))
            {
                this.isPositionFocus = true;
                RaisePropertyChanged(this.Position);
            }
            else if (element.Equals("Height"))
            {
                this.isHeightFocus = true;
                RaisePropertyChanged("Height");
            }
        }

        private bool isNameFocus = false;
        private bool isSurnameFocus = false;
        private bool isClubFocus = false;
        private bool isStartFocus = false;
        private bool isEndFocus = false;
        private bool isPositionFocus = false;
        private bool isHeightFocus = false;

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
                if (columnName.Equals("Name") && string.IsNullOrWhiteSpace(this.Name) && this.isNameFocus)
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
                else if (columnName.Equals("Position") && string.IsNullOrWhiteSpace(this.Position) && this.isPositionFocus)
                {
                    error = "Position is required!";
                }
                else if (columnName.Equals("Height") && this.isHeightFocus)
                {
                    if (this.Height < 1.50 || this.Height > 2.50)
                    {
                        error = "Height must be between 1.50 and 2.50 meters!";
                    }
                }

                this.AddPlayerCommand.RaiseCanExecuteChanged();
                this.EditPlayerCommand.RaiseCanExecuteChanged();

                return error;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null && propName != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                //if (propName.Equals("SelectedClub"))
                //{
                //    if (this.SelectedClub == null)
                //    {
                //        this.Model.PlayersList = null;
                //    }
                //    else
                //    {
                //        this.Model.PlayersList = this.Model.GetAllPlayersById(this.SelectedClub.ID_KLB);
                //    }
                //}
                if (propName.Equals("SelectedPlayer"))
                {
                    this.ClickEditPlayerCommand.RaiseCanExecuteChanged();
                    this.RemovePlayerCommand.RaiseCanExecuteChanged();

                    if (this.SelectedPlayer != null)
                    {
                        this.AddOrEditPlayerCanvasVis = Visibility.Hidden;
                    }
                }
            }
        }
    }
}

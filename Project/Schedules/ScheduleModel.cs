using Project.HelperClasses;
using Project.Referee;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Project.Schedules
{
    public class ScheduleModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private BindingList<Hala> halls = null;
        private BindingList<Schedule> schedules;
        private List<string> rounds = null;
        private Dictionary<string, string> columnFilters;
        private Dictionary<string, PropertyInfo> propertyCache;
        private string nameFilter = string.Empty;
        private string surnameFilter = string.Empty;
        private ObservableCollection<Selected> listOFReferees;
        private ICollectionView referees = null;

        public ScheduleModel()
        {
            this.schedules = new BindingList<Schedule>();
            this.Rounds = new List<string>();
            using (var db = new EuroleagueEntities3())
            {
                if (db.Klubs.Count() > 0)
                {
                    int count = (db.Klubs.Count() - 1) * 2 + 1;
                    for (int i = 1; i < count; i++)
                    {
                        this.Rounds.Add("Round " + i.ToString());
                    }
                }
            }

            this.Referees = new CollectionViewSource { Source = this.ListOfReferees }.View;
            this.FillColumnNames();
            this.propertyCache = new Dictionary<string, PropertyInfo>();
            this.Referees = CollectionViewSource.GetDefaultView(this.ListOfReferees);
        }

        #region properties

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

        public ObservableCollection<Selected> ListOfReferees
        {
            get
            {
                return this.listOFReferees;
            }
            set
            {
                this.listOFReferees = value;
                this.RaisePropertyChanged("ListOfReferees");
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

        public BindingList<Schedule> Schedules
        {
            get
            {
                return schedules;
            }

            set
            {
                schedules = value;
                this.RaisePropertyChanged("Schedules");
            }
        }

        public List<string> Rounds
        {
            get
            {
                return rounds;
            }

            set
            {
                rounds = value;
                this.RaisePropertyChanged("Rounds");
            }
        }

        public BindingList<Hala> Halls
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

        #endregion

        #region filter

        public void OnFilterApply()
        {
            this.Referees = CollectionViewSource.GetDefaultView(this.ListOfReferees);
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

        public void SelectedRefereesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // this.AddRefereesCommand.RaiseCanExecuteChanged();
            this.RaisePropertyChanged("ListOfReferees");
        }

        #region reservation

        public BindingList<Hala> GetAllHalls()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new BindingList<Hala>(db.Halas.ToList());
            }
        }

        public bool DateValid(string date)
        {
            try
            {
                DateTime.Parse(date);
                return true;
            }
            catch (FormatException)
            {
                throw;
                // return false;
            }
        }

        public string GetClubById(string id)
        {
            using (var db = new EuroleagueEntities3())
            {
                return db.Klubs.Where(x => x.ID_KLB.Equals(id)).FirstOrDefault().NAZ_KLB;
            }
        }

        public Rezervacija GetHallById(string id)
        {
            using (var db = new EuroleagueEntities3())
            {
                return db.Rezervacijas.Include("Hala").Where(x => x.SFR_REZ.Equals(id)).FirstOrDefault();
            }
        }

        public Rezervacija CheckIfReservationExists(string idGame)
        {
            using (var db = new EuroleagueEntities3())
            {
                return db.Rezervacijas.Where(x => x.SFR_REZ.Equals(idGame)).FirstOrDefault();
            }
        }

        #endregion

        #region conference

        public void ChangeConference(string idGame, bool isConferenceChecked)
        {
            using (var db = new EuroleagueEntities3())
            {
                Utakmica u = db.Utakmicas.Where(x => x.OZN_UTK.Equals(idGame)).FirstOrDefault();

                if (u != null)
                {
                    u.KONFMDJ_UTK = isConferenceChecked == true ? true : false;
                    db.Entry(u).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        #endregion

        #region change game result into db

        public void ChangeGameResult(Schedule selectedGame)
        {
            using (var db = new EuroleagueEntities3())
            {
                Utakmica u = db.Utakmicas.Where(x => x.OZN_UTK.Equals(selectedGame.IdGame)).FirstOrDefault();
                int oldPtsHome = u.DOMPOENI_UTK != null ? (int)u.DOMPOENI_UTK : 0;
                int oldPtsAway = u.GOSTPOENI_UTK != null ? (int)u.GOSTPOENI_UTK : 0;
                int oldDiffHome = u.DOMPOENI_UTK != null ? (int)(u.DOMPOENI_UTK - u.GOSTPOENI_UTK) : 0;
                int oldDiffAway = u.GOSTPOENI_UTK != null ? (int)(u.GOSTPOENI_UTK - u.DOMPOENI_UTK) : 0;
                u.DOMPOENI_UTK = selectedGame.PointsTeam1;
                u.GOSTPOENI_UTK = selectedGame.PointsTeam2;
                db.Entry(u).State = System.Data.Entity.EntityState.Modified;

                Klub k1 = db.Klubs.Where(x => x.ID_KLB.Equals(u.Klub_ID_KLB)).FirstOrDefault();
                Klub k2 = db.Klubs.Where(x => x.ID_KLB.Equals(u.Klub_ID_KLB1)).FirstOrDefault();

                k1.KOSRAZ_KLB -= oldDiffHome;
                k2.KOSRAZ_KLB -= oldDiffAway;
                k1.KOSRAZ_KLB += (int)(u.DOMPOENI_UTK - u.GOSTPOENI_UTK);
                k2.KOSRAZ_KLB += (int)(u.GOSTPOENI_UTK - u.DOMPOENI_UTK);

                if (oldPtsAway == 0 && oldPtsHome == 0)
                {
                    k1.POB_KLB = u.DOMPOENI_UTK > u.GOSTPOENI_UTK ? k1.POB_KLB + 1 : k1.POB_KLB;
                    k1.POR_KLB = u.DOMPOENI_UTK < u.GOSTPOENI_UTK ? k1.POR_KLB + 1 : k1.POR_KLB;
                    k2.POB_KLB = u.GOSTPOENI_UTK > u.DOMPOENI_UTK ? k2.POB_KLB + 1 : k2.POB_KLB;
                    k2.POR_KLB = u.GOSTPOENI_UTK < u.DOMPOENI_UTK ? k2.POR_KLB + 1 : k2.POR_KLB;
                }
                else if (oldPtsHome > oldPtsAway)
                {
                    if (u.GOSTPOENI_UTK > u.DOMPOENI_UTK)
                    {
                        k1.POB_KLB -= 1;
                        k1.POR_KLB += 1;
                        k2.POB_KLB += 1;
                        k2.POR_KLB -= 1;
                    }
                }
                else if (oldPtsAway > oldPtsHome)
                {
                    if (u.DOMPOENI_UTK > u.GOSTPOENI_UTK)
                    {
                        k1.POB_KLB += 1;
                        k1.POR_KLB -= 1;
                        k2.POB_KLB -= 1;
                        k2.POR_KLB += 1;
                    }
                }

                db.Entry(k1).State = System.Data.Entity.EntityState.Modified;
                db.Entry(k2).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();
            }
        }

        #endregion

        #region add or remove referees from game

        public void AddOrRemoveRefereesFromGame(ObservableCollection<Selected> ListOfReferees, string gameId)
        {
            foreach (Selected selReferee in ListOfReferees)
            {
                if (selReferee.IsSelected)
                {
                    using (var db = new EuroleagueEntities3())
                    {
                        if (db.Sudis.Where(x => x.Sudija_LICBR_SUD.Equals(selReferee.LICBR) && x.Utakmica_OZN_UTK.Equals(gameId)).FirstOrDefault() == null)
                        {
                            AddReferee(selReferee, gameId);
                        }
                    }
                }
                else
                {
                    using (var db = new EuroleagueEntities3())
                    {
                        Sudi s = db.Sudis.Where(x => x.Sudija_LICBR_SUD.Equals(selReferee.LICBR) && x.Utakmica_OZN_UTK.Equals(gameId)).FirstOrDefault();
                        if (s != null)
                        {
                            RemoveFromDbPreviousSelected(selReferee);
                        }
                    }
                }
            }
        }

        private void RemoveFromDbPreviousSelected(Selected selected)
        {
            using (var db = new EuroleagueEntities3())
            {
                Sudi s = db.Sudis.Where(x => x.Sudija_LICBR_SUD.Equals(selected.LICBR)).FirstOrDefault();
                db.Entry(s).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
        }

        private void AddReferee(Selected selected, string gameId)
        {
            using (var db = new EuroleagueEntities3())
            {
                db.Sudis.Add(new Sudi()
                {
                    ID_SUDI = Guid.NewGuid().ToString().Substring(0, 15),
                    Sudija_LICBR_SUD = selected.LICBR,
                    Utakmica_OZN_UTK = gameId
                });

                db.SaveChanges();
            }
        }

        #endregion

        #region get games for selected round

        public void GetGamesForSelectedRound(int round)
        {
            this.Schedules.Clear();
            new Thread(() => this.GetGamesFromDB(round)).Start();
            //this.GetGamesFromDB(round);
        }

        private void GetGamesFromDB(int round)
        {
            using (var db = new EuroleagueEntities3())
            {
                List<Utakmica> games = (List<Utakmica>)db.Utakmicas.Where(x => x.RBRKOLA_UTK == round + 1).ToList();

                foreach (Utakmica utk in games)
                {
                    string first = db.Klubs.Where(x => x.ID_KLB == utk.Klub_ID_KLB).First().NAZ_KLB;
                    string second = db.Klubs.Where(x => x.ID_KLB == utk.Klub_ID_KLB1).First().NAZ_KLB;

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.Schedules.Add(new Schedule()
                        {
                            Team1 = first,
                            Team2 = second,
                            PointsTeam1 = utk.DOMPOENI_UTK == null ? 0 : (int)utk.DOMPOENI_UTK,
                            PointsTeam2 = utk.GOSTPOENI_UTK == null ? 0 : (int)utk.GOSTPOENI_UTK,
                            IdGame = utk.OZN_UTK,
                            IdTeam1 = utk.Klub_ID_KLB,
                            IdTeam2 = utk.Klub_ID_KLB1
                        });
                    });
                }
            }
        }

        #endregion

        #region round robin and generate game

        public void RoundRobinSchedule()
        {
            List<string> allTeamsIds = new List<string>();
            string league = string.Empty;

            using (var db = new EuroleagueEntities3())
            {
                foreach (Klub k in db.Klubs)
                {
                    allTeamsIds.Add(k.ID_KLB);
                }

                league = db.Evroligas.First().OZN_LIG;
            }

            int numDays = allTeamsIds.Count() - 1;
            int halfsize = allTeamsIds.Count() / 2;
            List<string> teams = new List<string>();

            teams.AddRange(allTeamsIds);
            string temp = teams[0];
            teams.RemoveAt(0);
            int teamSize = teams.Count;

            for (int day = 0; day < numDays * 2; day++)
            {
                bool is1stRound = day % 2 == 0 ? true : false;
                int teamIdx = day % teamSize;

                if (is1stRound)
                {
                    GenerateGame(league, day + 1, teams[teamIdx], temp);
                }
                else
                {
                    GenerateGame(league, day + 1, temp, teams[teamIdx]);
                }

                for (int idx = 0; idx < halfsize; idx++)
                {
                    int firstTeam = (day + idx) % teamSize;
                    int secondTeam = ((day + teamSize) - idx) % teamSize;

                    if (firstTeam != secondTeam && is1stRound)
                    {
                        GenerateGame(league, day + 1, teams[firstTeam], teams[secondTeam]);
                    }
                    else if (firstTeam != secondTeam && !is1stRound)
                    {
                        GenerateGame(league, day + 1, teams[secondTeam], teams[firstTeam]);
                    }
                }
            }
        }

        private void GenerateGame(string leagueId, int round, string first, string second)
        {
            Utakmica utk = new Utakmica()
            {
                Evroliga_OZN_LIG = leagueId,
                RBRKOLA_UTK = round,
                Klub_ID_KLB = first,
                Klub_ID_KLB1 = second,
                OZN_UTK = Guid.NewGuid().ToString().Substring(0, 15)
            };

            using (var db = new EuroleagueEntities3())
            {
                db.Utakmicas.Add(utk);
                db.SaveChanges();
            }
        }

        #endregion

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
                if (columnName.Equals("Referees"))
                {
                    if (this.ListOfReferees.Where(x => x.IsSelected == true).Count() > 3)
                    {
                        error = "You can check only 3 referees per game!";
                    }
                }

                return error;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}

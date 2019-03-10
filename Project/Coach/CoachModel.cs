using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace Project.Coach
{
    public class CoachModel : INotifyPropertyChanged
    {
        private ObservableCollection<Trener> coachesList = null;
        private BindingList<Klub> clubs = null;

        public CoachModel()
        {
            this.clubs = this.GetAllClubs();
        }

        public ObservableCollection<Trener> CoachesList
        {
            get
            {
                return coachesList;
            }

            set
            {
                coachesList = value;
            }
        }

        public BindingList<Klub> Clubs
        {
            get
            {
                return clubs;
            }

            set
            {
                clubs = value;
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
                return false;
            }
        }

        public bool IsStartBeforeEnd(string start, string end)
        {
            return DateTime.Parse(start) < DateTime.Parse(end);
        }

        public ObservableCollection<Trener> GetAllCoaches()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Trener>(db.Treners.Include("Klub").ToList());
            }
        }

        public BindingList<Klub> GetAllClubs()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new BindingList<Klub>(db.Klubs.ToList());
            }
        }

        public Klub GetClubById(string id)
        {
            return this.Clubs.Where(x => x.ID_KLB.Equals(id)).FirstOrDefault();
            //using (var db = new EuroleagueEntities31())
            //{
            //    return db.Klubs.Where(x => x.ID_KLB.Equals(id)).FirstOrDefault();
            //}
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

    }
}

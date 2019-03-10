using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Referee
{
    public class RefereeModel
    {
        private ObservableCollection<Sudija> referees = null;

        public RefereeModel()
        {
            this.referees = this.GetAllReferees();
        }

        public ObservableCollection<Sudija> RefereesList
        {
            get
            {
                return referees;
            }

            set
            {
                referees = value;
            }
        }

        public ObservableCollection<Sudija> GetAllReferees()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Sudija>(db.Sudijas.ToList());
            }
        }
    }
}

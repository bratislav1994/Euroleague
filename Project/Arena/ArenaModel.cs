using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Project.HelperClasses;
using System.Collections.ObjectModel;

namespace Project.Arena
{
    public class ArenaModel
    {
        private ObservableCollection<Hala> hallsList = null;

        public ArenaModel()
        {
            this.hallsList = this.GetAllHalls();
        }
        
        public ObservableCollection<Hala> HallsList
        {
            get
            {
                return hallsList;
            }

            set
            {
                hallsList = value;
            }
        }

        public ObservableCollection<Hala> GetAllHalls()
        {
            using (var db = new EuroleagueEntities3())
            {
                return new ObservableCollection<Hala>(db.Halas.ToList());
            }
        }
    }
}

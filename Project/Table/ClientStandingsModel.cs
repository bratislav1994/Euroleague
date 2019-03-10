using Project.HelperClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Table
{
    public class ClientStandingsModel : INotifyPropertyChanged
    {
        private BindingList<Standings> tables = null;

        public ClientStandingsModel()
        {
            this.tables = new BindingList<Standings>();
            this.SortTable();
        }

        public void SortTable()
        {
            List<Standings> temp = new List<Standings>();

            using (var db = new EuroleagueEntities3())
            {
                foreach (Klub k in db.Klubs)
                {
                    temp.Add(new Standings() { ClubId = k.ID_KLB, Points = k.POB_KLB * 2 + k.POR_KLB, Number = k.POB_KLB, ClubName = k.NAZ_KLB, WinNumber = k.POB_KLB, LossNumber = k.POR_KLB, Diff = k.KOSRAZ_KLB });
                }
            }

            this.Tables = new BindingList<Standings>(temp.OrderByDescending(x => x.Points).ToList());
            
            for (int i = 0; i < Tables.Count; i++)
            {
                for (int j = 0; j < Tables.Count - 1; j++)
                {
                    if (Tables[j].Points == Tables[j + 1].Points &&
                        Tables[j].Diff < Tables[j + 1].Diff)
                    {
                        Standings s = Tables[j + 1];
                        Tables[j + 1] = Tables[j];
                        Tables[j] = s;
                    }
                }
            }

            for (int i = 0; i < Tables.Count; i++)
            {
                Tables[i].Number = i + 1;
            }
        }

        public BindingList<Standings> Tables
        {
            get
            {
                return tables;
            }

            set
            {
                tables = value;
                this.RaisePropertyChanged("Tables");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));


            }
        }
    }
}

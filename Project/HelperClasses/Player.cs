using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Design;

namespace Project.HelperClasses
{
    public class Player : INotifyPropertyChanged
    {
        private string idPlayer;
        private string name;
        private int pts;
        private int assist;
        private int reb;
        private string idClub;

        public event PropertyChangedEventHandler PropertyChanged;

        public Player()
        {

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
            }
        }

        public int Pts
        {
            get
            {
                return pts;
            }

            set
            {
                pts = value;
                this.RaisePropertyChanged("Pts");
            }
        }

        public int Assist
        {
            get
            {
                return assist;
            }

            set
            {
                assist = value;
                this.RaisePropertyChanged("Assist");
            }
        }

        public int Reb
        {
            get
            {
                return reb;
            }

            set
            {
                reb = value;
                this.RaisePropertyChanged("Reb");
            }
        }

        public string IdPlayer
        {
            get
            {
                return idPlayer;
            }

            set
            {
                idPlayer = value;
            }
        }

        public string IdClub
        {
            get
            {
                return idClub;
            }

            set
            {
                idClub = value;
            }
        }

        private void RaisePropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));

                //using (var db = new EuroleagueEntities3())
                //{
                //    IgracIgra p = db.IgracIgras.Where(x => x.Igrac_LICBR_IGR == this.IdPlayer).FirstOrDefault();

                //    if (p != null)
                //    {
                //        StatistickiPodaci sts = db.StatistickiPodacis.Where(x => x.OZN_STSPOD == p.OZN_STSPOD).FirstOrDefault();

                //        if (sts != null)
                //        {
                //            sts.PTS_STSPOD = this.Pts;
                //            sts.ASS_STSPOD = this.Assist;
                //            sts.SK_STSPOD = this.Reb;

                //            db.Entry(sts).State = System.Data.Entity.EntityState.Modified;
                //            db.SaveChanges();
                //        }
                //    }
                //}
            }
        }
    }
}

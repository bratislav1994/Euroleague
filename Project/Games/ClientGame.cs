using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Games
{
    public class ClientGame
    {
        public bool CheckPtsConstraints(int pts, string playerId, string idGame, string clubId)
        {
            using (var db = new EuroleagueEntities3())
            {
                var selectedPlayers = (from t1 in db.Igracs
                                       join t2 in db.Klubs on t1.Klub_ID_KLB equals t2.ID_KLB
                                       join t3 in db.IgracIgras on t1.LICBR_IGR equals t3.Igrac_LICBR_IGR
                                       where t3.Utakmica_OZN_UTK == idGame && t2.ID_KLB == clubId &&
                                       t1.LICBR_IGR != playerId
                                       select t3 ).ToList();

                Utakmica game = db.Utakmicas.Where(x => x.OZN_UTK.Equals(idGame)).FirstOrDefault();
                if (game.DOMPOENI_UTK == null || game.GOSTPOENI_UTK == null)
                {
                    return false;
                }
                int ptsTeam = game.Klub_ID_KLB.Equals(clubId) ? (int)game.DOMPOENI_UTK : (int)game.GOSTPOENI_UTK;             
                int sumPts = selectedPlayers.Sum(x => db.IgracIgras.Where(y => y.Utakmica_OZN_UTK.Equals(x.Utakmica_OZN_UTK) && y.Igrac_LICBR_IGR.Equals(x.Igrac_LICBR_IGR)).FirstOrDefault().POENI_IGRACIGRA);
                return ptsTeam >= (sumPts + pts);
            }            
        }

        public bool CheckAsConstraints(int asist, string playerId, string idGame, string clubId)
        {
            using (var db = new EuroleagueEntities3())
            {
                var selectedPlayers = (from t1 in db.Igracs
                                       join t2 in db.Klubs on t1.Klub_ID_KLB equals t2.ID_KLB
                                       join t3 in db.IgracIgras on t1.LICBR_IGR equals t3.Igrac_LICBR_IGR
                                       where t3.Utakmica_OZN_UTK == idGame && t2.ID_KLB == clubId &&
                                       t1.LICBR_IGR != playerId
                                       select t3).ToList();

                Utakmica game = db.Utakmicas.Where(x => x.OZN_UTK.Equals(idGame)).FirstOrDefault();
                if (game.DOMPOENI_UTK == null || game.GOSTPOENI_UTK == null)
                {
                    return false;
                }
                int ptsTeam = game.Klub_ID_KLB.Equals(clubId) ? (int)game.DOMPOENI_UTK : (int)game.GOSTPOENI_UTK;
                int sumAs = selectedPlayers.Sum(x => db.IgracIgras.Where(y => y.Utakmica_OZN_UTK.Equals(x.Utakmica_OZN_UTK) && y.Igrac_LICBR_IGR.Equals(x.Igrac_LICBR_IGR)).FirstOrDefault().AS_IGRACIGRA);
                return (ptsTeam / 2) > (sumAs + asist);
            }
        }
    }
}

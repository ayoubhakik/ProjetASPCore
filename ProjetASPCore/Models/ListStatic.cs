using projetASP.Models;
using ProjetASPCore.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetASPCore.Models
{
    public class ListStatic
    {
        public static List<StaticModel> GetCityPopulationList()
        {
            EtudiantContext db = new EtudiantContext();
            List<Etudiant> list = db.Etudiants.ToList();
            //initialisation des compteurs des filieres
            int info = 0, indus = 0, gtr = 0, gpmc = 0;

            //variable pour les nombre totale et le reste qui n'a pas choisi les filieres
            int nbrTotal = list.Count, nbrReste = 0;

            for (int i = 0; i < nbrTotal; i++)
            {
                if (list[i].Choix == null)
                {
                    //un etudiant avec null dans choix alors on va l'es ajouter dans le reste
                    nbrReste++;
                }
                //sinon on va traiter les choix comme ca
                else
                {
                    if (list[i].Validated)
                    {
                        char[] chiffr = (list[i].Choix).ToCharArray();

                        if (chiffr[0] == 'F')
                        {
                            info++;
                        }
                        if (chiffr[0] == 'P')
                        {
                            gpmc++;
                        }
                        if (chiffr[0] == 'T')
                        {
                            gtr++;
                        }
                        if (chiffr[0] == 'D')
                        {
                            indus++;
                        }
                    }

                }

            }

            //les pourcentages
            //double nbrTotalP = Convert.ToDouble(nbrTotal) / Convert.ToDouble(nbrTotal) * 100;
            //double nbrResteP = Convert.ToDouble(nbrReste) / Convert.ToDouble(nbrTotal) * 100;
            double infoP = Convert.ToDouble(info) / Convert.ToDouble(nbrTotal) * 100;
            double gtrP = Convert.ToDouble(gtr) / Convert.ToDouble(nbrTotal) * 100;
            double gpmcP = Convert.ToDouble(gpmc) / Convert.ToDouble(nbrTotal) * 100;
            double indusP = Convert.ToDouble(indus) / Convert.ToDouble(nbrTotal) * 100; 

            var list1 = new List<StaticModel>();
          
            list1.Add(new StaticModel {Filiere = "Info", nbr_stud = infoP });
            list1.Add(new StaticModel { Filiere = "GTR", nbr_stud = gtrP});
            list1.Add(new StaticModel { Filiere = "GPMC", nbr_stud = gpmcP });
            list1.Add(new StaticModel { Filiere = "Indus", nbr_stud = indusP});
           

            return list1;

        }
    }
}

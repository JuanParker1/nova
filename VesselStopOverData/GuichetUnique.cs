using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
   public class GuichetUnique
    {
        VSOMClassesDataContext dc = new VSOMClassesDataContext();
        VSOMAccessors vs = new VSOMAccessors();
        VsomParameters vsparam = new VsomParameters();
        public string Directory = @"G:\\gu_files";

        public string Navire()
        {
            List<NAVIRE> lst = vsparam.GetNaviresActifs();
            string path=string.Format("{0}\\{1}",Directory,"socomar_navire.txt");
            FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            foreach (NAVIRE nvr in lst)
            {
                writer.WriteLine(string.Format("{0}|{1}",nvr.CodeNav,nvr.NomNav));
            }

            writer.Close();
            return path;
        }

        public string Armateur()
        {
            List<ARMATEUR> lst = vsparam.GetArmateursActifs();
            string path=string.Format("{0}\\{1}",Directory,"socomar_armateur.txt");
            FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            foreach (ARMATEUR item in lst)
            {
                writer.WriteLine(string.Format("{0}|{1}",item.CodeArm,item.NomArm));
            }

            writer.Close();
            return path;
        }

        public string Acconier()
        {
            List<ACCONIER> lst = vsparam.GetAcconiersActifs();
            string path = string.Format("{0}\\{1}", Directory, "socomar_acconier.txt");
            FileStream fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            foreach (ACCONIER item in lst)
            {
                writer.WriteLine(string.Format("{0}|{1}", item.CodeAcc, item.NomAcc));
            }

            writer.Close();
            return path;
        }
       /// <summary>
       /// genère les fichiers de requests et met a jour la table requeste sur les ligne traité
       /// </summary>
        public void BuildRequest()
        {
            try
            {
                string path = string.Empty; FileStream fs1;
                var req = (from m in dc.GetTable<REQUESTS>() where m.ISTREATED == false select m).ToList<REQUESTS>();
                foreach (REQUESTS item in req)
                {

                    // var reqdetail = (from d in dc.GetTable<REQUESTS_DETAILS>() where d.IDREQUEST == item.IDAUTO select d).ToList<REQUESTS_DETAILS>();

                    var reqdetail = item.REQUESTS_DETAILS;

                    if (reqdetail.Count != 0)
                    {
                        var elt = (from r in dc.GetTable<REQUESTS_DETAILS>() select r).First<REQUESTS_DETAILS>();



                        if (item.TYPE == "validationman")
                        {
                            path = string.Format("{0}\\{1}_{2}-{3}-{4}-{5}-{6}h{7}mn{8}s.txt", Directory, item.TYPE, elt.ESC_NUM_MANIF, DateTime.Today.Year,
                            DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(fs1);

                            foreach (REQUESTS_DETAILS rq in reqdetail)
                            {
                                writer.WriteLine(string.Format("{0}|{1} |{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18}|{19}|{20}|{21}|{22}",
                                rq.VHL_CHASSIS, rq.VHL_NUM_BL, rq.VHL_DESC, rq.VHL_VOL, rq.VHL_LONGUEUR, rq.VHL_LARG, rq.VHL_HAUT,
                                rq.VHL_POIDS, rq.VHL_PORTE, rq.VHL_ATELE, " ", "Entree", rq.VHL_TYPE, rq.VHL_CONSIGNEE, rq.ESC_NUM_MANIF, rq.ESC_NUM_VOY,
                                rq.ESC_DATEARR_REEL, rq.ESC_DATEDEP_REEL, rq.ESC_ARMATEUR_CODE, rq.ESC_ACONIER_CODE, rq.ESC_CODE_NAVIRE, " ", rq.ESC_PORT_EMBA,
                                rq.ESC_DATEDEP_PREV, rq.ARMATEUR_NOM, rq.ACCONIER_NOM, rq.NAVIRE_NOM));
                            }

                            writer.Close();

                            //item.ISTREATED = true;
                            //dc.SubmitChanges();

                        }

                        if (item.TYPE == "accomplissement")
                        {

                            path = string.Format("{0}\\{1}_{2}-{3}-{4}-{5}-{6}h{7}mn{8}s.txt", Directory, item.TYPE, elt.VHL_CHASSIS, DateTime.Today.Year,
                           DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(fs1);
                            foreach (REQUESTS_DETAILS rq in reqdetail)
                            {
                                writer.WriteLine(string.Format("{0}|{1} |{2}|{3}", rq.VHL_CHASSIS, rq.VHL_ENLEVEUR_NOM, rq.VHL_ENLEVEUR_CNI, rq.VHL_ENLEVEUR_TEL));
                            }
                            writer.Close();

                            /*var _item = (from _m in dc.GetTable<REQUESTS>() where _m.IDAUTO == item.IDAUTO select _m).First<REQUESTS>();
                            _item.ISTREATED = true;
                            dc.SubmitChanges();*/

                        }

                        if (item.TYPE == "sortievhl")
                        {

                            path = string.Format("{0}\\{1}_{2}-{3}-{4}-{5}-{6}h{7}mn{8}s.txt", Directory, item.TYPE, elt.VHL_CHASSIS, DateTime.Today.Year,
                           DateTime.Today.Month, DateTime.Today.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
                            fs1 = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                            StreamWriter writer = new StreamWriter(fs1);
                            foreach (REQUESTS_DETAILS rq in reqdetail)
                            {
                                writer.WriteLine(string.Format("{0}|{1} |{2}|{3}", rq.VHL_CHASSIS, rq.VHL_NUM_BL, rq.VHL_DAT_S, rq.VHL_BS_REF, rq.VHL_BS_DATE));
                            }
                            writer.Close();

                            /*var _item = (from _m in dc.GetTable<REQUESTS>() where _m.IDAUTO == item.IDAUTO select _m).First<REQUESTS>();
                            _item.ISTREATED = true;
                            dc.SubmitChanges();*/
                            // dc.SubmitChanges(); Vhl_Chassis | Vhl_Num_Bl  | Vhl_Dat_S|Vhl_Bs_Ref|Vhl_Bs_Date
                        }


                        var matched = (from esc in dc.GetTable<REQUESTS>()
                                       where esc.IDAUTO == item.IDAUTO
                                       select esc).FirstOrDefault<REQUESTS>();

                        matched.ISTREATED = true;
                        dc.SubmitChanges();

                    }
                }
            }
            catch (Exception ex)
            { 
            //TODO: log dans windows event
            }
        }

       /// <summary>
       /// recupère le fichier de la douane et lintègre ds le systeme               
       /// </summary>
       /// <param name="path"></param>
        public void ReadRequest(string path)
        { 
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line); // Add to list.
                    
                }
            }

            if (list.Count > 0)
            {
                List<REQUESTS_DETAILS> lrd = new List<REQUESTS_DETAILS>();
                string s = string.Empty;
                for (int i=1; i<list.Count;i++)
                {
                 /*   BUREAU|VOYAGE|DATE_ARRIVEE|N_MANIF|AN_REG|DATE_REG|LIEU_DEPART|LIEU_DESTINATION|
                    CODE_TRANSPORTEUR|TRANSP|NATIONALITE_NAVIRE|NAVIRE|NBRE_BL|NBRE_COLIS_BL|TON_BRUT|
                    POIDS_BRUT_BL|TON_NET|DATE_DECHARGEMENT|BUREAU_DEST|N_LIGNE_BL|NUMERO_BL|POIDS_MANIFESTE|
                    COLIS_MANIFESTE|LOCALISATION
                    */
                    s = list[i];
                    string[] line = s.Split('|');
                    lrd.Add(new REQUESTS_DETAILS
                    {
                         
                    });
                }
            }
        }
    }
}

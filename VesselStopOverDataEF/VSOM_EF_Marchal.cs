using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VesselStopOverDataEF
{
   public class VSOM_EF_Marchal
    {
        public  BON_ENLEVEMENT InsertBonEnlevement(int idBL, string emetteurLettre, string destinataireLettre, DateTime dateEmissionLettre, DateTime dateValiditeLettre, List<VEHICULE> vehs, string autresInfos, int idUser)
        {
            using (var transaction = new NAVYEntities1())
            {
                transaction.Database.Log = Logger.WriteLog; 

                BON_ENLEVEMENT bonEnlevement = new BON_ENLEVEMENT();

                var matchedConnaissement =transaction.CONNAISSEMENT.Where(s=>s.IdBL==idBL).SingleOrDefault<CONNAISSEMENT>();

                  
                //List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                //if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Bon à enlever : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                //{
                //    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un BAE. Veuillez contacter un administrateur");
                //}

                //if (matchedConnaissement == null)
                //{
                //    throw new EnregistrementInexistant("Connaissement Inexistant");
                //}
                ESCALE esc = transaction.ESCALE.Where(s => s.IdEsc == matchedConnaissement.IdEsc).SingleOrDefault<ESCALE>();

                if (!transaction.OPERATION_CONNAISSEMENT.SingleOrDefault<OPERATION_CONNAISSEMENT>(op => op.IdTypeOp == 36 && op.IdBL == matchedConnaissement.IdBL).DateOp.HasValue && esc.IdAcc == 1)
                {
                    throw new ApplicationException("Enregistrement du BAE impossible : Ce connaissement n'a pas encore été accompli");
                }

                bonEnlevement.DateBAE = DateTime.Now;
                bonEnlevement.ELGBAE = emetteurLettre;
                bonEnlevement.DLGBAE = destinataireLettre;
                bonEnlevement.DELGBAE = dateEmissionLettre;
                bonEnlevement.DFLGBAE = dateValiditeLettre;
                bonEnlevement.CONNAISSEMENT = matchedConnaissement;
                bonEnlevement.AIBAE = autresInfos;
                bonEnlevement.IdU = idUser;

                transaction.SaveChanges();//.SubmitChanges();

                foreach (VEHICULE veh in vehs)
                {
                    var matchedVeh = transaction.VEHICULE.Where(s => s.IdVeh == veh.IdVeh).SingleOrDefault<VEHICULE>(); 

                    if (matchedVeh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce véhicule n'a pas encore été identifié");
                    }

                    if (matchedVeh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 3) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce véhicule n'a pas encore été réceptionné");
                    }

                    if (matchedVeh.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce véhicule fait déjà l'objet d'un BAE.");
                    }

                    matchedVeh.StatVeh = "Enlèvement";
                    matchedVeh.IdBAE = bonEnlevement.IdBAE;
                    //matchedVeh.FSVeh = bonEnlevement.DateBAE;

                    OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                    opVeh.IdVeh = matchedVeh.IdVeh;
                    opVeh.IdTypeOp = 4;
                    opVeh.DateOp = DateTime.Now;
                    opVeh.IdU = idUser;
                    opVeh.IdLieu = 3;
                    opVeh.AIOp = "Enlèvement en cours";

                    transaction.OPERATION_VEHICULE.Add(opVeh);
                    //dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                }

                transaction.SaveChanges();

             /*

                foreach (MAFI mafi in mfs)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == mafi.IdMafi
                                       select m).SingleOrDefault<MAFI>();

                    if (matchedMafi.OPERATION_MAFI.Count(op => op.IdTypeOp == 226) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce mafi n'a pas encore été identifié");
                    }

                    if (matchedMafi.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce mafi fait déjà l'objet d'un BAE.");
                    }

                    matchedMafi.StatMafi = "Enlèvement";
                    matchedMafi.IdBAE = bonEnlevement.IdBAE;

                    OPERATION_MAFI opMafi = new OPERATION_MAFI();
                    opMafi.IdMafi = matchedMafi.IdMafi;
                    opMafi.IdTypeOp = 228;
                    opMafi.DateOp = DateTime.Now;
                    opMafi.IdU = idUser;
                    opMafi.AIOp = "Enlèvement en cours";

                    dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);
                }

                dcMar.SubmitChanges();
                */


                OPERATION_CONNAISSEMENT matchedOpBL = transaction.OPERATION_CONNAISSEMENT.Where(s => s.IdBL == matchedConnaissement.IdBL && s.IdTypeOp == 39).SingleOrDefault<OPERATION_CONNAISSEMENT>();
                 
                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Enlèvement en cours";
                }

                OPERATION_MANIFESTE matchedOpMan = transaction.OPERATION_MANIFESTE.Where(s => s.IdMan == matchedConnaissement.IdMan && s.IdTypeOp == 47).SingleOrDefault<OPERATION_MANIFESTE>();
                
                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Enlèvement en cours";
                }

                //OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                //                                 where op.IdEsc == matchedConnaissement.IdEsc && op.IdTypeOp == 55
                //                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                //if (!matchedOpEsc.DateOp.HasValue)
                //{
                //    matchedOpEsc.DateOp = DateTime.Now;
                //    matchedOpEsc.IdU = idUser;
                //    matchedOpEsc.AIOp = "Enlèvement en cours";
                //}

                //dcMar.SubmitChanges();
                //transaction.Complete();
                transaction.SaveChanges();
                return bonEnlevement;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// cette classe represente l'ensemble des opération ne fesant pas intervenir la facturation
    /// </summary>
    public class VsomMarchal : VsomParameters
    {
       VSOMClassesDataContext dcMar;// = new VSOMClassesDataContext();

       //VsomParameters vsp = new VsomParameters();

       public VsomMarchal(VSOMClassesDataContext dc)
            : base(dc)
        {
            dcMar = dc;
        }

       public VsomMarchal()
           : base()
       {
           dcMar = new VSOMClassesDataContext();
       }

        #region escale + SOP

       public ESCALE UpdateNumEscale(int idEsc, int numEscale, int idUser)
       {
           using (var transaction = new System.Transactions.TransactionScope())
           {
               var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                    where esc.IdEsc == idEsc
                                    select esc).SingleOrDefault<ESCALE>();

               var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                  where u.IdU == idUser
                                  select u).SingleOrDefault<UTILISATEUR>();

               if (matchedUser == null)
               {
                   throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
               }

               if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Escale : Mise à jour numéro d'escale").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
               {
                   throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour le numéro d'escale. Veuillez contacter un administrateur");
               }

               if (matchedEscale == null)
               {
                   throw new EnregistrementInexistant("Escale inexistante");
               }

               matchedEscale.NumEsc = numEscale;

               dcMar.SubmitChanges();
               transaction.Complete();
               return matchedEscale;
           }
       }


       /// <summary>
       /// MAJ SOP
       /// </summary>
       /// <param name="idEsc">id escale</param>
       /// <param name="listeOpArm">liste operation armateur</param>
       /// <param name="idUser">id user</param>
       /// <returns></returns>
       public ESCALE UpdateSummaryOperations(int idEsc, List<ElementLigneOpArm> listeOpArm, int idUser)
       {
           using (var transaction = new System.Transactions.TransactionScope())
           {
               var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                    where esc.IdEsc == idEsc
                                    select esc).FirstOrDefault<ESCALE>();

               var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                  where u.IdU == idUser
                                  select u).FirstOrDefault<UTILISATEUR>();

               if (matchedUser == null)
               {
                   throw new EnregistrementInexistant("Utilisateur inexistant");
               }

               List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

               if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Escale : Mise à jour Summary of Operations").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
               {
                   throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour le summary of operations. Veuillez contacter un administrateur");
               }

               if (matchedEscale == null)
               {
                   throw new EnregistrementInexistant("Escale inexistante");
               }

               if (!matchedEscale.DRAEsc.HasValue)
               {
                   throw new ApplicationException("Echec de la mise à jour : Le navire n'est pas encore arrivé");
               }

               if (matchedEscale.SOP == "C")
               {
                   throw new ApplicationException("Echec de la mise à jour : Le summary of operations est déjà clôturé sur cette escale");
               }

               foreach (ElementLigneOpArm opArm in listeOpArm)
               {
                   var matchedOpArm = (from op in dcMar.GetTable<OPERATION_ARMATEUR>()
                                       where op.IdEsc == idEsc && op.IdTypeOp == opArm.IdTypeOp
                                       select op).FirstOrDefault<OPERATION_ARMATEUR>();

                   if (matchedOpArm != null)
                   {
                       matchedOpArm.QTE = opArm.Qte;
                       matchedOpArm.Poids = opArm.Poids;
                       matchedOpArm.Volume = opArm.Volume;
                   }
               }

               dcMar.SubmitChanges();
               transaction.Complete();
               return matchedEscale;
           }
       }


       public ESCALE CloturerEscale(int idEsc, string noteCloture, int idUser)
       {
           using (var transaction = new System.Transactions.TransactionScope())
           {
               // Vérification de l'existance des enregistrements pour contrainte d'intégrité
               var matchedEsc = (from esc in dcMar.GetTable<ESCALE>()
                                 where esc.IdEsc == idEsc
                                 select esc).SingleOrDefault<ESCALE>();

               if (matchedEsc == null)
               {
                   throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
               }

               var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                  where u.IdU == idUser
                                  select u).FirstOrDefault<UTILISATEUR>();

               if (matchedUser == null)
               {
                   throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
               }

               if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Escale : Clôture d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
               {
                   throw new HabilitationException("Vous n'avez pas les droits nécessaires pour clôturer un connaissement. Veuillez contacter un administrateur");
               }

               if (matchedEsc.CONNAISSEMENT.Count(bl => !bl.DVBL.HasValue) == 0)
               {
                   throw new ConnaissementException("Clôture impossible : Cette escale ne contient aucun connaissement validé");
               }

               //Date actuelle du système
               DateTime dte = DateTime.Now;

               OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                where op.IdEsc == matchedEsc.IdEsc && op.IdTypeOp == 58
                                                select op).SingleOrDefault<OPERATION_ESCALE>();

               matchedOpEsc.DateOp = DateTime.Now;
               matchedOpEsc.IdU = idUser;
               matchedOpEsc.AIOp = noteCloture;

               dcMar.OPERATION_ESCALE.Context.SubmitChanges();
               matchedEsc.StatEsc = "Clôturé";

               dcMar.SubmitChanges();
               transaction.Complete();
               return matchedEsc;
           }
       }

       #endregion

        #region manifeste : methode de correction

       
        public void CorrectionVolBL()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                foreach (CONNAISSEMENT booking in dcMar.CONNAISSEMENT.Where(book => book.SensBL == "E" && book.CONTENEUR.Count != 0))
                {
                    booking.VolBL = Math.Round((double)booking.CONTENEUR.Sum(ctr => ctr.VolMCtr.Value), 3);
                }
                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionCompteEscale()
        {
           /*** using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.RAEsc == "Y" && esc.DRAEsc.HasValue && esc.IdEsc >= 350
                               select esc).ToList<ESCALE>();

                SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
                string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
                
                foreach (ESCALE esc in listEsc)
                {
                    string result = sapWS.AddGLAccount(sessionID, "47129" + esc.NumEsc.ToString(), "Débours Maritimes - Escale " + esc.NumEsc.ToString());
                }

                sapWS.LogOut(sessionID);
            }
            * **/
        }


        public StringBuilder CorrectionLigneEcriture(List<VesselStopOverData.SocSAPWS.LigneJournal> listLignes)
        {
            SAPDataClassesDataContext dcSAP = new SAPDataClassesDataContext();

            var listLignesSAP = dcSAP.JDT1.Where(j => j.TransCode == "001");

            StringBuilder sb = new StringBuilder();

            foreach (VesselStopOverData.SocSAPWS.LigneJournal lg in listLignes)
            {
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    var matchedLigne = listLignesSAP.FirstOrDefault<JDT1>(j => j.Ref1 == lg.Reference1 && j.Line_ID == Convert.ToInt32(lg.ShortName));

                    matchedLigne.LineMemo = lg.LineMemo;

                    dcSAP.SubmitChanges();
                    transaction.Complete();
                }

                sb.Append("Batch : ").Append(lg.ControlAccount.ToString()).Append(" - Pièce : ").Append(lg.Reference1).Append(" - Line ID : ").Append(lg.ShortName).Append(Environment.NewLine);
            }
            return sb;
        }

        public void CorrectionGesparc()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {

                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.NumEsc == 2542
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    //esc.StatEsc = "En cours";
                    //esc.NbrePManEsc = 0;
                    //esc.RGPEsc = "Y";
                    //esc.AIEsc = "";

                    ////Statuts de traitement de l'escale
                    //List<TYPE_OPERATION> typeOpEsc = (from type in dcMar.GetTable<TYPE_OPERATION>()
                    //                                  where type.EltTypeOp == "E"
                    //                                  select type).ToList<TYPE_OPERATION>();

                    //foreach (TYPE_OPERATION typeOp in typeOpEsc)
                    //{
                    //    OPERATION_ESCALE opEsc = new OPERATION_ESCALE();
                    //    opEsc.IdEsc = esc.IdEsc;
                    //    opEsc.IdTypeOp = typeOp.IdTypeOp;
                    //    dcMar.OPERATION_ESCALE.InsertOnSubmit(opEsc);
                    //}
                    //dcMar.SubmitChanges();

                    //List<TYPE_OPERATION> typeOpArm = (from type in dcMar.GetTable<TYPE_OPERATION>()
                    //                                  where type.EltTypeOp == "A"
                    //                                  select type).ToList<TYPE_OPERATION>();

                    //foreach (TYPE_OPERATION typeOp in typeOpArm)
                    //{
                    //    OPERATION_ARMATEUR opArm = new OPERATION_ARMATEUR();
                    //    opArm.IdEsc = esc.IdEsc;
                    //    opArm.IdTypeOp = typeOp.IdTypeOp;
                    //    dcMar.OPERATION_ARMATEUR.InsertOnSubmit(opArm);
                    //}
                    //dcMar.SubmitChanges();

                    foreach (MANIFESTE man in esc.MANIFESTE)
                    {
                        ////Statuts de traitement du Manifeste
                        //List<TYPE_OPERATION> typeOpMan = (from type in dcMar.GetTable<TYPE_OPERATION>()
                        //                                  where type.EltTypeOp == "M"
                        //                                  select type).ToList<TYPE_OPERATION>();

                        //foreach (TYPE_OPERATION typeOp in typeOpMan)
                        //{
                        //    OPERATION_MANIFESTE opMan = new OPERATION_MANIFESTE();
                        //    opMan.IdMan = man.IdMan;
                        //    opMan.IdTypeOp = typeOp.IdTypeOp;
                        //    dcMar.OPERATION_MANIFESTE.InsertOnSubmit(opMan);
                        //}

                        //man.NPBLMan = 0;
                        //man.NPCMan = 0;
                        //man.NPGCMan = 0;
                        //man.NPVMan = 0;
                        //man.AIMan = "";
                        //man.FormatMan = 0;
                        //man.CAFMan = "";

                        //Insertion des éléments de éléments du manifeste
                        if (man.IdMan == 429)
                        {
                            foreach (CONNAISSEMENT bl in man.CONNAISSEMENT)
                            {
                                if (bl.BLIL == "Y")
                                {
                                    bl.CodeTVA = "TVAEX";
                                }
                                else
                                {
                                    bl.CodeTVA = "TVAAP";
                                }

                                bl.DVBL = DateTime.Now;
                                bl.DateAccBL = DateTime.Now;
                                bl.BLDette = "N";
                                bl.StatutBL = "Accompli";

                                //Statuts de traitement du BL
                                List<TYPE_OPERATION> typeOpBL = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                                 where type.EltTypeOp == "BL"
                                                                 select type).ToList<TYPE_OPERATION>();

                                foreach (TYPE_OPERATION typeOp in typeOpBL)
                                {
                                    OPERATION_CONNAISSEMENT opBL = new OPERATION_CONNAISSEMENT();
                                    opBL.IdBL = bl.IdBL;
                                    opBL.IdTypeOp = typeOp.IdTypeOp;
                                    if (opBL.IdTypeOp == 33)
                                    {
                                        opBL.DateOp = DateTime.Now;
                                        opBL.IdU = 1;
                                    }
                                    if (opBL.IdTypeOp == 34)
                                    {
                                        opBL.DateOp = DateTime.Now;
                                        opBL.IdU = 1;
                                        opBL.AIOp = "Traité";
                                    }
                                    if (opBL.IdTypeOp == 35)
                                    {
                                        opBL.DateOp = DateTime.Now;
                                        opBL.IdU = 1;
                                        opBL.AIOp = "";
                                    }
                                    if (opBL.IdTypeOp == 36)
                                    {
                                        opBL.DateOp = DateTime.Now;
                                        opBL.IdU = 1;
                                        opBL.AIOp = "";
                                    }
                                    dcMar.OPERATION_CONNAISSEMENT.InsertOnSubmit(opBL);
                                }

                                //foreach (VEHICULE veh in bl.VEHICULE)
                                //{
                                //    if (veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) == 0)
                                //    {
                                //        OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                                //        opVeh.IdVeh = veh.IdVeh;
                                //        opVeh.IdTypeOp = 1;
                                //        opVeh.DateOp = DateTime.Now;
                                //        opVeh.IdU = 1;
                                //        opVeh.IdLieu = 3;
                                //        opVeh.AIOp = "Identifié";
                                //        dcMar.OPERATION_VEHICULE.InsertOnSubmit(opVeh);
                                //    }
                                //    //veh.StatVeh = "Manifesté";
                                //}
                            }
                        }

                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public StringBuilder ImportationSAP(List<EcritureJournal> listEcritures)
        {
            StringBuilder sb = new StringBuilder();

            /**** 
             * SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
             
            foreach (EcritureJournal ec in listEcritures)
            {
                string idJE = sapWS.AddJournalEntryAS400(sessionID, ec.refDate, ec.taxDate, ec.dueDate, ec.memo, ec.ref1, ec.ref2, ec.ref3, ec.lignes.ToArray());

                sb.Append("Batch : ").Append(ec.batchNum.ToString()).Append(" - ").Append(idJE).Append(Environment.NewLine);

            }
            sapWS.LogOut(sessionID);**
             */

            return sb;
        }

        public void CorrectionAgencyFees()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.RAEsc == "Y" && esc.DRAEsc.HasValue
                               select esc).ToList<ESCALE>();

                // Les lignes de facturation armateur sont calculées pour les escales dont Socomar est acconier

                foreach (ESCALE esc in listEsc)
                {
                    // Mise à jour de l'élément de facturation armateur Agency Fees
                    DateTime dte = DateTime.Now;

                    ARTICLE articleAgencyFees = (from art in dcMar.GetTable<ARTICLE>()
                                                 from par in dcMar.GetTable<PARAMETRE>()
                                                 where art.CodeArticle == par.CodeAF && par.NomAF == "Agency fees"
                                                 select art).FirstOrDefault<ARTICLE>();

                    LIGNE_PRIX lpAgencyFees = articleAgencyFees.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                    var matchedEltAgencyFees = (from ef in dcMar.GetTable<ELEMENT_FACTURATION>()
                                                where ef.IdEsc == esc.IdEsc && ef.LibEF.Contains("Agency fees - Escale")
                                                select ef).FirstOrDefault<ELEMENT_FACTURATION>();

                    if (matchedEltAgencyFees != null)
                    {
                        matchedEltAgencyFees.DestEF = "A";
                        matchedEltAgencyFees.DateJEF = DateTime.Now;
                        matchedEltAgencyFees.CCArticle = articleAgencyFees.CCArticle;
                        matchedEltAgencyFees.CCCP = esc.ARMATEUR.CCArm;
                        matchedEltAgencyFees.EltFacture = "Arm";
                        matchedEltAgencyFees.LibEF = "Agency fees - Escale " + esc.NumEsc;
                        matchedEltAgencyFees.IdLP = lpAgencyFees.IdLP;
                        matchedEltAgencyFees.PUEF = lpAgencyFees.PU1LP;
                        matchedEltAgencyFees.QTEEF = 1;
                        matchedEltAgencyFees.UnitEF = "U";
                        matchedEltAgencyFees.CodeTVA = "TVAAP";
                        matchedEltAgencyFees.TauxTVA = dcMar.CODE_TVA.FirstOrDefault<CODE_TVA>(code => code.CodeTVA == "TVAAP").TauxTVA;

                        dcMar.ELEMENT_FACTURATION.Context.SubmitChanges();
                    }
                    else
                    {
                        ELEMENT_FACTURATION eltAgencyFees = new ELEMENT_FACTURATION();

                        eltAgencyFees.DestEF = "A";
                        eltAgencyFees.StatutEF = "En cours";
                        eltAgencyFees.DateJEF = DateTime.Now;
                        eltAgencyFees.CCArticle = articleAgencyFees.CCArticle;
                        eltAgencyFees.CCCP = esc.ARMATEUR.CCArm;
                        eltAgencyFees.EltFacture = "Arm";
                        eltAgencyFees.LibEF = "Agency fees - Escale " + esc.NumEsc;
                        eltAgencyFees.IdLP = lpAgencyFees.IdLP;
                        eltAgencyFees.PUEF = lpAgencyFees.PU1LP;
                        eltAgencyFees.IdEsc = esc.IdEsc;
                        eltAgencyFees.QTEEF = 1;
                        eltAgencyFees.UnitEF = "U";
                        eltAgencyFees.CodeTVA = "TVAAP";
                        eltAgencyFees.TauxTVA = dcMar.CODE_TVA.FirstOrDefault<CODE_TVA>(code => code.CodeTVA == "TVAAP").TauxTVA;

                        dcMar.ELEMENT_FACTURATION.InsertOnSubmit(eltAgencyFees);
                        dcMar.ELEMENT_FACTURATION.Context.SubmitChanges();
                    }
                }
                
                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public int CorrectionParc30000()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listConteneurs = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                      where ctr.StatutTC == "Parqué" && ctr.IsDoublon == "N" && ctr.CONTENEUR.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0
                                      orderby ctr.NumTC ascending
                                      select ctr).ToList<CONTENEUR_TC>();

                foreach (CONTENEUR_TC ctrTC in listConteneurs)
                {
                    //if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "En habillage" && matchedConteneurTC.StatutTC != "En réparation")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est ni retourné, ni présent dans le parc");
                    //}

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = ctrTC.IdCtr;
                    opCtr.IdTypeOp = 278;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = 1;
                    opCtr.AIOp = "Reprise";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    ctrTC.CONTENEUR.StatCtr = "Parqué";

                    ctrTC.StatutTC = "Parqué";
                    ctrTC.DateParquing = DateTime.Now;
                    ctrTC.IdUserParquing = 1;
                    ctrTC.IdParcParquing = 5;
                    ctrTC.IdEmplacementParc = 2668;
                    ctrTC.EtatRetourVide = "General Cargo Unchecked";

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = ctrTC.CONTENEUR.IdBL;
                    mvtTC.IdEsc = ctrTC.CONTENEUR.IdEsc;
                    mvtTC.IdParc = 5;
                    mvtTC.IdTC = ctrTC.IdTC;
                    mvtTC.IdTypeOp = 278;
                    mvtTC.IdUser = 1;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    dcMar.SubmitChanges();

                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdCtr = ctrTC.IdCtr;
                    occupation.IdEmpl = 2668;
                    occupation.IdTypeOp = 278;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                    dcMar.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return listConteneurs.Count;
            }
        }

        public int CorrectionSortieConteneur()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listConteneurs = (from ctr in dcMar.GetTable<CONTENEUR>()
                                      where !ctr.DSCtr.HasValue && ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(op => op.IdTypeOp == 18) != null
                                      select ctr).ToList<CONTENEUR>();

                foreach (CONTENEUR ctr in listConteneurs)
                {
                    ctr.DSCtr = ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(op => op.IdTypeOp == 18).DateOp;
                    dcMar.SubmitChanges();
                }
                dcMar.SubmitChanges();
                transaction.Complete();
                return listConteneurs.Count;
            }
        }

        public int CorrectionStatutConteneur()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listConteneurs = (from ctr in dcMar.GetTable<CONTENEUR>()
                                      where ctr.OPERATION_CONTENEUR.Count > 0
                                      select ctr).ToList<CONTENEUR>();
                int i = 0;
                foreach (CONTENEUR ctr in listConteneurs)
                {
                    ctr.StatCtr = ctr.OPERATION_CONTENEUR.OrderByDescending(o => o.IdTypeOp).First().TYPE_OPERATION.Statut;
                    i++;
                    dcMar.SubmitChanges();
                }
                transaction.Complete();
                return i;
            }
        }

        public int CorrectionImportDIT()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listMvt = (from mvt in dcMar.GetTable<MOUVEMENT_TC>()
                                      where mvt.IdUser == 127
                                      select mvt).ToList<MOUVEMENT_TC>();
                int i = 0;
                foreach (MOUVEMENT_TC mvt in listMvt)
                {
                    mvt.IdEsc = mvt.CONNAISSEMENT.IdEsc;
                    i++;
                    dcMar.SubmitChanges();
                }
                transaction.Complete();
                return i;
            }
        }

        public int CorrectionDoublon()
        {
            //using (var transaction = new System.Transactions.TransactionScope())
            //{
            //var listConteneur = (from ctrTC in dcMar.GetTable<CONTENEUR_TC>()
            //                     where ctrTC.NumTC.Contains("Doublon")
            //                     select ctrTC).ToList<CONTENEUR_TC>();

            int nbDoublon = 0;

            //foreach (CONTENEUR_TC ctrTC in listConteneur)
            //{
            //    ctrTC.NumTC = ctrTC.NumTC.Replace("-Doublon", "");
            //}

            //dcMar.SubmitChanges();

            //var listEscale = (from esc in dcMar.GetTable<ESCALE>()
            //                  select esc).ToList<ESCALE>();

            //foreach (ESCALE esc in listEscale)
            //{
            //    foreach (CONTENEUR ctr in esc.CONTENEUR.Where(c => c.SensCtr == "I"))
            //    {
            //        //using (var transaction = new System.Transactions.TransactionScope())
            //        //{
            //        //    if (ctr.CONTENEUR_TC.Count > 1)
            //        //    {
            //        //        var listCtrTC = (from ctrTC in ctr.CONTENEUR_TC
            //        //                         orderby ctrTC.MOUVEMENT_TC.Count descending
            //        //                         select ctrTC).ToList<CONTENEUR_TC>();

            //        //        listCtrTC.ElementAt(0).IsDoublon = "N";

            //        //        for (int i = 1; i < listCtrTC.Count; i++)
            //        //        {
            //        //            listCtrTC.ElementAt(i).IsDoublon = "Y";
            //        //            listCtrTC.ElementAt(i).NumTC = listCtrTC.ElementAt(i).NumTC + "-Doublon";
            //        //            nbDoublon++;
            //        //        }
            //        //    }

            //        //    dcMar.SubmitChanges();
            //        //    transaction.Complete();
            //        //}

            //        //var listeCtrTCIdem = (from ctrTC in dcMar.GetTable<CONTENEUR_TC>()
            //        //                      where ctrTC.NumTC == ctr.NumCtr && ctrTC.IdEscDébarquement == ctr.IdEsc
            //        //                      select ctrTC).ToList<CONTENEUR_TC>();

            //        //if (listeCtrTCIdem.Count > 1)
            //        //{
            //        //    using (var transaction = new System.Transactions.TransactionScope())
            //        //    {
            //        //        var listCtrTC = (from ctrTC in listeCtrTCIdem
            //        //                         orderby ctrTC.MOUVEMENT_TC.Count descending
            //        //                         select ctrTC).ToList<CONTENEUR_TC>();

            //        //        listCtrTC.ElementAt(0).IsDoublon = "N";

            //        //        for (int i = 1; i < listCtrTC.Count; i++)
            //        //        {
            //        //            listCtrTC.ElementAt(i).IsDoublon = "Y";
            //        //            listCtrTC.ElementAt(i).NumTC = listCtrTC.ElementAt(i).NumTC + "-Doublon";
            //        //            nbDoublon++;
            //        //        }

            //        //        dcMar.SubmitChanges();
            //        //        transaction.Complete();
            //        //    }
            //        //}
            //    }
            //}

            //var listTC = (from ctrTC in dcMar.GetTable<CONTENEUR_TC>()
            //              where ctrTC.IsDoublon == "Y"
            //              select ctrTC).ToList<CONTENEUR_TC>();

            //foreach (CONTENEUR_TC ctc in listTC)
            //{
            //    var listdoublon = (from ctrTC in dcMar.GetTable<CONTENEUR_TC>()
            //                       where ctrTC.IdEscDébarquement == ctc.IdEscDébarquement && ctrTC.NumTC == ctc.NumTC
            //                       select ctrTC).ToList<CONTENEUR_TC>();

            //    if (listdoublon.Count == 1)
            //    {
            //        listdoublon.FirstOrDefault().IsDoublon = "N";
            //    }
            //    dcMar.SubmitChanges();
            //}
                
                return nbDoublon;
            //}
        }

        public void CorrectionISPS()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.RAEsc == "Y" && esc.DRAEsc.HasValue && esc.IdEsc >= 350
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    // Mise à jour de l'élément de facturation armateur Agency Fees
                    DateTime dte = DateTime.Now;

                    ARTICLE articleAgencyFees = (from art in dcMar.GetTable<ARTICLE>()
                                                 from par in dcMar.GetTable<PARAMETRE>()
                                                 where art.CodeArticle == par.CodeAF && par.NomAF == "ISPS Security fees"
                                                 select art).FirstOrDefault<ARTICLE>();

                    LIGNE_PRIX lpAgencyFees = articleAgencyFees.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                    ELEMENT_FACTURATION eltAgencyFees = new ELEMENT_FACTURATION();

                    eltAgencyFees.DestEF = "A";
                    eltAgencyFees.StatutEF = "En cours";
                    eltAgencyFees.DateJEF = DateTime.Now;
                    eltAgencyFees.CCArticle = articleAgencyFees.CCArticle;
                    eltAgencyFees.CCCP = esc.ARMATEUR.CCArm;
                    eltAgencyFees.EltFacture = "Arm";
                    eltAgencyFees.LibEF = "ISPS Security fees - Escale " + esc.NumEsc;
                    eltAgencyFees.IdLP = lpAgencyFees.IdLP;
                    eltAgencyFees.PUEF = lpAgencyFees.PU1LP;
                    eltAgencyFees.IdEsc = esc.IdEsc;
                    eltAgencyFees.QTEEF = 1;
                    eltAgencyFees.UnitEF = "U";
                    eltAgencyFees.CodeTVA = "TVAAP";
                    eltAgencyFees.TauxTVA = dcMar.CODE_TVA.FirstOrDefault<CODE_TVA>(code => code.CodeTVA == "TVAAP").TauxTVA;

                    dcMar.ELEMENT_FACTURATION.InsertOnSubmit(eltAgencyFees);
                    dcMar.ELEMENT_FACTURATION.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionGestionnaireParcAuto()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                //var listVeh = (from veh in dcMar.GetTable<VEHICULE>()
                //               where (veh.OCCUPATION.Count(occ => !occ.DateFin.HasValue) > 0) || (veh.IdVehAP.HasValue && veh.VEHICULE1.OCCUPATION.Count(occ => !occ.DateFin.HasValue) > 0)
                //               select veh).ToList<VEHICULE>();

                var listVeh = (from veh in dcMar.GetTable<VEHICULE>()
                               where veh.CONNAISSEMENT.DVBL != null && veh.DSRVeh == null && veh.ELEMENT_FACTURATION.Count(el => el.LIGNE_PRIX.CodeArticle == 1820) == 0
                               select veh).ToList<VEHICULE>();

                DateTime dte = DateTime.Now;

                ARTICLE articleGestParcAuto = (from art in dcMar.GetTable<ARTICLE>()
                                               from par in dcMar.GetTable<PARAMETRE>()
                                               where art.CodeArticle == par.CodeAF && par.NomAF == "Prestation de gestionnaire de parc auto"
                                               select art).FirstOrDefault<ARTICLE>();

                LIGNE_PRIX lpGestParcAuto = null;

                LIGNE_PRIX lpGestParcAutoSupplVol = articleGestParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "SP" && lp.DDLP <= dte && lp.DFLP >= dte);

                foreach (VEHICULE veh in listVeh)
                {
                    // Element de gestionnaire parc auto
                    ELEMENT_FACTURATION eltFactGestParcAuto = new ELEMENT_FACTURATION();

                    if (veh.StatutCVeh == "U")
                    {
                        lpGestParcAuto = articleGestParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU" && lp.DDLP <= dte && lp.DFLP >= dte);
                    }
                    else if (veh.StatutCVeh == "N")
                    {
                        lpGestParcAuto = articleGestParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN" && lp.DDLP <= dte && lp.DFLP >= dte);
                    }

                    eltFactGestParcAuto.CCCP = veh.CONNAISSEMENT.CLIENT.CodeClient;
                    eltFactGestParcAuto.EltFacture = "Veh";
                    eltFactGestParcAuto.LibEF = "Prestation de gestionnaire parc auto - Chassis N° " + veh.NumChassis;
                    eltFactGestParcAuto.DateJEF = DateTime.Now;
                    eltFactGestParcAuto.IdLP = lpGestParcAuto.IdLP;
                    eltFactGestParcAuto.QTEEF = 1;
                    if (veh.VolCVeh < 16)
                    {
                        eltFactGestParcAuto.PUEF = lpGestParcAuto.PU1LP;
                    }
                    else if (veh.VolCVeh < 50)
                    {
                        eltFactGestParcAuto.PUEF = lpGestParcAuto.PU2LP;
                    }
                    else
                    {
                        eltFactGestParcAuto.PUEF = lpGestParcAuto.PU3LP;
                    }

                    eltFactGestParcAuto.UnitEF = lpGestParcAuto.UniteLP;
                    eltFactGestParcAuto.IdEsc = veh.CONNAISSEMENT.IdEsc;
                    eltFactGestParcAuto.IdMan = veh.CONNAISSEMENT.IdMan;
                    eltFactGestParcAuto.IdBL = veh.CONNAISSEMENT.IdBL;
                    eltFactGestParcAuto.IdVeh = veh.IdVeh;
                    eltFactGestParcAuto.CodeTVA = veh.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (veh.CONNAISSEMENT.CODE_TVA.CodeTVA == "TVAEX" ? "TVAEX" : veh.CONNAISSEMENT.CLIENT.CodeTVA);
                    eltFactGestParcAuto.TauxTVA = eltFactGestParcAuto.CodeTVA == "TVAEX" ? 0 : veh.CONNAISSEMENT.CODE_TVA.TauxTVA == 0 ? 0 : veh.CONNAISSEMENT.CLIENT.CODE_TVA.TauxTVA;
                    eltFactGestParcAuto.CCArticle = eltFactGestParcAuto.CodeTVA == "TVAEX" ? articleGestParcAuto.CCArticleEx : articleGestParcAuto.CCArticle;
                    eltFactGestParcAuto.DestEF = "C";
                    eltFactGestParcAuto.StatutEF = "En cours";

                    dcMar.GetTable<ELEMENT_FACTURATION>().InsertOnSubmit(eltFactGestParcAuto);

                    // Element de facturation Gestionnaire parc auto supplement volume vehicule
                    ELEMENT_FACTURATION eltFactGestParcAutoSupplementVolumeVehicule = new ELEMENT_FACTURATION();

                    eltFactGestParcAutoSupplementVolumeVehicule.CodeTVA = veh.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (veh.CONNAISSEMENT.CODE_TVA.CodeTVA == "TVAEX" ? "TVAEX" : veh.CONNAISSEMENT.CLIENT.CodeTVA);
                    eltFactGestParcAutoSupplementVolumeVehicule.TauxTVA = eltFactGestParcAutoSupplementVolumeVehicule.CodeTVA == "TVAEX" ? 0 : veh.CONNAISSEMENT.CODE_TVA.TauxTVA == 0 ? 0 : veh.CONNAISSEMENT.CLIENT.CODE_TVA.TauxTVA;
                    eltFactGestParcAutoSupplementVolumeVehicule.CCArticle = eltFactGestParcAutoSupplementVolumeVehicule.CodeTVA == "TVAEX" ? articleGestParcAuto.CCArticleEx : articleGestParcAuto.CCArticle;
                    eltFactGestParcAutoSupplementVolumeVehicule.CCCP = veh.CONNAISSEMENT.CLIENT.CodeClient;
                    eltFactGestParcAutoSupplementVolumeVehicule.EltFacture = "Veh";
                    eltFactGestParcAutoSupplementVolumeVehicule.LibEF = "Prestation de gestionnaire Parc auto - Supplément volume Chassis N° " + veh.NumChassis;
                    eltFactGestParcAutoSupplementVolumeVehicule.DateJEF = DateTime.Now;
                    eltFactGestParcAutoSupplementVolumeVehicule.IdLP = lpGestParcAutoSupplVol.IdLP;
                    eltFactGestParcAutoSupplementVolumeVehicule.QTEEF = veh.VolCVeh > 100 ? veh.VolCVeh - 100 : 0;
                    eltFactGestParcAutoSupplementVolumeVehicule.UnitEF = lpGestParcAutoSupplVol.UniteLP;
                    if (veh.CONNAISSEMENT.BLIL == "Y")
                    {
                        eltFactGestParcAutoSupplementVolumeVehicule.PUEF = lpGestParcAutoSupplVol.PU2LP;
                    }
                    else
                    {
                        if (veh.CONNAISSEMENT.BLGN == "Y")
                        {
                            eltFactGestParcAutoSupplementVolumeVehicule.PUEF = lpGestParcAutoSupplVol.PU2LP;
                        }
                        else
                        {
                            eltFactGestParcAutoSupplementVolumeVehicule.PUEF = lpGestParcAutoSupplVol.PU1LP;
                        }
                    }
                    eltFactGestParcAutoSupplementVolumeVehicule.IdEsc = veh.CONNAISSEMENT.IdEsc;
                    eltFactGestParcAutoSupplementVolumeVehicule.IdMan = veh.CONNAISSEMENT.IdMan;
                    eltFactGestParcAutoSupplementVolumeVehicule.IdBL = veh.CONNAISSEMENT.IdBL;
                    eltFactGestParcAutoSupplementVolumeVehicule.IdVeh = veh.IdVeh;
                    eltFactGestParcAutoSupplementVolumeVehicule.DestEF = "C";
                    eltFactGestParcAutoSupplementVolumeVehicule.StatutEF = "En cours";

                    dcMar.GetTable<ELEMENT_FACTURATION>().InsertOnSubmit(eltFactGestParcAutoSupplementVolumeVehicule);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionReportingNDS()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.RAEsc == "Y" && esc.DRAEsc.HasValue && esc.IdArm == 2
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    // Mise à jour de l'élément de facturation armateur Agency Fees
                    DateTime dte = DateTime.Now;

                    ARTICLE articleCommunication = (from art in dcMar.GetTable<ARTICLE>()
                                                    from par in dcMar.GetTable<PARAMETRE>()
                                                    where art.CodeArticle == par.CodeAF && par.NomAF == "Reporting, communication, courrier services"
                                                    select art).FirstOrDefault<ARTICLE>();

                    LIGNE_PRIX lpCommunication = articleCommunication.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                    ELEMENT_FACTURATION eltCommunication = new ELEMENT_FACTURATION();

                    eltCommunication.DestEF = "A";
                    eltCommunication.StatutEF = "En cours";
                    eltCommunication.DateJEF = DateTime.Now;
                    eltCommunication.CCArticle = articleCommunication.CCArticle;
                    eltCommunication.CCCP = esc.ARMATEUR.CCArm;
                    eltCommunication.EltFacture = "Arm";
                    eltCommunication.LibEF = "Reporting, communication, courrier services - Escale " + esc.NumEsc;
                    eltCommunication.IdLP = lpCommunication.IdLP;
                    eltCommunication.PUEF = lpCommunication.PU1LP;
                    eltCommunication.IdEsc = esc.IdEsc;
                    eltCommunication.QTEEF = 1;
                    eltCommunication.UnitEF = "U";
                    eltCommunication.CodeTVA = "TVAAP";
                    eltCommunication.TauxTVA = dcMar.CODE_TVA.FirstOrDefault<CODE_TVA>(code => code.CodeTVA == "TVAAP").TauxTVA;

                    dcMar.ELEMENT_FACTURATION.InsertOnSubmit(eltCommunication);
                    dcMar.ELEMENT_FACTURATION.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionOperationsArmateur()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.RAEsc == "Y"
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    var matchedOpArmVehU50 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 112
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehU50 != null)
                    {
                        matchedOpArmVehU50.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehU50.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehU50.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmVehU16 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 111
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehU16 != null)
                    {
                        matchedOpArmVehU16.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehU16.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehU16.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmVehU16P = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 110
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehU16P != null)
                    {
                        matchedOpArmVehU16P.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehU16P.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehU16P.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "U" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmVehN50 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 109
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehN50 != null)
                    {
                        matchedOpArmVehN50.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehN50.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehN50.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 50 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmVehN16 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 108
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehN16 != null)
                    {
                        matchedOpArmVehN16.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehN16.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehN16.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh >= 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmVehN16P = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 107
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmVehN16P != null)
                    {
                        matchedOpArmVehN16P.QTE = esc.VEHICULE.Count(veh => veh.StatutCVeh == "N" && veh.VolCVeh < 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0);
                        matchedOpArmVehN16P.Poids = esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh < 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.PoidsCVeh / 1000);
                        matchedOpArmVehN16P.Volume = Convert.ToInt32(esc.VEHICULE.Where(veh => veh.StatutCVeh == "N" && veh.VolCVeh < 16 && veh.OPERATION_VEHICULE.Count<OPERATION_VEHICULE>(v => v.IdTypeOp.Value == 1) > 0).Sum(veh => veh.VolCVeh.Value));
                    }

                    var matchedOpArmCtr101 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                           where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 101
                                           select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmCtr101 != null)
                    {
                        matchedOpArmCtr101.QTE = esc.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0);
                        matchedOpArmCtr101.Poids = esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.PoidsCCtr / 1000);
                        matchedOpArmCtr101.Volume = Convert.ToInt32(esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.VolMCtr.Value));
                    }

                    var matchedOpArmCtr117 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 117
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmCtr117 != null)
                    {
                        matchedOpArmCtr117.QTE = esc.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0);
                        matchedOpArmCtr117.Poids = esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.PoidsCCtr / 1000);
                        matchedOpArmCtr117.Volume = Convert.ToInt32(esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "20" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.VolMCtr.Value));
                    }                    

                    var matchedOpArmCtr102 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 102
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmCtr102 != null)
                    {
                        matchedOpArmCtr102.QTE = esc.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0);
                        matchedOpArmCtr102.Poids = esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.PoidsCCtr / 1000);
                        matchedOpArmCtr102.Volume = Convert.ToInt32(esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.VolMCtr.Value));
                    }
                    
                    var matchedOpArmCtr118 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                              where opArm.IdEsc == esc.IdEsc && opArm.IdTypeOp == 118
                                              select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArmCtr118 != null)
                    {
                        matchedOpArmCtr118.QTE = esc.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0);
                        matchedOpArmCtr118.Poids = esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.PoidsCCtr / 1000);
                        matchedOpArmCtr118.Volume = Convert.ToInt32(esc.CONTENEUR.Where(ctr => ctr.TypeCCtr.Substring(0, 2) == "40" && ctr.OPERATION_CONTENEUR.Count<OPERATION_CONTENEUR>(c => c.IdTypeOp.Value == 12) > 0).Sum(ctr => ctr.VolMCtr.Value));
                    }
                }
                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionStatut()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listVeh = (from veh in dcMar.GetTable<VEHICULE>()
                               from opveh in dcMar.GetTable<OPERATION_VEHICULE>()
                               where veh.IdVeh == opveh.IdVeh && opveh.IdTypeOp == 1 && veh.StatVeh == "Manifesté"
                               select veh).ToList<VEHICULE>();

                foreach (VEHICULE veh in listVeh)
                {
                    veh.StatVeh = "Identifié/Déchargé";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionSummaryNDS()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.IdArm == 2 && esc.RAEsc == "Y"
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    List<TYPE_OPERATION> typeOpArm = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                      where type.EltTypeOp == "A" && type.IdArm == esc.IdArm
                                                      select type).ToList<TYPE_OPERATION>();

                    foreach (TYPE_OPERATION typeOp in typeOpArm)
                    {
                        OPERATION_ARMATEUR opArm = new OPERATION_ARMATEUR();
                        opArm.IdEsc = esc.IdEsc;
                        opArm.IdTypeOp = typeOp.IdTypeOp;
                        dcMar.OPERATION_ARMATEUR.InsertOnSubmit(opArm);
                    }
                    dcMar.OPERATION_ARMATEUR.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionSummaryKARIM()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var listEsc = (from esc in dcMar.GetTable<ESCALE>()
                               where esc.IdArm == 24 && esc.RAEsc == "Y"
                               select esc).ToList<ESCALE>();

                foreach (ESCALE esc in listEsc)
                {
                    List<TYPE_OPERATION> typeOpArm = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                      where type.EltTypeOp == "A" && type.IdArm == esc.IdArm
                                                      select type).ToList<TYPE_OPERATION>();

                    foreach (TYPE_OPERATION typeOp in typeOpArm)
                    {
                        OPERATION_ARMATEUR opArm = new OPERATION_ARMATEUR();
                        opArm.IdEsc = esc.IdEsc;
                        opArm.IdTypeOp = typeOp.IdTypeOp;
                        dcMar.OPERATION_ARMATEUR.InsertOnSubmit(opArm);
                    }
                    dcMar.OPERATION_ARMATEUR.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }

        public void CorrectionInterchange()
        {

            var listCtr = (from ctr in dcMar.GetTable<CONTENEUR>()
                           select ctr).ToList<CONTENEUR>();

            var typesSin = (from type in dcMar.GetTable<TYPE_SINISTRE>()
                            where type.TypeMse == "C"
                            orderby type.IdTypeSinistre ascending
                            select type).ToList<TYPE_SINISTRE>();

            foreach (CONTENEUR ctr in listCtr)
            {
                using (var transaction = new System.Transactions.TransactionScope())
                {
                    foreach (TYPE_SINISTRE sin in typesSin)
                    {
                        if (dcMar.INTERCHANGE.FirstOrDefault<INTERCHANGE>(inter => inter.IdTypeSinistre == sin.IdTypeSinistre && inter.IdCtr == ctr.IdCtr) == null)
                        {
                            INTERCHANGE interchange = new INTERCHANGE();

                            interchange.IdCtr = ctr.IdCtr;
                            interchange.IdTypeSinistre = sin.IdTypeSinistre;

                            dcMar.GetTable<INTERCHANGE>().InsertOnSubmit(interchange);
                        }
                    }
                    dcMar.SubmitChanges();
                    transaction.Complete();
                }
            }            
        }

        public void CorrectionFactures()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {

                foreach (PROFORMA prof in dcMar.PROFORMA)
                {
                    List<ElementFacturation> listElts = GetLignesProf(prof.IdFP);
                    prof.MHT = Convert.ToInt32(listElts.Sum(elt => elt.MontantHT));
                    prof.MTVA = Convert.ToInt32(listElts.Sum(elt => elt.MontantTVA));
                    prof.MTTC = prof.MHT + prof.MTVA;
                }

                dcMar.SubmitChanges();

                foreach (FACTURE fact in dcMar.FACTURE)
                {
                    fact.MHT = fact.PROFORMA.MHT;
                    fact.MTVA = fact.PROFORMA.MTVA;
                    fact.MTTC = fact.PROFORMA.MTTC;
                }

                dcMar.SubmitChanges();

                foreach (PAYEMENT pay in dcMar.PAYEMENT)
                {
                    if (pay.ObjetPay != 2)
                    {
                        pay.MAPay = dcMar.FACTURE.Where(f => f.IdPay == pay.IdPay).Sum(p => p.MTTC);
                    }
                    pay.NumCheque = pay.AIPay == "" ? "" : pay.AIPay;
                    pay.RefVirement = pay.AIPay == "" ? "" : pay.AIPay;
                    pay.Caisse = pay.UTILISATEUR.Caisse;
                }

                dcMar.SubmitChanges();
                transaction.Complete();
            }
        }


        public void CorrectionRefPaiement()
        {
            SAPDataClassesDataContext dcSAP = new SAPDataClassesDataContext();
            foreach (FACTURE fact in dcMar.FACTURE.Where(f => f.IdPay.HasValue && f.PAYEMENT.IdPaySAP == 0).OrderBy(f => f.IdDocSAP))
            {
                try
                {
                    OINV invSAP = dcSAP.OINV.SingleOrDefault<OINV>(inv => inv.DocEntry == fact.IdDocSAP);
                    if (invSAP != null)
                    {
                        fact.PAYEMENT.IdPaySAP = invSAP.ReceiptNum.HasValue ? invSAP.ReceiptNum.Value : 0;
                        dcMar.SubmitChanges();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            
            

            //using (var transaction = new System.Transactions.TransactionScope())
            //{
                
            //    transaction.Complete();
            //}
        }

        public void CorrectionCompteComptables()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                foreach (ELEMENT_FACTURATION elt in dcMar.ELEMENT_FACTURATION.Where(el => el.IdBL.HasValue && el.StatutEF == "En cours"))
                {
                    elt.CodeTVA = elt.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : elt.CodeTVA;
                    elt.TauxTVA = elt.CONNAISSEMENT.BLIL == "Y" ? 0 : elt.TauxTVA;
                    elt.CCArticle = elt.CONNAISSEMENT.BLIL == "Y" ? elt.LIGNE_PRIX.ARTICLE.CCArticleEx : elt.LIGNE_PRIX.ARTICLE.CCArticle;
                }

                dcMar.SubmitChanges();
                transaction.Complete();

                //SAPDataClassesDataContext dcSAP = new SAPDataClassesDataContext();
                //foreach (INV1 inv1 in dcSAP.INV1.Where(i => i.VatGroup == "TVAEX" && i.AcctCode.StartsWith("7")))
                //{
                //    inv1.AcctCode = dcMar.ARTICLE.Single(a => a.CodeArticle.ToString() == inv1.ItemCode).CCArticleEx;
                //    OINV oinv = dcSAP.OINV.Single(i => i.DocEntry == inv1.DocEntry);
                //    foreach (JDT1 jdt1 in dcSAP.JDT1.Where(j => j.TransId == oinv.TransId))
                //    {
                //        if (jdt1.Account.StartsWith("7"))
                //        {
                //            jdt1.Account = dcMar.ARTICLE.FirstOrDefault(a => a.CCArticle == jdt1.Account || a.CCArticleEx == jdt1.Account).CCArticleEx;
                //            jdt1.ShortName = jdt1.Account;
                //        }
                //    }
                //}

                //foreach (INV1 inv1 in dcSAP.INV1.Where(i => i.VatGroup == "TVAAP" && i.AcctCode.StartsWith("7")))
                //{
                //    inv1.AcctCode = dcMar.ARTICLE.Single(a => a.CodeArticle.ToString() == inv1.ItemCode).CCArticle;
                //    OINV oinv = dcSAP.OINV.Single(i => i.DocEntry == inv1.DocEntry);
                //    foreach (JDT1 jdt1 in dcSAP.JDT1.Where(j => j.TransId == oinv.TransId))
                //    {
                //        if (jdt1.Account.StartsWith("7"))
                //        {
                //            jdt1.Account = dcMar.ARTICLE.FirstOrDefault(a => a.CCArticle == jdt1.Account || a.CCArticleEx == jdt1.Account).CCArticle;
                //            jdt1.ShortName = jdt1.Account;
                //        }
                //    }
                //}
                //dcSAP.SubmitChanges();
            }
        }

        public int GetMontantCommande(int docNum)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                SAPDataClassesDataContext dcSAP = new SAPDataClassesDataContext();

                if (dcSAP.OPOR.FirstOrDefault<OPOR>(cde => cde.DocNum == docNum) != null)
                {
                    return Convert.ToInt32(dcSAP.OPOR.FirstOrDefault<OPOR>(cde => cde.DocNum == docNum).DocTotal.Value);
                }
                else
                {
                    return 0;
                }
            }
        }

        public void CorrectionSAP()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                //Prendre toutes les factures à trier par num sap croissant exclure le -1
                //Comptabiliser toutes ces factures
                //Prendre toutes les factures payées et les ordonner par id pay croissant
                //Comptabiliser ces paiements
                //Comptabiliser les paiements de caution avec les bonnes dates

                /***SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
                string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
                ***/

                #region fake

                //foreach (FACTURE fact in dcMar.FACTURE.Where(f => f.IdDocSAP == -1).OrderBy(f => f.IdDocSAP))
                //{
                //    var matchedElementsFactures = (from elt in dcMar.GetTable<ELEMENT_FACTURATION>()
                //                                   from lp in dcMar.GetTable<LIGNE_PROFORMA>()
                //                                   where elt.IdJEF == lp.IdJEF && lp.IdFP == fact.PROFORMA.IdFP
                //                                   select elt).ToList<ELEMENT_FACTURATION>();

                //    List<Int32> idJEFs = new List<int>();
                //    foreach (ELEMENT_FACTURATION e in matchedElementsFactures)
                //    {
                //        idJEFs.Add(e.IdJEF);
                //    }

                //    List<ARTICLE> listArts = (from art in dcMar.GetTable<ARTICLE>()
                //                              from elt in dcMar.GetTable<ELEMENT_FACTURATION>().Where(el => idJEFs.Contains(el.IdJEF))
                //                              where art.CodeArticle == elt.LIGNE_PRIX.CodeArticle
                //                              orderby art.LibArticle ascending
                //                              select art).Distinct<ARTICLE>().ToList<ARTICLE>();

                //    List<LigneEcriture> elts = (from eltFact in dcMar.GetTable<ELEMENT_FACTURATION>()
                //                                from lp in dcMar.GetTable<LIGNE_PROFORMA>()
                //                                where eltFact.IdJEF == lp.IdJEF && lp.IdFP == fact.PROFORMA.IdFP
                //                                //group eltFact by new { eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.LibFamArt, eltFact.LIGNE_PRIX.ARTICLE.LibArticle } into g
                //                                select new LigneEcriture
                //                                {
                //                                    CodeArticle = eltFact.LIGNE_PRIX.CodeArticle.Value,
                //                                    AccountCode = eltFact.CCArticle,
                //                                    CodeTVA = eltFact.CodeTVA,
                //                                    PrixUnitaire = eltFact.PUEF.Value,
                //                                    Qte = eltFact.QTEEF.Value
                //                                }).ToList<LigneEcriture>();

                //    string date = fact.DateComptable.Value.Year.ToString() + "-" + FormatChiffre(fact.DateComptable.Value.Month) + "-" + FormatChiffre(fact.DateComptable.Value.Day);
                //    string taxDate = fact.DCFD.Value.Year.ToString() + "-" + FormatChiffre(fact.DCFD.Value.Month) + "-" + FormatChiffre(fact.DCFD.Value.Day);

                //    List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();

                //    //foreach (ARTICLE art in listArts)
                //    //{
                //    //    SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                //    //    lg.ItemCode = art.CodeArticle.ToString();
                //    //    lg.AccountCode = elts.FirstOrDefault<LigneEcriture>(l => l.CodeArticle == art.CodeArticle).AccountCode;
                //    //    lg.Quantity = elts.Where(l => l.CodeArticle == art.CodeArticle).Sum(l => l.Qte).ToString().Replace(",", ".");
                //    //    lg.UnitPrice = elts.FirstOrDefault<LigneEcriture>(l => l.CodeArticle == art.CodeArticle).PrixUnitaire;
                //    //    lg.VATGroup = elts.FirstOrDefault<LigneEcriture>(l => l.CodeArticle == art.CodeArticle).CodeTVA;
                //    //    lignes.Add(lg);
                //    //}

                //    foreach (LigneEcriture ligne in elts)
                //    {
                //        SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                //        lg.ItemCode = ligne.CodeArticle.ToString();
                //        lg.AccountCode = ligne.AccountCode;
                //        lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                //        lg.UnitPrice = ligne.PrixUnitaire;
                //        lg.VATGroup = ligne.CodeTVA;
                //        lignes.Add(lg);
                //    }

                //    string idDocSAP = sapWS.AddInvoice(sessionID, fact.CLIENT.CodeClient, date, taxDate, date, fact.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc + "-" + fact.PROFORMA.CONNAISSEMENT.NumBL, /*fact.AIFD*/ fact.PROFORMA.CONNAISSEMENT.ConsigneeBL, lignes.ToArray());

                //}

                #endregion
                
                /***
                 * //foreach (PAYEMENT pay in dcMar.PAYEMENT.Where(p => p.ModePay == 1 && p.ObjetPay != 2 && p.IdPay >= 190 && p.IdPay < 300).OrderBy(p => p.IdPay))
                VsomParameters vsparam = new VsomParameters();
                foreach (PAYEMENT pay in dcMar.PAYEMENT.Where(p => p.IdPay == 1279).OrderBy(p => p.IdPay))
                {
                    string date = pay.DatePay.Value.Year.ToString() + "-" + FormatChiffre(pay.DatePay.Value.Month) + "-" + FormatChiffre(pay.DatePay.Value.Day);
                    string cashAccount = null;
                    if (pay.ModePay == 1)
                    {
                        cashAccount = vsparam.GetUtilisateursByIdU(pay.IdU.Value).Caisse.Contains("57") ? vsparam.GetUtilisateursByIdU(pay.IdU.Value).Caisse.Trim() : "57110";
                    }
                    else if (pay.ModePay == 2)
                    {
                        cashAccount = "5131100";
                    }
                    else if (pay.ModePay == 3)
                    {
                        cashAccount = "5211100";
                    }

                    if (pay.ObjetPay == 2)
                    {

                        foreach (CONTENEUR c in dcMar.CONTENEUR.Where(ctr => ctr.IdPay == pay.IdPay))
                        {
                            List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

                            listLignesJournal.Add(
                                new SocSAPWS.LigneJournal
                                {
                                    AccountCode = cashAccount,
                                    ShortName = cashAccount,
                                    ControlAccount = cashAccount,
                                    Credit = 0,
                                    Debit = c.MCCtr.Value,
                                    DueDate = date,
                                    ReferenceDate1 = date,
                                    TaxDate = date
                                }
                            );

                            listLignesJournal.Add(
                                new SocSAPWS.LigneJournal
                                {
                                    AccountCode = c.CONNAISSEMENT.CLIENT.CCClient,
                                    ShortName = c.CONNAISSEMENT.CLIENT.CodeClient,
                                    ControlAccount = "41910",
                                    Credit = c.MCCtr.Value,
                                    Debit = 0,
                                    DueDate = date,
                                    ReferenceDate1 = date,
                                    TaxDate = date
                                }
                            );

                            string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, "Encaissement caution Ctr N° " + c.NumCtr, c.NumCtr, c.ESCALE.NumEsc.ToString(), c.CONNAISSEMENT.CLIENT.CodeClient, listLignesJournal.ToArray());
                        }
                    }
                    else
                    {
                        List<SocSAPWS.PayementFacture> listFacturesPayement = new List<SocSAPWS.PayementFacture>();
                        foreach (FACTURE fact in dcMar.FACTURE.Where(f => f.IdPay == pay.IdPay))
                        {
                            if (fact.PROFORMA.LIGNE_PROFORMA.Count(lp => lp.ELEMENT_FACTURATION.LIGNE_PRIX.CodeArticle == 1204) != 0)
                            {
                                cashAccount = "5711104";
                            }

                            SocSAPWS.PayementFacture p = new SocSAPWS.PayementFacture();
                            p.DocEntry = fact.IdDocSAP.ToString();
                            p.PaidSum = fact.MTTC.Value;

                            listFacturesPayement.Add(p);
                        }

                        dcMar.SubmitChanges();

                        string idPaySAP = "";
                        if (pay.ModePay == 1)
                        {
                            idPaySAP = sapWS.AddPayment(sessionID, pay.CLIENT.CodeClient, date, cashAccount, pay.MAPay.Value, date, date, listFacturesPayement.ToArray());

                            if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
                            {
                                throw new ApplicationException(idPaySAP + " " + pay.IdPay.ToString());
                            }
                        }
                        else if (pay.ModePay == 2)
                        {
                            idPaySAP = sapWS.AddPaymentCheck(sessionID, pay.CLIENT.CodeClient, date, pay.Banque, pay.Agence, pay.NumCompte, pay.MAPay.ToString(), pay.NumCheque.Trim(), listFacturesPayement.ToArray());

                            if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
                            {
                                throw new ApplicationException(idPaySAP + " " + pay.IdPay.ToString());
                            }
                        }
                        else if (pay.ModePay == 3)
                        {
                            idPaySAP = sapWS.AddPaymentAccount(sessionID, pay.CLIENT.CodeClient, date, pay.BANQUE1.CCBanque, pay.RefVirement, date, pay.MAPay.ToString(), listFacturesPayement.ToArray());

                            if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
                            {
                                throw new ApplicationException(idPaySAP + " " + pay.IdPay.ToString());
                            }
                        }
                    }

                }

                sapWS.LogOut(sessionID);
                 * 
                 */ 
            }
        }

        #endregion

        #region manifeste


        


        public MANIFESTE CloturerManifeste(int idMan, string noteCloture, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedMan = (from man in dcMar.GetTable<MANIFESTE>()
                                  where man.IdMan == idMan
                                  select man).SingleOrDefault<MANIFESTE>();

                if (matchedMan == null)
                {
                    throw new EnregistrementInexistant("Le manifeste auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Manifeste : Clôture d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour clôturer un manifeste. Veuillez contacter un administrateur");
                }

                if (matchedMan.CONNAISSEMENT.Count(bl => !bl.DVBL.HasValue) == 0)
                {
                    throw new ConnaissementException("Clôture impossible : Ce manifeste ne contient aucun connaissement validé");
                }

                //Date actuelle du système
                DateTime dte = DateTime.Now;

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedMan.IdMan && op.IdTypeOp == 58
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                matchedOpMan.DateOp = DateTime.Now;
                matchedOpMan.IdU = idUser;
                matchedOpMan.AIOp = noteCloture;

                dcMar.OPERATION_MANIFESTE.Context.SubmitChanges();
                matchedMan.StatutMan = "Clôturé";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMan;
            }
        }


        public MANIFESTE InsertManifeste(int idEsc, string codePort, short nbPrevBL, short nbPrevVeh, short nbPrevCtr, short nbPrevMafi, short nbPrevGC, short formatMan, string cheminFichier, List<CONNAISSEMENT> bls, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<MAFI> mfs, List<CONVENTIONNEL> gcs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedPort = (from port in dcMar.GetTable<PORT>()
                                   where port.CodePort == codePort
                                   select port).SingleOrDefault<PORT>();

                if (matchedPort == null)
                {
                    throw new EnregistrementInexistant("Le port auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Manifeste : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un nouveau manifeste. Veuillez contacter un administrateur");
                }

                // Insertion du manifeste
                MANIFESTE manifeste = new MANIFESTE();

                manifeste.ESCALE = matchedEscale;
                manifeste.PORT = matchedPort;
                manifeste.DCMan = DateTime.Now;
                manifeste.NPBLMan = nbPrevBL;
                manifeste.NPVMan = nbPrevVeh;
                manifeste.NPCMan = nbPrevCtr;
                manifeste.NPMMan = nbPrevMafi;
                manifeste.NPGCMan = nbPrevGC;
                manifeste.FormatMan = formatMan;
                manifeste.CAFMan = cheminFichier;
                manifeste.StatutMan = "En cours";
                manifeste.AIMan = autresInfos;

                dcMar.MANIFESTE.Context.SubmitChanges();

                //Statuts de traitement du Manifeste
                List<TYPE_OPERATION> typeOpMan = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                  where type.EltTypeOp == "M"
                                                  select type).ToList<TYPE_OPERATION>();

                foreach (TYPE_OPERATION typeOp in typeOpMan)
                {
                    OPERATION_MANIFESTE opMan = new OPERATION_MANIFESTE();
                    opMan.IdMan = manifeste.IdMan;
                    opMan.IdTypeOp = typeOp.IdTypeOp;
                    dcMar.OPERATION_MANIFESTE.InsertOnSubmit(opMan);
                }
                dcMar.OPERATION_MANIFESTE.Context.SubmitChanges();

                // Insertion des éléments de éléments du manifeste
                #region enregistrement connaissement
                foreach (CONNAISSEMENT bl in bls)
                {
                    try
                    {
                        bl.IdClient = 1; // Client divers par défaut
                        bl.MANIFESTE = manifeste;
                        bl.ESCALE = matchedEscale;
                        if (bl.TypeBL == "C")
                        {
                            bl.BLFO = "N";
                            bl.BLLT = "Y";
                        }
                        else
                        {
                            bl.BLFO = "Y";
                            bl.BLLT = "N";
                        }

                        if (bl.BLIL == "Y")
                        {
                            bl.CodeTVA = "TVAEX";
                        }
                        else
                        {
                            bl.CodeTVA = "TVAAP";
                        }

                        dcMar.SubmitChanges();

                        //Statuts de traitement du BL
                        List<TYPE_OPERATION> typeOpBL = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                         where type.EltTypeOp == "BL"
                                                         select type).ToList<TYPE_OPERATION>();

                        foreach (TYPE_OPERATION typeOp in typeOpBL)
                        {
                            OPERATION_CONNAISSEMENT opBL = new OPERATION_CONNAISSEMENT();
                            opBL.IdBL = bl.IdBL;
                            opBL.IdTypeOp = typeOp.IdTypeOp;
                            if (opBL.IdTypeOp == 33)
                            {
                                opBL.DateOp = DateTime.Now;
                                opBL.IdU = idUser;
                            }
                            dcMar.OPERATION_CONNAISSEMENT.InsertOnSubmit(opBL);
                        }
                        dcMar.SubmitChanges();
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                } 
                #endregion

                foreach (VEHICULE v in vehs)
                {
                    v.ESCALE = matchedEscale;
                    v.MANIFESTE = manifeste;
                    v.VehAttelle = "N";
                    v.VehChargé = "N";
                    v.VehCle = "N";
                    v.VehStart = "N";
                    v.VehPorte = "N";
                }

                foreach (CONTENEUR c in ctrs)
                {
                    c.ESCALE = matchedEscale;
                    c.MANIFESTE = manifeste;

                    c.NbDet = 2;

                    if (matchedEscale.DDechEsc.HasValue)
                    {
                        c.FFCtr = matchedEscale.DDechEsc.Value.AddDays(9);
                        c.FSCtr = matchedEscale.DDechEsc.Value.AddDays(9);
                        c.FFSCtr = ((Int32)matchedEscale.DDechEsc.Value.DayOfWeek) > 2 ? matchedEscale.DDechEsc.Value.AddDays(11) : matchedEscale.DDechEsc.Value.AddDays(10);
                    }
                }

                foreach (MAFI m in mfs)
                {
                    m.ESCALE = matchedEscale;
                    m.MANIFESTE = manifeste;
                }

                foreach (CONVENTIONNEL c in gcs)
                {
                    c.ESCALE = matchedEscale;
                    c.MANIFESTE = manifeste;
                }

                manifeste.VEHICULE.AddRange(vehs);
                manifeste.CONTENEUR.AddRange(ctrs);
                manifeste.MAFI.AddRange(mfs);
                manifeste.CONVENTIONNEL.AddRange(gcs);

                dcMar.SubmitChanges();

                foreach (CONVENTIONNEL c in manifeste.CONVENTIONNEL)
                {
                    if (c.NumGC.Trim() == "")
                    {
                        c.NumGC = "GC" + FormatRefGC(c.IdGC);
                    }
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return manifeste;
            }
        }


        /// <summary>
        /// correction d'un manifeste
        /// </summary>
        /// <param name="idMan"></param>
        /// <param name="idEsc"></param>
        /// <param name="codePort"></param>
        /// <param name="nbPrevBL"></param>
        /// <param name="nbPrevVeh"></param>
        /// <param name="nbPrevCtr"></param>
        /// <param name="nbPrevMafi"></param>
        /// <param name="nbPrevGC"></param>
        /// <param name="formatMan"></param>
        /// <param name="cheminFichier"></param>
        /// <param name="bls"></param>
        /// <param name="vehs"></param>
        /// <param name="ctrs"></param>
        /// <param name="mfs"></param>
        /// <param name="gcs"></param>
        /// <param name="autresInfos"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public MANIFESTE UpdateManifeste(int idMan, int idEsc, string codePort, short nbPrevBL, short nbPrevVeh, short nbPrevCtr, short nbPrevMafi, short nbPrevGC, short formatMan, string cheminFichier, List<CONNAISSEMENT> bls, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<MAFI> mfs, List<CONVENTIONNEL> gcs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedManifeste = (from man in dcMar.GetTable<MANIFESTE>()
                                        where man.IdMan == idMan
                                        select man).SingleOrDefault<MANIFESTE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Manifeste : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour modifier un matchedManifeste. Veuillez contacter un administrateur");
                }

                if (matchedManifeste == null)
                {
                    throw new EnregistrementInexistant("Le matchedManifeste auquel vous faites référence n'existe pas");
                }

                if (matchedManifeste.DVMan.HasValue
                    )
                {
                    throw new ApplicationException("Mise à jour du manifeste impossible : Ce manifeste a déjà été validé");
                }

                if (matchedManifeste.CONNAISSEMENT.Count(bl => bl.DVBL.HasValue) != 0)
                {
                    throw new ApplicationException("Mise à jour du manifeste impossible : Il existe des connaissements déjà validés");
                }

                // Suppression de tous les éléments liés au matchedManifeste existant
                dcMar.CONNAISSEMENT.DeleteAllOnSubmit<CONNAISSEMENT>(matchedManifeste.CONNAISSEMENT);
                dcMar.VEHICULE.DeleteAllOnSubmit<VEHICULE>(matchedManifeste.VEHICULE);
                dcMar.CONTENEUR.DeleteAllOnSubmit<CONTENEUR>(matchedManifeste.CONTENEUR);
                dcMar.MAFI.DeleteAllOnSubmit<MAFI>(matchedManifeste.MAFI);
                dcMar.CONVENTIONNEL.DeleteAllOnSubmit<CONVENTIONNEL>(matchedManifeste.CONVENTIONNEL);

                List<OPERATION_CONNAISSEMENT> opBLMan = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                         from bl in dcMar.GetTable<CONNAISSEMENT>()
                                                         where op.IdBL == bl.IdBL && bl.IdMan == matchedManifeste.IdMan
                                                         select op).ToList<OPERATION_CONNAISSEMENT>();

                dcMar.OPERATION_CONNAISSEMENT.DeleteAllOnSubmit<OPERATION_CONNAISSEMENT>(opBLMan);

                List<OPERATION_MANIFESTE> opsMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedManifeste.IdMan
                                                    select op).ToList<OPERATION_MANIFESTE>();

                dcMar.OPERATION_MANIFESTE.DeleteAllOnSubmit<OPERATION_MANIFESTE>(opsMan);

                dcMar.SubmitChanges();

                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedPort = (from port in dcMar.GetTable<PORT>()
                                   where port.CodePort == codePort
                                   select port).SingleOrDefault<PORT>();

                if (matchedPort == null)
                {
                    throw new EnregistrementInexistant("Le port auquel vous faites référence n'existe pas");
                }

                // Mise à jour des informations de base
                matchedManifeste.ESCALE = matchedEscale;
                matchedManifeste.PORT = matchedPort;
                matchedManifeste.NPBLMan = nbPrevBL;
                matchedManifeste.NPVMan = nbPrevVeh;
                matchedManifeste.NPCMan = nbPrevCtr;
                matchedManifeste.NPMMan = nbPrevMafi;
                matchedManifeste.NPGCMan = nbPrevGC;
                matchedManifeste.FormatMan = formatMan;
                matchedManifeste.CAFMan = cheminFichier;
                matchedManifeste.AIMan = autresInfos;

                List<TYPE_OPERATION> typeOpMan = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                  where type.EltTypeOp == "M"
                                                  select type).ToList<TYPE_OPERATION>();

                foreach (TYPE_OPERATION typeOp in typeOpMan)
                {
                    OPERATION_MANIFESTE opMan = new OPERATION_MANIFESTE();
                    opMan.IdMan = matchedManifeste.IdMan;
                    opMan.IdTypeOp = typeOp.IdTypeOp;
                    dcMar.OPERATION_MANIFESTE.InsertOnSubmit(opMan);
                }
                dcMar.OPERATION_MANIFESTE.Context.SubmitChanges();

                // Insertion des éléments de éléments du matchedManifeste
                foreach (CONNAISSEMENT bl in bls)
                {
                    bl.IdClient = 1; // Client divers par défaut
                    bl.MANIFESTE = matchedManifeste;
                    bl.ESCALE = matchedEscale;
                    if (bl.TypeBL == "C")
                    {
                        bl.BLFO = "N";
                        bl.BLLT = "Y";
                    }
                    else
                    {
                        bl.BLFO = "Y";
                        bl.BLLT = "N";
                    }

                    if (bl.BLIL == "Y")
                    {
                        bl.CodeTVA = "TVAEX";
                    }
                    else
                    {
                        bl.CodeTVA = "TVAAP";
                    }

                    dcMar.SubmitChanges();

                    //Statuts de traitement du BL
                    List<TYPE_OPERATION> typeOpBL = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                     where type.EltTypeOp == "BL"
                                                     select type).ToList<TYPE_OPERATION>();

                    foreach (TYPE_OPERATION typeOp in typeOpBL)
                    {
                        OPERATION_CONNAISSEMENT opBL = new OPERATION_CONNAISSEMENT();
                        opBL.IdBL = bl.IdBL;
                        opBL.IdTypeOp = typeOp.IdTypeOp;
                        if (opBL.IdTypeOp == 33)
                        {
                            opBL.DateOp = DateTime.Now;
                            opBL.IdU = idUser;
                        }
                        dcMar.OPERATION_CONNAISSEMENT.InsertOnSubmit(opBL);
                    }
                    dcMar.SubmitChanges();
                }

                foreach (VEHICULE v in vehs)
                {
                    v.ESCALE = matchedEscale;
                    v.MANIFESTE = matchedManifeste;
                    v.VehAttelle = "N";
                    v.VehChargé = "N";
                    v.VehCle = "N";
                    v.VehStart = "N";
                    v.VehPorte = "N";
                }

                foreach (CONTENEUR c in ctrs)
                {
                    c.ESCALE = matchedEscale;
                    c.MANIFESTE = matchedManifeste;
                }

                foreach (MAFI m in mfs)
                {
                    m.ESCALE = matchedEscale;
                    m.MANIFESTE = matchedManifeste;
                }

                foreach (CONVENTIONNEL c in gcs)
                {
                    c.ESCALE = matchedEscale;
                    c.MANIFESTE = matchedManifeste;
                }

                matchedManifeste.VEHICULE.AddRange(vehs);
                matchedManifeste.CONTENEUR.AddRange(ctrs);
                matchedManifeste.MAFI.AddRange(mfs);
                matchedManifeste.CONVENTIONNEL.AddRange(gcs);

                dcMar.SubmitChanges();

                foreach (CONVENTIONNEL c in matchedManifeste.CONVENTIONNEL)
                {
                    if (c.NumGC.Trim() == "")
                    {
                        c.NumGC = "GC" + FormatRefGC(c.IdGC);
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedManifeste;
            }
        }

        #endregion

        #region connaissement


        /// <summary>
        /// enregistrement de bl
        /// </summary>
        /// <param name="numBL">numero bl</param>
        /// <param name="idClient">id client</param>
        /// <param name="idEsc">id escal</param>
        /// <param name="idMan">id manifest</param>
        /// <param name="consBL">nom consignee</param>
        /// <param name="adresseBL">adresse </param>
        /// <param name="notifyBL">notify</param>
        /// <param name="emailBL">email</param>
        /// <param name="portProv">port provenance</param>
        /// <param name="destBL">destination bl</param>
        /// <param name="nomCharger">nom chargeur</param>
        /// <param name="adresseCharger">adresse chargeur</param>
        /// <param name="chkBLGN">incoterm grand nord</param>
        /// <param name="chkBLIL">incoterm hinterland</param>
        /// <param name="chkBLFO">incoterm free out</param>
        /// <param name="chkBLLT">incoterm line out</param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CONNAISSEMENT InsertConnaissement(string numBL, int idClient, int idEsc, int idMan, string consBL, string adresseBL, string notifyBL, string emailBL, string portProv, string destBL, string nomCharger, string adresseCharger, string chkBLGN, string chkBLIL, string chkBLFO, string chkBLLT, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.NumBL == numBL
                                            select bl).FirstOrDefault<CONNAISSEMENT>();

                if (matchedConnaissement != null)
                {
                    throw new ApplicationException("Il existe déjà un connaissement portant ce numéro");
                }

                var matchedEscale = (from es in dcMar.GetTable<ESCALE>()
                                     where es.IdEsc == idEsc
                                     select es).FirstOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("Escale inexistante");
                }

                var matchedManifeste = (from man in dcMar.GetTable<MANIFESTE>()
                                        where man.IdMan == idMan
                                        select man).FirstOrDefault<MANIFESTE>();

                if (matchedManifeste == null)
                {
                    throw new EnregistrementInexistant("Manifeste inexistant");
                }

                var matchedClient = (from clt in dcMar.GetTable<CLIENT>()
                                     where clt.IdClient == idClient
                                     select clt).FirstOrDefault<CLIENT>();

                if (matchedClient == null)
                {
                    throw new EnregistrementInexistant("Client inexistant");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                DateTime dte = DateTime.Now;

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Connaissement : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un connaissement. Veuillez contacter un administrateur");
                }

                CONNAISSEMENT con = new CONNAISSEMENT();
                con.NumBL = numBL;
                con.BLIL = chkBLIL;
                con.BLGN = chkBLGN;
                con.BLLT = chkBLLT;
                con.BLFO = chkBLFO;
                con.BlBloque = "N";
                con.BLER = "N";
                con.BLSocar = "N";
                con.SensBL = "I";
                con.CCBL = "N";
                con.TypeBL = "R";
                con.ConsigneeBL = consBL;
                con.AdresseBL = adresseBL;
                con.NotifyBL = notifyBL;
                con.LPBL = portProv;
                con.DPBL = "CMDLA";
                con.DCBL = DateTime.Now;
                con.StatutBL = matchedManifeste.DVMan.HasValue ? "Initié" : "Non initié";
                con.EtatBL = "O";
                con.CCBLMontant = 0;
                con.BLDette = "N";
                con.DetteMontant = 0;
                con.IdAcc = matchedEscale.IdAcc;
                con.IdEsc = matchedEscale.IdEsc;
                con.IdMan = matchedManifeste.IdMan;
                con.PoidsBL = 0;
                con.VolBL = 0;
                con.DestBL = destBL;
                con.NomCharger = nomCharger;
                con.AdresseCharger = adresseCharger;
                con.LastModif = DateTime.Now;
                con.EmailBL = emailBL;
                con.IdClient = idClient;
                con.IdU = matchedUser.IdU;

                if (con.BLIL == "Y")
                {
                    con.CodeTVA = "TVAEX";
                }
                else
                {
                    con.CodeTVA = "TVAAP";
                }

                dcMar.CONNAISSEMENT.InsertOnSubmit(con);
                dcMar.SubmitChanges();

                //Statuts de traitement du BL
                List<TYPE_OPERATION> typeOpBL = (from type in dcMar.GetTable<TYPE_OPERATION>()
                                                 where type.EltTypeOp == "BL"
                                                 select type).ToList<TYPE_OPERATION>();

                foreach (TYPE_OPERATION typeOp in typeOpBL)
                {
                    OPERATION_CONNAISSEMENT opBL = new OPERATION_CONNAISSEMENT();
                    opBL.IdBL = con.IdBL;
                    opBL.IdTypeOp = typeOp.IdTypeOp;
                    if (opBL.IdTypeOp == 33)
                    {
                        opBL.DateOp = DateTime.Now;
                        opBL.IdU = idUser;
                    }
                    dcMar.OPERATION_CONNAISSEMENT.InsertOnSubmit(opBL);
                }
                dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();

                dcMar.SubmitChanges();

                transaction.Complete();
                return con;
            }
        }

        // Cette opération rend le BL disponible pour des opérations d'enlèvement
        public CONNAISSEMENT AccomplirBL(int idBL, string nomMand, string cniMand, Nullable<DateTime> cniDelivreLe, string cniDelivreA, string telMandBL, string emailMandBL, string numContr, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Connaissement : Modification des informations l'accomplissement").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour accomplir un connaissement. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Le connaissement auquel vous faites référence n'existe pas");
                }

                if (!dcMar.OPERATION_CONNAISSEMENT.SingleOrDefault<OPERATION_CONNAISSEMENT>(op => op.IdTypeOp == 35 && op.IdBL == matchedConnaissement.IdBL).DateOp.HasValue)
                {
                    throw new ConnaissementException("Accomplissement impossible : Ce connaissement n'a pas encore été validé");
                }

                // L'accomplissement ne peut se faire que pour des BLs non clôturés
                if (matchedConnaissement.StatutBL == "Clôturé")
                {
                    throw new ConnaissementException("Echec d'accomplissement du BL : Le connaissement auquel vous faites référence a été clôturé");
                }

                matchedConnaissement.NomManBL = nomMand;
                matchedConnaissement.CNIManBL = cniMand;
                matchedConnaissement.DDCNIManBL = cniDelivreLe;
                matchedConnaissement.LDCNIManBL = cniDelivreA;
                matchedConnaissement.PhoneManBL = telMandBL;
                matchedConnaissement.EmailBL = emailMandBL;
                matchedConnaissement.NContribBL = numContr;
                matchedConnaissement.DateAccBL = DateTime.Now;
                matchedConnaissement.AIBL = autresInfos;
                matchedConnaissement.StatutBL = "Accompli";

                foreach (VEHICULE veh in matchedConnaissement.VEHICULE)
                {
                    veh.NomEnVeh = nomMand;
                    veh.CNIEnVeh = cniMand;
                    veh.TelenVeh = telMandBL;
                }

                foreach (CONTENEUR ctr in matchedConnaissement.CONTENEUR)
                {
                    ctr.NomEnCtr = nomMand;
                    ctr.CNIEnCtr = cniMand;
                    ctr.TelenCtr = telMandBL;
                }

                foreach (CONVENTIONNEL conv in matchedConnaissement.CONVENTIONNEL)
                {
                    conv.NomEnGC = nomMand;
                    conv.CNIEnGC = cniMand;
                    conv.TelenGC = telMandBL;
                }

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConnaissement.IdBL && op.IdTypeOp == 36
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                matchedOpBL.DateOp = DateTime.Now;
                matchedOpBL.IdU = idUser;
                matchedOpBL.AIOp = autresInfos;

                dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConnaissement;
            }
        }

        public CONNAISSEMENT CloturerConnaissement(int idBL, string noteCloture, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedBL = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

                if (matchedBL == null)
                {
                    throw new EnregistrementInexistant("Le connaissement auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Connaissement : Clôture d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour clôturer un connaissement. Veuillez contacter un administrateur");
                }

                if (matchedBL.StatutBL != "Non initié")
                {
                    throw new ConnaissementException("Clôture impossible : Le traitement de ce connaissement a déjà débuté");
                }

                //Date actuelle du système
                DateTime dte = DateTime.Now;

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedBL.IdBL && op.IdTypeOp == 42
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                matchedOpBL.DateOp = DateTime.Now;
                matchedOpBL.IdU = idUser;
                matchedOpBL.AIOp = noteCloture;

                dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                matchedBL.StatutBL = "Cloturé";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBL;
            }
        }


        #endregion

        #region client

        public CLIENT InsertOrUpdateClient(int id, string codeClient, string nomClient, string adrClient, string emailClient, string telClient, string numContrib, string numRegCommerce, string compteComptable, string codeTVA, string typeClient)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedClient = (from client in dcMar.GetTable<CLIENT>()
                                     where client.IdClient == id
                                     select client).SingleOrDefault<CLIENT>();

                var matchedCodeTVA = (from tva in dcMar.GetTable<CODE_TVA>()
                                      where tva.CodeTVA == codeTVA
                                      select tva).SingleOrDefault<CODE_TVA>();

                if (matchedCodeTVA == null)
                {
                    throw new EnregistrementInexistant("Le code de TVA auquel vous faites référence n'existe pas");
                }

                // Client inexistant : Insertion d'un nouveau client
                if (matchedClient == null)
                {
                    CLIENT client = new CLIENT();

                    var clientExist = (from clt in dcMar.GetTable<CLIENT>()
                                       where clt.CodeClient == codeClient
                                       select clt).SingleOrDefault<CLIENT>();

                    if (clientExist != null)
                    {
                        throw new ClientException("Ce code client a déjà été attribué au client " + clientExist.NomClient);
                    }

                    client.CodeClient = codeClient;
                    client.NomClient = nomClient;
                    client.AdrClient = adrClient;
                    client.EmailClient = emailClient;
                    client.TelClient = telClient;
                    client.CCClient = compteComptable;
                    client.CodeTVA = codeTVA;
                    client.TypeClient = typeClient;
                    client.NumContrib = numContrib;
                    client.NumRegCommerce = numRegCommerce;
                    client.StatutClient = "A";

                    dcMar.GetTable<CLIENT>().InsertOnSubmit(client);
                    dcMar.SubmitChanges();
                    transaction.Complete();
                    return client;
                }
                // CLient existant : Mise à jour d'un nouveau client
                else
                {
                    var clientExist = (from clt in dcMar.GetTable<CLIENT>()
                                       where clt.CodeClient == codeClient && clt.IdClient != id
                                       select clt).FirstOrDefault<CLIENT>();

                    if (clientExist != null)
                    {
                        throw new ClientException("Ce code client a déjà été attribué au client " + clientExist.NomClient);
                    }

                    matchedClient.CodeClient = codeClient;
                    matchedClient.NomClient = nomClient;
                    matchedClient.AdrClient = adrClient;
                    matchedClient.EmailClient = emailClient;
                    matchedClient.TelClient = telClient;
                    matchedClient.CCClient = compteComptable;
                    matchedClient.CodeTVA = codeTVA;
                    matchedClient.TypeClient = typeClient;
                    matchedClient.NumContrib = numContrib;
                    matchedClient.NumRegCommerce = numRegCommerce;

                    dcMar.SubmitChanges();
                    transaction.Complete();
                    return matchedClient;
                }
            }
        }

        #endregion

        #region cubage de vehicule


        public CUBAGE InsertProjetCubage(DateTime dateCreation, DateTime dateExecution, string autresInfos, List<VehiculeCubage> vehCubs, int idEsc, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Cubage : Enregistrement d'une nouvelle opération").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un projet de cubage. Veuillez contacter un administrateur");
                }

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                // Insertion du projet de cubage
                CUBAGE cubage = new CUBAGE();

                cubage.DateCubage = dateCreation;
                cubage.DateExCubage = dateExecution;
                cubage.AICubage = autresInfos;
                cubage.IdEsc = idEsc;

                dcMar.GetTable<CUBAGE>().InsertOnSubmit(cubage);
                dcMar.SubmitChanges();

                StringBuilder msgCorrections = new StringBuilder();

                // Insertion des véhicules à cuber
                foreach (VehiculeCubage veh in vehCubs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    if (matchedVeh == null)
                    {
                        throw new EnregistrementInexistant("Un des véhicules auquel vous faites référence n'existe pas");
                    }

                    if (!matchedVeh.MANIFESTE.DVMan.HasValue)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : Manifeste non validé").Append(Environment.NewLine);
                    }

                    if (matchedVeh.ELEMENT_FACTURATION.Count(el => (el.LibEF.Contains("Manutention") || el.LibEF.Contains("Séjour Parc Auto") || el.LibEF.Contains("Pénalité de stationnement") || el.LibEF.Contains("Debours PAD - TVA sur Pénalité de stationnement")) && el.LIGNE_PROFORMA.Count(lp => lp.PROFORMA.StatutFP == "O") != 0) != 0)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : En Proforma").Append(Environment.NewLine);
                    }

                    if (matchedVeh.ELEMENT_FACTURATION.Count(el => (el.LibEF.Contains("Manutention") || el.LibEF.Contains("Séjour Parc Auto") || el.LibEF.Contains("Pénalité de stationnement") || el.LibEF.Contains("Debours PAD - TVA sur Pénalité de stationnement")) && el.IdFD.HasValue && !el.IdFA.HasValue) != 0)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : En Facture").Append(Environment.NewLine);
                    }

                    CUBAGE_VEHICULE cubVeh = new CUBAGE_VEHICULE();
                    cubVeh.IdCubage = cubage.IdCubage;
                    cubVeh.IdVeh = veh.IdVeh;
                    cubVeh.LongAVeh = veh.LongueurManifeste;
                    cubVeh.LargAVeh = veh.LargeurManifeste;
                    cubVeh.HautAVeh = veh.HauteurManifeste;
                    cubVeh.VolAVeh = (float)veh.VolumeManifeste;
                    cubVeh.LongCVeh = 0;
                    cubVeh.LargCVeh = 0;
                    cubVeh.HautCVeh = 0;
                    cubVeh.VolCVeh = 0;

                    dcMar.GetTable<CUBAGE_VEHICULE>().InsertOnSubmit(cubVeh);
                }

                if (msgCorrections.ToString().Trim() != "")
                {
                    throw new ApplicationException("Les véhicules suivants : \n" + msgCorrections.ToString() + " Ne peuvent faire partie de projets de cubage. Veuillez au préalable annuler les proforma concernées et faire des avoirs sur les factures concernées. Aussi validez les manifestes des véhicules sélectionnés");
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return cubage;
            }
        }

        public CUBAGE AjoutVehiculesCubage(int idCub, List<VehiculeCubage> vehCubs, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Cubage : Modification des informations sur une opération existante").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un projet de cubage. Veuillez contacter un administrateur");
                }

                var matchedCubage = (from c in dcMar.GetTable<CUBAGE>()
                                     where c.IdCubage == idCub
                                     select c).FirstOrDefault<CUBAGE>();

                if (matchedCubage == null)
                {
                    throw new EnregistrementInexistant("Le projet de cubage auquel vous faites référence n'existe pas");
                }

                StringBuilder msgCorrections = new StringBuilder();

                // ajout des véhicules à cuber
                foreach (VehiculeCubage veh in vehCubs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    if (matchedVeh == null)
                    {
                        throw new EnregistrementInexistant("Un des véhicules auquel vous faites référence n'existe pas");
                    }

                    if (!matchedVeh.MANIFESTE.DVMan.HasValue)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : Manifeste non validé").Append(Environment.NewLine);
                    }

                    if (matchedVeh.ELEMENT_FACTURATION.Count(el => (el.LibEF.Contains("Manutention") || el.LibEF.Contains("Séjour Parc Auto")) && el.LIGNE_PROFORMA.Count(lp => lp.PROFORMA.StatutFP == "O") != 0) != 0)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : En Proforma").Append(Environment.NewLine);
                    }

                    if (matchedVeh.ELEMENT_FACTURATION.Count(el => (el.LibEF.Contains("Manutention") || el.LibEF.Contains("Séjour Parc Auto")) && el.IdFD.HasValue && !el.IdFA.HasValue) != 0)
                    {
                        msgCorrections.Append(matchedVeh.NumChassis + " " + matchedVeh.DescVeh + " BL : N° " + matchedVeh.CONNAISSEMENT.NumBL + " : En Facture").Append(Environment.NewLine);
                    }

                    CUBAGE_VEHICULE cubVeh = new CUBAGE_VEHICULE();
                    cubVeh.IdCubage = matchedCubage.IdCubage;
                    cubVeh.IdVeh = veh.IdVeh;
                    cubVeh.LongAVeh = veh.LongueurManifeste;
                    cubVeh.LargAVeh = veh.LargeurManifeste;
                    cubVeh.HautAVeh = veh.HauteurManifeste;
                    cubVeh.VolAVeh = (float)veh.VolumeManifeste;
                    cubVeh.LongCVeh = 0;
                    cubVeh.LargCVeh = 0;
                    cubVeh.HautCVeh = 0;
                    cubVeh.VolCVeh = 0;

                    dcMar.GetTable<CUBAGE_VEHICULE>().InsertOnSubmit(cubVeh);
                }

                if (msgCorrections.ToString().Trim() != "")
                {
                    throw new ApplicationException("Les véhicules suivants : \n" + msgCorrections.ToString() + " Ne peuvent faire partie de projets de cubage. Veuillez au préalable annuler les proforma concernées et faire des avoir sur les factures concernées. Aussi validez les manifestes des véhicules sélectionnés");
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedCubage;
            }
        }

        public CUBAGE RetirerVehiculesCubage(int idCub, List<VehiculeCubage> vehCubs, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Cubage : Modification des informations sur une opération existante").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un projet de cubage. Veuillez contacter un administrateur");
                }

                var matchedCubage = (from c in dcMar.GetTable<CUBAGE>()
                                     where c.IdCubage == idCub
                                     select c).FirstOrDefault<CUBAGE>();

                if (matchedCubage == null)
                {
                    throw new EnregistrementInexistant("Le projet de cubage auquel vous faites référence n'existe pas");
                }

                // suppression des véhicules à cuber
                foreach (VehiculeCubage veh in vehCubs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    if (matchedVeh == null)
                    {
                        throw new EnregistrementInexistant("Un des véhicules auquel vous faites référence n'existe pas");
                    }

                    var matchedCubVeh = (from cv in dcMar.GetTable<CUBAGE_VEHICULE>()
                                         where cv.IdVeh == matchedVeh.IdVeh && cv.IdCubage == idCub
                                         select cv).SingleOrDefault<CUBAGE_VEHICULE>();

                    if (matchedCubVeh != null)
                    {
                        dcMar.CUBAGE_VEHICULE.DeleteOnSubmit(matchedCubVeh);
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedCubage;
            }
        }

        public CUBAGE CloturerProjetCubage(int idCub, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedProjetCubage = (from cub in dcMar.GetTable<CUBAGE>()
                                           where cub.IdCubage == idCub
                                           select cub).SingleOrDefault<CUBAGE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Cubage : Clôture d'une opération existante").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour clôturer un projet de cubage. Veuillez contacter un administrateur");
                }

                if (matchedProjetCubage == null)
                {
                    throw new EnregistrementInexistant("Le projet de cubage auquel vous faites référence n'existe pas");
                }

                // Vérification que tous les véhicules du projet de cubage ont été cubés
                if (matchedProjetCubage.CUBAGE_VEHICULE.Count(veh => veh.VolCVeh != 0) < matchedProjetCubage.CUBAGE_VEHICULE.Count)
                {
                    throw new CubageException("Il existe des véhicules non cubés dans ce projet de cubage");
                }

                // Vérification que le projet de cubage n'a pas encore fait l'objet d'une clôture
                if (matchedProjetCubage.DateCloCubage.HasValue)
                {
                    throw new CubageException("Ce projet de cubage a déjà été clôturé.\nDate de clôture : " + matchedProjetCubage.DateCloCubage.Value);
                }

                matchedProjetCubage.DateCloCubage = DateTime.Now;

                #region fake
                //// La génération des éléments de facturation à la clôture d'un projet de cubage ne se fait que sur les escales dont Socomar est acconier
                //if (matchedProjetCubage.ESCALE.RAEsc == "Y")
                //{
                //    // Sélection des lignes de prix : Adaptée à partir de la routine de validation du manifeste
                //    List<PARAMETRE> parametres = dcMar.GetTable<PARAMETRE>().ToList<PARAMETRE>();
                //    List<ARTICLE> articles = dcMar.GetTable<ARTICLE>().ToList<ARTICLE>();

                //    DateTime dte = DateTime.Now;

                //    ARTICLE articleManutBord = (from art in articles
                //                                from par in parametres
                //                                where art.CodeArticle == par.CodeAF && par.NomAF == "Manutention Bord"
                //                                select art).SingleOrDefault<ARTICLE>();

                //    LIGNE_PRIX lpManutBord = articleManutBord.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                //    ARTICLE articleManutBordSupplementVolume = (from art in articles
                //                                                from par in parametres
                //                                                where art.CodeArticle == par.CodeAF && par.NomAF == "Manutention Bord - Supplément volume"
                //                                                select art).SingleOrDefault<ARTICLE>();

                //    LIGNE_PRIX lpManutBordSupplementVolume = articleManutBordSupplementVolume.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                //    ARTICLE articleManutTerre = (from art in articles
                //                                 from par in parametres
                //                                 where art.CodeArticle == par.CodeAF && par.NomAF == "Manutention Terre"
                //                                 select art).SingleOrDefault<ARTICLE>();

                //    LIGNE_PRIX lpManutTerre = articleManutTerre.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

                //    List<VEHICULE> listVehCub = (from veh in dcMar.GetTable<VEHICULE>()
                //                                 from cub in dcMar.GetTable<CUBAGE_VEHICULE>()
                //                                 where veh.IdVeh == cub.IdVeh && cub.IdCubage == idCub
                //                                 select veh).ToList<VEHICULE>();

                //    foreach (VEHICULE v in listVehCub)
                //    {
                //        double derogation = (v.CONNAISSEMENT.BLIL == "Y" || v.CONNAISSEMENT.BLGN == "Y") ? 0.25 : 0;

                //        // Element de facturation manutention bord
                //        var matchedEltManutBord = (from ef in dcMar.GetTable<ELEMENT_FACTURATION>()
                //                                   where ef.IdVeh == v.IdVeh && ef.LibEF.Contains("Manutention Bord Chassis")
                //                                   select ef).SingleOrDefault<ELEMENT_FACTURATION>();

                //        if (matchedEltManutBord != null)
                //        {
                //            matchedEltManutBord.PUEF = v.VolCVeh <= 10 ? lpManutBord.PU1LP : lpManutBord.PU2LP;
                //            matchedEltManutBord.CCArticle = v.CONNAISSEMENT.BLIL == "Y" ? articleManutBord.CCArticleEx : (articleManutBord.CodeTVA == "TVAAP" ? articleManutBord.CCArticle : articleManutBord.CCArticleEx);
                //            matchedEltManutBord.CodeTVA = v.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : "TVAAP";
                //            matchedEltManutBord.TauxTVA = v.CONNAISSEMENT.BLIL == "Y" ? 0 : (articleManutBord.CodeTVA == "TVAAP" ? lpManutBord.ARTICLE.CODE_TVA.TauxTVA : 0);
                //            matchedEltManutBord.CCCP = v.CONNAISSEMENT.CLIENT.CodeClient;
                //            matchedEltManutBord.QTEEF = v.VolCVeh;
                //            matchedEltManutBord.UnitEF = lpManutBord.UniteLP;
                //            if (v.CONNAISSEMENT.BLLT == "Y")
                //            {
                //                matchedEltManutBord.PUEF = 0;
                //            }
                //        }

                //        // Element de facturation manutention bord - supplement volume
                //        var matchedEltManutBordSupplementVolume = (from ef in dcMar.GetTable<ELEMENT_FACTURATION>()
                //                                                   where ef.IdVeh == v.IdVeh && ef.LibEF.Contains("Manutention Bord - Supplément volume Chassis")
                //                                                   select ef).SingleOrDefault<ELEMENT_FACTURATION>();

                //        if (matchedEltManutBordSupplementVolume != null)
                //        {
                //            matchedEltManutBordSupplementVolume.CCArticle = v.CONNAISSEMENT.BLIL == "Y" ? articleManutBordSupplementVolume.CCArticleEx : (articleManutBordSupplementVolume.CodeTVA == "TVAAP" ? articleManutBordSupplementVolume.CCArticle : articleManutBordSupplementVolume.CCArticleEx);
                //            matchedEltManutBordSupplementVolume.CodeTVA = v.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : "TVAAP";
                //            matchedEltManutBordSupplementVolume.TauxTVA = v.CONNAISSEMENT.BLIL == "Y" ? 0 : (articleManutBordSupplementVolume.CodeTVA == "TVAAP" ? lpManutBordSupplementVolume.ARTICLE.CODE_TVA.TauxTVA : 0);
                //            matchedEltManutBordSupplementVolume.CCCP = v.CONNAISSEMENT.CLIENT.CodeClient;
                //            matchedEltManutBordSupplementVolume.QTEEF = v.VolCVeh > 100 ? v.VolCVeh - 100 : 0;
                //            matchedEltManutBordSupplementVolume.UnitEF = lpManutBordSupplementVolume.UniteLP;
                //            matchedEltManutBordSupplementVolume.PUEF = v.CONNAISSEMENT.BLGN == "N" ? lpManutBordSupplementVolume.PU1LP : lpManutBordSupplementVolume.PU2LP;
                //            matchedEltManutBordSupplementVolume.PUEF = v.CONNAISSEMENT.BLIL == "Y" ? lpManutBordSupplementVolume.PU2LP : lpManutBordSupplementVolume.PU1LP;
                //        }

                //        // Element de facturation manutention terre Socomar
                //        var matchedEltManutTerre = (from ef in dcMar.GetTable<ELEMENT_FACTURATION>()
                //                                    where ef.IdVeh == v.IdVeh && ef.LibEF.Contains("Manutention Terre Chassis")
                //                                    select ef).SingleOrDefault<ELEMENT_FACTURATION>();

                //        if (matchedEltManutTerre != null)
                //        {
                //            matchedEltManutTerre.CCArticle = articleManutTerre.CODE_TVA.TauxTVA == 0 ? articleManutTerre.CCArticleEx : articleManutTerre.CCArticle;
                //            matchedEltManutTerre.CCCP = v.CONNAISSEMENT.CLIENT.CodeClient;
                //            matchedEltManutTerre.QTEEF = 1;
                //            if (v.VolCVeh < 16)
                //            {
                //                matchedEltManutTerre.PUEF = Convert.ToInt32(lpManutTerre.PU1LP - lpManutTerre.PU1LP * derogation);
                //            }
                //            else if (v.VolCVeh < 50)
                //            {
                //                matchedEltManutTerre.PUEF = Convert.ToInt32(lpManutTerre.PU2LP - lpManutTerre.PU2LP * derogation);
                //            }
                //            else
                //            {
                //                matchedEltManutTerre.PUEF = Convert.ToInt32(lpManutTerre.PU3LP - lpManutTerre.PU3LP * derogation);
                //            }
                //            matchedEltManutTerre.UnitEF = lpManutTerre.UniteLP;
                //            matchedEltManutTerre.CodeTVA = v.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : "TVAAP";
                //            matchedEltManutTerre.TauxTVA = articleManutTerre.CodeTVA == "TVAEX" ? 0 : v.CONNAISSEMENT.CODE_TVA.TauxTVA;
                //        }
                //    }
                //}

                #endregion
                
                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedProjetCubage;
            }
        }

        #endregion

        #region vehicule


        public void CorrigerReceptionVehiculeGesparc()
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {

                List<OCCUPATION> occups = dcMar.GetTable<OCCUPATION>().ToList<OCCUPATION>();

                foreach (OCCUPATION occ in occups)
                {
                    // inserer opération d'identification au parc
                    if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == occ.VEHICULE.IdVeh && v.IdTypeOp == 1 && v.IdLieu == 3) == 0)
                    {
                        OPERATION_VEHICULE op = new OPERATION_VEHICULE();
                        op.IdVeh = occ.VEHICULE.IdVeh;
                        op.IdTypeOp = 1;
                        op.DateOp = occ.DateDebut;
                        op.IdU = 1;
                        op.IdLieu = 3;
                        op.AIOp = "";

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(op);

                        OPERATION_CONNAISSEMENT matchedOpBL = (from opBL in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where opBL.IdBL == occ.VEHICULE.IdBL && opBL.IdTypeOp == 37
                                                               select opBL).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        if (!matchedOpBL.DateOp.HasValue)
                        {
                            matchedOpBL.DateOp = DateTime.Now;
                            matchedOpBL.IdU = 1;
                            matchedOpBL.AIOp = "Déchargement démarré";
                        }

                        if (occ.VEHICULE.CONNAISSEMENT.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0) == occ.VEHICULE.CONNAISSEMENT.VEHICULE.Count)
                        {
                            OPERATION_CONNAISSEMENT matchedOpBLTerm = (from opBLT in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                                       where opBLT.IdBL == occ.VEHICULE.IdBL && opBLT.IdTypeOp == 38
                                                                       select opBLT).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                            if (!matchedOpBLTerm.DateOp.HasValue)
                            {
                                matchedOpBLTerm.DateOp = DateTime.Now;
                                matchedOpBLTerm.IdU = 1;
                                matchedOpBLTerm.AIOp = "Déchargement terminé";
                            }
                        }

                        OPERATION_MANIFESTE matchedOpMan = (from opMan in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where opMan.IdMan == occ.VEHICULE.IdMan && opMan.IdTypeOp == 45
                                                            select opMan).SingleOrDefault<OPERATION_MANIFESTE>();

                        if (!matchedOpMan.DateOp.HasValue)
                        {
                            matchedOpMan.DateOp = DateTime.Now;
                            matchedOpMan.IdU = 1;
                            matchedOpMan.AIOp = "Déchargement démarré";
                        }

                        if (occ.VEHICULE.MANIFESTE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0) == occ.VEHICULE.MANIFESTE.VEHICULE.Count)
                        {
                            OPERATION_MANIFESTE matchedOpManTerm = (from opManT in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                                    where opManT.IdMan == occ.VEHICULE.IdMan && opManT.IdTypeOp == 46
                                                                    select opManT).SingleOrDefault<OPERATION_MANIFESTE>();

                            if (!matchedOpManTerm.DateOp.HasValue)
                            {
                                matchedOpManTerm.DateOp = DateTime.Now;
                                matchedOpManTerm.IdU = 1;
                                matchedOpManTerm.AIOp = "Déchargement terminé";
                            }
                        }

                        OPERATION_ESCALE matchedOpEsc = (from opEsc in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where opEsc.IdEsc == occ.VEHICULE.IdEsc && opEsc.IdTypeOp == 53
                                                         select opEsc).SingleOrDefault<OPERATION_ESCALE>();

                        if (!matchedOpEsc.DateOp.HasValue)
                        {
                            matchedOpEsc.DateOp = DateTime.Now;
                            matchedOpEsc.IdU = 1;
                            matchedOpEsc.AIOp = "Déchargement démarré";
                        }

                        if (occ.VEHICULE.ESCALE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0) == occ.VEHICULE.ESCALE.VEHICULE.Count)
                        {
                            OPERATION_ESCALE matchedOpEscTerm = (from opEscT in dcMar.GetTable<OPERATION_ESCALE>()
                                                                 where opEscT.IdEsc == occ.VEHICULE.IdEsc && opEscT.IdTypeOp == 54
                                                                 select opEscT).SingleOrDefault<OPERATION_ESCALE>();

                            if (!matchedOpEscTerm.DateOp.HasValue)
                            {
                                matchedOpEscTerm.DateOp = DateTime.Now;
                                matchedOpEscTerm.IdU = 1;
                                matchedOpEscTerm.AIOp = "Déchargement terminé";
                            }
                        }
                    }

                    if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == occ.VEHICULE.IdVeh && v.IdTypeOp == 3) == 0)
                    {
                        // inserer opération de reception au parc
                        OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                        opVeh.IdVeh = occ.VEHICULE.IdVeh;
                        opVeh.IdTypeOp = 3;
                        opVeh.DateOp = occ.DateDebut;
                        opVeh.IdU = 1;
                        opVeh.IdLieu = 3;
                        opVeh.AIOp = "";

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);
                    }

                    occ.VEHICULE.StatVeh = "Parqué";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return;
            }
        }


        public VEHICULE TransfertEmplacement(int idVeh, int idEmpl, int idLieu, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).FirstOrDefault<VEHICULE>();

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                // inserer opération de transfert d'emplacement
                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = idVeh;
                opVeh.IdTypeOp = 3;
                opVeh.DateOp = DateTime.Now;
                opVeh.IdU = idUser;
                opVeh.IdLieu = idLieu;
                opVeh.AIOp = observations;

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                // inserer opération de transfert d'identification au parc
                if (matchedVehicule.OPERATION_VEHICULE.Count(op => op.IdOpVeh == 1 && op.IdLieu == 3) == 0)
                {
                    OPERATION_VEHICULE opVehId = new OPERATION_VEHICULE();
                    opVehId.IdVeh = idVeh;
                    opVehId.IdTypeOp = 1;
                    opVehId.DateOp = DateTime.Now;
                    opVehId.IdU = idUser;
                    opVehId.IdLieu = idLieu;
                    opVehId.AIOp = observations;

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVehId);
                }

                // clôturer l'occupation actuelle

                var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                          where (occ.IdVeh == idVeh && !occ.DateFin.HasValue)
                                          select occ).FirstOrDefault<OCCUPATION>();

                if (matchedOcccupation != null)
                {
                    matchedOcccupation.DateFin = DateTime.Now;
                }
                //else
                //{
                //    throw new ApplicationException("Ce véhicule n'est parqué nulle part. Par conséquent, vous ne pouvez pas le transférer vers un autre emplacement");
                //}

                if (!matchedVehicule.IdVehAP.HasValue)
                {
                    //inserer opération d'occupation d'emplacement
                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdVeh = idVeh;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 4;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                }

                if (matchedVehicule.StatVeh != "Enlèvement" && matchedVehicule.StatVeh != "Livraison")
                {
                    matchedVehicule.StatVeh = "Parqué";
                }

                foreach (VEHICULE v1 in dcMar.VEHICULE.Where(v => v.IdVehAP == matchedVehicule.IdVeh && v.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0))
                {
                    // inserer opération de transfert d'emplacement
                    OPERATION_VEHICULE opVeh1 = new OPERATION_VEHICULE();
                    opVeh1.IdVeh = v1.IdVeh;
                    opVeh1.IdTypeOp = 3;
                    opVeh1.DateOp = DateTime.Now;
                    opVeh1.IdU = idUser;
                    opVeh1.IdLieu = idLieu;
                    opVeh1.AIOp = observations;

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh1);

                    // inserer opération de transfert d'identification au parc
                    if (v1.OPERATION_VEHICULE.Count(op => op.IdOpVeh == 1 && op.IdLieu == 3) == 0)
                    {
                        OPERATION_VEHICULE opVehId = new OPERATION_VEHICULE();
                        opVehId.IdVeh = v1.IdVeh;
                        opVehId.IdTypeOp = 1;
                        opVehId.DateOp = DateTime.Now;
                        opVehId.IdU = idUser;
                        opVehId.IdLieu = idLieu;
                        opVehId.AIOp = observations;

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVehId);
                    }

                    if (v1.StatVeh != "Enlèvement" && v1.StatVeh != "Livraison")
                    {
                        v1.StatVeh = "Parqué";
                    }

                    foreach (VEHICULE v2 in dcMar.VEHICULE.Where(v => v.IdVehAP == v1.IdVeh && v.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0))
                    {
                        // inserer opération de transfert d'emplacement
                        OPERATION_VEHICULE opVeh2 = new OPERATION_VEHICULE();
                        opVeh2.IdVeh = v2.IdVeh;
                        opVeh2.IdTypeOp = 3;
                        opVeh2.DateOp = DateTime.Now;
                        opVeh2.IdU = idUser;
                        opVeh2.IdLieu = idLieu;
                        opVeh2.AIOp = observations;

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh1);

                        // inserer opération de transfert d'identification au parc
                        if (v2.OPERATION_VEHICULE.Count(op => op.IdOpVeh == 1 && op.IdLieu == 3) == 0)
                        {
                            OPERATION_VEHICULE opVehId = new OPERATION_VEHICULE();
                            opVehId.IdVeh = v2.IdVeh;
                            opVehId.IdTypeOp = 1;
                            opVehId.DateOp = DateTime.Now;
                            opVehId.IdU = idUser;
                            opVehId.IdLieu = idLieu;
                            opVehId.AIOp = observations;

                            dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVehId);
                        }

                        if (v2.StatVeh != "Enlèvement" && v2.StatVeh != "Livraison")
                        {
                            v2.StatVeh = "Parqué";
                        }
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }

        public VEHICULE InsertVehicule(int idBL, string numChassis, string descVeh, string barCode, string statutVeh, string typeMVeh, int poidsMVeh, double volMVeh, double longMVeh, double largMVeh, double hautMVeh, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Véhicule : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un nouveau véhicule. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement inexistant");
                }

                VEHICULE veh = new VEHICULE();

                veh.ESCALE = matchedConnaissement.ESCALE;
                veh.MANIFESTE = matchedConnaissement.MANIFESTE;
                veh.CONNAISSEMENT = matchedConnaissement;
                veh.NumChassis = numChassis;
                veh.DescVeh = descVeh;
                veh.TypeMVeh = typeMVeh;
                veh.TypeCVeh = typeMVeh;
                veh.StatutVeh = statutVeh;
                veh.PoidsMVeh = poidsMVeh;
                veh.PoidsCVeh = poidsMVeh;
                veh.VolMVeh = volMVeh;
                veh.VolCVeh = volMVeh;
                veh.LongMVeh = (float)longMVeh;
                veh.LongCVeh = (float)longMVeh;
                veh.LargMVeh = (float)largMVeh;
                veh.LargCVeh = (float)largMVeh;
                veh.HautMVeh = (float)hautMVeh;
                veh.HautCVeh = (float)hautMVeh;
                veh.NumItem = 1;
                veh.InfoMan = "";
                veh.BarCode = barCode;
                veh.DCVeh = DateTime.Now;

                veh.VehAttelle = "N";
                veh.VehPorte = "N";
                veh.VehChargé = "N";
                veh.VehCle = "N";
                veh.VehStart = "N";

                veh.StatVeh = "Non initié";

                if (matchedConnaissement.DVBL.HasValue)
                {
                    veh.StatVeh = "Manifesté";
                }

                veh.SensVeh = "I";
                veh.StatutCVeh = statutVeh;
                veh.IdAcc = matchedConnaissement.ESCALE.ACCONIER.IdAcc;

                if (veh.ESCALE.DRAEsc.HasValue)
                {
                    veh.FFVeh = veh.ESCALE.DRAEsc.Value.AddDays(11);
                    veh.FSVeh = veh.ESCALE.DRAEsc.Value.AddDays(11);
                }

                dcMar.GetTable<VEHICULE>().InsertOnSubmit(veh);

                dcMar.SubmitChanges();
                transaction.Complete();
                return veh;
            }
        }


        public VEHICULE InsertVehiculeAP(int idBL, int idVehAP, string numChassis, string descVeh, string barCode, string statutVeh, string typeMVeh, int poidsMVeh, double volMVeh, double longMVeh, double largMVeh, double hautMVeh, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Véhicule : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un nouveau véhicule porté. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement inexistant");
                }

                var matchedVehAP = (from v in dcMar.GetTable<VEHICULE>()
                                    where v.IdVeh == idVehAP
                                    select v).FirstOrDefault<VEHICULE>();

                if (matchedVehAP == null)
                {
                    throw new EnregistrementInexistant("Le véhicule auquel vous faites référence n'existe pas");
                }

                VEHICULE veh = new VEHICULE();

                veh.ESCALE = matchedConnaissement.ESCALE;
                veh.MANIFESTE = matchedConnaissement.MANIFESTE;
                veh.CONNAISSEMENT = matchedConnaissement;
                veh.NumChassis = numChassis;
                veh.IdVehAP = idVehAP;
                veh.VehAttelle = "Y";
                veh.VehPorte = "Y";

                veh.VehChargé = "N";
                veh.VehCle = "N";
                veh.VehStart = "N";

                veh.DescVeh = descVeh;
                veh.TypeMVeh = typeMVeh;
                veh.TypeCVeh = typeMVeh;
                veh.StatutVeh = statutVeh;
                veh.PoidsMVeh = poidsMVeh;
                veh.PoidsCVeh = poidsMVeh;
                veh.VolMVeh = volMVeh;
                veh.VolCVeh = volMVeh;
                veh.LongMVeh = (float)longMVeh;
                veh.LongCVeh = (float)longMVeh;
                veh.LargMVeh = (float)largMVeh;
                veh.LargCVeh = (float)largMVeh;
                veh.HautMVeh = (float)hautMVeh;
                veh.HautCVeh = (float)hautMVeh;
                veh.NumItem = 1;
                veh.InfoMan = "";
                veh.BarCode = barCode;
                veh.DCVeh = DateTime.Now;
                veh.StatVeh = "Non initié";

                if (matchedConnaissement.DVBL.HasValue)
                {
                    veh.StatVeh = "Initié";
                }

                veh.SensVeh = "I";
                veh.StatutCVeh = statutVeh;
                veh.FFVeh = matchedVehAP.FFVeh;
                veh.FSVeh = matchedVehAP.FSVeh;
                veh.IdAcc = matchedConnaissement.ESCALE.ACCONIER.IdAcc;

                if (veh.ESCALE.DRAEsc.HasValue)
                {
                    veh.FFVeh = veh.ESCALE.DRAEsc.Value.AddDays(11);
                    veh.FSVeh = veh.ESCALE.DRAEsc.Value.AddDays(11);
                }

                dcMar.GetTable<VEHICULE>().InsertOnSubmit(veh);

                dcMar.SubmitChanges();
                transaction.Complete();
                return veh;
            }
        }


        public VEHICULE UpdateVehicule(int idVeh, string numChassis, string descVeh, string statutVeh, string typeMVeh, int poidsMVeh, double volMVeh, double longMVeh, double largMVeh, double hautMVeh, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un véhicule. Veuillez contacter un administrateur");
                }

                matchedVehicule.NumChassis = numChassis;
                matchedVehicule.DescVeh = descVeh;
                matchedVehicule.StatutVeh = statutVeh;
                //matchedVehicule.TypeMVeh = typeMVeh;
                matchedVehicule.PoidsMVeh = poidsMVeh;
                matchedVehicule.VolMVeh = volMVeh;
                matchedVehicule.LongMVeh = (float)longMVeh;
                matchedVehicule.LargMVeh = (float)largMVeh;
                matchedVehicule.HautMVeh = (float)hautMVeh;

                if (matchedVehicule.StatVeh == "Initié" && matchedVehicule.MANIFESTE.DVMan.HasValue)
                {
                    matchedVehicule.StatVeh = "Traité";
                    if (matchedVehicule.CONNAISSEMENT.VEHICULE.Count(veh => veh.StatVeh != "Traité" && veh.IdVeh != idVeh) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }

        public VEHICULE UpdateInfosLivraisonVeh(int idVeh, string nomEnleveur, string cniEnleveur, string telEnleveur, string numAttDedouanement, string numCivio, string numDeclDouane, string numQuittDouane, string numFactPAD, string numQuittPAD, string numBAE, string numBESC, string numSydonia, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Véhicule : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations de livraison. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                if (matchedVehicule.FSVeh.Value.Date < DateTime.Now.Date && matchedVehicule.IdAcc.Value == 1 && matchedVehicule.IdEsc >= 350 && matchedVehicule.VehAttelle != "Y" && matchedVehicule.VehPorte != "Y")
                {
                    throw new ApplicationException("La date à laquelle vous avez calculé le séjour a été dépassée, veuillez recalculer le séjour de ce véhicule");
                }

                matchedVehicule.NomEnVeh = nomEnleveur;
                matchedVehicule.CNIEnVeh = cniEnleveur;
                matchedVehicule.TelenVeh = telEnleveur;
                matchedVehicule.NumADDVeh = numAttDedouanement;
                matchedVehicule.NumCIVIOveh = numCivio;
                matchedVehicule.NumDDVeh = numDeclDouane;
                matchedVehicule.NumQDVeh = numQuittDouane;
                matchedVehicule.NumFPADVeh = numFactPAD;
                matchedVehicule.NumQPADVeh = numQuittPAD;
                matchedVehicule.NumAEPADVeh = numBAE;
                matchedVehicule.NumBESCVeh = numBESC;
                matchedVehicule.NumSydoniaVeh = numSydonia;

                if (!matchedVehicule.IdVehAP.HasValue)
                {
                    var listVehicules = (from veh in dcMar.GetTable<VEHICULE>()
                                         where veh.IdBL == matchedVehicule.IdBL && veh.IdVehAP.HasValue
                                         select veh).ToList<VEHICULE>();

                    foreach (VEHICULE v in listVehicules)
                    {
                        v.NomEnVeh = nomEnleveur;
                        v.CNIEnVeh = cniEnleveur;
                        v.TelenVeh = telEnleveur;
                        v.NumADDVeh = numAttDedouanement;
                        v.NumCIVIOveh = numCivio;
                        v.NumDDVeh = numDeclDouane;
                        v.NumQDVeh = numQuittDouane;
                        v.NumFPADVeh = numFactPAD;
                        v.NumQPADVeh = numQuittPAD;
                        v.NumAEPADVeh = numBAE;
                        v.NumBESCVeh = numBESC;
                        v.NumSydoniaVeh = numSydonia;
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }

        public VEHICULE UpdateInfosEnlevementVeh(int idVeh, DateTime sortiePrev, string numBESC, string nomEnleveur, string cniEnleveur, string telEnleveur, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Véhicule : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations d'enlèvement. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                if (matchedVehicule.FSVeh.Value.Date < sortiePrev.Date && matchedVehicule.IdAcc.Value == 1 && matchedVehicule.IdEsc >= 350 && matchedVehicule.VehAttelle != "Y" && matchedVehicule.VehPorte != "Y")
                {
                    throw new ApplicationException("La date de sortie ne peut pas dépasser la date de fin de séjour, recalculer le séjour de ce véhicule");
                }

                matchedVehicule.DSPVeh = sortiePrev;
                matchedVehicule.NomEnVeh = nomEnleveur;
                matchedVehicule.CNIEnVeh = cniEnleveur;
                matchedVehicule.TelenVeh = telEnleveur;
                matchedVehicule.NumBESCVeh = numBESC;

                if (!matchedVehicule.IdVehAP.HasValue)
                {
                    var listVehicules = (from veh in dcMar.GetTable<VEHICULE>()
                                         where veh.IdBL == matchedVehicule.IdBL && veh.IdVehAP.HasValue
                                         select veh).ToList<VEHICULE>();

                    foreach (VEHICULE v in listVehicules)
                    {
                        v.DSPVeh = sortiePrev;
                        v.NomEnVeh = nomEnleveur;
                        v.CNIEnVeh = cniEnleveur;
                        v.TelenVeh = telEnleveur;
                        v.NumBESCVeh = numBESC;
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }

        public VEHICULE UpdateNumChassis(int idVeh, string numChassis, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Vehicule : Mise à jour numéro chassis").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour le numéro de chassis. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                matchedVehicule.NumChassis = numChassis;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE UpdateVehicule(int idVeh, string statutCVeh, string typeCVeh, int poidsCVeh, double volCVeh, double longCVeh, double largCVeh, double hautCVeh, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).FirstOrDefault<VEHICULE>();

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un véhicule. Veuillez contacter un administrateur");
                }

                matchedVehicule.StatutCVeh = statutCVeh;
                matchedVehicule.TypeCVeh = typeCVeh;
                matchedVehicule.PoidsCVeh = poidsCVeh;
                matchedVehicule.VolCVeh = volCVeh;
                matchedVehicule.LongCVeh = (float)longCVeh;
                matchedVehicule.LargCVeh = (float)largCVeh;
                matchedVehicule.HautCVeh = (float)hautCVeh;

                if (matchedVehicule.StatVeh == "Initié" && matchedVehicule.MANIFESTE.DVMan.HasValue)
                {
                    matchedVehicule.StatVeh = "Traité";
                    if (matchedVehicule.CONNAISSEMENT.VEHICULE.Count(veh => veh.StatVeh != "Traité" && veh.IdVeh != idVeh) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE IdentifierVehicule(int idVeh, int idLieu, string vehPorte, string vehCle, string vehAttelle, string vehStart, string vehCharge, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from vehs in dcMar.GetTable<VEHICULE>()
                                       where vehs.IdVeh == idVeh
                                       select vehs).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                //if (!matchedVehicule.ESCALE.DRAEsc.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Le navire n'est pas encore arrivé");
                //}

                if (!matchedVehicule.MANIFESTE.DVMan.HasValue)
                {
                    throw new IdentificationException("Echec de l'identification : Manifeste non validé");
                }

                var opIdentification = (from op in dcMar.GetTable<OPERATION_VEHICULE>()
                                        where op.IdTypeOp == 1 && op.IdVeh == idVeh && op.IdLieu == idLieu
                                        select op).FirstOrDefault<OPERATION_VEHICULE>();

                if (opIdentification != null)
                {
                    throw new IdentificationException("Ce véhicule a déjà été identifié au lieu : " + opIdentification.LIEU.NomLieu);
                }

                matchedVehicule.VehPorte = vehPorte;
                matchedVehicule.VehCle = vehCle;
                matchedVehicule.VehAttelle = vehAttelle;
                matchedVehicule.VehStart = vehStart;
                matchedVehicule.VehChargé = vehCharge;
                matchedVehicule.StatVeh = "Identifié/Déchargé";

                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = idVeh;
                opVeh.IdTypeOp = 1;
                opVeh.DateOp = DateTime.Now;
                opVeh.IdU = idUser;
                opVeh.IdLieu = idLieu;
                opVeh.AIOp = observations;

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 37
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.CONNAISSEMENT.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.CONNAISSEMENT.VEHICULE.Count)
                {
                    OPERATION_CONNAISSEMENT matchedOpBLTerm = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 38
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBLTerm.DateOp.HasValue)
                    {
                        matchedOpBLTerm.DateOp = DateTime.Now;
                        matchedOpBLTerm.IdU = idUser;
                        matchedOpBLTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedVehicule.IdMan && op.IdTypeOp == 45
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.MANIFESTE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.MANIFESTE.VEHICULE.Count)
                {
                    OPERATION_MANIFESTE matchedOpManTerm = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where op.IdMan == matchedVehicule.IdMan && op.IdTypeOp == 46
                                                            select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpManTerm.DateOp.HasValue)
                    {
                        matchedOpManTerm.DateOp = DateTime.Now;
                        matchedOpManTerm.IdU = idUser;
                        matchedOpManTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedVehicule.IdEsc && op.IdTypeOp == 53
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.ESCALE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.ESCALE.VEHICULE.Count)
                {
                    OPERATION_ESCALE matchedOpEscTerm = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where op.IdEsc == matchedVehicule.IdEsc && op.IdTypeOp == 54
                                                         select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEscTerm.DateOp.HasValue)
                    {
                        matchedOpEscTerm.DateOp = DateTime.Now;
                        matchedOpEscTerm.IdU = idUser;
                        matchedOpEscTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                List<OPERATION_VEHICULE> matchedOpIdentification = (from op in dcMar.GetTable<OPERATION_VEHICULE>()
                                                                    where op.IdTypeOp == 1
                                                                    select op).ToList<OPERATION_VEHICULE>();

                if (matchedOpIdentification.Count == 1 && matchedVehicule.ESCALE.DRAEsc.HasValue)
                {
                    if (matchedVehicule.StatutCVeh == "U")
                    {
                        if (matchedVehicule.VolCVeh >= 50)
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 112
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }

                        }
                        else if (matchedVehicule.VolCVeh >= 16)
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 111
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }

                        }
                        else
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 110
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (matchedVehicule.VolCVeh >= 50)
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 109
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }
                        }
                        else if (matchedVehicule.VolCVeh >= 16)
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 108
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();


                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }

                        }
                        else
                        {
                            var matchedOpArm = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                                where opArm.IdEsc == matchedVehicule.IdEsc && opArm.IdTypeOp == 107
                                                select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                            if (matchedOpArm != null)
                            {
                                if (matchedOpArm.QTE.HasValue)
                                {
                                    matchedOpArm.QTE = matchedOpArm.QTE + 1;
                                }
                                else
                                {
                                    matchedOpArm.QTE = 1;
                                }

                                if (matchedOpArm.Poids.HasValue)
                                {
                                    matchedOpArm.Poids = matchedOpArm.Poids + matchedVehicule.PoidsCVeh / 1000;
                                }
                                else
                                {
                                    matchedOpArm.Poids = matchedVehicule.PoidsCVeh / 1000;
                                }

                                if (matchedOpArm.Volume.HasValue)
                                {
                                    matchedOpArm.Volume = matchedOpArm.Volume + Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                                else
                                {
                                    matchedOpArm.Volume = Convert.ToInt32(matchedVehicule.VolCVeh);
                                }
                            }
                        }
                    }
                }

                //if (matchedVehicule.ESCALE.ACCONIER.CodeAcc != "SOCOMAR")
                //{
                //    DateTime dte = DateTime.Now;

                //    ARTICLE articleManutTerre = (from art in dcMar.GetTable<ARTICLE>()
                //                                 from par in dcMar.GetTable<PARAMETRE>()
                //                                 where art.CodeArticle == par.CodeAF && par.NomAF == "Manutention Terre"
                //                                 select art).FirstOrDefault<ARTICLE>();

                //    LIGNE_PRIX lpManutTerre = articleManutTerre.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte && lp.LP == matchedVehicule.ESCALE.ACCONIER.IdAcc.ToString());

                //    // Element de facturation manutention terre Socomar
                //    ELEMENT_FACTURATION eltFactManutTerre = new ELEMENT_FACTURATION();

                //    eltFactManutTerre.CCArticle = articleManutTerre.CODE_TVA.TauxTVA == 0 ? articleManutTerre.CCArticleEx : articleManutTerre.CCArticle;
                //    eltFactManutTerre.CCCP = matchedVehicule.CONNAISSEMENT.CLIENT.CodeClient;
                //    eltFactManutTerre.EltFacture = "Veh";
                //    eltFactManutTerre.LibEF = "Manutention Terre Chassis N° " + matchedVehicule.NumChassis;
                //    eltFactManutTerre.DateJEF = DateTime.Now;
                //    eltFactManutTerre.IdLP = lpManutTerre.IdLP;
                //    eltFactManutTerre.QTEEF = 1;
                //    eltFactManutTerre.PUEF = lpManutTerre.PU1LP;
                //    eltFactManutTerre.UnitEF = lpManutTerre.UniteLP;
                //    eltFactManutTerre.IdEsc = matchedVehicule.IdEsc;
                //    eltFactManutTerre.IdMan = matchedVehicule.IdMan;
                //    eltFactManutTerre.IdBL = matchedVehicule.CONNAISSEMENT.IdBL;
                //    eltFactManutTerre.IdVeh = matchedVehicule.IdVeh;
                //    eltFactManutTerre.CodeTVA = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : "TVAAP";
                //    eltFactManutTerre.TauxTVA = articleManutTerre.CodeTVA == "TVAEX" ? 0 : matchedVehicule.CONNAISSEMENT.CODE_TVA.TauxTVA;
                //    eltFactManutTerre.DestEF = "P";
                //    eltFactManutTerre.StatutEF = "En cours";

                //    dcMar.GetTable<ELEMENT_FACTURATION>().InsertOnSubmit(eltFactManutTerre);
                //}

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE IdentifierVehiculeAP(int idVeh, int idLieu, string vehPorte, string vehCle, string vehAttelle, string vehStart, string vehCharge, int idVehAP, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from vehs in dcMar.GetTable<VEHICULE>()
                                       where vehs.IdVeh == idVeh
                                       select vehs).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                //if (!matchedVehicule.ESCALE.DRAEsc.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Le navire n'est pas encore arrivé");
                //}

                //if (!matchedVehicule.CONNAISSEMENT.DVBL.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Connaissement non validé");
                //}

                var opIdentification = (from op in dcMar.GetTable<OPERATION_VEHICULE>()
                                        where op.IdTypeOp == 1 && op.IdVeh == idVeh && op.IdLieu == idLieu
                                        select op).FirstOrDefault<OPERATION_VEHICULE>();

                if (opIdentification != null)
                {
                    throw new IdentificationException("Ce véhicule a déjà été identifié au lieu : " + opIdentification.LIEU.NomLieu);
                }

                matchedVehicule.VehPorte = vehPorte;
                matchedVehicule.IdVehAP = idVehAP;
                matchedVehicule.VehCle = vehCle;
                matchedVehicule.VehAttelle = vehAttelle;
                matchedVehicule.VehStart = vehStart;
                matchedVehicule.VehChargé = vehCharge;
                matchedVehicule.StatVeh = "Identifié/Déchargé";

                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = idVeh;
                opVeh.IdTypeOp = 1;
                opVeh.DateOp = DateTime.Now;
                opVeh.IdU = idUser;
                opVeh.IdLieu = idLieu;
                opVeh.AIOp = observations;

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 37
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.CONNAISSEMENT.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.CONNAISSEMENT.VEHICULE.Count)
                {
                    OPERATION_CONNAISSEMENT matchedOpBLTerm = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedVehicule.IdBL && op.IdTypeOp == 38
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBLTerm.DateOp.HasValue)
                    {
                        matchedOpBLTerm.DateOp = DateTime.Now;
                        matchedOpBLTerm.IdU = idUser;
                        matchedOpBLTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedVehicule.IdMan && op.IdTypeOp == 45
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.MANIFESTE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.MANIFESTE.VEHICULE.Count)
                {
                    OPERATION_MANIFESTE matchedOpManTerm = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where op.IdMan == matchedVehicule.IdMan && op.IdTypeOp == 46
                                                            select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpManTerm.DateOp.HasValue)
                    {
                        matchedOpManTerm.DateOp = DateTime.Now;
                        matchedOpManTerm.IdU = idUser;
                        matchedOpManTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedVehicule.IdEsc && op.IdTypeOp == 53
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedVehicule.ESCALE.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 1) != 0) == matchedVehicule.ESCALE.VEHICULE.Count)
                {
                    OPERATION_ESCALE matchedOpEscTerm = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where op.IdEsc == matchedVehicule.IdEsc && op.IdTypeOp == 54
                                                         select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEscTerm.DateOp.HasValue)
                    {
                        matchedOpEscTerm.DateOp = DateTime.Now;
                        matchedOpEscTerm.IdU = idUser;
                        matchedOpEscTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE SetVehiculeAP(int idVeh, string vehPorte, string vehAttelle, int idVehAP, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from vehs in dcMar.GetTable<VEHICULE>()
                                       where vehs.IdVeh == idVeh
                                       select vehs).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                matchedVehicule.VehPorte = vehPorte;
                matchedVehicule.IdVehAP = idVehAP;
                matchedVehicule.VehAttelle = vehAttelle;

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE CuberVehicule(int idVeh, double longVeh, double largVeh, double hautVeh, double volVeh, int idLieu, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedCubageVehicule = (from veh in dcMar.GetTable<CUBAGE_VEHICULE>()
                                             where veh.IdVeh == idVeh && !veh.CUBAGE.DateCloCubage.HasValue && !veh.DateVal.HasValue
                                             select veh).FirstOrDefault<CUBAGE_VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations de cubage").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour cuber un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedCubageVehicule == null)
                {
                    throw new EnregistrementInexistant("Ce véhicule n'existe dans aucun projet de cubage en cours");
                }

                if (matchedCubageVehicule.DateVal.HasValue)
                {
                    throw new CubageException("Le cubage de ce véhicule a déjà été validé");
                }

                // inserer opération de cubage
                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = idVeh;
                opVeh.IdTypeOp = 2;
                opVeh.DateOp = DateTime.Now;
                opVeh.IdU = idUser;
                opVeh.IdLieu = idLieu;
                opVeh.AIOp = observations;

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                // Mise à jour des dimensions
                matchedCubageVehicule.LongCVeh = (float)Math.Round(longVeh, 3);
                matchedCubageVehicule.LargCVeh = (float)Math.Round(largVeh, 3);
                matchedCubageVehicule.HautCVeh = (float)Math.Round(hautVeh, 3);
                matchedCubageVehicule.VolCVeh = (float)Math.Round(volVeh, 3);
                if (volVeh < 15)
                {
                    matchedCubageVehicule.TypeCVeh = "C";
                }
                else if (volVeh < 25)
                {
                    matchedCubageVehicule.TypeCVeh = "V";
                }
                else if (volVeh < 70)
                {
                    matchedCubageVehicule.TypeCVeh = "T";
                }
                else if (volVeh < 140)
                {
                    matchedCubageVehicule.TypeCVeh = "L";
                }
                else if (volVeh < 32500)
                {
                    matchedCubageVehicule.TypeCVeh = "P";
                }
                matchedCubageVehicule.DateCV = DateTime.Now;
                matchedCubageVehicule.AICB = observations;

                dcMar.SubmitChanges();

                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == matchedCubageVehicule.IdVeh
                                       select veh).FirstOrDefault<VEHICULE>();

                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE ReceptionnerVehicule(int idVeh, int idEmpl, int idLieu, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations de réception").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour réceptionner un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                // inserer opération d'identification au parc
                if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == idVeh && v.IdTypeOp == 1 && v.IdLieu == idLieu) == 0)
                {
                    OPERATION_VEHICULE op = new OPERATION_VEHICULE();
                    op.IdVeh = idVeh;
                    op.IdTypeOp = 1;
                    op.DateOp = DateTime.Now;
                    op.IdU = idUser;
                    op.IdLieu = idLieu;
                    op.AIOp = observations;

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(op);
                }

                if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == idVeh && v.IdTypeOp == 3) != 0)
                {
                    throw new ApplicationException("Ce véhicule a déjà été réceptionné");
                }

                // inserer opération de reception au parc
                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = idVeh;
                opVeh.IdTypeOp = 3;
                opVeh.DateOp = DateTime.Now;
                opVeh.IdU = idUser;
                opVeh.IdLieu = idLieu;
                opVeh.AIOp = observations;

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                if (!matchedVehicule.IdVehAP.HasValue)
                {
                    //inserer opération d'occupation d'emplacement
                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdVeh = idVeh;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 3;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                }

                matchedVehicule.StatVeh = "Parqué";

                foreach (VEHICULE v1 in dcMar.VEHICULE.Where(v => v.IdVehAP == matchedVehicule.IdVeh && v.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0))
                {
                    // inserer opération d'identification au parc
                    if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == v1.IdVeh && v.IdTypeOp == 1 && v.IdLieu == idLieu) == 0)
                    {
                        OPERATION_VEHICULE op = new OPERATION_VEHICULE();
                        op.IdVeh = v1.IdVeh;
                        op.IdTypeOp = 1;
                        op.DateOp = DateTime.Now;
                        op.IdU = idUser;
                        op.IdLieu = idLieu;
                        op.AIOp = observations;

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(op);
                    }

                    if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == v1.IdVeh && v.IdTypeOp == 3) == 0)
                    {
                        // inserer opération de reception au parc
                        OPERATION_VEHICULE opVehAP = new OPERATION_VEHICULE();
                        opVehAP.IdVeh = v1.IdVeh;
                        opVehAP.IdTypeOp = 3;
                        opVehAP.DateOp = DateTime.Now;
                        opVehAP.IdU = idUser;
                        opVehAP.IdLieu = idLieu;
                        opVehAP.AIOp = observations;

                        dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVehAP);
                    }

                    v1.StatVeh = "Parqué";

                    foreach (VEHICULE v2 in dcMar.VEHICULE.Where(v => v.IdVehAP == v1.IdVeh && v.OPERATION_VEHICULE.Count(o => o.IdTypeOp == 1) != 0))
                    {
                        // inserer opération d'identification au parc
                        if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == v2.IdVeh && v.IdTypeOp == 1 && v.IdLieu == idLieu) == 0)
                        {
                            OPERATION_VEHICULE op = new OPERATION_VEHICULE();
                            op.IdVeh = v2.IdVeh;
                            op.IdTypeOp = 1;
                            op.DateOp = DateTime.Now;
                            op.IdU = idUser;
                            op.IdLieu = idLieu;
                            op.AIOp = observations;

                            dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(op);
                        }

                        if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == v2.IdVeh && v.IdTypeOp == 3) == 0)
                        {
                            // inserer opération de reception au parc
                            OPERATION_VEHICULE opVehAP = new OPERATION_VEHICULE();
                            opVehAP.IdVeh = v2.IdVeh;
                            opVehAP.IdTypeOp = 3;
                            opVehAP.DateOp = DateTime.Now;
                            opVehAP.IdU = idUser;
                            opVehAP.IdLieu = idLieu;
                            opVehAP.AIOp = observations;

                            dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVehAP);
                        }

                        v2.StatVeh = "Parqué";
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE ReceptionnerVehiculeGesparc(string numChassis, string numBL, DateTime dateEntree, string ligPos, string colPos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.NumChassis == numChassis && veh.CONNAISSEMENT.NumBL == numBL
                                       select veh).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Modification des informations de réception").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour réceptionner un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                // inserer opération d'identification au parc
                if (dcMar.GetTable<OPERATION_VEHICULE>().Count(v => v.IdVeh == matchedVehicule.IdVeh && v.IdTypeOp == 1 && v.IdLieu == 3) == 0)
                {
                    OPERATION_VEHICULE op = new OPERATION_VEHICULE();
                    op.IdVeh = matchedVehicule.IdVeh;
                    op.IdTypeOp = 1;
                    op.DateOp = dateEntree;
                    op.IdU = idUser;
                    op.IdLieu = 3;
                    op.AIOp = "";

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(op);
                }

                // inserer opération de reception au parc
                OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                opVeh.IdVeh = matchedVehicule.IdVeh;
                opVeh.IdTypeOp = 3;
                opVeh.DateOp = dateEntree;
                opVeh.IdU = idUser;
                opVeh.IdLieu = 3;
                opVeh.AIOp = "";

                dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                //inserer opération d'occupation d'emplacement

                EMPLACEMENT empl = (from e in dcMar.GetTable<EMPLACEMENT>()
                                    where e.LigEmpl == ligPos && e.ColEmpl == colPos
                                    select e).FirstOrDefault<EMPLACEMENT>();

                if (empl != null)
                {
                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdVeh = matchedVehicule.IdVeh;
                    occupation.IdEmpl = empl.IdEmpl;
                    occupation.IdTypeOp = 3;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);

                    matchedVehicule.StatVeh = "Parqué";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public VEHICULE ConstaterSinistres(int idVeh, List<TypeSinistreVehicule> listeSinistre, int idUser, string autresInfos)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from vehs in dcMar.GetTable<VEHICULE>()
                                       where vehs.IdVeh == idVeh
                                       select vehs).FirstOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Véhicule : Constat de sinistre").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour constater un sinistre sur un véhicule. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                if (!matchedVehicule.CONNAISSEMENT.DVBL.HasValue)
                {
                    throw new IdentificationException("Echec de l'identification : Connaissement non validé");
                }

                OPERATION_SINISTRE opSin = new OPERATION_SINISTRE();
                opSin.DateSinistre = DateTime.Now;
                opSin.IdU = idUser;
                opSin.IdVeh = idUser;
                opSin.AISinistre = autresInfos;

                dcMar.OPERATION_SINISTRE.InsertOnSubmit(opSin);
                dcMar.OPERATION_SINISTRE.Context.SubmitChanges();

                foreach (TypeSinistreVehicule typeSin in listeSinistre)
                {
                    if (typeSin.TypeSinistre != "")
                    {
                        SINISTRE sin = new SINISTRE();
                        sin.DateSinistre = DateTime.Now;
                        sin.DescSinistre = "";
                        sin.IdTypeSinistre = typeSin.idTypeSinistre;
                        sin.IdVeh = idVeh;
                        sin.Br = typeSin.Br == true ? "Y" : "N";
                        sin.D = typeSin.D == true ? "Y" : "N";
                        sin.M = typeSin.M == true ? "Y" : "N";
                        sin.R = typeSin.R == true ? "Y" : "N";
                        sin.Re = typeSin.Re == true ? "Y" : "N";
                        sin.Ru = typeSin.Ru == true ? "Y" : "N";
                        sin.S = typeSin.S == true ? "Y" : "N";
                        sin.T = typeSin.T == true ? "Y" : "N";
                        sin.Be = typeSin.Be == true ? "Y" : "N";
                        sin.IdOpSinistre = opSin.IdOpSinistre;

                        dcMar.SINISTRE.InsertOnSubmit(sin);
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedVehicule;
            }
        }


        public CONVENTIONNEL ConvertirEnConventionnel(int idVeh, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcMar.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Vehicule : Convertir en accessoire").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour convertir un véhicule en conventionnel. Veuillez contacter un administrateur");
                }

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("Véhicule inexistant");
                }

                if (matchedVehicule.CONNAISSEMENT.DVBL.HasValue)
                {
                    throw new ApplicationException("Conversion impossible : le connaissement associé à déjà été validé");
                }

                CONNAISSEMENT bl = matchedVehicule.CONNAISSEMENT;
                CONVENTIONNEL conv = new CONVENTIONNEL()
                {
                    CONNAISSEMENT = bl,
                    IdBL = bl.IdBL,
                    NumGC = matchedVehicule.NumChassis,
                    DescGC = matchedVehicule.DescVeh,
                    TypeMGC = "I404",
                    TypeCGC = "I404",
                    PoidsMGC = matchedVehicule.PoidsMVeh,
                    PoidsCGC = matchedVehicule.PoidsCVeh,
                    VolMGC = matchedVehicule.VolMVeh,
                    VolCGC = matchedVehicule.VolCVeh,
                    LongMGC = matchedVehicule.LongMVeh,
                    LongCGC = matchedVehicule.LongCVeh,
                    LargMGC = matchedVehicule.LargMVeh,
                    LargCGC = matchedVehicule.LargCVeh,
                    HautMGC = matchedVehicule.HautMVeh,
                    HautCGC = matchedVehicule.HautCVeh,
                    NumItem = matchedVehicule.NumItem,
                    InfoMan = matchedVehicule.InfoMan,
                    DCGC = DateTime.Now,
                    MANIFESTE = matchedVehicule.MANIFESTE,
                    ESCALE = matchedVehicule.ESCALE,
                    StatGC = "Non initié",
                    SensGC = matchedVehicule.SensVeh
                };

                dcMar.GetTable<CONVENTIONNEL>().InsertOnSubmit(conv);
                dcMar.GetTable<VEHICULE>().DeleteOnSubmit(matchedVehicule);

                dcMar.SubmitChanges();
                transaction.Complete();
                return conv;
            }
        }

        public VEHICULE UpdateInfosCIVIO(SGS sgs, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                VEHICULE matchedVeh = null;

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Importation CIVIO").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour effectuer une importation de SGS. Veuillez contacter un administrateur");
                }

                var matchedChassis = (from veh in dcMar.GetTable<VEHICULE>()
                                      where veh.NumChassis == sgs.NumChassis
                                      select veh).ToList<VEHICULE>();

                if (matchedChassis.Count == 1)
                {
                    matchedChassis.FirstOrDefault<VEHICULE>().NumCIVIOveh = sgs.NumCIVIO;
                    matchedChassis.FirstOrDefault<VEHICULE>().NumChassisSGS = sgs.NumChassis;
                    matchedChassis.FirstOrDefault<VEHICULE>().CouleurVeh = sgs.CouleurVeh;

                    matchedVeh = matchedChassis.FirstOrDefault<VEHICULE>();

                    return matchedVeh;
                }

                var matchedVehiculesChassis = (from veh in dcMar.GetTable<VEHICULE>()
                                               where veh.NumChassis.EndsWith(sgs.NumChassis.Substring(sgs.NumChassis.Length - 6, 6))// || veh.NumChassis == sgs.NumChassis
                                               select veh).ToList<VEHICULE>();

                if (matchedVehiculesChassis.Count == 1)
                {
                    matchedVehiculesChassis.FirstOrDefault<VEHICULE>().NumCIVIOveh = sgs.NumCIVIO;
                    matchedVehiculesChassis.FirstOrDefault<VEHICULE>().NumChassisSGS = sgs.NumChassis;
                    matchedVehiculesChassis.FirstOrDefault<VEHICULE>().CouleurVeh = sgs.CouleurVeh;

                    matchedVeh = matchedVehiculesChassis.FirstOrDefault<VEHICULE>();
                }
                else if (matchedVehiculesChassis.Count > 1)
                {
                    var matchedVehiculesMarque = matchedVehiculesChassis.Where(v => v.DescVeh.StartsWith(sgs.MarqueVeh.Split(' ')[0]) || v.DescVeh.EndsWith(sgs.MarqueVeh.Split(' ').Length > 1 ? sgs.MarqueVeh.Split(' ')[1] : sgs.MarqueVeh.Split(' ')[0])).ToList<VEHICULE>();

                    if (matchedVehiculesMarque.Count == 1)
                    {
                        matchedVehiculesMarque.FirstOrDefault<VEHICULE>().NumCIVIOveh = sgs.NumCIVIO;
                        matchedVehiculesMarque.FirstOrDefault<VEHICULE>().NumChassisSGS = sgs.NumChassis;
                        matchedVehiculesMarque.FirstOrDefault<VEHICULE>().CouleurVeh = sgs.CouleurVeh;

                        matchedVeh = matchedVehiculesMarque.FirstOrDefault<VEHICULE>();
                    }
                    else if (matchedVehiculesMarque.Count > 1)
                    {
                        var matchedVehiculesAcconier = matchedVehiculesMarque.Where(v => v.ESCALE.ACCONIER.NomAcc == sgs.Acconier).ToList<VEHICULE>();

                        if (matchedVehiculesAcconier.Count == 1)
                        {
                            matchedVehiculesAcconier.FirstOrDefault<VEHICULE>().NumCIVIOveh = sgs.NumCIVIO;
                            matchedVehiculesAcconier.FirstOrDefault<VEHICULE>().NumChassisSGS = sgs.NumChassis;
                            matchedVehiculesAcconier.FirstOrDefault<VEHICULE>().CouleurVeh = sgs.CouleurVeh;

                            matchedVeh = matchedVehiculesAcconier.FirstOrDefault<VEHICULE>();
                        }
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();

                return matchedVeh;
            }
        }


        #endregion

        #region conteneur mouvement 
 
       
        /// <summary>
        /// MAJ mouvement conteneur source dit
        /// </summary>
        /// <param name="opDIT"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CONTENEUR_TC UpdateInfosTrackingDIT(OperationDIT opDIT, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                CONTENEUR_TC matchedConteneurTC = null;

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Importation Tracking DIT").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour effectuer une importation des operations tracking de DIT. Veuillez contacter un administrateur");
                }

                var matchedConteneurs = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                         where ctr.NumTC == opDIT.NumCtr && ctr.IsDoublon == "N"
                                         select ctr).ToList<CONTENEUR_TC>();

                if (matchedConteneurs.Count == 1)
                {
                    matchedConteneurTC = matchedConteneurs.FirstOrDefault();

                    if (opDIT.CodeOperation == "DEBA")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Débarqué";

                        if (matchedConteneurTC != null)
                        {

                            //Com_AH verifie si leconteneur est identifié code operation 12
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 12 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 12;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Débarqué";
                                matchedConteneurTC.DateDébarquementDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserDébarquement == null)
                                {
                                    matchedConteneurTC.IdUserDébarquement = 127;
                                }

                                //Com_AH place le conteneur au parc dit, 
                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 12;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 12 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "EMBA")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loaded";

                        if (matchedConteneurTC != null)
                        {
                            /*ah 4juillet16 : cargo loaded uniquement pour les ctr export, donc on verifie sil existe une operation
                            de cargo loaded avec le conteneur export
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 283 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();
                            */
                            //AH 4juillet16
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 283 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();
                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                /*AH 4juillet16
                                 * on utilise le conteneur export et non celui import 
                                 opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                 */
                                opCtr.IdCtr = matchedConteneurTC.IdCtrExport; //AH 4juillet16

                                opCtr.IdTypeOp = 283;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Cargo Loaded";
                                matchedConteneurTC.DateEmbarquement = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserEmbarquement == null)
                                {
                                    matchedConteneurTC.IdUserEmbarquement = 127;
                                }

                                CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                       where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                       orderby ctrE.IdCtr descending
                                                       select ctrE).FirstOrDefault<CONTENEUR>();

                                if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = ctrExport.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 283;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = ctrExport.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 283;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 283;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 283 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "ENVC")
                    {
                        var matchedOpSortieCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                  where op.IdCtr == matchedConteneurTC.IdCtr
                                                  orderby op.IdOpCtr descending
                                                  select op).FirstOrDefault<OPERATION_CONTENEUR>();

                        if (matchedOpSortieCtr != null && matchedOpSortieCtr.IdTypeOp == 18)
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Retourné";
                            if (!matchedConteneurTC.CONTENEUR.DRCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DRCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 19 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 19;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Retourné";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserRetourVide == null)
                                    {
                                        matchedConteneurTC.IdUserRetourVide = 127;
                                    }
                                    if (matchedConteneurTC.IdParcRetourVide == null)
                                    {
                                        matchedConteneurTC.IdParcRetourVide = 4;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 19;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 19 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }

                            }
                        }
                        else
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Parqué";

                            matchedConteneurTC.StatutTC = "Parqué";
                            matchedConteneurTC.IdParcParquing = 4;
                            matchedConteneurTC.IdEmplacementParc = 2665;

                            OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                            opCtr.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                            opCtr.IdTypeOp = 285;
                            opCtr.DateOp = opDIT.DateOp;
                            opCtr.IdU = 127;
                            opCtr.AIOp = "Transfert";

                            dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = opDIT.DateOp;
                            mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                            mvtTC.IdParc = 4;
                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                            mvtTC.IdTypeOp = 285;
                            mvtTC.IdUser = 127;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                            dcMar.SubmitChanges();

                            var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                                      where (occ.IdCtr == matchedConteneurTC.CONTENEUR.IdCtr && !occ.DateFin.HasValue)
                                                      select occ).FirstOrDefault<OCCUPATION>();

                            if (matchedOcccupation != null)
                            {
                                matchedOcccupation.DateFin = DateTime.Now;
                            }

                            OCCUPATION occupation = new OCCUPATION();
                            occupation.DateDebut = opDIT.DateOp;
                            occupation.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                            occupation.IdEmpl = 2665;
                            occupation.IdTypeOp = 285;

                            dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                            dcMar.SubmitChanges();
                        }
                    }
                    else if (opDIT.CodeOperation == "ENVW")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Retourné";
                        if (!matchedConteneurTC.CONTENEUR.DRCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DRCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 19 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 19;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Retourné";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserRetourVide == null)
                                {
                                    matchedConteneurTC.IdUserRetourVide = 127;
                                }
                                if (matchedConteneurTC.IdParcRetourVide == null)
                                {
                                    matchedConteneurTC.IdParcRetourVide = 4;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 19;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 19 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "EPEC")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loading";

                        if (matchedConteneurTC != null)
                        {
                            /*AH 4juillet16 on ne fait du cargo loading que sur les conteneur export, donc on verifie sil existe une operation
                            de cargo loading avec le conteneur export
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();
                            */

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                /*AH 4juillet16 on indique le conteneur export au lieur de limport
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                 * */
                                //AH 4juillet16
                                opCtr.IdCtr = matchedConteneurTC.IdCtrExport;

                                opCtr.IdTypeOp = 282;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Cargo Loading";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserRetourPlein == null)
                                {
                                    matchedConteneurTC.IdUserRetourPlein = 127;
                                }
                                if (matchedConteneurTC.IdParcRetourPlein == null)
                                {
                                    matchedConteneurTC.IdParcRetourPlein = 4;
                                }

                                CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                       where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                       orderby ctrE.IdCtr descending
                                                       select ctrE).FirstOrDefault<CONTENEUR>();

                                if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = ctrExport.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = ctrExport.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }

                        }
                    }
                    else if (opDIT.CodeOperation == "EPEW")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loading";

                        if (matchedConteneurTC != null)
                        {
                            /* AH 4juillet16 on ne fait du cargo loading que sur les conteneur export, donc on verifie sil existe une operation
                            de cargo loading avec le conteneur export
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();
                            */
                            //AH 4juillet16
                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();
                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                /*AH 4juillet utiliser le conteneur export au lieu du conteneur inport 
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                 * */
                                //AH 4juillet16
                                opCtr.IdCtr = matchedConteneurTC.IdCtrExport;

                                opCtr.IdTypeOp = 282;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Cargo Loading";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserRetourPlein == null)
                                {
                                    matchedConteneurTC.IdUserRetourPlein = 127;
                                }
                                if (matchedConteneurTC.IdParcRetourPlein == null)
                                {
                                    matchedConteneurTC.IdParcRetourPlein = 4;
                                }

                                CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                       where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                       orderby ctrE.IdCtr descending
                                                       select ctrE).FirstOrDefault<CONTENEUR>();

                                if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = ctrExport.IdBL;
                                    mvtTC.IdEsc = ctrExport.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }
                                else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                {
                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 282;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                }

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "LPID")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                        if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 18;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Sorti";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserSortiePlein == null)
                                {
                                    matchedConteneurTC.IdUserSortiePlein = 127;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 18;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "LPIS")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                        if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 18;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Sorti";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserSortiePlein == null)
                                {
                                    matchedConteneurTC.IdUserSortiePlein = 127;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 18;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "LPIW")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                        if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 18;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Sorti";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserSortiePlein == null)
                                {
                                    matchedConteneurTC.IdUserSortiePlein = 127;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 18;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "SDIT")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                        if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 18;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Sorti";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserSortiePlein == null)
                                {
                                    matchedConteneurTC.IdUserSortiePlein = 127;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 18;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "LPIZ")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                        if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                        {
                            matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                        }

                        if (matchedConteneurTC != null)
                        {

                            var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpCtr == null)
                            {
                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                opCtr.IdTypeOp = 18;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = opDIT.Observations;

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                matchedConteneurTC.StatutTC = "Sorti";
                                matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                if (matchedConteneurTC.IdUserSortiePlein == null)
                                {
                                    matchedConteneurTC.IdUserSortiePlein = 127;
                                }

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 18;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();
                            }
                            else
                            {
                                matchedOpCtr.DateOp = opDIT.DateOp;
                                var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                     where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                     select op).FirstOrDefault<MOUVEMENT_TC>();

                                if (matchedMvtCtr != null)
                                {
                                    matchedMvtCtr.DateMvt = opDIT.DateOp;
                                }

                                dcMar.SubmitChanges();
                            }
                        }
                    }
                    else if (opDIT.CodeOperation == "SOVC")
                    {
                        matchedConteneurTC.CONTENEUR.StatCtr = "Parqué";

                        matchedConteneurTC.StatutTC = "Parqué";
                        matchedConteneurTC.IdParcParquing = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 6 : 5;
                        matchedConteneurTC.IdEmplacementParc = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 2671 : 2668;

                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                        opCtr.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                        opCtr.IdTypeOp = 285;
                        opCtr.DateOp = opDIT.DateOp;
                        opCtr.IdU = 127;
                        opCtr.AIOp = "Transfert";

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = opDIT.DateOp;
                        mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                        mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                        mvtTC.IdParc = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 6 : 5;
                        mvtTC.IdTC = matchedConteneurTC.IdTC;
                        mvtTC.IdTypeOp = 285;
                        mvtTC.IdUser = 127;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                        dcMar.SubmitChanges();

                        var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                                  where (occ.IdCtr == matchedConteneurTC.CONTENEUR.IdCtr && !occ.DateFin.HasValue)
                                                  select occ).FirstOrDefault<OCCUPATION>();

                        if (matchedOcccupation != null)
                        {
                            matchedOcccupation.DateFin = DateTime.Now;
                        }

                        OCCUPATION occupation = new OCCUPATION();
                        occupation.DateDebut = opDIT.DateOp;
                        occupation.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                        occupation.IdEmpl = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 2671 : 2668;
                        occupation.IdTypeOp = 285;

                        dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                        dcMar.SubmitChanges();

                    }
                }
                else
                {
                    var matchedConteneursTCIdem = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                                   where ctr.NumTC.Contains(opDIT.NumCtr) && ctr.IsDoublon == "N"
                                                   orderby ctr.IdCtr descending
                                                   select ctr).FirstOrDefault<CONTENEUR_TC>();


                    if (matchedConteneursTCIdem != null)
                    {
                        matchedConteneurTC = matchedConteneursTCIdem;

                        if (opDIT.CodeOperation == "DEBA")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Débarqué";

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 12 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 12;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Débarqué";
                                    matchedConteneurTC.DateDébarquementDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserDébarquement == null)
                                    {
                                        matchedConteneurTC.IdUserDébarquement = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 12;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 12 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "EMBA")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loaded";

                            if (matchedConteneurTC != null)
                            {
                                /* AH 4juillet16 cargo loaded uniquement pour les ctr export, donc on verifie sil existe une operation
                            de cargo loaded avec le conteneur export
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 283 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();
                                */
                                //AH 4juillet16
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 283 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();
                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    /* AH utilser le conteneur export et non celui import
                                     opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                     */
                                    opCtr.IdCtr = matchedConteneurTC.IdCtrExport; //AH 4juillet16 
                                    opCtr.IdTypeOp = 283;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Cargo Loaded";
                                    matchedConteneurTC.DateEmbarquement = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserEmbarquement == null)
                                    {
                                        matchedConteneurTC.IdUserEmbarquement = 127;
                                    }

                                    CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                           where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                           orderby ctrE.IdCtr descending
                                                           select ctrE).FirstOrDefault<CONTENEUR>();

                                    if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 283 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 283;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 283 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 283;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 283 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                            mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 283;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 283 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "ENVC")
                        {
                            var matchedOpSortieCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                      where op.IdCtr == matchedConteneurTC.IdCtr
                                                      orderby op.IdOpCtr descending
                                                      select op).FirstOrDefault<OPERATION_CONTENEUR>();

                            if (matchedOpSortieCtr != null && matchedOpSortieCtr.IdTypeOp == 18)
                            {
                                matchedConteneurTC.CONTENEUR.StatCtr = "Retourné";
                                if (!matchedConteneurTC.CONTENEUR.DRCtr.HasValue)
                                {
                                    matchedConteneurTC.CONTENEUR.DRCtr = opDIT.DateOp;
                                }

                                if (matchedConteneurTC != null)
                                {
                                    var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                        where op.IdTypeOp == 19 && op.IdCtr == matchedConteneurTC.IdCtr
                                                        select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                    if (matchedOpCtr == null)
                                    {
                                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                        opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                        opCtr.IdTypeOp = 19;
                                        opCtr.DateOp = opDIT.DateOp;
                                        opCtr.IdU = 127;
                                        opCtr.AIOp = opDIT.Observations;

                                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                        matchedConteneurTC.StatutTC = "Retourné";
                                        matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                        if (matchedConteneurTC.IdUserRetourVide == null)
                                        {
                                            matchedConteneurTC.IdUserRetourVide = 127;
                                        }
                                        if (matchedConteneurTC.IdParcRetourVide == null)
                                        {
                                            matchedConteneurTC.IdParcRetourVide = 4;
                                        }

                                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                        mvtTC.DateMvt = opDIT.DateOp;
                                        mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                        mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                        mvtTC.IdParc = 4;
                                        mvtTC.IdTC = matchedConteneurTC.IdTC;
                                        mvtTC.IdTypeOp = 19;
                                        mvtTC.IdUser = 127;

                                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                        dcMar.SubmitChanges();
                                    }
                                    else
                                    {
                                        matchedOpCtr.DateOp = opDIT.DateOp;
                                        var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                             where op.IdTypeOp == 19 && op.IdTC == matchedConteneurTC.IdTC
                                                             select op).FirstOrDefault<MOUVEMENT_TC>();

                                        if (matchedMvtCtr != null)
                                        {
                                            matchedMvtCtr.DateMvt = opDIT.DateOp;
                                        }

                                        dcMar.SubmitChanges();
                                    }

                                }
                            }
                            else
                            {
                                matchedConteneurTC.CONTENEUR.StatCtr = "Parqué";

                                matchedConteneurTC.StatutTC = "Parqué";
                                matchedConteneurTC.IdParcParquing = 4;
                                matchedConteneurTC.IdEmplacementParc = 2665;

                                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                opCtr.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                                opCtr.IdTypeOp = 285;
                                opCtr.DateOp = opDIT.DateOp;
                                opCtr.IdU = 127;
                                opCtr.AIOp = "Transfert";

                                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                mvtTC.DateMvt = opDIT.DateOp;
                                mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                mvtTC.IdParc = 4;
                                mvtTC.IdTC = matchedConteneurTC.IdTC;
                                mvtTC.IdTypeOp = 285;
                                mvtTC.IdUser = 127;

                                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                dcMar.SubmitChanges();

                                var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                                          where (occ.IdCtr == matchedConteneurTC.CONTENEUR.IdCtr && !occ.DateFin.HasValue)
                                                          select occ).FirstOrDefault<OCCUPATION>();

                                if (matchedOcccupation != null)
                                {
                                    matchedOcccupation.DateFin = DateTime.Now;
                                }

                                OCCUPATION occupation = new OCCUPATION();
                                occupation.DateDebut = opDIT.DateOp;
                                occupation.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                                occupation.IdEmpl = 2665;
                                occupation.IdTypeOp = 285;

                                dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                                dcMar.SubmitChanges();
                            }
                        }
                        else if (opDIT.CodeOperation == "ENVW")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Retourné";
                            if (!matchedConteneurTC.CONTENEUR.DRCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DRCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 19 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 19;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Retourné";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserRetourVide == null)
                                    {
                                        matchedConteneurTC.IdUserRetourVide = 127;
                                    }
                                    if (matchedConteneurTC.IdParcRetourVide == null)
                                    {
                                        matchedConteneurTC.IdParcRetourVide = 4;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 19;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 19 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "EPEC")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loading";

                            if (matchedConteneurTC != null)
                            {
                                /*AH 4juillet16 
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();
                                */
                                //AH 4juillet16
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();
                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    //AH 4juillet16 opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdCtr = matchedConteneurTC.IdCtrExport; //AH 4juillet16

                                    opCtr.IdTypeOp = 282;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Cargo Loading";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserRetourPlein == null)
                                    {
                                        matchedConteneurTC.IdUserRetourPlein = 127;
                                    }
                                    if (matchedConteneurTC.IdParcRetourPlein == null)
                                    {
                                        matchedConteneurTC.IdParcRetourPlein = 4;
                                    }

                                    CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                           where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                           orderby ctrE.IdCtr descending
                                                           select ctrE).FirstOrDefault<CONTENEUR>();

                                    if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {

                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                            mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }

                            }
                        }
                        else if (opDIT.CodeOperation == "EPEW")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Cargo Loading";

                            if (matchedConteneurTC != null)
                            {
                                /*
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();
                                */
                                //AH 4juillet16
                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 282 && op.IdCtr == matchedConteneurTC.IdCtrExport
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    //AH 4juillet16 opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdCtr = matchedConteneurTC.IdCtrExport; //AH 4juillet2016

                                    opCtr.IdTypeOp = 282;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Cargo Loading";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserRetourPlein == null)
                                    {
                                        matchedConteneurTC.IdUserRetourPlein = 127;
                                    }
                                    if (matchedConteneurTC.IdParcRetourPlein == null)
                                    {
                                        matchedConteneurTC.IdParcRetourPlein = 4;
                                    }

                                    CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                                           where ctrE.NumCtr == matchedConteneurTC.NumTC && ctrE.SensCtr == "E"
                                                           orderby ctrE.IdCtr descending
                                                           select ctrE).FirstOrDefault<CONTENEUR>();

                                    if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (ctrExport != null && matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = ctrExport.IdBL;
                                            mvtTC.IdEsc = ctrExport.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }
                                    else if (matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 2)
                                    {
                                        //AH 2aout16 control doublon mouvement_tc
                                        var matchedmvttc = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                            where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                            select op).FirstOrDefault<MOUVEMENT_TC>();
                                        if (matchedmvttc == null)
                                        {
                                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                            mvtTC.DateMvt = opDIT.DateOp;
                                            mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                            mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                            mvtTC.IdParc = 4;
                                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                                            mvtTC.IdTypeOp = 282;
                                            mvtTC.IdUser = 127;

                                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                                        }
                                    }

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 282 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "LPID")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                            if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 18;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Sorti";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserSortiePlein == null)
                                    {
                                        matchedConteneurTC.IdUserSortiePlein = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 18;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "LPIS")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                            if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 18;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Sorti";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserSortiePlein == null)
                                    {
                                        matchedConteneurTC.IdUserSortiePlein = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 18;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "LPIW")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                            if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 18;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Sorti";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserSortiePlein == null)
                                    {
                                        matchedConteneurTC.IdUserSortiePlein = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 18;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "SDIT")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                            if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 18;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Sorti";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserSortiePlein == null)
                                    {
                                        matchedConteneurTC.IdUserSortiePlein = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 18;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "LPIZ")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Livré";
                            if (!matchedConteneurTC.CONTENEUR.DSCtr.HasValue)
                            {
                                matchedConteneurTC.CONTENEUR.DSCtr = opDIT.DateOp;
                            }

                            if (matchedConteneurTC != null)
                            {

                                var matchedOpCtr = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                    where op.IdTypeOp == 18 && op.IdCtr == matchedConteneurTC.IdCtr
                                                    select op).FirstOrDefault<OPERATION_CONTENEUR>();

                                if (matchedOpCtr == null)
                                {
                                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                                    opCtr.IdTypeOp = 18;
                                    opCtr.DateOp = opDIT.DateOp;
                                    opCtr.IdU = 127;
                                    opCtr.AIOp = opDIT.Observations;

                                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                                    matchedConteneurTC.StatutTC = "Sorti";
                                    matchedConteneurTC.DateRetourVideDIT = opDIT.DateOp;
                                    if (matchedConteneurTC.IdUserSortiePlein == null)
                                    {
                                        matchedConteneurTC.IdUserSortiePlein = 127;
                                    }

                                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                                    mvtTC.DateMvt = opDIT.DateOp;
                                    mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                                    mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                                    mvtTC.IdParc = 4;
                                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                                    mvtTC.IdTypeOp = 18;
                                    mvtTC.IdUser = 127;

                                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                                    dcMar.SubmitChanges();
                                }
                                else
                                {
                                    matchedOpCtr.DateOp = opDIT.DateOp;
                                    var matchedMvtCtr = (from op in dcMar.GetTable<MOUVEMENT_TC>()
                                                         where op.IdTypeOp == 18 && op.IdTC == matchedConteneurTC.IdTC
                                                         select op).FirstOrDefault<MOUVEMENT_TC>();

                                    if (matchedMvtCtr != null)
                                    {
                                        matchedMvtCtr.DateMvt = opDIT.DateOp;
                                    }

                                    dcMar.SubmitChanges();
                                }
                            }
                        }
                        else if (opDIT.CodeOperation == "SOVC")
                        {
                            matchedConteneurTC.CONTENEUR.StatCtr = "Parqué";

                            matchedConteneurTC.StatutTC = "Parqué";
                            matchedConteneurTC.IdParcParquing = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 6 : 5;
                            matchedConteneurTC.IdEmplacementParc = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 2671 : 2668;

                            OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                            opCtr.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                            opCtr.IdTypeOp = 285;
                            opCtr.DateOp = opDIT.DateOp;
                            opCtr.IdU = 127;
                            opCtr.AIOp = "Transfert";

                            dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = opDIT.DateOp;
                            mvtTC.IdBL = matchedConteneurTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = matchedConteneurTC.CONTENEUR.IdEsc;
                            mvtTC.IdParc = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 6 : 5;
                            mvtTC.IdTC = matchedConteneurTC.IdTC;
                            mvtTC.IdTypeOp = 285;
                            mvtTC.IdUser = 127;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                            dcMar.SubmitChanges();

                            var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                                      where (occ.IdCtr == matchedConteneurTC.CONTENEUR.IdCtr && !occ.DateFin.HasValue)
                                                      select occ).FirstOrDefault<OCCUPATION>();

                            if (matchedOcccupation != null)
                            {
                                matchedOcccupation.DateFin = DateTime.Now;
                            }

                            OCCUPATION occupation = new OCCUPATION();
                            occupation.DateDebut = opDIT.DateOp;
                            occupation.IdCtr = matchedConteneurTC.CONTENEUR.IdCtr;
                            occupation.IdEmpl = matchedConteneurTC.CONTENEUR.ESCALE.IdArm == 1 ? 2671 : 2668;
                            occupation.IdTypeOp = 285;

                            dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                            dcMar.SubmitChanges();

                        }
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneurTC;
            }
        }

        #endregion

        #region booking


        public CONNAISSEMENT ValiderClearance(int idBooking, string numContrat, string numDeclExport, string numBesc, string numHSCode, string infosValid, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBooking
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Validation d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une clearance. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Booking inexistant");
                }

                if (matchedBooking.DVCBLI.HasValue)
                {
                    throw new ApplicationException("Cette clearance a déjà été validé le " + matchedBooking.DVCBLI);
                }

                matchedBooking.DVCBLI = DateTime.Now;
                //matchedBooking.StatutBL = "Cargo Loading";
                matchedBooking.StatutBL = "Final Booking";
                matchedBooking.NumCtrBL = numContrat;
                matchedBooking.NumDEBL = numDeclExport;
                matchedBooking.NumBESCBL = numBesc;
                matchedBooking.NumHSCode = numHSCode;

                if (infosValid.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdBL = matchedBooking.IdBL;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = infosValid;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                matchedBooking.NumBL = "DLA" + matchedBooking.DPBL.Substring(2, 3) + FormatReferenceBooking(matchedBooking.IdBL);

                dcMar.SubmitChanges();

                var typesSinCtr = (from type in dcMar.GetTable<TYPE_SINISTRE>()
                                   where type.TypeMse == "C"
                                   orderby type.IdTypeSinistre ascending
                                   select type).ToList<TYPE_SINISTRE>();

                foreach (CONTENEUR ctr in matchedBooking.CONTENEUR)
                {
                    //ctr.StatCtr = "Cargo Loading";
                    ctr.StatCtr = "Final Booking";

                    foreach (TYPE_SINISTRE sin in typesSinCtr)
                    {
                        INTERCHANGE interchange = new INTERCHANGE();

                        interchange.IdCtr = ctr.IdCtr;
                        interchange.IdTypeSinistre = sin.IdTypeSinistre;

                        dcMar.GetTable<INTERCHANGE>().InsertOnSubmit(interchange);
                    }
                }

                foreach (CONVENTIONNEL conv in matchedBooking.CONVENTIONNEL)
                {
                    //conv.StatGC = "Cargo Loading";
                    conv.StatGC = "Final Booking";
                }

                //dcMar.SubmitChanges();

                foreach (DISPOSITION_CONTENEUR d in matchedBooking.DISPOSITION_CONTENEUR)
                {
                    if (d.RefDisposition == null)
                    {
                        d.RefDisposition = "MAD" + FormatReferenceMAD(d.IdDisposition);
                    }
                }
                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBooking;
            }
        }


        public CONNAISSEMENT TransfertBookingEscale(int idBooking, int idEsc, string infosTransfert, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBooking
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Validation d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour transferer un booking d'une escale à une autre. Veuillez contacter un administrateur");
                }

                var matchedEscale = (from es in dcMar.GetTable<ESCALE>()
                                     where es.IdEsc == idEsc
                                     select es).FirstOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("Escale inexistante");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Booking inexistant");
                }

                NOTE noteValid = new NOTE();
                noteValid.IdBL = matchedBooking.IdBL;
                noteValid.DateNote = DateTime.Now;
                noteValid.IdU = idUser;
                noteValid.TitreNote = "Transfert escale";
                noteValid.DescNote = "Ce booking a été transferé de l'escale " + matchedBooking.ESCALE.NumEsc.ToString() + " vers l'escale " + matchedEscale.NumEsc.ToString() + "\n" + infosTransfert;

                dcMar.NOTE.InsertOnSubmit(noteValid);

                dcMar.SubmitChanges();

                matchedBooking.ESCALE = matchedEscale;

                foreach (CONTENEUR ctr in matchedBooking.CONTENEUR)
                {
                    ctr.IdEsc = idEsc;
                }

                foreach (CONVENTIONNEL conv in matchedBooking.CONVENTIONNEL)
                {
                    conv.IdEsc = idEsc;
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBooking;
            }
        }



        public ElementBookingGC UpdateVolPoidsConvBooking(int idGC, short qteB, short qteR, short numItem, double poidsB, double poidsR, double poidsE, double volB, double volR, double volE, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedElt = (from elt in dcMar.GetTable<CONVENTIONNEL>()
                                  where elt.IdGC == idGC
                                  select elt).SingleOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Mise à jour des quantités poids et volumes").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les quantités, poids et volume d'un general cargo à export. Veuillez contacter un administrateur");
                }

                if (matchedElt == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                matchedElt.QteBGC = qteB;
                matchedElt.QteRGC = qteR;
                matchedElt.NumItem = numItem;
                matchedElt.PoidsMGC = poidsB;
                matchedElt.PoidsRGC = poidsR;
                matchedElt.PoidsCGC = poidsE;
                matchedElt.VolMGC = volB;
                matchedElt.VolRGC = volR;
                matchedElt.VolCGC = volE;

                ElementBookingGC eltBookGC = new ElementBookingGC();

                eltBookGC.IdGC = matchedElt.IdGC;
                eltBookGC.Description = matchedElt.DescGC;
                eltBookGC.Hauteur = matchedElt.HautCGC.Value;
                eltBookGC.Largeur = matchedElt.LargCGC.Value;
                eltBookGC.Longueur = matchedElt.LongCGC.Value;
                eltBookGC.NumGC = matchedElt.NumGC;
                eltBookGC.Poids = (matchedElt.StatGC == "Non initié" || matchedElt.StatGC == "Final Booking") ? (matchedElt.PoidsMGC.HasValue ? matchedElt.PoidsMGC.Value : 0) : (matchedElt.StatGC == "Cargo Loading" ? (matchedElt.PoidsRGC.HasValue ? matchedElt.PoidsRGC.Value : 0) : (matchedElt.PoidsCGC.HasValue ? matchedElt.PoidsCGC.Value : 0));
                eltBookGC.Quantite = (matchedElt.StatGC == "Non initié" || matchedElt.StatGC == "Final Booking") ? (matchedElt.QteBGC.HasValue ? matchedElt.QteBGC.Value : (short)0) : (matchedElt.StatGC == "Cargo Loading" ? (matchedElt.QteRGC.HasValue ? matchedElt.QteRGC.Value : (short)0) : (matchedElt.NumItem.HasValue ? matchedElt.NumItem.Value : (short)0));
                eltBookGC.TypeMses = matchedElt.TYPE_CONVENTIONNEL.LibTypeGC;
                eltBookGC.Volume = (matchedElt.StatGC == "Non initié" || matchedElt.StatGC == "Final Booking") ? (matchedElt.VolMGC.HasValue ? matchedElt.VolMGC.Value : 0) : (matchedElt.StatGC == "Cargo Loading" ? (matchedElt.VolRGC.HasValue ? matchedElt.VolRGC.Value : 0) : (matchedElt.VolCGC.HasValue ? matchedElt.VolCGC.Value : 0));
                eltBookGC.StatGC = matchedElt.StatGC;

                dcMar.SubmitChanges();
                transaction.Complete();
                return eltBookGC;
            }
        }


        public CONNAISSEMENT EclaterBLExport(int idBL, List<ElementBookingCtr> ctrs, List<ElementBookingGC> gcs, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBL
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Eclater BL").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour éclater un BL export. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Booking inexistant");
                }

                int idEsc = matchedBooking.IdEsc.Value;

                dcMar.SubmitChanges();

                CONNAISSEMENT newBooking = new CONNAISSEMENT();

                newBooking.BLIL = matchedBooking.BLIL;
                newBooking.BLGN = matchedBooking.BLGN;
                newBooking.SensBL = "E";
                newBooking.CCBL = matchedBooking.CCBL;

                if (gcs.Count == 0)
                {
                    newBooking.TypeBL = "FF";
                }
                else if (ctrs.Count == 0)
                {
                    newBooking.TypeBL = "BB";
                }
                else
                {
                    newBooking.TypeBL = "FB";
                }

                newBooking.ConsigneeBL = matchedBooking.ConsigneeBL;
                newBooking.AdresseBL = matchedBooking.AdresseBL;
                newBooking.NotifyBL = matchedBooking.NotifyBL;
                newBooking.TelNotify = matchedBooking.TelNotify;
                newBooking.EmailNotify = matchedBooking.EmailNotify;
                newBooking.EmailBL = matchedBooking.EmailBL;
                newBooking.PhoneManBL = matchedBooking.PhoneManBL;
                newBooking.ConsigneeBooking = matchedBooking.ConsigneeBooking;
                newBooking.AdresseConsignee = matchedBooking.AdresseConsignee;
                newBooking.AdresseNotify = matchedBooking.AdresseNotify;
                newBooking.Payor = matchedBooking.Payor;
                newBooking.ClearAgent = matchedBooking.Payor;
                newBooking.NoSEPBC = matchedBooking.NoSEPBC;
                newBooking.CodeTVA = matchedBooking.BLIL == "Y" ? "TVAEX" : matchedBooking.CLIENT.CodeTVA;
                newBooking.LPBL = matchedBooking.LPBL;
                newBooking.DPBL = matchedBooking.DPBL;
                newBooking.DCBL = DateTime.Now;
                newBooking.StatutBL = matchedBooking.StatutBL;
                newBooking.EtatBL = matchedBooking.EtatBL;
                newBooking.DescBL = matchedBooking.DescBL;
                newBooking.LDBL = matchedBooking.LDBL;
                newBooking.CCBLMontant = matchedBooking.CCBLMontant;
                newBooking.IdAcc = matchedBooking.ESCALE.IdAcc;
                newBooking.IdEsc = matchedBooking.ESCALE.IdEsc;
                newBooking.PoidsBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.Poids) : ((double)ctrs.Sum(ctr => ctr.Poids)) / 1000, 3);
                newBooking.VolBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.Volume) : ((double)ctrs.Sum(ctr => ctr.Volume)), 3);
                newBooking.LPFret = matchedBooking.DPBL;
                newBooking.IdU = idUser;
                newBooking.BLSocar = matchedBooking.BLSocar;
                newBooking.BLFO = matchedBooking.BLFO;
                newBooking.BLLT = matchedBooking.BLLT;
                newBooking.BlBloque = matchedBooking.BlBloque;
                newBooking.BLER = matchedBooking.BLER;
                newBooking.IdClient = matchedBooking.IdClient;

                // mettre à jour la clearance
                newBooking.DCBLI = matchedBooking.DCBLI;
                newBooking.NumCtrBL = matchedBooking.NumCtrBL;
                newBooking.NumDEBL = matchedBooking.NumDEBL;
                newBooking.NumBESCBL = matchedBooking.NumBESCBL;
                newBooking.NumHSCode = matchedBooking.NumHSCode;
                newBooking.AIBL = matchedBooking.AIBL;

                dcMar.GetTable<CONNAISSEMENT>().InsertOnSubmit(newBooking);
                dcMar.CONNAISSEMENT.Context.SubmitChanges();

                newBooking.NumBooking = newBooking.DPBL.Substring(2, 3).ToUpper() + FormatReferenceBooking(newBooking.IdBL);
                newBooking.NumBL = "DLA" + newBooking.NumBooking;
                dcMar.CONNAISSEMENT.Context.SubmitChanges();

                NOTE noteValid = new NOTE();
                noteValid.IdBL = matchedBooking.IdBL;
                noteValid.DateNote = DateTime.Now;
                noteValid.IdU = idUser;
                noteValid.TitreNote = "Eclatement BL";
                noteValid.DescNote = "BL export éclaté vers BL Id " + newBooking.IdBL;

                dcMar.NOTE.InsertOnSubmit(noteValid);

                List<CONVENTIONNEL> convs = new List<CONVENTIONNEL>();

                foreach (ElementBookingGC gc in gcs)
                {
                    var matchedConv = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                       where conv.IdGC == gc.IdGC
                                       select conv).SingleOrDefault<CONVENTIONNEL>();

                    if (matchedConv.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr == "Cargo Loaded") + matchedConv.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC == "Cargo Loaded") == matchedConv.CONNAISSEMENT.CONTENEUR.Count + matchedConv.CONNAISSEMENT.CONVENTIONNEL.Count - 1)
                    {
                        matchedConv.CONNAISSEMENT.StatutBL = "Cargo Loaded";
                    }

                    dcMar.SubmitChanges();

                    matchedConv.StatGC = gc.StatGC;
                    matchedConv.CONNAISSEMENT = newBooking;
                    matchedConv.IdEsc = newBooking.IdEsc;
                }

                List<CONTENEUR> conteneurs = new List<CONTENEUR>();

                foreach (ElementBookingCtr ct in ctrs)
                {
                    var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                            where ctr.IdCtr == ct.IdCtr
                                            select ctr).SingleOrDefault<CONTENEUR>();

                    if (matchedConteneur.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr == "Cargo Loaded") + matchedConteneur.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC == "Cargo Loaded") == matchedConteneur.CONNAISSEMENT.CONTENEUR.Count + matchedConteneur.CONNAISSEMENT.CONVENTIONNEL.Count - 1)
                    {
                        matchedConteneur.CONNAISSEMENT.StatutBL = "Cargo Loaded";
                    }

                    dcMar.SubmitChanges();

                    matchedConteneur.StatCtr = ct.StatCtr;
                    matchedConteneur.CONNAISSEMENT = newBooking;
                    matchedConteneur.IdEsc = newBooking.IdEsc;
                }

                dcMar.SubmitChanges();

                if (matchedBooking.CONVENTIONNEL.Count == 0)
                {
                    matchedBooking.TypeBL = "FF";
                }
                else if (matchedBooking.CONTENEUR.Count == 0)
                {
                    matchedBooking.TypeBL = "BB";
                }
                else
                {
                    matchedBooking.TypeBL = "FB";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return newBooking;
            }
        }


        public CONNAISSEMENT TransfertBookingGC(int idGC, int idBL, string infosTransfert, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBL
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedConventionnel = (from gc in dcMar.GetTable<CONVENTIONNEL>()
                                            where gc.IdGC == idGC
                                            select gc).SingleOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Validation d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour transferer une marchandise d'un booking à un autre. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Booking inexistant");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                if (matchedBooking.StatutBL != "Booking" && matchedBooking.StatutBL != "Clearance")
                {
                    throw new ApplicationException("Echec de l'opération : La clearance du booking cible a déjà été validée");
                }

                NOTE noteValid = new NOTE();
                noteValid.IdBL = matchedConventionnel.IdBL;
                noteValid.DateNote = DateTime.Now;
                noteValid.IdU = idUser;
                noteValid.TitreNote = "Transfert de booking";
                noteValid.DescNote = "La marchandise " + matchedConventionnel.NumGC + " " + matchedConventionnel.DescGC + " a été transférée du booking " + matchedConventionnel.CONNAISSEMENT.NumBooking + " vers le booking " + matchedBooking.NumBooking + "\n" + infosTransfert;

                dcMar.NOTE.InsertOnSubmit(noteValid);

                dcMar.SubmitChanges();

                if (matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC == "Cargo Loaded") + matchedConventionnel.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr == "Cargo Loaded") == matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count + matchedConventionnel.CONNAISSEMENT.CONTENEUR.Count - 1)
                {
                    matchedConventionnel.CONNAISSEMENT.StatutBL = "Cargo Loaded";
                }

                dcMar.SubmitChanges();

                if (matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count == 0)
                {
                    matchedConventionnel.CONNAISSEMENT.TypeBL = "FF";
                }
                else if (matchedConventionnel.CONNAISSEMENT.CONTENEUR.Count == 0)
                {
                    matchedConventionnel.CONNAISSEMENT.TypeBL = "BB";
                }
                else
                {
                    matchedConventionnel.CONNAISSEMENT.TypeBL = "FB";
                }

                dcMar.SubmitChanges();

                CONNAISSEMENT actualBooking = matchedConventionnel.CONNAISSEMENT;

                matchedConventionnel.StatGC = "Non initié";
                matchedConventionnel.CONNAISSEMENT = matchedBooking;
                matchedConventionnel.IdEsc = matchedBooking.IdEsc;

                dcMar.SubmitChanges();

                if (matchedBooking.CONVENTIONNEL.Count == 0)
                {
                    matchedBooking.TypeBL = "FF";
                }
                else if (matchedBooking.CONTENEUR.Count == 0)
                {
                    matchedBooking.TypeBL = "BB";
                }
                else
                {
                    matchedBooking.TypeBL = "FB";
                }

                if (actualBooking.CONVENTIONNEL.Count == 0)
                {
                    actualBooking.TypeBL = "FF";
                }
                else if (actualBooking.CONTENEUR.Count == 0)
                {
                    actualBooking.TypeBL = "BB";
                }
                else
                {
                    actualBooking.TypeBL = "FB";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return actualBooking;
            }
        }



        public CONVENTIONNEL ReceptionExportConventionnel(int idGC, string descGC, double volume, double poids, int qte, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from ctr in dcMar.GetTable<CONVENTIONNEL>()
                                            where ctr.IdGC == idGC
                                            select ctr).FirstOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Gestion des embarquements").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour réceptionner un conventionnel. Veuillez contacter un administrateur");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (!matchedConventionnel.ESCALE.DRAEsc.HasValue)
                {
                    throw new ApplicationException("Echec de la réception : Le navire n'est pas encore arrivé");
                }

                if (!matchedConventionnel.CONNAISSEMENT.DVCBLI.HasValue)
                {
                    throw new ApplicationException("Echec de la réception : La clearance n'a pas encore été validée");
                }

                if (matchedConventionnel.StatGC != "Final Booking")
                {
                    throw new ApplicationException("Ce conventionnel a déjà été réceptionné");
                }

                if (matchedConventionnel.ESCALE.SOP == "C")
                {
                    throw new ApplicationException("Echec de la réception : Le summary of operations a déjà été clôturé");
                }

                matchedConventionnel.DescGCRecept = descGC;
                matchedConventionnel.VolRGC = volume;
                matchedConventionnel.PoidsRGC = poids;
                matchedConventionnel.QteRGC = (short)qte;
                matchedConventionnel.StatGC = "Cargo Loading";

                dcMar.SubmitChanges();

                if (matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC == "Cargo Loading") + matchedConventionnel.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr == "Cargo Loading") == matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count + matchedConventionnel.CONNAISSEMENT.CONTENEUR.Count)
                {
                    matchedConventionnel.CONNAISSEMENT.StatutBL = "Cargo Loading";
                }

                if (matchedConventionnel.QteRGC < matchedConventionnel.QteBGC)
                {
                    NOTE noteShortDelivered = new NOTE();
                    noteShortDelivered.IdGC = matchedConventionnel.IdGC;
                    noteShortDelivered.DateNote = DateTime.Now;
                    noteShortDelivered.IdU = idUser;
                    noteShortDelivered.TitreNote = "Short Delivered";
                    noteShortDelivered.DescNote = "Booked : " + matchedConventionnel.QteBGC + "\n" + "Delivered : " + matchedConventionnel.QteRGC;

                    dcMar.NOTE.InsertOnSubmit(noteShortDelivered);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONNAISSEMENT AnnulerBooking(int idBook, string infosAnnulation, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedBooking = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                      where bl.IdBL == idBook
                                      select bl).SingleOrDefault<CONNAISSEMENT>();

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Le booking auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Annulation d'un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour annuler un booking. Veuillez contacter un administrateur");
                }

                if (matchedBooking.StatutBL == "Annulé")
                {
                    throw new ConnaissementException("Annulation impossible : Ce booking a déjà été annulé");
                }

                if (matchedBooking.StatutBL != "Booking")
                {
                    throw new ConnaissementException("Annulation impossible : Le traitement de ce connaissement a déjà débuté");
                }

                ////Date actuelle du système
                //DateTime dte = DateTime.Now;

                //OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                //                                       where op.IdBL == matchedBL.IdBL && op.IdTypeOp == 42
                //                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                //matchedOpBL.DateOp = DateTime.Now;
                //matchedOpBL.IdU = idUser;
                //matchedOpBL.AIOp = noteCloture;

                //dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                matchedBooking.StatutBL = "Annulé";

                if (infosAnnulation.Trim() != "")
                {
                    NOTE noteAnnulation = new NOTE();
                    noteAnnulation.IdBL = matchedBooking.IdBL;
                    noteAnnulation.DateNote = DateTime.Now;
                    noteAnnulation.IdU = idUser;
                    noteAnnulation.TitreNote = "Note d'annulation";
                    noteAnnulation.DescNote = infosAnnulation;

                    dcMar.NOTE.InsertOnSubmit(noteAnnulation);
                }

                foreach (CONTENEUR ctr in matchedBooking.CONTENEUR)
                {
                    ctr.StatCtr = "Annulé";
                }

                foreach (CONVENTIONNEL conv in matchedBooking.CONVENTIONNEL)
                {
                    conv.StatGC = "Annulé";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBooking;
            }
        }

        public CONNAISSEMENT TransfertBookingCtr(int idCtr, int idBL, string infosTransfert, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBL
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).SingleOrDefault<CONTENEUR>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Booking : Validation d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour transferer une marchandise d'un booking à un autre. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Booking inexistant");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                if (matchedBooking.StatutBL != "Booking" && matchedBooking.StatutBL != "Clearance")
                {
                    throw new ApplicationException("Echec de l'opération : La clearance du booking cible a déjà été validée");
                }

                NOTE noteValid = new NOTE();
                noteValid.IdBL = matchedConteneur.IdBL;
                noteValid.DateNote = DateTime.Now;
                noteValid.IdU = idUser;
                noteValid.TitreNote = "Transfert de booking";
                noteValid.DescNote = "La marchandise " + matchedConteneur.NumCtr + " " + matchedConteneur.DescCtr + " a été transférée du booking " + matchedConteneur.CONNAISSEMENT.NumBooking + " vers le booking " + matchedBooking.NumBooking + "\n" + infosTransfert;

                dcMar.NOTE.InsertOnSubmit(noteValid);

                dcMar.SubmitChanges();

                if (matchedConteneur.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr == "Cargo Loaded") + matchedConteneur.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC == "Cargo Loaded") == matchedConteneur.CONNAISSEMENT.CONTENEUR.Count + matchedConteneur.CONNAISSEMENT.CONVENTIONNEL.Count - 1)
                {
                    matchedConteneur.CONNAISSEMENT.StatutBL = "Cargo Loaded";
                }

                dcMar.SubmitChanges();

                if (matchedConteneur.CONNAISSEMENT.CONVENTIONNEL.Count == 0)
                {
                    matchedConteneur.CONNAISSEMENT.TypeBL = "FF";
                }
                else if (matchedConteneur.CONNAISSEMENT.CONTENEUR.Count == 0)
                {
                    matchedConteneur.CONNAISSEMENT.TypeBL = "BB";
                }
                else
                {
                    matchedConteneur.CONNAISSEMENT.TypeBL = "FB";
                }

                dcMar.SubmitChanges();

                CONNAISSEMENT actualBooking = matchedConteneur.CONNAISSEMENT;

                matchedConteneur.StatCtr = "Non initié";
                matchedConteneur.CONNAISSEMENT = matchedBooking;
                matchedConteneur.IdEsc = matchedBooking.IdEsc;

                dcMar.SubmitChanges();

                if (matchedBooking.CONVENTIONNEL.Count == 0)
                {
                    matchedBooking.TypeBL = "FF";
                }
                else if (matchedBooking.CONTENEUR.Count == 0)
                {
                    matchedBooking.TypeBL = "BB";
                }
                else
                {
                    matchedBooking.TypeBL = "FB";
                }

                if (actualBooking.CONVENTIONNEL.Count == 0)
                {
                    actualBooking.TypeBL = "FF";
                }
                else if (actualBooking.CONTENEUR.Count == 0)
                {
                    actualBooking.TypeBL = "BB";
                }
                else
                {
                    actualBooking.TypeBL = "FB";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return actualBooking;
            }
        }
          
        public CONNAISSEMENT InsertBooking(int idEsc, int idClient, string descBL, string lpbl, string dpbl, string ldbl, string blgn, string blil, string ccbl, string ccblMontant, string consigneeBL, string adresseBL, string emailBL, string phoneManBL, string consigneeBooking, string adresseConsignee, string notifyBL, string adresseNotify, string emailNotify, string telNotify, string notifyBL2, string adresseNotify2, string emailNotify2, string telNotify2, string payor, string clearAgent, string noSEPBC, List<ElementBookingCtr> ctrs, List<ElementBookingGC> gcs, List<DISPOSITION_CONTENEUR> dispCtr, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                var matchedClient = (from clt in dcMar.GetTable<CLIENT>()
                                     where clt.IdClient == idClient
                                     select clt).FirstOrDefault<CLIENT>();

                if (matchedClient == null)
                {
                    throw new EnregistrementInexistant("Le client auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un nouveau booking. Veuillez contacter un administrateur");
                }

                // Insertion du booking

                CONNAISSEMENT booking = new CONNAISSEMENT();

                booking.BLIL = blil;
                booking.BLGN = blgn;
                booking.SensBL = "E";
                booking.CCBL = ccbl;

                if (gcs.Count == 0)
                {
                    booking.TypeBL = "FF";
                }
                else if (ctrs.Count == 0)
                {
                    booking.TypeBL = "BB";
                }
                else
                {
                    booking.TypeBL = "FB";
                }

                //booking.TypeBL = gcs.Count > 0 ? "BB" : "FF";
                booking.ConsigneeBL = consigneeBL;
                booking.AdresseBL = adresseBL;
                booking.NotifyBL = notifyBL;
                booking.NotifyBL2 = notifyBL2;
                booking.TelNotify = telNotify;
                booking.EmailNotify = emailNotify;
                booking.TelNotify2 = telNotify2;
                booking.EmailNotify2 = emailNotify2;
                booking.EmailBL = emailBL;
                booking.PhoneManBL = phoneManBL;
                booking.ConsigneeBooking = consigneeBooking;
                booking.AdresseConsignee = adresseConsignee;
                booking.AdresseNotify = adresseNotify;
                booking.AdresseNotify2 = adresseNotify2;
                booking.Payor = payor;
                booking.ClearAgent = clearAgent;
                booking.NoSEPBC = noSEPBC;
                booking.CodeTVA = blil == "Y" ? "TVAEX" : matchedClient.CodeTVA;
                booking.LPBL = lpbl;
                booking.DPBL = dpbl;
                booking.DCBL = DateTime.Now;
                booking.StatutBL = "Booking";
                booking.EtatBL = "O";
                booking.DescBL = descBL;
                booking.LDBL = ldbl;
                booking.CCBLMontant = ccbl == "Y" ? Convert.ToInt32(ccblMontant) : 0;
                booking.IdAcc = matchedEscale.IdAcc;
                booking.IdEsc = matchedEscale.IdEsc;
                booking.PoidsBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.Poids) : ((double)ctrs.Sum(ctr => ctr.Poids)) / 1000, 3);
                booking.VolBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.Volume) : ((double)ctrs.Sum(ctr => ctr.Volume)), 3);
                booking.LPFret = dpbl;
                booking.IdU = idUser;
                booking.BLSocar = "N";
                booking.BLFO = "N";
                booking.BLLT = "N";
                booking.BlBloque = "N";
                booking.BLER = "N";
                booking.IdClient = idClient;

                dcMar.GetTable<CONNAISSEMENT>().InsertOnSubmit(booking);
                dcMar.CONNAISSEMENT.Context.SubmitChanges();

                booking.NumBooking = booking.DPBL.Substring(2, 3).ToUpper() + FormatReferenceBooking(booking.IdBL);
                booking.NumBL = "DLA" + booking.NumBooking;
                dcMar.CONNAISSEMENT.Context.SubmitChanges();

                List<CONVENTIONNEL> convs = new List<CONVENTIONNEL>();

                foreach (ElementBookingGC gc in gcs)
                {
                    CONVENTIONNEL conv = new CONVENTIONNEL();

                    conv.SensGC = "E";
                    conv.NumGC = gc.NumGC;
                    conv.DescGC = gc.Description;
                    conv.DescGCEmbarq = gc.Description;
                    conv.LongMGC = gc.Longueur;
                    conv.LargMGC = gc.Largeur;
                    conv.HautMGC = gc.Hauteur;
                    conv.LongCGC = gc.Longueur;
                    conv.LargCGC = gc.Largeur;
                    conv.HautCGC = gc.Hauteur;
                    conv.NumItem = gc.Quantite;
                    conv.QteBGC = gc.Quantite;
                    conv.VolMGC = gc.Volume;
                    conv.VolCGC = gc.Volume;
                    conv.PoidsMGC = gc.Poids;
                    conv.PoidsCGC = gc.Poids;
                    conv.TypeMGC = dcMar.GetTable<TYPE_CONVENTIONNEL>().SingleOrDefault<TYPE_CONVENTIONNEL>(tc => tc.LibTypeGC == gc.TypeMses).CodeTypeGC;
                    conv.TypeCGC = conv.TypeMGC;
                    conv.IdBL = booking.IdBL;
                    conv.IdEsc = booking.IdEsc;
                    conv.DCGC = DateTime.Now;
                    conv.StatGC = "Non initié";

                    convs.Add(conv);
                }

                dcMar.GetTable<CONVENTIONNEL>().InsertAllOnSubmit(convs);
                dcMar.CONVENTIONNEL.Context.SubmitChanges();

                //foreach (CONVENTIONNEL conv in convs)
                //{
                //    conv.NumGC = "GC" + FormatRefGC(conv.IdGC);
                //}

                dcMar.CONVENTIONNEL.Context.SubmitChanges();

                List<CONTENEUR> conteneurs = new List<CONTENEUR>();

                foreach (ElementBookingCtr ct in ctrs)
                {
                    CONTENEUR ctr = new CONTENEUR();

                    ctr.SensCtr = "E";
                    ctr.NumCtr = ct.NumCtr;
                    ctr.PropCtr = 1;
                    ctr.MCCtr = 1500000;
                    ctr.DescCtr = ct.Description;
                    ctr.IMDGCode = ct.UNCode;
                    ctr.DescMses = ct.DescMses;
                    ctr.StatutCtr = ct.StatutCtr;
                    ctr.TypeMCtr = ct.TypeCtr;
                    ctr.TypeCCtr = ct.TypeCtr;
                    ctr.TypeMses = ct.TypeMsesCtr;
                    ctr.VolMCtr = ct.Volume;
                    ctr.PoidsMCtr = ct.Poids;
                    ctr.PoidsCCtr = ct.Poids;
                    ctr.IdBL = booking.IdBL;
                    ctr.IdEsc = booking.IdEsc;
                    ctr.Seal1Ctr = ct.Seal1;
                    ctr.Seal2Ctr = ct.Seal2;
                    ctr.DCCtr = DateTime.Now;
                    ctr.StatCtr = "Non initié";
                    //AH 7juillet16
                    ctr.VGM = ct.VGM;

                    conteneurs.Add(ctr);
                }

                dcMar.GetTable<CONTENEUR>().InsertAllOnSubmit(conteneurs);
                dcMar.CONTENEUR.Context.SubmitChanges();

                // insertion de la mise à disposition de conteneur

                List<DISPOSITION_CONTENEUR> dispositionConteneur = new List<DISPOSITION_CONTENEUR>();

                foreach (DISPOSITION_CONTENEUR dCtr in dispCtr)
                {
                    DISPOSITION_CONTENEUR d = new DISPOSITION_CONTENEUR();
                    d.IdBooking = booking.IdBL;
                    d.NombreTC = dCtr.NombreTC;
                    d.TypeCtr = dCtr.TypeCtr;

                    dispositionConteneur.Add(d);
                }

                dcMar.GetTable<DISPOSITION_CONTENEUR>().InsertAllOnSubmit(dispositionConteneur);
                dcMar.DISPOSITION_CONTENEUR.Context.SubmitChanges();

                List<DISPOSITION_CONTENEUR> listDispoCtr = dcMar.DISPOSITION_CONTENEUR.Where(d => d.IdBooking.Value == booking.IdBL).ToList<DISPOSITION_CONTENEUR>();

                foreach (DISPOSITION_CONTENEUR dCtr in listDispoCtr)
                {
                    dCtr.RefDisposition = "MAD" + FormatReferenceMAD(dCtr.IdDisposition);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return booking;
            }
        }


        public CONNAISSEMENT UpdateBooking(int idBL, int idEsc, int idClient, string descBL, string lpbl, string dpbl, string ldbl, string blgn, string blil, string ccbl, string ccblMontant, string consigneeBL, string adresseBL, string emailBL, string phoneManBL, string consigneeBooking, string adresseConsignee, string notifyBL, string adresseNotify, string emailNotify, string telNotify, string notifyBL2, string adresseNotify2, string emailNotify2, string telNotify2, string payor, string clearAgent, string noSEPBC, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, List<DISPOSITION_CONTENEUR> dispCtr, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBL
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Ajout d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour effectuer une clearance. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Le booking auquel vous faites référence n'existe pas");
                }

                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedClient = (from clt in dcMar.GetTable<CLIENT>()
                                     where clt.IdClient == idClient
                                     select clt).FirstOrDefault<CLIENT>();

                if (matchedClient == null)
                {
                    throw new EnregistrementInexistant("Le client auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un booking. Veuillez contacter un administrateur");
                }

                dcMar.CONVENTIONNEL.DeleteAllOnSubmit<CONVENTIONNEL>(matchedBooking.CONVENTIONNEL);
                dcMar.CONTENEUR.DeleteAllOnSubmit<CONTENEUR>(matchedBooking.CONTENEUR);
                dcMar.DISPOSITION_CONTENEUR.DeleteAllOnSubmit<DISPOSITION_CONTENEUR>(matchedBooking.DISPOSITION_CONTENEUR.Where(dd => dd.CONTENEUR_TC.Count == 0));

                dcMar.SubmitChanges();

                matchedBooking.BLIL = blil;
                matchedBooking.BLGN = blgn;
                matchedBooking.SensBL = "E";
                matchedBooking.CCBL = ccbl;

                if (gcs.Count == 0)
                {
                    matchedBooking.TypeBL = "FF";
                }
                else if (ctrs.Count == 0)
                {
                    matchedBooking.TypeBL = "BB";
                }
                else
                {
                    matchedBooking.TypeBL = "FB";
                }

                //matchedBooking.TypeBL = gcs.Count > 0 ? "BB" : "FF";
                matchedBooking.ConsigneeBL = consigneeBL;
                matchedBooking.AdresseBL = adresseBL;
                matchedBooking.NotifyBL = notifyBL;
                matchedBooking.NotifyBL2 = notifyBL2;
                matchedBooking.TelNotify = telNotify;
                matchedBooking.EmailNotify = emailNotify;
                matchedBooking.TelNotify2 = telNotify2;
                matchedBooking.EmailNotify2 = emailNotify2;
                matchedBooking.EmailBL = emailBL;
                matchedBooking.PhoneManBL = phoneManBL;
                matchedBooking.ConsigneeBooking = consigneeBooking;
                matchedBooking.AdresseConsignee = adresseConsignee;
                matchedBooking.AdresseNotify = adresseNotify;
                matchedBooking.AdresseNotify2 = adresseNotify2;
                matchedBooking.Payor = payor;
                matchedBooking.ClearAgent = clearAgent;
                matchedBooking.NoSEPBC = noSEPBC;
                matchedBooking.CodeTVA = blil == "Y" ? "TVAEX" : matchedClient.CodeTVA;
                matchedBooking.LPBL = lpbl;
                matchedBooking.DPBL = dpbl;
                matchedBooking.EtatBL = "O";
                matchedBooking.DescBL = descBL;
                matchedBooking.LDBL = ldbl;
                matchedBooking.CCBLMontant = ccbl == "Y" ? Convert.ToInt32(ccblMontant) : 0;
                matchedBooking.IdAcc = matchedEscale.IdAcc;
                matchedBooking.IdEsc = matchedEscale.IdEsc;
                matchedBooking.ESCALE = matchedEscale;
                matchedBooking.PoidsBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.PoidsMGC.Value) : ((double)ctrs.Sum(ctr => ctr.PoidsMCtr.Value)) / 1000, 3);
                matchedBooking.VolBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.VolMGC.Value) : ((double)ctrs.Sum(ctr => ctr.VolMCtr.Value)), 3);
                matchedBooking.LPFret = dpbl;
                matchedBooking.DCBLI = DateTime.Now;
                matchedBooking.IdU = idUser;
                matchedBooking.BLSocar = "N";
                matchedBooking.BLFO = "N";
                matchedBooking.BLLT = "N";
                matchedBooking.BlBloque = "N";
                matchedBooking.BLER = "N";
                matchedBooking.CLIENT = matchedClient;
                matchedBooking.IdClient = idClient;
                matchedBooking.NumBooking = matchedBooking.DPBL.Substring(2, 3).ToUpper() + FormatReferenceBooking(matchedBooking.IdBL);
                matchedBooking.NumBL = "DLA" + matchedBooking.NumBooking;

                dcMar.SubmitChanges();

                foreach (CONVENTIONNEL conv in gcs)
                {
                    conv.CONNAISSEMENT = matchedBooking;
                    conv.ESCALE = matchedEscale;
                    string type = conv.TypeMGC;
                    conv.TypeMGC = dcMar.GetTable<TYPE_CONVENTIONNEL>().SingleOrDefault<TYPE_CONVENTIONNEL>(tc => tc.LibTypeGC == type).CodeTypeGC;
                    conv.TypeCGC = conv.TypeMGC;
                }

                foreach (CONTENEUR ctr in ctrs)
                {
                    ctr.CONNAISSEMENT = matchedBooking;
                    ctr.ESCALE = matchedEscale;
                }

                foreach (DISPOSITION_CONTENEUR d in dispCtr)
                {
                    d.CONNAISSEMENT = matchedBooking;
                }

                matchedBooking.CONTENEUR.AddRange(ctrs);
                matchedBooking.CONVENTIONNEL.AddRange(gcs);
                matchedBooking.DISPOSITION_CONTENEUR.AddRange(dispCtr);

                dcMar.SubmitChanges();

                foreach (DISPOSITION_CONTENEUR d in matchedBooking.DISPOSITION_CONTENEUR)
                {
                    if (d.RefDisposition == null)
                    {
                        d.RefDisposition = "MAD" + FormatReferenceMAD(d.IdDisposition);
                    }
                }

                foreach (CONTENEUR ctr in ctrs)
                {
                    //AH 25juillet16, il arrive que ds conteneur_tc ua deux valeur, il faut prendre le dernier.
                    CONTENEUR_TC matechedctc = (from tc in dcMar.GetTable<CONTENEUR_TC>()
                                                where tc.NumTC == ctr.NumCtr && !tc.DateRetourPlein.HasValue
                                                orderby tc.IdCtr descending
                                                select tc).FirstOrDefault<CONTENEUR_TC>();
                    //if (dcMar.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>(tc => tc.NumTC == ctr.NumCtr && !tc.DateRetourPlein.HasValue) != null)
                    if (matechedctc != null)
                    {
                        //dcMar.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>(tc => tc.NumTC == ctr.NumCtr && !tc.DateRetourPlein.HasValue).IdCtrExport = ctr.IdCtr;
                        matechedctc.IdCtrExport = ctr.IdCtr;
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBooking;
            }
        }

        public CONNAISSEMENT UpdateClearance(int idBL, int idEsc, int idClient, string descBL, string lpbl, string dpbl, string ldbl, string blgn, string blil, string ccbl, string ccblMontant, string consigneeBL, string adresseBL, string emailBL, string phoneManBL, string consigneeBooking, string adresseConsignee, string notifyBL, string adresseNotify, string emailNotify, string telNotify, string payor, string clearAgent, string noSEPBC, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, string numContrat, string numBesc, string numDeclExport, string numHSCode, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedBooking = (from book in dcMar.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBL
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Ajout d'information de clearance").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour effectuer une clearance. Veuillez contacter un administrateur");
                }

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Le booking auquel vous faites référence n'existe pas");
                }

                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedClient = (from clt in dcMar.GetTable<CLIENT>()
                                     where clt.IdClient == idClient
                                     select clt).FirstOrDefault<CLIENT>();

                if (matchedClient == null)
                {
                    throw new EnregistrementInexistant("Le client auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Booking : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un nouveau booking. Veuillez contacter un administrateur");
                }

                dcMar.CONVENTIONNEL.DeleteAllOnSubmit<CONVENTIONNEL>(matchedBooking.CONVENTIONNEL);
                dcMar.CONTENEUR.DeleteAllOnSubmit<CONTENEUR>(matchedBooking.CONTENEUR);

                dcMar.SubmitChanges();

                matchedBooking.BLIL = blil;
                matchedBooking.BLGN = blgn;
                matchedBooking.SensBL = "E";
                matchedBooking.CCBL = ccbl;

                if (gcs.Count == 0)
                {
                    matchedBooking.TypeBL = "FF";
                }
                else if (ctrs.Count == 0)
                {
                    matchedBooking.TypeBL = "BB";
                }
                else
                {
                    matchedBooking.TypeBL = "FB";
                }

                //matchedBooking.TypeBL = gcs.Count > 0 ? "BB" : "FF";
                matchedBooking.ConsigneeBL = consigneeBL;
                matchedBooking.AdresseBL = adresseBL;
                matchedBooking.NotifyBL = notifyBL;
                matchedBooking.TelNotify = telNotify;
                matchedBooking.EmailNotify = emailNotify;
                matchedBooking.EmailBL = emailBL;
                matchedBooking.PhoneManBL = phoneManBL;
                matchedBooking.ConsigneeBooking = consigneeBooking;
                matchedBooking.AdresseConsignee = adresseConsignee;
                matchedBooking.AdresseNotify = adresseNotify;
                matchedBooking.Payor = payor;
                matchedBooking.ClearAgent = clearAgent;
                matchedBooking.NoSEPBC = noSEPBC;
                matchedBooking.CodeTVA = blil == "Y" ? "TVAEX" : matchedClient.CodeTVA;
                matchedBooking.LPBL = lpbl;
                matchedBooking.DPBL = dpbl;
                matchedBooking.EtatBL = "O";
                matchedBooking.DescBL = descBL;
                matchedBooking.LDBL = ldbl;
                matchedBooking.CCBLMontant = ccbl == "Y" ? Convert.ToInt32(ccblMontant) : 0;
                matchedBooking.IdAcc = matchedEscale.IdAcc;
                matchedBooking.IdEsc = matchedEscale.IdEsc;
                matchedBooking.ESCALE = matchedEscale;
                matchedBooking.PoidsBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.PoidsMGC.Value) : ((double)ctrs.Sum(ctr => ctr.PoidsMCtr.Value)) / 1000, 3);
                matchedBooking.VolBL = Math.Round(gcs.Count > 0 ? gcs.Sum(gc => gc.VolMGC.Value) : ((double)ctrs.Sum(ctr => ctr.VolMCtr.Value)) / 1000, 3);
                matchedBooking.LPFret = dpbl;
                matchedBooking.StatutBL = "Clearance";
                matchedBooking.DCBLI = DateTime.Now;
                matchedBooking.NumCtrBL = numContrat;
                matchedBooking.NumDEBL = numDeclExport;
                matchedBooking.NumBESCBL = numBesc;
                matchedBooking.NumHSCode = numHSCode;
                matchedBooking.AIBL = autresInfos;
                matchedBooking.IdU = idUser;
                matchedBooking.BLSocar = "N";
                matchedBooking.BLFO = "N";
                matchedBooking.BLLT = "N";
                matchedBooking.BlBloque = "N";
                matchedBooking.BLER = "N";
                matchedBooking.CLIENT = matchedClient;
                matchedBooking.IdClient = idClient;
                matchedBooking.NumBooking = matchedBooking.DPBL.Substring(2, 3).ToUpper() + FormatReferenceBooking(matchedBooking.IdBL);

                matchedBooking.NumBL = "DLA" + matchedBooking.NumBooking;

                dcMar.SubmitChanges();

                foreach (CONVENTIONNEL conv in gcs)
                {
                    conv.CONNAISSEMENT = matchedBooking;
                    conv.ESCALE = matchedEscale;
                    string type = conv.TypeMGC;
                    conv.TypeMGC = dcMar.GetTable<TYPE_CONVENTIONNEL>().SingleOrDefault<TYPE_CONVENTIONNEL>(tc => tc.LibTypeGC == type).CodeTypeGC;
                    conv.TypeCGC = conv.TypeMGC;
                }

                foreach (CONTENEUR ctr in ctrs)
                {
                    ctr.CONNAISSEMENT = matchedBooking;
                    ctr.ESCALE = matchedEscale;
                }

                matchedBooking.CONTENEUR.AddRange(ctrs);
                matchedBooking.CONVENTIONNEL.AddRange(gcs);

                dcMar.SubmitChanges();

                foreach (CONTENEUR ctr in ctrs)
                {
                    if (dcMar.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>(tc => tc.NumTC == ctr.NumCtr && !tc.DateRetourPlein.HasValue) != null)
                    {
                        dcMar.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>(tc => tc.NumTC == ctr.NumCtr && !tc.DateRetourPlein.HasValue).IdCtrExport = ctr.IdCtr;
                    }
                }

                dcMar.SubmitChanges();

                foreach (DISPOSITION_CONTENEUR d in matchedBooking.DISPOSITION_CONTENEUR)
                {
                    if (d.RefDisposition == null)
                    {
                        d.RefDisposition = "MAD" + FormatReferenceMAD(d.IdDisposition);
                    }
                }
                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedBooking;
            }
        }

        #endregion

        #region Extension franchise

        public EXTENSION_FRANCHISE InsertExtensionFranchise(int idBL, int nbSej, int nbSures, int nbDet, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                EXTENSION_FRANCHISE extensionFranchise = new EXTENSION_FRANCHISE();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande d'extention de franchise : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une demande d'extension de franchise. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                extensionFranchise.DateDEXT = DateTime.Now;
                extensionFranchise.CONNAISSEMENT = matchedConnaissement;
                extensionFranchise.NbreSej = nbSej;
                extensionFranchise.NbreStat = 0;
                extensionFranchise.NbreSures = nbSures;
                extensionFranchise.NbreDet = nbDet;
                extensionFranchise.ObsDEXT = autresInfos;
                extensionFranchise.IdU = idUser;

                dcMar.GetTable<EXTENSION_FRANCHISE>().InsertOnSubmit(extensionFranchise);

                dcMar.SubmitChanges();
                transaction.Complete();
                return extensionFranchise;
            }
        }

        public EXTENSION_FRANCHISE ValiderExtensionFranchise(int idDEXT, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDemande = (from ext in dcMar.GetTable<EXTENSION_FRANCHISE>()
                                      where ext.IdDEXT == idDEXT
                                      select ext).SingleOrDefault<EXTENSION_FRANCHISE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande d'extention de franchise : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une demande d'extension de franchise. Veuillez contacter un administrateur");
                }

                if (matchedDemande == null)
                {
                    throw new EnregistrementInexistant("Demande d'extension de franchise inexistante");
                }

                matchedDemande.IdUV = idUser;
                matchedDemande.AIVDEXT = autresInfos;
                matchedDemande.DatevDEXT = DateTime.Now;

                if (autresInfos.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdDEXT = matchedDemande.IdDEXT;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = autresInfos;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                // Mise à jour des dates de fin de franchise de tous les véhicules du connaissement
                foreach (VEHICULE v in matchedDemande.CONNAISSEMENT.VEHICULE)
                {
                    var matchedVeh = (from veh in dcMar.GetTable<VEHICULE>()
                                      where veh.IdVeh == v.IdVeh
                                      select veh).SingleOrDefault<VEHICULE>();

                    if (matchedVeh != null)
                    {
                        matchedVeh.FFVeh = matchedVeh.FFVeh.Value.AddDays(matchedDemande.NbreSej.Value);
                    }
                }
                dcMar.VEHICULE.Context.SubmitChanges();

                // Mise à jour des dates de fin de séjour, de fin de franchise et des surestaries de tous les conteneurs du connaissement
                foreach (CONTENEUR c in matchedDemande.CONNAISSEMENT.CONTENEUR)
                {
                    var matchedCtr = (from ctr in dcMar.GetTable<CONTENEUR>()
                                      where ctr.IdCtr == c.IdCtr
                                      select ctr).SingleOrDefault<CONTENEUR>();

                    if (matchedCtr != null)
                    {
                        matchedCtr.FFCtr = matchedCtr.FFCtr.Value.AddDays(matchedDemande.NbreSures.Value);
                        //matchedCtr.FFSCtr = matchedCtr.FFSCtr.Value.AddDays(matchedDemande.NbreSures.Value);
                        matchedCtr.FFSCtr = matchedCtr.FFSCtr.Value.AddDays(matchedDemande.NbreSures.Value);
                        matchedCtr.NbDet = matchedDemande.NbreDet.Value == 0 ? matchedCtr.NbDet.Value : matchedDemande.NbreDet.Value;
                    }
                }
                dcMar.CONTENEUR.Context.SubmitChanges();

                // Mise à jour des dates de fin de franchise de tous les conventionnels de l'escale
                foreach (CONVENTIONNEL c in matchedDemande.CONNAISSEMENT.CONVENTIONNEL)
                {
                    var matchedConv = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                       where conv.IdGC == c.IdGC
                                       select conv).SingleOrDefault<CONVENTIONNEL>();

                    if (matchedConv != null)
                    {
                        matchedConv.FFGC = matchedConv.FFGC.Value.AddDays(matchedDemande.NbreSej.Value);
                    }
                }
                dcMar.CONVENTIONNEL.Context.SubmitChanges();

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedDemande;
            }
        }

        #endregion

        #region demande de reduction


        public DEMANDE_REDUCTION InsertDemandeReduction(int idBL, short codeArticle, string libelle, double pourcentage, int montantHT, float tauxTVA, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                DEMANDE_REDUCTION demandeReduction = new DEMANDE_REDUCTION();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de réduction : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une demande de reduction. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                demandeReduction.DateDDR = DateTime.Now;
                demandeReduction.CONNAISSEMENT = matchedConnaissement;
                demandeReduction.LibDDR = libelle;
                demandeReduction.Pourcent = pourcentage;
                demandeReduction.MHT = montantHT;
                demandeReduction.TauxTVA = tauxTVA;
                demandeReduction.CodeArticle = codeArticle;
                demandeReduction.ObsDDR = autresInfos;
                demandeReduction.IdU = idUser;

                demandeReduction.StatutRed = "En cours";

                dcMar.GetTable<DEMANDE_REDUCTION>().InsertOnSubmit(demandeReduction);

                dcMar.SubmitChanges();
                transaction.Complete();
                return demandeReduction;
            }
        }


        public DEMANDE_REDUCTION UpdateDemandeReduction(int idDDR, int idBL, short codeArticle, string libelle, double pourcentage, int montantHT, float tauxTVA, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedReduction = (from dr in dcMar.GetTable<DEMANDE_REDUCTION>()
                                        where dr.IdDDR == idDDR
                                        select dr).SingleOrDefault<DEMANDE_REDUCTION>();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de réduction : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour modifier une demande de reduction. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                NOTE noteModif = new NOTE();
                noteModif.IdDDR = matchedReduction.IdDDR;
                noteModif.DateNote = DateTime.Now;
                noteModif.IdU = idUser;
                noteModif.TitreNote = "Note de modification";
                noteModif.DescNote = "Ancienne valeur : \nLibellé : " + matchedReduction.LibDDR + "\nValeur : " + matchedReduction.Pourcent;

                dcMar.NOTE.InsertOnSubmit(noteModif);

                matchedReduction.CONNAISSEMENT = matchedConnaissement;
                matchedReduction.LibDDR = libelle;
                matchedReduction.Pourcent = pourcentage;
                matchedReduction.MHT = montantHT;
                matchedReduction.TauxTVA = tauxTVA;
                matchedReduction.CodeArticle = codeArticle;
                matchedReduction.ObsDDR = autresInfos;
                matchedReduction.IdU = idUser;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedReduction;
            }
        }


        public DEMANDE_REDUCTION AnnulerReduction(int idRed, string infosAnnulation, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedReduc = (from dr in dcMar.GetTable<DEMANDE_REDUCTION>()
                                    where dr.IdDDR == idRed
                                    select dr).SingleOrDefault<DEMANDE_REDUCTION>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Demande de réduction : Annulation d'un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour annuler un ordre de service. Veuillez contacter un administrateur");
                }

                if (matchedReduc == null)
                {
                    throw new EnregistrementInexistant("L'ordre de service auquel vous faites référence n'existe pas");
                }

                if (matchedReduc.DatevDDR.HasValue)
                {
                    throw new ApplicationException("Annulation impossible : Cette demande de réduction a déjà été validée");
                }

                matchedReduc.StatutRed = "Annulé";

                if (infosAnnulation.Trim() != "")
                {
                    NOTE noteAnnulation = new NOTE();
                    noteAnnulation.IdDDR = matchedReduc.IdBL;
                    noteAnnulation.DateNote = DateTime.Now;
                    noteAnnulation.IdU = idUser;
                    noteAnnulation.TitreNote = "Note d'annulation";
                    noteAnnulation.DescNote = infosAnnulation;

                    dcMar.NOTE.InsertOnSubmit(noteAnnulation);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedReduc;
            }
        }

        #endregion

        #region demande de restitution de caution


        public DEMANDE_CAUTION InsertDemandeRestitutionCaution(int idBL, List<CONTENEUR> ctrs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                DEMANDE_CAUTION demandeCaution = new DEMANDE_CAUTION();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de restitution de caution : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une demande de restitution de caution. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                demandeCaution.DateDRC = DateTime.Now;
                demandeCaution.CONNAISSEMENT = matchedConnaissement;
                demandeCaution.AIDRC = autresInfos;
                demandeCaution.IdU = idUser;

                dcMar.SubmitChanges();

                foreach (CONTENEUR ctr in ctrs)
                {
                    if (!ctr.IdPay.HasValue)
                    {
                        throw new ApplicationException("Enregistrement de la demande de restitution de caution impossible : Le conteneur " + ctr.NumCtr + " BL N° " + ctr.CONNAISSEMENT.NumBL + " Consignee : " + ctr.CONNAISSEMENT.ConsigneeBL + " à une caution impayée");
                    }

                    ctr.IdDRC = demandeCaution.IdDRC;
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return demandeCaution;
            }
        }


        public DEMANDE_CAUTION ValiderDemandeCaution(int idDRC, string infosValid, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDRC = (from drc in dcMar.GetTable<DEMANDE_CAUTION>()
                                  where drc.IdDRC == idDRC
                                  select drc).SingleOrDefault<DEMANDE_CAUTION>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de restitution de caution : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une demande de restitution de caution. Veuillez contacter un administrateur");
                }

                if (matchedDRC == null)
                {
                    throw new EnregistrementInexistant("Demande de restitution de caution inexistante");
                }

                matchedDRC.DVDRC = DateTime.Now;
                matchedDRC.IdUV = idUser;
                matchedDRC.AIVDRC = infosValid;

                if (infosValid.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdDRC = matchedDRC.IdDRC;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = infosValid;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedDRC;
            }
        }

        #endregion

        #region Bon à enlever

        public BON_ENLEVEMENT InsertBonEnlevement(int idBL, string emetteurLettre, string destinataireLettre, DateTime dateEmissionLettre, DateTime dateValiditeLettre, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, List<MAFI> mfs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                BON_ENLEVEMENT bonEnlevement = new BON_ENLEVEMENT();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Bon à enlever : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un BAE. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                if (!dcMar.OPERATION_CONNAISSEMENT.SingleOrDefault<OPERATION_CONNAISSEMENT>(op => op.IdTypeOp == 36 && op.IdBL == matchedConnaissement.IdBL).DateOp.HasValue && matchedConnaissement.ESCALE.IdAcc == 1)
                {
                    throw new ConnaissementException("Enregistrement du BAE impossible : Ce connaissement n'a pas encore été accompli");
                }

                bonEnlevement.DateBAE = DateTime.Now;
                bonEnlevement.ELGBAE = emetteurLettre;
                bonEnlevement.DLGBAE = destinataireLettre;
                bonEnlevement.DELGBAE = dateEmissionLettre;
                bonEnlevement.DFLGBAE = dateValiditeLettre;
                bonEnlevement.CONNAISSEMENT = matchedConnaissement;
                bonEnlevement.AIBAE = autresInfos;
                bonEnlevement.IdU = idUser;

                dcMar.SubmitChanges();

                foreach (VEHICULE veh in vehs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

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

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                }

                dcMar.SubmitChanges();

                foreach (CONTENEUR ctr in ctrs)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    if (matchedCtr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 12) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conteneur n'a pas encore été identifié");
                    }

                    if (matchedCtr.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conteneur fait déjà l'objet d'un BAE.");
                    }

                    matchedCtr.StatCtr = "Enlèvement";
                    matchedCtr.IdBAE = bonEnlevement.IdBAE;
                    //matchedCtr.FSCtr = bonEnlevement.DateBAE;

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedCtr.IdCtr;
                    opCtr.IdTypeOp = 14;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = "Enlèvement en cours";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    if (matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedCtr.IdBL;
                        mvtTC.IdEsc = matchedCtr.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdTC;
                        mvtTC.IdTypeOp = 14;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }
                }

                dcMar.SubmitChanges();

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

                foreach (CONVENTIONNEL conv in gcs)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    if (matchedGC.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 25) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce general cargo n'a pas encore été identifié");
                    }

                    if (matchedGC.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conventionnel fait déjà l'objet d'un BAE.");
                    }

                    matchedGC.StatGC = "Enlèvement";
                    matchedGC.IdBAE = bonEnlevement.IdBAE;

                    OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                    opGC.IdGC = matchedGC.IdGC;
                    opGC.IdTypeOp = 28;
                    opGC.DateOp = DateTime.Now;
                    opGC.IdU = idUser;
                    opGC.AIOp = "Enlèvement en cours";

                    dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);
                }

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConnaissement.IdBL && op.IdTypeOp == 39
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Enlèvement en cours";
                }

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedConnaissement.IdMan && op.IdTypeOp == 47
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Enlèvement en cours";
                }

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedConnaissement.IdEsc && op.IdTypeOp == 55
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Enlèvement en cours";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return bonEnlevement;
            }
        }

        public BON_ENLEVEMENT InsertBonEnlevement(int idBL, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, List<MAFI> mfs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                BON_ENLEVEMENT bonEnlevement = new BON_ENLEVEMENT();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Bon à enlever : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un BAE. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                if (!dcMar.OPERATION_CONNAISSEMENT.FirstOrDefault<OPERATION_CONNAISSEMENT>(op => op.IdTypeOp == 36 && op.IdBL == matchedConnaissement.IdBL).DateOp.HasValue && matchedConnaissement.ESCALE.IdAcc == 1)
                {
                    throw new ConnaissementException("Enregistrement du BAE impossible : Ce connaissement n'a pas encore été accompli");
                }

                bonEnlevement.DateBAE = DateTime.Now;
                bonEnlevement.CONNAISSEMENT = matchedConnaissement;
                bonEnlevement.AIBAE = autresInfos;
                bonEnlevement.IdU = idUser;

                dcMar.SubmitChanges();

                foreach (VEHICULE veh in vehs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

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

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);

                }

                dcMar.SubmitChanges();

                foreach (CONTENEUR ctr in ctrs)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    if (matchedCtr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 12) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conteneur n'a pas encore été identifié");
                    }

                    if (matchedCtr.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conteneur fait déjà l'objet d'un BAE.");
                    }

                    matchedCtr.StatCtr = "Enlèvement";
                    matchedCtr.IdBAE = bonEnlevement.IdBAE;
                    //matchedCtr.FSCtr = bonEnlevement.DateBAE;

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedCtr.IdCtr;
                    opCtr.IdTypeOp = 14;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = "Enlèvement en cours";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    if (matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedCtr.IdBL;
                        mvtTC.IdEsc = matchedCtr.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdTC;
                        mvtTC.IdTypeOp = 14;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }
                }

                dcMar.SubmitChanges();

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

                foreach (CONVENTIONNEL conv in gcs)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    if (matchedGC.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 25) == 0)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce general cargo n'a pas encore été identifié");
                    }

                    if (matchedGC.IdBAE.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du BAE impossible. Ce conventionnel fait déjà l'objet d'un BAE.");
                    }

                    matchedGC.StatGC = "Enlèvement";
                    matchedGC.IdBAE = bonEnlevement.IdBAE;

                    OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                    opGC.IdGC = matchedGC.IdGC;
                    opGC.IdTypeOp = 28;
                    opGC.DateOp = DateTime.Now;
                    opGC.IdU = idUser;
                    opGC.AIOp = "Enlèvement en cours";

                    dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);
                }

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConnaissement.IdBL && op.IdTypeOp == 39
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Enlèvement en cours";
                }

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedConnaissement.IdMan && op.IdTypeOp == 47
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Enlèvement en cours";
                }

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedConnaissement.IdEsc && op.IdTypeOp == 55
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Enlèvement en cours";
                }

                dcMar.SubmitChanges();

                if (bonEnlevement.CONNAISSEMENT.ESCALE.IdAcc != 1)
                {
                    ValiderBonEnlevement(bonEnlevement.IdBAE, "Validation automatique BAE autres acconiers", idUser);
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return bonEnlevement;
            }
        }

        public BON_ENLEVEMENT ValiderBonEnlevement(int idBAE, string infosValid, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBAE = (from bae in dcMar.GetTable<BON_ENLEVEMENT>()
                                  where bae.IdBAE == idBAE
                                  select bae).SingleOrDefault<BON_ENLEVEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Bon à enlever : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider un BAE. Veuillez contacter un administrateur");
                }

                if (matchedBAE == null)
                {
                    throw new EnregistrementInexistant("Bon à enlever inexistant");
                }

                if (matchedBAE.DVBAE.HasValue)
                {
                    throw new ApplicationException("Ce BAE a déjà été validé le " + matchedBAE.DVBAE + " par " + matchedBAE.UTILISATEUR.NU);
                }

                matchedBAE.DVBAE = DateTime.Now;
                matchedBAE.IdUV = idUser;
                matchedBAE.AIVBAE = infosValid;

                if (infosValid.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdBAE = matchedBAE.IdBAE;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = infosValid;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                dcMar.SubmitChanges();

                foreach (VEHICULE veh in matchedBAE.VEHICULE)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                    opVeh.IdVeh = matchedVeh.IdVeh;
                    opVeh.IdTypeOp = 5;
                    opVeh.DateOp = DateTime.Now;
                    opVeh.IdU = idUser;
                    opVeh.IdLieu = 3;
                    opVeh.AIOp = "Enlèvement validé";

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);
                }

                foreach (CONTENEUR ctr in matchedBAE.CONTENEUR)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedCtr.IdCtr;
                    opCtr.IdTypeOp = 15;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = "Enlèvement validé";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    if (matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedCtr.IdBL;
                        mvtTC.IdEsc = matchedCtr.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdTC;
                        mvtTC.IdTypeOp = 15;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }
                }

                foreach (MAFI mafi in matchedBAE.MAFI)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == mafi.IdMafi
                                       select m).SingleOrDefault<MAFI>();

                    OPERATION_MAFI opMafi = new OPERATION_MAFI();
                    opMafi.IdMafi = matchedMafi.IdMafi;
                    opMafi.IdTypeOp = 229;
                    opMafi.DateOp = DateTime.Now;
                    opMafi.IdU = idUser;
                    opMafi.AIOp = "Enlèvement validé";

                    dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);
                }

                foreach (CONVENTIONNEL conv in matchedBAE.CONVENTIONNEL)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                    opGC.IdGC = matchedGC.IdGC;
                    opGC.IdTypeOp = 29;
                    opGC.DateOp = DateTime.Now;
                    opGC.IdU = idUser;
                    opGC.AIOp = "Enlèvement validé";

                    dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);
                }

                dcMar.SubmitChanges();

                if (matchedBAE.CONNAISSEMENT.IdAcc != 1)
                {
                    OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                           where op.IdBL == matchedBAE.IdBL && op.IdTypeOp == 36
                                                           select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBL.DateOp.HasValue)
                    {
                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Accomplissement automatique";
                    }

                    dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();

                int nbVehBL = matchedBAE.CONNAISSEMENT.VEHICULE.Count;
                int nbVehBLBAE = matchedBAE.CONNAISSEMENT.VEHICULE.Count(veh => veh.IdBAE.HasValue);

                int nbCtrBL = matchedBAE.CONNAISSEMENT.CONTENEUR.Count;
                int nbCtrBLBAE = matchedBAE.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.IdBAE.HasValue);

                int nbMafiBL = matchedBAE.CONNAISSEMENT.MAFI.Count;
                int nbMafiBLBAE = matchedBAE.CONNAISSEMENT.MAFI.Count(mf => mf.IdBAE.HasValue);

                int nbGCBL = matchedBAE.CONNAISSEMENT.CONVENTIONNEL.Count;
                int nbGCBLBAE = matchedBAE.CONNAISSEMENT.CONVENTIONNEL.Count(gc => gc.IdBAE.HasValue);

                if (nbVehBL == nbVehBLBAE && nbCtrBL == nbCtrBLBAE && nbMafiBL == nbMafiBLBAE && nbGCBL == nbGCBLBAE && matchedBAE.CONNAISSEMENT.BON_ENLEVEMENT.Count(bae => !bae.DVBAE.HasValue) == 0)
                {
                    OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                           where op.IdBL == matchedBAE.IdBL && op.IdTypeOp == 40
                                                           select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBL.DateOp.HasValue)
                    {
                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Enlèvement Terminé";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehMan = matchedBAE.CONNAISSEMENT.MANIFESTE.VEHICULE.Count;
                int nbVehManBAE = matchedBAE.CONNAISSEMENT.MANIFESTE.VEHICULE.Count(veh => veh.IdBAE.HasValue);

                int nbCtrMan = matchedBAE.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count;
                int nbCtrManBAE = matchedBAE.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count(ctr => ctr.IdBAE.HasValue);

                int nbMafiMan = matchedBAE.CONNAISSEMENT.MANIFESTE.MAFI.Count;
                int nbMafiManBAE = matchedBAE.CONNAISSEMENT.MANIFESTE.MAFI.Count(mf => mf.IdBAE.HasValue);

                int nbGCMan = matchedBAE.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count;
                int nbGCManBAE = matchedBAE.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count(gc => gc.IdBAE.HasValue);

                if (nbVehMan == nbVehManBAE && nbCtrMan == nbCtrManBAE && nbMafiMan == nbMafiManBAE && nbGCMan == nbGCManBAE && matchedBAE.CONNAISSEMENT.MANIFESTE.CONNAISSEMENT.Count(bl => bl.BON_ENLEVEMENT.Count(bae => !bae.DVBAE.HasValue) == 0) == 0)
                {
                    OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                        where op.IdMan == matchedBAE.CONNAISSEMENT.IdMan && op.IdTypeOp == 48
                                                        select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpMan.DateOp.HasValue)
                    {
                        matchedOpMan.DateOp = DateTime.Now;
                        matchedOpMan.IdU = idUser;
                        matchedOpMan.AIOp = "Enlèvement Terminé";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehEsc = matchedBAE.CONNAISSEMENT.ESCALE.VEHICULE.Count;
                int nbVehEscBAE = matchedBAE.CONNAISSEMENT.ESCALE.VEHICULE.Count(veh => veh.IdBAE.HasValue);

                int nbCtrEsc = matchedBAE.CONNAISSEMENT.ESCALE.CONTENEUR.Count;
                int nbCtrEscBAE = matchedBAE.CONNAISSEMENT.ESCALE.CONTENEUR.Count(ctr => ctr.IdBAE.HasValue);

                int nbMafiEsc = matchedBAE.CONNAISSEMENT.ESCALE.MAFI.Count;
                int nbMafiEscBAE = matchedBAE.CONNAISSEMENT.ESCALE.MAFI.Count(mf => mf.IdBAE.HasValue);

                int nbGCEsc = matchedBAE.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count;
                int nbGCEscBAE = matchedBAE.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count(gc => gc.IdBAE.HasValue);

                if (nbVehEsc == nbVehEscBAE && nbCtrEsc == nbCtrEscBAE && nbMafiEsc == nbMafiEscBAE && nbGCEsc == nbGCEscBAE && matchedBAE.CONNAISSEMENT.ESCALE.CONNAISSEMENT.Count(bl => bl.BON_ENLEVEMENT.Count(bae => !bae.DVBAE.HasValue) == 0) == 0)
                {
                    OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                     where op.IdEsc == matchedBAE.CONNAISSEMENT.IdEsc && op.IdTypeOp == 56
                                                     select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEsc.DateOp.HasValue)
                    {
                        matchedOpEsc.DateOp = DateTime.Now;
                        matchedOpEsc.IdU = idUser;
                        matchedOpEsc.AIOp = "Enlèvement Terminé";
                    }
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedBAE;
            }
        }

        #endregion
 
        #region demande de livraison

        public DEMANDE_LIVRAISON ValiderDemandeLivraison(int idDL, string infosValid, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDL = (from dl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                 where dl.IdDBL == idDL
                                 select dl).SingleOrDefault<DEMANDE_LIVRAISON>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de livraison : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une demande de livraison. Veuillez contacter un administrateur");
                }

                if (matchedDL == null)
                {
                    throw new EnregistrementInexistant("Demande de livraison inexistante");
                }

                if (matchedDL.DVDBL.HasValue)
                {
                    throw new ApplicationException("Cette demande de livraison a déjà été validée le " + matchedDL.DVDBL + " par " + dcMar.UTILISATEUR.Single<UTILISATEUR>(u => u.IdU == matchedDL.IdUV).NU);
                }

                if (!matchedDL.DateDepotDBL.HasValue)
                {
                    throw new ApplicationException("Validation impossible : veuillez renseigner la date de mise à jour du dossier physique");
                }

                matchedDL.DVDBL = DateTime.Now;
                matchedDL.IdUV = idUser;
                matchedDL.AIVDBL = infosValid;

                if (infosValid.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdDBL = matchedDL.IdDBL;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = infosValid;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                dcMar.SubmitChanges();

                foreach (VEHICULE veh in matchedDL.VEHICULE)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                    opVeh.IdVeh = matchedVeh.IdVeh;
                    opVeh.IdTypeOp = 7;
                    opVeh.DateOp = DateTime.Now;
                    opVeh.IdU = idUser;
                    opVeh.IdLieu = 3;
                    opVeh.AIOp = "Demande de livraison validée";

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);
                }

                foreach (CONTENEUR ctr in matchedDL.CONTENEUR)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedCtr.IdCtr;
                    opCtr.IdTypeOp = 17;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = "Demande de livraison validée";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);


                    if (matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedCtr.IdBL;
                        mvtTC.IdEsc = matchedCtr.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdTC;
                        mvtTC.IdTypeOp = 17;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }

                }

                foreach (MAFI mf in matchedDL.MAFI)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == mf.IdMafi
                                       select m).SingleOrDefault<MAFI>();

                    OPERATION_MAFI opMafi = new OPERATION_MAFI();
                    opMafi.IdMafi = matchedMafi.IdMafi;
                    opMafi.IdTypeOp = 231;
                    opMafi.DateOp = DateTime.Now;
                    opMafi.IdU = idUser;
                    opMafi.AIOp = "Demande de livraison validée";

                    dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);
                }

                foreach (CONVENTIONNEL conv in matchedDL.CONVENTIONNEL)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                    opGC.IdGC = matchedGC.IdGC;
                    opGC.IdTypeOp = 31;
                    opGC.DateOp = DateTime.Now;
                    opGC.IdU = idUser;
                    opGC.AIOp = "Demande de livraison validée";

                    dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);
                }

                dcMar.SubmitChanges();

                int nbVehBL = matchedDL.CONNAISSEMENT.VEHICULE.Count;
                int nbVehBLDBL = matchedDL.CONNAISSEMENT.VEHICULE.Count(veh => veh.IdDBL.HasValue);

                int nbCtrBL = matchedDL.CONNAISSEMENT.CONTENEUR.Count;
                int nbCtrBLDBL = matchedDL.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.IdDBL.HasValue);

                int nbMafiBL = matchedDL.CONNAISSEMENT.MAFI.Count;
                int nbMafiBLDBL = matchedDL.CONNAISSEMENT.MAFI.Count(mf => mf.IdDBL.HasValue);

                int nbGCBL = matchedDL.CONNAISSEMENT.CONVENTIONNEL.Count;
                int nbGCBLDBL = matchedDL.CONNAISSEMENT.CONVENTIONNEL.Count(gc => gc.IdDBL.HasValue);

                if (nbVehBL == nbVehBLDBL && nbCtrBL == nbCtrBLDBL && nbMafiBL == nbMafiBLDBL && nbGCBL == nbGCBLDBL && matchedDL.CONNAISSEMENT.DEMANDE_LIVRAISON.Count(dbl => !dbl.DVDBL.HasValue) == 0)
                {
                    OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                           where op.IdBL == matchedDL.IdBL && op.IdTypeOp == 42
                                                           select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBL.DateOp.HasValue)
                    {
                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Livraison Terminée";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehMan = matchedDL.CONNAISSEMENT.MANIFESTE.VEHICULE.Count;
                int nbVehManDBL = matchedDL.CONNAISSEMENT.MANIFESTE.VEHICULE.Count(veh => veh.IdDBL.HasValue);

                int nbCtrMan = matchedDL.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count;
                int nbCtrManDBL = matchedDL.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count(ctr => ctr.IdDBL.HasValue);

                int nbMafiMan = matchedDL.CONNAISSEMENT.MANIFESTE.MAFI.Count;
                int nbMafiManDBL = matchedDL.CONNAISSEMENT.MANIFESTE.MAFI.Count(mf => mf.IdDBL.HasValue);

                int nbGCMan = matchedDL.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count;
                int nbGCManDBL = matchedDL.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count(gc => gc.IdDBL.HasValue);

                if (nbVehMan == nbVehManDBL && nbCtrMan == nbCtrManDBL && nbMafiMan == nbMafiManDBL && nbGCMan == nbGCManDBL && matchedDL.CONNAISSEMENT.MANIFESTE.CONNAISSEMENT.Count(bl => bl.DEMANDE_LIVRAISON.Count(liv => !liv.DVDBL.HasValue) == 0) == 0)
                {
                    OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                        where op.IdMan == matchedDL.CONNAISSEMENT.IdMan && op.IdTypeOp == 50
                                                        select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpMan.DateOp.HasValue)
                    {
                        matchedOpMan.DateOp = DateTime.Now;
                        matchedOpMan.IdU = idUser;
                        matchedOpMan.AIOp = "Livraison Terminée";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehEsc = matchedDL.CONNAISSEMENT.ESCALE.VEHICULE.Count;
                int nbVehEscDBL = matchedDL.CONNAISSEMENT.ESCALE.VEHICULE.Count(veh => veh.IdDBL.HasValue);

                int nbCtrEsc = matchedDL.CONNAISSEMENT.ESCALE.CONTENEUR.Count;
                int nbCtrEscDBL = matchedDL.CONNAISSEMENT.ESCALE.CONTENEUR.Count(ctr => ctr.IdDBL.HasValue);

                int nbMafiEsc = matchedDL.CONNAISSEMENT.ESCALE.MAFI.Count;
                int nbMafiEscDBL = matchedDL.CONNAISSEMENT.ESCALE.MAFI.Count(mf => mf.IdDBL.HasValue);

                int nbGCEsc = matchedDL.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count;
                int nbGCEscDBL = matchedDL.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count(gc => gc.IdDBL.HasValue);

                if (nbVehEsc == nbVehEscDBL && nbCtrEsc == nbCtrEscDBL && nbMafiEsc == nbMafiEscDBL && nbGCEsc == nbGCEscDBL && matchedDL.CONNAISSEMENT.ESCALE.CONNAISSEMENT.Count(bl => bl.DEMANDE_LIVRAISON.Count(liv => !liv.DVDBL.HasValue) == 0) == 0)
                {
                    OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                     where op.IdEsc == matchedDL.CONNAISSEMENT.IdEsc && op.IdTypeOp == 58
                                                     select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEsc.DateOp.HasValue)
                    {
                        matchedOpEsc.DateOp = DateTime.Now;
                        matchedOpEsc.IdU = idUser;
                        matchedOpEsc.AIOp = "Livraison Terminée";
                    }
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedDL;
            }
        }

        public DEMANDE_LIVRAISON MAJDateDepotLivraison(int idDL, DateTime dateDepot, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDL = (from dl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                 where dl.IdDBL == idDL
                                 select dl).SingleOrDefault<DEMANDE_LIVRAISON>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                //List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                //if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de livraison : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                //{
                //    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une demande de livraison. Veuillez contacter un administrateur");
                //}

                if (matchedDL == null)
                {
                    throw new EnregistrementInexistant("Demande de livraison inexistante");
                }

                matchedDL.DateDepotDBL = dateDepot;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedDL;
            }
        }

        public DEMANDE_LIVRAISON InsertDemandeLivraison(int idBL, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, List<MAFI> mfs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                DEMANDE_LIVRAISON demandeLivraison = new DEMANDE_LIVRAISON();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de livraison : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une demande de livraison. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                demandeLivraison.DateDBL = DateTime.Now;
                demandeLivraison.CONNAISSEMENT = matchedConnaissement;
                demandeLivraison.AIDBL = autresInfos;
                demandeLivraison.IdU = idUser;
                if (matchedConnaissement.ESCALE.IdAcc == 1)
                {
                    demandeLivraison.DateDepotDBL = DateTime.Now;
                }

                dcMar.SubmitChanges();

                foreach (VEHICULE veh in vehs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    if (matchedVeh.IdDBL.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce véhicule fait déjà l'objet d'un bon de livraison.");
                    }

                    if (matchedVeh.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0 && !matchedVeh.IdVehAP.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce véhicule n'est parqué nulle part");
                    }

                    //if (matchedVeh.VISITE_VEHICULE.DEMANDE_VISITE.IdTypeVisite != 1)
                    //{
                    //    throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce véhicule n'a pas encore fait l'objet d'une visite douane.");
                    //}

                    matchedVeh.StatVeh = "Livraison";
                    matchedVeh.IdDBL = demandeLivraison.IdDBL;

                    OPERATION_VEHICULE opVeh = new OPERATION_VEHICULE();
                    opVeh.IdVeh = matchedVeh.IdVeh;
                    opVeh.IdTypeOp = 6;
                    opVeh.DateOp = DateTime.Now;
                    opVeh.IdU = idUser;
                    opVeh.IdLieu = 3;
                    opVeh.AIOp = "Livraison en cours";

                    dcMar.GetTable<OPERATION_VEHICULE>().InsertOnSubmit(opVeh);
                }

                foreach (CONTENEUR ctr in ctrs)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    if (matchedCtr.IdDBL.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce conteneur fait déjà l'objet d'un bon de livraison.");
                    }

                    matchedCtr.StatCtr = "Livraison";
                    matchedCtr.IdDBL = demandeLivraison.IdDBL;

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedCtr.IdCtr;
                    opCtr.IdTypeOp = 16;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = "Livraison en cours";

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    if (matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedCtr.IdBL;
                        mvtTC.IdEsc = matchedCtr.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = matchedCtr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().IdTC;
                        mvtTC.IdTypeOp = 16;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }
                }

                foreach (MAFI mf in mfs)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == mf.IdMafi
                                       select m).SingleOrDefault<MAFI>();

                    if (matchedMafi.IdDBL.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce conteneur fait déjà l'objet d'un bon de livraison.");
                    }

                    matchedMafi.StatMafi = "Livraison";
                    matchedMafi.IdDBL = demandeLivraison.IdDBL;

                    OPERATION_MAFI opMafi = new OPERATION_MAFI();
                    opMafi.IdMafi = matchedMafi.IdMafi;
                    opMafi.IdTypeOp = 230;
                    opMafi.DateOp = DateTime.Now;
                    opMafi.IdU = idUser;
                    opMafi.AIOp = "Livraison en cours";

                    dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);
                }

                foreach (CONVENTIONNEL conv in gcs)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    if (matchedGC.IdDBL.HasValue)
                    {
                        throw new ApplicationException("Enregistrement du bon de livraison impossible. Ce general cargo fait déjà l'objet d'un bon de livraison.");
                    }

                    matchedGC.StatGC = "Livraison";
                    matchedGC.IdDBL = demandeLivraison.IdDBL;

                    OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                    opGC.IdGC = matchedGC.IdGC;
                    opGC.IdTypeOp = 30;
                    opGC.DateOp = DateTime.Now;
                    opGC.IdU = idUser;
                    opGC.AIOp = "Livraison en cours";

                    dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);
                }

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConnaissement.IdBL && op.IdTypeOp == 41
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Livraison en cours";
                }

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedConnaissement.IdMan && op.IdTypeOp == 49
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Livraison en cours";
                }

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedConnaissement.IdEsc && op.IdTypeOp == 57
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Livraison en cours";
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return demandeLivraison;
            }
        }

        #endregion

        #region demande de visite

        public DEMANDE_VISITE ValiderDemandeVisite(int idDV, string infosValid, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDV = (from dv in dcMar.GetTable<DEMANDE_VISITE>()
                                 where dv.IdDV == idDV
                                 select dv).SingleOrDefault<DEMANDE_VISITE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de visite : Validation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour valider une demande de visite. Veuillez contacter un administrateur");
                }

                if (matchedDV == null)
                {
                    throw new EnregistrementInexistant("Demande de visite inexistante");
                }

                matchedDV.DVDV = DateTime.Now;
                matchedDV.IdUV = idUser;
                matchedDV.AIVDV = infosValid;

                if (infosValid.Trim() != "")
                {
                    NOTE noteValid = new NOTE();
                    noteValid.IdDV = matchedDV.IdDV;
                    noteValid.DateNote = DateTime.Now;
                    noteValid.IdU = idUser;
                    noteValid.TitreNote = "Note de validation";
                    noteValid.DescNote = infosValid;

                    dcMar.NOTE.InsertOnSubmit(noteValid);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedDV;
            }
        }

        public DEMANDE_VISITE InsertDemandeVisite(int idBL, int idTypeVisite, List<VEHICULE> vehs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                DEMANDE_VISITE demandeVisite = new DEMANDE_VISITE();

                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Demande de visite : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une demande de visite. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                demandeVisite.DateDV = DateTime.Now;
                demandeVisite.IdTypeVisite = idTypeVisite;
                demandeVisite.CONNAISSEMENT = matchedConnaissement;
                demandeVisite.AIDV = autresInfos;
                demandeVisite.IdU = idUser;

                dcMar.SubmitChanges();

                List<VISITE_VEHICULE> listVeh = new List<VISITE_VEHICULE>();

                foreach (VEHICULE veh in vehs)
                {
                    var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                              where (occ.IdVeh == veh.IdVeh && !occ.DateFin.HasValue)
                                              select occ).FirstOrDefault<OCCUPATION>();

                    if (matchedOcccupation == null)
                    {
                        throw new ApplicationException("Enregistrement de la demande de visite impossible : Le véhicule " + veh.NumChassis + " BL N° " + veh.CONNAISSEMENT.NumBL + " Consignee : " + veh.CONNAISSEMENT.ConsigneeBL + " n'est parqué nulle part");
                    }

                    VISITE_VEHICULE visVeh = new VISITE_VEHICULE();
                    visVeh.IdVeh = veh.IdVeh;
                    visVeh.IdDV = demandeVisite.IdDV;
                    //demandeVisite.IdVeh = veh.IdVeh;

                    listVeh.Add(visVeh);
                }

                dcMar.VISITE_VEHICULE.InsertAllOnSubmit<VISITE_VEHICULE>(listVeh);

                dcMar.SubmitChanges();
                transaction.Complete();
                return demandeVisite;
            }
        }

        #endregion

        #region conteneur + operation


        public CONTENEUR TransfEmplacementConteneur(int idCtr, int idEmpl, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de transfert d'emplacement").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de transfert d'emplacement de conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneurTC != null)
                {

                    //if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "Parqué" && matchedConteneurTC.StatutTC != "En réparation")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est pas présent dans le parc");
                    //}

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 285;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    matchedConteneur.StatCtr = "Parqué";

                    matchedConteneurTC.StatutTC = "Parqué";
                    matchedConteneurTC.IdParcParquing = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    matchedConteneurTC.IdEmplacementParc = idEmpl;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 285;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    dcMar.SubmitChanges();

                    var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                              where (occ.IdCtr == idCtr && !occ.DateFin.HasValue)
                                              select occ).FirstOrDefault<OCCUPATION>();

                    if (matchedOcccupation != null)
                    {
                        matchedOcccupation.DateFin = DateTime.Now;
                    }

                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdCtr = idCtr;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 285;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                    dcMar.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR UpdateInfosLivraisonCtr(int idCtr, string nomEnleveur, string cniEnleveur, string telEnleveur, string numAttDedouanement, string numCivio, string numDeclDouane, string numQuittDouane, string numFactPAD, string numQuittPAD, string numBAE, string numBESC, string numSydonia, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).SingleOrDefault<CONTENEUR>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Conteneur : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations de livraison. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                //if (matchedConteneur.FSCtr < DateTime.Now.Date)
                //{
                //    throw new ApplicationException("La date à laquelle vous avez calculé le séjour a été dépassée, veuillez recalculer le séjour de ce conteneur");
                //}

                matchedConteneur.NomEnCtr = nomEnleveur;
                matchedConteneur.CNIEnCtr = cniEnleveur;
                matchedConteneur.TelenCtr = telEnleveur;
                matchedConteneur.NumADDCtr = numAttDedouanement;
                matchedConteneur.NumAVICtr = numCivio;
                matchedConteneur.NumDDCtr = numDeclDouane;
                matchedConteneur.NumQDCtr = numQuittDouane;
                matchedConteneur.NumFPADCtr = numFactPAD;
                matchedConteneur.NumQPADCtr = numQuittPAD;
                matchedConteneur.NumAEPADCtr = numBAE;
                matchedConteneur.NumBESCCtr = numBESC;
                matchedConteneur.NumSydoniaCtr = numSydonia;

                //matchedConteneur.StatCtr = "Livraison";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR UpdateInfosEnlevementCtr(int idCtr, DateTime sortiePrev, string numBESC, string nomEnleveur, string cniEnleveur, string telEnleveur, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).SingleOrDefault<CONTENEUR>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Conteneur : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations d'enlèvement. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneur.FSCtr.Value.Date < sortiePrev.Date /*&& matchedConteneur.ESCALE.IdArm != 2*/)
                {
                    throw new ApplicationException("La date de sortie ne peut pas dépasser la date de fin de séjour, recalculer le séjour de ce conteneur");
                }

                matchedConteneur.DSPCtr = sortiePrev;
                matchedConteneur.NomEnCtr = nomEnleveur;
                matchedConteneur.CNIEnCtr = cniEnleveur;
                matchedConteneur.TelenCtr = telEnleveur;
                matchedConteneur.NumBESCCtr = numBESC;
                //matchedConteneur.StatCtr = "Enlèvement";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR InsertConteneur(int idBL, string numCtr, string descCtr, string descMsesCtr, string imdgCode, string typeMCtr, string etatCtr, int poidsMCtr, string seal1, string seal2, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Conteneur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un nouveau conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement inexistant");
                }

                CONTENEUR ctr = new CONTENEUR();

                ctr.ESCALE = matchedConnaissement.ESCALE;
                ctr.MANIFESTE = matchedConnaissement.MANIFESTE;
                ctr.CONNAISSEMENT = matchedConnaissement;
                ctr.NumCtr = numCtr;
                ctr.DescCtr = descCtr;
                ctr.DescMses = descMsesCtr;
                ctr.TypeMCtr = typeMCtr;
                ctr.TypeCCtr = typeMCtr;
                ctr.StatutCtr = etatCtr;
                ctr.VolMCtr = 0;
                ctr.PoidsMCtr = poidsMCtr;
                ctr.PoidsCCtr = poidsMCtr;
                ctr.Seal1Ctr = seal1;
                ctr.Seal2Ctr = seal2;
                ctr.NumItem = 1;
                ctr.InfoMan = "";
                ctr.IMDGCode = imdgCode;
                ctr.DCCtr = DateTime.Now;
                ctr.StatCtr = "Non initié";

                if (matchedConnaissement.DVBL.HasValue)
                {
                    ctr.StatCtr = "Manifesté";
                }

                ctr.SensCtr = "I";
                ctr.PropCtr = 1;

                if (ctr.ESCALE.DDechEsc.HasValue)
                {
                    ctr.FFCtr = ctr.ESCALE.DDechEsc.Value.AddDays(9);
                    ctr.FSCtr = ctr.ESCALE.DDechEsc.Value.AddDays(9);
                    ctr.NbDet = 2;
                    ctr.FFSCtr = ((Int32)ctr.ESCALE.DDechEsc.Value.DayOfWeek) > 2 ? ctr.ESCALE.DDechEsc.Value.AddDays(11) : ctr.ESCALE.DDechEsc.Value.AddDays(10);
                }

                dcMar.GetTable<CONTENEUR>().InsertOnSubmit(ctr);

                dcMar.SubmitChanges();

                if (matchedConnaissement.MANIFESTE.DVMan.HasValue)
                {
                    var typesSinCtr = (from type in dcMar.GetTable<TYPE_SINISTRE>()
                                       where type.TypeMse == "C"
                                       orderby type.IdTypeSinistre ascending
                                       select type).ToList<TYPE_SINISTRE>();

                    foreach (TYPE_SINISTRE sin in typesSinCtr)
                    {
                        INTERCHANGE interchange = new INTERCHANGE();

                        interchange.IdCtr = ctr.IdCtr;
                        interchange.IdTypeSinistre = sin.IdTypeSinistre;

                        dcMar.GetTable<INTERCHANGE>().InsertOnSubmit(interchange);
                    }
                }

                // Création du conteneur TC

                CONTENEUR_TC ctrTC = new CONTENEUR_TC();

                ctrTC.NumTC = numCtr;
                ctrTC.TypeCtr = typeMCtr;
                ctrTC.TypeCtr = ctrTC.TypeCtr.Replace("BX", "DV");
                ctrTC.DateCréationTC = DateTime.Now;
                ctrTC.IdEscDébarquement = matchedConnaissement.IdEsc;
                ctrTC.IdBLDébarquement = matchedConnaissement.IdBL;
                ctrTC.IdCtr = ctr.IdCtr;
                ctrTC.StatutTC = "Manifesté";
                ctrTC.IsDoublon = "N";

                dcMar.GetTable<CONTENEUR_TC>().InsertOnSubmit(ctrTC);

                dcMar.SubmitChanges();

                transaction.Complete();
                return ctr;
            }
        }

        public CONTENEUR UpdateConteneur(int idCtr, string numCtr, int proprietaire, string description, string imdgCode, string descMses, string typeMses, string seal1, string seal2, string statutCtr, string typeMCtr, string typeCCtr, int poidsCCtr, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                matchedConteneur.NumCtr = numCtr;
                matchedConteneur.PropCtr = (short)proprietaire;
                matchedConteneur.DescCtr = description;
                matchedConteneur.IMDGCode = imdgCode;
                matchedConteneur.DescMses = descMses;
                matchedConteneur.TypeMses = typeMses;
                matchedConteneur.StatutCtr = statutCtr;
                matchedConteneur.TypeMCtr = typeMCtr;
                matchedConteneur.TypeCCtr = typeCCtr;
                matchedConteneur.PoidsCCtr = poidsCCtr;
                matchedConteneur.Seal1Ctr = seal1;
                matchedConteneur.Seal2Ctr = seal2;

                if (matchedConteneur.StatCtr == "Initié" && matchedConteneur.MANIFESTE.DVMan.HasValue)
                {
                    matchedConteneur.StatCtr = "Traité";
                    if (matchedConteneur.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.StatCtr != "Traité" && ctr.IdCtr != idCtr) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedConteneur.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                List<PARAMETRE> parametres = dcMar.GetTable<PARAMETRE>().ToList<PARAMETRE>();
                List<ARTICLE> articles = dcMar.GetTable<ARTICLE>().ToList<ARTICLE>();

                DateTime dte = DateTime.Now;


                //Calcul du montant de la caution
                if (matchedConteneur.PropCtr == 1 && !matchedConteneur.IdPay.HasValue)
                {
                    if (matchedConteneur.CONNAISSEMENT.DestBL == "DLA")
                    {
                        matchedConteneur.MCCtr = matchedConteneur.TYPE_CONTENEUR != null ? matchedConteneur.TYPE_CONTENEUR.PUDLA : 1;
                    }
                    else if (matchedConteneur.CONNAISSEMENT.DestBL == "CMR")
                    {
                        matchedConteneur.MCCtr = matchedConteneur.TYPE_CONTENEUR != null ? matchedConteneur.TYPE_CONTENEUR.PUCMR : 1;
                    }
                    else if (matchedConteneur.CONNAISSEMENT.DestBL == "HINT")
                    {
                        matchedConteneur.MCCtr = matchedConteneur.TYPE_CONTENEUR != null ? matchedConteneur.TYPE_CONTENEUR.PUINT : 1;
                    }
                }
                else if (matchedConteneur.PropCtr == 2)
                {
                    matchedConteneur.MCCtr = 0;
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR VendreConteneur(CONTENEUR_TC ctr, DateTime datevente, string client, string observation, int iduser)
        {
            throw new ApplicationException("Non disponible");
        }


        public CONTENEUR PositionnerConteneur(int idTC, int idEmpl, int idDisposition, string destination, List<TYPE_SINISTRE> listeSinistre, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                CONTENEUR ctr = null;

                var matchedConteneurTC = (from tc in dcMar.GetTable<CONTENEUR_TC>()
                                          where tc.IdTC == idTC
                                          select tc).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de positionnement de contenur vide").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de positionnement de conteneur vide. Veuillez contacter un administrateur");
                }

                if (matchedConteneurTC != null)
                {
                    //if (DateTime.Now.Date > matchedConteneurTC.CONTENEUR.ESCALE.DRAEsc.Value)
                    //{
                    //    throw new ApplicationException("Positionnement impossible : le navire est déjà arrivé");
                    //}

                    //if (matchedConteneurTC.StatutTC != "Parqué")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est pas parqué");
                    //}

                    OPERATION_CONTENEUR matchedOpPositionnement = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                                   where op.IdCtr == matchedConteneurTC.IdCtr && op.IdTypeOp == 281
                                                                   select op).FirstOrDefault<OPERATION_CONTENEUR>();
                    //AH 7aout dans le cas d'un conteneur retourné non empoté, loperation de parquage avait été deja effectuée. il faut donc autoriser pour ce conteneur
                    //loperation de reparquage. il faut verifier sil existe une opération de retour non empoté sur le conteneur. si oui, autorisé le parquage

                    OPERATION_CONTENEUR matchedRetourNonEmpoté = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                                                  where op.IdCtr == matchedConteneurTC.IdCtr && op.IdTypeOp == 290
                                                                  select op).FirstOrDefault<OPERATION_CONTENEUR>();

                    if (matchedOpPositionnement != null)
                    {
                        if (matchedRetourNonEmpoté == null)//sil n'existe pas doperation de retour non empoté
                        {
                            throw new ApplicationException("Ce conteneur a déjà été positionné");
                        }
                        else if (matchedConteneurTC.IdCtrExport != null)
                        {
                            throw new ApplicationException("Ce conteneur a déjà été positionné");
                        }
                    }

                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = matchedConteneurTC.IdCtr;
                    opCtr.IdTypeOp = 281;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    matchedConteneurTC.CONTENEUR.StatCtr = "Mis à disposition";

                    matchedConteneurTC.StatutTC = "Mis à disposition";
                    matchedConteneurTC.DateSortieVide = DateTime.Now;
                    matchedConteneurTC.IdUserSortieVide = matchedUser.IdU;
                    matchedConteneurTC.IdEscSortieVide = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    matchedConteneurTC.IdMiseADisposition = idDisposition;
                    matchedConteneurTC.IdBooking = dcMar.DISPOSITION_CONTENEUR.FirstOrDefault<DISPOSITION_CONTENEUR>(dd => dd.IdDisposition == idDisposition).IdBooking;
                    matchedConteneurTC.DestinationSortieVide = destination;

                    ctr = new CONTENEUR();

                    ctr.SensCtr = "E";
                    ctr.NumCtr = matchedConteneurTC.NumTC;
                    ctr.PropCtr = 1;
                    ctr.MCCtr = 1500000;
                    ctr.DescCtr = "";
                    ctr.IMDGCode = "";
                    ctr.DescMses = "";
                    ctr.StatutCtr = "";
                    ctr.TypeMCtr = matchedConteneurTC.TypeCtr;
                    ctr.TypeCCtr = matchedConteneurTC.TypeCtr;
                    ctr.TypeMses = "";
                    ctr.VolMCtr = 0;
                    ctr.PoidsMCtr = 0;
                    ctr.PoidsCCtr = 0;
                    ctr.IdBL = dcMar.DISPOSITION_CONTENEUR.FirstOrDefault<DISPOSITION_CONTENEUR>(dd => dd.IdDisposition == idDisposition).IdBooking;
                    ctr.IdEsc = dcMar.DISPOSITION_CONTENEUR.FirstOrDefault<DISPOSITION_CONTENEUR>(dd => dd.IdDisposition == idDisposition).CONNAISSEMENT.ESCALE.IdEsc;
                    ctr.Seal1Ctr = "";
                    ctr.Seal2Ctr = "";
                    ctr.DCCtr = DateTime.Now;
                    ctr.StatCtr = "Non initié";

                    dcMar.GetTable<CONTENEUR>().InsertOnSubmit(ctr);

                    dcMar.SubmitChanges();

                    matchedConteneurTC.IdBooking = ctr.IdBL;
                    matchedConteneurTC.IdCtrExport = ctr.IdCtr;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = ctr.IdBL;
                    mvtTC.IdEsc = ctr.IdEsc;
                    mvtTC.IdParc = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 281;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                              where (occ.IdCtr == matchedConteneurTC.IdCtr && !occ.DateFin.HasValue)
                                              select occ).FirstOrDefault<OCCUPATION>();

                    if (matchedOcccupation != null)
                    {
                        matchedOcccupation.DateFin = DateTime.Now;
                    }

                    foreach (TYPE_SINISTRE typeSin in listeSinistre)
                    {
                        var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                               where inter.IdCtr == matchedConteneurTC.IdCtr
                                               select inter).ToList<INTERCHANGE>();

                        listInterchange.FirstOrDefault<INTERCHANGE>(inter => inter.IdTypeSinistre == typeSin.IdTypeSinistre).InfoSortieVide = "Y";
                        dcMar.INTERCHANGE.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return ctr;
            }
        }


        public CONTENEUR RestituerConteneur(int idCtr, List<TYPE_SINISTRE> listeSinistre, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de restitution").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de retour de restitution de conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneurTC != null)
                {

                    if (matchedConteneurTC.StatutTC != "Cargo Loaded")
                    {
                        throw new ApplicationException("Ce conteneur n'est pas emarqué");
                    }

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 287;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    //matchedConteneur.StatCtr = "Retourné à l'armateur";

                    //matchedConteneurTC.StatutTC = "Retourné à l'armateur";
                    //matchedConteneurTC.DateRetourArmateur = DateTime.Now;
                    //matchedConteneurTC.IdUserRetourArmateur = matchedUser.IdU;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = matchedConteneurTC.IdParcParquing;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 287;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    foreach (TYPE_SINISTRE typeSin in listeSinistre)
                    {
                        var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                               where inter.IdCtr == idCtr
                                               select inter).ToList<INTERCHANGE>();

                        listInterchange.FirstOrDefault<INTERCHANGE>(ctr => ctr.IdTypeSinistre == typeSin.IdTypeSinistre).InfoRestitution = "Y";
                        dcMar.INTERCHANGE.Context.SubmitChanges();
                    }

                    dcMar.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }


        public CONTENEUR ConstatContradictoire(int idCtr, List<TYPE_SINISTRE> listeSinistre, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de constat contradictoire").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de retour de constat contradictoire. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneurTC != null)
                {

                    if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "Parqué" && matchedConteneurTC.StatutTC != "En réparation" && matchedConteneurTC.StatutTC != "En habillage")
                    {
                        throw new ApplicationException("Ce conteneur n'est pas présent dans le parc");
                    }

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 286;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    //matchedConteneur.StatCtr = "Retourné à l'armateur";

                    //matchedConteneurTC.StatutTC = "Retourné à l'armateur";
                    //matchedConteneurTC.DateRetourArmateur = DateTime.Now;
                    //matchedConteneurTC.IdUserRetourArmateur = matchedUser.IdU;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = matchedConteneurTC.IdParcParquing;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 286;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    foreach (TYPE_SINISTRE typeSin in listeSinistre)
                    {
                        var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                               where inter.IdCtr == idCtr
                                               select inter).ToList<INTERCHANGE>();

                        listInterchange.FirstOrDefault<INTERCHANGE>(ctr => ctr.IdTypeSinistre == typeSin.IdTypeSinistre).InfoConstatContradictoire = "Y";
                        dcMar.INTERCHANGE.Context.SubmitChanges();
                    }

                    dcMar.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR IdentifierConteneur(int idCtr, List<TYPE_SINISTRE> listeSinistre, string seal1, string seal2, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                //if (!matchedConteneur.ESCALE.DRAEsc.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Le navire n'est pas encore arrivé");
                //}

                if (!matchedConteneur.MANIFESTE.DVMan.HasValue)
                {
                    throw new IdentificationException("Echec de l'identification : Manifeste non validé");
                }

                if (matchedConteneur.ESCALE.SOP == "C")
                {
                    throw new ApplicationException("Echec de l'embarquement : Le summary of operations a déjà été clôturé");
                }

                var opIdentification = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                        where op.IdTypeOp == 12 && op.IdCtr == idCtr
                                        select op).FirstOrDefault<OPERATION_CONTENEUR>();

                if (opIdentification != null)
                {
                    if (matchedConteneur.StatCtr == "Manifesté" || (matchedConteneurTC != null ? matchedConteneurTC.StatutTC == "Manifesté" : true))
                    {
                        matchedConteneur.StatCtr = "Déchargé";
                    }
                    else
                    {
                        throw new IdentificationException("Ce conteneur a déjà été identifié");
                    }
                }

                matchedConteneur.StatCtr = "Déchargé";

                if (opIdentification == null)
                {
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 12;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);
                }

                dcMar.SubmitChanges();

                if (matchedConteneurTC != null)
                {
                    matchedConteneurTC.StatutTC = "Débarqué";
                    matchedConteneurTC.DateDébarquementSCR = DateTime.Now;
                    matchedConteneurTC.IdUserDébarquement = matchedUser.IdU;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = 4;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 12;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    dcMar.SubmitChanges();
                }

                var listConteneurIdem = (from ctr in dcMar.GetTable<CONTENEUR>()
                                         where ctr.NumCtr == matchedConteneur.NumCtr && ctr.IdCtr != idCtr && ctr.IdMan == matchedConteneur.IdMan
                                         select ctr).ToList<CONTENEUR>();

                foreach (CONTENEUR ctr in listConteneurIdem)
                {
                    var opIdentIdem = (from op in dcMar.GetTable<OPERATION_CONTENEUR>()
                                       where op.IdTypeOp == 12 && op.IdCtr == ctr.IdCtr
                                       select op).FirstOrDefault<OPERATION_CONTENEUR>();

                    if (opIdentIdem == null)
                    {
                        OPERATION_CONTENEUR opCtrIdem = new OPERATION_CONTENEUR();
                        opCtrIdem.IdCtr = ctr.IdCtr;
                        opCtrIdem.IdTypeOp = 12;
                        opCtrIdem.DateOp = DateTime.Now;
                        opCtrIdem.IdU = idUser;
                        opCtrIdem.AIOp = observations;

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtrIdem);
                    }

                    ctr.StatCtr = "Déchargé";
                    if (ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>() != null)
                    {
                        CONTENEUR_TC ctrTC = ctr.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>();
                        ctrTC.StatutTC = "Débarqué";
                        ctrTC.DateDébarquementSCR = DateTime.Now;
                        ctrTC.IdUserDébarquement = matchedUser.IdU;

                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = DateTime.Now;
                        mvtTC.IdBL = matchedConteneur.IdBL;
                        mvtTC.IdEsc = matchedConteneur.IdEsc;
                        mvtTC.IdParc = 4;
                        mvtTC.IdTC = ctrTC.IdTC;
                        mvtTC.IdTypeOp = 12;
                        mvtTC.IdUser = matchedUser.IdU;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                    }

                    dcMar.SubmitChanges();
                }

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConteneur.IdBL && op.IdTypeOp == 37
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Déchargement démarré";
                }

                if (matchedConteneur.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 12) != 0) == matchedConteneur.CONNAISSEMENT.CONTENEUR.Count)
                {
                    OPERATION_CONNAISSEMENT matchedOpBLTerm = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedConteneur.IdBL && op.IdTypeOp == 38
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBLTerm.DateOp.HasValue)
                    {
                        matchedOpBLTerm.DateOp = DateTime.Now;
                        matchedOpBLTerm.IdU = idUser;
                        matchedOpBLTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedConteneur.IdMan && op.IdTypeOp == 45
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedConteneur.MANIFESTE.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 12) != 0) == matchedConteneur.MANIFESTE.CONTENEUR.Count)
                {
                    OPERATION_MANIFESTE matchedOpManTerm = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where op.IdMan == matchedConteneur.IdMan && op.IdTypeOp == 46
                                                            select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpManTerm.DateOp.HasValue)
                    {
                        matchedOpManTerm.DateOp = DateTime.Now;
                        matchedOpManTerm.IdU = idUser;
                        matchedOpManTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedConteneur.IdEsc && op.IdTypeOp == 53
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedConteneur.ESCALE.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 12) != 0) == matchedConteneur.ESCALE.CONTENEUR.Count)
                {
                    OPERATION_ESCALE matchedOpEscTerm = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where op.IdEsc == matchedConteneur.IdEsc && op.IdTypeOp == 54
                                                         select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEscTerm.DateOp.HasValue)
                    {
                        matchedOpEscTerm.DateOp = DateTime.Now;
                        matchedOpEscTerm.IdU = idUser;
                        matchedOpEscTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                matchedConteneur.Seal1Ctr = seal1;
                matchedConteneur.Seal2Ctr = seal2;

                foreach (TYPE_SINISTRE typeSin in listeSinistre)
                {
                    var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                           where inter.IdCtr == idCtr
                                           select inter).ToList<INTERCHANGE>();

                    listInterchange.FirstOrDefault<INTERCHANGE>(ctr => ctr.IdTypeSinistre == typeSin.IdTypeSinistre).InfoIdentification = "Y";
                    dcMar.INTERCHANGE.Context.SubmitChanges();
                }

                if (matchedConteneur.ESCALE.DRAEsc.HasValue)
                {
                    if (matchedConteneur.TypeCCtr.Substring(0, 2) == "20")
                    {
                        var matchedOpArm101 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                               where opArm.IdEsc == matchedConteneur.IdEsc && opArm.IdTypeOp == 101
                                               select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                        if (matchedOpArm101 != null)
                        {
                            if (matchedOpArm101.QTE.HasValue)
                            {
                                matchedOpArm101.QTE = matchedOpArm101.QTE + 1;
                            }
                            else
                            {
                                matchedOpArm101.QTE = 1;
                            }

                            if (matchedOpArm101.Poids.HasValue)
                            {
                                matchedOpArm101.Poids = matchedOpArm101.Poids + matchedConteneur.PoidsCCtr / 1000;
                            }
                            else
                            {
                                matchedOpArm101.Poids = matchedConteneur.PoidsCCtr / 1000;
                            }

                            if (matchedOpArm101.Volume.HasValue)
                            {
                                matchedOpArm101.Volume = matchedOpArm101.Volume + Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                            else
                            {
                                matchedOpArm101.Volume = Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                        }


                        var matchedOpArm117 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                               where opArm.IdEsc == matchedConteneur.IdEsc && opArm.IdTypeOp == 117
                                               select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                        if (matchedOpArm117 != null)
                        {
                            if (matchedOpArm117.QTE.HasValue)
                            {
                                matchedOpArm117.QTE = matchedOpArm117.QTE + 1;
                            }
                            else
                            {
                                matchedOpArm117.QTE = 1;
                            }

                            if (matchedOpArm117.Poids.HasValue)
                            {
                                matchedOpArm117.Poids = matchedOpArm117.Poids + matchedConteneur.PoidsCCtr / 1000;
                            }
                            else
                            {
                                matchedOpArm117.Poids = matchedConteneur.PoidsCCtr / 1000;
                            }

                            if (matchedOpArm117.Volume.HasValue)
                            {
                                matchedOpArm117.Volume = matchedOpArm117.Volume + Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                            else
                            {
                                matchedOpArm117.Volume = Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                        }
                    }
                    else if (matchedConteneur.TypeCCtr.Substring(0, 2) == "40")
                    {
                        var matchedOpArm102 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                               where opArm.IdEsc == matchedConteneur.IdEsc && opArm.IdTypeOp == 102
                                               select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                        if (matchedOpArm102 != null)
                        {
                            if (matchedOpArm102.QTE.HasValue)
                            {
                                matchedOpArm102.QTE = matchedOpArm102.QTE + 1;
                            }
                            else
                            {
                                matchedOpArm102.QTE = 1;
                            }

                            if (matchedOpArm102.Poids.HasValue)
                            {
                                matchedOpArm102.Poids = matchedOpArm102.Poids + matchedConteneur.PoidsCCtr / 1000;
                            }
                            else
                            {
                                matchedOpArm102.Poids = matchedConteneur.PoidsCCtr / 1000;
                            }

                            if (matchedOpArm102.Volume.HasValue)
                            {
                                matchedOpArm102.Volume = matchedOpArm102.Volume + Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                            else
                            {
                                matchedOpArm102.Volume = Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                        }

                        var matchedOpArm118 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                               where opArm.IdEsc == matchedConteneur.IdEsc && opArm.IdTypeOp == 118
                                               select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                        if (matchedOpArm118 != null)
                        {
                            if (matchedOpArm118.QTE.HasValue)
                            {
                                matchedOpArm118.QTE = matchedOpArm118.QTE + 1;
                            }
                            else
                            {
                                matchedOpArm118.QTE = 1;
                            }

                            if (matchedOpArm118.Poids.HasValue)
                            {
                                matchedOpArm118.Poids = matchedOpArm118.Poids + matchedConteneur.PoidsCCtr / 1000;
                            }
                            else
                            {
                                matchedOpArm118.Poids = matchedConteneur.PoidsCCtr / 1000;
                            }

                            if (matchedOpArm118.Volume.HasValue)
                            {
                                matchedOpArm118.Volume = matchedOpArm118.Volume + Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                            else
                            {
                                matchedOpArm118.Volume = Convert.ToInt32(matchedConteneur.VolMCtr);
                            }
                        }
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }

        public CONTENEUR SortirConteneur(int idCtr, DateTime dateSortie, List<TYPE_SINISTRE> listeSinistre, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de sortie").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de sortie sur un conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                               where dbl.IdDBL == matchedConteneur.IdDBL
                                               select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                if (matchedDemandeLivraison != null)
                {
                    if (!matchedDemandeLivraison.DVDBL.HasValue)
                    {
                        throw new ApplicationException("La demande de livraison de ce conteneur n'a pas été validée");
                    }
                }
                else
                {
                    throw new ApplicationException("Ce conteneur ne fait l'objet d'aucun bon de livrason");
                }

                if (!matchedConteneur.IdBS.HasValue)
                {
                    throw new ApplicationException("Sortie impossible : Ce conteneur ne fait l'objet d'aucun bon de sortie");
                }

                if (matchedConteneur.DSCtr.HasValue)
                {
                    throw new ApplicationException("Sortie impossible : Ce conteneur est déjà sorti");
                }

                if (matchedConteneur.FSCtr.Value.Date.AddDays(1) < dateSortie.Date && matchedUser.LU != "Admin")
                {
                    throw new ApplicationException("La date à laquelle vous avez calculé les surestaries a été dépassée, veuillez recalculer les surestaries de ce conteneur");
                }

                // inserer opération de sortie
                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                opCtr.IdCtr = idCtr;
                opCtr.IdTypeOp = 18;
                opCtr.DateOp = dateSortie;//AH 3aout16 DateTime.Now;
                opCtr.IdU = idUser;
                opCtr.AIOp = observations;

                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                matchedConteneur.DSCtr = dateSortie;
                matchedConteneur.StatCtr = "Livré";

                if (matchedConteneurTC != null)
                {
                    matchedConteneurTC.StatutTC = "Sorti";
                    matchedConteneurTC.DateSortiePleinSCR = DateTime.Now;
                    matchedConteneurTC.IdUserSortiePlein = matchedUser.IdU;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = dateSortie;//AH 3aout16 DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = 4;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 18;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    dcMar.SubmitChanges();
                }

                foreach (TYPE_SINISTRE typeSin in listeSinistre)
                {
                    var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                           where inter.IdCtr == idCtr
                                           select inter).ToList<INTERCHANGE>();

                    listInterchange.FirstOrDefault<INTERCHANGE>(ctr => ctr.IdTypeSinistre == typeSin.IdTypeSinistre).InfoSortie = "Y";
                    dcMar.INTERCHANGE.Context.SubmitChanges();
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }


        public CONTENEUR_TC InsertOperationTracking(int idTypeOp, int idCtr, DateTime dateOp, int idUser, int idParc, string statut)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedCtrTC = (from ctrTC in dcMar.GetTable<CONTENEUR_TC>()
                                    where ctrTC.IdCtr == idCtr
                                    select ctrTC).SingleOrDefault<CONTENEUR_TC>();

                var matchedMvt = (from mvtTC in dcMar.GetTable<MOUVEMENT_TC>()
                                  where mvtTC.CONTENEUR_TC.IdCtr == idCtr && mvtTC.IdTypeOp == idTypeOp
                                  select mvtTC).FirstOrDefault<MOUVEMENT_TC>();

                if (matchedCtrTC != null && matchedMvt == null)
                {
                    if (idTypeOp == 18)
                    {
                        // inserer opération de sortie
                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                        opCtr.IdCtr = idCtr;
                        opCtr.IdTypeOp = 18;
                        opCtr.DateOp = dateOp;
                        opCtr.IdU = idUser;
                        opCtr.AIOp = "";

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                        matchedCtrTC.CONTENEUR.DSCtr = dateOp;

                        matchedCtrTC.DateSortiePleinSCR = dateOp;
                        matchedCtrTC.IdUserSortiePlein = idUser;

                        MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                        mvtTC.DateMvt = dateOp;
                        mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                        mvtTC.IdEsc = matchedCtrTC.CONTENEUR.IdEsc;
                        mvtTC.IdParc = idParc;
                        mvtTC.IdTC = matchedCtrTC.IdTC;
                        mvtTC.IdTypeOp = 18;
                        mvtTC.IdUser = idUser;

                        dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                        dcMar.SubmitChanges();
                    }
                    else if (idTypeOp == 19)
                    {
                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                        opCtr.IdCtr = idCtr;
                        opCtr.IdTypeOp = 19;
                        opCtr.DateOp = DateTime.Now;
                        opCtr.IdU = idUser;
                        opCtr.AIOp = "";

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                        matchedCtrTC.CONTENEUR.DRCtr = dateOp;

                        if (matchedCtrTC != null)
                        {
                            matchedCtrTC.DateRetourVideSCR = dateOp;
                            matchedCtrTC.IdUserRetourVide = idUser;
                            matchedCtrTC.IdParcRetourVide = idParc;

                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = matchedCtrTC.CONTENEUR.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 19;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                            dcMar.SubmitChanges();
                        }
                    }
                    else if (idTypeOp == 282)
                    {
                        // inserer opération de cargo loading
                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                        opCtr.IdCtr = matchedCtrTC.IdCtr;
                        opCtr.IdTypeOp = 282;
                        opCtr.DateOp = dateOp;
                        opCtr.IdU = idUser;
                        opCtr.AIOp = "";

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                        CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                               where ctrE.NumCtr == matchedCtrTC.NumTC && ctrE.SensCtr == "E"
                                               orderby ctrE.IdCtr descending
                                               select ctrE).FirstOrDefault<CONTENEUR>();

                        if (ctrExport != null && matchedCtrTC.CONTENEUR.ESCALE.IdArm == 1)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = ctrExport.IdBL;
                            mvtTC.IdEsc = ctrExport.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 282;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }
                        else if (ctrExport != null && matchedCtrTC.CONTENEUR.ESCALE.IdArm == 2)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = ctrExport.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 282;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }
                        else if (matchedCtrTC.CONTENEUR.ESCALE.IdArm == 2)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = matchedCtrTC.CONTENEUR.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 282;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }

                        matchedCtrTC.DateRetourPlein = dateOp;
                        matchedCtrTC.IdUserRetourPlein = idUser;
                    }
                    else if (idTypeOp == 283)
                    {
                        OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                        opCtr.IdCtr = matchedCtrTC.IdCtr;
                        opCtr.IdTypeOp = 283;
                        opCtr.DateOp = dateOp;
                        opCtr.IdU = idUser;
                        opCtr.AIOp = "";

                        dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                        CONTENEUR ctrExport = (from ctrE in dcMar.GetTable<CONTENEUR>()
                                               where ctrE.NumCtr == matchedCtrTC.NumTC && ctrE.SensCtr == "E"
                                               orderby ctrE.IdCtr descending
                                               select ctrE).FirstOrDefault<CONTENEUR>();

                        if (ctrExport != null && matchedCtrTC.CONTENEUR.ESCALE.IdArm == 1)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = ctrExport.IdBL;
                            mvtTC.IdEsc = ctrExport.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 283;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }
                        else if (ctrExport != null && matchedCtrTC.CONTENEUR.ESCALE.IdArm == 2)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = ctrExport.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 283;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }
                        else if (matchedCtrTC.CONTENEUR.ESCALE.IdArm == 2)
                        {
                            MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                            mvtTC.DateMvt = dateOp;
                            mvtTC.IdBL = matchedCtrTC.CONTENEUR.IdBL;
                            mvtTC.IdEsc = matchedCtrTC.CONTENEUR.IdEsc;
                            mvtTC.IdParc = idParc;
                            mvtTC.IdTC = matchedCtrTC.IdTC;
                            mvtTC.IdTypeOp = 283;
                            mvtTC.IdUser = idUser;

                            dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);
                        }

                        matchedCtrTC.DateRetourPlein = dateOp;
                        matchedCtrTC.IdUserRetourPlein = idUser;
                    }

                    matchedCtrTC.StatutTC = statut;
                    matchedCtrTC.CONTENEUR.StatCtr = statut;
                }
                else if (matchedMvt != null)
                {
                    throw new ApplicationException("Ce mouvement existe déjà sur ce conteneur");
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedCtrTC;
            }
        }


        public CONTENEUR ParquerConteneur(int idCtr, int idEmpl, string etat, List<TYPE_SINISTRE> listeSinistre, string observations, int idUser, DateTime _date)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de parquing").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de parquing de conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneur.StatCtr == "Cargo Loaded")
                {
                    throw new ApplicationException("Operation de parquinq impossible : Ce conteneur a déjà été embarqué");
                }

                if (matchedConteneurTC != null)
                {
                    //if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "En habillage" && matchedConteneurTC.StatutTC != "En réparation")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est ni retourné, ni présent dans le parc");
                    //}

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 278;
                    opCtr.DateOp = _date; //DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    matchedConteneur.StatCtr = "Parqué";

                    matchedConteneurTC.StatutTC = "Parqué";
                    matchedConteneurTC.DateParquing = DateTime.Now;
                    matchedConteneurTC.IdUserParquing = matchedUser.IdU;
                    matchedConteneurTC.IdParcParquing = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    matchedConteneurTC.IdEmplacementParc = idEmpl;
                    matchedConteneurTC.EtatRetourVide = etat;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = _date;//DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 278;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    dcMar.SubmitChanges();

                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = _date;//DateTime.Now;
                    occupation.IdCtr = idCtr;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 278;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                    dcMar.SubmitChanges();

                    foreach (TYPE_SINISTRE typeSin in listeSinistre)
                    {
                        var listInterchange = (from inter in dcMar.GetTable<INTERCHANGE>()
                                               where inter.IdCtr == idCtr
                                               select inter).ToList<INTERCHANGE>();

                        listInterchange.FirstOrDefault<INTERCHANGE>(ctr => ctr.IdTypeSinistre == typeSin.IdTypeSinistre).InfoParquing = "Y";
                        dcMar.INTERCHANGE.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }


        public CONTENEUR TransfConteneurZoneReparation(int idCtr, int idEmpl, string reparation, string nettoyage, string lavage, string irreparable, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de transfert en zone de réparation").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de transfert de conteneur en zone de réparation. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneurTC != null)
                {

                    //if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "Parqué" && matchedConteneurTC.StatutTC != "En habillage")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est pas présent dans le parc");
                    //}

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 279;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    matchedConteneur.StatCtr = "En réparation";

                    matchedConteneurTC.StatutTC = "En réparation";
                    matchedConteneurTC.DateTransfertZR = DateTime.Now;
                    matchedConteneurTC.IdUserTransfertZR = matchedUser.IdU;
                    matchedConteneurTC.IdParcTransfertZR = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    matchedConteneurTC.IdEmplacementZR = idEmpl;
                    matchedConteneurTC.Reparation = reparation;
                    matchedConteneurTC.Nettoyage = nettoyage;
                    matchedConteneurTC.Lavage = lavage;
                    matchedConteneurTC.Irreparable = irreparable;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 279;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                              where (occ.IdCtr == matchedConteneurTC.IdCtr && !occ.DateFin.HasValue)
                                              select occ).FirstOrDefault<OCCUPATION>();

                    if (matchedOcccupation != null)
                    {
                        matchedOcccupation.DateFin = DateTime.Now;
                    }

                    //inserer opération d'occupation d'emplacement
                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdCtr = matchedConteneurTC.IdCtr;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 279;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }


        public CONTENEUR TransfConteneurZoneHabillage(int idCtr, int idEmpl, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                        where ctr.IdCtr == idCtr
                                        select ctr).FirstOrDefault<CONTENEUR>();

                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de transfert en zone d'habillage").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de transfert de conteneur en zone d'habillage. Veuillez contacter un administrateur");
                }

                if (matchedConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }

                if (matchedConteneurTC != null)
                {

                    //if (matchedConteneurTC.StatutTC != "Retourné" && matchedConteneurTC.StatutTC != "Parqué" && matchedConteneurTC.StatutTC != "En réparation")
                    //{
                    //    throw new ApplicationException("Ce conteneur n'est pas présent dans le parc");
                    //}

                    // inserer opération de retour
                    OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                    opCtr.IdCtr = idCtr;
                    opCtr.IdTypeOp = 280;
                    opCtr.DateOp = DateTime.Now;
                    opCtr.IdU = idUser;
                    opCtr.AIOp = observations;

                    dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                    matchedConteneur.StatCtr = "En habillage";

                    matchedConteneurTC.StatutTC = "En habillage";
                    matchedConteneurTC.DateTransfertZH = DateTime.Now;
                    matchedConteneurTC.IdUserTransfertZH = matchedUser.IdU;
                    matchedConteneurTC.IdParcTransfertZH = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    matchedConteneurTC.IdEmplacementZH = idEmpl;

                    MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                    mvtTC.DateMvt = DateTime.Now;
                    mvtTC.IdBL = matchedConteneur.IdBL;
                    mvtTC.IdEsc = matchedConteneur.IdEsc;
                    mvtTC.IdParc = dcMar.EMPLACEMENT.FirstOrDefault<EMPLACEMENT>(em => em.IdEmpl == idEmpl).IdParc;
                    mvtTC.IdTC = matchedConteneurTC.IdTC;
                    mvtTC.IdTypeOp = 280;
                    mvtTC.IdUser = matchedUser.IdU;

                    dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                    var matchedOcccupation = (from occ in dcMar.GetTable<OCCUPATION>()
                                              where (occ.IdCtr == matchedConteneurTC.IdCtr && !occ.DateFin.HasValue)
                                              select occ).FirstOrDefault<OCCUPATION>();

                    if (matchedOcccupation != null)
                    {
                        matchedOcccupation.DateFin = DateTime.Now;
                    }

                    //inserer opération d'occupation d'emplacement
                    OCCUPATION occupation = new OCCUPATION();
                    occupation.DateDebut = DateTime.Now;
                    occupation.IdCtr = matchedConteneurTC.IdCtr;
                    occupation.IdEmpl = idEmpl;
                    occupation.IdTypeOp = 280;

                    dcMar.GetTable<OCCUPATION>().InsertOnSubmit(occupation);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneur;
            }
        }
        
        public MOUVEMENT_TC UpdateOperationTracking(int idMvt, DateTime dateOp, int idUser, int idParc, string statut)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMvt = (from elt in dcMar.GetTable<MOUVEMENT_TC>()
                                  where elt.IdMvt == idMvt
                                  select elt).SingleOrDefault<MOUVEMENT_TC>();

                matchedMvt.DateMvt = dateOp.AddHours(matchedMvt.DateMvt.Value.Hour).AddMinutes(matchedMvt.DateMvt.Value.Minute).AddSeconds(matchedMvt.DateMvt.Value.Second);
                matchedMvt.IdUser = idUser;
                matchedMvt.IdParc = idParc;

                var matchedListOp = (from elt in dcMar.GetTable<OPERATION_CONTENEUR>()
                                     where elt.IdTypeOp == matchedMvt.IdTypeOp && elt.IdCtr == matchedMvt.CONTENEUR_TC.IdCtr
                                     select elt).ToList<OPERATION_CONTENEUR>();

                foreach (OPERATION_CONTENEUR opCtr in matchedListOp)
                {
                    opCtr.DateOp = matchedMvt.DateMvt;
                    opCtr.IdU = idUser;
                }

                matchedMvt.CONTENEUR_TC.StatutTC = statut;
                matchedMvt.CONTENEUR_TC.CONTENEUR.StatCtr = statut;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMvt;
            }
        }

     
        /// <summary>
        /// Retour vide non empoté sur parc socomar; retourn le conteneur export
        /// </summary>
        /// <param name="idCtr"></param>
        /// <param name="dateRetour"></param>
        /// <param name="idParc"></param>
        /// <param name="observations"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public CONTENEUR_TC RetourVideConteneur(int idCtr, DateTime dateRetour, int idParc, string observations, int idUser)
        {

            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConteneurTC = (from ctr in dcMar.GetTable<CONTENEUR_TC>()
                                          where ctr.IdCtr == idCtr
                                          select ctr).FirstOrDefault<CONTENEUR_TC>();



                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Conteneur : Enregistrement de l'opération de retour").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de retour de conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConteneurTC == null)
                {
                    throw new EnregistrementInexistant("Conteneur inexistant");
                }


                if (matchedConteneurTC.StatutTC != "Mis à disposition")
                {
                    throw new ApplicationException("Ce conteneur ne peut plus faire l'objet de retour vide");
                }

                var matchedExportConteneur = (from ctr in dcMar.GetTable<CONTENEUR>()
                                              where ctr.IdCtr == matchedConteneurTC.IdCtrExport
                                              select ctr).FirstOrDefault<CONTENEUR>();
                if (matchedExportConteneur == null)
                {
                    throw new EnregistrementInexistant("Conteneur <<export>> inexistant");
                }

                if (matchedExportConteneur.CONNAISSEMENT.StatutBL != "Booking")
                {
                    throw new ApplicationException("Impossible d'effectuer cette opération. Statut booking : " + matchedExportConteneur.CONNAISSEMENT.StatutBL);
                }

                if (matchedExportConteneur.SensCtr != "E")
                {
                    throw new EnregistrementInexistant("Erreur de linkage du conteneur. Veuillez contacter l'administrateur");
                }

                // inserer opération de retour vide
                OPERATION_CONTENEUR opCtr = new OPERATION_CONTENEUR();
                opCtr.IdCtr = idCtr;
                opCtr.IdTypeOp = 290;
                opCtr.DateOp = dateRetour;
                opCtr.IdU = idUser;
                opCtr.AIOp = observations;

                dcMar.GetTable<OPERATION_CONTENEUR>().InsertOnSubmit(opCtr);

                matchedConteneurTC.StatutTC = "Retourné";
                matchedConteneurTC.IdCtrExport = null;
                //matchedConteneurTC.DateRetourVideSCR = dateRetour;
                //matchedConteneurTC.IdUserRetourVide = matchedUser.IdU;
                //matchedConteneurTC.IdParcRetourVide = idParc;

                MOUVEMENT_TC mvtTC = new MOUVEMENT_TC();

                mvtTC.DateMvt = dateRetour;
                mvtTC.IdBL = matchedConteneurTC.IdBooking;
                mvtTC.IdEsc = matchedExportConteneur.IdEsc;
                mvtTC.IdParc = idParc;
                mvtTC.IdTC = matchedConteneurTC.IdTC;
                mvtTC.IdTypeOp = 290;
                mvtTC.IdUser = matchedUser.IdU;

                dcMar.GetTable<MOUVEMENT_TC>().InsertOnSubmit(mvtTC);

                dcMar.GetTable<CONTENEUR>().DeleteOnSubmit(matchedExportConteneur);

                string machinename = string.Empty;
                try { machinename = Environment.MachineName; }
                catch { }

                JOURNAL journal = new JOURNAL
                {
                    IdU = matchedUser.IdU,
                    IdOp = 46,
                    DOP = DateTime.Now,
                    IDEC = string.Format("PC:{0}; Operation:Retour non empoté; CtrId:{1}; Bl:{2}; Esc:{3}", machinename, matchedExportConteneur.IdCtr,
                    matchedExportConteneur.IdBL, matchedExportConteneur.IdEsc)
                };
                dcMar.GetTable<JOURNAL>().InsertOnSubmit(journal);

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConteneurTC;
            }
        }

        #endregion

        #region bon de sortie

        public BON_SORTIE InsertBonSortie(int idBL, List<VEHICULE> vehs, List<CONTENEUR> ctrs, List<CONVENTIONNEL> gcs, List<MAFI> mfs, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Bon de sortie : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer un bon de sortie. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement Inexistant");
                }

                BON_SORTIE bonSortie = new BON_SORTIE();

                bonSortie.DateBS = DateTime.Now;
                bonSortie.IdBL = matchedConnaissement.IdBL;
                bonSortie.AIBS = autresInfos;
                bonSortie.IdU = idUser;

                dcMar.GetTable<BON_SORTIE>().InsertOnSubmit(bonSortie);

                dcMar.SubmitChanges();
                //TODO: il faut penser apuré ce bon de sortie genere qui ne sera affectue a aucun vehicule.

                foreach (VEHICULE veh in vehs)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == veh.IdVeh
                                      select v).SingleOrDefault<VEHICULE>();

                    var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                                   where dbl.IdDBL == matchedVeh.IdDBL
                                                   select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                    if (matchedDemandeLivraison != null)
                    {
                        if (!matchedDemandeLivraison.DVDBL.HasValue)
                        {
                            throw new ApplicationException("La demande de livraison de ce véhicule n'a pas été validée");
                        }
                    }  
                    else
                    {
                        throw new ApplicationException("Ce véhicule ne fait l'objet d'aucun bon de livrason");
                    }
                    //AH control de sejour parc auto
                    if (matchedVeh.FSVeh < DateTime.Today)
                    { throw new ApplicationException("Bon de sortie impossible : Fin de séjour calculé à " + matchedVeh.FSVeh+" pour "+matchedVeh.NumChassis); }

                    //matchedVeh.StatVeh = "Livré";
                    matchedVeh.IdBS = bonSortie.IdBS;
                }

                #region sortie conteneur
                foreach (CONTENEUR ctr in ctrs)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == ctr.IdCtr
                                      select c).SingleOrDefault<CONTENEUR>();

                    var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                                   where dbl.IdDBL == matchedCtr.IdDBL
                                                   select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                    if (matchedDemandeLivraison != null)
                    {
                        if (!matchedDemandeLivraison.DVDBL.HasValue)
                        {
                            throw new ApplicationException("La demande de livraison de ce conteneur n'a pas été validée");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Ce conteneur ne fait l'objet d'aucun bon de livrason");
                    }

                    //matchedCtr.StatCtr = "Livré";
                    matchedCtr.IdBS = bonSortie.IdBS;

                } 
                #endregion

                #region sortie mafi
                foreach (MAFI mf in mfs)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == mf.IdMafi
                                       select m).SingleOrDefault<MAFI>();

                    var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                                   where dbl.IdDBL == matchedMafi.IdDBL
                                                   select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                    if (matchedDemandeLivraison != null)
                    {
                        if (!matchedDemandeLivraison.DVDBL.HasValue)
                        {
                            throw new ApplicationException("La demande de livraison de ce mafi n'a pas été validée");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Ce mafi ne fait l'objet d'aucun bon de livrason");
                    }

                    //matchedCtr.StatCtr = "Livré";
                    matchedMafi.IdBS = bonSortie.IdBS;

                } 
                #endregion

                #region sortie conventionnel
                foreach (CONVENTIONNEL conv in gcs)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == conv.IdGC
                                     select c).SingleOrDefault<CONVENTIONNEL>();

                    var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                                   where dbl.IdDBL == matchedGC.IdDBL
                                                   select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                    if (matchedDemandeLivraison != null)
                    {
                        if (!matchedDemandeLivraison.DVDBL.HasValue)
                        {
                            throw new ApplicationException("La demande de livraison de ce general cargo n'a pas été validée");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Ce general cargo ne fait l'objet d'aucun bon de livrason");
                    }

                    //matchedGC.StatGC = "Livré";
                    matchedGC.IdBS = bonSortie.IdBS;
                } 
                #endregion

                int nbVehBL = bonSortie.CONNAISSEMENT.VEHICULE.Count;
                int nbVehBLBS = bonSortie.CONNAISSEMENT.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrBL = bonSortie.CONNAISSEMENT.CONTENEUR.Count;
                int nbCtrBLBS = bonSortie.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbMafiBL = bonSortie.CONNAISSEMENT.MAFI.Count;
                int nbMafiBLBS = bonSortie.CONNAISSEMENT.MAFI.Count(mf => mf.IdBS.HasValue);

                int nbGCBL = bonSortie.CONNAISSEMENT.CONVENTIONNEL.Count;
                int nbGCBLBS = bonSortie.CONNAISSEMENT.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehBL == nbVehBLBS && nbCtrBL == nbCtrBLBS && nbGCBL == nbGCBLBS && nbMafiBL == nbMafiBLBS && nbCtrBL == 0)
                {
                    OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                           where op.IdBL == bonSortie.IdBL && op.IdTypeOp == 43
                                                           select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBL.DateOp.HasValue)
                    {
                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Clôture";
                    }

                    bonSortie.CONNAISSEMENT.StatutBL = "Cloturé";
                }

                dcMar.SubmitChanges();

                int nbVehMan = bonSortie.CONNAISSEMENT.MANIFESTE.VEHICULE.Count;
                int nbVehManBS = bonSortie.CONNAISSEMENT.MANIFESTE.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrMan = bonSortie.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count;
                int nbCtrManBS = bonSortie.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbMafiMan = bonSortie.CONNAISSEMENT.MANIFESTE.MAFI.Count;
                int nbMafiManBS = bonSortie.CONNAISSEMENT.MANIFESTE.MAFI.Count(mf => mf.IdBS.HasValue);

                int nbGCMan = bonSortie.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count;
                int nbGCManBS = bonSortie.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehMan == nbVehManBS && nbCtrMan == nbCtrManBS && nbGCMan == nbGCManBS && nbMafiMan == nbMafiManBS && nbCtrMan == 0)
                {
                    OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                        where op.IdMan == bonSortie.CONNAISSEMENT.IdMan && op.IdTypeOp == 51
                                                        select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpMan.DateOp.HasValue)
                    {
                        matchedOpMan.DateOp = DateTime.Now;
                        matchedOpMan.IdU = idUser;
                        matchedOpMan.AIOp = "Clôture";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehEsc = bonSortie.CONNAISSEMENT.ESCALE.VEHICULE.Count;
                int nbVehEscBS = bonSortie.CONNAISSEMENT.ESCALE.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrEsc = bonSortie.CONNAISSEMENT.ESCALE.CONTENEUR.Count;
                int nbCtrEscBS = bonSortie.CONNAISSEMENT.ESCALE.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbMafiEsc = bonSortie.CONNAISSEMENT.ESCALE.MAFI.Count;
                int nbMafiEscBS = bonSortie.CONNAISSEMENT.ESCALE.MAFI.Count(mf => mf.IdBS.HasValue);

                int nbGCEsc = bonSortie.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count;
                int nbGCEscBS = bonSortie.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehEsc == nbVehEscBS && nbCtrEsc == nbCtrEscBS && nbGCEsc == nbGCEscBS && nbMafiEsc == nbMafiEscBS && nbCtrEsc == 0)
                {
                    OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                     where op.IdEsc == bonSortie.CONNAISSEMENT.IdEsc && op.IdTypeOp == 59
                                                     select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEsc.DateOp.HasValue)
                    {
                        matchedOpEsc.DateOp = DateTime.Now;
                        matchedOpEsc.IdU = idUser;
                        matchedOpEsc.AIOp = "Clôture";
                    }
                }

                dcMar.SubmitChanges();

                dcMar.SubmitChanges();
                transaction.Complete();
                return bonSortie;
            }
        }

        #endregion

        #region ordre de service


        public ORDRE_SERVICE UpdateOrdreService(int idOS, int idFsseur, int idEsc, int idBL, string libOS, DateTime datePrevEx, DateTime dateReelleEx, int idVeh, int idCtr, int idMafi, int idGC, List<LIGNE_SERVICE> lignesOS, string autresInfos, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedOS = (from os in dcMar.GetTable<ORDRE_SERVICE>()
                                 where os.IdOS == idOS
                                 select os).SingleOrDefault<ORDRE_SERVICE>();

                var matchedEscale = (from esc in dcMar.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                var matchedBL = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).FirstOrDefault<CONNAISSEMENT>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Ordre de service : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un ordre de service. Veuillez contacter un administrateur");
                }

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                if (matchedOS == null)
                {
                    throw new EnregistrementInexistant("L'ordre de service auquel vous faites référence n'existe pas");
                }

                if (idBL != -1 && matchedBL == null)
                {
                    throw new EnregistrementInexistant("Le connaissement auquel vous faites référence n'existe pas");
                }

                dcMar.LIGNE_SERVICE.DeleteAllOnSubmit<LIGNE_SERVICE>(matchedOS.LIGNE_SERVICE);

                matchedOS.DPExOS = datePrevEx;
                matchedOS.DCrOS = DateTime.Now;
                if (dateReelleEx.Ticks != 0)
                {
                    matchedOS.DRExOS = dateReelleEx;
                }
                matchedOS.LibOS = libOS;
                matchedOS.IdFsseur = idFsseur;
                matchedOS.IdEsc = matchedEscale.IdEsc;
                if (idBL != -1)
                {
                    matchedOS.IdBL = matchedBL.IdBL;
                }
                if (idVeh != -1)
                {
                    var matchedVeh = (from v in dcMar.GetTable<VEHICULE>()
                                      where v.IdVeh == idVeh
                                      select v).FirstOrDefault<VEHICULE>();

                    matchedOS.IdVeh = idVeh;
                }

                if (idCtr != -1)
                {
                    var matchedCtr = (from c in dcMar.GetTable<CONTENEUR>()
                                      where c.IdCtr == idCtr
                                      select c).FirstOrDefault<CONTENEUR>();

                    matchedOS.IdCtr = idCtr;
                }

                if (idMafi != -1)
                {
                    var matchedMafi = (from m in dcMar.GetTable<MAFI>()
                                       where m.IdMafi == idMafi
                                       select m).FirstOrDefault<MAFI>();

                    matchedOS.IdMafi = idMafi;
                }

                if (idGC != -1)
                {
                    var matchedGC = (from c in dcMar.GetTable<CONVENTIONNEL>()
                                     where c.IdGC == idGC
                                     select c).FirstOrDefault<CONVENTIONNEL>();

                    matchedOS.IdGC = idGC;
                }

                matchedOS.ObsOS = autresInfos;

                dcMar.SubmitChanges();

                foreach (LIGNE_SERVICE lg in lignesOS)
                {
                    lg.IdOS = matchedOS.IdOS;
                }

                matchedOS.LIGNE_SERVICE.AddRange(lignesOS);

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedOS;
            }
        }

        public ORDRE_SERVICE AnnulerOrdreService(int idOS, string infosAnnulation, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                // Vérification de l'existance des enregistrements pour contrainte d'intégrité
                var matchedOS = (from os in dcMar.GetTable<ORDRE_SERVICE>()
                                 where os.IdOS == idOS
                                 select os).SingleOrDefault<ORDRE_SERVICE>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Ordre de service : Annulation d'un élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour annuler un ordre de service. Veuillez contacter un administrateur");
                }

                if (matchedOS == null)
                {
                    throw new EnregistrementInexistant("L'ordre de service auquel vous faites référence n'existe pas");
                }

                if (matchedOS.DVOS.HasValue)
                {
                    throw new ApplicationException("Annulation impossible : Cet ordre de service a déjà été validé");
                }

                matchedOS.StatutOS = "Annulé";

                if (infosAnnulation.Trim() != "")
                {
                    NOTE noteAnnulation = new NOTE();
                    noteAnnulation.IdOS = matchedOS.IdBL;
                    noteAnnulation.DateNote = DateTime.Now;
                    noteAnnulation.IdU = idUser;
                    noteAnnulation.TitreNote = "Note d'annulation";
                    noteAnnulation.DescNote = infosAnnulation;

                    dcMar.NOTE.InsertOnSubmit(noteAnnulation);
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedOS;
            }
        }


        #endregion

        #region conventionnel


        public CONVENTIONNEL UpdateInfosLivraisonGC(int idGC, string nomEnleveur, string cniEnleveur, string telEnleveur, string numAttDedouanement, string numCivio, string numDeclDouane, string numQuittDouane, string numFactPAD, string numQuittPAD, string numBAE, string numBESC, string numSydonia, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConv = (from gc in dcMar.GetTable<CONVENTIONNEL>()
                                   where gc.IdGC == idGC
                                   select gc).SingleOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "General cargo : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations de livraison. Veuillez contacter un administrateur");
                }

                if (matchedConv == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                matchedConv.NomEnGC = nomEnleveur;
                matchedConv.CNIEnGC = cniEnleveur;
                matchedConv.TelenGC = telEnleveur;
                matchedConv.NumADDGC = numAttDedouanement;
                matchedConv.NumCIVIOGC = numCivio;
                matchedConv.NumDDGC = numDeclDouane;
                matchedConv.NumQDGC = numQuittDouane;
                matchedConv.NumFPADGC = numFactPAD;
                matchedConv.NumQPADGC = numQuittPAD;
                matchedConv.NumAEPADGC = numBAE;
                matchedConv.NumBESCGC = numBESC;
                matchedConv.NumSydoniaGC = numSydonia;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConv;
            }
        }

        public CONVENTIONNEL UpdateInfosEnlevementGC(int idGC, DateTime sortiePrev, string numBESC, string nomEnleveur, string cniEnleveur, string telEnleveur, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConv = (from gc in dcMar.GetTable<CONVENTIONNEL>()
                                   where gc.IdGC == idGC
                                   select gc).SingleOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "General cargo : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations d'enlèvement. Veuillez contacter un administrateur");
                }

                if (matchedConv == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                matchedConv.DSPGC = sortiePrev;
                matchedConv.NomEnGC = nomEnleveur;
                matchedConv.CNIEnGC = cniEnleveur;
                matchedConv.TelenGC = telEnleveur;
                matchedConv.NumBESCGC = numBESC;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConv;
            }
        }

        public CONVENTIONNEL SortirConventionnel(int idGC, DateTime dateSortie, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).FirstOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Enregistrement de l'opération de sortie").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de sortie sur un general cargo. Veuillez contacter un administrateur");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("General cargo inexistant");
                }

                var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                               where dbl.IdDBL == matchedConventionnel.IdDBL
                                               select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                if (matchedDemandeLivraison != null)
                {
                    if (!matchedDemandeLivraison.DVDBL.HasValue)
                    {
                        throw new ApplicationException("La demande de livraison de ce general cargo n'a pas été validée");
                    }
                }
                else
                {
                    throw new ApplicationException("Ce general cargo ne fait l'objet d'aucun bon de livrason");
                }

                // inserer opération de sortie
                OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                opGC.IdGC = idGC;
                opGC.IdTypeOp = 32;
                opGC.DateOp = DateTime.Now;
                opGC.IdU = idUser;
                opGC.AIOp = observations;

                dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);

                matchedConventionnel.DSPGC = DateTime.Now;
                matchedConventionnel.StatGC = "Sortie";

                //Clôture des éléments macro
                int nbVehBL = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.VEHICULE.Count;
                int nbVehBLBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrBL = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.CONTENEUR.Count;
                int nbCtrBLBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbGCBL = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.CONVENTIONNEL.Count;
                int nbGCBLBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehBL == nbVehBLBS && nbCtrBL == nbCtrBLBS && nbGCBL == nbGCBLBS)
                {
                    OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                           where op.IdBL == matchedConventionnel.BON_SORTIE.IdBL && op.IdTypeOp == 43
                                                           select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBL.DateOp.HasValue)
                    {
                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Clôture";
                    }

                    matchedConventionnel.BON_SORTIE.CONNAISSEMENT.StatutBL = "Cloturé";
                }

                dcMar.SubmitChanges();

                int nbVehMan = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.VEHICULE.Count;
                int nbVehManBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrMan = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count;
                int nbCtrManBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbGCMan = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count;
                int nbGCManBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.MANIFESTE.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehMan == nbVehManBS && nbCtrMan == nbCtrManBS && nbGCMan == nbGCManBS)
                {
                    OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                        where op.IdMan == matchedConventionnel.BON_SORTIE.CONNAISSEMENT.IdMan && op.IdTypeOp == 51
                                                        select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpMan.DateOp.HasValue)
                    {
                        matchedOpMan.DateOp = DateTime.Now;
                        matchedOpMan.IdU = idUser;
                        matchedOpMan.AIOp = "Clôture";
                    }
                }

                dcMar.SubmitChanges();

                int nbVehEsc = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.VEHICULE.Count;
                int nbVehEscBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.VEHICULE.Count(veh => veh.IdBS.HasValue);

                int nbCtrEsc = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.CONTENEUR.Count;
                int nbCtrEscBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.CONTENEUR.Count(ctr => ctr.IdBS.HasValue);

                int nbGCEsc = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count;
                int nbGCEscBS = matchedConventionnel.BON_SORTIE.CONNAISSEMENT.ESCALE.CONVENTIONNEL.Count(gc => gc.IdBS.HasValue);

                if (nbVehEsc == nbVehEscBS && nbCtrEsc == nbCtrEscBS && nbGCEsc == nbGCEscBS)
                {
                    OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                     where op.IdEsc == matchedConventionnel.BON_SORTIE.CONNAISSEMENT.IdEsc && op.IdTypeOp == 59
                                                     select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEsc.DateOp.HasValue)
                    {
                        matchedOpEsc.DateOp = DateTime.Now;
                        matchedOpEsc.IdU = idUser;
                        matchedOpEsc.AIOp = "Clôture";
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONVENTIONNEL CuberConventionnel(int idGC, double longGC, double largGC, double hautGC, double volGC, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).FirstOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Modification des informations de cubage").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour cuber un general cargo. Veuillez contacter un administrateur");
                }

                var matchedOpCubage = (from op in dcMar.GetTable<OPERATION_CONVENTIONNEL>()
                                       where op.IdGC == idGC && op.IdTypeOp == 26
                                       select op).FirstOrDefault<OPERATION_CONVENTIONNEL>();

                if (matchedOpCubage != null)
                {
                    throw new CubageException("Ce general cargo a déjà été cubé");
                }

                // inserer opération de cubage
                OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                opGC.IdGC = idGC;
                opGC.IdTypeOp = 26;
                opGC.DateOp = DateTime.Now;
                opGC.IdU = idUser;
                opGC.AIOp = observations;

                dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);

                // Mise à jour des dimensions
                matchedConventionnel.LongCGC = (float)Math.Round(longGC, 3);
                matchedConventionnel.LargCGC = (float)Math.Round(largGC, 3);
                matchedConventionnel.HautCGC = (float)Math.Round(hautGC, 3);
                matchedConventionnel.VolCGC = (float)Math.Round(volGC, 3);

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONVENTIONNEL ReceptionnerConventionnel(int idGC, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).FirstOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Modification des informations de réception").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour réceptionner un general cargo. Veuillez contacter un administrateur");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("General cargo inexistant");
                }

                // inserer opération de reception au parc
                OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                opGC.IdGC = idGC;
                opGC.IdTypeOp = 27;
                opGC.DateOp = DateTime.Now;
                opGC.IdU = idUser;
                opGC.AIOp = observations;

                dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);

                matchedConventionnel.StatGC = "Parqué";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONVENTIONNEL IdentifierConventionnel(int idGC, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).FirstOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un general cargo. Veuillez contacter un administrateur");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("General cargo inexistant");
                }

                //if (!matchedConventionnel.ESCALE.DRAEsc.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Le navire n'est pas encore arrivé");
                //}

                if (!matchedConventionnel.MANIFESTE.DVMan.HasValue)
                {
                    throw new IdentificationException("Echec de l'identification : Manifeste non validé");
                }

                var opIdentification = (from op in dcMar.GetTable<OPERATION_CONVENTIONNEL>()
                                        where op.IdTypeOp == 25 && op.IdGC == idGC
                                        select op).FirstOrDefault<OPERATION_CONVENTIONNEL>();

                if (opIdentification != null)
                {
                    throw new IdentificationException("Ce general cargo a déjà été identifié.");
                }

                matchedConventionnel.StatGC = "Identifié/Déchargé";

                OPERATION_CONVENTIONNEL opGC = new OPERATION_CONVENTIONNEL();
                opGC.IdGC = idGC;
                opGC.IdTypeOp = 25;
                opGC.DateOp = DateTime.Now;
                opGC.IdU = idUser;
                opGC.AIOp = observations;

                dcMar.GetTable<OPERATION_CONVENTIONNEL>().InsertOnSubmit(opGC);

                dcMar.SubmitChanges();

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedConventionnel.IdBL && op.IdTypeOp == 37
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 25) != 0) == matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count)
                {
                    OPERATION_CONNAISSEMENT matchedOpBLTerm = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedConventionnel.IdBL && op.IdTypeOp == 38
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBLTerm.DateOp.HasValue)
                    {
                        matchedOpBLTerm.DateOp = DateTime.Now;
                        matchedOpBLTerm.IdU = idUser;
                        matchedOpBLTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedConventionnel.IdMan && op.IdTypeOp == 45
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedConventionnel.MANIFESTE.CONVENTIONNEL.Count(conv => conv.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 25) != 0) == matchedConventionnel.MANIFESTE.CONVENTIONNEL.Count)
                {
                    OPERATION_MANIFESTE matchedOpManTerm = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where op.IdMan == matchedConventionnel.IdMan && op.IdTypeOp == 46
                                                            select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpManTerm.DateOp.HasValue)
                    {
                        matchedOpManTerm.DateOp = DateTime.Now;
                        matchedOpManTerm.IdU = idUser;
                        matchedOpManTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedConventionnel.IdEsc && op.IdTypeOp == 53
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedConventionnel.ESCALE.CONVENTIONNEL.Count(conv => conv.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 25) != 0) == matchedConventionnel.ESCALE.CONVENTIONNEL.Count)
                {
                    OPERATION_ESCALE matchedOpEscTerm = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where op.IdEsc == matchedConventionnel.IdEsc && op.IdTypeOp == 54
                                                         select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEscTerm.DateOp.HasValue)
                    {
                        matchedOpEscTerm.DateOp = DateTime.Now;
                        matchedOpEscTerm.IdU = idUser;
                        matchedOpEscTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONVENTIONNEL UpdateNumGC(int idGC, string numGC, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).SingleOrDefault<CONVENTIONNEL>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "General cargo : Mise à jour numéro general cargo").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour le numéro de general cargo. Veuillez contacter un administrateur");
                }

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("General cargo inexistant");
                }

                matchedConventionnel.NumGC = numGC;

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConventionnel;
            }
        }


        public CONVENTIONNEL InsertConventionnel(int idBL, string numGC, string descGC, string typeMGC, string barCode, int poidsMGC, double volMGC, double longMGC, double largMGC, double hautMGC, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Véhicule : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un nouveau véhicule. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement inexistant");
                }

                CONVENTIONNEL conv = new CONVENTIONNEL();

                conv.ESCALE = matchedConnaissement.ESCALE;
                conv.MANIFESTE = matchedConnaissement.MANIFESTE;
                conv.CONNAISSEMENT = matchedConnaissement;
                conv.NumGC = numGC;
                conv.DescGC = descGC;
                conv.TypeMGC = typeMGC;
                conv.TypeCGC = typeMGC;
                conv.PoidsMGC = poidsMGC;
                conv.PoidsCGC = poidsMGC;
                conv.VolMGC = volMGC;
                conv.VolCGC = volMGC;
                conv.LongMGC = (float)longMGC;
                conv.LongCGC = (float)longMGC;
                conv.LargMGC = (float)largMGC;
                conv.LargCGC = (float)largMGC;
                conv.HautMGC = (float)hautMGC;
                conv.HautCGC = (float)hautMGC;
                conv.NumItem = 1;
                conv.InfoMan = "";
                conv.BarCode = barCode;
                conv.DCGC = DateTime.Now;
                conv.StatGC = "Non initié";

                if (matchedConnaissement.DVBL.HasValue)
                {
                    conv.StatGC = "Manifesté";
                }

                conv.SensGC = "I";

                if (conv.ESCALE.DRAEsc.HasValue)
                {
                    conv.FFGC = conv.ESCALE.DRAEsc.Value.AddDays(11);
                    conv.FSGC = conv.ESCALE.DRAEsc.Value.AddDays(11);
                }

                dcMar.GetTable<CONVENTIONNEL>().InsertOnSubmit(conv);

                dcMar.SubmitChanges();
                transaction.Complete();
                return conv;
            }
        }

        public CONVENTIONNEL UpdateConventionnel(int idGC, string numGC, string descGC, string typeMGC, double poidsMGC, double volMGC, double longMGC, double largMGC, double hautMGC, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConv = (from veh in dcMar.GetTable<CONVENTIONNEL>()
                                   where veh.IdGC == idGC
                                   select veh).SingleOrDefault<CONVENTIONNEL>();

                if (matchedConv == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Modification des informations de base").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un general cargo. Veuillez contacter un administrateur");
                }

                matchedConv.NumGC = numGC;
                matchedConv.DescGC = descGC;
                //matchedVehicule.TypeMVeh = typeMVeh;
                matchedConv.PoidsMGC = poidsMGC;
                matchedConv.VolMGC = volMGC;
                matchedConv.LargMGC = (float)longMGC;
                matchedConv.LargMGC = (float)largMGC;
                matchedConv.HautMGC = (float)hautMGC;

                if (matchedConv.StatGC == "Initié" && matchedConv.MANIFESTE.DVMan.HasValue)
                {
                    matchedConv.StatGC = "Traité";
                    if (matchedConv.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC != "Traité" && conv.IdGC != idGC) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedConv.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConv;
            }
        }

        public CONVENTIONNEL UpdateConventionnel(int idGC, string typeCGC, double poidsCGC, double volCGC, double longCGC, double largCGC, double hautCGC, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConventionnel = (from conv in dcMar.GetTable<CONVENTIONNEL>()
                                            where conv.IdGC == idGC
                                            select conv).FirstOrDefault<CONVENTIONNEL>();

                if (matchedConventionnel == null)
                {
                    throw new EnregistrementInexistant("Conventionnel inexistant");
                }

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "General cargo : Modification des informations de base").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un general cargo. Veuillez contacter un administrateur");
                }

                matchedConventionnel.TypeCGC = typeCGC;
                matchedConventionnel.PoidsCGC = poidsCGC;
                matchedConventionnel.VolCGC = volCGC;
                matchedConventionnel.LongCGC = (float)longCGC;
                matchedConventionnel.LargCGC = (float)largCGC;
                matchedConventionnel.HautCGC = (float)hautCGC;

                if (matchedConventionnel.StatGC == "Initié" && matchedConventionnel.MANIFESTE.DVMan.HasValue)
                {
                    matchedConventionnel.StatGC = "Traité";
                    if (matchedConventionnel.CONNAISSEMENT.CONVENTIONNEL.Count(conv => conv.StatGC != "Traité" && conv.IdGC != idGC) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedConventionnel.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedConventionnel;
            }
        }

        #endregion

        #region mafi


        public MAFI UpdateInfosLivraisonMafi(int idMafi, string nomEnleveur, string cniEnleveur, string telEnleveur, string numAttDedouanement, string numCivio, string numDeclDouane, string numQuittDouane, string numFactPAD, string numQuittPAD, string numBAE, string numBESC, string numSydonia, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMafi = (from ctr in dcMar.GetTable<MAFI>()
                                   where ctr.IdMafi == idMafi
                                   select ctr).SingleOrDefault<MAFI>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Mafi : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations de livraison. Veuillez contacter un administrateur");
                }

                if (matchedMafi == null)
                {
                    throw new EnregistrementInexistant("Mafi inexistant");
                }

                //if (matchedConteneur.FSCtr < DateTime.Now.Date)
                //{
                //    throw new ApplicationException("La date à laquelle vous avez calculé le séjour a été dépassée, veuillez recalculer le séjour de ce conteneur");
                //}

                matchedMafi.NomEnMafi = nomEnleveur;
                matchedMafi.CNIEnMafi = cniEnleveur;
                matchedMafi.TelenMafi = telEnleveur;
                matchedMafi.NumADDMafi = numAttDedouanement;
                matchedMafi.NumAVIMafi = numCivio;
                matchedMafi.NumDDMafi = numDeclDouane;
                matchedMafi.NumQDMafi = numQuittDouane;
                matchedMafi.NumFPADMafi = numFactPAD;
                matchedMafi.NumQPADMafi = numQuittPAD;
                matchedMafi.NumAEPADMafi = numBAE;
                matchedMafi.NumBESCMafi = numBESC;
                matchedMafi.NumSydoniaMafi = numSydonia;

                //matchedConteneur.StatCtr = "Livraison";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMafi;
            }
        }

        public MAFI UpdateInfosEnlevementMafi(int idMafi, DateTime sortiePrev, string numBESC, string nomEnleveur, string cniEnleveur, string telEnleveur, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMafi = (from mf in dcMar.GetTable<MAFI>()
                                   where mf.IdMafi == idMafi
                                   select mf).SingleOrDefault<MAFI>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Mafi : Modification des informations sur les opérations liées").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour les informations d'enlèvement. Veuillez contacter un administrateur");
                }

                if (matchedMafi == null)
                {
                    throw new EnregistrementInexistant("Mafi inexistant");
                }

                if (matchedMafi.FSMafi < sortiePrev.Date)
                {
                    throw new ApplicationException("La date de sortie ne peut pas dépasser la date de fin de séjour, recalculer le séjour de ce mafi");
                }

                matchedMafi.DSPMafi = sortiePrev;
                matchedMafi.NomEnMafi = nomEnleveur;
                matchedMafi.CNIEnMafi = cniEnleveur;
                matchedMafi.TelenMafi = telEnleveur;
                matchedMafi.NumBESCMafi = numBESC;
                //matchedConteneur.StatCtr = "Enlèvement";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMafi;
            }
        }

        public MAFI InsertMafi(int idBL, string numMafi, string descMafi, string descMsesMafi, string imdgCode, string typeMMafi, int poidsMMafi, double volMafi, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedConnaissement = (from bl in dcMar.GetTable<CONNAISSEMENT>()
                                            where bl.IdBL == idBL
                                            select bl).SingleOrDefault<CONNAISSEMENT>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                if (matchedUser != null && GetOperationsUtilisateurMar(idUser).Where(op => op.NomOp == "Conteneur : Enregistrement d'un nouvel élément").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour créer un nouveau conteneur. Veuillez contacter un administrateur");
                }

                if (matchedConnaissement == null)
                {
                    throw new EnregistrementInexistant("Connaissement inexistant");
                }

                MAFI mafi = new MAFI();

                mafi.ESCALE = matchedConnaissement.ESCALE;
                mafi.MANIFESTE = matchedConnaissement.MANIFESTE;
                mafi.CONNAISSEMENT = matchedConnaissement;
                mafi.NumMafi = numMafi;
                mafi.DescMafi = descMafi;
                mafi.DescMses = descMsesMafi;
                mafi.TypeMMafi = typeMMafi;
                mafi.TypeCMafi = typeMMafi;
                mafi.VolMMafi = 0;
                mafi.PoidsMMafi = poidsMMafi;
                mafi.PoidsCMafi = poidsMMafi;
                mafi.VolMMafi = (float)volMafi;
                mafi.NumItem = 1;
                mafi.InfoMan = "";
                mafi.IMDGCode = imdgCode;
                mafi.DCMafi = DateTime.Now;
                mafi.StatMafi = "Non initié";
                mafi.SensMafi = "I";

                if (matchedConnaissement.DVBL.HasValue)
                {
                    mafi.StatMafi = "Manifesté";
                }

                dcMar.GetTable<MAFI>().InsertOnSubmit(mafi);

                dcMar.SubmitChanges();
                transaction.Complete();
                return mafi;
            }
        }

        public MAFI UpdateMafi(int idMafi, string numMafi, string description, string imdgCode, string descMses, string typeMMafi, string typeCMafi, int poidsCMafi, double volMafi, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMafi = (from mf in dcMar.GetTable<MAFI>()
                                   where mf.IdMafi == idMafi
                                   select mf).FirstOrDefault<MAFI>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Mafi : Modification des informations sur un élément existant").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour mettre à jour un mafi. Veuillez contacter un administrateur");
                }

                if (matchedMafi == null)
                {
                    throw new EnregistrementInexistant("Mafi inexistant");
                }

                matchedMafi.NumMafi = numMafi;
                matchedMafi.DescMafi = description;
                matchedMafi.IMDGCode = imdgCode;
                matchedMafi.DescMses = descMses;
                matchedMafi.TypeMMafi = typeMMafi;
                matchedMafi.TypeCMafi = typeCMafi;
                matchedMafi.PoidsCMafi = poidsCMafi;
                matchedMafi.VolMMafi = (float)volMafi;

                if (matchedMafi.StatMafi == "Initié" && matchedMafi.MANIFESTE.DVMan.HasValue)
                {
                    matchedMafi.StatMafi = "Traité";
                    if (matchedMafi.CONNAISSEMENT.MAFI.Count(mf => mf.StatMafi != "Traité" && mf.IdMafi != idMafi) == 0)
                    {
                        OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedMafi.IdBL && op.IdTypeOp == 34
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                        matchedOpBL.DateOp = DateTime.Now;
                        matchedOpBL.IdU = idUser;
                        matchedOpBL.AIOp = "Traité";

                        dcMar.OPERATION_CONNAISSEMENT.Context.SubmitChanges();
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMafi;
            }
        }

        public MAFI IdentifierMafi(int idMafi, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMafi = (from ctr in dcMar.GetTable<MAFI>()
                                   where ctr.IdMafi == idMafi
                                   select ctr).FirstOrDefault<MAFI>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Mafi : Modification des informations d'identification").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour identifier un mafi. Veuillez contacter un administrateur");
                }

                if (matchedMafi == null)
                {
                    throw new EnregistrementInexistant("Mafi inexistant");
                }

                //if (!matchedConteneur.ESCALE.DRAEsc.HasValue)
                //{
                //    throw new IdentificationException("Echec de l'identification : Le navire n'est pas encore arrivé");
                //}

                if (!matchedMafi.MANIFESTE.DVMan.HasValue)
                {
                    throw new IdentificationException("Echec de l'identification : Manifeste non validé");
                }

                var opIdentification = (from op in dcMar.GetTable<OPERATION_MAFI>()
                                        where op.IdTypeOp == 226 && op.IdMafi == idMafi
                                        select op).FirstOrDefault<OPERATION_MAFI>();

                if (opIdentification != null)
                {
                    throw new IdentificationException("Ce mafi a déjà été identifié");
                }

                matchedMafi.StatMafi = "Déchargé";

                OPERATION_MAFI opMafi = new OPERATION_MAFI();
                opMafi.IdMafi = idMafi;
                opMafi.IdTypeOp = 226;
                opMafi.DateOp = DateTime.Now;
                opMafi.IdU = idUser;
                opMafi.AIOp = observations;

                dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);

                dcMar.SubmitChanges();

                var listMafiIdem = (from mf in dcMar.GetTable<MAFI>()
                                    where mf.NumMafi == matchedMafi.NumMafi && mf.IdMafi != idMafi && mf.IdMan == matchedMafi.IdMan
                                    select mf).ToList<MAFI>();

                foreach (MAFI mafi in listMafiIdem)
                {
                    var opIdentIdem = (from op in dcMar.GetTable<OPERATION_MAFI>()
                                       where op.IdTypeOp == 226 && op.IdMafi == mafi.IdMafi
                                       select op).FirstOrDefault<OPERATION_MAFI>();

                    if (opIdentIdem != null)
                    {
                        throw new IdentificationException("Ce mafi a déjà été identifié");
                    }

                    mafi.StatMafi = "Déchargé";

                    OPERATION_MAFI opMafiIdem = new OPERATION_MAFI();
                    opMafiIdem.IdMafi = mafi.IdMafi;
                    opMafiIdem.IdTypeOp = 226;
                    opMafiIdem.DateOp = DateTime.Now;
                    opMafiIdem.IdU = idUser;
                    opMafiIdem.AIOp = observations;

                    dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafiIdem);

                    dcMar.SubmitChanges();
                }

                OPERATION_CONNAISSEMENT matchedOpBL = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                       where op.IdBL == matchedMafi.IdBL && op.IdTypeOp == 37
                                                       select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                if (!matchedOpBL.DateOp.HasValue)
                {
                    matchedOpBL.DateOp = DateTime.Now;
                    matchedOpBL.IdU = idUser;
                    matchedOpBL.AIOp = "Déchargement démarré";
                }

                if (matchedMafi.CONNAISSEMENT.MAFI.Count(mf => mf.OPERATION_MAFI.Count(op => op.IdTypeOp == 226) != 0) == matchedMafi.CONNAISSEMENT.MAFI.Count)
                {
                    OPERATION_CONNAISSEMENT matchedOpBLTerm = (from op in dcMar.GetTable<OPERATION_CONNAISSEMENT>()
                                                               where op.IdBL == matchedMafi.IdBL && op.IdTypeOp == 38
                                                               select op).SingleOrDefault<OPERATION_CONNAISSEMENT>();

                    if (!matchedOpBLTerm.DateOp.HasValue)
                    {
                        matchedOpBLTerm.DateOp = DateTime.Now;
                        matchedOpBLTerm.IdU = idUser;
                        matchedOpBLTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_MANIFESTE matchedOpMan = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                    where op.IdMan == matchedMafi.IdMan && op.IdTypeOp == 45
                                                    select op).SingleOrDefault<OPERATION_MANIFESTE>();

                if (!matchedOpMan.DateOp.HasValue)
                {
                    matchedOpMan.DateOp = DateTime.Now;
                    matchedOpMan.IdU = idUser;
                    matchedOpMan.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedMafi.MANIFESTE.MAFI.Count(mf => mf.OPERATION_MAFI.Count(op => op.IdTypeOp == 226) != 0) == matchedMafi.MANIFESTE.MAFI.Count)
                {
                    OPERATION_MANIFESTE matchedOpManTerm = (from op in dcMar.GetTable<OPERATION_MANIFESTE>()
                                                            where op.IdMan == matchedMafi.IdMan && op.IdTypeOp == 46
                                                            select op).SingleOrDefault<OPERATION_MANIFESTE>();

                    if (!matchedOpManTerm.DateOp.HasValue)
                    {
                        matchedOpManTerm.DateOp = DateTime.Now;
                        matchedOpManTerm.IdU = idUser;
                        matchedOpManTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();

                OPERATION_ESCALE matchedOpEsc = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                 where op.IdEsc == matchedMafi.IdEsc && op.IdTypeOp == 53
                                                 select op).SingleOrDefault<OPERATION_ESCALE>();

                if (!matchedOpEsc.DateOp.HasValue)
                {
                    matchedOpEsc.DateOp = DateTime.Now;
                    matchedOpEsc.IdU = idUser;
                    matchedOpEsc.AIOp = "Déchargement démarré";
                }

                dcMar.SubmitChanges();

                if (matchedMafi.ESCALE.MAFI.Count(mf => mf.OPERATION_MAFI.Count(op => op.IdTypeOp == 226) != 0) == matchedMafi.ESCALE.MAFI.Count)
                {
                    OPERATION_ESCALE matchedOpEscTerm = (from op in dcMar.GetTable<OPERATION_ESCALE>()
                                                         where op.IdEsc == matchedMafi.IdEsc && op.IdTypeOp == 54
                                                         select op).SingleOrDefault<OPERATION_ESCALE>();

                    if (!matchedOpEscTerm.DateOp.HasValue)
                    {
                        matchedOpEscTerm.DateOp = DateTime.Now;
                        matchedOpEscTerm.IdU = idUser;
                        matchedOpEscTerm.AIOp = "Déchargement terminé";
                    }
                }

                dcMar.SubmitChanges();


                if (matchedMafi.TypeCMafi.Substring(0, 2) == "20")
                {
                    var matchedOpArm101 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                           where opArm.IdEsc == matchedMafi.IdEsc && opArm.IdTypeOp == 101
                                           select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArm101 != null)
                    {
                        if (matchedOpArm101.QTE.HasValue)
                        {
                            matchedOpArm101.QTE = matchedOpArm101.QTE + 1;
                        }
                        else
                        {
                            matchedOpArm101.QTE = 1;
                        }

                        if (matchedOpArm101.Poids.HasValue)
                        {
                            matchedOpArm101.Poids = matchedOpArm101.Poids + matchedMafi.PoidsCMafi / 1000;
                        }
                        else
                        {
                            matchedOpArm101.Poids = matchedMafi.PoidsCMafi / 1000;
                        }

                        if (matchedOpArm101.Volume.HasValue)
                        {
                            matchedOpArm101.Volume = matchedOpArm101.Volume + Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                        else
                        {
                            matchedOpArm101.Volume = Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                    }

                    var matchedOpArm117 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                           where opArm.IdEsc == matchedMafi.IdEsc && opArm.IdTypeOp == 117
                                           select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArm117 != null)
                    {
                        if (matchedOpArm117.QTE.HasValue)
                        {
                            matchedOpArm117.QTE = matchedOpArm117.QTE + 1;
                        }
                        else
                        {
                            matchedOpArm117.QTE = 1;
                        }

                        if (matchedOpArm117.Poids.HasValue)
                        {
                            matchedOpArm117.Poids = matchedOpArm117.Poids + matchedMafi.PoidsCMafi / 1000;
                        }
                        else
                        {
                            matchedOpArm117.Poids = matchedMafi.PoidsCMafi / 1000;
                        }

                        if (matchedOpArm117.Volume.HasValue)
                        {
                            matchedOpArm117.Volume = matchedOpArm117.Volume + Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                        else
                        {
                            matchedOpArm117.Volume = Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                    }
                }
                else if (matchedMafi.TypeCMafi.Substring(0, 2) == "40")
                {
                    var matchedOpArm102 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                           where opArm.IdEsc == matchedMafi.IdEsc && opArm.IdTypeOp == 102
                                           select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArm102 != null)
                    {
                        if (matchedOpArm102.QTE.HasValue)
                        {
                            matchedOpArm102.QTE = matchedOpArm102.QTE + 1;
                        }
                        else
                        {
                            matchedOpArm102.QTE = 1;
                        }

                        if (matchedOpArm102.Poids.HasValue)
                        {
                            matchedOpArm102.Poids = matchedOpArm102.Poids + matchedMafi.PoidsCMafi / 1000;
                        }
                        else
                        {
                            matchedOpArm102.Poids = matchedMafi.PoidsCMafi / 1000;
                        }

                        if (matchedOpArm102.Volume.HasValue)
                        {
                            matchedOpArm102.Volume = matchedOpArm102.Volume + Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                        else
                        {
                            matchedOpArm102.Volume = Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                    }


                    var matchedOpArm118 = (from opArm in dcMar.GetTable<OPERATION_ARMATEUR>()
                                           where opArm.IdEsc == matchedMafi.IdEsc && opArm.IdTypeOp == 118
                                           select opArm).FirstOrDefault<OPERATION_ARMATEUR>();

                    if (matchedOpArm118 != null)
                    {
                        if (matchedOpArm118.QTE.HasValue)
                        {
                            matchedOpArm118.QTE = matchedOpArm118.QTE + 1;
                        }
                        else
                        {
                            matchedOpArm118.QTE = 1;
                        }

                        if (matchedOpArm118.Poids.HasValue)
                        {
                            matchedOpArm118.Poids = matchedOpArm118.Poids + matchedMafi.PoidsCMafi / 1000;
                        }
                        else
                        {
                            matchedOpArm118.Poids = matchedMafi.PoidsCMafi / 1000;
                        }

                        if (matchedOpArm118.Volume.HasValue)
                        {
                            matchedOpArm118.Volume = matchedOpArm118.Volume + Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                        else
                        {
                            matchedOpArm118.Volume = Convert.ToInt32(matchedMafi.VolMMafi);
                        }
                    }
                }

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMafi;
            }
        }
        
        public MAFI SortirMafi(int idMafi, DateTime dateSortie, string observations, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedMafi = (from mf in dcMar.GetTable<MAFI>()
                                   where mf.IdMafi == idMafi
                                   select mf).FirstOrDefault<MAFI>();

                var matchedUser = (from u in dcMar.GetTable<UTILISATEUR>()
                                   where u.IdU == idUser
                                   select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("Utilisateur inexistant");
                }

                List<OPERATION> operationsUser = GetOperationsUtilisateurMar(idUser);

                if (matchedUser != null && operationsUser.Where(op => op.NomOp == "Mafi : Enregistrement de l'opération de sortie").FirstOrDefault<OPERATION>() == null && matchedUser.LU != "Admin")
                {
                    throw new HabilitationException("Vous n'avez pas les droits nécessaires pour enregistrer une opération de sortie sur un mafi. Veuillez contacter un administrateur");
                }

                if (matchedMafi == null)
                {
                    throw new EnregistrementInexistant("Mafi inexistant");
                }

                var matchedDemandeLivraison = (from dbl in dcMar.GetTable<DEMANDE_LIVRAISON>()
                                               where dbl.IdDBL == matchedMafi.IdDBL
                                               select dbl).FirstOrDefault<DEMANDE_LIVRAISON>();

                if (matchedDemandeLivraison != null)
                {
                    if (!matchedDemandeLivraison.DVDBL.HasValue)
                    {
                        throw new ApplicationException("La demande de livraison de ce mafi n'a pas été validée");
                    }
                }
                else
                {
                    throw new ApplicationException("Ce mafi ne fait l'objet d'aucun bon de livrason");
                }

                if (!matchedMafi.IdBS.HasValue)
                {
                    throw new ApplicationException("Sortie impossible : Ce mafi ne fait l'objet d'aucun bon de sortie");
                }

                // inserer opération de sortie
                OPERATION_MAFI opMafi = new OPERATION_MAFI();
                opMafi.IdMafi = idMafi;
                opMafi.IdTypeOp = 232;
                opMafi.DateOp = DateTime.Now;
                opMafi.IdU = idUser;
                opMafi.AIOp = observations;

                dcMar.GetTable<OPERATION_MAFI>().InsertOnSubmit(opMafi);

                matchedMafi.DSMafi = DateTime.Now;
                matchedMafi.StatMafi = "Sorti";

                dcMar.SubmitChanges();
                transaction.Complete();
                return matchedMafi;
            }
        }

        #endregion

        #region formatter

        private static string FormatReferenceMAD(int entier)
        {
            Int32 i = entier;
            if (i >= 10000000)
            {
                return i.ToString();
            }
            else if (i >= 1000000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100000)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10000)
            {
                return "000" + i.ToString();
            }
            else if (i >= 1000)
            {
                return "0000" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00000" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000000" + i.ToString();
            }
            else
            {
                return "0000000" + i.ToString();
            }
        }

        private static string FormatRefGC(int entier)
        {
            Int32 i = entier;
            if (i >= 10000000)
            {
                return i.ToString();
            }
            else if (i >= 1000000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100000)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10000)
            {
                return "000" + i.ToString();
            }
            else if (i >= 1000)
            {
                return "0000" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00000" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000000" + i.ToString();
            }
            else
            {
                return "0000000" + i.ToString();
            }
        }

        public static string FormatReferenceBooking(int entier)
        {
            Int32 i = entier;
            if (i >= 10000000)
            {
                return i.ToString();
            }
            else if (i >= 1000000)
            {
                return "0" + i.ToString();
            }
            else if (i >= 100000)
            {
                return "00" + i.ToString();
            }
            else if (i >= 10000)
            {
                return "000" + i.ToString();
            }
            else if (i >= 1000)
            {
                return "0000" + i.ToString();
            }
            else if (i >= 100)
            {
                return "00000" + i.ToString();
            }
            else if (i >= 10)
            {
                return "000000" + i.ToString();
            }
            else
            {
                return "0000000" + i.ToString();
            }
        }

        #endregion

        public List<OPERATION> GetOperationsUtilisateurMar(int idUser)
        {
            return (from op in dcMar.GetTable<OPERATION>()
                    from dr in dcMar.GetTable<DROIT>()
                    where dr.IdOp == op.IdOp && dr.IdU == idUser
                    select op).ToList<OPERATION>();
        }

        public List<CLIENT_M_PAY> LoadClientMPAY(int idclient)
        {
            List<CLIENT_M_PAY> lst = (from m in dcMar.GetTable<CLIENT_M_PAY>() where m.IDCLIENT == idclient select m).ToList<CLIENT_M_PAY>();
            return lst;
        }

        public ClientCondFin LoadClientFinancialCondition(int idclient)
        {
            ClientCondFin ccf = new ClientCondFin();
            List<CLIENT_FIN_COND> matchedcli = (from m in dcMar.GetTable<CLIENT_FIN_COND>() where m.IDCLIENT ==idclient select m).ToList<CLIENT_FIN_COND>();
            if ( matchedcli.Count == 2)
            {
                #region import
                CLIENT_FIN_COND cfc_imp = matchedcli.Single(s => s.SENS == "I");
                ccf.caution_montant_import = cfc_imp.CAUTION_MT.Value;
                ccf.Caution_type_import = cfc_imp.CAUTION_T;

                ccf.Conv_manut_import = cfc_imp.CONV_MANUT.Value ;
                ccf.Conv_od_import = cfc_imp.CONV_OP_DIVERS.Value;
                ccf.Conv_sejour_import = cfc_imp.CONV_SEJOUR.Value;

                ccf.Detention_import = cfc_imp.CTR_DET.Value;
                ccf.fret_import = cfc_imp.CTR_FRET.Value;
                ccf.Operation_divers_import = cfc_imp.CTR_OP_DIVERS.Value;
                ccf.Reparation_import = cfc_imp.CTR_REP.Value;
                ccf.Surestarie_import = cfc_imp.CTR_SUR.Value;
                ccf.Valeur_residuel_import = cfc_imp.CTR_VAL_RSDL.Value;
                ccf.Frais_Dossier_import = cfc_imp.DOSSIER.Value;

                ccf.Veh_fret_import = cfc_imp.VEH_FRET.Value;
                ccf.Veh_manut_import = cfc_imp.VEH_MANUT.Value;
                ccf.Veh_od_import = cfc_imp.VEH_OP_DIVERS.Value;
                ccf.Veh_pg_import = cfc_imp.VEH_PG.Value;
                ccf.Veh_sejour_import = cfc_imp.VEH_SEJOUR.Value;

                ccf.Caution_type_import = cfc_imp.CAUTION_T;
                ccf.Plafond_import = cfc_imp.PLAFOND.Value;
                #endregion


                #region export
                CLIENT_FIN_COND cfc_exp = matchedcli.Single(s => s.SENS == "E");
                ccf.Caution_montant_export = cfc_exp.CAUTION_MT.Value;
                  //ccf.Caution_type_import = cfc_exp.CAUTION_T;
                  ccf.Conv_manut_export = cfc_exp.CONV_MANUT.Value;
                  ccf.Conv_od_export = cfc_exp.CONV_OP_DIVERS.Value;
                  ccf.Conv_sejour_export = cfc_exp.CONV_SEJOUR.Value;

                  ccf.Detention_export = cfc_exp.CTR_DET.Value;
                  ccf.fret_export = cfc_exp.CTR_FRET.Value;
                  ccf.Operation_divers_export = cfc_exp.CTR_OP_DIVERS.Value;
                  ccf.Reparation_export = cfc_exp.CTR_REP.Value ;
                 ccf.Surestarie_export=cfc_exp.CTR_SUR.Value ;
                ccf.Valeur_residuel_export=cfc_exp.CTR_VAL_RSDL.Value ;
                 ccf.Frais_Dossier_export=cfc_exp.DOSSIER.Value ;

                ccf.Veh_fret_export=cfc_exp.VEH_FRET.Value ;
               ccf.Veh_manut_export=  cfc_exp.VEH_MANUT.Value ;
               ccf.Veh_od_export=  cfc_exp.VEH_OP_DIVERS.Value ;
               ccf.Veh_pg_export=  cfc_exp.VEH_PG.Value ;
              ccf.Veh_sejour_export=   cfc_exp.VEH_SEJOUR.Value ;

              ccf.Plafond_export = cfc_exp.PLAFOND.Value;
              ccf.Covvd = cfc_exp.COVVD.Value;
                #endregion
            }

            return ccf;
        }
        public void InsertClientFinancialCondition(CLIENT_FIN_COND imp, CLIENT_FIN_COND exp, List<CLIENT_M_PAY> cmp_imp, List<CLIENT_M_PAY> cmp_exp)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                //recherche si le clent en question est deja configurer.
                List<CLIENT_FIN_COND> matchedcli = (from m in dcMar.GetTable<CLIENT_FIN_COND>() where m.IDCLIENT == imp.IDCLIENT select m).ToList<CLIENT_FIN_COND>();
                //les deux sens sont configurer imp et exp
                if (matchedcli != null && matchedcli.Count == 2)
                { 
                  //mettre a jour les elements
                    CLIENT_FIN_COND _imp = matchedcli.Single(s => s.SENS == "I");
                    _imp.CAUTION_M_PAY = imp.CAUTION_M_PAY;
                    _imp.CAUTION_MT = imp.CAUTION_MT;
                    _imp.CAUTION_T = imp.CAUTION_T;
                    _imp.CONV_MANUT = imp.CONV_MANUT;
                    _imp.CONV_OP_DIVERS = imp.CONV_OP_DIVERS;
                    _imp.CONV_SEJOUR = imp.CONV_SEJOUR;
                    _imp.COVVD = imp.COVVD;
                    _imp.CTR_DET = imp.CTR_DET;
                    _imp.CTR_FRET = imp.CTR_FRET;
                    _imp.CTR_M_PAY = imp.CTR_M_PAY;
                    _imp.CTR_OP_DIVERS = imp.CTR_OP_DIVERS;
                    _imp.CTR_REP = imp.CTR_REP;
                    _imp.CTR_SUR = imp.CTR_SUR;
                    _imp.CTR_VAL_RSDL = imp.CTR_VAL_RSDL;
                    _imp.DOSSIER = imp.DOSSIER;
                    _imp.VEH_FRET = imp.VEH_FRET;
                    _imp.VEH_MANUT = imp.VEH_MANUT;
                    _imp.VEH_OP_DIVERS = imp.VEH_OP_DIVERS;
                    _imp.VEH_PG = imp.VEH_PG;
                    _imp.VEH_SEJOUR = imp.VEH_SEJOUR;
                    _imp.PLAFOND = imp.PLAFOND;

                    CLIENT_FIN_COND _exp = matchedcli.Single(s => s.SENS == "E");
                    _exp.CAUTION_M_PAY = exp.CAUTION_M_PAY;
                    _exp.CAUTION_MT = exp.CAUTION_MT;
                    _exp.CAUTION_T = exp.CAUTION_T;
                    _exp.CONV_MANUT = exp.CONV_MANUT;
                    _exp.CONV_OP_DIVERS = exp.CONV_OP_DIVERS;
                    _exp.CONV_SEJOUR = exp.CONV_SEJOUR;
                    _exp.COVVD = exp.COVVD;
                    _exp.CTR_DET = exp.CTR_DET;
                    _exp.CTR_FRET = exp.CTR_FRET;
                    _exp.CTR_M_PAY = exp.CTR_M_PAY;
                    _exp.CTR_OP_DIVERS = exp.CTR_OP_DIVERS;
                    _exp.CTR_REP = exp.CTR_REP;
                    _exp.CTR_SUR = exp.CTR_SUR;
                    _exp.CTR_VAL_RSDL = exp.CTR_VAL_RSDL;
                    _exp.DOSSIER = exp.DOSSIER;
                    _exp.VEH_FRET = exp.VEH_FRET;
                    _exp.VEH_MANUT = exp.VEH_MANUT;
                    _exp.VEH_OP_DIVERS = exp.VEH_OP_DIVERS;
                    _exp.VEH_PG = exp.VEH_PG;
                    _exp.VEH_SEJOUR = exp.VEH_SEJOUR;
                    _exp.PLAFOND = exp.PLAFOND;
                    List<CLIENT_M_PAY> _del = (from m in dcMar.GetTable<CLIENT_M_PAY>() where m.IDCLIENT == imp.IDCLIENT select m).ToList<CLIENT_M_PAY>();
                    dcMar.CLIENT_M_PAY.DeleteAllOnSubmit(_del);
                    dcMar.SubmitChanges();

                    dcMar.CLIENT_M_PAY.InsertAllOnSubmit(cmp_imp);
                    dcMar.CLIENT_M_PAY.InsertAllOnSubmit(cmp_exp);
                    dcMar.SubmitChanges();
                    
                }
                if (matchedcli == null || matchedcli.Count==0 )
                {
                    //imp.DATE_ENREG = DateTime.Now;
                    //exp.DATE_ENREG = DateTime.Now;
                    dcMar.CLIENT_FIN_COND.InsertOnSubmit(imp);
                    dcMar.CLIENT_FIN_COND.InsertOnSubmit(exp);
                    dcMar.SubmitChanges();
                    dcMar.CLIENT_M_PAY.InsertAllOnSubmit(cmp_imp);
                    dcMar.CLIENT_M_PAY.InsertAllOnSubmit(cmp_exp);
                    dcMar.SubmitChanges();
                    
                }
                if (matchedcli != null && matchedcli.Count != 2 && matchedcli.Count!=0)
                {
                    throw new ApplicationException("La configuration du client n'est pas conforme. Veuillez contacter l'administrateur");
                }
                transaction.Complete();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represent les methode d'envoie de données de NOVA a SAP.
    /// </summary>
    public class VsomSAP
    {
         private string sap_svr = string.Empty; // "192.168.0.30";
        private string sap_licence =string.Empty; //"192.168.0.30";
        private string sap_db_name = string.Empty; //"SOCOMAR_PRODUCTION";
        private string sap_db_usr = string.Empty;
        private string sap_db_pwd = string.Empty;

        public VsomSAP()
        {
           string cnstr = VesselStopOverData.Properties.Settings.Default.SOCOMAR_PRODUCTIONConnectionString;
            //TODO: recuperer les valeur du connexion string
            System.Data.SqlClient.SqlConnectionStringBuilder scb = new System.Data.SqlClient.SqlConnectionStringBuilder(cnstr);
            sap_svr = scb.DataSource;
            sap_licence = scb.DataSource;
            sap_db_name = scb.InitialCatalog;
            sap_db_usr = scb.UserID;
            sap_db_pwd = scb.Password;
        }

        public string PaiementProforma(Dictionary<string,int> lstfact,short modePay,string codeclient,string date, string cashAccount, int mrpay,
            string banque, string agence, string numCompte, string numCheque, string mapay, string refVirt)
        {
            return "200001";
            //SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
           
            //List<SocSAPWS.PayementFacture> listFacturesPayement = new List<SocSAPWS.PayementFacture>();
            ////creer la listfacturespayement avec le dictionnaires des factures
            //foreach(var pair in lstfact)
            //{
            //   SocSAPWS.PayementFacture p = new SocSAPWS.PayementFacture();
            //            p.DocEntry =pair.Key; //fact.IdDocSAP.ToString();
            //            p.PaidSum =pair.Value; //fact.MTTC.Value;

            //            listFacturesPayement.Add(p);
            //}
            //string idPaySAP=string.Empty;
            // string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);

            //if (modePay == 1)
            //{
            //    idPaySAP = sapWS.AddPayment(sessionID, codeclient, date, cashAccount, mrpay, date, date, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée. \n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 2)
            //{
            //    idPaySAP = sapWS.AddPaymentCheck(sessionID, codeclient, date, banque, agence, numCompte, mapay, numCheque, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée.\n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 4)//traite
            //{
            //    idPaySAP = sapWS.AddPaymentCheck(sessionID, codeclient, date, banque, agence, numCompte,
            //              mapay, numCheque, listFacturesPayement.ToArray());


            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 3)
            //{
            //    idPaySAP = sapWS.AddPaymentAccount(sessionID, codeclient, date, "5141100"/*ccBanque*/, refVirt, date, mapay, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée.\n " + idPaySAP);
            //    }
            //}

            //#region logique non correct

            ///*
            //* a ne plus considerer car fause ecriture selon yamtheu
            //* if (payement.RetIS == "Y")
            //{
            //    List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = payement.CONNAISSEMENT.CLIENT.CCClient,
            //            ShortName = payement.CONNAISSEMENT.CLIENT.CodeClient,
            //            ControlAccount = payement.CONNAISSEMENT.CLIENT.CCClient,
            //            Credit = listProformas.Sum(p => p.MTTC.Value) - payement.MRPay.Value,
            //            Debit = 0,
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = "4456100",
            //            ShortName = "4456100",
            //            ControlAccount = "4456100",
            //            Credit = 0,
            //            Debit = listProformas.Sum(p => p.MTVA.Value),
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = "4411102",
            //            ShortName = "4411102",
            //            ControlAccount = "4411102",
            //            Credit = 0,
            //            Debit = listProformas.Sum(p => p.MTTC.Value) - listProformas.Sum(p => p.MTVA.Value) - payement.MRPay.Value,
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, "Retenue à la source - Client " + payement.CONNAISSEMENT.CLIENT.CodeClient, "", payement.CONNAISSEMENT.ESCALE.NumEsc.ToString(), "", listLignesJournal.ToArray());

            //    int result;
            //    int idje = Int32.TryParse(idJE, out result) ? result : -1;

            //    if (idje == -1)
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idJE);
            //    }
            //}*/
            
            //#endregion

            //sapWS.LogOut(sessionID);
            //return idPaySAP;
        }

        public string PaiementFacture(Dictionary<string,int> lstfact,short modePay,string codeclient,string date, string cashAccount, int mrpay,
            string banque, string agence, string numCompte, string numCheque, string mapay, string refVirt)
        {
            return "200000";

            //SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            //string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            ////recontruire la liste listefacturepayment
            //List<SocSAPWS.PayementFacture> listFacturesPayement = new List<SocSAPWS.PayementFacture>();
               
            //foreach (var pair in lstfact)
            //{
            //    SocSAPWS.PayementFacture p = new SocSAPWS.PayementFacture();
            //    p.DocEntry = pair.Key; //fact.IdDocSAP.ToString();
            //    p.PaidSum = pair.Value; //fact.MTTC.Value;

            //    listFacturesPayement.Add(p);
            //}

            //string idPaySAP=string.Empty;

            //if (modePay == 1)
            //{
            //     idPaySAP = sapWS.AddPayment(sessionID, codeclient, date, cashAccount,mrpay, date, date, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 2)
            //{
            //     idPaySAP = sapWS.AddPaymentCheck(sessionID, codeclient, date, banque, agence, numCompte, mapay, numCheque, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 4)//traite
            //{
            //     idPaySAP = sapWS.AddPaymentCheck(sessionID, codeclient, date, banque, agence, numCompte,mapay, numCheque, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idPaySAP);
            //    }
            //}
            //else if (modePay == 3)
            //{
            //     idPaySAP = sapWS.AddPaymentAccount(sessionID, codeclient, date, "5141100"/*ccBanque*/, refVirt, date, mapay, listFacturesPayement.ToArray());
            //    if (!idPaySAP.Contains("La facture est déjà clôturée ou bloquée"))
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idPaySAP);
            //    }
            //}

            //#region retenu a la source erroné : fausse ecriture constacté par yamtheu

            ///*
            // * fausse ecriture constacté par yamtheu
            //if (payement.RetIS == "Y")
            //{
            //    List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = payement.CONNAISSEMENT.CLIENT.CCClient,
            //            ShortName = payement.CONNAISSEMENT.CLIENT.CodeClient,
            //            ControlAccount = payement.CONNAISSEMENT.CLIENT.CCClient,
            //            Credit = listFactures.Sum(f => f.MTTC.Value) - payement.MRPay.Value,
            //            Debit = 0,
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = "4456100",
            //            ShortName = "4456100",
            //            ControlAccount = "4456100",
            //            Credit = 0,
            //            Debit = listFactures.Sum(f => f.MTVA.Value),
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    listLignesJournal.Add(
            //        new SocSAPWS.LigneJournal
            //        {
            //            AccountCode = "4411102",
            //            ShortName = "4411102",
            //            ControlAccount = "4411102",
            //            Credit = 0,
            //            Debit = listFactures.Sum(f => f.MTTC.Value) - listFactures.Sum(f => f.MTVA.Value) - payement.MRPay.Value,
            //            DueDate = date,
            //            ReferenceDate1 = date,
            //            TaxDate = date,
            //            VATGroup = ""
            //        }
            //    );

            //    StringBuilder sb = new StringBuilder();
            //    foreach (FACTURE f in listFactures)
            //    {
            //        if (sb.Length != 0)
            //        {
            //            sb.Append("-").Append(f.IdDocSAP.ToString());
            //        }
            //        else
            //        {
            //            sb.Append(f.IdDocSAP.ToString());
            //        }

            //    }

            //    string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, "Retenue à la source - Client " + payement.CONNAISSEMENT.CLIENT.CodeClient, "", payement.CONNAISSEMENT.ESCALE.NumEsc.ToString(), sb.ToString(), listLignesJournal.ToArray());

            //    int result;
            //    int idje = Int32.TryParse(idJE, out result) ? result : -1;

            //    if (idje == -1)
            //    {
            //        throw new ApplicationException("Echec du transfert des données à l'ERP. La transaction a été abandonnée \n " + idJE);
            //    }
            //}*/
            
            //#endregion
           
            //sapWS.LogOut(sessionID);
            //return idPaySAP;
        }

        public string InsertAvoirPartiel(List<LigneEcriture> elts, string codeclient, string date, string taxDate, int idFactSAP,string numEsc, string numBl,
                                         string consignee)
        {

            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneAvoir> lignes = new List<SocSAPWS.LigneAvoir>();

            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneAvoir lg = new SocSAPWS.LigneAvoir();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA;
                lg.Quantity = Math.Abs(ligne.Qte).ToString().Replace(",", ".");
                lignes.Add(lg);
            }
            string idDocSAP = sapWS.AddPartialCreditNote(sessionID, codeclient, date, taxDate, date, numEsc + "-" + numBl, 
                              consignee + " basé sur fact. " + idFactSAP.ToString(), lignes.ToArray());

            sapWS.LogOut(sessionID);

            return idDocSAP;
        }

        public string ValideProforma(List<LigneEcriture> elts,string date,string taxDate,string codeclient, string numesc, string numbl, int idfp,string consignee)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();

            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA;
                lignes.Add(lg);
            }
            string idDocSAP = sapWS.AddInvoice(sessionID, codeclient, date, taxDate, date, numesc + "-" + numbl + "-" + idfp, consignee, lignes.ToArray());

            sapWS.LogOut(sessionID);
            return idDocSAP;
        }

        public string PaiementCaution(string ccclient,string codeclient, double map, string date,string cashAccount, string numCtr,
            string numEsc)
        {

            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);

            List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

            listLignesJournal.Add(
                new SocSAPWS.LigneJournal
                {
                    AccountCode = ccclient,
                    ShortName = codeclient,
                    ControlAccount = "4191101",
                    Credit = 0,
                    Debit = map,
                    DueDate = date,
                    ReferenceDate1 = date,
                    TaxDate = date,
                    VATGroup = ""
                }
            );

            listLignesJournal.Add(
                new SocSAPWS.LigneJournal
                {
                    AccountCode = cashAccount,
                    ShortName = cashAccount,
                    ControlAccount = cashAccount,
                    Credit = map,
                    Debit = 0,
                    DueDate = date,
                    ReferenceDate1 = date,
                    TaxDate = date,
                    VATGroup = ""
                }
            );

            string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, 
                   "Restitution caution Ctr N° " + numCtr, numCtr, numEsc,codeclient, listLignesJournal.ToArray());

            sapWS.LogOut(sessionID);
            return idJE;
        }

        public string EncaissementCaution(string cclient, string codeclient, double mcctr, string date,string cashAccount,
            string numCtr, string numesc)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);

           
            List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

            listLignesJournal.Add(
                new SocSAPWS.LigneJournal
                {
                    AccountCode = cclient,
                    ShortName = codeclient,
                    ControlAccount = "4191101",
                    Credit = mcctr,
                    Debit = 0,
                    DueDate = date,
                    ReferenceDate1 = date,
                    TaxDate = date,
                    VATGroup = ""
                }
            );

            listLignesJournal.Add(
                new SocSAPWS.LigneJournal
                {
                    AccountCode = cashAccount,
                    ShortName = cashAccount,
                    ControlAccount = cashAccount,
                    Credit = 0,
                    Debit = mcctr,
                    DueDate = date,
                    ReferenceDate1 = date,
                    TaxDate = date,
                    VATGroup = ""
                }
            );

            string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, "Encaissement caution Ctr N° " + numCtr, numCtr,  
                numesc,codeclient, listLignesJournal.ToArray());

            sapWS.LogOut(sessionID);

            return idJE;
        }

        public string Facture(List<LigneEcriture> elts, string date, string taxDate, string codeclient, string numesc,string numbl,int idfp,string consignee)
        {
            //SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            //string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            //List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();

            //foreach (LigneEcriture ligne in elts)
            //{
            //    SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
            //    lg.ItemCode = ligne.CodeArticle.ToString();
            //    lg.AccountCode = ligne.AccountCode;
            //    lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
            //    lg.UnitPrice = ligne.PrixUnitaire;
            //    lg.VATGroup = ligne.CodeTVA;
            //    lignes.Add(lg);
            //}
            //string idDocSAP = sapWS.AddInvoice(sessionID, codeclient, date, taxDate, date,numesc + "-" + numbl + "-" + idfp, consignee, lignes.ToArray());

            //sapWS.LogOut(sessionID);
            //return idDocSAP;

            return (idfp+5000).ToString();
        }

        /// <summary>
        /// step 1 de cloruture OS. il faut l'appel du step 2 apres cette methode
        /// </summary>
        /// <param name="elts"></param>
        /// <param name="codefrs"></param>
        /// <param name="date"></param>
        /// <param name="numesc"></param>
        /// <param name="idos"></param>
        /// <param name="libos"></param>
        /// <param name="idpo"></param>
        /// <returns></returns>
        public string ClotureOS_1(List<LigneEcriture> elts, string codefrs,string date,string numesc,string idos, string libos,int idpo )
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneCommandeFournisseur> lignes = new List<SocSAPWS.LigneCommandeFournisseur>();

            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneCommandeFournisseur lg = new SocSAPWS.LigneCommandeFournisseur();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA == "TVAEX" ? "D0" : "D4";
                lignes.Add(lg);
            }
            string idPDN = sapWS.AddPurchaseDeliveryNote(sessionID, codefrs, date, date, date, numesc + "-" + idos,libos, idpo, lignes.ToArray());
            sapWS.LogOut(sessionID);
            return idPDN;
        }

        public string ClotureOS_2(List<LigneEcriture> elts,string codefrs,string date,string numesc, string idos, string numFacture, string libos, int idpn)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneCommandeFournisseur> lignes = new List<SocSAPWS.LigneCommandeFournisseur>();
                foreach (LigneEcriture ligne in elts)
                {
                SocSAPWS.LigneCommandeFournisseur lg = new SocSAPWS.LigneCommandeFournisseur();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA == "TVAEX" ? "D0" : "D4";
                lignes.Add(lg);
                }
            string idPINV = sapWS.AddPurchaseInvoice(sessionID, codefrs, date, date, date,
                                    numesc + "-" + idos + (numFacture.Trim() != "" ? "-" + numFacture.Trim() : ""),
                                   libos,idpn , lignes.ToArray());
            sapWS.LogOut(sessionID);
            return idPINV;

        }

        public string FactureSpot(List<LigneEcriture> elts,string date, string taxDate,string codeclient, string ai)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();


            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA;
                lignes.Add(lg);
            }
            string idDocSAP = sapWS.AddInvoice(sessionID, codeclient, date, taxDate, date,
                 "FactureSpotNOVA-" + codeclient, ai, lignes.ToArray());

            sapWS.LogOut(sessionID);
            return idDocSAP;
        }
        
        public string ValideOSArmateur(List<LigneEcriture> elts, string date, string numEsc, string libod, int idos, string ccarm)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
                
                    List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();

                    foreach (LigneEcriture ligne in elts)
                    {
                        listLignesJournal.Add(
                        new SocSAPWS.LigneJournal
                        {
                            AccountCode = ligne.AccountCode,
                            ShortName = ligne.AccountCode,
                            ControlAccount = ligne.AccountCode,
                            Credit = 0,
                            Debit = Math.Round(ligne.PrixUnitaire * ligne.Qte * ((ligne.CodeTVA == "TVAAP" || ligne.CodeTVA == "TVATI" || ligne.CodeTVA=="TVADA") ? 1.1925f : 1), 0, MidpointRounding.AwayFromZero),
                            DueDate = date,
                            ReferenceDate1 = date,
                            TaxDate = date,
                            VATGroup = ""
                        }
                    );
                    }

                    listLignesJournal.Add(
                        new SocSAPWS.LigneJournal
                        {
                            AccountCode = "4711100",
                            ShortName = "4711100",
                            ControlAccount = "4711100",
                            Credit = elts.Sum(lg => Math.Round(lg.PrixUnitaire * lg.Qte * ((lg.CodeTVA == "TVAAP" || lg.CodeTVA == "TVATI" || lg.CodeTVA == "TVADA") ? 1.1925f : 1), 0, MidpointRounding.AwayFromZero)),
                            Debit = 0,
                            DueDate = date,
                            ReferenceDate1 = date,
                            TaxDate = date,
                            VATGroup = ""
                        }
                    );

                    string idJE = sapWS.AddJournalEntry(sessionID, date, date, date, "Val. OS N° " + idos + " - " + libod,idos.ToString(), 
                                  numEsc, ccarm, listLignesJournal.ToArray());

                    sapWS.LogOut(sessionID);
                    return idJE;
        }

        public string ValideOSEscal(List<LigneEcriture> elts, string codefrs, string date, string numesc,string idos, string libos)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService(); 
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneCommandeFournisseur> lignes = new List<SocSAPWS.LigneCommandeFournisseur>();

            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneCommandeFournisseur lg = new SocSAPWS.LigneCommandeFournisseur();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA == "TVAEX" ? "D0" : "D4";
                lignes.Add(lg);
            }
            string idDocSAP = sapWS.AddPurchaseOrder(sessionID, codefrs, date, date, date, numesc + "-" + idos, libos, lignes.ToArray());

            sapWS.LogOut(sessionID);
            return idDocSAP;
        }

        public int FactureSOCOMAR(string date, string numEsc, List<LigneEcriture> elts)
        {
            List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();
             
                //cree les ligneecriture de agency fees et isps et (export freight)
                foreach (LigneEcriture ligne in elts.Where(l => l.CodeArticle != 2202))
                {
                    SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                    lg.ItemCode = ligne.CodeArticle.ToString();
                    lg.AccountCode = ligne.AccountCode;
                    lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                    lg.UnitPrice = ligne.PrixUnitaire;
                    lg.VATGroup = ligne.CodeTVA;
                    lignes.Add(lg);
                }

                // cree une ligne pour les SOP
                if (elts.Count(e => e.CodeArticle == 2202) != 0)
                {
                    SocSAPWS.LigneFacture lgStevedoring = new SocSAPWS.LigneFacture();
                    lgStevedoring.ItemCode = elts.FirstOrDefault(e => e.CodeArticle == 2202).CodeArticle.ToString();
                    lgStevedoring.AccountCode = elts.FirstOrDefault(e => e.CodeArticle == 2202).AccountCode;
                    lgStevedoring.Quantity = "1";
                    lgStevedoring.UnitPrice = Math.Round(elts.Where(el => el.CodeArticle == 2202).Sum(e => e.PrixUnitaire * e.Qte), 0, MidpointRounding.AwayFromZero);
                    lgStevedoring.VATGroup = "TVAAP";
                    lignes.Add(lgStevedoring);
                }

            //ecrit une ligne de facture ds le C0390
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);

            string idDocSAP = sapWS.AddInvoice(sessionID, "C0390", date, date, date, numEsc,
                                               "Facture socomar - Escale : " + numEsc, lignes.ToArray());

            sapWS.LogOut(sessionID);


            int result;
            int idFSoc = Int32.TryParse(idDocSAP, out result) ? result : -1;


            if (idFSoc == -1)
            {
                throw new ApplicationException("Echec du transfert des données à l'ERP 1 : \n " + idDocSAP.ToString());
            }
            else
            {

                // contre ecriture comptable ecrit au credit de C0390 le montant de la facture et au debit de 4711106 le detail des element de la facture
                sapWS = new SocSAPWS.SocSAPWebService();

                List<SocSAPWS.LigneJournal> listLignesJournal = new List<SocSAPWS.LigneJournal>();
                //ligne de 70611XX a ecrire ds 4711106
                double _debit = 0;
                foreach (LigneEcriture ligne in elts)
                {
                    /* listLignesJournal.Add(
                     new SocSAPWS.LigneJournal
                     {
                         AccountCode = "4711106",
                         ShortName = "4711106",
                         ControlAccount = "4711106",
                         Credit = 0,
                         Debit = Math.Round(ligne.PrixUnitaire * ligne.Qte * (ligne.CodeTVA == "TVAAP" ? 1.1925f : 1), 0, MidpointRounding.AwayFromZero),
                         DueDate = date,
                         ReferenceDate1 = date,
                         TaxDate = date,
                         VATGroup = ""
                     }
                 );*/
                    _debit += Math.Round(ligne.PrixUnitaire * ligne.Qte * ((ligne.CodeTVA == "TVAAP" || ligne.CodeTVA == "TVATI") ? 1.1925f : 1), 0, MidpointRounding.AwayFromZero);
                }

                listLignesJournal.Add(
                  new SocSAPWS.LigneJournal
                  {
                      AccountCode = "4711106",
                      ShortName = "4711106",
                      ControlAccount = "4711106",
                      Credit = 0,
                      Debit = _debit,
                      DueDate = date,
                      ReferenceDate1 = date,
                      TaxDate = date,
                      VATGroup = ""
                  }
              );

                listLignesJournal.Add(
                       new SocSAPWS.LigneJournal
                       {
                           AccountCode = "C0390",
                           ShortName = "C0390",
                           ControlAccount = "4111100",
                           Credit = elts.Sum(lg => Math.Round(lg.PrixUnitaire * lg.Qte * ((lg.CodeTVA == "TVAAP" || lg.CodeTVA == "TVATI") ? 1.1925f : 1), 0, MidpointRounding.AwayFromZero)),
                           Debit = 0,
                           DueDate = date,
                           ReferenceDate1 = date,
                           TaxDate = date,
                           VATGroup = ""
                       }
                   );


                string sessionID2 = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);

                string idJE = sapWS.AddJournalEntry(sessionID2, date, date, date, "Facture socomar  - Escale : " + numEsc, "IDFactSOCOMAR SAP " + idFSoc.ToString(),
                                 numEsc, "C0390", listLignesJournal.ToArray());

                sapWS.LogOut(sessionID2);

                int result2;
                int idFSoc2 = Int32.TryParse(idJE, out result2) ? result2 : -1;
                if (idFSoc2 == -1)
                {
                    throw new ApplicationException(string.Format("Echec du transfert des données à l'ERP 2. code entrée1: {0} , erreur 2 : \n {1}",
                                                                 idDocSAP.ToString(), idJE));
                }

            }

            return idFSoc;
        }

         
        public string InsertAvoirFactureSpot(List<LigneEcriture> elts, string date, string taxDate, int idfacture, string codeclient, string nomclient)
        {
            SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            List<SocSAPWS.LigneAvoir> lignes = new List<SocSAPWS.LigneAvoir>();

            foreach (LigneEcriture ligne in elts)
            {
                SocSAPWS.LigneAvoir lg = new SocSAPWS.LigneAvoir();
                lg.ItemCode = ligne.CodeArticle.ToString();
                lg.AccountCode = ligne.AccountCode;
                lg.UnitPrice = ligne.PrixUnitaire;
                lg.VATGroup = ligne.CodeTVA;
                lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                lignes.Add(lg);
            }

            //AH 13juillet16 control sil faut effectuer une ecriture dans SAP
            string idDocSAP = string.Empty;
            idDocSAP = sapWS.AddCreditNote(sessionID, nomclient, date, taxDate, date,
                      idfacture.ToString() + "-" + codeclient, nomclient,idfacture, lignes.ToArray());

            sapWS.LogOut(sessionID);

            return idDocSAP;
        }

        public string InsertAvoir(List<LigneEcriture> elts,bool writeSAP,string date,string taxDate,string codeClient, string numEsc, 
                      string numBl, string consignee, int idfacture, int idDocAvoirSAP)
        {
            //SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
            //string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
            //List<SocSAPWS.LigneAvoir> lignes = new List<SocSAPWS.LigneAvoir>();

            //foreach (LigneEcriture ligne in elts)
            //{
            //    SocSAPWS.LigneAvoir lg = new SocSAPWS.LigneAvoir();
            //    lg.ItemCode = ligne.CodeArticle.ToString();
            //    lg.AccountCode = ligne.AccountCode;
            //    lg.UnitPrice = ligne.PrixUnitaire;
            //    lg.VATGroup = ligne.CodeTVA;
            //    lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
            //    lignes.Add(lg);
            //}

            ////AH 13juillet16 control sil faut effectuer une ecriture dans SAP
            //string idDocSAP = string.Empty;
            //if (writeSAP == true)
            //{
            //    idDocSAP = sapWS.AddCreditNote(sessionID, codeClient, date, taxDate, date, numEsc + "-" + numBl, consignee,
            //               idfacture, lignes.ToArray());
            //}
            //else
            //{
            //    idDocSAP = idDocAvoirSAP.ToString();
            //}

            //sapWS.LogOut(sessionID);
            //return idDocSAP;
            return (idfacture + 2000).ToString();
        }

        public string FactureArmateur(List<LigneEcriture> elts, string _date, string ccArm, string numEsc)
        {
               SocSAPWS.SocSAPWebService sapWS = new SocSAPWS.SocSAPWebService();
                string sessionID = sapWS.Login(sap_svr, sap_db_name, "dst_MSSQL2008", sap_db_usr, sap_db_pwd, "nova", "Passw0rd", "ln_French", sap_licence);
                List<SocSAPWS.LigneFacture> lignes = new List<SocSAPWS.LigneFacture>();
               
                foreach (LigneEcriture ligne in elts.Where(l => l.CodeArticle != 2202))
                {
                    SocSAPWS.LigneFacture lg = new SocSAPWS.LigneFacture();
                    lg.ItemCode = ligne.CodeArticle.ToString();
                    lg.AccountCode = ligne.AccountCode;
                    lg.Quantity = ligne.Qte.ToString().Replace(",", ".");
                    lg.UnitPrice = ligne.PrixUnitaire;
                    lg.VATGroup = ligne.CodeTVA;
                    lignes.Add(lg);
                }

                if (elts.Count(e => e.CodeArticle == 2202) != 0)
                {
                    SocSAPWS.LigneFacture lgStevedoring = new SocSAPWS.LigneFacture();
                    lgStevedoring.ItemCode = elts.FirstOrDefault(e => e.CodeArticle == 2202).CodeArticle.ToString();
                    lgStevedoring.AccountCode = elts.FirstOrDefault(e => e.CodeArticle == 2202).AccountCode;
                    lgStevedoring.Quantity = "1";
                    lgStevedoring.UnitPrice = Math.Round(elts.Where(el => el.CodeArticle == 2202).Sum(e => e.PrixUnitaire * e.Qte), 0, MidpointRounding.AwayFromZero);
                    lgStevedoring.VATGroup = "TVAAP";
                    lignes.Add(lgStevedoring);
                }
                string idDocSAP = string.Empty;
                 idDocSAP = sapWS.AddInvoice(sessionID, ccArm, _date, _date, _date, numEsc, "Facture armateur - Escale : " + numEsc, lignes.ToArray());

                sapWS.LogOut(sessionID);
                return idDocSAP;

        }
    }
     
}

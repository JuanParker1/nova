using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represente les methodes de generation de fichier extracte pour SAGE
    /// </summary>
    public class VsomSAGE: SuperClass
    {
        VSOMClassesDataContext dcSage = new VSOMClassesDataContext();
        public VsomSAGE()
            : base()
        { }

        /// <summary>
        /// génère le fichier edi compatible avec sage
        /// </summary>
        public string NOVAReport(DateTime debut, DateTime fin)
        {
            //TODO: prise en compte des   facture annulé dans la journée/ des paiement annulée / des avoir partiel facture spote
            StringBuilder sb = new StringBuilder();

            //creation des date piece pour paiement et avoirs
            DateTime _tempdate = debut;  List<string> dpieces = new List<string>();
            dpieces.Add(DateFormat(_tempdate));
            while (_tempdate != fin)
            {  
            _tempdate=_tempdate.AddDays(1);
            dpieces.Add(DateFormat(_tempdate));
                 
            }
             
            //liste des factures
            DateTime _fin = fin.AddHours(23);

            var lst = (from fact in dcSage.GetTable<FACTURE>()
                       where fact.DCFD >= debut && fact.DCFD <= _fin
                       //&& fact.IdFP.HasValue
                       select fact).ToList<FACTURE>();

            List<CODE_TVA> lct = (from m in dcSage.GetTable<CODE_TVA>() select m).ToList<CODE_TVA>();
            List<NOVA_SAGE> sage= new List<NOVA_SAGE>(); double credit=0; double debit=0;
             

            #region ecriture facture 
            string tes0 = string.Empty;
            try
            {

                foreach (FACTURE f in lst)
                {
                    tes0 = f.IdDocSAP.ToString();
                    List<ELEMENT_FACTURATION> lp;
                    // List<LIGNE_PROFORMA> lp = f.PROFORMA.LIGNE_PROFORMA.ToList<LIGNE_PROFORMA>();
                    if (f.StatutFD == "A" && f.IdFP != null)
                    {
                        lp = (from m in dcSage.GetTable<ELEMENT_FACTURATION>()
                              from p in dcSage.GetTable<LIGNE_PROFORMA>()
                              where p.IdJEF == m.IdJEF && p.IdFP == f.IdFP
                              select m).ToList<ELEMENT_FACTURATION>();
                    }
                    else
                    { lp = (from m in dcSage.GetTable<ELEMENT_FACTURATION>() where m.IdFD == f.IdFD select m).ToList(); }


                    string _Description = f.IdFP.HasValue ? f.PROFORMA.CONNAISSEMENT.NumBL : "SPOT";
                    //LIGNE_PROFORMA _lp;
                    ELEMENT_FACTURATION _lp;
                    credit = 0; debit = Convert.ToDouble(f.MTTC.ToString());

                    for (int i = 0; i < lp.Count; i++)
                    {
                        _lp = lp[i];
                        double montant = _lp.CodeTVA == "TVADA" ? Math.Round(Convert.ToDouble(_lp.PUEF.Value * _lp.QTEEF * 1.1925f), 0, MidpointRounding.AwayFromZero) : Math.Round(Convert.ToDouble(_lp.PUEF.Value * _lp.QTEEF), 0, MidpointRounding.AwayFromZero);

                        sage.Add(new NOVA_SAGE
                        {
                            Code = "300",
                            InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                            FC = "FC",
                            CodeTitle = _lp.TauxTVA == 0 ? _lp.LIGNE_PRIX.ARTICLE.CCArticleEx : _lp.LIGNE_PRIX.ARTICLE.CCArticle,
                            X = "G",
                            CustomerCode = "",
                            InvoiceNumber = "FC" + f.IdDocSAP.ToString(),
                            Description = _Description + "-" + _lp.CodeArticle,
                            PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                            DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                            DebitCredit = "C",
                            GrossAmount = montant.ToString(),
                            N = "N"
                        });
                        //cumul credit
                        credit += montant;

                        //il reste le taux tva 

                        if (_lp.CodeTVA == "TVAAP" && _lp.TauxTVA != 0)
                        {
                            sage.Add(new NOVA_SAGE
                            {
                                Code = "300",
                                InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                                FC = "FC",
                                CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte,
                                X = "G",
                                CustomerCode = "",
                                InvoiceNumber = "FC" + f.IdFD.ToString(),
                                Description = _Description,
                                //PayType = f.CLIENT.CodeClient == "C0200" ? PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))) : "S",
                                PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                                DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                                DebitCredit = "C",
                                GrossAmount = Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                                N = "N"
                            });

                            //ajoute le montant au credit 
                            credit += Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero);
                        }

                        if (_lp.CodeTVA == "TVATI" && _lp.TauxTVA != 0)
                        {
                            sage.Add(new NOVA_SAGE
                            {
                                Code = "300",
                                InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                                FC = "FC",
                                CodeTitle = lct.Single(p => p.CodeTVA == "TVATI").CCompte,
                                X = "G",
                                CustomerCode = "",
                                InvoiceNumber = "FC" + f.IdFD.ToString(),
                                Description = _Description,
                                //PayType = f.CLIENT.CodeClient == "C0200" ? PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))) : "S",
                                PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                                DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                                DebitCredit = "C",
                                GrossAmount = Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                                N = "N"
                            });

                            //ajoute le montant au credit 
                            credit += Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero);
                        }
                    }
                    /* fin detail element facture*/

                    if (debit != credit)
                    {
                        if (Math.Abs(debit - credit) < 100)
                        {
                            if (credit > debit)
                            {
                                sage.Add(new NOVA_SAGE
                                {
                                    Code = "300",
                                    InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                                    FC = "FC",
                                    CodeTitle = "6515102",
                                    X = "G",
                                    CustomerCode = "",
                                    InvoiceNumber = "FC" + f.IdFD.ToString(),
                                    Description = _Description,
                                    //PayType = "S",
                                    PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                                    DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                                    DebitCredit = "D",
                                    GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                    N = "N"
                                });
                            }

                            if (credit < debit)
                            {
                                sage.Add(new NOVA_SAGE
                                {
                                    Code = "300",
                                    InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                                    FC = "FC",
                                    CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                                    X = "G",
                                    CustomerCode = "",
                                    InvoiceNumber = "FC" + f.IdFD.ToString(),
                                    Description = _Description,
                                    //PayType = "S",
                                    PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                                    DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                                    DebitCredit = "C",
                                    GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                    N = "N"
                                });
                            }
                        }
                        else
                        {
                            sb.AppendLine(string.Format("La transaction {0} doit être véridier. Valeur decard : {1}", "FC" + f.IdFD.ToString(), (debit - credit)));
                        }
                    }

                    //ecrit au debit le montant de la facture au compte client

                    sage.Add(new NOVA_SAGE
                    {
                        Code = "300",
                        InvoiceDate = DateFormat(f.DateComptable.Value.AddDays(0)),
                        FC = "FC",
                        CodeTitle = "4111100",
                        X = "X",
                        CustomerCode = f.CLIENT.CodeClient,
                        InvoiceNumber = "FC" + f.IdFD.ToString(),
                        Description = _Description + "-" + f.CLIENT.CodeClient,
                        //PayType = f.CLIENT.CodeClient == "C0200" ? PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))) : "S",
                        PayType = PayMode((f.PAYEMENT != null ? f.PAYEMENT.ModePay.Value : short.Parse("0"))),
                        DatePay = (f.PAYEMENT != null ? DateFormat(f.PAYEMENT.DatePay.Value) : DateFormat(f.DCFD.Value)),
                        DebitCredit = "D",
                        GrossAmount = f.MTTC.Value.ToString(),
                        N = "N"
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} , {1}, {2}", tes0, ex.Message, ex.StackTrace));
            }
            #endregion

            
            //on prend les paiements a partir des transactions. a la difference des factures qui sont pris directement de facture
            //du fait des oublie  de ligne genere par le trigger

            List<TRANSACTIONS> lstPaiement = (from m in dcSage.GetTable<TRANSACTIONS>() where m.TPIECE == "RC" &&
                                                dpieces.Contains(m.DPIECE)  select m).ToList<TRANSACTIONS>();

            List<string> distinctTransaction;
            string tes = string.Empty;// = new StringBuilder();
            try
            {

                #region ecritures des paiements
                //recupère les idunique de reference transaction utile pour le controle decriture debit credi
                distinctTransaction = lstPaiement.Select(p => p.REF).Distinct().ToList();
                
                foreach (string str in distinctTransaction)
                {
                   

                    tes=str;
                    //select les transaction liee a cette reference
                    List<TRANSACTIONS> curTrans = (from m in lstPaiement where m.REF.Equals(str) select m).ToList();
                    //control si la trancation comporte les ligne de credit de debit sinon ecrir dans le recap de generation
                    if (curTrans.Count(p => p.SENS == "C") == 0 || curTrans.Count(p => p.SENS == "D") != 1)
                    {
                        sb.AppendLine(string.Format("La transaction {0} n'est pas conforme", str));
                    }
                    else
                    {
                        //recupere des donnée du paiement correctement formater la description
                        string idpay = str.Remove(0, 2);
                        PAYEMENT pay = (from m in dcSage.GetTable<PAYEMENT>() where m.IdPay.ToString() == idpay select m).FirstOrDefault();
                        //la liste des factures ayant fais l'objet de ce payement
                        List<FACTURE> fact = pay == null ? null : (from m in dcSage.GetTable<FACTURE>() where m.IdPay == pay.IdPay select m).ToList();
                        //FACTURE fact = pay == null ? null : (from m in dcSage.GetTable<FACTURE>() where m.IdPay == pay.IdPay select m).FirstOrDefault();
                        string _Description1;
                        string invnum;
                        //if (pay == null && fact==null)
                        if (pay == null && fact.Count == 0)
                        {
                            sb.AppendLine(string.Format("La transaction {0} est incoherente ", str));
                        }
                        else
                        {
                            //if (pay != null && fact == null)
                            if (pay != null && fact.Count == 0) //caution encaissement ou rembourssement
                            {

                                //encaissement
                                if (pay.ObjetPay == 2)
                                {
                                    int _montan = 0;

                                    #region code
                                    _Description1 = "DC-" + pay.CONNAISSEMENT.NumBL;
                                    //liste tous les conteneur ayant fais l'objet de ce paiement
                                    List<CONTENEUR> lctr = (from ctr in dcSage.GetTable<CONTENEUR>() where ctr.IdPay == pay.IdPay select ctr).ToList();
                                    //cree les ligne sage. le payment c'est une ligne credit et une ligne debit. donc on fait ceci
                                    TRANSACTIONS _paycredit = (from m in curTrans where m.SENS == "C" select m).FirstOrDefault();
                                    TRANSACTIONS _paydebit = (from m in curTrans where m.SENS == "D" select m).FirstOrDefault();

                                    foreach (CONTENEUR ctr in lctr)
                                    {
                                        sage.Add(new NOVA_SAGE
                                        {
                                            Code = _paycredit.CJ,
                                            InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                            FC = _paycredit.TPIECE,
                                            CodeTitle = _paycredit.CMPTGEN,
                                            X = _paycredit.CMPTTYP,
                                            CustomerCode = _paycredit.CMPTAUX,
                                            InvoiceNumber = _paycredit.REF,
                                            Description = _Description1 + '-' + ctr.NumCtr, //tran.LIB,
                                            PayType = _paycredit.PAYMOD,
                                            DatePay = _paycredit.DATECH,
                                            DebitCredit = "C",
                                            GrossAmount = ctr.MCCtr.Value.ToString(),
                                            N = _paycredit.TYPE
                                        });
                                        sage.Add(new NOVA_SAGE
                                        {
                                            Code = _paydebit.CJ,
                                            InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                            FC = _paydebit.TPIECE,
                                            CodeTitle = _paydebit.CMPTGEN,
                                            X = _paydebit.CMPTTYP,
                                            CustomerCode = _paydebit.CMPTAUX,
                                            InvoiceNumber = _paydebit.REF,
                                            Description = _Description1 + '-' + ctr.NumCtr,
                                            PayType = _paydebit.PAYMOD,
                                            DatePay = _paydebit.DATECH,
                                            DebitCredit = "D",
                                            GrossAmount = ctr.MCCtr.Value.ToString(),
                                            N = _paydebit.TYPE
                                        });
                                        _montan += ctr.MCCtr.Value;
                                    }
                                    #endregion

                                    if (_montan != int.Parse(_paydebit.MONTANT))
                                    {
                                        sb.AppendLine(string.Format("La transaction {0} est incomplete. Montant : {1}, Debit : {2} ", str, _montan, _paydebit.MONTANT));
                                    }

                                }

                                //rembourssement
                                if (pay.ObjetPay == 3)
                                {
                                    #region code
                                    _Description1 = "PC-" + pay.CONNAISSEMENT.NumBL;
                                    //retrouver tous les paiements assoicié au rembourssemen puis les conteneur associes
                                    List<CONTENEUR> lstCtr2 = (from p in dcSage.GetTable<PAYEMENT>()
                                                               from m in dcSage.GetTable<CONTENEUR>()
                                                               where p.IdPay == m.IdPay && p.IdPayDRC == pay.IdPay
                                                               select m).ToList();
                                    //cree les ligne sage. le payment c'est une ligne credit et une ligne debit. donc on fait ceci
                                    TRANSACTIONS _paycredit = (from m in curTrans where m.SENS == "C" select m).FirstOrDefault();
                                    TRANSACTIONS _paydebit = (from m in curTrans where m.SENS == "D" select m).FirstOrDefault();
                                    int _montan = 0;
                                    foreach (CONTENEUR ctr in lstCtr2)
                                    {
                                        sage.Add(new NOVA_SAGE
                                        {
                                            Code = _paycredit.CJ,
                                            InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                            FC = _paycredit.TPIECE,
                                            CodeTitle = _paycredit.CMPTGEN,
                                            X = _paycredit.CMPTTYP,
                                            CustomerCode = _paycredit.CMPTAUX,
                                            InvoiceNumber = _paycredit.REF,
                                            Description = _Description1 + '-' + ctr.NumCtr, //tran.LIB,
                                            PayType = _paycredit.PAYMOD,
                                            DatePay = _paycredit.DATECH,
                                            DebitCredit = "C",
                                            GrossAmount = ctr.MCCtr.Value.ToString(),
                                            N = _paycredit.TYPE
                                        });
                                        sage.Add(new NOVA_SAGE
                                        {
                                            Code = _paydebit.CJ,
                                            InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                            FC = _paydebit.TPIECE,
                                            CodeTitle = _paydebit.CMPTGEN,
                                            X = _paydebit.CMPTTYP,
                                            CustomerCode = _paydebit.CMPTAUX,
                                            InvoiceNumber = _paydebit.REF,
                                            Description = _Description1 + '-' + ctr.NumCtr,
                                            PayType = _paydebit.PAYMOD,
                                            DatePay = _paydebit.DATECH,
                                            DebitCredit = "D",
                                            GrossAmount = ctr.MCCtr.Value.ToString(),
                                            N = _paydebit.TYPE
                                        });
                                        _montan += ctr.MCCtr.Value;
                                    }
                                    #endregion

                                    if (_montan != int.Parse(_paydebit.MONTANT))
                                    {
                                        sb.AppendLine(string.Format("La transaction {0} est incomplete. Montant : {1}, Debit : {2} ", str, _montan, _paydebit.MONTANT));
                                    }
                                }

                            }
                            else
                            {
                                TRANSACTIONS _paycredit = (from m in curTrans where m.SENS == "C" select m).FirstOrDefault();
                                TRANSACTIONS _paydebit = (from m in curTrans where m.SENS == "D" select m).FirstOrDefault();
                                
                                if (_paydebit.PAYMOD == "V") //chercher le compte banque correspondant
                                {
                                    _paydebit.CMPTGEN = "5141100"; //pay.BANQUE1.CCBanque;
                                }
                                //pour chaque facture on cree la ligne de paiement sur la base du paiement initial

                                foreach (FACTURE _fact in fact)
                                {
                                    invnum = "FC"+_fact.IdDocSAP.ToString();
                                    string _Description2 = _fact.IdFP == null ? "SPOT-" + _paycredit.REF : pay.CONNAISSEMENT.NumBL + "-"+_paycredit.REF ;//
                                    
                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _paycredit.CJ,
                                        InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                        FC = _paycredit.TPIECE,
                                        CodeTitle = _paycredit.CMPTGEN,
                                        X = _paycredit.CMPTTYP,
                                        CustomerCode = _paycredit.CMPTAUX,
                                        InvoiceNumber = invnum, //tran.REF,
                                        Description = _Description2, //tran.LIB,
                                        PayType = _paycredit.PAYMOD,
                                        DatePay = _paycredit.DATECH,
                                        DebitCredit = _paycredit.SENS,
                                        GrossAmount = _paycredit.CJ == "EC" ? (-1 * _fact.MTTC).ToString() : _fact.MTTC.ToString(),
                                        N = _paycredit.TYPE
                                    });
                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _paydebit.CJ,
                                        InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                        FC = _paydebit.TPIECE,
                                        CodeTitle = _paydebit.CMPTGEN,
                                        X = _paydebit.CMPTTYP,
                                        CustomerCode = _paydebit.CMPTAUX,
                                        InvoiceNumber = invnum, //tran.REF,
                                        Description = _Description2, //tran.LIB,
                                        PayType = _paydebit.PAYMOD,
                                        DatePay = _paydebit.DATECH,
                                        DebitCredit = _paydebit.SENS,
                                        GrossAmount = _paycredit.CJ == "EC" ? (-1 * _fact.MTTC).ToString() : _fact.MTTC.ToString(),
                                        N = _paydebit.TYPE
                                    });
                                }

                                //ajout des avoir eventuel ayant fais lobjet de ce paiement..
                                List<AVOIR> lstavoir = pay == null ? null : (from m in dcSage.GetTable<AVOIR>() where m.IdPay == pay.IdPay select m).ToList();
                                foreach (AVOIR _av in lstavoir)
                                {
                                    invnum = "AV" + _av.IdDocSAP.ToString();
                                    string _Description2 = invnum + " - " + _paycredit.REF;

                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _paycredit.CJ,
                                        InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                        FC = _paycredit.TPIECE,
                                        CodeTitle = _paycredit.CMPTGEN,
                                        X = _paycredit.CMPTTYP,
                                        CustomerCode = _paycredit.CMPTAUX,
                                        InvoiceNumber = invnum, //tran.REF,
                                        Description = _Description2, //tran.LIB,
                                        PayType = _paycredit.PAYMOD,
                                        DatePay = _paycredit.DATECH,
                                        DebitCredit = _paycredit.SENS,
                                        GrossAmount = _paycredit.CJ == "EC" ? ( _av.MTTC).ToString() : (-1*_av.MTTC).ToString(),
                                        N = _paycredit.TYPE
                                    });
                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _paydebit.CJ,
                                        InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                        FC = _paydebit.TPIECE,
                                        CodeTitle = _paydebit.CMPTGEN,
                                        X = _paydebit.CMPTTYP,
                                        CustomerCode = _paydebit.CMPTAUX,
                                        InvoiceNumber = invnum, //tran.REF,
                                        Description = _Description2, //tran.LIB,
                                        PayType = _paydebit.PAYMOD,
                                        DatePay = _paydebit.DATECH,
                                        DebitCredit = _paydebit.SENS,
                                        GrossAmount = _paycredit.CJ == "EC" ? ( _av.MTTC).ToString() : (-1*_av.MTTC).ToString(),
                                        N = _paydebit.TYPE
                                    });
                                }
                            }
                            #region olde code

                            /* //_Description1 = fact == null ? _Description1 : _Description1 + "-" + "FC" + fact.IdDocSAP.ToString();
                         foreach (TRANSACTIONS tran in curTrans)
                         {
                             invnum=fact==null ? tran.REF : "FC"+fact.IdDocSAP.ToString();
                            
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = tran.CJ,
                                 InvoiceDate = DateFormat(pay.DatePay.Value.AddDays(0)), //tran.DPIECE,
                                 FC = tran.TPIECE,
                                 CodeTitle = tran.CMPTGEN,
                                 X = tran.CMPTTYP,
                                 CustomerCode = tran.CMPTAUX,
                                 InvoiceNumber = invnum, //tran.REF,
                                 Description = _Description1+'-'+tran.REF, //tran.LIB,
                                 PayType = tran.PAYMOD,
                                 DatePay = tran.DATECH,
                                 DebitCredit = tran.SENS,
                                 GrossAmount = tran.MONTANT.ToString(),
                                 N = tran.TYPE
                             });
                         }*/

                            #endregion
                        }
                    }
                }

                #endregion
                 
            }
            catch (Exception ex)
            {
                
                throw new Exception (string.Format("Paiement: {0} , {1}, {2}",tes,ex.Message,ex.StackTrace));
            }
             
 
            List<TRANSACTIONS> lstAvoirs = (from m in dcSage.GetTable<TRANSACTIONS>() where m.TPIECE == "AC" && dpieces.Contains(m.DPIECE) select m).ToList<TRANSACTIONS>();
           // List<TRANSACTIONS> lstAvoirs = (from m in dcSage.GetTable<TRANSACTIONS>() where (m.REF == "AC6110") select m).ToList<TRANSACTIONS>();
           
            String tes2 = string.Empty;  
            try
            {
                #region avoirs

                distinctTransaction = lstAvoirs.Select(p => p.REF).Distinct().ToList();
                credit = 0; debit = 0;
                foreach (string str in distinctTransaction)
                {
                    tes2=str;

                    //select les transaction liee a cette reference
                    List<TRANSACTIONS> curTrans = (from m in lstAvoirs where m.REF.Equals(str) select m).ToList();
                    //control si la trancation comporte les ligne de credit de debit sinon ecrir dans le recap de generation
                    if (curTrans.Count(p => p.SENS == "C") == 0 || curTrans.Count(p => p.SENS == "D") != 1)
                    {
                        sb.AppendLine(string.Format("La transaction {0}  n'est pas conforme ", str));
                    }
                    else
                    {
                        credit = 0; debit = 0;
                        string idav = str.Remove(0, 2);
                        //recupèration de la ligne avoir et element facturation
                        AVOIR Av = (from a in dcSage.GetTable<AVOIR>() where a.IdFA.ToString() == idav select a).FirstOrDefault();
                        string num = "FC" + Av.FACTURE.IdDocSAP.ToString();

                        #region ligne avoir

                        foreach (TRANSACTIONS _lp in curTrans)
                        {
                            double montant = _lp.CODE_TVA == "TVADA" ? Math.Round((Convert.ToDouble(_lp.MONTANT) * 1.1925f), 0, MidpointRounding.AwayFromZero) : Math.Round(Convert.ToDouble(_lp.MONTANT), 0, MidpointRounding.AwayFromZero);

                            sage.Add(new NOVA_SAGE
                            {
                                Code = _lp.CJ,
                                InvoiceDate = _lp.DPIECE,
                                FC = _lp.TPIECE,
                                CodeTitle = _lp.CMPTGEN,
                                X = _lp.CMPTTYP,
                                CustomerCode = _lp.CMPTAUX,
                                InvoiceNumber = num, //tran.REF,
                                Description = _lp.REF, //tran.LIB,
                                PayType = _lp.PAYMOD,
                                DatePay = _lp.DATECH,
                                DebitCredit = _lp.SENS,
                                GrossAmount = montant.ToString(),
                                N = _lp.TYPE
                            });
                            //cumul credit
                            if (_lp.SENS == "C")
                                credit += montant;
                            else
                                debit += montant;

                            //il reste le taux tva 

                            if (_lp.CODE_TVA == "TVAAP" && _lp.TAUX != "0")
                            {
                                sage.Add(new NOVA_SAGE
                                {
                                    Code = _lp.CJ,
                                    InvoiceDate = _lp.DPIECE,
                                    FC = _lp.TPIECE,
                                    CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte,
                                    X = "G",
                                    CustomerCode = "",
                                    InvoiceNumber = num,
                                    Description = _lp.REF,
                                    PayType = _lp.PAYMOD,
                                    DatePay = _lp.DATECH,
                                    DebitCredit = "C",
                                    GrossAmount = Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                                    N = "N"
                                });

                                //ajoute le montant au credit 
                                credit += Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero);
                            }

                            if (_lp.CODE_TVA == "TVATI" && _lp.TAUX != "0")
                            {
                                sage.Add(new NOVA_SAGE
                                {
                                    Code = _lp.CJ,
                                    InvoiceDate = _lp.DPIECE,
                                    FC = _lp.TPIECE,
                                    CodeTitle = lct.Single(p => p.CodeTVA == "TVATI").CCompte,
                                    X = "G",
                                    CustomerCode = "",
                                    InvoiceNumber = num,
                                    Description = _lp.REF,
                                    PayType = _lp.PAYMOD,
                                    DatePay = _lp.DATECH,
                                    DebitCredit = "C",
                                    GrossAmount = Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                                    N = "N"
                                });

                                //ajoute le montant au credit 
                                credit += Math.Round(Convert.ToDouble(montant * 0.1925f), 0, MidpointRounding.AwayFromZero);
                            }
                        }

                        #endregion

                        /* fin detail element avoir*/

                        #region control arrondi avec compte complemetaire

                        TRANSACTIONS _lp_ = curTrans[0];
                        if (debit != credit)
                        {
                            if (Math.Abs(debit - credit) < 100)
                            {
                                if (credit > debit)
                                {
                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _lp_.CJ,
                                        InvoiceDate = _lp_.DPIECE,
                                        FC = _lp_.TPIECE,
                                        CodeTitle = "6515102",
                                        X = "G",
                                        CustomerCode = "",
                                        InvoiceNumber = num,
                                        Description = _lp_.REF,
                                        PayType = _lp_.PAYMOD,
                                        DatePay = _lp_.DATECH,
                                        DebitCredit = "D",
                                        GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                        N = "N"
                                    });
                                }

                                if (credit < debit)
                                {
                                    sage.Add(new NOVA_SAGE
                                    {
                                        Code = _lp_.CJ,
                                        InvoiceDate = _lp_.DPIECE,
                                        FC = _lp_.TPIECE,
                                        CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                                        X = "G",
                                        CustomerCode = "",
                                        InvoiceNumber = num,
                                        Description = _lp_.REF,
                                        PayType = _lp_.PAYMOD,
                                        DatePay = _lp_.DATECH,
                                        DebitCredit = "C",
                                        GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                        N = "N"
                                    });
                                }
                            }
                            else
                            {
                                sb.AppendLine(string.Format("La transaction {0} doit être véridier. Valeur decard : {1}", str, (debit - credit)));
                            }
                        }

                        #endregion

                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Avoir: {0} , {1}, {2}", tes2, ex.Message, ex.StackTrace));
            }
             List<TRANSACTIONS> lstFacturSocomar = (from m in dcSage.GetTable<TRANSACTIONS>() where m.TPIECE == "FC" && m.LIB.StartsWith("SOC") && dpieces.Contains(m.DPIECE) select m).ToList<TRANSACTIONS>();

             #region facture socomar

             distinctTransaction = lstFacturSocomar.Select(p => p.REF).Distinct().ToList();
             foreach (string str in distinctTransaction)
             {
                 //select les transaction liee a cette reference
                 List<TRANSACTIONS> curTrans = (from m in lstFacturSocomar where m.REF.Equals(str) select m).ToList();
                 credit = 0; debit = 0;
                 foreach (TRANSACTIONS _lp in curTrans)
                 {
                     //double montant = _lp.CODE_TVA == "TVADA" ? Math.Round((Convert.ToDouble(_lp.MONTANT) * 1.1925f), 0, MidpointRounding.AwayFromZero) : Math.Round(Convert.ToDouble(_lp.MONTANT), 0, MidpointRounding.AwayFromZero);

                     sage.Add(new NOVA_SAGE
                     {
                         Code = _lp.CJ,
                         InvoiceDate = _lp.DPIECE,
                         FC = _lp.TPIECE,
                         CodeTitle = _lp.CMPTGEN,
                         X = _lp.CMPTTYP,
                         CustomerCode = _lp.CMPTAUX,
                         InvoiceNumber = _lp.REF,
                         Description = _lp.LIB,
                         PayType = _lp.PAYMOD,
                         DatePay = _lp.DATECH,
                         DebitCredit = _lp.SENS,
                         GrossAmount = _lp.MONTANT.ToString(),
                         N = _lp.TYPE
                     });
                     //cumul credit
                     if (_lp.SENS == "C")
                         credit += Convert.ToDouble(_lp.MONTANT);
                     else
                         debit += Convert.ToDouble(_lp.MONTANT);

                     //il reste le taux tva 

                     if (_lp.CODE_TVA == "TVAAP" && _lp.TAUX != "0")
                     {
                         sage.Add(new NOVA_SAGE
                         {
                             Code = _lp.CJ,
                             InvoiceDate = _lp.DPIECE,
                             FC = _lp.TPIECE,
                             CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte,
                             X = "G",
                             CustomerCode = "",
                             InvoiceNumber = _lp.REF,
                             Description = _lp.LIB,
                             PayType = _lp.PAYMOD,
                             DatePay = _lp.DATECH,
                             DebitCredit = "C",
                             GrossAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                             N = "N"
                         });

                         //ajoute le montant au credit 
                         credit += Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero);
                     }


                 }

                 #region control arrondi avec compte complemetaire

                 TRANSACTIONS _lp_ = curTrans[0];
                 if (debit != credit)
                 {
                     if (Math.Abs(debit - credit) < 100)
                     {
                         if (credit > debit)
                         {
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp_.CJ,
                                 InvoiceDate = _lp_.DPIECE,
                                 FC = _lp_.TPIECE,
                                 CodeTitle = "6515102",
                                 X = "G",
                                 CustomerCode = "",
                                 InvoiceNumber = _lp_.REF,
                                 Description = _lp_.LIB,
                                 PayType = _lp_.PAYMOD,
                                 DatePay = _lp_.DATECH,
                                 DebitCredit = "D",
                                 GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                 N = "N"
                             });
                         }

                         if (credit < debit)
                         {
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp_.CJ,
                                 InvoiceDate = _lp_.DPIECE,
                                 FC = _lp_.TPIECE,
                                 CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                                 X = "G",
                                 CustomerCode = "",
                                 InvoiceNumber = _lp_.REF,
                                 Description = _lp_.LIB,
                                 PayType = _lp_.PAYMOD,
                                 DatePay = _lp_.DATECH,
                                 DebitCredit = "C",
                                 GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                 N = "N"
                             });
                         }
                     }
                     else
                     {
                         sb.AppendLine(string.Format("La transaction {0} doit être véridier. Valeur decard : {1}", str, (debit - credit)));
                     }
                 }

                 #endregion
                
             } 
             #endregion
             
             List<TRANSACTIONS> lstFacturArmateur = (from m in dcSage.GetTable<TRANSACTIONS>() where m.TPIECE == "FC" && m.LIB.StartsWith("Arm") && dpieces.Contains(m.DPIECE) select m).ToList<TRANSACTIONS>();

             #region facture Armateur

             distinctTransaction = lstFacturArmateur.Select(p => p.REF).Distinct().ToList();
             foreach (string str in distinctTransaction)
             {
                 //select les transaction liee a cette reference
                 List<TRANSACTIONS> curTrans = (from m in lstFacturArmateur where m.REF.Equals(str) select m).ToList();
                 credit = 0; debit = 0;
                 foreach (TRANSACTIONS _lp in curTrans)
                 {
                     //double montant = _lp.CODE_TVA == "TVADA" ? Math.Round((Convert.ToDouble(_lp.MONTANT) * 1.1925f), 0, MidpointRounding.AwayFromZero) : Math.Round(Convert.ToDouble(_lp.MONTANT), 0, MidpointRounding.AwayFromZero);

                     sage.Add(new NOVA_SAGE
                     {
                         Code = _lp.CJ,
                         InvoiceDate = _lp.DPIECE,
                         FC = _lp.TPIECE,
                         CodeTitle = _lp.CMPTGEN,
                         X = _lp.CMPTTYP,
                         CustomerCode = _lp.CMPTAUX,
                         InvoiceNumber = _lp.REF,
                         Description = _lp.LIB,
                         PayType = _lp.PAYMOD,
                         DatePay = _lp.DATECH,
                         DebitCredit = _lp.SENS,
                         GrossAmount = _lp.MONTANT.ToString(),
                         N = _lp.TYPE
                     });
                     //cumul credit
                     if (_lp.SENS == "C")
                         credit += Convert.ToDouble(_lp.MONTANT);
                     else
                         debit += Convert.ToDouble(_lp.MONTANT);

                     //il reste le taux tva 

                     if (_lp.CODE_TVA == "TVAAP" && _lp.TAUX != "0")
                     {
                         sage.Add(new NOVA_SAGE
                         {
                             Code = _lp.CJ,
                             InvoiceDate = _lp.DPIECE,
                             FC = _lp.TPIECE,
                             CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte,
                             X = "G",
                             CustomerCode = "",
                             InvoiceNumber = _lp.REF,
                             Description = _lp.LIB,
                             PayType = _lp.PAYMOD,
                             DatePay = _lp.DATECH,
                             DebitCredit = "C",
                             GrossAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                             N = "N"
                         });

                         //ajoute le montant au credit 
                         credit += Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero);
                     }


                 }

                 #region control arrondi avec compte complemetaire

                 TRANSACTIONS _lp_ = curTrans[0];
                 if (debit != credit)
                 {
                     if (Math.Abs(debit - credit) < 100)
                     {
                         if (credit > debit)
                         {
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp_.CJ,
                                 InvoiceDate = _lp_.DPIECE,
                                 FC = _lp_.TPIECE,
                                 CodeTitle = "6515102",
                                 X = "G",
                                 CustomerCode = "",
                                 InvoiceNumber = _lp_.REF,
                                 Description = _lp_.LIB,
                                 PayType = _lp_.PAYMOD,
                                 DatePay = _lp_.DATECH,
                                 DebitCredit = "D",
                                 GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                 N = "N"
                             });
                         }

                         if (credit < debit)
                         {
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp_.CJ,
                                 InvoiceDate = _lp_.DPIECE,
                                 FC = _lp_.TPIECE,
                                 CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                                 X = "G",
                                 CustomerCode = "",
                                 InvoiceNumber = _lp_.REF,
                                 Description = _lp_.LIB,
                                 PayType = _lp_.PAYMOD,
                                 DatePay = _lp_.DATECH,
                                 DebitCredit = "C",
                                 GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                 N = "N"
                             });
                         }
                     }
                     else
                     {
                         sb.AppendLine(string.Format("La transaction {0} doit être véridier. Valeur decard : {1}", str, (debit - credit)));
                     }
                 }

                 #endregion

             }
             #endregion
             
             List<TRANSACTIONS> lstOS = (from m in dcSage.GetTable<TRANSACTIONS>() where m.TPIECE == "FF"  && dpieces.Contains(m.DPIECE) select m).ToList<TRANSACTIONS>();
            

             #region cloture OS

             distinctTransaction = lstOS.Select(p => p.LIB).Distinct().ToList();
              tes2 = string.Empty; 
             try
             {
                 foreach (string str in distinctTransaction)
                 {
                     tes2 = str;
                     //select les transaction liee a cette reference
                     List<TRANSACTIONS> curTrans = (from m in lstOS where m.LIB.Equals(str) orderby m.SENS select m).ToList();
                     credit = 0; debit = 0;

                     //recharger lOS pour retrouver la date de validation just pour opération 2016
                     //string[] libos = curTrans[0].LIB.Split('-');
                     //int idos = int.Parse(libos[0].Substring(2, (libos[0].Length - 2)));
                     //ORDRE_SERVICE os = (from m in dcSage.GetTable<ORDRE_SERVICE>() where m.IdOS == idos select m).FirstOrDefault();

                     foreach (TRANSACTIONS _lp in curTrans)
                     {
                         //double montant = _lp.CODE_TVA == "TVADA" ? Math.Round((Convert.ToDouble(_lp.MONTANT) * 1.1925f), 0, MidpointRounding.AwayFromZero) : Math.Round(Convert.ToDouble(_lp.MONTANT), 0, MidpointRounding.AwayFromZero);

                         //cumul credit
                         if (_lp.SENS == "D")
                         {

                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp.CJ,
                                 InvoiceDate = _lp.DPIECE, //os==null ? "311216" : DateFormat(os.DVOS.Value),
                                 FC = _lp.TPIECE,
                                 CodeTitle = _lp.CMPTGEN,
                                 X = _lp.CMPTTYP,
                                 CustomerCode = _lp.CMPTAUX,
                                 InvoiceNumber = _lp.REF,
                                 Description = _lp.LIB,
                                 PayType = _lp.PAYMOD,
                                 DatePay = _lp.DATECH,
                                 DebitCredit = _lp.SENS,
                                 GrossAmount = _lp.MONTANT.ToString(),
                                 N = _lp.TYPE
                             });

                             debit += Convert.ToDouble(_lp.MONTANT);
                         }
                         else
                         {
                         }

                         //il reste le taux tva 

                         if (_lp.TAUX != "0")
                         {
                             sage.Add(new NOVA_SAGE
                             {
                                 Code = _lp.CJ,
                                 InvoiceDate = _lp.DPIECE,
                                 FC = _lp.TPIECE,
                                 CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte,
                                 X = "G",
                                 CustomerCode = "",
                                 InvoiceNumber = _lp.REF,
                                 Description = _lp.LIB,
                                 PayType = _lp.PAYMOD,
                                 DatePay = _lp.DATECH,
                                 DebitCredit = "D",
                                 GrossAmount = Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero).ToString(),
                                 N = "N"
                             });

                             //ajoute le montant au credit 
                             debit += Math.Round(Convert.ToDouble(Convert.ToDouble(_lp.MONTANT) * 0.1925f), 0, MidpointRounding.AwayFromZero);
                         }


                     }
                     //creation ligne fournisseur
                     TRANSACTIONS _lp2 = curTrans.Single(p => p.SENS == "C");
                     sage.Add(new NOVA_SAGE
                     {
                         Code = _lp2.CJ,
                         InvoiceDate = _lp2.DPIECE, //os == null ? "311216" : DateFormat(os.DVOS.Value),
                         FC = _lp2.TPIECE,
                         CodeTitle = _lp2.CMPTGEN,
                         X = _lp2.CMPTTYP,
                         CustomerCode = _lp2.CMPTAUX,
                         InvoiceNumber = _lp2.REF,
                         Description = _lp2.LIB,
                         PayType = _lp2.PAYMOD,
                         DatePay = _lp2.DATECH,
                         DebitCredit = _lp2.SENS,
                         GrossAmount = debit.ToString(),
                         N = _lp2.TYPE
                     });


                     #region control arrondi avec compte complemetaire

                     //TRANSACTIONS _lp_ = curTrans[0];
                     //if (debit != credit)
                     //{
                     //    if (Math.Abs(debit - credit) < 100)
                     //    {
                     //        if (credit > debit)
                     //        {
                     //            sage.Add(new NOVA_SAGE
                     //            {
                     //                Code = _lp_.CJ,
                     //                InvoiceDate = _lp_.DPIECE,
                     //                FC = _lp_.TPIECE,
                     //                CodeTitle = "6515102",
                     //                X = "G",
                     //                CustomerCode = "",
                     //                InvoiceNumber = _lp_.REF,
                     //                Description = _lp_.LIB,
                     //                PayType = _lp_.PAYMOD,
                     //                DatePay = _lp_.DATECH,
                     //                DebitCredit = "D",
                     //                GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                     //                N = "N"
                     //            });
                     //        }

                     //        if (credit < debit)
                     //        {
                     //            sage.Add(new NOVA_SAGE
                     //            {
                     //                Code = _lp_.CJ,
                     //                InvoiceDate = _lp_.DPIECE,
                     //                FC = _lp_.TPIECE,
                     //                CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                     //                X = "G",
                     //                CustomerCode = "",
                     //                InvoiceNumber = _lp_.REF,
                     //                Description = _lp_.LIB,
                     //                PayType = _lp_.PAYMOD,
                     //                DatePay = _lp_.DATECH,
                     //                DebitCredit = "C",
                     //                GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                     //                N = "N"
                     //            });
                     //        }
                     //    }
                     //    else
                     //    {
                     //        sb.AppendLine(string.Format("La transaction {0} doit être véridier. Valeur decard : {1}", str, (debit - credit)));
                     //    }
                     //}

                     #endregion

                 }

             }
             catch (Exception ex)
             {
                 throw new Exception(string.Format("Facture Fournisseur : {0} , {1}, {2}", tes2, ex.Message, ex.StackTrace));
             }
             #endregion


             string _path = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + String.Format("\\sage_{0}_{1}.PNM", DateFormat(debut), DateFormat(fin));
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(_path))
            {
                sw.WriteLine("SOCOMAR - NOVA");
                foreach(NOVA_SAGE ns in sage)
                {
                sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", 
                       ns.Code,ns.InvoiceDate,ns.FC,ns.CodeTitle,ns.X,ns.CustomerCode,ns.InvoiceNumber,ns.Description,ns.PayType,ns.DatePay,ns.DebitCredit,
                       ns.GrossAmount,ns.N));
                }
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + String.Format("\\log_sage_{0}_{1}.txt", DateFormat(debut), DateFormat(fin))))
            {
                sw.WriteLine(sb.ToString());
            }

            return sb.ToString(); ;
        }

        public string NOVA_TRANSACTIONS()
        { 
           //listing des transaction du jour
            string today = DateFormat(DateTime.Today.AddDays(-3));
            List<TRANSACTIONS> lst = (from m in dcSage.GetTable<TRANSACTIONS>() where m.DPIECE==today select m).ToList<TRANSACTIONS>();
            StringBuilder sb = new StringBuilder();
            //recupère les idunique de reference transaction utile pour le controle decriture debit credi
            List<string> distinctTransaction = lst.Select(p => p.REF).Distinct().ToList();

            List<NOVA_SAGE> sage = new List<NOVA_SAGE>();
            NOVA_SAGE _debit;   double montantAP; double montantTI;
            //pour chaque transaction preparer lecriture comptable complete

            List<CODE_TVA> lct = (from m in dcSage.GetTable<CODE_TVA>() select m).ToList<CODE_TVA>();
            double credit=0; double debit=0;

            foreach (string str in distinctTransaction)
            {
                //select les transaction liee a cette reference
                List<TRANSACTIONS> curTrans = (from m in lst where m.REF.Equals(str) select m).ToList();
                //control si la trancation comporte les ligne de credit de debit sinon ecrir dans le recap de generation
                if (curTrans.Count(p => p.SENS == "C") == 0 || curTrans.Count(p => p.SENS == "D")!=1)
                {
                    sb.AppendLine(string.Format("La transaction {0}  n'est pas conforme ",str));
                }
                else
                {
                    _debit=new NOVA_SAGE();
                    montantAP = 0; montantTI = 0; credit = 0; debit = 0;
                    foreach (TRANSACTIONS tran in curTrans)
                    {
                        //garde la transaction de debit pour la fin
                        if (tran.SENS == "C")
                        {
                            //montant hors tax arrondi
                            double mt = Math.Round(Convert.ToDouble(tran.MONTANT), 0, MidpointRounding.AwayFromZero);
                            //valeur tva
                            double tv = tran.CODE_TVA == null ? 0 : Math.Round(Convert.ToDouble(mt * lct.Single(p => p.CodeTVA == tran.CODE_TVA).TauxTVA / 100), 0, MidpointRounding.AwayFromZero);

                            sage.Add(new NOVA_SAGE
                                {
                                    Code = tran.CJ,
                                    InvoiceDate = tran.DPIECE,
                                    FC = tran.TPIECE,
                                    CodeTitle = tran.CMPTGEN,
                                    X = tran.CMPTTYP,
                                    CustomerCode = tran.CMPTAUX,
                                    InvoiceNumber = tran.REF,
                                    Description = tran.LIB,
                                    PayType = tran.PAYMOD,
                                    DatePay = tran.DATECH,
                                    DebitCredit = tran.SENS,
                                    GrossAmount = tran.CODE_TVA == "TVADA" ? (mt + tv).ToString() : mt.ToString(),
                                    N = tran.TYPE
                                });

                            montantTI = tran.CODE_TVA == "TVATI" ? (montantTI + tv) : 0;
                            montantAP = tran.CODE_TVA == "TVAAP" ? (montantAP + tv) : 0;
                            //cumul du credit
                            credit += tran.CODE_TVA == "TVADA" ? (mt + tv) : mt;
                        }
                        else
                        {
                            _debit = new NOVA_SAGE
                              {
                                  Code = tran.CJ,
                                  InvoiceDate = tran.DPIECE,
                                  FC = tran.TPIECE,
                                  CodeTitle = tran.CMPTGEN,
                                  X = tran.CMPTTYP,
                                  CustomerCode = tran.CMPTAUX,
                                  InvoiceNumber = tran.REF,
                                  Description = tran.LIB,
                                  PayType = tran.PAYMOD,
                                  DatePay = tran.DATECH,
                                  DebitCredit = tran.SENS,
                                  GrossAmount = tran.MONTANT,
                                  N = tran.TYPE
                              };

                            debit = Math.Round(Convert.ToDouble(tran.MONTANT), 0, MidpointRounding.AwayFromZero);

                        }
                    }
                    //controle de code tva et generation ligne tva supplementaire
                    if (curTrans.Count(p => p.CODE_TVA == "TVAAP") > 0)
                    {
                        //liste tous ce qui est tvati et calcule
                        double tvaap = 0;
                        List<TRANSACTIONS> ttvati = (from m in curTrans where m.CODE_TVA == "TVAAP" select m).ToList();
                        foreach (TRANSACTIONS tr in ttvati)
                        {
                            tvaap += Math.Round(Convert.ToDouble(Math.Round(Convert.ToDouble(tr.MONTANT), 0, MidpointRounding.AwayFromZero) * lct.Single(p => p.CodeTVA == tr.CODE_TVA).TauxTVA / 100), 0, MidpointRounding.AwayFromZero);
                        }

                        //cumul credit
                        credit += tvaap;

                        sage.Add(new NOVA_SAGE
                        {
                            Code = _debit.Code,
                            InvoiceDate = _debit.InvoiceDate,
                            FC = _debit.FC,
                            CodeTitle = lct.Single(p => p.CodeTVA == "TVAAP").CCompte, //TODO: COMPTE GENERAL TVAAP
                            X = "G",
                            CustomerCode = "",
                            InvoiceNumber = _debit.InvoiceNumber,
                            Description = _debit.Description,
                            PayType = "S",
                            DatePay = _debit.DatePay,
                            DebitCredit = "C",
                            GrossAmount =montantAP.ToString(), //Math.Round((montantAP * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                            N = _debit.N
                        });
                    }

                     if (curTrans.Count(p => p.CODE_TVA == "TVATI") > 0)
                     { 
                         //liste tous ce qui est tvati et calcule
                         double tvati = 0;
                         List<TRANSACTIONS> ttvati = (from m in curTrans where m.CODE_TVA == "TVATI" select m).ToList();
                         foreach (TRANSACTIONS tr in ttvati)
                         {
                            tvati+= Math.Round(Convert.ToDouble( Math.Round(Convert.ToDouble(tr.MONTANT),0, MidpointRounding.AwayFromZero) * lct.Single(p => p.CodeTVA == tr.CODE_TVA).TauxTVA / 100), 0, MidpointRounding.AwayFromZero);
                         }
                         //cumul credit
                         credit += tvati;

                         sage.Add(new NOVA_SAGE
                         {
                             Code = _debit.Code,
                             InvoiceDate = _debit.InvoiceDate,
                             FC = _debit.FC,
                             CodeTitle = lct.Single(p => p.CodeTVA == "TVATI").CCompte, //TODO: COMPTE GENERAL TVATI
                             X = "G",
                             CustomerCode = "",
                             InvoiceNumber = _debit.InvoiceNumber,
                             Description = _debit.Description,
                             PayType = "S",
                             DatePay = _debit.DatePay,
                             DebitCredit = "C",
                             GrossAmount =tvati.ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                             N = _debit.N
                         });
                     }

                     if (debit != credit)
                     {
                         if (Math.Abs(debit - credit) < 20)
                         {
                             if (credit > debit)
                             {
                                 sage.Add(new NOVA_SAGE
                                 {
                                     Code = _debit.Code,
                                     InvoiceDate = _debit.InvoiceDate,
                                     FC = _debit.FC,
                                     CodeTitle = "6515102", //TODO: COMPTE GENERAL TVATI
                                     X = "G",
                                     CustomerCode = "",
                                     InvoiceNumber = _debit.InvoiceNumber,
                                     Description = _debit.Description,
                                     PayType = "S",
                                     DatePay = _debit.DatePay,
                                     DebitCredit = "D",
                                     GrossAmount = (credit - debit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                     N = _debit.N
                                 });
                             }

                             if (credit < debit)
                             {
                                 sage.Add(new NOVA_SAGE
                                 {
                                     Code = _debit.Code,
                                     InvoiceDate = _debit.InvoiceDate,
                                     FC = _debit.FC,
                                     CodeTitle = "7582103", //TODO: COMPTE GENERAL TVATI
                                     X = "G",
                                     CustomerCode = "",
                                     InvoiceNumber = _debit.InvoiceNumber,
                                     Description = _debit.Description,
                                     PayType = "S",
                                     DatePay = _debit.DatePay,
                                     DebitCredit = "C",
                                     GrossAmount = (debit - credit).ToString(), //Math.Round((montantTI * 0.1925), 0, MidpointRounding.AwayFromZero).ToString(),
                                     N = _debit.N
                                 });
                             }
                         }
                         else
                         {
                             sb.AppendLine(string.Format("La transaction {0} doit être véridier", _debit.InvoiceNumber));
                         }
                     }
                     

                    //ajout de l'operation de debit a la liste
                     sage.Add(_debit);


                     
                }
            }

            //generation du fichier sur la base de la liste des ecritures.
            using(System.IO.StreamWriter sw = new System.IO.StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop)+"\\sage_"+today+".PNM"))
            {
                sw.WriteLine("SOCOMAR - NOVA");
                foreach(NOVA_SAGE ns in sage)
                {
                sw.WriteLine(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}", 
                       ns.Code,ns.InvoiceDate,ns.FC,ns.CodeTitle,ns.X,ns.CustomerCode,ns.InvoiceNumber,ns.Description,ns.PayType,ns.DatePay,ns.DebitCredit,
                       ns.GrossAmount,ns.N));
                }
            }

            Console.WriteLine(sb.ToString());
            return sb.ToString();
        }

        private string DateFormat(DateTime date)
        {
            return string.Format("{0:ddMMyy}", date);
        }

        private string PayMode(short mode)
        {
            string _mode = string.Empty;
            switch (mode)
            { 
                case 1:
                    _mode = "E";
                    break;
                case 2:
                    _mode = "C";
                    break;
                case 3:
                    _mode = "V";
                    break;
                case 4:
                    _mode = "T";
                    break;
                default:
                    return "S";

            }
            return _mode;
        }
    }

    internal class NOVA_SAGE
    {
        private string _code; private string _invoicedate; private string _fc; private string _codetitle;
        private string _x; private string _customercode; private string _invoicenumber; private string _description;
        private string _paytype; private string _datepay; private string _debitcredit; private string _grossamount; private string _n;
        public string Code { get { return _code.PadRight(3); } set { _code = value; } }
        public string InvoiceDate { get { return _invoicedate; } set { _invoicedate = value; } }
        public string FC { get { return _fc; } set { _fc = value; } }
        public string CodeTitle { get { return _codetitle.PadRight(13); } set { _codetitle = value; } }
        public string X { get { return _x; } set { _x = value; } }
        public string CustomerCode { get { return _customercode.PadRight(13); } set { _customercode = value; } }
        public string InvoiceNumber { get { return _invoicenumber.PadRight(13); } set { _invoicenumber = value; } }
        public string Description { get { return _description.PadRight(25); } set { _description = value; } }
        public string PayType { get { return _paytype; } set { _paytype = value; } }
        public string DatePay { get { return _datepay; } set { _datepay = value; } }
        public string DebitCredit { get { return _debitcredit; } set { _debitcredit = value; } }
        public string GrossAmount { get { return _grossamount.PadLeft(20); } set { _grossamount = value; } }
        public string N { get { return _n; } set { _n = value; } }

    }
}

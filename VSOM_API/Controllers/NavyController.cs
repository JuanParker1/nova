using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using VSOM_API.Models;
namespace VSOM_API.Controllers
{
    //[EnableCors(origins: "http://app.socomar-cameroun.com", headers: "*", methods: "*")]
    public class NavyController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Navy/Test/")]
        public IHttpActionResult Test()
        { return Ok("ok"); }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl">numero de bl</param>
        /// <param name="type">inv, quote</param>
        /// <param name="tok">token de communication. contient le id client 1 pour particulier ou >1 pour entreprise</param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
       // [Route("api/Navy/Doc/")]
        public IHttpActionResult GetDoc(string bl,string type,string tok)
        {

            BL blview = null;
            DateTime startdate = DateTime.Parse("1/1/2017");
            //le token devrai permettre de savoir el type de client entreprise ou client et gerer la requete en consequence
            try
            {
                //var identity = (ClaimsIdentity)User.Identity;
                int idclient = int.Parse(tok);
                using (var ctx = new VSOMEntities())
                {
                    CONNAISSEMENT con = null;
                    //con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                    //                        .FirstOrDefault<CONNAISSEMENT>();
                    if (type == "quote" || type == "inv" || type == "obl" || type == "ovin")
                    {
                        //if (!tok.Equals("1"))
                        con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.IdClient.Value == idclient && s.DCBL.Value >= startdate && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.StatutBL != "Traité" && s.TypeBL == "R")
                                                .FirstOrDefault<CONNAISSEMENT>();
                        /*if (tok.Equals("1"))
                            con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                                                   .FirstOrDefault<CONNAISSEMENT>();*/
                    }
                    else
                    {
                        /*con = ctx.CONNAISSEMENTs.Where(s => s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                                .FirstOrDefault<CONNAISSEMENT>();*/
                    }

                    if (con != null)
                    {
                        if (type == "obl") //demande particulier controle dexistance de BL
                        {
                           
                            //selection d'un index aleatoire de chassis
                            var rand =new Random();
                            int indveh = rand.Next(con.VEHICULEs.Count);
                            List<VEHICULE> lveh = con.VEHICULEs.ToList<VEHICULE>();

                            VEHICULE veh = lveh[indveh];
                            blview = new BL() { NumBl = "1", Lib="*******"+veh.NumChassis.Remove(0,6) };
                        }

                        if (type == "ovin") //controle de chassis
                        { 
                          
                        }

                        if( type == "quote" || type == "inv")
                        {
                            //demande entreprise
                            blview = new BL()
                            {
                                Adresse = con.AdresseBL,
                                CodeTva = con.CodeTVA,
                                Consignee = con.ConsigneeBL,
                                Notify = con.NotifyBL,
                                NumBl = con.NumBL,
                                Statut = con.StatutBL ,
                                FinFranchise = con.VEHICULEs.ToList<VEHICULE>()[0].FFVeh.Value.ToShortDateString(),
                                Vehicules = new List<Cars>()
                            };
                        }

                        #region bl list vehicule
                        if (type == "quote")
                        {
                            foreach (VEHICULE veh in con.VEHICULEs)
                            {
                                if (veh.IdVehAP == null && veh.StatVeh != "Livré")
                                {
                                    blview.Vehicules.Add(new Cars
                                    {
                                        Chassis = veh.NumChassis,
                                        Description = veh.DescVeh,
                                        Vol = veh.VolCVeh.Value,
                                        Porte = veh.VehPorte,
                                        Attelle = veh.VehAttelle,
                                        Statut = veh.StatVeh,
                                        Longueur = veh.LongCVeh.Value,
                                        Largueur = veh.LargCVeh.Value,
                                        Charge = veh.VehChargé,
                                        FinFranchise = veh.FFVeh.Value
                                        //FinSejour = veh.FSVeh.Value,
                                        //SortiePrevu = veh.DSPVeh.Value
                                    });
                                }
                            }
                        } 
                        #endregion

                        #region bl factures
                        if (type == "inv")
                        {
                            List<FACTURE> invs = (from f in ctx.FACTUREs join p in ctx.PROFORMAs on f.IdFP equals p.IdFP where p.IdBL == con.IdBL select f).ToList<FACTURE>();
                            blview.Invoices = new List<Invoices>();
                            foreach (FACTURE fact in invs)
                            {
                                blview.Invoices.Add(new Invoices { Date = fact.DCFD.Value.ToShortDateString(), HT = fact.MHT.Value, MTTC = fact.MTTC.Value, TVA = fact.MTVA.Value, Statut = fact.StatutFD == "P" ? "Payée" : (fact.StatutFD == "O" ? "En cours" : "Annulée"), Num = fact.IdDocSAP.Value.ToString() });
                            }
                            //liste avoires
                            List<AVOIR> av = ctx.AVOIRs.Where(a => a.IdBL == con.IdBL).ToList<AVOIR>();
                            blview.CN = new List<CreditNotes>();
                            foreach (AVOIR obj in av)
                            {
                                blview.CN.Add(new CreditNotes { Date = obj.DCFA.Value.ToShortDateString(), HT = obj.MHT.Value, MTTC = obj.MTTC.Value, TVA = obj.MTVA.Value, Statut = "", Num = obj.IdDocSAP.Value.ToString() });
                            }
                            //liste payments
                            List<PAYEMENT> pays = ctx.PAYEMENTs.Where(p => p.IdBL == con.IdBL).ToList<PAYEMENT>();
                            blview.Payments = new List<Payments>();
                            foreach (PAYEMENT obj in pays)
                            {
                                blview.Payments.Add(new Payments { Date = obj.DatePay.Value.ToShortDateString(), MTTC = obj.MRPay.Value, Statut = obj.StatutPay == "O" ? "" : "Annulé", Num = obj.IdPaySAP.Value.ToString() });
                            }
                        } 
                        #endregion
                    }
                }


                if (blview == null)
                {
                    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Aucun connaissement trouvé")
                    };
                    throw new HttpResponseException(message);
                   // return NotFound();
                }

                
                return Ok(blview);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_doc_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, type, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year,DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.InnerException); sw.WriteLine("StackTrace : " + ex.StackTrace);
                }


                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Demande non traitée : ")
                };
                throw new HttpResponseException(message);
                //return NotFound();
            }
        }
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="chassis"></param>
        /// <param name="date"></param>
        /// <param name="type">quote,valide</param>
        /// <param name="tok">idclient si quotation et numero quotation si demande de facture</param>
        /// <param name="owner">type de client : entreprise (2) ou particulier(1)</param>
        /// <param name="tkusr">id du compte effectuant loperation (9999) pour le particulier</param>
        /// <returns></returns>
        
        [Authorize]
       //[AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetQuotation(string bl, string chassis, string date, string type , string tok, int owner, int tkusr )
        {
            //if (!ModelState.IsValid)  return BadRequest("Invalid data.");
            List<InvoiceDetails> details = null; List<InvoiceDetails> pendingElmt = null; StringBuilder sb = new StringBuilder();

            try
            {
                int idclient = int.Parse(tok); DateTime startdate = DateTime.Parse("1/1/2017"); string _chassis = string.Empty; DateTime dateFin = DateTime.Parse(date);
                CONNAISSEMENT con = null;
                using (var ctx = new VSOMEntities())
                {
                    
                    con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.DCBL.Value >= startdate && s.IdClient == idclient && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.StatutBL != "Traité" && s.TypeBL == "R")
                                            .FirstOrDefault<CONNAISSEMENT>();
                    
                    if (con != null)
                    {
                        //sb.AppendLine("Connaissement trouve");

                        if (type == "inv")
                        {

                        }
                        else
                        {
                            details = new List<InvoiceDetails>();

                            List<VEHICULE> _lst0 = con.VEHICULEs.ToList();
                            //je filtre les vehicule deja sortie
                            List<VEHICULE> _lst = new List<VEHICULE>();
                            if (_lst0.Count != 0) _lst = _lst0.Where(s => s.StatVeh != "Livré").ToList();

                            //test de franchises : 
                            if (dateFin < _lst[0].FFVeh)
                            {
                                //sb.AppendLine("Date quotation inferrieur date FFVEH");
                                throw new ApplicationException("Veuillez entrer une date supérieur ou égale à la franchise. ");
                            }

                            //recupère les element non payés
                            #region recupère les element non payés
                            List<ELEMENT_FACTURATION> pendel = ctx.ELEMENT_FACTURATION.Where(el => el.IdBL == con.IdBL && el.StatutEF != "Annule" && el.StatutEF != "Facturé" && (el.EltFacture == "MF" || el.EltFacture == "Veh" || el.EltFacture == "BL")).ToList<ELEMENT_FACTURATION>();
                            pendingElmt = new List<InvoiceDetails>();
                            foreach (ELEMENT_FACTURATION el in pendel)
                            {
                                if (el.PUEF != 0 && el.JrVeh != 0)
                                {
                                    InvoiceDetails id = new InvoiceDetails();
                                    id.Ref = el.IdJEF.ToString();
                                    id.Prix = el.PUEF.Value;
                                    id.TvaCode = el.CodeTVA;
                                    id.TvaTaux = el.TauxTVA.Value;
                                    id.Libelle = el.LibEF;
                                    id.Qte = Math.Round(el.QTEEF.Value,3, MidpointRounding.AwayFromZero);
                                    id.Unit = el.UnitEF;
                                    id.Ht = Math.Round((id.Prix * id.Qte),0, MidpointRounding.AwayFromZero);
                                    id.Tva = Math.Round((id.Ht * (id.TvaTaux/100)), 0, MidpointRounding.AwayFromZero); ;
                                    id.MT = id.Ht + id.Tva;
                                    id.Code = el.CodeArticle;
                                    pendingElmt.Add(id);
                                }
                            }
                           // sb.AppendLine("Recupération element de faction non facturée : "+pendingElmt.Count+" elements troivé");
                            #endregion

                            if (chassis == "undefined" || chassis == "null")
                            {
                                _chassis = "All";
                                #region MyRegion all
                                //sb.AppendLine("Quotation ensemble des vehicules du bl");

                                List<ARTICLE> articles = ctx.ARTICLEs.ToList<ARTICLE>();

                                foreach (VEHICULE matchedVehicule in _lst)
                                {
                                    if (!matchedVehicule.IdVehAP.HasValue)
                                    {
                                        DateTime dte = DateTime.Now;
                                        DateTime finAncienSejour = DateTime.Now;

                                        if (matchedVehicule.FSVeh.HasValue)
                                        {
                                            finAncienSejour = matchedVehicule.FSVeh.Value;
                                        }

                                        matchedVehicule.FSVeh = dateFin;

                                        List<VEHICULE> vehsAP = (from v in _lst
                                                                 where v.IdVehAP == matchedVehicule.IdVeh
                                                                 select v).ToList<VEHICULE>();

                                        #region selection des articles et ligne de prix
                                        ARTICLE articleSejourParcAuto = (from art in articles where art.CodeArticle == 1801 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpSejourParcAuto = null;
                                        LIGNE_PRIX lpSejourParcAuto_old = null;

                                        ARTICLE articleDeboursPADPenalite = (from art in articles where art.CodeArticle == 1815 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpDeboursPADPenalite = null;
                                        LIGNE_PRIX lpDeboursPADPenalite_old = null;
                                        double coefPoidsVeh = 1; 
                                        #region old price
                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            if (matchedVehicule.VolCVeh >= 50) 
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" &&  lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                           else if (matchedVehicule.VolCVeh >= 16) 
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                        }
                                        else if (matchedVehicule.StatutCVeh == "N")
                                        {
                                           if (matchedVehicule.VolCVeh >= 50) 
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16) 
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                        } 
                                        #endregion
                                        #region new price
                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            //AH2jan18 if (matchedVehicule.VolCVeh >= 50)
                                            if (matchedVehicule.PoidsCVeh >= 10000)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                coefPoidsVeh =(double) matchedVehicule.PoidsCVeh.Value / 1000;
                                            }
                                            //AH2jan18 else if (matchedVehicule.VolCVeh >= 16)
                                            else if (matchedVehicule.PoidsCVeh >= 2500)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                        }
                                        else if (matchedVehicule.StatutCVeh == "N")
                                        {
                                            //AH2jan18 if (matchedVehicule.VolCVeh >= 50)
                                            if (matchedVehicule.PoidsCVeh >= 10000)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            //AH2jan18 else if (matchedVehicule.VolCVeh >= 16)
                                            else if (matchedVehicule.PoidsCVeh >= 2500)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                        } 
                                        #endregion
                                        sb.AppendLine("ligne de prix recupérées");
                                        #endregion

                                        bool isEltsNotFree = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && (ef.CodeArticle == "1801") && (ef.StatutEF == "Proforma" || ef.IdFD != null))
                                                              .ToList<ELEMENT_FACTURATION>().Count != 0;

                                       // isEltsNotFree = false;
                                       
                                        if (!isEltsNotFree)
                                        {
                                            sb.AppendLine("Bl");
                                            #region  bloc bl element facture empty
                                            //applique au VAE
                                            int nbrvae = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && ef.CodeArticle == "1605").Count();
                                            if (nbrvae != 0)
                                            {
                                                #region VAE
                                                double derogation = 0;//AH (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value * 2; //lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                /* pas de debours pad en VAE
                                                 * InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                eltFactDeboursPADParcAuto.TvaCode = articleDeboursPADPenalite.CodeTVA;// "TVAEX";
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                eltFactDeboursPADParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                details.Add(eltFactDeboursPADParcAuto);
                                                */
                                                #endregion

                                            }
                                            else
                                            {
                                                //AH 5jan18 retrait des derogation sur le stationnement : 
                                                double derogation =(con.BLIL == "Y" ) ? 0.75 : 0;
                                                //control de quotation antérieur a la date de stationnement deja effectué

                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10)
                                                {
                                                    #region niveau 9jours
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                   // eltFactSejourParcAuto.Prix = eltFactSejourParcAuto.Prix * (matchedVehicule.PoidsCVeh / 1000);
                                                    eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAuto.Qty = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactSejourParcAuto.Qte = eltFactSejourParcAuto.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                   if(eltFactSejourParcAuto.Qte!=0) details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX 
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactDeboursPADParcAuto.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAuto.Qty = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactDeboursPADParcAuto.Qte = eltFactDeboursPADParcAuto.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    if(eltFactDeboursPADParcAuto.Qte!=0) details.Add(eltFactDeboursPADParcAuto);

                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10 + 20)
                                                {
                                                    #region niveau 9 + 20 jours

                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qte = 10 * coefPoidsVeh; 
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    if(eltFactSejourParcAutoN1.Qte!=0) details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    // eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 10*coefPoidsVeh; 
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                   if(eltFactSejourParcAutoN1.Qte!=0) details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - 10;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    if(eltFactSejourParcAutoN2.Qte!=0) details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - 10;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                   if(eltFactDeboursPADParcAutoN2.Qte!=0) details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion

                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10 + 20 + 30)
                                                {
                                                    #region 9 + 20 + 30
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                   if(eltFactSejourParcAutoN1.Qte!=0) details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                   if(eltFactDeboursPADParcAutoN1.Qte!=0) details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = 20;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    if(eltFactSejourParcAutoN2.Qte!=0) details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = 20;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    if(eltFactDeboursPADParcAutoN2.Qte!=0) details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20;
                                                    eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                   if(eltFactSejourParcAutoN3.Qte!=0) details.Add(eltFactSejourParcAutoN3);


                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20;
                                                    eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                   if(eltFactDeboursPADParcAutoN3.Qte!=0) details.Add(eltFactDeboursPADParcAutoN3);

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region niveau 4
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                   if(eltFactSejourParcAutoN1.Qte!=0) details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = 20;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                   if(eltFactSejourParcAutoN2.Qte!=0) details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = 20;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                   if(eltFactDeboursPADParcAutoN2.Qte!=0) details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20 + 30).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN3.Qty = 30;
                                                    eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                   if(eltFactSejourParcAutoN3.Qte!=0) details.Add(eltFactSejourParcAutoN3);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20 + 30).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qty = 30;
                                                    eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                   if(eltFactDeboursPADParcAutoN3.Qte!=0) details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN4.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20 - 30;
                                                    eltFactSejourParcAutoN4.Qte = eltFactSejourParcAutoN4.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                    eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                   if(eltFactSejourParcAutoN4.Qte!=0) details.Add(eltFactSejourParcAutoN4);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN4.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20 - 30;
                                                    eltFactDeboursPADParcAutoN4.Qte = eltFactDeboursPADParcAutoN4.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN4.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN4.Ht = eltFactDeboursPADParcAutoN4.Prix * eltFactDeboursPADParcAutoN4.Qte;
                                                    eltFactDeboursPADParcAutoN4.Tva = Math.Round((eltFactDeboursPADParcAutoN4.Ht * eltFactDeboursPADParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN4.MT = eltFactDeboursPADParcAutoN4.Ht + eltFactDeboursPADParcAutoN4.Tva;
                                                   if(eltFactDeboursPADParcAutoN4.Qte!=0) details.Add(eltFactDeboursPADParcAutoN4);

                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region block bl encours de traitement

                                            List<ELEMENT_FACTURATION> eltSejourCalcules = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh &&
                                                                                          ef.CodeArticle == "1801" && ef.StatutEF != "Annule").ToList<ELEMENT_FACTURATION>();
                                            int nbrvae = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && ef.CodeArticle == "1605").Count();

                                            if (nbrvae != 0)
                                            {
                                                // Gestion pour les véhicules vendus aux enchères
                                                #region VAE
                                                double derogation = 0;// (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value * 2; //lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.JrVeh) <= 10) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.JrVeh.Value) : 0;
                                                //AH  "Pénalité de stationnement
                                                eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                               /*pas de debourd pad en VAE 
                                                * InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                // eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                eltFactDeboursPADParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                //AH Débours PAD : Pénalité de stationnement
                                                eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP; ;
                                                eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                details.Add(eltFactDeboursPADParcAuto);
                                                */ 
                                                #endregion
                                            }
                                            else
                                            {
                                                //control de cotation antérieur a la date de stationnement deja effectué
                                                if (dateFin < finAncienSejour)
                                                {
                                                    throw new ApplicationException("Le stationnement  est deja calculé jusqu'au" + finAncienSejour.ToShortDateString());
                                                }
                                                else
                                                {
                                                    // Gestion pour les véhicules suivant le flux normal
                                                    double derogation_new =  (con.BLIL == "Y") ? 0.75 : 0;
                                                    double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                    int jourpalier = 0; double oldsejour = 0;
                                                    double nbroldsejour = eltSejourCalcules.Where(el => el.IdLP == 14 || el.IdLP == 47).Sum(el => el.JrVeh).Value;
                                                   //8janv18 pour definir le temps de sejour du palier 1 est effectue avec le nouveau prix
                                                    double nbrnewsejour = eltSejourCalcules.Where(el => el.IdLP == 668 || el.IdLP == 669).Sum(el => el.JrVeh).Value;

                                                    if ((nbroldsejour+nbrnewsejour) >= 9) jourpalier = 9; else jourpalier = 10;

                                                    if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                        #region niveau 1
                                                        InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();
                                                        
                                                        eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAuto.Qty = dateFin.Date < finAncienSejour ? 0 : ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + oldsejour) <= jourpalier) ? (dateFin - matchedVehicule.FFVeh.Value).Days - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAuto.Qte = eltFactSejourParcAuto.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qty)).ToShortDateString();
                                                        eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                        eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                        details.Add(eltFactSejourParcAuto);

                                                        InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                        eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                        // eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAuto.Qty = dateFin.Date < finAncienSejour ? 0 : ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) + oldsejour) <= jourpalier) ? (dateFin - matchedVehicule.FFVeh.Value).Days - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAuto.Qte = eltFactDeboursPADParcAuto.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAuto.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qty)).ToShortDateString();
                                                        eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                        eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                        details.Add(eltFactDeboursPADParcAuto);
                                                        #endregion
                                                    }
                                                    else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier + 20)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                        #region niveau 2
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase.Value == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                        details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN2);
                                                        #endregion
                                                    }
                                                    else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier + 20 + 30)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                       
                                                        #region niveau 3
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        if (eltFactSejourParcAutoN1.Qte != 0) details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        if (eltFactDeboursPADParcAutoN1.Qte != 0) details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;

                                                        if (eltFactSejourParcAutoN2.Qte != 0) details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        if (eltFactDeboursPADParcAutoN2.Qte != 0) details.Add(eltFactDeboursPADParcAutoN2);


                                                        InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                        eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                        details.Add(eltFactSejourParcAutoN3);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                        eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN3);

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                       
                                                        #region niveau 4
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        if (eltFactSejourParcAutoN1.Qte != 0) details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        if (eltFactDeboursPADParcAutoN1.Qte != 0) details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;

                                                        if (eltFactSejourParcAutoN2.Qte != 0) details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        if (eltFactDeboursPADParcAutoN2.Qte != 0) details.Add(eltFactDeboursPADParcAutoN2);


                                                        InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN3.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 30) ? 30 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                        eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                        if (eltFactSejourParcAutoN3.Qte != 0) details.Add(eltFactSejourParcAutoN3);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN3.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 30) ? 30 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                        eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); 
                                                        eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                        if (eltFactDeboursPADParcAutoN3.Qte != 0) details.Add(eltFactDeboursPADParcAutoN3);

                                                        InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU4LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltFactSejourParcAutoN3.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU4LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN4.Qte = eltFactSejourParcAutoN4.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN4.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte + eltFactSejourParcAutoN4.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                        eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                        details.Add(eltFactSejourParcAutoN4);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU4LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltFactDeboursPADParcAutoN3.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU4LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN4.Qte = eltFactDeboursPADParcAutoN4.Qty * coefPoidsVeh;
                                                        eltFactDeboursPADParcAutoN4.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte + eltFactDeboursPADParcAutoN4.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN4.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN4.Ht = eltFactDeboursPADParcAutoN4.Prix * eltFactDeboursPADParcAutoN4.Qte;
                                                        eltFactDeboursPADParcAutoN4.Tva = Math.Round((eltFactDeboursPADParcAutoN4.Ht * eltFactDeboursPADParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN4.MT = eltFactDeboursPADParcAutoN4.Ht + eltFactDeboursPADParcAutoN4.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN4);

                                                        #endregion
                                                    }
                                                }
                                            }

                                            #endregion
                                        }

                                        // dcAcc.SubmitChanges();

                                    }
                                }
                                #endregion
                            }

                            if (chassis != "undefined" && chassis != "null")
                            {
                                _chassis = chassis;

                                #region all region
                                VEHICULE matchedVehicule = _lst.Single(veh => veh.NumChassis == chassis);
                                if (matchedVehicule != null)
                                {
                                    if (!matchedVehicule.IdVehAP.HasValue)
                                    {
                                        DateTime dte = DateTime.Now;
                                        DateTime finAncienSejour = DateTime.Now;

                                        if (matchedVehicule.FSVeh.HasValue)
                                        {
                                            finAncienSejour = matchedVehicule.FSVeh.Value;
                                        }

                                        matchedVehicule.FSVeh = dateFin;

                                        List<VEHICULE> vehsAP = (from v in _lst
                                                                 where v.IdVehAP == matchedVehicule.IdVeh
                                                                 select v).ToList<VEHICULE>();

                                        List<ARTICLE> articles = ctx.ARTICLEs.ToList<ARTICLE>();


                                        #region selection des articles et ligne de prix
                                        ARTICLE articleSejourParcAuto = (from art in articles where art.CodeArticle == 1801 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpSejourParcAuto = null;
                                        LIGNE_PRIX lpSejourParcAuto_old = null;

                                        ARTICLE articleDeboursPADPenalite = (from art in articles where art.CodeArticle == 1815 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpDeboursPADPenalite = null;
                                        LIGNE_PRIX lpDeboursPADPenalite_old = null;
                                        double coefPoidsVeh = 1;
                                        #region old price
                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                        }
                                        else if (matchedVehicule.StatutCVeh == "N")
                                        {
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                            else
                                            {
                                                lpSejourParcAuto_old = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                                lpDeboursPADPenalite_old = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DFLP == DateTime.Parse("31/12/2017"));
                                            }
                                        }
                                        #endregion
                                        #region new price
                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            //AH2jan18 if (matchedVehicule.VolCVeh >= 50)
                                            if (matchedVehicule.PoidsCVeh >= 10000)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                            }
                                            //AH2jan18 else if (matchedVehicule.VolCVeh >= 16)
                                            else if (matchedVehicule.PoidsCVeh >= 2500)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                        }
                                        else if (matchedVehicule.StatutCVeh == "N")
                                        {
                                            //AH2jan18 if (matchedVehicule.VolCVeh >= 50)
                                            if (matchedVehicule.PoidsCVeh >= 10000)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            //AH2jan18 else if (matchedVehicule.VolCVeh >= 16)
                                            else if (matchedVehicule.PoidsCVeh >= 2500)
                                            {
                                                coefPoidsVeh = (double)matchedVehicule.PoidsCVeh.Value / 1000;
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                        }
                                        #endregion
                                        sb.AppendLine("ligne de prix recupérées");
                                        #endregion

                                        bool isEltsNotFree = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && (ef.CodeArticle == "1801") && (ef.StatutEF == "Proforma" || ef.IdFD != null))
                                                              .ToList<ELEMENT_FACTURATION>().Count != 0;

                                        //isEltsNotFree = false;
                                        
                                        if (!isEltsNotFree)
                                        {
                                            sb.AppendLine("Bl");
                                            #region  bloc bl element facture empty
                                            //applique au VAE
                                            int nbrvae = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && ef.CodeArticle == "1605").Count();
                                            if (nbrvae != 0)
                                            {
                                                #region VAE
                                                double derogation = 0;//AH (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value * 2; //lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                /* pas de debours pad en VAE
                                                 * InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                eltFactDeboursPADParcAuto.TvaCode = articleDeboursPADPenalite.CodeTVA;// "TVAEX";
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                eltFactDeboursPADParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                details.Add(eltFactDeboursPADParcAuto);
                                                */
                                                #endregion

                                            }
                                            else
                                            {
                                                double derogation = 0; //AH retrait derogation sur stationnement janv18 (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                //control de quotation antérieur a la date de stationnement deja effectué

                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10)
                                                {
                                                    #region niveau 9jours
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    // eltFactSejourParcAuto.Prix = eltFactSejourParcAuto.Prix * (matchedVehicule.PoidsCVeh / 1000);
                                                    eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAuto.Qty = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactSejourParcAuto.Qte = eltFactSejourParcAuto.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX 
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactDeboursPADParcAuto.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAuto.Qty = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactDeboursPADParcAuto.Qte = eltFactDeboursPADParcAuto.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    details.Add(eltFactDeboursPADParcAuto);

                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10 + 20)
                                                {
                                                    #region niveau 9 + 20 jours

                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qte = 10 * coefPoidsVeh;
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    // eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 10 * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - 10;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - 10;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion

                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 10 + 20 + 30)
                                                {
                                                    #region 9 + 20 + 30
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = 20;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = 20;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20;
                                                    eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3);


                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20;
                                                    eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region niveau 4
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN1.Qty = 10;
                                                    eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVATI
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qty = 10;
                                                    eltFactDeboursPADParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN2.Qty = 20;
                                                    eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN2.Qty = 20;
                                                    eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20 + 30).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN3.Qty = 30;
                                                    eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(10 + 20 + 30).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qty = 30;
                                                    eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAutoN4.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                    eltFactSejourParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20 - 30;
                                                    eltFactSejourParcAutoN4.Qte = eltFactSejourParcAutoN4.Qty * coefPoidsVeh;
                                                    eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                    eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                    details.Add(eltFactSejourParcAutoN4);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);
                                                    eltFactDeboursPADParcAutoN4.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                    eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 10 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 10 - 20 - 30;
                                                    eltFactDeboursPADParcAutoN4.Qte = eltFactDeboursPADParcAutoN4.Qty * coefPoidsVeh;
                                                    eltFactDeboursPADParcAutoN4.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN4.Ht = eltFactDeboursPADParcAutoN4.Prix * eltFactDeboursPADParcAutoN4.Qte;
                                                    eltFactDeboursPADParcAutoN4.Tva = Math.Round((eltFactDeboursPADParcAutoN4.Ht * eltFactDeboursPADParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN4.MT = eltFactDeboursPADParcAutoN4.Ht + eltFactDeboursPADParcAutoN4.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN4);

                                                    #endregion
                                                }
                                            }
                                            #endregion
                                        
                                        }
                                        else
                                        {

                                            #region block bl encours de traitement

                                            List<ELEMENT_FACTURATION> eltSejourCalcules = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh &&
                                                                                          ef.CodeArticle == "1801" && ef.StatutEF != "Annule").ToList<ELEMENT_FACTURATION>();
                                            int nbrvae = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && ef.CodeArticle == "1605").Count();

                                            if (nbrvae != 0)
                                            {
                                                // Gestion pour les véhicules vendus aux enchères
                                                #region VAE
                                                double derogation = 0;// (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value * 2; //lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.JrVeh) <= 10) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.JrVeh.Value) : 0;
                                                //AH  "Pénalité de stationnement
                                                eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                /*pas de debourd pad en VAE 
                                                 * InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                 eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                 eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                 // eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                 eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                 eltFactDeboursPADParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                 //AH Débours PAD : Pénalité de stationnement
                                                 eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                 eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP; ;
                                                 eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                 eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                 eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                 details.Add(eltFactDeboursPADParcAuto);
                                                 */
                                                #endregion
                                            }
                                            else
                                            {
                                                //control de cotation antérieur a la date de stationnement deja effectué
                                                if (dateFin < finAncienSejour)
                                                {
                                                    throw new ApplicationException("Le stationnement  est deja calculé jusqu'au" + finAncienSejour.ToShortDateString());
                                                }
                                                else
                                                {
                                                    // Gestion pour les véhicules suivant le flux normal
                                                    double derogation_new = 0; ;//AH5jan18 retrait dergation sur stationnement (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                    double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                    int jourpalier = 0; double oldsejour = 0;
                                                    double nbroldsejour = eltSejourCalcules.Where(el => el.IdLP == 14 || el.IdLP == 47).Sum(el => el.JrVeh).Value;
                                                    //8janv18 pour definir le temps de sejour du palier 1 est effectue avec le nouveau prix
                                                    double nbrnewsejour = eltSejourCalcules.Where(el => el.IdLP == 668 || el.IdLP == 669).Sum(el => el.JrVeh).Value;

                                                    if (nbroldsejour >= 9) jourpalier = 9; else jourpalier = 10;

                                                    if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                        #region niveau 1
                                                        InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                        eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAuto.Qty = dateFin.Date < finAncienSejour ? 0 : ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + oldsejour) <= jourpalier) ? (dateFin - matchedVehicule.FFVeh.Value).Days - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAuto.Qte = eltFactSejourParcAuto.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAuto.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                        eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                        eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                        details.Add(eltFactSejourParcAuto);

                                                        InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                        eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                        // eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAuto.Qty = dateFin.Date < finAncienSejour ? 0 : ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) + oldsejour) <= jourpalier) ? (dateFin - matchedVehicule.FFVeh.Value).Days - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAuto.Qte = eltFactDeboursPADParcAuto.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAuto.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                        eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                        details.Add(eltFactDeboursPADParcAuto);
                                                        #endregion
                                                    }
                                                    else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier + 20)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;
                                                        #region niveau 2
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase.Value == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                        details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN2);
                                                        #endregion
                                                    }
                                                    else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= jourpalier + 20 + 30)
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;

                                                        #region niveau 3
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        if (eltFactSejourParcAutoN1.Qte != 0) details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        if (eltFactDeboursPADParcAutoN1.Qte != 0) details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;

                                                        if (eltFactSejourParcAutoN2.Qte != 0) details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        if (eltFactDeboursPADParcAutoN2.Qte != 0) details.Add(eltFactDeboursPADParcAutoN2);


                                                        InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                        eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                        details.Add(eltFactSejourParcAutoN3);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN3.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                        eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN3);

                                                        #endregion
                                                    }
                                                    else
                                                    {
                                                        oldsejour = eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value;

                                                        #region niveau 4
                                                        InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactSejourParcAutoN1.Qte = eltFactSejourParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH "Pénalité de stationnement
                                                        eltFactSejourParcAutoN1.Code = articleSejourParcAuto.CodeArticle.ToString();

                                                        eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                        eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                        if (eltFactSejourParcAutoN1.Qte != 0) details.Add(eltFactSejourParcAutoN1);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN1.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh) + oldsejour) < jourpalier) ? jourpalier - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) + oldsejour) : 0;
                                                        eltFactDeboursPADParcAutoN1.Qte = eltFactDeboursPADParcAutoN1.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN1.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                        eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                        if (eltFactDeboursPADParcAutoN1.Qte != 0) details.Add(eltFactDeboursPADParcAutoN1);

                                                        InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN2.Qte = eltFactSejourParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN2.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                        eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;

                                                        if (eltFactSejourParcAutoN2.Qte != 0) details.Add(eltFactSejourParcAutoN2);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN2.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 20) ? 20 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN2.Qte = eltFactDeboursPADParcAutoN2.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN2.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                        eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                        if (eltFactDeboursPADParcAutoN2.Qte != 0) details.Add(eltFactDeboursPADParcAutoN2);


                                                        InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN3.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 30) ? 30 - (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactSejourParcAutoN3.Qte = eltFactSejourParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN3.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                        eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                        if (eltFactSejourParcAutoN3.Qte != 0) details.Add(eltFactSejourParcAutoN3);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN3.Qty = ((eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) < 30) ? 30 - (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh).Value + eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh).Value) : 0;
                                                        eltFactDeboursPADParcAutoN3.Qte = eltFactDeboursPADParcAutoN3.Qty * coefPoidsVeh;
                                                        //AH Débours PAD : Pénalité de stationnement 
                                                        eltFactDeboursPADParcAutoN3.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                        eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero);
                                                        eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                        if (eltFactDeboursPADParcAutoN3.Qte != 0) details.Add(eltFactDeboursPADParcAutoN3);

                                                        InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                        eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation_new;
                                                        eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                        eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactSejourParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU4LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltFactSejourParcAutoN3.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU4LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactSejourParcAutoN4.Qte = eltFactSejourParcAutoN4.Qty * coefPoidsVeh;
                                                        //AH Pénalité de stationnement
                                                        eltFactSejourParcAutoN4.Code = articleSejourParcAuto.CodeArticle.ToString();
                                                        eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte + eltFactSejourParcAutoN4.Qte)).ToShortDateString();
                                                        eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                        eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                        eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                        details.Add(eltFactSejourParcAutoN4);

                                                        InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                        eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation_new;
                                                        eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                        // eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                        eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                        eltFactDeboursPADParcAutoN4.Qty = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU4LP * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation_new) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation_new)).Sum(el => el.JrVeh.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltFactDeboursPADParcAutoN3.Qte - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU4LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite_old.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.JrVeh.Value);
                                                        eltFactDeboursPADParcAutoN4.Qte = eltFactDeboursPADParcAutoN4.Qty * coefPoidsVeh;
                                                        eltFactDeboursPADParcAutoN4.Code = articleDeboursPADPenalite.CodeArticle.ToString();
                                                        //AH Débours PAD : Pénalité de stationnement
                                                        eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte + eltFactDeboursPADParcAutoN4.Qte)).ToShortDateString();
                                                        eltFactDeboursPADParcAutoN4.Unit = lpDeboursPADPenalite.UniteLP;
                                                        eltFactDeboursPADParcAutoN4.Ht = eltFactDeboursPADParcAutoN4.Prix * eltFactDeboursPADParcAutoN4.Qte;
                                                        eltFactDeboursPADParcAutoN4.Tva = Math.Round((eltFactDeboursPADParcAutoN4.Ht * eltFactDeboursPADParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                        eltFactDeboursPADParcAutoN4.MT = eltFactDeboursPADParcAutoN4.Ht + eltFactDeboursPADParcAutoN4.Tva;
                                                        details.Add(eltFactDeboursPADParcAutoN4);

                                                        #endregion
                                                    }
                                                }
                                            }

                                            #endregion
                                        
                                        }


                                        // dcAcc.SubmitChanges();

                                    }
                                }
                                //else { return NotFound(); } 
                                #endregion
                            }

                        }
                    }

                }

                if (type == "inv")
                {

                    #region enregistrement quodation valide et envoie de mail team socomar
                    //verifie si la validation correspond a l'existant
                    
                    var req = new requetes
                    {
                        ETAT = owner== 2 ? "Pending" : "Processing",
                        ID_QUOTATION = int.Parse(tok),
                        IDCOMPTES = owner,
                        LIBELLE = " Quotation " + tok + " sur BL " + bl + " pour stationnement au " + date,
                        REC_TIME = DateTime.Now
                    };
                    //string email 
                    using (var ctx = new RM_VSOMEntities1())
                    {
                        //TODO: verifie sil y a une quotation deja encours et non valide 

                        quotation quot = ctx.quotation.Where(s => s.ID == req.ID_QUOTATION && s.COMPTES_ID == owner && s.STATUT == "Pending").FirstOrDefault<quotation>();
                        if (quot != null)
                        {
                            ctx.requetes.Add(req);

                            quot.STATUT = "Valide";
                            ctx.SaveChanges();
                        }
                    }

                    if (req.ID == 0)
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_quotation_valid_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, chassis, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                        {
                            sw.WriteLine("echec validation : " + req.ID + " quotation : " + req.ID_QUOTATION);
                        }
                        return NotFound();
                    }
                    else
                    {
                        //envoie de mail pour notification
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("webmail.socomar-cameroun.com");

                        mail.From = new MailAddress("automate@socomar-cameroun.com");
                        mail.To.Add("vehicules@socomar-cameroun.com");
                        mail.CC.Add("support@socomar-cameroun.com");
                        mail.Subject = "Quotation numéro " + tok + " sur BL numero " + bl;
                        //mail.Body = "Merci. Votre quotation numéro  " + req.ID_QUOTATION.Value + "  est en cours de traitement. Vous recevrez votre facture par mail très bientôt";
                        mail.Body = "Une nouvelle quotation est validée. Veuillez proceder a son traitement";
                        SmtpServer.Credentials = new System.Net.NetworkCredential("automate@socomar-cameroun.com", "Socom@r17!");
                        SmtpServer.Send(mail);

                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\quotation_valid_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, chassis, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                        {
                            sw.WriteLine("ok validation : " + req.ID + " quotation : " + req.ID_QUOTATION);
                        }
                        return Ok(req.ID_QUOTATION);
                    } 
                    #endregion
                }
                else
                {
                    if (details == null || details.Count == 0)
                    { return NotFound(); }
                    else
                    {

                        QuotationInvoice qi = new QuotationInvoice();

                        qi.HT = String.Format("{0:N0} XAF", (details.Sum(e => e.Ht) + pendingElmt.Sum(e => e.Ht)) );
                        qi.TVA = String.Format("{0:N0} XAF", (details.Sum(e => e.Tva) + pendingElmt.Sum(e => e.Tva)) );
                        qi.MT = String.Format("{0:N0} XAF", (details.Sum(e => e.MT) + pendingElmt.Sum(e => e.MT)) );
                        qi.Lignes = (from  m in details where m.Qte!=0 select m).ToList();
                        qi.OLignes = (from m in pendingElmt where m.Qte != 0 select m).ToList();

                        if (idclient == 1)//separation concerne uniquement client particulier
                        {
                            List<InvoiceDetails> id = new List<InvoiceDetails>();
                            id.AddRange(details);
                            id.AddRange(pendingElmt);

                            List<string> fmlst = new List<string>(); fmlst.Add("1601"); fmlst.Add("2101"); fmlst.Add("1703"); fmlst.Add("1702"); fmlst.Add("1701"); fmlst.Add("1820");
                            qi.FMLignes = (from m in id where fmlst.Contains(m.Code) select m).ToList();
                            qi.FMHT = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.Ht)); ;
                            qi.FMTVA = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.Tva)) ;
                            qi.FMMT = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.MT));

                            List<string> slst = new List<string>(); slst.Add("1801");  slst.Add("1815");
                            qi.SLignes = (from m in id where slst.Contains(m.Code) select m).ToList();
                            qi.SLHT = String.Format("{0:N0} XAF", qi.SLignes.Sum(e => e.Ht));
                            qi.SLTVA = String.Format("{0:N0} XAF", qi.SLignes.Sum(e => e.Tva));
                            qi.SLMT =String.Format("{0:N0} XAF",  qi.SLignes.Sum(e => e.MT));

                            foreach (InvoiceDetails ids in qi.FMLignes) id.Remove(ids);
                            foreach (InvoiceDetails ids in qi.SLignes) id.Remove(ids);
                            qi.DTLignes = id;
                            qi.DTHT = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.Ht));
                            qi.DTTVA = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.Tva));
                            qi.DTMT = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.MT));

                        }

                        //enregistre la quotation et ses details
                        var quot = new quotation
                           {
                               BL = bl,
                               CHASSIS = _chassis,
                               COMPTES_ID = owner,
                               ENDDATE = dateFin,
                               LEVEL = _chassis == "All" ? "BL" : "Chassis",
                               REC_DATE = DateTime.Now,
                               STATUT = "Pending",
                               HT = int.Parse((details.Sum(e => e.Ht) + pendingElmt.Sum(e => e.Ht)).ToString()),
                               TVA = int.Parse((details.Sum(e => e.Tva) + pendingElmt.Sum(e => e.Tva)).ToString()),
                               TTC = int.Parse((details.Sum(e => e.MT) + pendingElmt.Sum(e => e.MT)).ToString())
                               ,
                               USERS_ID = tkusr,
                               CLIENTS_ID = idclient
                           };
                        using (var ctx = new RM_VSOMEntities1())
                        {

                            ctx.quotation.Add(quot);
                            ctx.SaveChanges();
                            qi.Ref = quot.ID.ToString();
                        }

                        //string str = qi.ToXML();
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\quotation_{0}_{1}_{2}.xml", qi.Ref, bl,tkusr)))
                        {
                            sw.WriteLine(qi.ToXML());
                        }
                        return Ok(qi);
                    }
                }

            }
            catch (ApplicationException ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_quotation_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, chassis, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message application exception: " + ex.Message);
                }

                var message = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(ex.Message)
                };
                throw new HttpResponseException(message);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_quotation_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, chassis, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    //sw.WriteLine("message : " + ex.Message + " -------- innerexception : " + ex.StackTrace);
                    sw.WriteLine("message exception: " + ex.Message); sw.WriteLine("innerexception : " + ex.InnerException); sw.WriteLine("StackTrace : " + ex.StackTrace);
                }

                var message = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Demande non traitée : Veuillez ressayer.")
                };
                throw new HttpResponseException(message);

            }


        }

        [Authorize]
        [HttpGet]
        public IHttpActionResult GetCashing(string bl, int tkusr, string type , int fm , int sl , int dt)
        {
            BL blview = null; List<InvoiceDetails> pendingElmt = null; ESCALE esc = null;
            try
            {
                CONNAISSEMENT con = null; //je force que le client qui peut verifier ce bl doit etre comptant


                if (type == "get") //recuperation des donnees pour creation proforma
                {
                    #region global
                    using (var ctx = new VSOMEntities())
                    {

                        con = ctx.CONNAISSEMENTs.Where(s => s.IdClient.Value == 1 && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Traité" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                                    .FirstOrDefault<CONNAISSEMENT>();

                        if (con != null)
                        {
                            #region recuperation des element en pending sur le BL client
                            List<ELEMENT_FACTURATION> pendel = ctx.ELEMENT_FACTURATION.Where(el => el.IdBL == con.IdBL && el.StatutEF != "Annule" && el.StatutEF != "Facturé" && (el.EltFacture == "MF" || el.EltFacture == "Veh" || el.EltFacture == "BL")).ToList<ELEMENT_FACTURATION>();
                            pendingElmt = new List<InvoiceDetails>();
                            foreach (ELEMENT_FACTURATION el in pendel)
                            {
                                if (el.PUEF != 0 && el.JrVeh != 0)
                                {
                                    InvoiceDetails id = new InvoiceDetails();
                                    id.Ref = el.IdJEF.ToString();
                                    id.Prix = el.PUEF.Value;
                                    id.TvaCode = el.CodeTVA;
                                    id.TvaTaux = el.TauxTVA.Value;
                                    id.Libelle = el.LibEF;
                                    id.Qte = Math.Round(el.QTEEF.Value, 3, MidpointRounding.AwayFromZero);
                                    id.Unit = el.UnitEF;
                                    id.Ht = Math.Round((id.Prix * id.Qte), 0, MidpointRounding.AwayFromZero);
                                    id.Tva = Math.Round((id.Ht * (id.TvaTaux / 100)), 0, MidpointRounding.AwayFromZero); ;
                                    id.MT = id.Ht + id.Tva;
                                    id.Code = el.CodeArticle;
                                    pendingElmt.Add(id);
                                }
                            }
                            #endregion

                           // esc = ctx.ESCALEs.Where(s => s.IdEsc == con.IdEsc).FirstOrDefault<ESCALE>();
                        }
                    }


                    if (pendingElmt.Count > 0)
                    {

                        QuotationInvoice qi = new QuotationInvoice();

                        #region creation et generation reponse http
                        qi.HT = String.Format("{0:N0} XAF", pendingElmt.Sum(e => e.Ht));
                        qi.TVA = String.Format("{0:N0} XAF", (pendingElmt.Sum(e => e.Tva)));
                        qi.MT = String.Format("{0:N0} XAF", (pendingElmt.Sum(e => e.MT)));

                        qi.OLignes = (from m in pendingElmt where m.Qte != 0 select m).ToList();
                        List<InvoiceDetails> id = new List<InvoiceDetails>();
                        id.AddRange(pendingElmt);

                        List<string> fmlst = new List<string>(); fmlst.Add("1601"); fmlst.Add("2101"); fmlst.Add("1703");
                        fmlst.Add("1702"); fmlst.Add("1701"); fmlst.Add("1820");
                        qi.FMLignes = (from m in id where fmlst.Contains(m.Code) select m).ToList();
                        qi.FMHT = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.Ht)); ;
                        qi.FMTVA = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.Tva));
                        qi.FMMT = String.Format("{0:N0} XAF", qi.FMLignes.Sum(e => e.MT));

                        List<string> slst = new List<string>(); slst.Add("1801"); slst.Add("1815");
                        qi.SLignes = (from m in id where slst.Contains(m.Code) select m).ToList();
                        qi.SLHT = String.Format("{0:N0} XAF", qi.SLignes.Sum(e => e.Ht));
                        qi.SLTVA = String.Format("{0:N0} XAF", qi.SLignes.Sum(e => e.Tva));
                        qi.SLMT = String.Format("{0:N0} XAF", qi.SLignes.Sum(e => e.MT));

                        foreach (InvoiceDetails ids in qi.FMLignes) id.Remove(ids);
                        foreach (InvoiceDetails ids in qi.SLignes) id.Remove(ids);
                        qi.DTLignes = id;
                        qi.DTHT = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.Ht));
                        qi.DTTVA = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.Tva));
                        qi.DTMT = String.Format("{0:N0} XAF", qi.DTLignes.Sum(e => e.MT));

                        #endregion


                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\cashing_{0}_{1}_{2}.xml", bl, tkusr, type)))
                        {
                            sw.WriteLine(qi.ToXML());
                        }
                        return Ok(qi);
                    }
                    else
                        return NotFound(); 
                    #endregion

                }
                else
                    if (type == "go") //confirmation creation proforma. ici je cree direct la profoma. vu que cela ne rentre pas ds ma comptabilite
                    {
                         
                        using (var ctx = new VSOMEntities())
                        {
                            con = ctx.CONNAISSEMENTs.Where(s => s.IdClient.Value == 1 && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Traité" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                                    .FirstOrDefault<CONNAISSEMENT>();

                            if (con != null)
                            {
                                #region recuperation des element en pending sur le BL client
                                List<ELEMENT_FACTURATION> pendel = ctx.ELEMENT_FACTURATION.Where(el => el.IdBL == con.IdBL && el.StatutEF != "Annule" && el.StatutEF != "Facturé" && (el.EltFacture == "MF" || el.EltFacture == "Veh" || el.EltFacture == "BL")).ToList<ELEMENT_FACTURATION>();
                                pendingElmt = new List<InvoiceDetails>();
                                foreach (ELEMENT_FACTURATION el in pendel)
                                {
                                    if (el.PUEF != 0 && el.QTEEF != 0)
                                    {
                                        InvoiceDetails id = new InvoiceDetails();
                                        id.Ref = el.IdJEF.ToString();
                                        id.Prix = el.PUEF.Value;
                                        id.TvaCode = el.CodeTVA;
                                        id.TvaTaux = el.TauxTVA.Value;
                                        id.Libelle = el.LibEF;
                                        id.Qte = Math.Round(el.QTEEF.Value, 3, MidpointRounding.AwayFromZero);
                                        id.Unit = el.UnitEF;
                                        id.Ht = Math.Round((id.Prix * id.Qte), 0, MidpointRounding.AwayFromZero);
                                        id.Tva = Math.Round((id.Ht * (id.TvaTaux / 100)), 0, MidpointRounding.AwayFromZero); ;
                                        id.MT = id.Ht + id.Tva;
                                        id.Code = el.CodeArticle;
                                        pendingElmt.Add(id);
                                    }
                                }
                                #endregion

                                esc = ctx.ESCALEs.Where(s => s.IdEsc == con.IdEsc).FirstOrDefault<ESCALE>();
                                if (pendel.Count > 0)
                                {
                                    QuotationInvoice qi = new QuotationInvoice();
                                
                                    List<string> codeart = new List<string>();
                                    foreach (ELEMENT_FACTURATION e in pendel)
                                    {
                                        codeart.Add(e.CodeArticle);
                                    }
                                    List<ARTICLE> listArts = ctx.ARTICLEs.Where(s => codeart.Contains(s.CodeArticle.ToString())).OrderBy(s => s.LibArticle).ToList<ARTICLE>();
                                    List<string> fmlst = new List<string>(); fmlst.Add("1601"); fmlst.Add("2101"); fmlst.Add("1703");
                                    fmlst.Add("1702"); fmlst.Add("1701"); fmlst.Add("1820");
                                    List<string> slst = new List<string>(); slst.Add("1801"); slst.Add("1815");
                                    //idu 176 compte nova user soline
                                    #region mise en place des lignes proforma
                                    List<ELEMENT_FACTURATION> _id = new List<ELEMENT_FACTURATION>();
                                    if (fm == 1) _id.AddRange((from m in pendel where fmlst.Contains(m.CodeArticle) select m).ToList());
                                    if (sl == 1) _id.AddRange((from m in pendel where slst.Contains(m.CodeArticle) select m).ToList());
                                   // if (dt == 1) _id.AddRange(qi.DTLignes);
                                    #endregion

                                    PROFORMA proforma = new PROFORMA();
                                    proforma.AIFP = "";
                                    proforma.DCFP = DateTime.Now;
                                    proforma.IdArm = esc.IdArm;
                                    proforma.AIFP = "Created by soline";
                                    proforma.StatutFP = "O";
                                    proforma.IdBL = con.IdBL;
                                    proforma.IdU = 176;
                                    proforma.IdClient = con.IdClient;
                                    proforma.ClientFacture = "CLIENT COMPTANT";
                                    proforma.MHT = Convert.ToInt32(_id.Sum(elt => Math.Abs(Math.Round((Math.Round(elt.QTEEF.Value, 3, MidpointRounding.AwayFromZero) * elt.PUEF.Value), 0, MidpointRounding.AwayFromZero))));
                                    proforma.MTVA = Convert.ToInt32(_id.Sum(elt => Math.Abs(Math.Round((Math.Round((Math.Round(elt.QTEEF.Value, 3, MidpointRounding.AwayFromZero) * elt.PUEF.Value), 0, MidpointRounding.AwayFromZero) * (elt.TauxTVA.Value / 100)), 0, MidpointRounding.AwayFromZero))));
                                    proforma.MTTC = proforma.MHT + proforma.MTVA;

                                    foreach (ARTICLE art in listArts)
                                    { 
                                        if (proforma.AIFP != "")
                                        {
                                            proforma.AIFP = proforma.AIFP + " - " + art.LibArticle;
                                        }
                                        else
                                        {
                                            proforma.AIFP = art.LibArticle;
                                        }
                                    }
                                    ctx.PROFORMAs.Add(proforma);
                                    ctx.SaveChanges();

                                    StringBuilder msgVehEnProforma = new StringBuilder();

                                    foreach (ELEMENT_FACTURATION elt in _id)
                                    {
                                        LIGNE_PROFORMA ligne = new LIGNE_PROFORMA();
                                        ligne.IdJEF = elt.IdJEF;
                                        ligne.IdFP = proforma.IdFP;
                                        ligne.StatutLP = "O";

                                        /*var matchedElt = (from e in dcAcc.GetTable<ELEMENT_FACTURATION>()
                                                          where e.IdJEF == elt.IdElt
                                                          select e).SingleOrDefault<ELEMENT_FACTURATION>();*/

                                        ligne.PUEF = elt.PUEF;
                                        ligne.QTEEF = elt.QTEEF;
                                        ligne.TauxTVA = elt.TauxTVA;

                                        /*if (matchedElt.VEHICULE != null && matchedElt.VEHICULE.CUBAGE_VEHICULE.Count(cb => !cb.DateVal.HasValue) != 0 && (matchedElt.LibEF.Contains("Manutention") || matchedElt.LibEF.Contains("Séjour Parc Auto")))
                                        {
                                            msgVehEnProforma.Append(matchedElt.VEHICULE.NumChassis + " " + matchedElt.VEHICULE.DescVeh).Append(Environment.NewLine);
                                        }

                                        if (matchedElt.StatutEF == "Facturé")
                                        {
                                            throw new ApplicationException("Cet élément de facture a déjà été facturé : " + matchedElt.LibEF + ".\nEchec de création de la proforma");
                                        }
                                        */
                                        elt.StatutEF = "Proforma";
                                        ctx.LIGNE_PROFORMA.Add(ligne);
                                        
                                    }
                                    ctx.SaveChanges();
                                    /*if (msgVehEnProforma.ToString().Trim() != "")
                                    {
                                        throw new ApplicationException("Les véhicules suivants : \n" + msgVehEnProforma.ToString() + " Sont en cubage. Veuillez au préalable valider les cubages des véhicules concernés");
                                    }*/
                                    return Ok(proforma.IdFP.ToString());
                                }
                                else { return NotFound(); }
                            }
                            else
                            {
                                return NotFound();
                            }
                             

                        }
                        //
                    }
                    else
                        return NotFound();
               
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_cashing_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, type, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    //sw.WriteLine("message : " + ex.Message + " -------- innerexception : " + ex.StackTrace);
                    sw.WriteLine("message exception: " + ex.Message); sw.WriteLine("innerexception : " + ex.InnerException); sw.WriteLine("StackTrace : " + ex.StackTrace);
                }

                var message = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Demande non traitée : Veuillez ressayer.")
                };
                throw new HttpResponseException(message);
            }
        }

        [Authorize]
        [HttpGet]
        public IHttpActionResult GetVin(string bl, string vin, string type)
        {
            BL blview = null;
            DateTime startdate = DateTime.Parse("1/1/2017");
            //le token devrai permettre de savoir el type de client entreprise ou client et gerer la requete en consequence
            try
            {
                //var identity = (ClaimsIdentity)User.Identity;
                //int idclient = int.Parse(tok);
                using (var ctx = new VSOMEntities())
                {
                    CONNAISSEMENT con = null; //je force que le client qui peut verifier ce bl doit etre comptant
                    con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.IdClient.Value == 1 && s.DCBL.Value >= startdate && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Traité" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                            .FirstOrDefault<CONNAISSEMENT>();
                         
                    if (con != null)
                    {
                        if (type == "obl") //demande particulier controle dexistance de BL
                        {

                            //selection d'un index aleatoire de chassis
                            var rand = new Random();
                            int indveh = rand.Next(con.VEHICULEs.Count);
                            List<VEHICULE> lveh = con.VEHICULEs.ToList<VEHICULE>();

                            VEHICULE veh = lveh[indveh];
                            blview = new BL() { NumBl = "1", Lib = veh.NumChassis.Remove(0, 6) };
                        }

                        if (type == "ovin") //controle de chassis
                        {
                            List<VEHICULE> lveh = con.VEHICULEs.ToList<VEHICULE>(); string _vin = vin.ToUpper();
                            VEHICULE veh = lveh.SingleOrDefault(s => s.NumChassis == _vin);
                            if (veh == null)
                            {
                                blview = new BL() { NumBl = bl, Lib = "0" };
                            }
                            else
                            {
                                ESCALE esc = ctx.ESCALEs.Where(s => s.IdEsc == con.IdEsc).FirstOrDefault<ESCALE>();
                                blview = new BL()
                                {
                                    NumBl = bl,
                                    Lib = "1",
                                    Consignee = con.ConsigneeBL,
                                    Notify = con.NotifyBL, NumMan=esc.NumManifestSydonia,
                                    FinFranchise = con.VEHICULEs.ToList<VEHICULE>()[0].FFVeh.Value.ToShortDateString(),
                                    Vehicules = new List<Cars>()
                                };

                                foreach (VEHICULE _veh in con.VEHICULEs)
                                {
                                    if (_veh.IdVehAP == null && veh.StatVeh != "Livré")
                                    {
                                        blview.Vehicules.Add(new Cars
                                        {
                                            Chassis = _veh.NumChassis,
                                            Description = _veh.DescVeh,
                                            Vol = _veh.VolCVeh.Value,
                                            Porte = _veh.VehPorte,
                                            Attelle = _veh.VehAttelle,
                                            Statut = _veh.StatVeh,
                                            Longueur = _veh.LongCVeh.Value,
                                            Largueur = _veh.LargCVeh.Value,
                                            Charge = _veh.VehChargé,
                                            FinFranchise = _veh.FFVeh.Value
                                            //FinSejour = veh.FSVeh.Value,
                                            //SortiePrevu = veh.DSPVeh.Value
                                        });
                                    }
                                }
                            }
                        }

                        
                    }
                }

                if (blview == null)
                {
                    var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                    {
                        Content = new StringContent("Aucun connaissement trouvé")
                    };
                    throw new HttpResponseException(message);
                    // return NotFound();
                }


                return Ok(blview);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_vin_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, type, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.InnerException); sw.WriteLine("StackTrace : " + ex.StackTrace);
                }


                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Demande non traitée : ")
                };
                throw new HttpResponseException(message);
                //return NotFound();
            }
        }

        /// <summary>
        /// permet definir le access code qui sera utiliser pour controler le token des requete http avec le web api
        /// methode appeler uniquement par le controller query/Auth de la vue
        /// </summary>
        /// <param name="code"></param>
        /// <param name="compte"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Navy/Svr/")]
        public IHttpActionResult GetAccess(string code, int compte, int client)
        {
            try
            {

                using (var ctx = new RM_VSOMEntities1())
                {
                    ctx.acces.Add(new acces{ CODE=code, ID_COMPTE=compte, REC_TIME=DateTime.Now, USED=0 });
                    
                    ctx.SaveChanges();
                }
                return Ok("insert");
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_access_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", code, compte, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year,DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.StackTrace);
                }

                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Demande non traitée : ")
                };
                throw new HttpResponseException(message);
            }
        }

        [Authorize]
        [HttpGet]
        public IHttpActionResult GetRecap(int owner, string from, string to)
        {
            try
            {
                List<Requests> lstreq = new List<Requests>();
                DateTime debut = DateTime.Parse(from); DateTime fin = DateTime.Parse(to);
                using (var ctx = new RM_VSOMEntities1())
                {
                    List<requetes> lst = ctx.requetes.Where(s => s.IDCOMPTES == owner && s.REC_TIME >= debut && s.REC_TIME <= fin).ToList<requetes>();
                    if (lst != null)
                    {
                        foreach (requetes req in lst)
                        {
                            lstreq.Add(new Requests { date=req.REC_TIME.Value.ToShortDateString(), description=req.LIBELLE, num=req.ID.ToString() , statut=req.ETAT });
                        }
                    }
                }
                if (lstreq.Count == 0)
                    return NotFound();
                else
                return Ok(lstreq);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_recap_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", owner, to, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.StackTrace);
                }

                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Demande non traitée : ")
                };
                throw new HttpResponseException(message);
            }
        }
    }
}

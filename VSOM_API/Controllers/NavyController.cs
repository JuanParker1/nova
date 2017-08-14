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
        [AllowAnonymous]
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
                    if (type == "quote" || type=="inv")
                    {
                        //if (!tok.Equals("1"))
                            con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.IdClient.Value == idclient&& s.DCBL.Value>=startdate && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
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
                        blview = new BL()
                        {
                            Adresse = con.AdresseBL,
                            CodeTva = con.CodeTVA,
                            Consignee = con.ConsigneeBL,
                            Notify = con.NotifyBL,
                            NumBl = con.NumBL,
                            Statut = con.StatutBL,
                            FinFranchise=con.VEHICULEs.ToList<VEHICULE>()[0].FFVeh.Value.ToShortDateString(),
                            Vehicules = new List<Cars>()
                        };

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

                        if (type == "inv")
                        {
                            List<FACTURE> invs = (from f in ctx.FACTUREs join p in ctx.PROFORMAs on f.IdFP equals p.IdFP where p.IdBL == con.IdBL select f).ToList<FACTURE>();
                            blview.Invoices = new List<Invoices>();
                            foreach (FACTURE fact in invs)
                            {
                                blview.Invoices.Add(new Invoices { Date = fact.DCFD.Value.ToShortDateString(), HT = fact.MHT.Value, MTTC = fact.MTTC.Value, TVA = fact.MTVA.Value, Statut = fact.StatutFD == "P" ? "Payée" : (fact.StatutFD=="O"? "En cours" :"Annulée"), Num = fact.IdDocSAP.Value.ToString() });
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
                                blview.Payments.Add(new Payments { Date = obj.DatePay.Value.ToShortDateString(), MTTC = obj.MRPay.Value, Statut = obj.StatutPay=="O"? "" :"Annulé" , Num = obj.IdPaySAP.Value.ToString() });
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
        /// <param name="tok"></param>
        /// <returns></returns>
        
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetQuotation(string bl, string chassis, string date, string type , string tok, int owner )
        {
            //if (!ModelState.IsValid)  return BadRequest("Invalid data.");
            List<InvoiceDetails> details = null; List<InvoiceDetails> pendingElmt = null; StringBuilder sb = new StringBuilder();

            try
            {
                int idclient = int.Parse(tok); DateTime startdate = DateTime.Parse("1/1/2017"); string _chassis = string.Empty; DateTime dateFin = DateTime.Parse(date);
                CONNAISSEMENT con = null;
                using (var ctx = new VSOMEntities())
                {

                    con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.DCBL.Value >= startdate && s.IdClient == idclient && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
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
                                throw new ApplicationException("Veuillez entrer une date correcte");
                            }

                            //recupère les element non payés
                            #region recupère les element non payés
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
                                    id.Qte = el.QTEEF.Value;
                                    id.Unit = el.UnitEF;
                                    id.Ht = Math.Round((id.Prix * id.Qte),0, MidpointRounding.AwayFromZero);
                                    id.Tva = Math.Round((id.Ht * (id.TvaTaux/100)), 0, MidpointRounding.AwayFromZero); ;
                                    id.MT = id.Ht + id.Tva;
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

                                        ARTICLE articleDeboursPADPenalite = (from art in articles where art.CodeArticle == 1815 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpDeboursPADPenalite = null;

                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
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
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN2" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN1" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                        }
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
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

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

                                                #endregion

                                            }
                                            else
                                            {
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9)
                                                {
                                                    #region niveau 9jours
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX 
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    details.Add(eltFactDeboursPADParcAuto);

                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20)
                                                {
                                                    #region niveau 9 + 20 jours

                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - 9;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - 9;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion

                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20 + 30)
                                                {
                                                    #region 9 + 20 + 30
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = 20;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = 20;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20;
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

                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20;
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

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = 20;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = 20;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20 + 30).ToShortDateString();

                                                    eltFactSejourParcAutoN3.Qte = 30;
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

                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20 + 30).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qte = 30;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20 - 30;
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

                                                    eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20 - 30;
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
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                //AH  "Pénalité de stationnement
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

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
                                                #endregion
                                            }
                                            else
                                            {

                                                // Gestion pour les véhicules suivant le flux normal
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9)
                                                {
                                                    #region niveau 1
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    // eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    details.Add(eltFactDeboursPADParcAuto);
                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20)
                                                {
                                                    #region niveau 2
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20 + 30)
                                                {
                                                    #region niveau 3
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement 
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
                                                    #region niveau 4
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);



                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 30) ? 30 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement 
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3); ;

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltFactSejourParcAutoN3.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte + eltFactSejourParcAutoN4.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                    eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                    details.Add(eltFactSejourParcAutoN4);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltFactDeboursPADParcAutoN3.Qte;
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

                                        ARTICLE articleDeboursPADPenalite = (from art in articles where art.CodeArticle == 1815 select art).FirstOrDefault<ARTICLE>();
                                        LIGNE_PRIX lpDeboursPADPenalite = null;

                                        if (matchedVehicule.StatutCVeh == "U")
                                        {
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VU3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
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
                                            if (matchedVehicule.VolCVeh >= 50)
                                            {
                                                lpSejourParcAuto = articleSejourParcAuto.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                                lpDeboursPADPenalite = articleDeboursPADPenalite.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.LP == "VN3" && lp.DDLP <= dte && lp.DFLP >= dte);
                                            }
                                            else if (matchedVehicule.VolCVeh >= 16)
                                            {
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

                                        bool isEltsNotFree = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && (ef.CodeArticle == "1801") && (ef.StatutEF == "Proforma" || ef.IdFD != null))
                                                              .ToList<ELEMENT_FACTURATION>().Count != 0;

                                        isEltsNotFree = false;
                                        if (!isEltsNotFree)
                                        {
                                            #region  bloc bl element facture empty
                                            //applique au VAE
                                            int nbrvae = ctx.ELEMENT_FACTURATION.Where(ef => ef.IdVeh == matchedVehicule.IdVeh && ef.CodeArticle == "1605").Count();
                                            if (nbrvae != 0)
                                            {
                                                #region VAE
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                eltFactDeboursPADParcAuto.TvaCode = articleDeboursPADPenalite.CodeTVA;// "TVAEX";
                                                //eltFactDeboursPADParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                eltFactDeboursPADParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                details.Add(eltFactDeboursPADParcAuto);

                                                #endregion

                                            }
                                            else
                                            {
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9)
                                                {
                                                    #region niveau 9jours
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAuto.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();
                                                    eltFactSejourParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAuto.Qte = dateFin.Date < matchedVehicule.FFVeh ? 0 : (dateFin.Date - matchedVehicule.FFVeh.Value).Days;
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    details.Add(eltFactDeboursPADParcAuto);

                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20)
                                                {
                                                    #region niveau 9 + 20 jours

                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - 9;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - 9;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion

                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20 + 30)
                                                {
                                                    #region 9 + 20 + 30
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = 20;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = 20;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20;
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

                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20;
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

                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();

                                                    eltFactSejourParcAutoN1.Qte = 9;
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

                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Qte = 9;
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactSejourParcAutoN2.Qte = 20;
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

                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN2.Qte = 20;
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20 + 30).ToShortDateString();

                                                    eltFactSejourParcAutoN3.Qte = 30;
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays(9 + 20 + 30).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN3.Qte = 30;
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactSejourParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20 - 30;
                                                    eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                    eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                    details.Add(eltFactSejourParcAutoN4);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN4.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + matchedVehicule.FFVeh.Value.AddDays(1 + 9 + 20 + 30).ToShortDateString() + " - " + matchedVehicule.FFVeh.Value.AddDays((dateFin - matchedVehicule.FFVeh.Value).Days).ToShortDateString();

                                                    eltFactDeboursPADParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - 9 - 20 - 30;
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
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                //AH  "Pénalité de stationnement
                                                eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                details.Add(eltFactSejourParcAuto);

                                                InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                //eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                eltFactDeboursPADParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP.Value * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                //AH Débours PAD : Pénalité de stationnement
                                                eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP; ;
                                                eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                details.Add(eltFactDeboursPADParcAuto);
                                                #endregion
                                            }
                                            else
                                            {

                                                // Gestion pour les véhicules suivant le flux normal
                                                double derogation = (con.BLIL == "Y" || con.BLGN == "Y") ? 0.75 : 0;
                                                if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9)
                                                {
                                                    #region niveau 1
                                                    InvoiceDetails eltFactSejourParcAuto = new InvoiceDetails();

                                                    eltFactSejourParcAuto.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAuto.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAuto.TvaTaux = eltFactSejourParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAuto.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAuto.Qte)).ToShortDateString();
                                                    eltFactSejourParcAuto.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAuto.Ht = eltFactSejourParcAuto.Prix * eltFactSejourParcAuto.Qte;
                                                    eltFactSejourParcAuto.Tva = Math.Round((eltFactSejourParcAuto.Ht * eltFactSejourParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAuto.MT = eltFactSejourParcAuto.Ht + eltFactSejourParcAuto.Tva;
                                                    details.Add(eltFactSejourParcAuto);

                                                    InvoiceDetails eltFactDeboursPADParcAuto = new InvoiceDetails();

                                                    eltFactDeboursPADParcAuto.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAuto.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA; //TVAEX
                                                    //eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAuto.Qte = dateFin.Date < finAncienSejour ? 0 : (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) <= 9) ? (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAuto.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAuto.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAuto.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAuto.Ht = eltFactDeboursPADParcAuto.Prix * eltFactDeboursPADParcAuto.Qte;
                                                    eltFactDeboursPADParcAuto.Tva = Math.Round((eltFactDeboursPADParcAuto.Ht * eltFactDeboursPADParcAuto.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAuto.MT = eltFactDeboursPADParcAuto.Ht + eltFactDeboursPADParcAuto.Tva;
                                                    details.Add(eltFactDeboursPADParcAuto);
                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20)
                                                {
                                                    #region niveau 2
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase.Value == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (dateFin - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);
                                                    #endregion
                                                }
                                                else if ((dateFin - matchedVehicule.FFVeh.Value).Days <= 9 + 20 + 30)
                                                {
                                                    #region niveau 3
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);

                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement 
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
                                                    #region niveau 4
                                                    InvoiceDetails eltFactSejourParcAutoN1 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN1.Prix = lpSejourParcAuto.PU1LP.Value - lpSejourParcAuto.PU1LP.Value * derogation;
                                                    eltFactSejourParcAutoN1.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN1.TvaTaux = eltFactSejourParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH "Pénalité de stationnement
                                                    eltFactSejourParcAutoN1.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN1.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN1.Ht = eltFactSejourParcAutoN1.Prix * eltFactSejourParcAutoN1.Qte;
                                                    eltFactSejourParcAutoN1.Tva = Math.Round((eltFactSejourParcAutoN1.Ht * eltFactSejourParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN1.MT = eltFactSejourParcAutoN1.Ht + eltFactSejourParcAutoN1.Tva;
                                                    details.Add(eltFactSejourParcAutoN1);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN1 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN1.Prix = lpDeboursPADPenalite.PU1LP.Value - lpDeboursPADPenalite.PU1LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN1.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN1.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF) < 9) ? 9 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN1.Libelle = articleDeboursPADPenalite.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN1.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN1.Ht = eltFactDeboursPADParcAutoN1.Prix * eltFactDeboursPADParcAutoN1.Qte;
                                                    eltFactDeboursPADParcAutoN1.Tva = Math.Round((eltFactDeboursPADParcAutoN1.Ht * eltFactDeboursPADParcAutoN1.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN1.MT = eltFactDeboursPADParcAutoN1.Ht + eltFactDeboursPADParcAutoN1.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN1);

                                                    InvoiceDetails eltFactSejourParcAutoN2 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN2.Prix = lpSejourParcAuto.PU2LP.Value - lpSejourParcAuto.PU2LP.Value * derogation;
                                                    eltFactSejourParcAutoN2.TvaCode = con.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN2.TvaTaux = eltFactSejourParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN2.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN2.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN2.Ht = eltFactSejourParcAutoN2.Prix * eltFactSejourParcAutoN2.Qte;
                                                    eltFactSejourParcAutoN2.Tva = Math.Round((eltFactSejourParcAutoN2.Ht * eltFactSejourParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN2.MT = eltFactSejourParcAutoN2.Ht + eltFactSejourParcAutoN2.Tva;
                                                    details.Add(eltFactSejourParcAutoN2);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN2 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN2.Prix = lpDeboursPADPenalite.PU2LP.Value - lpDeboursPADPenalite.PU2LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN2.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    // eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN2.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 20) ? 20 - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Débours PAD : Pénalité de stationnement
                                                    eltFactDeboursPADParcAutoN2.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN2.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN2.Ht = eltFactDeboursPADParcAutoN2.Prix * eltFactDeboursPADParcAutoN2.Qte;
                                                    eltFactDeboursPADParcAutoN2.Tva = Math.Round((eltFactDeboursPADParcAutoN2.Ht * eltFactDeboursPADParcAutoN2.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN2.MT = eltFactDeboursPADParcAutoN2.Ht + eltFactDeboursPADParcAutoN2.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN2);



                                                    InvoiceDetails eltFactSejourParcAutoN3 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN3.Prix = lpSejourParcAuto.PU3LP.Value - lpSejourParcAuto.PU3LP.Value * derogation;
                                                    eltFactSejourParcAutoN3.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN3.TvaTaux = eltFactSejourParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN3.Qte = (eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) < 30) ? 30 - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) : 0;
                                                    //AH Pénalité de stationnement 
                                                    eltFactSejourParcAutoN3.Libelle = articleSejourParcAuto.LibArticle + "Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN3.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN3.Ht = eltFactSejourParcAutoN3.Prix * eltFactSejourParcAutoN3.Qte;
                                                    eltFactSejourParcAutoN3.Tva = Math.Round((eltFactSejourParcAutoN3.Ht * eltFactSejourParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN3.MT = eltFactSejourParcAutoN3.Ht + eltFactSejourParcAutoN3.Tva;
                                                    details.Add(eltFactSejourParcAutoN3); ;

                                                    InvoiceDetails eltFactDeboursPADParcAutoN3 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN3.Prix = lpDeboursPADPenalite.PU3LP.Value - lpDeboursPADPenalite.PU3LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN3.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN3.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte;
                                                    //AH Débours PAD : Pénalité de stationnement 
                                                    eltFactDeboursPADParcAutoN3.Libelle = articleDeboursPADPenalite.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactDeboursPADParcAutoN1.Qte + eltFactDeboursPADParcAutoN2.Qte + eltFactDeboursPADParcAutoN3.Qte)).ToShortDateString();
                                                    eltFactDeboursPADParcAutoN3.Unit = lpDeboursPADPenalite.UniteLP;
                                                    eltFactDeboursPADParcAutoN3.Ht = eltFactDeboursPADParcAutoN3.Prix * eltFactDeboursPADParcAutoN3.Qte;
                                                    eltFactDeboursPADParcAutoN3.Tva = Math.Round((eltFactDeboursPADParcAutoN3.Ht * eltFactDeboursPADParcAutoN3.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactDeboursPADParcAutoN3.MT = eltFactDeboursPADParcAutoN3.Ht + eltFactDeboursPADParcAutoN3.Tva;
                                                    details.Add(eltFactDeboursPADParcAutoN3);

                                                    InvoiceDetails eltFactSejourParcAutoN4 = new InvoiceDetails();

                                                    eltFactSejourParcAutoN4.Prix = lpSejourParcAuto.PU4LP.Value - lpSejourParcAuto.PU4LP.Value * derogation;
                                                    eltFactSejourParcAutoN4.TvaCode = matchedVehicule.CONNAISSEMENT.BLIL == "Y" ? "TVAEX" : (con.CodeTVA == "TVAEX" ? "TVAEX" : con.CLIENT.CodeTVA);
                                                    eltFactSejourParcAutoN4.TvaTaux = eltFactSejourParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactSejourParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU4LP * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpSejourParcAuto.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpSejourParcAuto.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactSejourParcAutoN1.Qte - eltFactSejourParcAutoN2.Qte - eltFactSejourParcAutoN3.Qte;
                                                    //AH Pénalité de stationnement
                                                    eltFactSejourParcAutoN4.Libelle = articleSejourParcAuto.LibArticle + " Chassis N° " + matchedVehicule.NumChassis + " : " + finAncienSejour.AddDays(1 + Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte)).ToShortDateString() + " - " + finAncienSejour.AddDays(Convert.ToInt32(eltFactSejourParcAutoN1.Qte + eltFactSejourParcAutoN2.Qte + eltFactSejourParcAutoN3.Qte + eltFactSejourParcAutoN4.Qte)).ToShortDateString();
                                                    eltFactSejourParcAutoN4.Unit = lpSejourParcAuto.UniteLP;
                                                    eltFactSejourParcAutoN4.Ht = eltFactSejourParcAutoN4.Prix * eltFactSejourParcAutoN4.Qte;
                                                    eltFactSejourParcAutoN4.Tva = Math.Round((eltFactSejourParcAutoN4.Ht * eltFactSejourParcAutoN4.TvaTaux), 0, MidpointRounding.AwayFromZero); ;
                                                    eltFactSejourParcAutoN4.MT = eltFactSejourParcAutoN4.Ht + eltFactSejourParcAutoN4.Tva;
                                                    details.Add(eltFactSejourParcAutoN4);

                                                    InvoiceDetails eltFactDeboursPADParcAutoN4 = new InvoiceDetails();

                                                    eltFactDeboursPADParcAutoN4.Prix = lpDeboursPADPenalite.PU4LP.Value - lpDeboursPADPenalite.PU4LP.Value * derogation;
                                                    eltFactDeboursPADParcAutoN4.TvaCode = lpDeboursPADPenalite.ARTICLE.CodeTVA;//TVAEX
                                                    //eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
                                                    eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : con.CodeTVA == "TVAEX" ? 0 : (con.CLIENT.CodeTVA == "TVAEX" ? 0 : 0.1925f);

                                                    eltFactDeboursPADParcAutoN4.Qte = (dateFin.Date - matchedVehicule.FFVeh.Value).Days - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU4LP * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU4LP * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU3LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU2LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltSejourCalcules.Where(el => el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * (1 - derogation) || el.PUEFBase == lpDeboursPADPenalite.PU1LP.Value * 1.6 * (1 - derogation)).Sum(el => el.QTEEF.Value) - eltFactDeboursPADParcAutoN1.Qte - eltFactDeboursPADParcAutoN2.Qte - eltFactDeboursPADParcAutoN3.Qte;
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

                    //verifie si la validation correspond a l'existant

                    var req = new requetes
                    {
                        ETAT = "Pending",
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
                       mail.Body="Une nouvelle quotation est validée. Veuillez proceder a son traitement";
                        SmtpServer.Credentials = new System.Net.NetworkCredential("automate@socomar-cameroun.com", "Socom@r17!");
                        SmtpServer.Send(mail);

                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\quotation_valid_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", bl, chassis, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                        {
                            sw.WriteLine("ok validation : " + req.ID + " quotation : " + req.ID_QUOTATION);
                        }
                        return Ok(req.ID_QUOTATION);
                    }
                }
                else
                {
                    if (details == null || details.Count == 0)
                    { return NotFound(); }
                    else
                    {

                        QuotationInvoice qi = new QuotationInvoice();

                        qi.HT = details.Sum(e => e.Ht) + pendingElmt.Sum(e => e.Ht);
                        qi.TVA = details.Sum(e => e.Tva) + pendingElmt.Sum(e => e.Tva);
                        qi.MT = details.Sum(e => e.MT) + pendingElmt.Sum(e => e.MT);
                        qi.Lignes = details;
                        qi.OLignes = pendingElmt;

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
                               HT = int.Parse(qi.HT.ToString()),
                               TVA = int.Parse(qi.TVA.ToString()),
                               TTC = int.Parse(qi.MT.ToString())

                           };
                        using (var ctx = new RM_VSOMEntities1())
                        {

                            ctx.quotation.Add(quot);
                            ctx.SaveChanges();
                            qi.Ref = quot.ID.ToString();
                        }

                        //string str = qi.ToXML();
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\quotation_{0}_{1}.xml", qi.Ref, bl)))
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

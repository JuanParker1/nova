using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VSOM_API.Models;
namespace VSOM_API.Controllers
{
    public class Vsom0Controller : ApiController
    {
        /*
         * list de chassis par bl
         */ 
        public IHttpActionResult GetBl(string bl, string token)
        {
            BL blview = null;
            //le token devrai permettre de savoir el type de client entreprise ou client et gerer la requete en consequence
            try
            {
                using (var ctx = new VSOMEntities())
                {
                    CONNAISSEMENT con = null;
                    if (token != "1")
                        con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.IdClient.ToString() == token && s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                            .FirstOrDefault<CONNAISSEMENT>();
                    if (token == "1")
                        con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                                               .FirstOrDefault<CONNAISSEMENT>();
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
                            Vehicules = new List<Cars>()
                        };

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
                }


                if (blview == null)
                {
                    return NotFound();
                }

                return Ok(blview);
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        public IHttpActionResult PostQuotation(QuotationViewModel quotation)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data.");

            using (var ctx = new VSOMEntities())
            {
                /*ctx.Students.Add(new Student()
                {
                    StudentID = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName
                });

                ctx.SaveChanges();
                 */ 
            }

            return Ok();
        }

        public IHttpActionResult GetQuotation(string bl, string chassis, string date)
        {
            //if (!ModelState.IsValid)  return BadRequest("Invalid data.");
            List<InvoiceDetails> details=null;
            try
            {
                using (var ctx = new VSOMEntities())
                {
                    CONNAISSEMENT con = ctx.CONNAISSEMENTs.Include("VEHICULEs").Where(s => s.NumBL == bl && s.StatutBL != "Initié" && s.StatutBL != "Cloturé" && s.TypeBL == "R")
                                        .FirstOrDefault<CONNAISSEMENT>();
                    if (con != null)
                    {

                        details = new List<InvoiceDetails>();
                        DateTime dateFin = DateTime.Parse(date);
                        List<VEHICULE> _lst = con.VEHICULEs.ToList();

                        if (chassis == "undefined" || chassis == "null")
                        {
                            #region MyRegion all
                            

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
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
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

                                    List<ELEMENT_FACTURATION> eltSejourCalcules = ctx.ELEMENT_FACTURATION.Where(ef=> ef.IdVeh == matchedVehicule.IdVeh && 
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
                                        eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                            eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                            eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;

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
                                                eltFactDeboursPADParcAuto.TvaTaux = eltFactDeboursPADParcAuto.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN1.TvaTaux = eltFactDeboursPADParcAutoN1.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN2.TvaTaux = eltFactDeboursPADParcAutoN2.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN3.TvaTaux = eltFactDeboursPADParcAutoN3.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                                                eltFactDeboursPADParcAutoN4.TvaTaux = eltFactDeboursPADParcAutoN4.TvaCode == "TVAEX" ? 0 : 0.1925f;
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
                            else { return NotFound(); }
                        }
                    }
                     
                }

                if (details == null || details.Count == 0)
                { return NotFound(); }
                else
                {
                    QuotationInvoice qi = new QuotationInvoice();
                    qi.HT = details.Sum(e => e.Ht);
                    qi.TVA = details.Sum(e => e.Tva);
                    qi.MT = details.Sum(e => e.MT);
                    qi.Lignes = details;
                    return Ok(qi);
                }
                

            }
            catch (Exception ex)
            {
                return NotFound();
            }
             

        }

        
    }
}

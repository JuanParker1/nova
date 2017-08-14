using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VSOM_API.Controllers
{
    public class RHController : ApiController
    {
        //[AllowAnonymous]
        //[HttpGet]
        //[Route("api/RH/Test/")]
        //public IHttpActionResult Test()
        //{ return Ok("ok"); }

       
        [AllowAnonymous]
        [HttpGet]
        public IHttpActionResult GetRecapPointage(int idsvc , int idperiode)
        {
            try
            {
                List<POINTAGES> lst = null;
                double diff=0; int heurNormal=0; int t120=0; int t130=0; int t140=0;
                int t150 = 0; int t200 = 0; int thm = 0; int nbrheurpause = 0;
                string dayname; bool estferie = false; bool abateau=false;
                using (var ctx = new INHOUSEAPPEntities1 ())
                {
                    lst = ctx.POINTAGES.Where(s => s.ID_PERIODE == idperiode && s.PRM_EMPL.IDSERVICE==idsvc).OrderBy(s => s.IDAUTO).ToList<POINTAGES>();

                    //pour chaque ligne , je recalcule le diff et le dispache puis je retour un objet pointage mofidier
                    foreach (POINTAGES pt in lst)
                    {
                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0; estferie = false; abateau = false;

                        dayname = pt.JOUR.DayOfWeek.ToString();
                        //recupère le statut de la semaine
                        #region statut jour
                        PRM_PROGRAM prog = ctx.PRM_PROGRAM.Where(s => s.JOUR == pt.JOUR).FirstOrDefault();
                        if (prog == null)
                        {
                            PRM_SVC_HOUR psh = (from m in ctx.PRM_SVC_HOUR join empl in ctx.PRM_EMPL on m.IDSERVICE equals empl.IDSERVICE where m.OP==0 select m).FirstOrDefault();
                            estferie = false;
                            abateau = false;
                            if (psh.IDSERVICE == 3 || psh.IDSERVICE == 2) nbrheurpause = 2; else nbrheurpause = 1;
                        }
                        else
                        {
                            PRM_SVC_HOUR psh = (from m in ctx.PRM_SVC_HOUR join empl in ctx.PRM_EMPL on m.IDSERVICE equals empl.IDSERVICE where m.OP == prog.OP select m).FirstOrDefault();
                            estferie = prog.FERIE == 1 ? true : false;
                            abateau = prog.OP == 1 ? true : false;
                            if (psh.IDSERVICE == 3 || psh.IDSERVICE == 2) nbrheurpause = 2; else nbrheurpause = 1;
                        }

                        #endregion
                         

                        if (dayname == "Sunday" || dayname=="Dimanche")
                        {
                            #region dimanche
                            if (estferie)
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region dimanche ferie jour

                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }
                                    else
                                    if (pt.MHS == pt.SHE)//pas de pausse
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    if (pt.MHS != pt.SHE)//avec pausse
                                    {
                                        diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    //tous a 140
                                    
                                        pt.DIFF = (int)diff;
                                        pt.T140 =  (int)diff;
                                        pt.T120 = pt.T130 = pt.T150 = pt.T200 = pt.HM= 0;
                                    
                                    #endregion
                                }
                                //TODO: controler que dans la nuit il ny a pas de a pause
                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region dimanche ferie nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //heure de travail porte a 50%
                                        pt.DIFF = (int)diff;
                                        pt.T150 = (int)diff;
                                        pt.T120 = pt.T130 = pt.T140 = pt.T200 =pt.HM= 0;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = (int)diff;
                                        pt.T150 = 8; pt.T140 = 5; pt.HM = abateau == true ? 1 : 0;
                                        pt.T120 = pt.T130 = pt.T200 = 0;
                                        //8 a 50% et 5 a 40% + si bateau => heuremajoree sinon pas dheur majoree ,
                                    }

                                    
                                    #endregion
                                }
                            }
                            else
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region dimanche normal jour
                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }

                                    if (pt.MHS == pt.SHE)//pas de pausse
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    if (pt.MHS != pt.SHE)//avec pausse
                                    {
                                        diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    //tous a 140, moins heure de pause sil y a bateau
                                    if (diff > 0)
                                    {
                                        pt.DIFF = abateau == true ? (int)diff : (int)(diff - nbrheurpause);
                                        pt.T140 =  pt.DIFF;// (int)diff;
                                        pt.T120 = pt.T130 = pt.T150 = pt.T200 = pt.HM = 0;
                                    }
                                    else
                                    {
                                        pt.T120 = pt.T130 = pt.T150 = pt.T200 = pt.HM =pt.DIFF=pt.T140= 0;
                                    }
                                    #endregion
                                }

                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region dimanche normal nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //je lui porte le tous a 50% quil y ai navire ou pas
                                        pt.DIFF = (int)diff;
                                        pt.T150 = pt.DIFF;
                                        pt.T120 = pt.T130 = pt.T140 = pt.T200 = pt.HM = 0;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = (int)diff;
                                        pt.T150 = 8;
                                        pt.T140 = 4;
                                        pt.T130 = pt.T120 = 0;
                                        pt.HM = abateau == true ? 1 : 0;
                                    }
                                    #endregion
                                }
                            } 
                            #endregion
                        }
                        else if (dayname == "Samedi" || dayname == "Saterday")
                        {
                            if (estferie)
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region samedi ferie jour

                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }
                                    else
                                        if (pt.MHS == pt.SHE)//pas de pausse
                                        {
                                            diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        }
                                        else
                                            if (pt.MHS != pt.SHE)//avec pausse
                                            {
                                                diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                            }

                                    //tous a 140

                                    pt.DIFF = (int)diff;
                                    pt.T140 = (int)diff;
                                    pt.T120 = pt.T130 = pt.T150 = pt.T200 = pt.HM = 0;

                                    #endregion
                                }
                                //TODO: controler que dans la nuit il ny a pas de a pause
                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region samedi ferie nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //heure de travail porte a 50%
                                        pt.DIFF = (int)diff;
                                        pt.T150 = (int)diff;
                                        pt.T120 = pt.T130 = pt.T140 = pt.T200 = pt.HM = 0;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = 13;
                                        pt.T150 = 8;
                                        //recupère le statut des element dans la semaine
                                        #region statut pourcentage employé
                                        var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        #endregion
                                        //je repartie les 5h restant sur la semaine
                                        t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0;
                                        t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        int hrest = 5;// a repartir sur les taux en fonction du statut
                                        if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } }
                                        if (t130 < 8 && hrest > 0) { int r = 8 - t130; if (hrest > r) { pt.T130 = r; hrest = hrest - r; } else { pt.T130 = hrest; hrest = 0; } }
                                        if (hrest > 0) { pt.T140 = hrest; }

                                        pt.HM = abateau == true ? 1 : 0;
                                    }


                                    #endregion
                                }
                            }
                            else
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region samedi normal jour
                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }

                                    if (pt.MHS == pt.SHE)//pas de pausse
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    if (pt.MHS != pt.SHE)//avec pausse
                                    {
                                        diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    if (diff > 0) //parceque tous ce quil fait est HS
                                    {
                                        //on repartie sur la semaine
                                        #region statut pourcentage employé
                                        var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        #endregion
                                        //je repartie les 5h restant sur la semaine
                                        t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0;
                                        t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        int hrest = (int)diff;// a repartir sur les taux en fonction du statut
                                        if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } }
                                        if (t130 < 8 && hrest > 0) { int r = 8 - t130; if (hrest > r) { pt.T130 = r; hrest = hrest - r; } else { pt.T130 = hrest; hrest = 0; } }
                                        if (hrest > 0) { pt.T140 = hrest; }
                                        pt.DIFF = (int)diff;
                                        pt.HM = 0;
                                    }
                                    else
                                    {
                                        pt.DIFF = pt.T120 = pt.T130 = pt.T140 = pt.T150 = pt.HM = 0;
                                    }
                                     
                                    #endregion
                                }

                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region samedi normal nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        pt.HM = pt.T120 = pt.T130 = pt.T140 = pt.T200 = 0;
                                        pt.T150 = (int)diff;
                                        pt.DIFF = (int)diff;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = 13;
                                        pt.T150 = 8;
                                        //recupère le statut des element dans la semaine
                                        #region statut pourcentage employé
                                        var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        #endregion
                                        //je repartie les 5h restant sur la semaine
                                        t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0;
                                        t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        int hrest = 5;// a repartir sur les taux en fonction du statut
                                        if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } }
                                        if (t130 < 8 && hrest > 0) { int r = 8 - t130; if (hrest > r) { pt.T130 = r; hrest = hrest - r; } else { pt.T130 = hrest; hrest = 0; } }
                                        if (hrest > 0) { pt.T140 = hrest; } 
                                        pt.HM = abateau == true ? 1 : 0;
                                    }
                                    #endregion
                                }
                            } 
                        }
                        else
                        {
                            #region lundi a vendredi
                            if (estferie)
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region ferie jour
                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }

                                    if (pt.MHS == pt.SHE)//pas de pausse
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //heure de pause
                                        diff = abateau == true ? diff : (int)(diff - nbrheurpause);
                                    }

                                    if (pt.MHS != pt.SHE)//avec pausse
                                    {
                                        diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    if (diff > 0)
                                    {
                                        pt.DIFF = abateau == true ? (int)diff : (int)(diff - nbrheurpause);
                                        pt.T140 = pt.DIFF;
                                        pt.T120 = pt.T130 = pt.T150 = pt.HM = 0;
                                    }
                                    else
                                        pt.DIFF = pt.T120 = pt.T130 = pt.T140 = pt.T150 = pt.HM = 0;

                                    #endregion
                                }

                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region ferie nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //tous a 50%
                                        pt.T120 = pt.T130 = pt.T140 = pt.HM = 0;
                                        pt.T150 = (int)diff;
                                        pt.DIFF = (int)diff;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = 13;
                                        pt.T150 = 8;
                                        pt.T140 = 5;
                                        pt.T120 = pt.T130 = pt.HM = 0;
                                        ////recupère le statut des element dans la semaine
                                        //#region statut pourcentage employé
                                        //var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        //new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        //#endregion
                                        ////je repartie les 5h restant sur la semaine
                                        //t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        //t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0 ;
                                        //t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        //t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        //int hrest = 5;// a repartir sur les taux en fonction du statut
                                        //if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } } 
                                        //if(t130<8 && hrest>t120) {int r=8-t130; if(hrest>r){pt.T130=r;hrest=hrest-r;}else{pt.T130=hrest;hrest=0;}}
                                        //if( hrest>0){pt.T140=hrest;  }

                                        //pt.HM = abateau == true ? 1 : 0;
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                if (pt.SHIFT == 1)//jour
                                {
                                    #region normal jour
                                    if (pt.MHE == pt.SHS)//sans operation
                                    {
                                        diff = heurNormal = t120 = t130 = t140 = t150 = t200 = thm = 0;
                                    }

                                    if (pt.MHS == pt.SHE)//pas de pausse
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        //controle de pausse
                                        diff = abateau == true ? diff : (int)(diff - nbrheurpause);
                                    }

                                    if (pt.MHS != pt.SHE)//avec pausse
                                    {
                                        diff = Math.Round((pt.MHS.TotalHours - pt.MHE.TotalHours) + (pt.SHS.TotalHours - pt.SHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                    }

                                    if (diff > 8) //parcequil faut 8h avant davoir les hs
                                    {
                                        //on repartie sur la semaine
                                        #region statut pourcentage employé
                                        var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        #endregion
                                        //je repartie les 5h restant sur la semaine
                                        t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0;
                                        t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        int hrest = (int)diff - 8; ;// a repartir sur les taux en fonction du statut
                                        if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } }
                                        if (t130 < 8 && hrest > 0) { int r = 8 - t130; if (hrest > r) { pt.T130 = r; hrest = hrest - r; } else { pt.T130 = hrest; hrest = 0; } }
                                        if (hrest > 0) { pt.T140 = hrest; }
                                        pt.DIFF = (int)diff;
                                        pt.HM = 0;
                                    }
                                    else
                                    {
                                        pt.DIFF = pt.T120 = pt.T130 = pt.T140 = pt.T150 = pt.HM = 0;
                                    }
                                    #endregion
                                }

                                if (pt.SHIFT == 2) //nuit
                                {
                                    #region normal nuit
                                    if (pt.SHS >= pt.MHE) //pas la nuit complet
                                    {
                                        diff = Math.Round((pt.SHS.TotalHours - pt.MHE.TotalHours), 0, MidpointRounding.AwayFromZero);
                                        pt.HM = pt.T120 = pt.T130 = pt.T140 = pt.T200 = 0;
                                        pt.T150 = (int)diff;
                                        pt.DIFF = (int)diff;
                                    }
                                    else //nuit complete
                                    {
                                        diff = 13;
                                        pt.DIFF = 13;
                                        pt.T150 = 8;
                                        //recupère le statut des element dans la semaine
                                        #region statut pourcentage employé
                                        var query = ctx.Database.SqlQuery<EmplWeekStat_Result>("EmplWeekStat @idempl,@idweek",
                                        new SqlParameter("@idempl", pt.ID_EMPL), new SqlParameter("@idweek", pt.IDCAL)).ToList();

                                        #endregion
                                        //je repartie les 5h restant sur la semaine
                                        t120 = query[0].T120.HasValue == true ? query[0].T120.Value : 0;
                                        t130 = query[0].T130.HasValue == true ? query[0].T130.Value : 0;
                                        t140 = query[0].T140.HasValue == true ? query[0].T140.Value : 0;
                                        t150 = query[0].T150.HasValue == true ? query[0].T150.Value : 0;
                                        int hrest = 5;// a repartir sur les taux en fonction du statut
                                        if (t120 < 8) { int r = 8 - t120; if (hrest > r) { pt.T120 = r; hrest = hrest - r; } else { pt.T120 = hrest; hrest = 0; } }
                                        if (t130 < 8 && hrest > 0) { int r = 8 - t130; if (hrest > r) { pt.T130 = r; hrest = hrest - r; } else { pt.T130 = hrest; hrest = 0; } }
                                        if (hrest > 0) { pt.T140 = hrest; }

                                        pt.HM = abateau == true ? 1 : 0;
                                    }
                                    #endregion
                                }
                            }
                            #endregion
                        }

                        ctx.SaveChanges();
                        //ctx.SaveChanges();
                    }

                    
                }
              return  Ok("ok");
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\error_rh_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", "", "", DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    //sw.WriteLine("message : " + ex.Message + " -------- innerexception : " + ex.StackTrace);
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.InnerException); sw.WriteLine("StackTrace : " + ex.StackTrace);
                }

                var message = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent("Demande non traitée : Veuillez ressayer.")
                };
                throw new HttpResponseException(message);
            }

        }
    }
}

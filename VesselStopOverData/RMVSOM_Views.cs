using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
  public  class RMVSOM_Marchal
    {
        public List<RMVSOM_Views> GetList(string statut, DateTime from, DateTime to)
        {
            //string str = System.Configuration.ConfigurationManager.ConnectionStrings[0].ConnectionString;
            List<RMVSOM_Views> lst = new List<RMVSOM_Views>(); List<requetes> _lr = null;
            using (var ctx = new RM_VSOMEntities())
            {
                if(statut==string.Empty)
                     _lr = ctx.requetes.Where(r => r.REC_TIME >= from && r.REC_TIME <= to).ToList<requetes>();
                else
                 _lr = ctx.requetes.Where(r => r.ETAT == statut && r.REC_TIME >= from && r.REC_TIME <= to).ToList<requetes>();

                foreach (requetes r in _lr)
                {
                    lst.Add(new RMVSOM_Views
                    {
                        Date = r.REC_TIME.Value,
                        HT = r.quotation.HT.Value,
                        TTC = r.quotation.TTC.Value,
                        TVA = r.quotation.TVA.Value,
                        Level = r.quotation.LEVEL,
                        Client = string.Empty,
                        Libelle = r.LIBELLE,
                        NumBL = r.quotation.BL,
                        NumQuotation = r.ID_QUOTATION,
                        Chassis= r.quotation.CHASSIS,
                        DateSejour = r.quotation.ENDDATE.Value,
                        NumRequete = r.ID,
                        Statut = r.ETAT
                    });
                }
            }
           
            return lst;
        }

        public RMVSOM_Views LoadQuotationForm(RMVSOM_Views _view)
        {
            
            using (var ctx = new VesselStopOverData.RM_VSOMEntities())
            {
                VesselStopOverData.requetes req = ctx.requetes.Where(s => s.ID == _view.NumRequete).FirstOrDefault();
                VesselStopOverData.comptes cmt = ctx.comptes.Where(s => s.ID == req.IDCOMPTES.Value).FirstOrDefault();
                _view.Date = req.REC_TIME.Value;
                _view.Statut = req.ETAT;
                _view.User = cmt.NAME + " " + cmt.SURNAME;
                _view.EmailUser = cmt.EMAIL;
                if(req.IDFACTURE!=null)
                _view.Facture = req.IDFACTURE.Value;
                
            }
            using (var ctx = new VSOMClassesDataContext())
            {
                CONNAISSEMENT con = (from m in ctx.GetTable<CONNAISSEMENT>() where m.NumBL == _view.NumBL select m).FirstOrDefault();
                _view.NumVoy = con.ESCALE.NumVoySCR;
                _view.Escal = con.ESCALE.NumEsc.Value.ToString();
                _view.Consignee = con.ConsigneeBL;
                _view.IdBL = con.IdBL; _view.Navire = con.ESCALE.NAVIRE.NomNav;
                _view.NavireDateArrivee = con.ESCALE.DDechEsc.Value;
            }
            return _view;
        }

        public void ValideRequetes(int numrequete)
        {
            using (var ctx = new VesselStopOverData.RM_VSOMEntities())
            {
                VesselStopOverData.requetes req = ctx.requetes.Where(s => s.ID == numrequete).FirstOrDefault();

                req.ETAT = "Proforma";
                ctx.SaveChanges();

            }
        }

        public void FactureRequetes(int numrequete,int numfacture)
        {
            using (var ctx = new VesselStopOverData.RM_VSOMEntities())
            {
                VesselStopOverData.requetes req = ctx.requetes.Where(s => s.ID == numrequete).FirstOrDefault();
                req.IDFACTURE = numfacture;
                req.ETAT = "Proccessed";
                ctx.SaveChanges();

            }
        }

    }
   public class RMVSOM_Views
    {
        public int NumQuotation { get; set; }
        public int NumRequete { get; set; }
        public string Libelle { get; set; }
        public string NumBL { get; set; }
        public int IdBL { get; set; }
        public string Consignee { get; set; }
        public string Escal { get; set; }
        public string NumVoy { get; set; }
        public string Navire { get; set; }
        public DateTime NavireDateArrivee { get; set; }
        public string Level { get; set; }
        public string Chassis { get; set; }
        public int HT { get; set; }
        public int TVA { get; set; }
        public int TTC { get; set; }
        public int NHT { get; set; }
        public int NTVA { get; set; }
        public int NTTC { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateSejour { get; set; }
        public string Client { get; set; }
        public string Statut { get; set; }
        public string User { get; set; }
        public string EmailUser { get; set; }
        public int Facture { get; set; }
        //public List<RMVSOM_ViewsDetails> Details { get; set; }
    }

    class RMVSOM_ViewsDetails
    {
    }
}

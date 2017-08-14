using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    public class SuperClass
    {
        public VSOMClassesDataContext dcConfig;  
        public SuperClass()
        {
         //dc = new VSOMClassesDataContext();
        }
        public SuperClass(VSOMClassesDataContext _dc)
        {
            dcConfig = _dc;
        }
        public List<OPERATION> GetOperationsUtilisateur(int idUser, VSOMClassesDataContext dc)
        {
            return (from op in dc.GetTable<OPERATION>()
                    from dr in dc.GetTable<DROIT>()
                    where dr.IdOp == op.IdOp && dr.IdU == idUser
                    select op).ToList<OPERATION>();
        }
        public List<PAYEMENT> GetPaiementsFactureOfConnaissement(int idBL, VSOMClassesDataContext dc)
        {
            return (from pay in dc.GetTable<PAYEMENT>()
                    where pay.IdBL == idBL && (pay.ObjetPay == 0 || pay.ObjetPay == 1)
                    select pay).ToList<PAYEMENT>();
        }

        public List<ElementFacturation> GetLignesProf(int idProf,VSOMClassesDataContext dc)
        {
            return (from ligneProf in dc.GetTable<LIGNE_PROFORMA>()
                    from elt in dc.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdJEF == ligneProf.IdJEF && ligneProf.IdFP == idProf
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = ligneProf.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = ligneProf.PUEF.Value,
                        MontantHT = Math.Round(ligneProf.PUEF.Value * ligneProf.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((ligneProf.PUEF * ligneProf.QTEEF * ligneProf.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((ligneProf.PUEF * ligneProf.QTEEF * (1 + ligneProf.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represent l'ensemble des requete de selection de données du systeme
    /// </summary>
   public class VsomParameters: SuperClass
    {
       VSOMClassesDataContext dcPar; 
       public VsomParameters(): base()
       {
           dcPar = new VSOMClassesDataContext();
       }

       public VsomParameters( VSOMClassesDataContext dc)
           : base()
       {
           dcPar = dc;// new VSOMClassesDataContext();
       }

       #region utilisateur

       public List<OPERATION> GetOperationsUtilisateur(int idUser)
       {
           return (from op in dcPar.GetTable<OPERATION>()
                   from dr in dcPar.GetTable<DROIT>()
                   where dr.IdOp == op.IdOp && dr.IdU == idUser
                   select op).ToList<OPERATION>();
       }


       public List<UTILISATEUR> GetUtilisateurs()
       {
           return (from user in dcPar.GetTable<UTILISATEUR>()
                   orderby user.NU ascending
                   select user).ToList<UTILISATEUR>();
       }

       public List<UTILISATEUR> GetUtilisateursByStatut(string statut)
       {
           return (from user in dcPar.GetTable<UTILISATEUR>()
                   where user.EU == statut
                   orderby user.NU ascending
                   select user).ToList<UTILISATEUR>();
       }

       public UTILISATEUR GetUtilisateursByIdU(int idU)
       {
           return (from user in dcPar.GetTable<UTILISATEUR>()
                   where user.IdU == idU
                   orderby user.NU ascending
                   select user).SingleOrDefault<UTILISATEUR>();
       }


       public List<UTILISATEUR> GetUtilisateursByNom(string nom)
       {
           return (from user in dcPar.GetTable<UTILISATEUR>()
                   where user.NU.Contains(nom)
                   orderby user.NU ascending
                   select user).ToList<UTILISATEUR>();
       }

       public List<UTILISATEUR> GetUtilisateursByAcconier(string acconier)
       {
           return (from user in dcPar.GetTable<UTILISATEUR>()
                   where user.ACCONIER.NomAcc.Contains(acconier)
                   orderby user.NU ascending
                   select user).ToList<UTILISATEUR>();
       } 
       #endregion

        public List<OPERATION> GetOperations()
        {
            return (from op in dcPar.GetTable<OPERATION>()
                    orderby op.NomOp ascending
                    select op).ToList<OPERATION>();
        }

       
        public List<TYPE_CONTENEUR> GetTypesConteneurs()
        {
            return (from type in dcPar.GetTable<TYPE_CONTENEUR>()
                    orderby type.LibTypeCtr ascending
                    select type).ToList<TYPE_CONTENEUR>();
        }

        public List<TYPE_CONVENTIONNEL> GetTypesConventionnelsImport()
        {
            return (from type in dcPar.GetTable<TYPE_CONVENTIONNEL>()
                    where type.SensGC == "I"
                    orderby type.LibTypeGC ascending
                    select type).ToList<TYPE_CONVENTIONNEL>();
        }

        public List<TYPE_CONVENTIONNEL> GetTypesConventionnelsExport()
        {
            return (from type in dcPar.GetTable<TYPE_CONVENTIONNEL>()
                    where type.SensGC == "E"
                    orderby type.LibTypeGC ascending
                    select type).ToList<TYPE_CONVENTIONNEL>();
        }

        public List<TypeSinistreVehicule> GetTypesSinistreVeh()
        {
            return (from type in dcPar.GetTable<TYPE_SINISTRE>()
                    where type.TypeMse == "V"
                    orderby type.LibTypeSinistre ascending
                    select new TypeSinistreVehicule
                    {
                        idTypeSinistre = type.IdTypeSinistre,
                        TypeSinistre = type.LibTypeSinistre
                    }).ToList<TypeSinistreVehicule>();
        }

        public List<TYPE_VISITE> GetTypesVisites()
        {
            return (from type in dcPar.GetTable<TYPE_VISITE>()
                    orderby type.LibTypeVisite ascending
                    select type).ToList<TYPE_VISITE>();
        }

        public List<TYPE_SINISTRE> GetTypesSinistreCtr()
        {
            return (from type in dcPar.GetTable<TYPE_SINISTRE>()
                    where type.TypeMse == "C"
                    orderby type.IdTypeSinistre ascending
                    select type).ToList<TYPE_SINISTRE>();
        }

        public List<TYPE_VEHICULE> GetTypesVehicules()
        {
            return (from type in dcPar.GetTable<TYPE_VEHICULE>()
                    orderby type.LibTypeVeh ascending
                    select type).ToList<TYPE_VEHICULE>();
        }

        public List<FAMILLE_ARTICLE> GetFamillesArticlesClients()
        {
            return (from fam in dcPar.GetTable<FAMILLE_ARTICLE>()
                    where fam.Niveau == "C"
                    orderby fam.LibFamArt ascending
                    select fam).ToList<FAMILLE_ARTICLE>();
        }

        public List<FAMILLE_ARTICLE> GetFamillesArticles()
        {
            return (from fam in dcPar.GetTable<FAMILLE_ARTICLE>()
                    where fam.Niveau == "C" || fam.Niveau == "E"
                    orderby fam.LibFamArt ascending
                    select fam).ToList<FAMILLE_ARTICLE>();
        }

        public List<FAMILLE_ARTICLE> GetFamillesArticlesArmateurs()
        {
            return (from fam in dcPar.GetTable<FAMILLE_ARTICLE>()
                    where fam.Niveau == "E"
                    orderby fam.LibFamArt ascending
                    select fam).ToList<FAMILLE_ARTICLE>();
        }

        public List<ARTICLE> GetArticleByFamille(int codeFamille)
        {
            return (from art in dcPar.GetTable<ARTICLE>()
                    where art.CodeFamArt == codeFamille && art.Statut=="A"
                    orderby art.LibArticle ascending
                    select art).ToList<ARTICLE>();
        }

        public List<ARTICLE> GetArticleByCodeArticle(int codeArticle)
        {
            return (from art in dcPar.GetTable<ARTICLE>()
                    where art.CodeArticle == codeArticle && art.Statut == "A"
                    orderby art.LibArticle ascending
                    select art).ToList<ARTICLE>();
        }

        public List<ARTICLE> GetArticleAll()
        {
            return (from art in dcPar.GetTable<ARTICLE>()
                    orderby art.CodeArticle ascending
                    select art).ToList<ARTICLE>();
        }

        public List<CLIENT> GetClientsActifs()
        {
            return (from clt in dcPar.GetTable<CLIENT>()
                    where clt.StatutClient == "A"
                    orderby clt.NomClient ascending
                    select clt).ToList<CLIENT>();
        }

        public List<FOURNISSEUR> GetFournisseursActifs()
        {
            return (from fsseur in dcPar.GetTable<FOURNISSEUR>()
                    where fsseur.StatutFsseur == "A"
                    orderby fsseur.NomFsseur ascending
                    select fsseur).ToList<FOURNISSEUR>();
        }

        public List<BANQUE> GetBanques()
        {
            return (from bank in dcPar.GetTable<BANQUE>()
                    orderby bank.CCBanque ascending
                    select bank).ToList<BANQUE>();
        }

        public List<LIEU> GetLieux()
        {
            return (from lieu in dcPar.GetTable<LIEU>()
                    orderby lieu.IdLieu ascending
                    select lieu).ToList<LIEU>();
        }

        public List<ACCONIER> GetAcconiersActifs()
        {
            return (from acc in dcPar.GetTable<ACCONIER>()
                    where acc.StatutAcc == "A"
                    orderby acc.NomAcc ascending
                    select acc).ToList<ACCONIER>();
        }

        public List<ACCONIER> GetAcconiersByCodeAcc(string codeAcc)
        {
            return (from acc in dcPar.GetTable<ACCONIER>()
                    where acc.CodeAcc.Contains(codeAcc)
                    orderby acc.NomAcc ascending
                    select acc).ToList<ACCONIER>();
        }

        public List<ACCONIER> GetAcconiersByNomAcc(string nomAcc)
        {
            return (from acc in dcPar.GetTable<ACCONIER>()
                    where acc.NomAcc.Contains(nomAcc)
                    orderby acc.NomAcc ascending
                    select acc).ToList<ACCONIER>();
        }

        public List<NAVIRE> GetNaviresActifs()
        {
            return (from nav in dcPar.GetTable<NAVIRE>()
                    where nav.StatutNav == "A"
                    orderby nav.NomNav ascending
                    select nav).ToList<NAVIRE>();
        }

        public List<NAVIRE> GetNaviresByCodeNav(string codeNav)
        {
            return (from nav in dcPar.GetTable<NAVIRE>()
                    where nav.CodeNav.Contains(codeNav)
                    orderby nav.NomNav ascending
                    select nav).ToList<NAVIRE>();
        }

        public List<NAVIRE> GetNaviresByNomNav(string nomNav)
        {
            return (from nav in dcPar.GetTable<NAVIRE>()
                    where nav.NomNav.Contains(nomNav)
                    orderby nav.NomNav ascending
                    select nav).ToList<NAVIRE>();
        }

        public List<ARMATEUR> GetArmateursActifs()
        {
            return (from arm in dcPar.GetTable<ARMATEUR>()
                    where arm.StatutArm == "A"
                    orderby arm.NomArm ascending
                    select arm).ToList<ARMATEUR>();
        }

        public List<ARMATEUR> GetArmateursByCodeArm(string codeArm)
        {
            return (from arm in dcPar.GetTable<ARMATEUR>()
                    where arm.CodeArm.Contains(codeArm)
                    orderby arm.NomArm ascending
                    select arm).ToList<ARMATEUR>();
        }

        public List<ARMATEUR> GetArmateursByNomArm(string nomArm)
        {
            return (from arm in dcPar.GetTable<ARMATEUR>()
                    where arm.NomArm.Contains(nomArm)
                    orderby arm.NomArm ascending
                    select arm).ToList<ARMATEUR>();
        }

        public List<PORT> GetPortsOrderByCode()
        {
            return (from prt in dcPar.GetTable<PORT>()
                    orderby prt.CodePort ascending
                    select prt).ToList<PORT>();
        }

        public List<PORT> GetPortsOrderByNom()
        {
            return (from prt in dcPar.GetTable<PORT>()
                    orderby prt.NomPort ascending
                    select prt).ToList<PORT>();
        }

        public List<PORT> GetPortsByCodePort(string codePort)
        {
            return (from prt in dcPar.GetTable<PORT>()
                    where prt.CodePort.Contains(codePort)
                    orderby prt.CodePort, prt.NomPort ascending
                    select prt).ToList<PORT>();
        }

        public List<PORT> GetPortsByNomPort(string nomPort)
        {
            return (from prt in dcPar.GetTable<PORT>()
                    where prt.NomPort.Contains(nomPort)
                    orderby prt.CodePort, prt.NomPort ascending
                    select prt).ToList<PORT>();
        }

        public List<PORT> GetPortsByPaysPort(string paysPort)
        {
            return (from prt in dcPar.GetTable<PORT>()
                    where prt.PaysPort.Contains(paysPort)
                    orderby prt.CodePort, prt.NomPort ascending
                    select prt).ToList<PORT>();
        }

        public List<CODE_TVA> GetCodesTVA()
        {
            return (from code in dcPar.GetTable<CODE_TVA>()
                    orderby code.LibTVA ascending
                    select code).ToList<CODE_TVA>();
        }

        public List<OPERATION_ARMATEUR> GetOperationArmOfEscale(int idEsc)
        {
            return (from op in dcPar.GetTable<OPERATION_ARMATEUR>()
                    where op.IdEsc == idEsc
                    orderby op.TYPE_OPERATION.LibTypeOp ascending
                    select op).ToList<OPERATION_ARMATEUR>();
        }

        public List<ElementCompteArmRegroup> GetCompteArmRegroup(int idEsc)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdEsc == idEsc && eltFact.PUEF != 0 && eltFact.QTEEF != 0 && !eltFact.IdBL.HasValue
                    group eltFact by new { eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.LibFamArt, eltFact.LIGNE_PRIX.ARTICLE.LibArticle } into g
                    select new ElementCompteArmRegroup
                    {
                        Rubrique = g.Key.LibArticle,
                        Regroup = g.Key.LibFamArt,
                        MontantHT = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round(Convert.ToDouble(g.Sum(eltFact => (eltFact.PUEF * eltFact.QTEEF * eltFact.TauxTVA) / 100).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF * (1 + eltFact.TauxTVA / 100)).Value), 0, MidpointRounding.AwayFromZero)
                    }).ToList<ElementCompteArmRegroup>();
        }

        public LIGNE_PRIX GetLignePrixOpArm(int idArm, int codeLP)
        {
            DateTime dte = DateTime.Now;

            return (from lp in dcPar.GetTable<LIGNE_PRIX>()
                    where lp.CodeLP == codeLP.ToString() && lp.LP == idArm.ToString() && lp.DDLP <= dte && lp.DFLP >= dte
                    select lp).FirstOrDefault<LIGNE_PRIX>();
        }

        public List<PARAMETRE> GetModesPayement()
        {
            return (from par in dcPar.GetTable<PARAMETRE>()
                    where par.Test == "P"
                    orderby par.NomAF ascending
                    select par).ToList<PARAMETRE>();
        }
        
        public List<CLIENT> GetClientByCode(string codeClient)
        {
            return (from clt in dcPar.GetTable<CLIENT>()
                    where clt.CodeClient.Contains(codeClient)
                    select clt).ToList<CLIENT>();
        }

        public List<CLIENT> GetClientByNom(string nomClient)
        {
            return (from clt in dcPar.GetTable<CLIENT>()
                    where clt.NomClient.Contains(nomClient)
                    select clt).ToList<CLIENT>();
        }


        #region Conteneur

        public List<CONTENEUR> GetConteneursImport()
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.SensCtr == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportByNumBL(string numBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.CONNAISSEMENT.NumBL.Contains(numBL) && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByNumBL(string numBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.CONTENEUR.CONNAISSEMENT.NumBL.Contains(numBL) /*&& ctr.CONTENEUR.CONNAISSEMENT.SensBL == "I"*/ && ctr.IsDoublon == "N"
                    orderby ctr.CONTENEUR.NumCtr ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR> GetConteneursByNumBL(string numBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.CONNAISSEMENT.NumBL.Contains(numBL)// && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public CONTENEUR GetConteneurByIdCtr(int idCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdCtr == idCtr
                    orderby ctr.NumCtr ascending
                    select ctr).SingleOrDefault<CONTENEUR>();
        }

        public CONTENEUR GetConteneurImportByIdCtr(int idCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdCtr == idCtr && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).SingleOrDefault<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportByStatut(string statut)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.StatCtr == statut && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportCautionPayee(int idBL, int idPay)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdPay.HasValue && ctr.IdBL == idBL && ctr.IdPay == idPay && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportCautionImpayee(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where !ctr.IdPay.HasValue && ctr.IdBL == idBL && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<PAYEMENT> GetCautionNonRestitueeByIdBL(int idBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    from drc in dcPar.GetTable<DEMANDE_CAUTION>()
                    where pay.IdBL == idBL && drc.IdBL == idBL && drc.DVDRC.HasValue && !pay.IdPayDRC.HasValue && pay.ObjetPay == 2 && pay.CONNAISSEMENT.SensBL == "I"
                    orderby pay.IdPay ascending
                    select pay).ToList<PAYEMENT>();
        }

        public List<CONTENEUR> GetConteneursImportByNumEscale(int numEsc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.ESCALE.NumEsc == numEsc && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportByNumCtr(string numCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.NumCtr.Contains(numCtr) && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByNumCtr(string numCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.CONTENEUR.NumCtr.Contains(numCtr) /*&& ctr.CONTENEUR.CONNAISSEMENT.SensBL == "I"*/ && ctr.IsDoublon == "N"
                    orderby ctr.CONTENEUR.NumCtr ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        //public List<CONTENEUR> GetConteneursEmbarques(int idEsc)
        //{
        //    return (from ctr in dcPar.GetTable<CONTENEUR>()
        //            where ctr.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283 && op.IdEsc == idEsc) != null
        //            orderby ctr.NumCtr ascending
        //            select ctr).ToList<CONTENEUR>();
        //}

        //public List<CONTENEUR> GetConteneursEmbarques(int idEsc)
        //{
        //    return (from ctr in dcPar.GetTable<CONTENEUR>()
        //            from mvt in dcPar.GetTable<MOUVEMENT_TC>()
        //            where mvt.IdEsc == idEsc && mvt.IdTypeOp == 283 && ctr.IdCtr == mvt.CONTENEUR_TC.IdCtr && ctr.StatCtr == "Cargo Loaded"
        //            orderby ctr.NumCtr ascending
        //            select ctr).GroupBy(c => c.NumCtr).Select(c => c.First()).ToList<CONTENEUR>();
        //}

        public List<CONTENEUR> GetConteneursEmbarques(int idEsc)
        {
            var matchedEsc = (from esc in dcPar.GetTable<ESCALE>()
                              where esc.IdEsc == idEsc
                              select esc).FirstOrDefault<ESCALE>();

            if (matchedEsc != null && matchedEsc.IdArm == 1)
            {
                return (from ctr in dcPar.GetTable<CONTENEUR>()
                        where ctr.IdEsc == idEsc && ctr.StatCtr == "Cargo Loaded" && ctr.SensCtr == "E" && ctr.ESCALE.IdArm == 1
                        orderby ctr.NumCtr ascending
                        select ctr).ToList<CONTENEUR>();
            }
            else if (matchedEsc != null && matchedEsc.IdArm == 2)
            {
                return (from ctr in dcPar.GetTable<CONTENEUR>()
                        where ctr.IdEsc == idEsc && ctr.StatCtr == "Cargo Loaded" && ctr.SensCtr == "I" && ctr.ESCALE.IdArm == 2
                        orderby ctr.NumCtr ascending
                        select ctr).ToList<CONTENEUR>();
            }
            else
            {
                return new List<CONTENEUR>();
            }
        }

        public List<CONTENEUR> GetConteneursDebarques(int idEsc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    from mvt in dcPar.GetTable<MOUVEMENT_TC>()
                    where mvt.IdEsc == idEsc && mvt.IdTypeOp == 12 && ctr.IdCtr == mvt.CONTENEUR_TC.IdCtr
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<OPERATION_CONTENEUR> GetConteneursDebarques(int idArm, DateTime dateDebut, DateTime dateFin)
        {
            return (from opCtr in dcPar.GetTable<OPERATION_CONTENEUR>()
                    from ctr in dcPar.GetTable<CONTENEUR>()
                    where opCtr.CONTENEUR.ESCALE.ARMATEUR.IdArm == idArm && opCtr.IdTypeOp == 12 && ctr.IdCtr == opCtr.IdCtr && opCtr.DateOp.Value >= dateDebut && opCtr.DateOp.Value <= dateFin.AddHours(23).AddMinutes(59).AddSeconds(59)
                    orderby ctr.NumCtr ascending
                    select opCtr).ToList<OPERATION_CONTENEUR>();
        }

        public List<OPERATION_CONTENEUR> GetConteneursEmbarques(int idArm, DateTime dateDebut, DateTime dateFin)
        {
            return (from opCtr in dcPar.GetTable<OPERATION_CONTENEUR>()
                    from ctr in dcPar.GetTable<CONTENEUR>()
                    where opCtr.CONTENEUR.ESCALE.ARMATEUR.IdArm == idArm && opCtr.IdTypeOp == 283 && ctr.IdCtr == opCtr.IdCtr && opCtr.DateOp.Value >= dateDebut && opCtr.DateOp.Value <= dateFin.AddHours(23).AddMinutes(59).AddSeconds(59)
                    orderby ctr.NumCtr ascending
                    select opCtr).ToList<OPERATION_CONTENEUR>();
        }

        public List<PARC> GetParcs(string type)
        {
            return (from parc in dcPar.GetTable<PARC>()
                    where parc.Type == type
                    orderby parc.NomParc ascending
                    select parc).ToList<PARC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByNumEscale(int numEsc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.CONTENEUR.ESCALE.NumEsc == numEsc /*&& ctr.CONTENEUR.CONNAISSEMENT.SensBL == "I"*/ && ctr.IsDoublon == "N"
                    orderby ctr.CONTENEUR.NumCtr ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByDescription(string description)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.CONTENEUR.DescCtr.Contains(description) /*&& ctr.CONTENEUR.CONNAISSEMENT.SensBL == "I"*/ && ctr.IsDoublon == "N"
                    orderby ctr.CONTENEUR.NumCtr ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByParc(string parc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.PARC.NomParc.Contains(parc) && (ctr.StatutTC == "Parqué" || ctr.StatutTC == "Réparation" || ctr.StatutTC == "Nettoyage") && ctr.IsDoublon == "N"
                    orderby ctr.CONTENEUR.NumCtr ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR> GetConteneursImportByDescription(string description)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.DescCtr.Contains(description) && ctr.CONNAISSEMENT.SensBL == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportOfConnaissement(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdBL == idBL && ctr.SensCtr == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportLivraisonOfConnaissement(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdBL == idBL && ctr.IdBAE.HasValue && ctr.BON_ENLEVEMENT.DVBAE.HasValue && !ctr.IdDBL.HasValue && ctr.SensCtr == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursPourRecapSurestaries(DateTime debut, DateTime fin)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    //from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    //where ctr.IdCtr == elt.IdCtr && elt.LIGNE_PROFORMA.FirstOrDefault(fp => fp.PROFORMA.DVFP.HasValue).PROFORMA.DVFP >= debut && elt.LIGNE_PROFORMA.FirstOrDefault(fp => fp.PROFORMA.DVFP.HasValue).PROFORMA.DVFP <= fin && elt.IdFD.HasValue && elt.LIGNE_PRIX.CodeArticle == 1805
                    //where ctr.IdCtr == elt.IdCtr && ctr.DSCtr >= debut && ctr.DSCtr <= fin /*&& elt.IdFD.HasValue*/ && elt.LIGNE_PRIX.CodeArticle == 1805
                    where ctr.DSCtr.HasValue && ctr.DSCtr >= debut && ctr.DSCtr <= fin && ctr.ESCALE.IdArm == 1
                    orderby ctr.NumCtr
                    select ctr).Distinct<CONTENEUR>().ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursPourRecapDetention(DateTime debut, DateTime fin)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    //from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    //where ctr.IdCtr == elt.IdCtr && elt.LIGNE_PROFORMA.FirstOrDefault(fp => fp.PROFORMA.DVFP.HasValue).PROFORMA.DVFP >= debut && elt.LIGNE_PROFORMA.FirstOrDefault(fp => fp.PROFORMA.DVFP.HasValue).PROFORMA.DVFP <= fin && elt.IdFD.HasValue && elt.LIGNE_PRIX.CodeArticle == 1807
                    //where ctr.IdCtr == elt.IdCtr && ctr.DRCtr >= debut && ctr.DRCtr <= fin && elt.IdFD.HasValue && elt.LIGNE_PRIX.CodeArticle == 1807
                    where ctr.DRCtr.HasValue && ctr.DRCtr >= debut && ctr.DRCtr <= fin && ctr.ESCALE.IdArm == 1
                    orderby ctr.NumCtr
                    select ctr).Distinct<CONTENEUR>().ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportSortieOfConnaissement(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdBL == idBL && ctr.IdBAE.HasValue && ctr.BON_ENLEVEMENT.DVBAE.HasValue && ctr.DEMANDE_LIVRAISON.DVDBL.HasValue && ctr.SensCtr == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursImportOfDBL(int idDBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdDBL == idDBL && ctr.SensCtr == "I"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursOfDemandeCaution(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdBL == idBL && ctr.CONNAISSEMENT.DEMANDE_CAUTION.Count != 0
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExport()
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.SensCtr == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByNumBooking(string numBooking)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.CONNAISSEMENT.NumBooking.Contains(numBooking) && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public CONTENEUR GetConteneurExportByIdCtr(int idCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.IdCtr == idCtr && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).SingleOrDefault<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByStatut(string statut)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.StatCtr == statut && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByNumEscale(int numEsc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.ESCALE.NumEsc == numEsc && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByPortDest(string codePort)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.CONNAISSEMENT.DPBL.Contains(codePort) && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByNumCtr(string numCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.NumCtr.Contains(numCtr) && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        public List<CONTENEUR> GetConteneursExportByDescription(string description)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    where ctr.DescCtr.Contains(description) && ctr.CONNAISSEMENT.SensBL == "E"
                    orderby ctr.NumCtr ascending
                    select ctr).ToList<CONTENEUR>();
        }

        #endregion

        #region Mafi

        public List<MAFI> GetMafisImportOfDBL(int idDBL)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.IdDBL == idDBL && mf.SensMafi == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportSortieOfConnaissement(int idBL)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.IdBL == idBL && mf.IdBAE.HasValue && mf.BON_ENLEVEMENT.DVBAE.HasValue && mf.DEMANDE_LIVRAISON.DVDBL.HasValue && mf.SensMafi == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportLivraisonOfConnaissement(int idBL)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.IdBL == idBL && mf.IdBAE.HasValue && mf.BON_ENLEVEMENT.DVBAE.HasValue && !mf.IdDBL.HasValue && mf.SensMafi == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public MAFI GetMafiImportByIdMafi(int idMafi)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.IdMafi == idMafi && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).SingleOrDefault<MAFI>();
        }

        public List<MAFI> GetMafisImport()
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.SensMafi == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportOfConnaissement(int idBL)
        {
            return (from m in dcPar.GetTable<MAFI>()
                    where m.IdBL == idBL && m.SensMafi == "I"
                    orderby m.NumMafi ascending
                    select m).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportByStatut(string statut)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.StatMafi == statut && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportByNumBL(string numBL)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.CONNAISSEMENT.NumBL.Contains(numBL) && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisByNumBL(string numBL)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.CONNAISSEMENT.NumBL.Contains(numBL)// && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportByNumMafi(string numMf)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.NumMafi.Contains(numMf) && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportByNumEscale(int numEsc)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.ESCALE.NumEsc == numEsc && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        public List<MAFI> GetMafisImportByDescription(string description)
        {
            return (from mf in dcPar.GetTable<MAFI>()
                    where mf.DescMafi.Contains(description) && mf.CONNAISSEMENT.SensBL == "I"
                    orderby mf.NumMafi ascending
                    select mf).ToList<MAFI>();
        }

        #endregion

        #region Conventionnel

        public List<CONVENTIONNEL> GetConventionnelsImportLivraisonOfConnaissement(int idBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdBL == idBL && conv.IdBAE.HasValue && conv.BON_ENLEVEMENT.DVBAE.HasValue && !conv.IdDBL.HasValue && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsImportSortieOfConnaissement(int idBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdBL == idBL && conv.IdBAE.HasValue && conv.BON_ENLEVEMENT.DVBAE.HasValue && conv.DEMANDE_LIVRAISON.DVDBL.HasValue && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsOfConnaissement(int idBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdBL == idBL && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsImportOfDBL(int idDBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdDBL == idDBL && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public CONVENTIONNEL GetConventionnelByIdGC(int idGC)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdGC == idGC && conv.CONNAISSEMENT.SensBL == "I"
                    orderby conv.NumGC ascending
                    select conv).SingleOrDefault<CONVENTIONNEL>();
        }

        public CONVENTIONNEL GetConventionnelById(int idGC)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdGC == idGC
                    orderby conv.NumGC ascending
                    select conv).SingleOrDefault<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsByNumGC(string numGC)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.NumGC.Contains(numGC) && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsImport()
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsImportByStatut(string statut)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.StatGC == statut && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsImportByNumBL(string numBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.CONNAISSEMENT.NumBL.Contains(numBL) && conv.CONNAISSEMENT.SensBL == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsByNumBL(string numBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.CONNAISSEMENT.NumBL.Contains(numBL) //&& conv.CONNAISSEMENT.SensBL == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsByNumEscale(int numEscale)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.ESCALE.NumEsc == numEscale && conv.SensGC == "I"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public CONVENTIONNEL GetConventionnelExportByIdGC(int idGC)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.IdGC == idGC && conv.CONNAISSEMENT.SensBL == "E"
                    orderby conv.NumGC ascending
                    select conv).SingleOrDefault<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExportByNumGC(string numGC)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.NumGC.Contains(numGC) && conv.SensGC == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExport()
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.SensGC == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExportByStatut(string statut)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.StatGC == statut && conv.SensGC == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExportByNumBooking(string numBooking)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.CONNAISSEMENT.NumBooking.Contains(numBooking) && conv.CONNAISSEMENT.SensBL == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExportByNumEscale(int numEscale)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.ESCALE.NumEsc == numEscale && conv.SensGC == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        public List<CONVENTIONNEL> GetConventionnelsExportByPortDest(string codePort)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    where conv.CONNAISSEMENT.DPBL.Contains(codePort) && conv.SensGC == "E"
                    orderby conv.NumGC ascending
                    select conv).ToList<CONVENTIONNEL>();
        }

        #endregion

        #region Cubage

        public List<CUBAGE> GetCubages()
        {
            return (from cub in dcPar.GetTable<CUBAGE>()
                    orderby cub.DateCubage descending
                    select cub).ToList<CUBAGE>();
        }

        public List<CUBAGE> GetCubagesEnCours()
        {
            return (from cub in dcPar.GetTable<CUBAGE>()
                    where !cub.DateCloCubage.HasValue
                    orderby cub.DateCubage descending
                    select cub).ToList<CUBAGE>();
        }

        public List<CUBAGE> GetCubagesClotures()
        {
            return (from cub in dcPar.GetTable<CUBAGE>()
                    where cub.DateCloCubage.HasValue
                    orderby cub.DateCubage descending
                    select cub).ToList<CUBAGE>();
        }

        public CUBAGE GetCubageByIdCub(int idCub)
        {
            return (from cub in dcPar.GetTable<CUBAGE>()
                    where cub.IdCubage == idCub
                    select cub).SingleOrDefault<CUBAGE>();
        }

        public List<CUBAGE> GetCubageByNumEsc(int numEsc)
        {
            return (from cub in dcPar.GetTable<CUBAGE>()
                    where cub.ESCALE.NumEsc == numEsc
                    select cub).ToList<CUBAGE>();
        }

        #endregion

        #region Ordre de service

        public ORDRE_SERVICE GetServiceByIdOS(int idOS)
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where os.IdOS == idOS
                    orderby os.IdOS descending
                    select os).SingleOrDefault<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetServiceByNumEscale(int numEscale)
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where os.ESCALE.NumEsc == numEscale
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetServiceByNumBL(string numBL)
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where os.CONNAISSEMENT.NumBL.Contains(numBL)
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetServices()
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetServicesByStatut(string statut)
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where os.StatutOS == statut
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        #endregion

        #region sinistre

        public List<OPERATION_SINISTRE> GetSinistres()
        {
            return (from sin in dcPar.GetTable<OPERATION_SINISTRE>()
                    orderby sin.DateSinistre descending
                    select sin).ToList<OPERATION_SINISTRE>();
        }

        public List<OPERATION_SINISTRE> GetSinistresByNumChassis(string numChassis)
        {
            return (from sin in dcPar.GetTable<OPERATION_SINISTRE>()
                    where sin.VEHICULE.NumChassis == numChassis
                    orderby sin.DateSinistre descending
                    select sin).ToList<OPERATION_SINISTRE>();
        }

        public List<OPERATION_SINISTRE> GetSinistresByNumBL(string numBL)
        {
            return (from sin in dcPar.GetTable<OPERATION_SINISTRE>()
                    where sin.VEHICULE.CONNAISSEMENT.NumBL == numBL
                    orderby sin.DateSinistre descending
                    select sin).ToList<OPERATION_SINISTRE>();
        }

        #endregion

        #region Element de facturation

        public List<ElementFacturation> GetElementFacturationBL(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationBLFree(int idBL , VSOMClassesDataContext _dc)
        {
            return (from elt in _dc.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule" && elt.StatutEF != "Facturé"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }
        public List<ElementFacturation> GetElementFacturationBLFree(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule" && elt.StatutEF != "Facturé"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationProf(int idProf)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    from lp in dcPar.GetTable<LIGNE_PROFORMA>()
                    where elt.IdJEF == lp.IdJEF && lp.IdFP == idProf && lp.QTEEF != 0 && lp.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = lp.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = lp.PUEF.Value,
                        MontantHT = Math.Round(lp.PUEF.Value * lp.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((lp.PUEF * lp.QTEEF * lp.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((lp.PUEF * elt.QTEEF * (1 + lp.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationEsc(int idEsc)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdEsc == idEsc && !elt.IdBL.HasValue && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }
       /// <summary>
       /// utiliser par l'ecran avoirspot pour lister sur un escale les lignes compulsory
       /// </summary>
       /// <param name="numesc"></param>
       /// <returns></returns>
        public List<ElementFacturation> GetElementFacturationCompulsory(int numesc)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumEsc.Value == numesc && elt.IdEsc == esc.IdEsc && elt.CodeArticle == "2207" && elt.StatutEF == "En Cours"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF+"-" + elt.ESCALE.NumVoySCR,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF,
                        Compte=elt.CCArticle
                    }).ToList<ElementFacturation>();
        }

       /// <summary>
       /// utiliser par lecran avoirspotselect pour lister les lignes maintenance conteneur
       /// </summary>
       /// <param name="numesc"></param>
       /// <returns></returns>
        public List<ElementFacturation> GetElementFacturationMaintenanceCtr(int numesc)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumEsc.Value == numesc && elt.IdEsc == esc.IdEsc && elt.CodeArticle == "1823" && elt.StatutEF == "En Cours"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF ,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF,
                        Compte = elt.CCArticle
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationBooking(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementNonFactureBL(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.QTEEF != 0 && elt.PUEF != 0 && !elt.IdFD.HasValue && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationVeh(int idVeh)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdVeh == idVeh && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationVehFree(int idVeh)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdVeh == idVeh && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule" && elt.StatutEF != "Facturé"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationCtr(int idCtr)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdCtr == idCtr && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        MontantDIT = elt.PTDIT.Value,
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationMafi(int idMafi)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdMafi == idMafi && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        //MontantDIT = elt.PTDIT.Value,
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementFacturationConv(int idGC)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdGC == idGC && elt.QTEEF != 0 && elt.PUEF != 0 && elt.StatutEF != "Annule"
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetLignesProf(int idProf)
        {
            return (from ligneProf in dcPar.GetTable<LIGNE_PROFORMA>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
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

        public List<ElementFacturation> GetLignesProfPourAvoir(int idProf)
        {
            return (from ligneProf in dcPar.GetTable<LIGNE_PROFORMA>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdJEF == ligneProf.IdJEF && elt.StatutEF == "Facturé" && ligneProf.IdFP == idProf
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

        public List<ElementFacturation> GetLignesFactDIT(int idFact)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdFactDIT == idFact
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementLigneOS> GetLignesOS(int idOS)
        {
            return (from elt in dcPar.GetTable<LIGNE_SERVICE>()
                    where elt.IdOS == idOS
                    select new ElementLigneOS
                    {
                        Code = elt.CodeArticle.ToString(),
                        Libelle = elt.ARTICLE.LibArticle,
                        //AH8nov16 PrixTotal = (float)Convert.ToDouble(elt.PULS * elt.QLS * ((elt.ARTICLE.CODE_TVA.TauxTVA.ToString()=="0") ? 1: 1.1925)),
                        PrixTotal = (float)Convert.ToDouble(elt.PULS * elt.QLS * ((elt.CodeTVA == "TVAEX") ? 1 : 1.1925)),
                        PrixUnitaire = elt.PULS.Value,
                        Qte = (float)elt.QLS,
                        Remarques = elt.AILS,
                        CodeTVA=elt.ARTICLE.CodeTVA,
                        //AH8nov16 TVA = (float)Convert.ToDouble(elt.PULS * elt.QLS * ((elt.ARTICLE.CODE_TVA.TauxTVA.ToString()=="TVAEX") ? 0 :  0.1925)),
                        TVA = (float)Convert.ToDouble(elt.PULS * elt.QLS * ((elt.CodeTVA == "TVAEX") ? 0 : 0.1925)),
                        Unite = elt.ULS
                    }).ToList<ElementLigneOS>();
        }

        public List<ElementLigneFactureSpot> GetLignesAvoirSpot(int idavoir)
        {
            return (from la in dcPar.GetTable<LIGNE_AVOIR_SPOT>() where la.IdFA == idavoir select new ElementLigneFactureSpot {
             Code=la.CodeArticle.ToString(), Libelle=la.Lib, Qte=la.QTEEF, PrixUnitaire=(int)la.PUEF, HT=la.QTEEF*la.PUEF,
             TVA=la.TauxTVA , PrixTotal=  (la.PUEF*la.QTEEF) 
            }).ToList<ElementLigneFactureSpot>();
        }

        public List<ElementLigneFactureSpot> GetLignesAvoirFSpot(int idavoir)
        {
             return (from ligneAvoir in dcPar.GetTable<LIGNE_AVOIR>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                     where elt.IdJEF == ligneAvoir.IdJEF && ligneAvoir.IdFA == idavoir
                    orderby elt.IdJEF, elt.LibEF ascending
                     select new ElementLigneFactureSpot
                    {
                         
                        Code = elt.CodeArticle,
                        Libelle = elt.LibEF,
                        Qte = (float)ligneAvoir.QTEEF.Value,
                        Unite = elt.UnitEF,
                        HT =( ligneAvoir.PUEF.Value * ligneAvoir.QTEEF.Value),

                        PrixUnitaire = Convert.ToInt32(ligneAvoir.PUEF.Value), 
                       // MontantHT = Math.Round(ligneAvoir.PUEF.Value * ligneAvoir.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        TVA = Math.Round((ligneAvoir.PUEF * ligneAvoir.QTEEF * ligneAvoir.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        PrixTotal = Math.Round((ligneAvoir.PUEF * ligneAvoir.QTEEF * (1 + ligneAvoir.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero)
                       /* IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF*/
                    }).ToList<ElementLigneFactureSpot>();
        }

        public List<ElementFacturation> GetLignesAvoir(int idAvoir)
        {
            return (from ligneAvoir in dcPar.GetTable<LIGNE_AVOIR>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdJEF == ligneAvoir.IdJEF && ligneAvoir.IdFA == idAvoir
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = ligneAvoir.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = ligneAvoir.PUEF.Value,
                        MontantHT = Math.Round(ligneAvoir.PUEF.Value * ligneAvoir.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((ligneAvoir.PUEF * ligneAvoir.QTEEF * ligneAvoir.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((ligneAvoir.PUEF * ligneAvoir.QTEEF * (1 + ligneAvoir.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ELEMENT_FACTURATION> GetEltFactByCodeArticle(int codeArt, int idVeh)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdVeh == idVeh && elt.StatutEF == "Facturé" && elt.IdFD.HasValue && elt.LIGNE_PRIX.CodeArticle == codeArt
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ELEMENT_FACTURATION> GetElementDITByIdCtr(int idCtr)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdCtr == idCtr && elt.PUEF != 0 && elt.LIGNE_PRIX.ARTICLE.CodeFamArt == 14
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ELEMENT_FACTURATION> GetElementFactDITByIdBL(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.PUEF != 0 && elt.LIGNE_PRIX.ARTICLE.CodeFamArt == 14
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ElementFacturation> GetElementDITByIdBL(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.PUEF != 0 && elt.LIGNE_PRIX.ARTICLE.CodeFamArt == 14
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ElementFacturation> GetElementDITByIdBLNonFacture(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.PUEF != 0 && elt.LIGNE_PRIX.ARTICLE.CodeFamArt == 14 && !elt.IdFactDIT.HasValue
                    orderby elt.IdJEF, elt.LibEF ascending
                    select new ElementFacturation
                    {
                        IdElt = elt.IdJEF,
                        CodeArticle = elt.LIGNE_PRIX.CodeArticle.Value,
                        LibArticle = elt.LibEF,
                        Qte = elt.QTEEF.Value,
                        Unite = elt.UnitEF,
                        PrixUnitaire = elt.PUEF.Value,
                        MontantHT = Math.Round(elt.PUEF.Value * elt.QTEEF.Value, 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round((elt.PUEF * elt.QTEEF * elt.TauxTVA / 100).Value, 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round((elt.PUEF * elt.QTEEF * (1 + elt.TauxTVA / 100)).Value, 0, MidpointRounding.AwayFromZero),
                        IsProforma = elt.LIGNE_PROFORMA.Count != 0 ? true : false,
                        IsFacture = elt.IdFD != null ? true : false,
                        IsNew = (elt.LIGNE_PROFORMA.Count == 0 && elt.IdFD == null) ? true : false,
                        Statut = elt.StatutEF
                    }).ToList<ElementFacturation>();
        }

        public List<ELEMENT_FACTURATION> GetElementFacturationGCByIdGC(int idGC)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdGC == idGC && elt.PUEF != 0 && !elt.IdFD.HasValue //&& elt.LIGNE_PRIX.CodeArticle != 1802
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ELEMENT_FACTURATION> GetElementFacturationVehByIdVeh(int idVeh)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdVeh == idVeh && elt.PUEF != 0 && !elt.IdFD.HasValue  //&& elt.LIGNE_PRIX.CodeArticle != 1802
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ELEMENT_FACTURATION> GetElementFacturationBLByIdBL(int idBL)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdBL == idBL && elt.PUEF != 0 && !elt.IdFD.HasValue  //&& elt.LIGNE_PRIX.CodeArticle != 1802
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ELEMENT_FACTURATION> GetElementFacturationMafiByIdMafi(int idMafi)
        {
            return (from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where elt.IdMafi == idMafi && elt.PUEF != 0 && !elt.IdFD.HasValue  //&& elt.LIGNE_PRIX.CodeArticle != 1802 && elt.LIGNE_PRIX.CodeArticle != 1805
                    orderby elt.IdJEF, elt.LibEF ascending
                    select elt).ToList<ELEMENT_FACTURATION>();
        }

        public List<ElementFacturation> GetElementSOCARBESCBL(int idBL)
        {
            DateTime dte = DateTime.Now;

            List<ElementFacturation> eltsBESC = new List<ElementFacturation>();

            //selection de la ligne de prix SOCAR : BESC
            ARTICLE articleDeboursSOCARBESC = (from art in dcPar.GetTable<ARTICLE>()
                                               from par in dcPar.GetTable<PARAMETRE>()
                                               where art.CodeArticle == par.CodeAF && par.NomAF == "Débours SOCAR : BESC"
                                               select art).SingleOrDefault<ARTICLE>();

            LIGNE_PRIX lpDeboursSOCARBESC = articleDeboursSOCARBESC.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

            CONNAISSEMENT con = (from bl in dcPar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

            ElementFacturation eltVeh = new ElementFacturation();
            eltVeh.CodeArticle = articleDeboursSOCARBESC.CodeArticle;
            eltVeh.LibArticle = articleDeboursSOCARBESC.LibArticle + " - Veh";
            eltVeh.Qte = con.VEHICULE.Count(veh => !veh.IdVehAP.HasValue);
            eltVeh.Unite = lpDeboursSOCARBESC.UniteLP;
            if (eltVeh.Qte > 0)
            {
                eltVeh.PrixUnitaire = lpDeboursSOCARBESC.PU1LP.Value;
                if (con.CODE_TVA.TauxTVA == 0 || lpDeboursSOCARBESC.ARTICLE.CODE_TVA.TauxTVA == 0)
                {
                    eltVeh.MontantTVA = 0;
                    eltVeh.MontantHT = Math.Round(eltVeh.PrixUnitaire * eltVeh.Qte, 0, MidpointRounding.AwayFromZero);
                    eltVeh.MontantTTC = eltVeh.MontantHT;
                }
                else
                {
                    eltVeh.MontantHT = Math.Round(eltVeh.PrixUnitaire * eltVeh.Qte, 0, MidpointRounding.AwayFromZero);
                    eltVeh.MontantTVA = Math.Round(eltVeh.PrixUnitaire * eltVeh.Qte * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                    eltVeh.MontantTTC = Math.Round(eltVeh.PrixUnitaire * eltVeh.Qte + eltVeh.MontantTVA, 0, MidpointRounding.AwayFromZero);
                }

                eltsBESC.Add(eltVeh);
            }

            ElementFacturation eltCtr20 = new ElementFacturation();
            eltCtr20.CodeArticle = articleDeboursSOCARBESC.CodeArticle;
            eltCtr20.LibArticle = articleDeboursSOCARBESC.LibArticle + " Ctr 20";
            eltCtr20.Qte = con.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "20");
            eltCtr20.Unite = lpDeboursSOCARBESC.UniteLP;
            if (eltCtr20.Qte > 0)
            {
                eltCtr20.PrixUnitaire = lpDeboursSOCARBESC.PU1LP.Value;
                if (con.CODE_TVA.TauxTVA == 0 || lpDeboursSOCARBESC.ARTICLE.CODE_TVA.TauxTVA == 0)
                {
                    eltCtr20.MontantTVA = 0;
                    eltCtr20.MontantHT = Math.Round(eltCtr20.PrixUnitaire * eltCtr20.Qte, 0, MidpointRounding.AwayFromZero);
                    eltCtr20.MontantTTC = eltCtr20.MontantHT;
                }
                else
                {
                    eltCtr20.MontantHT = Math.Round(eltCtr20.PrixUnitaire * eltCtr20.Qte, 0, MidpointRounding.AwayFromZero);
                    eltCtr20.MontantTVA = Math.Round(eltCtr20.PrixUnitaire * eltCtr20.Qte * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                    eltCtr20.MontantTTC = Math.Round(eltCtr20.PrixUnitaire * eltCtr20.Qte + eltCtr20.MontantTVA, 0, MidpointRounding.AwayFromZero);
                }

                eltsBESC.Add(eltCtr20);
            }

            ElementFacturation eltCtr40 = new ElementFacturation();
            eltCtr40.CodeArticle = articleDeboursSOCARBESC.CodeArticle;
            eltCtr40.LibArticle = articleDeboursSOCARBESC.LibArticle + " Ctr 40";
            eltCtr40.Qte = con.CONTENEUR.Count(ctr => ctr.TypeCCtr.Substring(0, 2) == "40");
            eltCtr40.Unite = lpDeboursSOCARBESC.UniteLP;
            if (eltCtr40.Qte > 0)
            {
                eltCtr40.PrixUnitaire = lpDeboursSOCARBESC.PU1LP.Value;
                if (con.CODE_TVA.TauxTVA == 0 || lpDeboursSOCARBESC.ARTICLE.CODE_TVA.TauxTVA == 0)
                {
                    eltCtr40.MontantTVA = 0;
                    eltCtr40.MontantHT = Math.Round(eltCtr40.PrixUnitaire * eltCtr40.Qte, 0, MidpointRounding.AwayFromZero);
                    eltCtr40.MontantTTC = eltCtr40.MontantHT;
                }
                else
                {
                    eltCtr40.MontantHT = Math.Round(eltCtr40.PrixUnitaire * eltCtr40.Qte, 0, MidpointRounding.AwayFromZero);
                    eltCtr40.MontantTVA = Math.Round(eltCtr40.PrixUnitaire * eltCtr40.Qte * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                    eltCtr40.MontantTTC = Math.Round(eltCtr40.PrixUnitaire * eltCtr40.Qte + eltCtr40.MontantTVA, 0, MidpointRounding.AwayFromZero);
                }

                eltsBESC.Add(eltCtr40);
            }

            return eltsBESC;
        }

        public List<ElementFacturation> GetElementSOCARFretBL(int idBL, string isSocar, double montantFret)
        {
            DateTime dte = DateTime.Now;

            //selection de la ligne de prix SOCAR : Fret
            ARTICLE articleDeboursSOCARFret = (from art in dcPar.GetTable<ARTICLE>()
                                               from par in dcPar.GetTable<PARAMETRE>()
                                               where art.CodeArticle == par.CodeAF && par.NomAF == "Débours SOCAR : Fret à collecter"
                                               select art).SingleOrDefault<ARTICLE>();

            LIGNE_PRIX lpDeboursSOCARFret = articleDeboursSOCARFret.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte && lp.LP == isSocar);

            //selection de la ligne de prix SOCAR : Commission
            ARTICLE articleCommissionSOCARFret = (from art in dcPar.GetTable<ARTICLE>()
                                                  from par in dcPar.GetTable<PARAMETRE>()
                                                  where art.CodeArticle == par.CodeAF && par.NomAF == "Commission sur Fret à collecter"
                                                  select art).SingleOrDefault<ARTICLE>();

            LIGNE_PRIX lpCommissionSOCARFret = articleCommissionSOCARFret.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

            CONNAISSEMENT con = (from bl in dcPar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

            List<ElementFacturation> elementsFret = new List<ElementFacturation>();

            ElementFacturation eltFret = new ElementFacturation();
            eltFret.CodeArticle = articleDeboursSOCARFret.CodeArticle;
            eltFret.LibArticle = lpDeboursSOCARFret.DescLP;
            eltFret.Qte = 1;
            eltFret.Unite = lpDeboursSOCARFret.UniteLP;
            eltFret.PrixUnitaire = montantFret;
            if (con.CODE_TVA.TauxTVA == 0 || lpDeboursSOCARFret.ARTICLE.CODE_TVA.TauxTVA == 0)
            {
                eltFret.MontantTVA = 0;
                eltFret.MontantHT = montantFret;
                eltFret.MontantTTC = montantFret;
            }
            else
            {
                eltFret.MontantHT = montantFret;
                eltFret.MontantTVA = Math.Round(montantFret * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                eltFret.MontantTTC = montantFret + eltFret.MontantTVA;
            }

            elementsFret.Add(eltFret);

            if (montantFret >= 1000000)
            {
                ElementFacturation eltCommission = new ElementFacturation();
                eltCommission.CodeArticle = articleCommissionSOCARFret.CodeArticle;
                eltCommission.LibArticle = articleCommissionSOCARFret.LibArticle;
                eltCommission.Qte = 1;
                eltCommission.Unite = lpCommissionSOCARFret.UniteLP;
                eltCommission.PrixUnitaire = Math.Round(montantFret * 0.03, 0, MidpointRounding.AwayFromZero);
                if (con.CODE_TVA.TauxTVA == 0 || lpCommissionSOCARFret.ARTICLE.CODE_TVA.TauxTVA == 0)
                {
                    eltCommission.MontantTVA = 0;
                    eltCommission.MontantHT = Math.Round(montantFret * 0.03, 0, MidpointRounding.AwayFromZero);
                    eltCommission.MontantTTC = eltCommission.MontantHT;
                }
                else
                {
                    eltCommission.MontantHT = Math.Round(montantFret * 0.03, 0, MidpointRounding.AwayFromZero);
                    eltCommission.MontantTVA = Math.Round(eltCommission.MontantHT * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                    eltCommission.MontantTTC = eltCommission.MontantHT + eltCommission.MontantTVA;
                }

                elementsFret.Add(eltCommission);
            }

            return elementsFret;
        }

        public ElementFacturation GetElementSOCARDetteBL(int idBL, double montantDette)
        {
            DateTime dte = DateTime.Now;

            //selection de la ligne de prix SOCAR : Dette
            ARTICLE articleDeboursSOCARDette = (from art in dcPar.GetTable<ARTICLE>()
                                                from par in dcPar.GetTable<PARAMETRE>()
                                                where art.CodeArticle == par.CodeAF && par.NomAF == "Débours SOCAR : Dette à collecter"
                                                select art).SingleOrDefault<ARTICLE>();

            LIGNE_PRIX lpDeboursSOCARDette = articleDeboursSOCARDette.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

            CONNAISSEMENT con = (from bl in dcPar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

            ElementFacturation elt = new ElementFacturation();
            elt.CodeArticle = articleDeboursSOCARDette.CodeArticle;
            elt.LibArticle = articleDeboursSOCARDette.LibArticle;
            elt.Qte = 1;
            elt.Unite = lpDeboursSOCARDette.UniteLP;
            elt.PrixUnitaire = montantDette;
            if (con.CODE_TVA.TauxTVA == 0 || lpDeboursSOCARDette.ARTICLE.CODE_TVA.TauxTVA == 0)
            {
                elt.MontantTVA = 0;
                elt.MontantHT = montantDette;
                elt.MontantTTC = montantDette;
            }
            else
            {
                elt.MontantHT = montantDette;
                elt.MontantTVA = Math.Round(montantDette * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                elt.MontantTTC = montantDette + elt.MontantTVA;
            }
            return elt;
        }

        public ElementFacturation GetElementFraisReleaseBL(int idBL)
        {
            DateTime dte = DateTime.Now;

            //selection de la ligne de prix SOCAR : Dette
            ARTICLE articleFraisReleaseBL = (from art in dcPar.GetTable<ARTICLE>()
                                             from par in dcPar.GetTable<PARAMETRE>()
                                             where art.CodeArticle == par.CodeAF && par.NomAF == "BL Express Release Fee"
                                             select art).SingleOrDefault<ARTICLE>();

            LIGNE_PRIX lpFraisReleaseBL = articleFraisReleaseBL.LIGNE_PRIX.FirstOrDefault<LIGNE_PRIX>(lp => lp.DDLP <= dte && lp.DFLP >= dte);

            CONNAISSEMENT con = (from bl in dcPar.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

            ElementFacturation elt = new ElementFacturation();
            elt.CodeArticle = articleFraisReleaseBL.CodeArticle;
            elt.LibArticle = articleFraisReleaseBL.LibArticle;
            elt.PrixUnitaire = lpFraisReleaseBL.PU1LP.Value;
            elt.Qte = 1;
            elt.Unite = lpFraisReleaseBL.UniteLP;
            elt.PrixUnitaire = elt.PrixUnitaire;
            if (con.CODE_TVA.TauxTVA == 0 || lpFraisReleaseBL.ARTICLE.CODE_TVA.TauxTVA == 0)
            {
                elt.MontantTVA = 0;
                elt.MontantHT = elt.PrixUnitaire;
                elt.MontantTTC = elt.PrixUnitaire;
            }
            else
            {
                elt.MontantHT = elt.PrixUnitaire;
                elt.MontantTVA = Math.Round(elt.PrixUnitaire * con.CODE_TVA.TauxTVA.Value / 100, 0, MidpointRounding.AwayFromZero);
                elt.MontantTTC = elt.PrixUnitaire + elt.MontantTVA;
            }
            return elt;
        }

        public List<ElementCompteEscale> GetValeurEscale(int idEsc)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdEsc == idEsc && eltFact.PUEF != 0 && eltFact.QTEEF != 0
                    group eltFact by eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.LibFamArt into g
                    select new ElementCompteEscale
                    {
                        FamilleService = g.Key,
                        MontantHT = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round(Convert.ToDouble(g.Sum(eltFact => (eltFact.PUEF * eltFact.QTEEF * eltFact.TauxTVA) / 100).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF * (1 + eltFact.TauxTVA / 100)).Value), 0, MidpointRounding.AwayFromZero)
                    }).ToList<ElementCompteEscale>();
        }

        public List<ElementCompteDIT> GetCompteDITEscale(int idEsc)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdEsc == idEsc && eltFact.PUEF != 0 && eltFact.QTEEF != 0 && eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.CodeFamArt == 14
                    group eltFact by eltFact.LIGNE_PRIX.DescLP into g
                    select new ElementCompteDIT
                    {
                        RubriqueFacturation = g.Key,
                        MontantFacture = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                        MontantDIT = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PTDIT).Value), 0, MidpointRounding.AwayFromZero)
                    }).ToList<ElementCompteDIT>();
        }

        public List<ElementCompteFactureDIT> GetCompteFacturationDITEscale(int idEsc)
        {
            return (from fact in dcPar.GetTable<FACTURE_DIT>()
                    from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdEsc == idEsc && bl.IdBL == fact.IdBL
                    select new ElementCompteFactureDIT
                    {
                        NumFacture = fact.NumFactDIT,
                        DateFacturation = fact.DateFact.Value,
                        NumBL = fact.CONNAISSEMENT.NumBL,
                        MontantFacture = fact.MntFact.Value,
                        ConsigneeBL = fact.CONNAISSEMENT.ConsigneeBL
                    }).ToList<ElementCompteFactureDIT>();
        }

        public List<ElementCompteSEPBC> GetCompteSEPBCEscale(int idEsc)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdEsc == idEsc && eltFact.PUEF != 0 && eltFact.QTEEF != 0 && eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.CodeFamArt == 11
                    group eltFact by eltFact.LIGNE_PRIX.DescLP into g
                    select new ElementCompteSEPBC
                    {
                        RubriqueFacturation = g.Key,
                        MontantFacture = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                    }).ToList<ElementCompteSEPBC>();
        }

        public List<ElementCompteDIT> GetCompteDITManifeste(int idMan)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdMan == idMan && eltFact.PUEF != 0 && eltFact.QTEEF != 0 && eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.CodeFamArt == 14
                    group eltFact by eltFact.LIGNE_PRIX.DescLP into g
                    select new ElementCompteDIT
                    {
                        RubriqueFacturation = g.Key,
                        MontantFacture = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                        MontantDIT = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PTDIT).Value), 0, MidpointRounding.AwayFromZero)
                    }).ToList<ElementCompteDIT>();
        }

        public List<ElementSituationCompteClient> GetSituationCompteClient(int idClient)
        {
            List<ElementSituationCompteClient> eltCpteClient = new List<ElementSituationCompteClient>();
            var cons = (from con in dcPar.GetTable<CONNAISSEMENT>()
                        where con.IdClient == idClient && con.SensBL == "I"
                        select con).ToList<CONNAISSEMENT>();
            ElementSituationCompteClient eltConnaissement = new ElementSituationCompteClient();
            eltConnaissement.Rubrique = "Connaissements";
            eltConnaissement.Nombre = cons.Count;
            eltConnaissement.MontantTotal = Math.Round(Convert.ToDouble(cons.Sum(c => c.ELEMENT_FACTURATION.ToList<ELEMENT_FACTURATION>().Sum(el => el.PUEF * el.QTEEF * (1 + el.TauxTVA / 100).Value))), 0, MidpointRounding.AwayFromZero);
            eltConnaissement.Encours = cons.Count(c => c.StatutBL != "C");
            eltConnaissement.ValeurEncours = Math.Round(Convert.ToDouble(cons.Where(cn => cn.StatutBL != "C").Sum(c => c.ELEMENT_FACTURATION.ToList<ELEMENT_FACTURATION>().Sum(el => el.PUEF * el.QTEEF * (1 + el.TauxTVA / 100).Value))), 0, MidpointRounding.AwayFromZero);
            eltCpteClient.Add(eltConnaissement);

            var facts = (from fact in dcPar.GetTable<FACTURE>()
                         where fact.IdClient == idClient
                         select fact).ToList<FACTURE>();
            ElementSituationCompteClient eltFacture = new ElementSituationCompteClient();
            eltFacture.Rubrique = "Factures";
            eltFacture.Nombre = facts.Count;
            eltFacture.MontantTotal = Math.Round(Convert.ToDouble(facts.Where(fc => fc.StatutFD == "O").Sum(f => f.MTTC)), 0, MidpointRounding.AwayFromZero);
            eltFacture.Encours = facts.Count(fc => fc.PROFORMA.CONNAISSEMENT.PAYEMENT.Count == 0);
            eltFacture.ValeurEncours = Math.Round(Convert.ToDouble(facts.Where(fc => fc.PROFORMA.CONNAISSEMENT.PAYEMENT.Count == 0).Sum(f => f.MTTC)), 0, MidpointRounding.AwayFromZero);
            eltCpteClient.Add(eltFacture);

            var payements = (from pay in dcPar.GetTable<PAYEMENT>()
                             from bl in dcPar.GetTable<CONNAISSEMENT>()
                             where pay.IdBL == bl.IdBL && bl.IdClient == idClient
                             select pay).ToList<PAYEMENT>();
            ElementSituationCompteClient eltPayement = new ElementSituationCompteClient();
            eltPayement.Rubrique = "Paiements";
            eltPayement.Nombre = payements.Count;
            eltPayement.MontantTotal = Math.Round(Convert.ToDouble(payements.Sum(p => p.MAPay)), 0, MidpointRounding.AwayFromZero);
            eltCpteClient.Add(eltPayement);
            return eltCpteClient;
        }

        public List<ElementCompteManifeste> GetCompteManifeste(int idMan)
        {
            return (from eltFact in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where eltFact.IdMan == idMan && eltFact.PUEF != 0 && eltFact.QTEEF != 0
                    group eltFact by eltFact.LIGNE_PRIX.ARTICLE.FAMILLE_ARTICLE.LibFamArt into g
                    select new ElementCompteManifeste
                    {
                        FamilleService = g.Key,
                        MontantHT = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTVA = Math.Round(Convert.ToDouble(g.Sum(eltFact => (eltFact.PUEF * eltFact.QTEEF * eltFact.TauxTVA) / 100).Value), 0, MidpointRounding.AwayFromZero),
                        MontantTTC = Math.Round(Convert.ToDouble(g.Sum(eltFact => eltFact.PUEF * eltFact.QTEEF * (1 + eltFact.TauxTVA / 100)).Value), 0, MidpointRounding.AwayFromZero)
                    }).ToList<ElementCompteManifeste>();
        }

        #endregion

        #region Proforma

        public List<PROFORMA> GetProformas()
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    orderby prof.IdFP descending
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasEnAttente()
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.DVFP == null && prof.StatutFP != "A"
                    orderby prof.IdFP descending
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasValidees()
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.DVFP != null && prof.StatutFP != "A"
                    orderby prof.IdFP descending
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasAnnulees()
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.StatutFP == "A"
                    orderby prof.IdFP descending
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasOfConnaissement(int idBL)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.IdBL == idBL
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasOfConnaissementEnAttente(int idBL)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.IdBL == idBL && !prof.DVFP.HasValue && prof.StatutFP != "A"
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasByNumBL(string numBL)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.CONNAISSEMENT.NumBL.Contains(numBL)
                    select prof).ToList<PROFORMA>();
        }

        public List<PROFORMA> GetProformasByNumEsc(string numEsc)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.CONNAISSEMENT.ESCALE.NumEsc.ToString().Contains(numEsc)
                    select prof).ToList<PROFORMA>();
        }

        public PROFORMA GetProformaByIdProf(int idProf)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    where prof.IdFP == idProf
                    select prof).SingleOrDefault<PROFORMA>();
        }

        public List<PROFORMA> GetProformaByIdCtrAndArticle(int idCtr, int codeArticle)
        {
            return (from prof in dcPar.GetTable<PROFORMA>()
                    from lg in dcPar.GetTable<LIGNE_PROFORMA>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where prof.IdFP == lg.IdFP && lg.IdJEF == elt.IdJEF && elt.IdCtr == idCtr && elt.LIGNE_PRIX.CodeArticle == codeArticle
                    orderby prof.IdFP
                    select prof).ToList<PROFORMA>();
        }

        #endregion

        #region Avoir

        public List<AVOIR> GetAvoirs()
        {
            return (from av in dcPar.GetTable<AVOIR>()
                    orderby av.IdFA descending
                    select av).ToList<AVOIR>();
        }

        public List<AVOIR> GetAvoirsCurrentYear()
        {
            return (from av in dcPar.GetTable<AVOIR>()
                    where av.DCFA.Value.Year == DateTime.Today.Year
                    orderby av.IdFA descending
                    select av).ToList<AVOIR>();
        }

        public List<AVOIR> GetAvoirsEnAttente()
        {
            return (from av in dcPar.GetTable<AVOIR>()
                    where av.DVFA == null && av.StatutFA != "A"
                    orderby av.IdFA descending
                    select av).ToList<AVOIR>();
        }

        public List<AVOIR> GetAvoirsValides()
        {
            return (from av in dcPar.GetTable<AVOIR>()
                    where av.DVFA != null && av.StatutFA != "A"
                    orderby av.IdFA descending
                    select av).ToList<AVOIR>();
        }

        public AVOIR GetAvoirByIdAvoir(int idAvoir)
        {
            return (from av in dcPar.GetTable<AVOIR>()
                    where av.IdFA == idAvoir
                    select av).SingleOrDefault<AVOIR>();
        }

        #endregion

        #region Facture

        public List<FACTURE> GetFactures()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public FACTURE GetFactureByIdFact(int idFact)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.IdFD == idFact
                    select fact).SingleOrDefault<FACTURE>();
        }

        public FACTURE GetFactureByIdDocSAP(int idFactSAP)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.IdDocSAP == idFactSAP
                    && fact.IdFP != null //AH pour eliminer les facture spot qui ne sont lie a aucun bl
                    select fact).SingleOrDefault<FACTURE>();
        }

        public FACTURE GetFactureSpotByIdDocSAP(int idFactSAP)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.IdDocSAP == idFactSAP && fact.IdFP == null
                    select fact).SingleOrDefault<FACTURE>();
        }

        public List<ElementLigneFactureSpot> GetFactureSpotElement(int idFactNOVA)
        {
            return (from m in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where m.IdFD == idFactNOVA
                    select new ElementLigneFactureSpot
                    {
                        Code = m.CodeArticle,
                        Libelle = m.LibEF,
                        PrixUnitaire = (int)Convert.ToInt32(m.PUEF),
                        Qte = (float)Convert.ToDouble(m.QTEEF),
                        //TVA = ((float)m.TauxTVA * (int)m.PUEF) ,
                        Unite = m.UnitEF,
                        //PrixTotal = (float)(m.QTEEF * m.PUEF) + ((float)m.TauxTVA * (int)m.PUEF) 
                        PrixTotal = (float)Convert.ToDouble(m.PUEF * m.QTEEF * ((m.CODE_TVA.TauxTVA.ToString()=="0" ) ?1 :  1.1925)),
                        TVA = (float)Convert.ToDouble(m.PUEF * m.QTEEF * ((m.CODE_TVA.TauxTVA.ToString()=="0") ? 0: 0.1925))

                    }).ToList<ElementLigneFactureSpot>();
        }

        public List<FACTURE> GetFactureSpot(DateTime debut, DateTime fin)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.DCFD >= debut && fact.DCFD <= fin && fact.IdFP == null
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesEnAttentePaiement()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where !fact.IdPay.HasValue && fact.StatutFD != "A"
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFactureByIdCtrAndCodeArticle(int idCtr, int codeArticle)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    from prof in dcPar.GetTable<PROFORMA>()
                    from lg in dcPar.GetTable<LIGNE_PROFORMA>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where prof.IdFP == lg.IdFP && lg.IdJEF == elt.IdJEF && elt.IdCtr == idCtr && fact.IdFP == prof.IdFP && elt.LIGNE_PRIX.CodeArticle == codeArticle
                    orderby fact.IdDocSAP
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementBL()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.StatutFD != "A" && fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementBLEnAttentePaiement()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where !fact.IdPay.HasValue && fact.StatutFD != "A" && fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementVeh()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementVehEnAttentePaiement()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where !fact.IdPay.HasValue && fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }
        

       /// <summary>
       /// get spot facture for client
       /// </summary>
       /// <param name="idclient"></param>
       /// <returns></returns>
        public List<FACTURE> getFactureUnPaidByClient(int idclient)
        {
            return (from m in dcPar.GetTable<FACTURE>()
                    where m.IdClient == idclient && m.StatutFD == "O" && m.IdFP== null
                    orderby m.DCFD
                    select m).ToList<FACTURE>();
        }

        public List<FACTURE> getFactureAllUnPaidByClient(int idclient)
        {
            return (from m in dcPar.GetTable<FACTURE>()
                    where m.IdClient == idclient && m.StatutFD == "O" && m.DCFD> DateTime.Parse("01/01/2015")
                    orderby m.DCFD descending
                    select m).ToList<FACTURE>();
        }

        public List<AVOIR> GetAvailableAvoirSpotByClient(int idclient)
        {
            return (from m in dcPar.GetTable<AVOIR>()
                    where m.IdClient == idclient && m.TypeAvoir == "Spot" && m.IdPay==null
                    orderby m.DCFA
                    select m).ToList<AVOIR>();
        }

        public List<FACTURE> GetFacturesElementCtr()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementCtrEnAttentePaiement()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where !fact.IdPay.HasValue && fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && lg.ELEMENT_FACTURATION.IdCtr.HasValue && !lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementGC()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesElementGCEnAttentePaiement()
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where !fact.IdPay.HasValue && fact.StatutFD != "A" && (fact.PROFORMA.LIGNE_PROFORMA.Count(lg => lg.ELEMENT_FACTURATION.IdBL.HasValue && !lg.ELEMENT_FACTURATION.IdVeh.HasValue && !lg.ELEMENT_FACTURATION.IdCtr.HasValue && lg.ELEMENT_FACTURATION.IdGC.HasValue) != 0 || fact.PROFORMA.CONNAISSEMENT.CONTENEUR.Count != 0)
                    orderby fact.IdFD descending
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesOfConnaissement(int idBL)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.PROFORMA.IdBL == idBL
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesByNumBL(string numBL)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.PROFORMA.CONNAISSEMENT.NumBL.Contains(numBL)
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesByEscale(string numEsc)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc.ToString().Contains(numEsc)
                    select fact).ToList<FACTURE>();
        }

        //public List<FACTURE> GetFacturesOfConnaissementPayees(int idBL, int idPay)
        //{
        //    return (from fact in dcPar.GetTable<FACTURE>()
        //            where fact.PROFORMA.IdBL == idBL && fact.IdPay == idPay
        //            select fact).ToList<FACTURE>();
        //}

        public List<FACTURE> GetFacturesOfConnaissementPayees(int idBL, int idPay)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.PROFORMA.IdBL == idBL && fact.LIGNE_PAYEMENT.Count(lp => lp.IdPay == idPay) != 0
                    select fact).ToList<FACTURE>();
        }

        public List<FACTURE> GetFacturesNonPayeesOfConnaissement(int idBL)
        {
            return (from fact in dcPar.GetTable<FACTURE>()
                    where fact.PROFORMA.IdBL == idBL && !fact.IdPay.HasValue && fact.StatutFD == "O"
                    select fact).ToList<FACTURE>();
        }

        //public List<FACTURE> GetFacturesNonPayeesOfConnaissement(int idBL)
        //{
        //    return (from fact in dcPar.GetTable<FACTURE>()
        //            where fact.PROFORMA.IdBL == idBL && fact.Solde > 10 && fact.StatutFD == "O"
        //            select fact).ToList<FACTURE>();
        //}

        #endregion

        #region Facture DIT

        public List<FACTURE_DIT> GetFacturesDIT()
        {
            return (from fact in dcPar.GetTable<FACTURE_DIT>()
                    orderby fact.IdFactDIT descending
                    select fact).ToList<FACTURE_DIT>();
        }

        public FACTURE_DIT GetFacturesDITByIdFact(int idFactDIT)
        {
            return (from fact in dcPar.GetTable<FACTURE_DIT>()
                    where fact.IdFactDIT == idFactDIT
                    orderby fact.IdFactDIT descending
                    select fact).SingleOrDefault<FACTURE_DIT>();
        }

        public List<FACTURE_DIT> GetFacturesDITByNumBL(string numBL)
        {
            return (from fact in dcPar.GetTable<FACTURE_DIT>()
                    where fact.CONNAISSEMENT.NumBL.Contains(numBL)
                    orderby fact.IdFactDIT descending
                    select fact).ToList<FACTURE_DIT>();
        }

        public List<FACTURE_DIT> GetFacturesDITByNumEsc(int numEsc)
        {
            return (from fact in dcPar.GetTable<FACTURE_DIT>()
                    where fact.CONNAISSEMENT.ESCALE.NumEsc.ToString().Contains(numEsc.ToString())
                    orderby fact.IdFactDIT descending
                    select fact).ToList<FACTURE_DIT>();
        }

        #endregion

        #region Paiement

        public List<PAYEMENT> GetPaiements()
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsFacture()
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.FACTURE.Count != 0
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsCaution()
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.CONTENEUR.Count != 0
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsSOCAR()
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.FACTURE.Count(f => f.PROFORMA.LIGNE_PROFORMA.Count(lp => lp.ELEMENT_FACTURATION.LibEF.Contains("SOCAR")) != 0) != 0
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsByNumBL(string numBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.CONNAISSEMENT.NumBL.Contains(numBL)
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsByNumCtr(string numCtr)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.CONTENEUR.Count(ctr => ctr.NumCtr.Contains(numCtr)) != 0
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsByNumChassis(string numChassis)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.CONNAISSEMENT.VEHICULE.Count(v => v.NumChassis.Contains(numChassis)) != 0
                    orderby pay.IdPay descending
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementByIdFact(int idFact)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.FACTURE.Count(f => f.IdDocSAP == idFact) != 0
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementByIdEscale(int numEsc)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.CONNAISSEMENT.ESCALE.NumEsc == numEsc
                    select pay).ToList<PAYEMENT>();
        }

        public PAYEMENT GetPaiementByIdPay(int idPay)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.IdPay == idPay
                    select pay).SingleOrDefault<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsOfConnaissement(int idBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.IdBL == idBL
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsFactureOfConnaissement(int idBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.IdBL == idBL && (pay.ObjetPay == 0 || pay.ObjetPay == 1)
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetPaiementsCautionOfConnaissement(int idBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.IdBL == idBL && pay.ObjetPay == 2
                    select pay).ToList<PAYEMENT>();
        }

        public List<PAYEMENT> GetRestitutionCautionOfConnaissement(int idBL)
        {
            return (from pay in dcPar.GetTable<PAYEMENT>()
                    where pay.IdBL == idBL && pay.IdPayDRC.HasValue
                    select pay).ToList<PAYEMENT>();
        }

        #endregion

        #region Ordre de service

        public List<ORDRE_SERVICE> GetOrdresService()
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetOrdresServiceEnAttente()
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where !os.DClOS.HasValue
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        public List<ORDRE_SERVICE> GetOrdresServiceValides()
        {
            return (from os in dcPar.GetTable<ORDRE_SERVICE>()
                    where os.DClOS.HasValue
                    orderby os.IdOS descending
                    select os).ToList<ORDRE_SERVICE>();
        }

        #endregion

        #region Bon à enlever

        public List<BON_ENLEVEMENT> GetBonsEnlever()
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        /// <summary>
        /// BAE validée dans la periode
        /// </summary>
        /// <param name="debu"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        public List<BON_ENLEVEMENT> GetBonsEnleverParcAuto(DateTime debu, DateTime fin)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.DateBAE >= debu && bon.DateBAE <= fin && bon.VEHICULE.Count() != 0 && bon.DVBAE.HasValue
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByIdAcc(int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByNumBL(string numBL)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.CONNAISSEMENT.NumBL.Contains(numBL) && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByNumBL(string numBL, int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.CONNAISSEMENT.NumBL.Contains(numBL) && bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByNumChassis(string numChassis)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByNumChassis(string numChassis, int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByConsignee(string consignee)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverByConsignee(string consignee, int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bon.CONNAISSEMENT.SensBL == "I"
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverEnAttente()
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where !bon.DVBAE.HasValue
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverEnAttenteByIdAcc(int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where !bon.DVBAE.HasValue && bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverValides()
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.DVBAE.HasValue
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        public List<BON_ENLEVEMENT> GetBonsEnleverValidesByIdAcc(int idAcc)
        {
            return (from bon in dcPar.GetTable<BON_ENLEVEMENT>()
                    where bon.DVBAE.HasValue && bon.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bon.IdBAE descending
                    select bon).ToList<BON_ENLEVEMENT>();
        }

        #endregion

        #region Demande de livraison

        public List<DEMANDE_LIVRAISON> GetDemandesLivraison()
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }
        /// <summary>
        /// demande de livraison cree vehicule ds une periode
        /// </summary>
        /// <param name="debu"></param>
        /// <param name="fin"></param>
        /// <returns></returns>
        public List<DEMANDE_LIVRAISON> GetDemandesLivraison(DateTime debu, DateTime fin)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.DateDBL >= debu && dl.DateDBL <= fin && dl.VEHICULE.Count > 0
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public DEMANDE_LIVRAISON GetDemandeLivraisonByIdDBL(int idDBL)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.IdDBL == idDBL
                    orderby dl.IdDBL descending
                    select dl).FirstOrDefault<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByIdAcc(int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByNumBL(string numBL)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.CONNAISSEMENT.NumBL.Contains(numBL) && dl.CONNAISSEMENT.SensBL == "I"
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByNumBL(string numBL, int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.CONNAISSEMENT.NumBL.Contains(numBL) && dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dl.CONNAISSEMENT.SensBL == "I"
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByNumChassis(string numChassis)
        {
            return (from dde in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dde.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && dde.CONNAISSEMENT.SensBL == "I"
                    orderby dde.IdDBL descending
                    select dde).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByNumChassis(string numChassis, int idAcc)
        {
            return (from dde in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dde.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && dde.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dde.CONNAISSEMENT.SensBL == "I"
                    orderby dde.IdDBL descending
                    select dde).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByConsignee(string consignee)
        {
            return (from dde in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dde.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && dde.CONNAISSEMENT.SensBL == "I"
                    orderby dde.IdDBL descending
                    select dde).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonByConsignee(string consignee, int idAcc)
        {
            return (from dde in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dde.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && dde.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dde.CONNAISSEMENT.SensBL == "I"
                    orderby dde.IdDBL descending
                    select dde).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonEnAttente()
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where !dl.DVDBL.HasValue
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonSansDossierPhysique()
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where !dl.DateDepotDBL.HasValue
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonAvecDossierPhysique()
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.DateDepotDBL.HasValue
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonEnAttenteByIdAcc(int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where !dl.DVDBL.HasValue && dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonAvecDossierPhysiqueByIdAcc(int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.DVDBL.HasValue && dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonSansDossierPhysiqueByIdAcc(int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where !dl.DVDBL.HasValue && dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonValides()
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.DVDBL.HasValue
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        public List<DEMANDE_LIVRAISON> GetDemandesLivraisonValidesByIdAcc(int idAcc)
        {
            return (from dl in dcPar.GetTable<DEMANDE_LIVRAISON>()
                    where dl.DVDBL.HasValue && dl.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dl.IdDBL descending
                    select dl).ToList<DEMANDE_LIVRAISON>();
        }

        #endregion

        #region Bon de sortie

        public List<BON_SORTIE> GetBonsSortie()
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieByIdAcc(int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieVeh()
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdVeh.HasValue
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieVeh(DateTime debu, DateTime fin)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdVeh.HasValue && bs.DateBS >= debu && bs.DateBS <= fin
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieVehByIdAcc(int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdVeh.HasValue && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieCtr()
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdCtr.HasValue
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieCtrByIdAcc(int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdCtr.HasValue && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieGC()
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdGC.HasValue
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonsSortieGCByIdAcc(int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.IdGC.HasValue && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByNumBL(string numBL)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.CONNAISSEMENT.NumBL.Contains(numBL) && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByNumBL(string numBL, int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.CONNAISSEMENT.NumBL.Contains(numBL) && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByNumChassis(string numChassis)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByNumChassis(string numChassis, int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.VEHICULE.Count(veh => veh.NumChassis.Contains(numChassis)) != 0 && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByConsignee(string consignee)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        public List<BON_SORTIE> GetBonSortieByConsignee(string consignee, int idAcc)
        {
            return (from bs in dcPar.GetTable<BON_SORTIE>()
                    where bs.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && bs.CONNAISSEMENT.ESCALE.IdAcc == idAcc && bs.CONNAISSEMENT.SensBL == "I"
                    orderby bs.IdBS descending
                    select bs).ToList<BON_SORTIE>();
        }

        #endregion

        #region Demande de réduction

        public DEMANDE_REDUCTION GetDemandesReductionById(int idRed)
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    where red.IdDDR == idRed
                    select red).SingleOrDefault<DEMANDE_REDUCTION>();
        }

        public List<DEMANDE_REDUCTION> GetDemandesReductionByNumBL(string numBL)
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    where red.CONNAISSEMENT.NumBL.Contains(numBL)
                    orderby red.IdDDR descending
                    select red).ToList<DEMANDE_REDUCTION>();
        }

        public List<DEMANDE_REDUCTION> GetDemandesReduction()
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    orderby red.IdDDR descending
                    select red).ToList<DEMANDE_REDUCTION>();
        }

        public List<DEMANDE_REDUCTION> GetDemandesReductionEnAttente()
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    where !red.DatevDDR.HasValue && red.StatutRed == "En cours"
                    orderby red.IdDDR descending
                    select red).ToList<DEMANDE_REDUCTION>();
        }

        public List<DEMANDE_REDUCTION> GetDemandesReductionAnnulee()
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    where !red.DatevDDR.HasValue && red.StatutRed == "Annulé"
                    orderby red.IdDDR descending
                    select red).ToList<DEMANDE_REDUCTION>();
        }

        public List<DEMANDE_REDUCTION> GetDemandesReductionValidees()
        {
            return (from red in dcPar.GetTable<DEMANDE_REDUCTION>()
                    where red.DatevDDR.HasValue
                    orderby red.IdDDR descending
                    select red).ToList<DEMANDE_REDUCTION>();
        }

        public List<ARTICLE> GetArticlesFacturablesByIdBL(int idBL)
        {
            return (from art in dcPar.GetTable<ARTICLE>()
                    from elt in dcPar.GetTable<ELEMENT_FACTURATION>()
                    where art.CodeArticle == elt.LIGNE_PRIX.ARTICLE.CodeArticle && (elt.LIGNE_PRIX.ARTICLE.CodeArticle == 1801 || elt.LIGNE_PRIX.ARTICLE.CodeArticle == 1805 || elt.LIGNE_PRIX.ARTICLE.CodeArticle == 1807 || elt.LIGNE_PRIX.ARTICLE.CodeArticle == 1815) && elt.IdBL == idBL
                    select art).Distinct<ARTICLE>().ToList<ARTICLE>();
        }

        #endregion

        #region Demande d'extension de franchise

        public EXTENSION_FRANCHISE GetExtensionFranchiseByIdExt(int idExt)
        {
            return (from ext in dcPar.GetTable<EXTENSION_FRANCHISE>()
                    where ext.IdDEXT == idExt
                    orderby ext.IdDEXT descending
                    select ext).SingleOrDefault<EXTENSION_FRANCHISE>();
        }

        public List<EXTENSION_FRANCHISE> GetExtensionsFranchise()
        {
            return (from ext in dcPar.GetTable<EXTENSION_FRANCHISE>()
                    orderby ext.IdDEXT descending
                    select ext).ToList<EXTENSION_FRANCHISE>();
        }

        public List<EXTENSION_FRANCHISE> GetExtensionsFranchiseEnAttente()
        {
            return (from ext in dcPar.GetTable<EXTENSION_FRANCHISE>()
                    where !ext.DatevDEXT.HasValue
                    orderby ext.IdDEXT descending
                    select ext).ToList<EXTENSION_FRANCHISE>();
        }

        public List<EXTENSION_FRANCHISE> GetExtensionsFranchiseValidees()
        {
            return (from ext in dcPar.GetTable<EXTENSION_FRANCHISE>()
                    where ext.DatevDEXT.HasValue
                    orderby ext.IdDEXT descending
                    select ext).ToList<EXTENSION_FRANCHISE>();
        }

        #endregion

        #region Demande de visite

        public List<DEMANDE_VISITE> GetDemandesVisite()
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisite(int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByNumBL(string numBL)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.CONNAISSEMENT.NumBL.Contains(numBL) && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByNumBL(string numBL, int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.CONNAISSEMENT.NumBL.Contains(numBL) && dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByNumChassis(string numChassis)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.VISITE_VEHICULE.VEHICULE.NumChassis.Contains(numChassis) && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByNumChassis(string numChassis, int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.VISITE_VEHICULE.VEHICULE.NumChassis.Contains(numChassis) && dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByConsignee(string consignee)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteByConsignee(string consignee, int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.CONNAISSEMENT.ConsigneeBL.Contains(consignee) && dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc && dv.CONNAISSEMENT.SensBL == "I"
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteEnAttente()
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where !dv.DVDV.HasValue
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteEnAttente(int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where !dv.DVDV.HasValue && dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteValidees()
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.DVDV.HasValue
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        public List<DEMANDE_VISITE> GetDemandesVisiteValidees(int idAcc)
        {
            return (from dv in dcPar.GetTable<DEMANDE_VISITE>()
                    where dv.DVDV.HasValue && dv.CONNAISSEMENT.ESCALE.IdAcc == idAcc
                    orderby dv.IdDV descending
                    select dv).ToList<DEMANDE_VISITE>();
        }

        #endregion

        #region Demande de restitution de caution

        public List<DEMANDE_CAUTION> GetDemandesRestitution()
        {
            return (from drc in dcPar.GetTable<DEMANDE_CAUTION>()
                    orderby drc.IdDRC descending
                    select drc).ToList<DEMANDE_CAUTION>();
        }

        public List<DEMANDE_CAUTION> GetDemandesRestitutionEnAttente()
        {
            return (from drc in dcPar.GetTable<DEMANDE_CAUTION>()
                    where !drc.DVDRC.HasValue
                    orderby drc.IdDRC descending
                    select drc).ToList<DEMANDE_CAUTION>();
        }

        public List<DEMANDE_CAUTION> GetDemandesRestitutionValidees()
        {
            return (from drc in dcPar.GetTable<DEMANDE_CAUTION>()
                    where drc.DVDRC.HasValue
                    orderby drc.IdDRC descending
                    select drc).ToList<DEMANDE_CAUTION>();
        }

        #endregion

        #region Statistiques

        public StatutLoadUnload GetStatutDechargementEsc(int idEsc)
        {
            return (from op in dcPar.GetTable<TYPE_OPERATION>()
                    select new StatutLoadUnload
                    {
                        NbVehicule = op.OPERATION_VEHICULE.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.VEHICULE.IdEsc == idEsc).GroupBy(v => v.IdVeh).ToList().Count,
                        NbConteneur = op.OPERATION_CONTENEUR.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONTENEUR.IdEsc == idEsc).GroupBy(c => c.CONTENEUR.NumCtr).ToList().Count,
                        NbMafi = op.OPERATION_MAFI.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.MAFI.IdEsc == idEsc).GroupBy(m => m.MAFI.NumMafi).ToList().Count,
                        NbConventionnel = op.OPERATION_CONVENTIONNEL.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONVENTIONNEL.IdEsc == idEsc).ToList().Count
                    }).FirstOrDefault<StatutLoadUnload>();
        }

        public StatutLoadUnload GetStatutDechargementMan(int idMan)
        {
            return (from op in dcPar.GetTable<TYPE_OPERATION>()
                    select new StatutLoadUnload
                    {
                        NbVehicule = op.OPERATION_VEHICULE.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.VEHICULE.IdMan == idMan).GroupBy(v => v.IdVeh).ToList().Count,
                        NbConteneur = op.OPERATION_CONTENEUR.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONTENEUR.IdMan == idMan).GroupBy(c => c.CONTENEUR.NumCtr).ToList().Count,
                        NbMafi = op.OPERATION_MAFI.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.MAFI.IdMan == idMan).GroupBy(m => m.MAFI.NumMafi).ToList().Count,
                        NbConventionnel = op.OPERATION_CONVENTIONNEL.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONVENTIONNEL.IdMan == idMan).ToList().Count
                    }).FirstOrDefault<StatutLoadUnload>();
        }

        public StatutLoadUnload GetStatutDechargementBL(int idBL)
        {
            return (from op in dcPar.GetTable<TYPE_OPERATION>()
                    select new StatutLoadUnload
                    {
                        NbVehicule = op.OPERATION_VEHICULE.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.VEHICULE.IdBL == idBL).GroupBy(v => v.IdVeh).ToList().Count,
                        NbConteneur = op.OPERATION_CONTENEUR.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONTENEUR.IdBL == idBL).GroupBy(c => c.CONTENEUR.NumCtr).ToList().Count,
                        NbMafi = op.OPERATION_MAFI.Where(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.MAFI.IdBL == idBL).GroupBy(m => m.MAFI.NumMafi).ToList().Count,
                        NbConventionnel = op.OPERATION_CONVENTIONNEL.Count(o => o.TYPE_OPERATION.LibTypeOp == "Identification" && o.CONVENTIONNEL.IdBL == idBL)
                    }).FirstOrDefault<StatutLoadUnload>();
        }

        public StatutLoadUnload GetStatutChargementBooking(int idBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdBL == idBL
                    select new StatutLoadUnload
                    {
                        NbConteneur = bl.CONTENEUR.Count(c => c.StatutCtr == "Cargo Loaded"),
                        NbConventionnel = bl.CONVENTIONNEL.Count(c => c.StatGC == "Cargo Loaded")
                    }).FirstOrDefault<StatutLoadUnload>();
        }

        public List<HistoriqueOperation> GetHistoriqueOperationsVeh(int idVeh)
        {
            return (from op in dcPar.GetTable<OPERATION_VEHICULE>()
                    where op.IdVeh == idVeh
                    orderby op.IdTypeOp ascending
                    select new HistoriqueOperation
                    {
                        Operation = op.TYPE_OPERATION.LibTypeOp,
                        DateOperation = op.DateOp.Value,
                        LieuOperation = op.LIEU.NomLieu,
                        ExecutePar = dcPar.GetTable<UTILISATEUR>().Where(user => user.IdU == op.IdU).SingleOrDefault<UTILISATEUR>().NU,
                        Commentaires = op.AIOp
                    }).ToList<HistoriqueOperation>();
        }

        public List<HistoriqueOperation> GetHistoriqueOperationsCtr(int idCtr)
        {
            return (from op in dcPar.GetTable<OPERATION_CONTENEUR>()
                    where op.IdCtr == idCtr
                    orderby op.IdTypeOp ascending
                    select new HistoriqueOperation
                    {
                        Operation = op.TYPE_OPERATION.LibTypeOp,
                        DateOperation = op.DateOp.Value,
                        ExecutePar = dcPar.GetTable<UTILISATEUR>().Where(user => user.IdU == op.IdU).SingleOrDefault<UTILISATEUR>().NU,
                        Commentaires = op.AIOp
                    }).ToList<HistoriqueOperation>();
        }

        public List<HistoriqueOperation> GetHistoriqueOperationsGC(int idGC)
        {
            return (from op in dcPar.GetTable<OPERATION_CONVENTIONNEL>()
                    where op.IdGC == idGC
                    orderby op.IdTypeOp ascending
                    select new HistoriqueOperation
                    {
                        Operation = op.TYPE_OPERATION.LibTypeOp,
                        DateOperation = op.DateOp.Value,
                        ExecutePar = dcPar.GetTable<UTILISATEUR>().Where(user => user.IdU == op.IdU).SingleOrDefault<UTILISATEUR>().NU,
                        Commentaires = op.AIOp
                    }).ToList<HistoriqueOperation>();
        }

        public List<HistoriqueOperation> GetHistoriqueOperationsMafi(int idMafi)
        {
            return (from op in dcPar.GetTable<OPERATION_MAFI>()
                    where op.IdMafi == idMafi
                    orderby op.IdTypeOp ascending
                    select new HistoriqueOperation
                    {
                        Operation = op.TYPE_OPERATION.LibTypeOp,
                        DateOperation = op.DateOp.Value,
                        ExecutePar = dcPar.GetTable<UTILISATEUR>().Where(user => user.IdU == op.IdU).SingleOrDefault<UTILISATEUR>().NU,
                        Commentaires = op.AIOp
                    }).ToList<HistoriqueOperation>();
        }

        public List<HistoriqueInterchange> GetInterchangeCtr(int idCtr)
        {
            return (from inter in dcPar.GetTable<INTERCHANGE>()
                    where inter.IdCtr == idCtr
                    orderby inter.IdTypeSinistre ascending
                    select new HistoriqueInterchange
                    {
                        Code = inter.IdTypeSinistre,
                        Type = dcPar.GetTable<TYPE_SINISTRE>().Where(t => t.IdTypeSinistre == inter.IdTypeSinistre).SingleOrDefault<TYPE_SINISTRE>().LibTypeSinistre,
                        Identification = inter.InfoIdentification,
                        Sortie = inter.InfoSortie,
                        Retour = inter.InfoRetour,
                        Parquing = inter.InfoParquing,
                        SortieVide = inter.InfoSortieVide,
                        RetourPlein = inter.InfoRetourPlein,
                        Embarquement = inter.InfoEmbarquement,
                        RetourArmateur = inter.InfoRetourArmateur
                    }).ToList<HistoriqueInterchange>();
        }

        #endregion

        #region connaissement

        public List<CONNAISSEMENT> GetConnaissementsImport()
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsImportByIdMan(int idMan)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.IdMan == idMan && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsFactures()
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.ELEMENT_FACTURATION.Count(elt => elt.IdFD.HasValue) != 0 && con.StatutBL != "Cloturé"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsPayablesByNumBL(string numBL)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.NumBL.Contains(numBL) && ((con.PROFORMA.Count(p => !p.DVFP.HasValue && p.LIGNE_PROFORMA.Count(lp => lp.ELEMENT_FACTURATION.IdFD.HasValue) == 0) > 0) || (con.ELEMENT_FACTURATION.Count(elt => elt.IdFD.HasValue) != 0) || (con.CONTENEUR.Count(ctr => !ctr.IdPay.HasValue) > 0)/* || (con.PROFORMA.Count(prof => prof.FACTURE.Count(f => f.Solde > 10) != 0) != 0)*/)// && con.StatutBL != "Cloturé"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsFacturablesByNumBL(string numBL)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.NumBL.Contains(numBL) && con.ELEMENT_FACTURATION.Count != 0// && con.StatutBL != "Cloturé"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsAFacturer()
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.ELEMENT_FACTURATION.Count(elt => elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFD.HasValue) != 0 && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsAFacturer(string numBL)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.NumBL.Contains(numBL) && con.ELEMENT_FACTURATION.Count(elt => elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFD.HasValue) != 0 && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsAFacturerByConsignee(string conBL)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.ConsigneeBL.Contains(conBL) && con.ELEMENT_FACTURATION.Count(elt => elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFD.HasValue) != 0 && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsAFacturerByEscale(int numEsc)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.ESCALE.NumEsc == numEsc && con.ELEMENT_FACTURATION.Count(elt => elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFD.HasValue) != 0 && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsAFacturer(int idMan)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.IdMan == idMan && con.ELEMENT_FACTURATION.Count(elt => elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFD.HasValue) != 0 && con.SensBL == "I"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsWithDITByNumBL(string numBL)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.NumBL.Contains(numBL) && con.ELEMENT_FACTURATION.Count(elt => elt.LIGNE_PRIX.ARTICLE.CodeFamArt == 14 && elt.PUEF != 0 && elt.QTEEF != 0 && !elt.IdFactDIT.HasValue) != 0 && con.StatutBL != "Cloturé"
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementsByIdClient(int idClient)
        {
            return (from con in dcPar.GetTable<CONNAISSEMENT>()
                    where con.SensBL == "I" && con.IdClient == idClient
                    orderby con.NumBL ascending
                    select con).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumBLAll(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumBLAll(string numBL, string statut)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.SensBL == "I" && bl.StatutBL == statut
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) /*&& bl.StatutBL != "Cloturé"*/ && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourExtensionByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumBL(string numBL, int idAcc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.ESCALE.IdAcc == idAcc && bl.StatutBL != "Cloturé" && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByConsignee(string consignee)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ConsigneeBL.Contains(consignee) && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }


        public List<HIST_CONNAISSEMENT> GetHistConnaissement(int idBL)
        {
            return (from bl in dcPar.GetTable<HIST_CONNAISSEMENT>()
                    where bl.CONNAISSEMENT.IdBL == idBL && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<HIST_CONNAISSEMENT>();
        }

        public List<OPERATION_CONNAISSEMENT> GetStatutsConnaissement(int idBL)
        {
            return (from bl in dcPar.GetTable<OPERATION_CONNAISSEMENT>()
                    where bl.IdBL == idBL
                    orderby bl.IdTypeOp ascending
                    select bl).ToList<OPERATION_CONNAISSEMENT>();
        }

        public List<ElementCompte> GetCompteBL(int idBL)
        {

            List<ElementCompte> listFactures = (from fact in dcPar.GetTable<FACTURE>()
                                                where fact.PROFORMA.IdBL == idBL
                                                select new ElementCompte
                                                {
                                                    Id = fact.IdFD,
                                                    Libelle = "Facture N° " + fact.PROFORMA.CONNAISSEMENT.NumBL + " - " + fact.IdDocSAP,
                                                    TypeDoc = "FA",
                                                    Debit = fact.MTTC.Value,
                                                    DateComptable = fact.DateComptable.Value
                                                }).ToList<ElementCompte>();

            List<ElementCompte> listPayements = (from pay in dcPar.GetTable<PAYEMENT>()
                                                 where pay.IdBL == idBL && !pay.IdPayDRC.HasValue
                                                 select new ElementCompte
                                                 {
                                                     Id = pay.IdPay,
                                                     Libelle = "Paiement N° " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                     TypeDoc = "PA",
                                                     Credit = pay.MAPay.Value,
                                                     DateComptable = pay.DatePay.Value
                                                 }).ToList<ElementCompte>();

            List<ElementCompte> listRestCaution = (from pay in dcPar.GetTable<PAYEMENT>()
                                                   where pay.IdBL == idBL && pay.IdPayDRC.HasValue
                                                   select new ElementCompte
                                                   {
                                                       Id = pay.IdPay,
                                                       Libelle = "Rest. Caution N° " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                       TypeDoc = "PA",
                                                       Debit = pay.MAPay.Value,
                                                       DateComptable = pay.DatePay.Value
                                                   }).ToList<ElementCompte>();

            List<ElementCompte> listPayementsAnnules = (from pay in dcPar.GetTable<PAYEMENT>()
                                                        where pay.IdBL == idBL && pay.StatutPay == "A"
                                                        select new ElementCompte
                                                        {
                                                            Id = pay.IdPay,
                                                            Libelle = "Annulation Paiement N° " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                            TypeDoc = "PA",
                                                            Debit = pay.MAPay.Value,
                                                            DateComptable = pay.DatePay.Value
                                                        }).ToList<ElementCompte>();

            List<ElementCompte> listAvoirs = (from av in dcPar.GetTable<AVOIR>()
                                              where av.IdBL == idBL
                                              select new ElementCompte
                                              {
                                                  Id = av.IdFA,
                                                  Libelle = "Avoir N° " + av.CONNAISSEMENT.NumBL + " - " + av.IdDocSAP,
                                                  TypeDoc = "CN",
                                                  Credit = av.MTTC.Value,
                                                  DateComptable = av.DCFA.Value
                                              }).ToList<ElementCompte>();

            List<ElementCompte> elts = new List<ElementCompte>();
            elts.AddRange(listFactures);
            elts.AddRange(listPayements);
            elts.AddRange(listRestCaution);
            elts.AddRange(listPayementsAnnules);
            elts.AddRange(listAvoirs);

            return elts.OrderBy(e => e.DateComptable).ToList();
        }

        public List<CONNAISSEMENT> GetConnaissementByConsignee(string consignee, string statut)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ConsigneeBL.Contains(consignee) && bl.StatutBL == statut && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementEnlevementByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.BON_ENLEVEMENT.Count != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumBL(int idMan, string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.IdMan == idMan && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevementByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && ((bl.StatutBL == "Accompli" && bl.IdAcc == 1) || bl.IdAcc != 1 || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourCautionByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.CONTENEUR.Count(ctr => ctr.IdPay.HasValue && ctr.OPERATION_CONTENEUR.FirstOrDefault<OPERATION_CONTENEUR>(op => op.IdTypeOp.Value == 19) != null) != 0 && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevementByNumBL(string numBL, int idAcc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.ESCALE.IdAcc == idAcc && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevement()
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where (bl.StatutBL == "Accompli" || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevement(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && (bl.StatutBL == "Accompli" || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevementByConsignee(string consBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ConsigneeBL.Contains(consBL) && (bl.StatutBL == "Accompli" || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevementByEscale(int numEsc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ESCALE.NumEsc == numEsc && (bl.StatutBL == "Accompli" || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourEnlevementByIdMan(int idMan)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdMan == idMan && (bl.StatutBL == "Accompli" || bl.BLER == "Y") && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraisonByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraisonByNumBL(string numBL, int idAcc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.ESCALE.IdAcc == idAcc && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraison()
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraison(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraisonByConsignee(string consBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ConsigneeBL.Contains(consBL) && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraisonByEscale(int numEsc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ESCALE.NumEsc == numEsc && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourLivraisonByIdMan(int idMan)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdMan == idMan && bl.BON_ENLEVEMENT.Count(bae => bae.DVBAE.HasValue) != 0 && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortieByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 17 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 31 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortieByNumBL(string numBL, int idAcc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && bl.ESCALE.IdAcc == idAcc && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 17 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 31 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortie()
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 18 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 32 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortie(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.NumBL.Contains(numBL) && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 18 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 32 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortieByConsignee(string consBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ConsigneeBL.Contains(consBL) && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 18 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 32 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortieByEscale(int numEsc)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.ESCALE.NumEsc == numEsc && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 18 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 32 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementPourSortieByIdMan(int idMan)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdMan == idMan && (bl.VEHICULE.Count(veh => veh.OPERATION_VEHICULE.Count(op => op.IdTypeOp == 9 && op.DateOp.HasValue) != 0 && !veh.IdBS.HasValue) != 0 || bl.CONTENEUR.Count(ctr => ctr.OPERATION_CONTENEUR.Count(op => op.IdTypeOp == 18 && op.DateOp.HasValue) != 0 && !ctr.IdBS.HasValue) != 0 || bl.CONVENTIONNEL.Count(gc => gc.OPERATION_CONVENTIONNEL.Count(op => op.IdTypeOp == 32 && op.DateOp.HasValue) != 0 && !gc.IdBS.HasValue) != 0) && bl.SensBL == "I" && bl.StatutBL != "Cloturé"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public CONNAISSEMENT GetConnaissementByIdBL(int idBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdBL == idBL// && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).SingleOrDefault<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumEscale(int numEscale)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByNumEscale(int numEscale, string statut)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.StatutBL == statut && bl.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByEscaleAndNumBL(int numEscale, string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && bl.NumBL.Contains(numBL)// && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementSOCARByNumEscale(int numEscale)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && bl.BLSocar == "Y" && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementSOCARByNumBL(string numBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && bl.NumBL.Contains(numBL) && bl.BLSocar == "Y" && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementSOCARByConsignee(string consBL)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && bl.ConsigneeBL.Contains(consBL) && bl.BLSocar == "Y" && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementSOCARByIdMan(int idMan)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdMan == idMan && bl.BLSocar == "Y" && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByStatut(string statut)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.StatutBL == statut && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetConnaissementByStatutAndIdMan(string statut, int idMan)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdMan == idMan && bl.StatutBL == statut && bl.SensBL == "I"
                    orderby bl.NumBL ascending
                    select bl).ToList<CONNAISSEMENT>();
        }

        #endregion


        #region Booking

        public CONNAISSEMENT GetBookingByIdBooking(int idBooking)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.IdBL == idBooking && book.SensBL == "E"
                    orderby book.NumBL ascending
                    select book).SingleOrDefault<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBooking()
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBookingByStatut(string statut)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.StatutBL == statut && book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBookingByNumBooking(string numBooking)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.NumBooking.Contains(numBooking) && book.StatutBL != "Cloturé" && book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }
      
        public List<CONNAISSEMENT> GetBookingAllByNumBooking(string numBooking)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.NumBooking.Contains(numBooking) && book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBookingAllByConsignee(string consBooking)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.ConsigneeBooking.Contains(consBooking) && book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBookingAllByShipper(string shipBooking)
        {
            return (from book in dcPar.GetTable<CONNAISSEMENT>()
                    where book.ConsigneeBL.Contains(shipBooking) && book.SensBL == "E"
                    orderby book.NumBooking descending
                    select book).ToList<CONNAISSEMENT>();
        }

        public List<CONNAISSEMENT> GetBookingByNumEscale(int numEscale)
        {
            return (from bl in dcPar.GetTable<CONNAISSEMENT>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where bl.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && bl.SensBL == "E"
                    orderby bl.NumBooking descending
                    select bl).ToList<CONNAISSEMENT>();
        }

        public List<DISPOSITION_CONTENEUR> GetDispositionCtr(int idBooking)
        {
            return (from d in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where d.IdBooking == idBooking
                    orderby d.TypeCtr ascending
                    select d).ToList<DISPOSITION_CONTENEUR>();
        }

        public int GetQuantiteCtrEnStock(string typeCtr)
        {
            return (from c in dcPar.GetTable<CONTENEUR_TC>()
                    where c.TypeCtr == typeCtr && c.StatutTC == "Parqué"
                    select c).ToList<CONTENEUR_TC>().Count;
        }

        public DISPOSITION_CONTENEUR GetMiseDispositionByIdDisposition(int idDisposition)
        {
            return (from d in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where d.IdDisposition == idDisposition
                    select d).FirstOrDefault<DISPOSITION_CONTENEUR>();
        }

        public List<ElementBookingCtr> GetConteneursOfBooking(int idBL)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR>()
                    from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdBL == idBL && bl.IdBL == ctr.IdBL && bl.SensBL == "E"

                    orderby ctr.NumCtr ascending
                    select new ElementBookingCtr
                    {
                        IdCtr = ctr.IdCtr,
                        DescMses = ctr.DescMses,
                        Description = ctr.DescCtr,
                        NumCtr = ctr.NumCtr,
                        Poids = (ctr.StatCtr == "Non initié" || ctr.StatCtr == "Final Booking") ? ctr.PoidsMCtr.Value : (ctr.StatCtr == "Cargo Loading" ? ctr.PoidsCCtr.Value : ctr.PoidsCCtr.Value),
                        Seal1 = ctr.Seal1Ctr,
                        Seal2 = ctr.Seal2Ctr,
                        StatutCtr = ctr.StatutCtr,
                        TypeCtr = ctr.TypeCCtr,
                        UNCode = ctr.IMDGCode,
                        Volume = ctr.VolMCtr.Value,
                        TypeMsesCtr = ctr.TypeMses,
                        StatCtr = ctr.StatCtr,
                        //AH7juillet16
                        VGM = (ctr.VGM.HasValue) ? (int)ctr.VGM : 0
                    }).ToList<ElementBookingCtr>();
        }

        public List<ElementBookingGC> GetConventionnelsOfBooking(int idBL)
        {
            return (from conv in dcPar.GetTable<CONVENTIONNEL>()
                    from bl in dcPar.GetTable<CONNAISSEMENT>()
                    where bl.IdBL == idBL && bl.IdBL == conv.IdBL && bl.SensBL == "E"
                    orderby conv.NumGC ascending
                    select new ElementBookingGC
                    {
                        IdGC = conv.IdGC,
                        Description = conv.DescGC,
                        Hauteur = conv.HautCGC.Value,
                        Largeur = conv.LargCGC.Value,
                        Longueur = conv.LongCGC.Value,
                        NumGC = conv.NumGC,
                        Poids = (conv.StatGC == "Non initié" || conv.StatGC == "Final Booking") ? (conv.PoidsMGC.HasValue ? conv.PoidsMGC.Value : 0) : (conv.StatGC == "Cargo Loading" ? (conv.PoidsRGC.HasValue ? conv.PoidsRGC.Value : 0) : (conv.PoidsCGC.HasValue ? conv.PoidsCGC.Value : 0)),
                        Quantite = (conv.StatGC == "Non initié" || conv.StatGC == "Final Booking") ? (conv.QteBGC.HasValue ? conv.QteBGC.Value : (short)0) : (conv.StatGC == "Cargo Loading" ? (conv.QteRGC.HasValue ? conv.QteRGC.Value : (short)0) : (conv.NumItem.HasValue ? conv.NumItem.Value : (short)0)),
                        TypeMses = conv.TYPE_CONVENTIONNEL.LibTypeGC,
                        Volume = (conv.StatGC == "Non initié" || conv.StatGC == "Final Booking") ? (conv.VolMGC.HasValue ? conv.VolMGC.Value : 0) : (conv.StatGC == "Cargo Loading" ? (conv.VolRGC.HasValue ? conv.VolRGC.Value : 0) : (conv.VolCGC.HasValue ? conv.VolCGC.Value : 0)),
                        StatGC = conv.StatGC
                    }).ToList<ElementBookingGC>();
        }

        #endregion


        #region mise a disposition + 

        public List<DISPOSITION_CONTENEUR> GetDemandesDisposition()
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionEnCours()
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.CONTENEUR_TC.Count < dd.NombreTC
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionEnCoursByTypeCtr(string typeCtr)
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.TypeCtr == typeCtr && dd.CONTENEUR_TC.Count < dd.NombreTC
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionEnCoursByTypeCtrAndBooking(string typeCtr, string numBooking)
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.TypeCtr == typeCtr && dd.CONTENEUR_TC.Count < dd.NombreTC && dd.CONNAISSEMENT.NumBooking.Contains(numBooking)
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionCloturees()
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.CONTENEUR_TC.Count == dd.NombreTC
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionByNumBooking(string numBooking)
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.CONNAISSEMENT.NumBL.Contains(numBooking)
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionByRefDisposition(string refDisposition)
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.RefDisposition.Contains(refDisposition)
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<DISPOSITION_CONTENEUR> GetDemandesDispositionByNumEscale(int numEscale)
        {
            return (from dd in dcPar.GetTable<DISPOSITION_CONTENEUR>()
                    where dd.CONNAISSEMENT.ESCALE.NumEsc == numEscale
                    orderby dd.IdBooking descending
                    select dd).ToList<DISPOSITION_CONTENEUR>();
        }

        public List<MOUVEMENT_TC> GetDailyMoves(int idArm, DateTime dateDebut, DateTime dateFin)
        {
            return (from mvtTC in dcPar.GetTable<MOUVEMENT_TC>()
                    where mvtTC.CONTENEUR_TC.CONTENEUR.ESCALE.IdArm == idArm && mvtTC.TYPE_OPERATION.CodeMvt != "" && mvtTC.TYPE_OPERATION.CodeMvt != "051" && mvtTC.TYPE_OPERATION.CodeMvt != "141" && mvtTC.DateMvt.Value >= dateDebut && mvtTC.DateMvt.Value <= dateFin.AddHours(23).AddMinutes(59).AddSeconds(59)
                    orderby mvtTC.TYPE_OPERATION.CodeMvt ascending
                    select mvtTC).ToList<MOUVEMENT_TC>();
        }

        public List<OPERATION_CONTENEUR> GetDailyMovesExcel(int idArm, DateTime dateDebut, DateTime dateFin)
        {
            return (from opCtr in dcPar.GetTable<OPERATION_CONTENEUR>()
                    where opCtr.CONTENEUR.ESCALE.IdArm == idArm && opCtr.TYPE_OPERATION.CodeMvt != "" && opCtr.TYPE_OPERATION.CodeMvt != "051" && opCtr.TYPE_OPERATION.CodeMvt != "141" && opCtr.DateOp.Value >= dateDebut && opCtr.DateOp.Value <= dateFin.AddHours(23).AddMinutes(59).AddSeconds(59)
                    orderby opCtr.TYPE_OPERATION.CodeMvt ascending
                    select opCtr).ToList<OPERATION_CONTENEUR>();
        }


        #endregion

        #region Vehicule

        public List<VEHICULE> GetVehiculesImport()
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImport(int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByStatut(string statut)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByStatut(string numChassis, string statut)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(numChassis) && veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByNumBLAndStatut(string numBL, string statut)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByStatut(string statut, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.ESCALE.IdAcc == idAcc && veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByStatut(string numChassis, string statut, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(numChassis) && veh.ESCALE.IdAcc == idAcc && veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByNumBLAndStatut(string numBL, string statut, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.ESCALE.IdAcc == idAcc && veh.StatVeh == statut && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubage()
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubage(string numChassis)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(numChassis) && veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubageByNumBL(string numBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubageByIdAcc(int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubageByIdAcc(string numChassis, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(numChassis) && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesEnCubageByNumBLAndIdAcc(string numBL, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I" && veh.CUBAGE_VEHICULE.Count(cub => !cub.CUBAGE.DateCloCubage.HasValue) != 0
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumChassis(string chassis)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(chassis) && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumChassis(string chassis, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis.Contains(chassis) && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumEscale(int numEscale)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.ESCALE.NumEsc == numEscale && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumEscale(int numEscale, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.ESCALE.NumEsc == numEscale && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportByNumBL(string numBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumBL(string numBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL)//&& veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByNumBL(string numBL, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.CONNAISSEMENT.NumBL.Contains(numBL) && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByDescription(string description)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.DescVeh.Contains(description) && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesByDescription(string description, int idAcc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.DescVeh.Contains(description) && veh.ESCALE.IdAcc == idAcc && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public VEHICULE GetVehiculeByIdVeh(int idVeh)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdVeh == idVeh && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).SingleOrDefault<VEHICULE>();
        }

        public VEHICULE GetVehiculeByNumChassisAndNumBL(string numChassis, string numBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.NumChassis == numChassis && veh.CONNAISSEMENT.NumBL == numBL && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).SingleOrDefault<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesOfConnaissement(int idBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdBL == idBL && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesLivraisonOfConnaissement(int idBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdBL == idBL && veh.IdBAE.HasValue && veh.BON_ENLEVEMENT.DVBAE.HasValue && !veh.IdDBL.HasValue && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesSortieOfConnaissement(int idBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdBL == idBL && veh.IdBAE.HasValue && veh.BON_ENLEVEMENT.DVBAE.HasValue && veh.DEMANDE_LIVRAISON.DVDBL.HasValue && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesImportOfDBL(int idDBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdDBL == idDBL && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesPourEnlevementOfConnaissement(int idBL)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdBL == idBL && (veh.CONNAISSEMENT.StatutBL == "Accompli" || veh.CONNAISSEMENT.BLER == "Y") && veh.SensVeh == "I" && veh.CONNAISSEMENT.StatutBL != "Cloturé"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesOfConnaissementNonThis(int idBL, int idVeh)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdBL == idBL && veh.IdVeh != idVeh && !veh.IdVehAP.HasValue && veh.SensVeh == "I"
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesOfVisite(int idDV)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.VISITE_VEHICULE.IdDV == idDV
                    orderby veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public VEHICULE GetVehiculePrincipal(int idVehAP)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdVeh == idVehAP
                    select veh).SingleOrDefault<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesAPByIdVeh(int idVeh)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.IdVehAP == idVeh
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesExtractManutAutresAcconiers(DateTime debut, DateTime fin, bool isRoulant)
        {
            if (isRoulant == true)
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.IdAcc != 1 && !veh.IdVehAP.HasValue && veh.IdBS.HasValue && veh.ESCALE.DRAEsc.Value >= debut && veh.ESCALE.DRAEsc.Value <= fin
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
            else
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.IdAcc != 1 && veh.IdBS.HasValue && veh.ESCALE.DRAEsc.Value >= debut && veh.ESCALE.DRAEsc.Value <= fin
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
        }

        public List<VEHICULE> GetVehiculesExtractSejourAutresAcconiers(DateTime debut, DateTime fin, bool isRoulant)
        {
            if (isRoulant == true)
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.IdAcc != 1 && !veh.IdVehAP.HasValue && veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
            else
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.IdAcc != 1 && veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
        }

        public List<VEHICULE> GetVehiculesExtractSejourDiversReversements(DateTime debut, DateTime fin, bool isRoulant)
        {
            if (isRoulant == true)
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where !veh.IdVehAP.HasValue && veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
            else
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
        }

        public List<VEHICULE> GetVehiculesExtractEntrees(DateTime debut, DateTime fin, bool isRoulant)
        {
            if (isRoulant == true)
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.DRAEsc.Value >= debut && veh.ESCALE.DRAEsc.Value <= fin && !veh.IdVehAP.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
            else
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.ESCALE.DRAEsc.Value >= debut && veh.ESCALE.DRAEsc.Value <= fin
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }

        }

        public List<VEHICULE> GetVehiculesExtractSorties(DateTime debut, DateTime fin, bool isRoulant)
        {
            if (isRoulant == true)
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue && !veh.IdVehAP.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }
            else
            {
                return (from veh in dcPar.GetTable<VEHICULE>()
                        where veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                        orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                        select veh).ToList<VEHICULE>();
            }

        }

        public List<VEHICULE> GetVehiculesExtractSortiesCNCC(DateTime debut, DateTime fin)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where veh.DSRVeh.Value >= debut && veh.DSRVeh.Value <= fin && veh.IdBS.HasValue
                    orderby veh.DSRVeh ascending, veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();
        }

        public List<VEHICULE> GetVehiculesExtractParques(int idParc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    where (veh.OCCUPATION.Count(occ => !occ.DateFin.HasValue) > 0 && veh.OCCUPATION.OrderByDescending(oc => oc.DateDebut).First<OCCUPATION>().EMPLACEMENT.PARC.IdParc == idParc)
                    || (veh.IdVehAP.HasValue && veh.VEHICULE1.OCCUPATION.Count(occ => !occ.DateFin.HasValue) > 0 && veh.VEHICULE1.OCCUPATION.OrderByDescending(oc => oc.DateDebut).First<OCCUPATION>().EMPLACEMENT.PARC.IdParc == idParc)
                    orderby veh.ESCALE.NumEsc ascending, veh.ESCALE.ACCONIER.NomAcc ascending, veh.CONNAISSEMENT.NumBL ascending, veh.NumChassis ascending
                    select veh).ToList<VEHICULE>();

        }

        public List<EMPLACEMENT> GetEmplacementDisposByIdParc(int idParc, double longVeh, double largVeh)
        {
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where (empl.IdParc == idParc && longVeh >= Convert.ToDouble(empl.LongMin)
                    && longVeh <= Convert.ToDouble(empl.LongMax) && largVeh >= Convert.ToDouble(empl.LargMin)
                    && largVeh <= Convert.ToDouble(empl.LargMax) && empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0
                    && empl.StatutEmpl == "A")
                    select empl).ToList<EMPLACEMENT>();
        }

        public List<EMPLACEMENT> GetEmplacementByIdParc(int idParc)
        {
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where empl.IdParc == idParc && empl.StatutEmpl == "A"
                    select empl).ToList<EMPLACEMENT>();
        }


        public string GetPositionVehicule(int idVeh)
        {
            var matchedVehicule = (from veh in dcPar.GetTable<VEHICULE>()
                                   where veh.IdVeh == idVeh
                                   select veh).FirstOrDefault<VEHICULE>();

            if (!matchedVehicule.IdVehAP.HasValue)
            {
                List<OCCUPATION> occupations = matchedVehicule.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
                if (occupations.Count != 0)
                {
                    return occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc + " : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
                }
                else
                {
                    return "Non parqué";
                }
            }
            else
            {
                return GetPositionVehicule(matchedVehicule.IdVehAP.Value);
            }
        }

        public string GetPositionConteneur(int idCtr)
        {
            var matchedCtr = (from ctr in dcPar.GetTable<CONTENEUR>()
                              where ctr.IdCtr == idCtr
                              select ctr).FirstOrDefault<CONTENEUR>();

            List<OCCUPATION> occupations = matchedCtr.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
            if (occupations.Count != 0)
            {
                return occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc + " : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
            }
            else
            {
                return "Non parqué";
            }
        }

        public string GetPosVeh(int idVeh)
        {
            var matchedVehicule = (from veh in dcPar.GetTable<VEHICULE>()
                                   where veh.IdVeh == idVeh
                                   select veh).FirstOrDefault<VEHICULE>();

            if (!matchedVehicule.IdVehAP.HasValue)
            {
                List<OCCUPATION> occupations = matchedVehicule.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
                if (occupations.Count != 0)
                {
                    return occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
                }
                else
                {
                    return "Non parqué";
                }
            }
            else
            {
                return GetPosVeh(matchedVehicule.IdVehAP.Value);
            }
        }


        public EMPLACEMENT GetEmplacementByIdEmpl(int idEmpl)
        {
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where empl.IdEmpl == idEmpl && empl.StatutEmpl == "A"
                    select empl).FirstOrDefault<EMPLACEMENT>();
        }

        public List<EMPLACEMENT> GetEmplacementConteneurByIdParc(int idParc, string statut)
        {
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where empl.IdParc == idParc && empl.TypeEmpl == statut && empl.StatutEmpl == "A"
                    select empl).ToList<EMPLACEMENT>();
        }

        public List<Emplacement> GetEmplacementDispos()
        {
            //AH upd 23juillet pour filtrer les emplacements actif uniquement
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0 && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                    orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                    select new Emplacement
                    {
                        DateEntree = "",
                        Empl = empl.LigEmpl + empl.ColEmpl,
                        NumBL = "",
                        Consignee = "",
                        NumChassis = "",
                        Marque = "",
                        NumEsc = "",
                        Parc = empl.PARC.NomParc
                            //AH 23juillet ajout pour suppression emplacement 
                        ,
                        IdEmpl = empl.IdEmpl
                    }).ToList<Emplacement>();
        }

        public List<Emplacement> GetEmplacementsAll()
        {
            List<Emplacement> emplsAll = new List<Emplacement>();

            //AH upd 23juillet pour filtrer les emplacements actif uniquement

            emplsAll.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                               where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                               orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                               select new Emplacement
                               {
                                   DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                                   Empl = empl.LigEmpl + empl.ColEmpl,
                                   NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                                   Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                                   NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                                   Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                                   NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                                   Parc = empl.PARC.NomParc
                                       //AH 23juillet ajout pour suppression emplacement
                                   ,
                                   IdEmpl = empl.IdEmpl
                               }).ToList<Emplacement>());

            //AH upd 23juillet pour filtrer les emplacements actif uniquement

            emplsAll.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                               where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0 && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                               orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                               select new Emplacement
                               {
                                   DateEntree = "",
                                   Empl = empl.LigEmpl + empl.ColEmpl,
                                   NumBL = "",
                                   Consignee = "",
                                   NumChassis = "",
                                   Marque = "",
                                   NumEsc = "",
                                   Parc = empl.PARC.NomParc
                               }).ToList<Emplacement>());

            return emplsAll;

        }

        public List<Emplacement> GetEmplacementsByNumChassis(string numChassis)
        {
            List<Emplacement> emplsByChassis = new List<Emplacement>();

            emplsByChassis.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                                     where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && empl.OCCUPATION.Skip<OCCUPATION>(empl.OCCUPATION.Count - 1).First<OCCUPATION>().VEHICULE.NumChassis.Contains(numChassis)
                                     && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                                     orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                                     select new Emplacement
                                     {
                                         DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                                         Empl = empl.LigEmpl + empl.ColEmpl,
                                         NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                                         Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                                         NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                                         Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                                         NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                                         Parc = empl.PARC.NomParc
                                             //AH 23juillet ajout pour suppression emplacement 
                                         ,
                                         IdEmpl = empl.IdEmpl
                                     }).ToList<Emplacement>());

            return emplsByChassis;

        }

        public List<Emplacement> GetEmplacementsByNumBL(string numBL)
        {
            List<Emplacement> emplsByBL = new List<Emplacement>();

            emplsByBL.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                                where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && empl.OCCUPATION.Skip<OCCUPATION>(empl.OCCUPATION.Count - 1).First<OCCUPATION>().VEHICULE.CONNAISSEMENT.NumBL.Contains(numBL)
                                && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                                orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                                select new Emplacement
                                {
                                    DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                                    Empl = empl.LigEmpl + empl.ColEmpl,
                                    NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                                    Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                                    NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                                    Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                                    NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                                    Parc = empl.PARC.NomParc
                                        //AH 23juillet ajout pour suppression emplacement 
                                    ,
                                    IdEmpl = empl.IdEmpl
                                }).ToList<Emplacement>());

            return emplsByBL;

        }

        public List<Emplacement> GetEmplacementsByEmpl(string numEmpl)
        {
            List<Emplacement> emplsByNumEmpl = new List<Emplacement>();

            emplsByNumEmpl.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                                     where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && (empl.LigEmpl + empl.ColEmpl).Contains(numEmpl) && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                                     orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                                     select new Emplacement
                                     {
                                         DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                                         Empl = empl.LigEmpl + empl.ColEmpl,
                                         NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                                         Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                                         NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                                         Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                                         NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                                         Parc = empl.PARC.NomParc
                                             //AH 23juillet ajout pour suppression emplacement 
                                         ,
                                         IdEmpl = empl.IdEmpl
                                     }).ToList<Emplacement>());

            return emplsByNumEmpl;

        }

        public List<Emplacement> GetEmplacementsByNumEsc(string numEsc)
        {
            List<Emplacement> emplsByBL = new List<Emplacement>();

            emplsByBL.AddRange((from empl in dcPar.GetTable<EMPLACEMENT>()
                                where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && empl.OCCUPATION.Skip<OCCUPATION>(empl.OCCUPATION.Count - 1).First<OCCUPATION>().VEHICULE.ESCALE.NumEsc.ToString().Contains(numEsc)
                                && empl.PARC.IdParc == 1 && empl.StatutEmpl == "A"
                                orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                                select new Emplacement
                                {
                                    DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                                    Empl = empl.LigEmpl + empl.ColEmpl,
                                    NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                                    Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                                    NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                                    Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                                    NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                                    Parc = empl.PARC.NomParc
                                        //AH 23juillet ajout pour suppression emplacement 
                                    ,
                                    IdEmpl = empl.IdEmpl
                                }).ToList<Emplacement>());

            return emplsByBL;

        }

        public List<Emplacement> GetEmplacementOccupes()
        {
            return (from empl in dcPar.GetTable<EMPLACEMENT>()
                    where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) != 0 && empl.IdParc == 1 && empl.StatutEmpl == "A"
                    orderby empl.LigEmpl ascending, empl.ColEmpl ascending
                    select new Emplacement
                    {
                        DateEntree = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).DateDebut.Value.ToShortDateString(),
                        Empl = empl.LigEmpl + empl.ColEmpl,
                        NumBL = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.NumBL,
                        Consignee = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.CONNAISSEMENT.ConsigneeBL,
                        NumChassis = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.NumChassis,
                        Marque = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.DescVeh,
                        NumEsc = empl.OCCUPATION.FirstOrDefault<OCCUPATION>(oc => !oc.DateFin.HasValue).VEHICULE.ESCALE.NumEsc.Value.ToString(),
                        Parc = empl.PARC.NomParc
                            //AH 23juillet ajout pour suppression emplacement 
                        ,
                        IdEmpl = empl.IdEmpl
                    }).ToList<Emplacement>();
        }

        public List<VehiculeCubage> GetVehiculesEnCubage(int idCub)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    from cub in dcPar.GetTable<CUBAGE_VEHICULE>()
                    where veh.IdVeh == cub.IdVeh && cub.IdCubage == idCub
                    orderby veh.NumChassis ascending
                    select new VehiculeCubage
                    {
                        IdVeh = cub.IdVeh,
                        NumChassis = veh.NumChassis,
                        NumBL = veh.CONNAISSEMENT.NumBL,
                        Description = veh.DescVeh,
                        LongueurManifeste = cub.LongAVeh.Value,
                        LargeurManifeste = cub.LargAVeh.Value,
                        HauteurManifeste = cub.HautAVeh.Value,
                        VolumeManifeste = cub.VolAVeh.Value,
                        LongueurCube = cub.LongCVeh.Value,
                        LargeurCube = cub.LargCVeh.Value,
                        HauteurCube = cub.HautCVeh.Value,
                        VolumeCube = cub.VolCVeh.Value,
                        IsCubed = cub.VolCVeh.Value != 0,
                        IsValidated = cub.DateVal.HasValue,
                        DateVal = cub.DateVal,
                        Etat = cub.VEHICULE.StatutVeh
                    }).ToList<VehiculeCubage>();
        }

        public List<VehiculeCubage> GetVehiculesNonCubesOfCubage(int idCub)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    from cub in dcPar.GetTable<CUBAGE_VEHICULE>()
                    where veh.IdVeh == cub.IdVeh && cub.IdCubage == idCub && cub.VolCVeh.Value == 0
                    select new VehiculeCubage
                    {
                        IdVeh = cub.IdVeh,
                        NumChassis = veh.NumChassis,
                        NumBL = veh.CONNAISSEMENT.NumBL,
                        Description = veh.DescVeh,
                        LongueurManifeste = cub.LongAVeh.Value,
                        LargeurManifeste = cub.LargAVeh.Value,
                        HauteurManifeste = cub.HautAVeh.Value,
                        VolumeManifeste = cub.VolAVeh.Value,
                        LongueurCube = cub.LongCVeh.Value,
                        LargeurCube = cub.LargCVeh.Value,
                        HauteurCube = cub.HautCVeh.Value,
                        VolumeCube = cub.VolCVeh.Value,
                        IsCubed = cub.VolCVeh.Value != 0,
                        IsValidated = cub.DateVal.HasValue,
                        DateVal = cub.DateVal,
                        Etat = cub.VEHICULE.StatutVeh
                    }).ToList<VehiculeCubage>();
        }

        public List<VehiculeCubage> GetVehiculesEscalePourCubage(int idEsc)
        {
            return (from veh in dcPar.GetTable<VEHICULE>()
                    //where veh.IdEsc == idEsc && veh.CUBAGE_VEHICULE.Count(cubVeh => !cubVeh.CUBAGE.DateCloCubage.HasValue) == 0
                    where veh.IdEsc == idEsc && veh.CUBAGE_VEHICULE.Count(cubVeh => !cubVeh.DateVal.HasValue) == 0 && veh.StatVeh != "Livré" && !veh.IdVehAP.HasValue
                    select new VehiculeCubage
                    {
                        IdVeh = veh.IdVeh,
                        NumChassis = veh.NumChassis,
                        NumBL = veh.CONNAISSEMENT.NumBL,
                        Description = veh.DescVeh,
                        LongueurManifeste = veh.LongMVeh.Value,
                        LargeurManifeste = veh.LargMVeh.Value,
                        HauteurManifeste = veh.HautMVeh.Value,
                        VolumeManifeste = veh.VolMVeh.Value,
                        LongueurCube = veh.LongCVeh.Value,
                        LargeurCube = veh.LargCVeh.Value,
                        HauteurCube = veh.HautCVeh.Value,
                        VolumeCube = veh.VolCVeh.Value,
                        Etat = veh.StatutVeh
                    }).ToList<VehiculeCubage>();
        }

        #endregion


        #region manifeste

        public List<MANIFESTE> GetManifestes()
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }


        public List<OPERATION_MANIFESTE> GetStatutsManifeste(int idMan)
        {
            return (from man in dcPar.GetTable<OPERATION_MANIFESTE>()
                    where man.IdMan == idMan
                    orderby man.IdTypeOp ascending
                    select man).ToList<OPERATION_MANIFESTE>();
        }

        public List<ElementCompte> GetCompteMan(int idMan)
        {

            List<ElementCompte> listFactures = (from fact in dcPar.GetTable<FACTURE>()
                                                where fact.PROFORMA.CONNAISSEMENT.IdMan == idMan
                                                select new ElementCompte
                                                {
                                                    Id = fact.IdFD,
                                                    Libelle = "Facture N° " + fact.PROFORMA.CONNAISSEMENT.MANIFESTE.CodePort + " - " + fact.IdDocSAP,
                                                    TypeDoc = "FA",
                                                    Debit = fact.MTTC.Value,
                                                    DateComptable = fact.DateComptable.Value
                                                }).ToList<ElementCompte>();

            List<ElementCompte> listPayements = (from pay in dcPar.GetTable<PAYEMENT>()
                                                 where pay.CONNAISSEMENT.IdMan == idMan && !pay.IdPayDRC.HasValue
                                                 select new ElementCompte
                                                 {
                                                     Id = pay.IdPay,
                                                     Libelle = "Paiement N° " + pay.CONNAISSEMENT.MANIFESTE.CodePort + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                     TypeDoc = "PA",
                                                     Credit = pay.MAPay.Value,
                                                     DateComptable = pay.DatePay.Value
                                                 }).ToList<ElementCompte>();

            List<ElementCompte> listRestCaution = (from pay in dcPar.GetTable<PAYEMENT>()
                                                   where pay.CONNAISSEMENT.IdMan == idMan && pay.IdPayDRC.HasValue
                                                   select new ElementCompte
                                                   {
                                                       Id = pay.IdPay,
                                                       Libelle = "Rest. Caution N° " + pay.CONNAISSEMENT.MANIFESTE.CodePort + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                       TypeDoc = "PA",
                                                       Debit = pay.MAPay.Value,
                                                       DateComptable = pay.DatePay.Value
                                                   }).ToList<ElementCompte>();

            List<ElementCompte> listPayementsAnnules = (from pay in dcPar.GetTable<PAYEMENT>()
                                                        where pay.CONNAISSEMENT.IdMan == idMan && pay.StatutPay == "A"
                                                        select new ElementCompte
                                                        {
                                                            Id = pay.IdPay,
                                                            Libelle = "Annulation Paiement N° " + pay.CONNAISSEMENT.MANIFESTE.CodePort + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                            TypeDoc = "PA",
                                                            Debit = pay.MAPay.Value,
                                                            DateComptable = pay.DatePay.Value
                                                        }).ToList<ElementCompte>();

            List<ElementCompte> listAvoirs = (from av in dcPar.GetTable<AVOIR>()
                                              where av.CONNAISSEMENT.IdMan == idMan
                                              select new ElementCompte
                                              {
                                                  Id = av.IdFA,
                                                  Libelle = "Avoir N° " + av.CONNAISSEMENT.NumBL + " - " + av.IdDocSAP,
                                                  TypeDoc = "CN",
                                                  Credit = av.MTTC.Value,
                                                  DateComptable = av.DCFA.Value
                                              }).ToList<ElementCompte>();

            List<ElementCompte> elts = new List<ElementCompte>();
            elts.AddRange(listFactures);
            elts.AddRange(listPayements);
            elts.AddRange(listRestCaution);
            elts.AddRange(listPayementsAnnules);
            elts.AddRange(listAvoirs);

            return elts.OrderBy(e => e.DateComptable).ToList();
        }

        public MANIFESTE GetManifesteByIdMan(int idMan)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    where man.IdMan == idMan
                    select man).SingleOrDefault<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesOfEscale(int idEsc)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    where man.IdEsc == idEsc
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByNumEscale(int numEscale)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.NumEsc == numEscale
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByNumEscale(int numEscale, string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.NumEsc == numEscale && man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeAcconier(string codeAcc)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from acc in dcPar.GetTable<ACCONIER>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdAcc == acc.IdAcc && acc.CodeAcc == codeAcc
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeAcconier(string codeAcc, string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from acc in dcPar.GetTable<ACCONIER>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdAcc == acc.IdAcc && acc.CodeAcc == codeAcc && man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeArmateur(string codeArm)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from arm in dcPar.GetTable<ARMATEUR>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdArm == arm.IdArm && arm.CodeArm == codeArm
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeArmateur(string codeArm, string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from arm in dcPar.GetTable<ARMATEUR>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdArm == arm.IdArm && arm.CodeArm == codeArm && man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeNavire(string codeNav)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from nav in dcPar.GetTable<NAVIRE>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdNav == nav.IdNav && nav.CodeNav == codeNav
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodeNavire(string codeNav, string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    from nav in dcPar.GetTable<NAVIRE>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where man.IdEsc == esc.IdEsc && esc.IdNav == nav.IdNav && nav.CodeNav == codeNav && man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodePort(string codePort)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    where man.PORT.CodePort == codePort
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByCodePort(string codePort, string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    where man.PORT.CodePort == codePort && man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        public List<MANIFESTE> GetManifestesByStatut(string statut)
        {
            return (from man in dcPar.GetTable<MANIFESTE>()
                    where man.StatutMan == statut
                    orderby man.DCMan descending
                    select man).ToList<MANIFESTE>();
        }

        #endregion

        #region escale

        public List<ESCALE> GetEscales()
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByNumEscale(int numEscale)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumEsc.ToString().Contains(numEscale.ToString())
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByNumEscale(int numEscale, string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumEsc.ToString().Contains(numEscale.ToString()) && esc.StatEsc == statut
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesGrimaldiByNumEscale(int numEscale)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumEsc.ToString().Contains(numEscale.ToString()) && esc.IdAcc == 1
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByNumVoyage(string numVoyage)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumVoySCR == numVoyage
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByNumVoyage(string numVoyage, string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.NumVoySCR == numVoyage && esc.StatEsc == statut
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeAcconier(string codeAcconier)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from acc in dcPar.GetTable<ACCONIER>()
                    where acc.IdAcc == esc.IdAcc && acc.CodeAcc == codeAcconier
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeAcconier(string codeAcconier, string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from acc in dcPar.GetTable<ACCONIER>()
                    where acc.IdAcc == esc.IdAcc && acc.CodeAcc == codeAcconier && esc.StatEsc == statut
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeArmateur(string codeArmateur)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from arm in dcPar.GetTable<ARMATEUR>()
                    where arm.IdArm == esc.IdArm && arm.CodeArm == codeArmateur
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeArmateur(string codeArmateur, string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from arm in dcPar.GetTable<ARMATEUR>()
                    where arm.IdArm == esc.IdArm && arm.CodeArm == codeArmateur && esc.StatEsc == statut
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeNavire(string codeNavire)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from nav in dcPar.GetTable<NAVIRE>()
                    where nav.IdNav == esc.IdNav && nav.CodeNav == codeNavire
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByCodeNavire(string codeNavire, string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    from nav in dcPar.GetTable<NAVIRE>()
                    where nav.IdNav == esc.IdNav && nav.CodeNav == codeNavire && esc.StatEsc == statut
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public List<ESCALE> GetEscalesByStatut(string statut)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.StatEsc == statut
                    orderby esc.NumEsc descending
                    select esc).ToList<ESCALE>();
        }

        public ESCALE GetEscaleById(int idEsc)
        {
            return (from esc in dcPar.GetTable<ESCALE>()
                    where esc.IdEsc == idEsc
                    orderby esc.NumEsc descending
                    select esc).SingleOrDefault<ESCALE>();
        }

        public List<OPERATION_ESCALE> GetStatutsEscale(int idEsc)
        {
            return (from esc in dcPar.GetTable<OPERATION_ESCALE>()
                    where esc.IdEsc == idEsc
                    orderby esc.IdTypeOp ascending
                    select esc).ToList<OPERATION_ESCALE>();
        }

        public List<ElementCompte> GetCompteEsc(int idEsc)
        {

            List<ElementCompte> listFactures = (from fact in dcPar.GetTable<FACTURE>()
                                                where fact.PROFORMA.CONNAISSEMENT.IdEsc == idEsc
                                                select new ElementCompte
                                                {
                                                    Id = fact.IdFD,
                                                    Libelle = "Facture N° " + fact.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc + " - " + fact.PROFORMA.CONNAISSEMENT.NumBL + " - " + fact.IdFD,
                                                    TypeDoc = "FA",
                                                    Debit = fact.MTTC.Value,
                                                    DateComptable = fact.DateComptable.Value
                                                }).ToList<ElementCompte>();

            List<ElementCompte> listPayements = (from pay in dcPar.GetTable<PAYEMENT>()
                                                 where pay.CONNAISSEMENT.IdEsc == idEsc && !pay.IdPayDRC.HasValue
                                                 select new ElementCompte
                                                 {
                                                     Id = pay.IdPay,
                                                     Libelle = "Paiement N° " + pay.CONNAISSEMENT.ESCALE.NumEsc + " - " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                     TypeDoc = "PA",
                                                     Credit = pay.MAPay.Value,
                                                     DateComptable = pay.DatePay.Value
                                                 }).ToList<ElementCompte>();

            List<ElementCompte> listRestCaution = (from pay in dcPar.GetTable<PAYEMENT>()
                                                   where pay.CONNAISSEMENT.IdEsc == idEsc && pay.IdPayDRC.HasValue
                                                   select new ElementCompte
                                                   {
                                                       Id = pay.IdPay,
                                                       Libelle = "Rest. Caution N° " + pay.CONNAISSEMENT.ESCALE.NumEsc + " - " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                       TypeDoc = "PA",
                                                       Debit = pay.MAPay.Value,
                                                       DateComptable = pay.DatePay.Value
                                                   }).ToList<ElementCompte>();

            List<ElementCompte> listPayementsAnnules = (from pay in dcPar.GetTable<PAYEMENT>()
                                                        where pay.CONNAISSEMENT.IdEsc == idEsc && pay.StatutPay == "A"
                                                        select new ElementCompte
                                                        {
                                                            Id = pay.IdPay,
                                                            Libelle = "Annulation Paiement N° " + pay.CONNAISSEMENT.ESCALE.NumEsc + " - " + pay.CONNAISSEMENT.NumBL + " - " + pay.IdPay + " - " + pay.IdPaySAP,
                                                            TypeDoc = "PA",
                                                            Debit = pay.MAPay.Value,
                                                            DateComptable = pay.DatePay.Value
                                                        }).ToList<ElementCompte>();

            List<ElementCompte> listAvoirs = (from av in dcPar.GetTable<AVOIR>()
                                              where av.CONNAISSEMENT.IdEsc == idEsc
                                              select new ElementCompte
                                              {
                                                  Id = av.IdFA,
                                                  Libelle = "Avoir N° " + av.CONNAISSEMENT.NumBL + " - " + av.IdDocSAP,
                                                  TypeDoc = "CN",
                                                  Credit = av.MTTC.Value,
                                                  DateComptable = av.DCFA.Value
                                              }).ToList<ElementCompte>();

            List<ElementCompte> elts = new List<ElementCompte>();
            elts.AddRange(listFactures);
            elts.AddRange(listPayements);
            elts.AddRange(listRestCaution);
            elts.AddRange(listPayementsAnnules);
            elts.AddRange(listAvoirs);

            return elts.OrderBy(e => e.DateComptable).ToList();
        }


        #endregion

        #region Conteneur TC

        public List<CONTENEUR_TC> GetConteneursTC()
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.IsDoublon == "N"
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByStatut(string statut)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.StatutTC == statut && ctr.IsDoublon == "N"
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByStatutForWAF(string statut)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where ctr.StatutTC == statut && ctr.IsDoublon == "N" && ctr.IdEscDébarquement == esc.IdEsc && esc.IdArm == 1
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByStatutAndTypeForWAF(string statut, string typeCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    from esc in dcPar.GetTable<ESCALE>()
                    where ctr.StatutTC == statut && ctr.IsDoublon == "N" && ctr.TypeCtr == typeCtr && ctr.IdEscDébarquement == esc.IdEsc && esc.IdArm == 1
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByTypeAndStatut(string typeCtr, string statut, int idParc)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.StatutTC == statut && ctr.TypeCtr == typeCtr && ctr.IdParcParquing == idParc && ctr.IsDoublon == "N"
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public List<CONTENEUR_TC> GetConteneurTCsByTypeAndStatutAndNumTC(string typeCtr, string statut, int idParc, string numTC)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.StatutTC == statut && ctr.TypeCtr == typeCtr && ctr.IdParcParquing == idParc && ctr.NumTC.Contains(numTC) && ctr.IsDoublon == "N"
                    orderby ctr.NumTC ascending
                    select ctr).ToList<CONTENEUR_TC>();
        }

        public CONTENEUR_TC GetConteneurTCByIdCtr(int idCtr)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.IdCtr == idCtr && ctr.IsDoublon == "N"
                    orderby ctr.NumTC ascending
                    select ctr).FirstOrDefault<CONTENEUR_TC>();
        }

        public List<TYPE_OPERATION> GetTypeOperationsCtr()
        {
            return (from typeOp in dcPar.GetTable<TYPE_OPERATION>()
                    where typeOp.CodeMvt != null
                    orderby typeOp.CodeMvt ascending
                    select typeOp).ToList<TYPE_OPERATION>();
        }

        public CONTENEUR_TC GetConteneurTCByIdTC(int idTC)
        {
            return (from ctr in dcPar.GetTable<CONTENEUR_TC>()
                    where ctr.IdTC == idTC
                    orderby ctr.NumTC ascending
                    select ctr).FirstOrDefault<CONTENEUR_TC>();
        }

        #endregion
    }
}

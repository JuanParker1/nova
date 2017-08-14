using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represente l'ensemble des données de base du systeme : acconnier, armateur,utilisateur,navire,port etc..
    /// </summary>
    public class VsomConfig:SuperClass
    {
        VSOMClassesDataContext dcConfig = new VSOMClassesDataContext();
        public VsomConfig():base()
        {
        }

        #region Emplacements

        public string InsertEmplacement(string lign, int ind1, int ind2, int userid)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                string response = string.Empty;
                string machinename = string.Empty;
                try { machinename = Environment.MachineName; }
                catch { }
                EMPLACEMENT emp; string col;
                int i = ind1;
                while (i <= ind2)
                {
                    if (i < 10)
                        col = string.Format("0{0}", i);
                    else col = i.ToString();

                    var matchedEmp = (from n in dcConfig.GetTable<EMPLACEMENT>()
                                      where n.LigEmpl == lign && n.ColEmpl == col
                                      select n).SingleOrDefault<EMPLACEMENT>();
                    if (matchedEmp == null)
                    {
                        emp = new EMPLACEMENT();
                        emp.ColEmpl = col;
                        emp.IdParc = 1;
                        emp.LargMax = 20;
                        emp.LargMin = 0;
                        emp.LigEmpl = lign;
                        emp.LongMax = 200;
                        emp.LongMin = 0;
                        emp.StatutEmpl = "A";
                        dcConfig.GetTable<EMPLACEMENT>().InsertOnSubmit(emp);

                        JOURNAL journal = new JOURNAL
                        {
                            IdU = userid,
                            IdOp = 253, //operation de creation d'emplacement
                            DOP = DateTime.Now,
                            IDEC = string.Format("PC:{0};Emp:{1}{2}", machinename, lign, col)
                        };
                        dcConfig.GetTable<JOURNAL>().InsertOnSubmit(journal);

                    }
                    else
                        response = string.Format("{0}{1}, {2}", lign, col, response);

                    i += 1;
                }

                dcConfig.SubmitChanges();
                transaction.Complete();
                return response;
            }
        }

        public string DeleteEmplacements(List<Emplacement> lst, int iduser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUser = (from user in dcConfig.GetTable<UTILISATEUR>()
                                   where user.IdU == iduser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }
                string lst_idemp = string.Empty;
                string resp = string.Empty;
                foreach (Emplacement emp in lst)
                {
                    //realod emplacement pour control occupation
                    var matchedemp = (from empl in dcConfig.GetTable<EMPLACEMENT>()
                                      where empl.OCCUPATION.Count(occ => !occ.DateFin.HasValue) == 0 && empl.IdParc == 1 && empl.StatutEmpl == "A" && empl.IdEmpl == emp.IdEmpl
                                      select empl).SingleOrDefault<EMPLACEMENT>();

                    if (matchedemp != null)
                    {
                        matchedemp.StatutEmpl = "I";
                    }
                    else
                    {
                        resp = string.Format("{0},{1}", emp.Empl, resp);
                    }

                }

                string machinename = string.Empty;
                try { machinename = Environment.MachineName; }
                catch { }

                JOURNAL journal = new JOURNAL
                {
                    IdU = iduser,
                    IdOp = 254, //operation de suppression d'emplacement
                    DOP = DateTime.Now,
                    IDEC = string.Format("PC:{0};Emp del:{1}", machinename, resp)
                }; dcConfig.GetTable<JOURNAL>().InsertOnSubmit(journal);

                dcConfig.SubmitChanges();
                transaction.Complete();
                return resp;
            }
        }

        #endregion

        /*
      * HA 13juin16 ajout du parametre coderadio
      */
        public NAVIRE InsertOrUpdateNavire(int idNav, string codeNav, string nomNav, string observations, int idArm, string codeRadio)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedNavire = (from n in dcConfig.GetTable<NAVIRE>()
                                     where n.IdNav == idNav
                                     select n).SingleOrDefault<NAVIRE>();

                if (matchedNavire == null)
                {
                    NAVIRE n = new NAVIRE();

                    n.CodeNav = codeNav;
                    n.NomNav = nomNav;
                    n.IdArm = idArm;
                    n.StatutNav = "A";
                    n.AINav = observations;
                    //HA 13juin16
                    n.CodeRadio = codeRadio;

                    dcConfig.GetTable<NAVIRE>().InsertOnSubmit(n);
                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return n;
                }
                else
                {
                    matchedNavire.CodeNav = codeNav;
                    matchedNavire.NomNav = nomNav;
                    matchedNavire.IdArm = idArm;
                    matchedNavire.StatutNav = "A";
                    matchedNavire.AINav = observations;
                    //HA 13juin16
                    matchedNavire.CodeRadio = codeRadio;

                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return matchedNavire;
                }
            }
        }

        public ARTICLE InsertArticle(int idfamille, int codearticle, string libarticle, string ccarticle, string ccarticleex, string codetva, int pu, string unite)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matched = (from n in dcConfig.GetTable<ARTICLE>()
                               where n.CodeArticle == codearticle
                               select n).SingleOrDefault<ARTICLE>();

                if (matched == null)
                {
                    matched = new ARTICLE();
                    matched.CodeArticle = (short)codearticle;
                    matched.CodeFamArt = (short)idfamille;
                    matched.LibArticle = libarticle; matched.CCArticle = ccarticle; matched.CCArticleEx = ccarticleex;
                    matched.CodeTVA = codetva;
                    matched.Statut = "A";
                    dcConfig.GetTable<ARTICLE>().InsertOnSubmit(matched);

                    LIGNE_PRIX lp = new LIGNE_PRIX();
                    lp.CodeArticle = matched.CodeArticle;
                    lp.DescLP = matched.LibArticle;
                    lp.PU1LP = (short)pu;
                    lp.UniteLP = unite;
                    lp.DDLP = DateTime.Today;
                    lp.DFLP = DateTime.Today.AddYears(10);
                    
                    dcConfig.GetTable<LIGNE_PRIX>().InsertOnSubmit(lp);

                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return matched;
                }
                else
                {
                    throw new ApplicationException("Un article portant ce code existe deja");
                }
            }
        }

        public ARTICLE ActiveArticle(short codearticle, string motif)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matched = (from n in dcConfig.GetTable<ARTICLE>()
                               where n.CodeArticle == codearticle
                               select n).SingleOrDefault<ARTICLE>();
                matched.Statut = "A";
                dcConfig.SubmitChanges();
                transaction.Complete();
 
                return matched;
            }
        }

        public ARTICLE DesactiveArticle(short codearticle, string motif)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matched = (from n in dcConfig.GetTable<ARTICLE>()
                               where n.CodeArticle == codearticle
                               select n).SingleOrDefault<ARTICLE>();
                matched.Statut = "I";

                //string machinename = string.Empty;
                //try { machinename = Environment.MachineName; }
                //catch { }

                //JOURNAL journal = new JOURNAL
                //{
                //    IdU = iduser,
                //    IdOp = 290,
                //    DOP = DateTime.Now,
                //    IDEC = string.Format("PC:{0}", machinename)
                //};
                //dcConfig.GetTable<JOURNAL>().InsertOnSubmit(journal);

                dcConfig.SubmitChanges();
                transaction.Complete();
                return matched;
            }
        }

        public ARTICLE UpdateArticle(int idfamille, int codearticle, string libarticle, string ccarticle, string ccarticleex, string codetva, int pu, string unite)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matched = (from n in dcConfig.GetTable<ARTICLE>()
                               where n.CodeArticle == codearticle
                               select n).SingleOrDefault<ARTICLE>();

                matched.CodeArticle = (short)codearticle;
                matched.CodeFamArt = (short)idfamille;
                matched.LibArticle = libarticle; matched.CCArticle = ccarticle; matched.CCArticleEx = ccarticleex;
                matched.CodeTVA = codetva;

                dcConfig.SubmitChanges();
                transaction.Complete();
                return matched;
            }
        }

        public ACCONIER InsertOrUpdateAcconier(int idAcc, string codeAcc, string nomAcc, string observations)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedAcconier = (from a in dcConfig.GetTable<ACCONIER>()
                                       where a.IdAcc == idAcc
                                       select a).SingleOrDefault<ACCONIER>();

                if (matchedAcconier == null)
                {
                    ACCONIER a = new ACCONIER();

                    a.CodeAcc = codeAcc;
                    a.NomAcc = nomAcc;
                    a.StatutAcc = "A";
                    //a.CCAcc = ccAcc;
                    a.AIAcc = observations;

                    dcConfig.GetTable<ACCONIER>().InsertOnSubmit(a);
                    dcConfig.SubmitChanges();

                    LIGNE_PRIX lp = new LIGNE_PRIX();

                    lp.CodeArticle = 1702;
                    lp.DescLP = "Manutention Terre Socomar - Véhicule";
                    lp.PU1LP = 10000;
                    lp.PU2LP = 0;
                    lp.PU3LP = 0;
                    lp.PU4LP = 0;
                    lp.PU5LP = 0;
                    lp.DDLP = new DateTime(2012, 01, 01);
                    lp.DFLP = new DateTime(2020, 12, 31);
                    lp.UniteLP = "u";
                    lp.LP = a.IdAcc.ToString();

                    dcConfig.GetTable<LIGNE_PRIX>().InsertOnSubmit(lp);
                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return a;
                }
                else
                {
                    matchedAcconier.CodeAcc = codeAcc;
                    matchedAcconier.NomAcc = nomAcc;
                    //matchedAcconier.CCAcc = ccAcc;
                    matchedAcconier.StatutAcc = "A";
                    matchedAcconier.AIAcc = observations;

                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return matchedAcconier;
                }
            }
        }

        public ARMATEUR InsertOrUpdateArmateur(int idArm, string codeArm, string nomArm, string adresseArm, string ccArm, string observations)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedArmateur = (from a in dcConfig.GetTable<ARMATEUR>()
                                       where a.IdArm == idArm
                                       select a).SingleOrDefault<ARMATEUR>();

                if (matchedArmateur == null)
                {
                    ARMATEUR a = new ARMATEUR();

                    a.CodeArm = codeArm;
                    a.NomArm = nomArm;
                    a.AdresseArm = adresseArm;
                    a.StatutArm = "A";
                    a.CCArm = ccArm;
                    a.AIArm = observations;

                    dcConfig.GetTable<ARMATEUR>().InsertOnSubmit(a);
                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return a;
                }
                else
                {
                    matchedArmateur.CodeArm = codeArm;
                    matchedArmateur.NomArm = nomArm;
                    matchedArmateur.AdresseArm = adresseArm;
                    matchedArmateur.CCArm = ccArm;
                    matchedArmateur.StatutArm = "A";
                    matchedArmateur.AIArm = observations;

                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return matchedArmateur;
                }
            }
        }

        public PORT InsertOrUpdatePort(string codePort, string nomPort, string paysPort)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedPort = (from p in dcConfig.GetTable<PORT>()
                                   where p.CodePort == codePort
                                   select p).SingleOrDefault<PORT>();

                if (matchedPort == null)
                {
                    PORT p = new PORT();

                    p.CodePort = codePort;
                    p.NomPort = nomPort;
                    p.PaysPort = paysPort;

                    dcConfig.GetTable<PORT>().InsertOnSubmit(p);
                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return p;
                }
                else
                {
                    matchedPort.NomPort = nomPort;
                    matchedPort.PaysPort = paysPort;

                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    return matchedPort;
                }
            }
        }

        public UTILISATEUR InsertOrUpdateUtilisateur(int idU, string login, string password, string nom, string caisse, int idAcc, int idParc,
                           string statut, string observations, List<OPERATION> droits, int userAction)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUser = (from u in dcConfig.GetTable<UTILISATEUR>()
                                   where u.IdU == idU
                                   select u).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    UTILISATEUR user = new UTILISATEUR();

                    user.LU = login;
                    /* 12/06/16 
                     * on crypte maintenant le mot de passe pour ne plus l'avoir en claire dans le systeme
                     * user.MPU = password;
                     */
                    PwdHash pwdhash = new PwdHash(password);
                    user.MPU = pwdhash.Encrypt();

                    user.NU = nom;
                    user.Caisse = caisse;
                    user.IdAcc = idAcc;
                    if (idParc != -1)
                    {
                        user.IdParc = idParc;
                    }
                    user.EU = statut;
                    user.AIU = observations;

                    dcConfig.GetTable<UTILISATEUR>().InsertOnSubmit(user);

                    dcConfig.SubmitChanges();

                    foreach (OPERATION op in droits)
                    {
                        DROIT d = new DROIT();
                        d.IdU = user.IdU;
                        d.IdOp = op.IdOp;

                        dcConfig.DROIT.InsertOnSubmit(d);
                    }

                    string machinename = string.Empty;
                    try { machinename = Environment.MachineName; }
                    catch { }
                    var json = string.Format("ID:{0},LU:{1},MPW:{2}", user.IdU, user.LU, user.MPU);
                    JOURNAL journal = new JOURNAL
                    {
                        IdU = (int)userAction,
                        IdOp = 260,
                        DOP = DateTime.Now,
                        IDEC = string.Format("PC:{0}UserAct:{1},JSON:{2}", machinename, userAction, json)
                    };
                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    /*12/06/16
                     * remet le pwd sans hash
                     */
                    user.MPU = password;
                    return user;
                }
                else
                {
                    // matchedUser.MPU = password;
                    /* 12/06/16 
                     * on crypte maintenant le mot de passe pour ne plus l'avoir en claire dans le systeme
                     * matchedUser.MPU = password;
                     */
                    PwdHash pwdhash = new PwdHash(password);
                    matchedUser.MPU = pwdhash.Encrypt();

                    matchedUser.NU = nom;
                    matchedUser.Caisse = caisse;
                    matchedUser.IdAcc = idAcc;
                    if (idParc != -1)
                    {
                        matchedUser.IdParc = idParc;
                    }
                    matchedUser.EU = statut;
                    matchedUser.AIU = observations;

                    List<DROIT> listDroits = dcConfig.DROIT.Where(d => d.IdU == idU).ToList();

                    dcConfig.DROIT.DeleteAllOnSubmit(listDroits);

                    dcConfig.SubmitChanges();

                    foreach (OPERATION op in droits)
                    {
                        DROIT d = new DROIT();
                        d.IdU = matchedUser.IdU;
                        d.IdOp = op.IdOp;

                        dcConfig.DROIT.InsertOnSubmit(d);
                    }

                    string machinename = string.Empty;
                    try { machinename = Environment.MachineName; }
                    catch { }
                    var json = string.Format("ID:{0},LU:{1},MPW:{2},EU:{3}", matchedUser.IdU, matchedUser.LU, matchedUser.MPU, matchedUser.EU);

                    JOURNAL journal = new JOURNAL
                    {
                        IdU = (int)userAction,
                        IdOp = 260,
                        DOP = DateTime.Now,
                        IDEC = string.Format("PC:{0},UserAct:{1},JSON:{2}", machinename, userAction, json)
                    };
                    dcConfig.GetTable<JOURNAL>().InsertOnSubmit(journal);


                    dcConfig.SubmitChanges();
                    transaction.Complete();
                    /*12/06/16
                     * remet le pwd sans hash
                     */
                    ///matchedUser.MPU = password;
                    return matchedUser;
                }
            }
        }

    }
}

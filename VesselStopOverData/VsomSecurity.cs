using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represente l'authentification des utilisateur et de log des opérations 
    /// </summary>
   public class VsomSecurity:SuperClass
    {
       VSOMClassesDataContext dcSecur = new VSOMClassesDataContext();
       public VsomSecurity()
           : base()
       { }
        #region Droits
        
        public UTILISATEUR SeConnecter(string compte, string password)
        {

            using (var transaction = new System.Transactions.TransactionScope())
            {
                /* 12/06/16 pour mettre en place le cryptage de mot de passe
                 * var matchedUtilisateur = (from u in dcSecur.GetTable<UTILISATEUR>()
                                           where u.LU == compte && u.MPU == password
                                           select u).FirstOrDefault<UTILISATEUR>();
                 */
                PwdHash pwdhash = new PwdHash(password);
                string pwd = pwdhash.Encrypt();
                var matchedUtilisateur = (from u in dcSecur.GetTable<UTILISATEUR>()
                                          where u.LU == compte && u.MPU == pwd
                                          select u).FirstOrDefault<UTILISATEUR>();

                if (matchedUtilisateur == null)
                {
                    throw new ConnexionException("Paramètres de connexion incorrects !!!");
                }

                if (matchedUtilisateur.EU != "A")
                {
                    throw new ConnexionException("Votre compte est actuellement désactivé; veuillez contacter votre administrateur !!!");
                }

                string machinename = string.Empty;
                try { machinename = Environment.MachineName; }
                catch { }

                JOURNAL journal = new JOURNAL
                {
                    IdU = matchedUtilisateur.IdU,
                    IdOp = 1,
                    DOP = DateTime.Now,
                    IDEC = string.Format("PC:{0}", machinename)
                };
                dcSecur.GetTable<JOURNAL>().InsertOnSubmit(journal);
                dcSecur.SubmitChanges();
                transaction.Complete();

                /* met a jour la valeurdu mot de passe en decrypte. sil arrive ici c'est que le password saisie est bon, dc on peut le use */
                matchedUtilisateur.MPU = password;
                return matchedUtilisateur;
            }
        }

        public UTILISATEUR UpdatePassword(int idUser, string password)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedUtilisateur = (from u in dcSecur.GetTable<UTILISATEUR>()
                                          where u.IdU == idUser
                                          select u).SingleOrDefault<UTILISATEUR>();

                if (matchedUtilisateur == null)
                {
                    throw new EnregistrementInexistant("Cet utilisateur n'existe pas");
                }

                /*
                 * 12/06/16 crypte le mot de passe avant de le save
                 */
                PwdHash pwdhash = new PwdHash(password);
                matchedUtilisateur.MPU = pwdhash.Encrypt(); //password

                string machinename = string.Empty;
                try { machinename = Environment.MachineName; }
                catch { }

                JOURNAL journal = new JOURNAL
                {
                    IdU = matchedUtilisateur.IdU,
                    IdOp = 251,
                    DOP = DateTime.Now,
                    IDEC = string.Format("PC:{0}", machinename)
                };
                dcSecur.GetTable<JOURNAL>().InsertOnSubmit(journal);

                dcSecur.SubmitChanges();
                transaction.Complete();

                //on remet le new password non crypte avant denvoie lobjet
                matchedUtilisateur.MPU = password;
                return matchedUtilisateur;
            }
        }

        #endregion

        #region journal
        public void Log(int idoperation, int iduser, string comment, System.Transactions.TransactionScope trans )
        {
            string machinename = string.Empty;
            try { machinename = Environment.MachineName; }
            catch { }

            JOURNAL journal = new JOURNAL
            {
                IdU = iduser,
                IdOp = 1,
                DOP = DateTime.Now,
                IDEC = string.Format("PC:{0}", machinename)
            };
            dcSecur.GetTable<JOURNAL>().InsertOnSubmit(journal);
            dcSecur.SubmitChanges();
            trans.Complete();
        }
        #endregion
    }
}

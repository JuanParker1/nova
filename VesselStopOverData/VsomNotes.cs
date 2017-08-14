using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VesselStopOverData
{
    /// <summary>
    /// represente l'ensemble des insertion de note du systeme. elle ne manipule que la table NOTE 
    /// </summary>
    public class VsomNotes : SuperClass
    {
        VSOMClassesDataContext dcNotes = new VSOMClassesDataContext();

        public VsomNotes() : base()
        { 
        
        }

        public NOTE InsertNoteEscale(int idEsc, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedEscale = (from esc in dcNotes.GetTable<ESCALE>()
                                     where esc.IdEsc == idEsc
                                     select esc).SingleOrDefault<ESCALE>();

                if (matchedEscale == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdEsc = idEsc;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteManifeste(int idMan, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedManifeste = (from man in dcNotes.GetTable<MANIFESTE>()
                                        where man.IdMan == idMan
                                        select man).SingleOrDefault<MANIFESTE>();

                if (matchedManifeste == null)
                {
                    throw new EnregistrementInexistant("Le manifeste auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdMan = idMan;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteConnaissement(int idBL, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBL = (from bl in dcNotes.GetTable<CONNAISSEMENT>()
                                 where bl.IdBL == idBL
                                 select bl).SingleOrDefault<CONNAISSEMENT>();

                if (matchedBL == null)
                {
                    throw new EnregistrementInexistant("Le connaissement auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdBL = idBL;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteBooking(int idBooking, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBooking = (from book in dcNotes.GetTable<CONNAISSEMENT>()
                                      where book.IdBL == idBooking
                                      select book).SingleOrDefault<CONNAISSEMENT>();

                if (matchedBooking == null)
                {
                    throw new EnregistrementInexistant("Le booking auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdBL = idBooking;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteVehicule(int idVeh, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVehicule = (from veh in dcNotes.GetTable<VEHICULE>()
                                       where veh.IdVeh == idVeh
                                       select veh).SingleOrDefault<VEHICULE>();

                if (matchedVehicule == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdVeh = idVeh;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteConteneur(int idCtr, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedCtr = (from ctr in dcNotes.GetTable<CONTENEUR>()
                                  where ctr.IdCtr == idCtr
                                  select ctr).SingleOrDefault<CONTENEUR>();

                if (matchedCtr == null)
                {
                    throw new EnregistrementInexistant("Le conteneur auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdCtr = idCtr;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteConventionnel(int idGC, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedGC = (from gc in dcNotes.GetTable<CONVENTIONNEL>()
                                 where gc.IdGC == idGC
                                 select gc).SingleOrDefault<CONVENTIONNEL>();

                if (matchedGC == null)
                {
                    throw new EnregistrementInexistant("Le conventionnel auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdGC = idGC;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteCubage(int idCub, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedCubage = (from cub in dcNotes.GetTable<CUBAGE>()
                                     where cub.IdCubage == idCub
                                     select cub).SingleOrDefault<CUBAGE>();

                if (matchedCubage == null)
                {
                    throw new EnregistrementInexistant("Le projet de cubage auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdCubage = idCub;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteReduction(int idDDR, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedDDR = (from ddr in dcNotes.GetTable<DEMANDE_REDUCTION>()
                                  where ddr.IdDDR == idDDR
                                  select ddr).SingleOrDefault<DEMANDE_REDUCTION>();

                if (matchedDDR == null)
                {
                    throw new EnregistrementInexistant("La demande de réduction à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdDDR = idDDR;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteProforma(int idProf, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedProforma = (from prof in dcNotes.GetTable<PROFORMA>()
                                       where prof.IdFP == idProf
                                       select prof).SingleOrDefault<PROFORMA>();

                if (matchedProforma == null)
                {
                    throw new EnregistrementInexistant("La proforma à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdFP = idProf;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteFacture(int idFact, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedFacture = (from fact in dcNotes.GetTable<FACTURE>()
                                      where fact.IdFD == idFact
                                      select fact).SingleOrDefault<FACTURE>();

                if (matchedFacture == null)
                {
                    throw new EnregistrementInexistant("La facture à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdFD = idFact;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteOrdreService(int idOS, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedOS = (from os in dcNotes.GetTable<ORDRE_SERVICE>()
                                 where os.IdOS == idOS
                                 select os).SingleOrDefault<ORDRE_SERVICE>();

                if (matchedOS == null)
                {
                    throw new EnregistrementInexistant("L'escale à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdOS = idOS;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteBonEnlever(int idBAE, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedBAE = (from bae in dcNotes.GetTable<BON_ENLEVEMENT>()
                                  where bae.IdBAE == idBAE
                                  select bae).SingleOrDefault<BON_ENLEVEMENT>();

                if (matchedBAE == null)
                {
                    throw new EnregistrementInexistant("Le bon à enlever auquel vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdBAE = idBAE;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteLivraison(int idDBL, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedLivraison = (from liv in dcNotes.GetTable<DEMANDE_LIVRAISON>()
                                        where liv.IdDBL == idDBL
                                        select liv).SingleOrDefault<DEMANDE_LIVRAISON>();

                if (matchedLivraison == null)
                {
                    throw new EnregistrementInexistant("La demande de livraison à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdDBL = idDBL;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteRestCaution(int idDRC, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedRestCaution = (from rest in dcNotes.GetTable<DEMANDE_CAUTION>()
                                          where rest.IdDRC == idDRC
                                          select rest).SingleOrDefault<DEMANDE_CAUTION>();

                if (matchedRestCaution == null)
                {
                    throw new EnregistrementInexistant("La demande de restitution de caution à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdDRC = idDRC;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteDemandeVisite(int idDV, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedVisite = (from visite in dcNotes.GetTable<DEMANDE_VISITE>()
                                     where visite.IdDV == idDV
                                     select visite).SingleOrDefault<DEMANDE_VISITE>();

                if (matchedVisite == null)
                {
                    throw new EnregistrementInexistant("La demande de visite à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdDV = idDV;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteExtensionFranchise(int idDEXT, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedExtensionFranchise = (from ext in dcNotes.GetTable<EXTENSION_FRANCHISE>()
                                                 where ext.IdDEXT == idDEXT
                                                 select ext).SingleOrDefault<EXTENSION_FRANCHISE>();

                if (matchedExtensionFranchise == null)
                {
                    throw new EnregistrementInexistant("La demande d'extension de franchise à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.IdDEXT = idDEXT;
                note.TitreNote = titreNote;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

        public NOTE InsertNoteFactDIT(int idFactDIT, string titreNote, string descNote, int idUser)
        {
            using (var transaction = new System.Transactions.TransactionScope())
            {
                var matchedFactDIT = (from factDIT in dcNotes.GetTable<FACTURE_DIT>()
                                      where factDIT.IdFactDIT == idFactDIT
                                      select factDIT).SingleOrDefault<FACTURE_DIT>();

                if (matchedFactDIT == null)
                {
                    throw new EnregistrementInexistant("La facture DIT à laquelle vous faites référence n'existe pas");
                }

                var matchedUser = (from user in dcNotes.GetTable<UTILISATEUR>()
                                   where user.IdU == idUser
                                   select user).SingleOrDefault<UTILISATEUR>();

                if (matchedUser == null)
                {
                    throw new EnregistrementInexistant("L'utilisateur auquel vous faites référence n'existe pas");
                }

                NOTE note = new NOTE();
                note.TitreNote = titreNote;
                note.IdFactDIT = idFactDIT;
                note.DescNote = descNote;
                note.IdU = idUser;
                note.DateNote = DateTime.Now;

                dcNotes.NOTE.InsertOnSubmit(note);
                dcNotes.NOTE.Context.SubmitChanges();

                transaction.Complete();
                return note;
            }
        }

    }
}

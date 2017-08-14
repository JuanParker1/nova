using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    class FormLoader
    {
        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;
        //private //VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;

        public FormLoader()
        {
            vsomAcc = new VSOMAccessors();
        }
        public FormLoader(UTILISATEUR user)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            vsomAcc = new VSOMAccessors();
            utilisateur = user;
            operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
        }

        

        public void LoadEscaleForm(EscaleForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
            form.txtNumVoyageSCR.Text = esc.NumVoySCR;
            form.txtNomCapt.Text = esc.NomCpt;
            form.txtDateDepart.SelectedDate = esc.DDEsc;
            form.txtDateArrPrev.SelectedDate = esc.DPAEsc;
            form.txtDateArrReelle.SelectedDate = esc.DRAEsc;
            form.txtDateCreation.SelectedDate = esc.DCEsc;
            form.txtDateDech.SelectedDate = esc.DDechEsc;
            form.txtNbManifestesPrevus.Text = esc.NbrePManEsc.ToString();
            form.txtNbManifestesReel.Text = esc.MANIFESTE.Count.ToString();
            form.cbNavire.SelectedItem = esc.NAVIRE.NomNav;
            form.cbArmateur.SelectedItem = esc.ARMATEUR.NomArm;
            form.cbAcconier.SelectedItem = esc.ACCONIER.NomAcc;
            form.txtNumSydonia.Text = esc.NumManifestSydonia;
            form.txtNumVoyageDIT.Text = esc.NumVoyDIT;
            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(esc.AIEsc)));

            if (esc.RGPEsc.Equals("Y"))
            {
                form.chkGestPar.IsChecked = true;
            }
            else
            {
                form.chkGestPar.IsChecked = false;
            }

            if (esc.RREsc.Equals("Y"))
            {
                form.chkRepresentant.IsChecked = true;
            }
            else
            {
                form.chkRepresentant.IsChecked = false;
            }

            if (esc.RAEsc.Equals("Y"))
            {
                form.chkAcconier.IsChecked = true;
            }
            else
            {
                form.chkAcconier.IsChecked = false;
            }

            if (esc.RCEsc.Equals("Y"))
            {
                form.chkConsignataire.IsChecked = true;
            }
            else
            {
                form.chkConsignataire.IsChecked = false;
            }

            if (esc.MANIFESTE.Count != 0)
            {
                form.chkAcconier.IsEnabled = false;
                form.chkConsignataire.IsEnabled = false;
                form.chkGestPar.IsEnabled = false;
                form.chkRepresentant.IsEnabled = false;

                form.cbAcconier.IsEnabled = false;
                form.cbArmateur.IsEnabled = false;
                form.cbNavire.IsEnabled = false;
            }
            else
            {
                form.chkAcconier.IsEnabled = true;
                form.chkConsignataire.IsEnabled = true;
                form.chkGestPar.IsEnabled = true;
                form.chkRepresentant.IsEnabled = true;

                form.cbAcconier.IsEnabled = true;
                form.cbArmateur.IsEnabled = true;
                form.cbNavire.IsEnabled = true;
            }

            if (operationsUser.Where(op => op.NomOp == "Escale : Imprimer Booking").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                form.btnImprimerBooking.IsEnabled = false;
            }

            if (operationsUser.Where(op => op.NomOp == "Escale : Imprimer Recap Booking").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                form.btnImprimerRecap.IsEnabled = false;
            }

            if (operationsUser.Where(op => op.NomOp == "Escale : Imprimer Facture Stevedoring").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                form.btnImprimerFactureStevedoring.IsEnabled = false;
            }

            if (operationsUser.Where(op => op.NomOp == "Escale : Importation SOCAR").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                form.btnImportSOCAR.IsEnabled = false;
            }

            if (operationsUser.Where(op => op.NomOp == "Escale : Edition Manifeste ASYCUDA").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                form.btnManifestASYCUDA.IsEnabled = false;
            }

            //Gestion de la facturation armateur
            //form.btnFacturerArm.IsEnabled = esc.SOP == "C" && !esc.IdFArm.HasValue ? true : false;
            form.borderEtat.Visibility = esc.SOP == "C" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsEscale(esc.IdEsc);

            //chargement des manifestes
            form.manifestes = esc.MANIFESTE.ToList<MANIFESTE>();
            form.dataGridManifeste.ItemsSource = form.manifestes;
            form.lblStatutManifeste.Content = form.manifestes.Count + " Manifeste(s)";

            // valeur de l'escale
            form.eltsValeurEsc = vsomAcc.GetValeurEscale(esc.IdEsc);
            form.dataGridValeurEsc.ItemsSource = form.eltsValeurEsc;
            form.montantHTCpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantHT);
            form.montantTVACpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantTTC);

            //Element facturation escale
            form.eltsFact = vsomAcc.GetElementFacturationEsc(esc.IdEsc);
            form.dataGridEltsFactEsc.ItemsSource = form.eltsFact;
            form.eltsRegroupArm = vsomAcc.GetCompteArmRegroup(esc.IdEsc);
            System.Windows.Data.ListCollectionView collection = new System.Windows.Data.ListCollectionView(form.eltsRegroupArm);
            collection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("Regroup"));
            form.dataGridEltsRegroupArm.ItemsSource = collection;

            // valeur armateur
            form.montantHTCpteArm.Content = form.eltsFact.Sum(elt => elt.MontantHT);
            form.montantTVACpteArm.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteArm.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            //chargement des factures armateur
            form.facturesArmateur = esc.FACTURE_ARMATEUR.ToList<FACTURE_ARMATEUR>();
            form.dataGridFactArm.ItemsSource = form.facturesArmateur;

            //chargement facture socomar
            form.facturesSocomar = esc.FACTURE_SOCOMAR.ToList<FACTURE_SOCOMAR>();
            form.dataGridFactSoc.ItemsSource = form.facturesSocomar;

            //Compte Escale
            form.dataGridCompteEsc.ItemsSource = vsomAcc.GetCompteEsc(esc.IdEsc);

            ////compte du DIT
            //form.eltsCompteDIT = vsomAcc.GetCompteDITEscale(esc.IdEsc);
            //form.dataGridCompteDIT.ItemsSource = form.eltsCompteDIT;
            //form.facturesDIT = vsomAcc.GetCompteFacturationDITEscale(esc.IdEsc);
            //form.dataGridFactDIT.ItemsSource = form.facturesDIT;
            //form.montantFactureCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantFacture);
            //form.montantDITCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantDIT);

            //compte SEPBC
            form.eltsCompteSEPBC = vsomAcc.GetCompteSEPBCEscale(esc.IdEsc);
            form.dataGridCompteSEPBC.ItemsSource = form.eltsCompteSEPBC;
            form.montantFactureCpteSEPBC.Content = form.eltsCompteSEPBC.Sum(elt => elt.MontantFacture);

            //Compte du PAD

            // statut de déchargement
            form.statutDechargement = vsomAcc.GetStatutDechargementEsc(esc.IdEsc);
            form.nbBL.Content = esc.CONNAISSEMENT.Count(bl => bl.SensBL == "I");
            form.nbVeh.Content = form.statutDechargement.NbVehicule + "/" + esc.VEHICULE.Count;
            form.nbCont.Content = form.statutDechargement.NbConteneur + "/" + esc.CONTENEUR.Where(c => c.SensCtr == "I").GroupBy(c => c.NumCtr).ToList().Count;
            form.nbMafi.Content = form.statutDechargement.NbMafi + "/" + esc.MAFI.GroupBy(m => m.NumMafi).ToList().Count;
            form.nbCargos.Content = form.statutDechargement.NbConventionnel + "/" + esc.CONVENTIONNEL.Count(c => c.SensGC == "I");
            form.volTonVeh.Content = Math.Round(esc.VEHICULE.Sum(veh => veh.VolMVeh.Value), 3) + " m³ - " + Math.Round(esc.VEHICULE.Sum(veh => (double)veh.PoidsMVeh.Value / 1000), 3) + " t";
            form.volTonCtr.Content = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "I").Sum(ctr => ctr.VolMCtr) + " m³ - " + Math.Round(esc.CONTENEUR.Where(ctr => ctr.SensCtr == "I").Sum(ctr => (double)ctr.PoidsMCtr.Value / 1000), 3) + " t";
            form.volTonMafi.Content = esc.MAFI.Where(mf => mf.SensMafi == "I").Sum(mf => mf.VolMMafi) + " m³ - " + Math.Round(esc.MAFI.Where(mf => mf.SensMafi == "I").Sum(mf => (double)mf.PoidsMMafi.Value / 1000), 3) + " t";
            form.volTonCargo.Content = Math.Round(esc.CONVENTIONNEL.Where(conv => conv.SensGC == "I").Sum(conv => conv.VolMGC.Value), 3) + " m³ - " + Math.Round(esc.CONVENTIONNEL.Where(conv => conv.SensGC == "I").Sum(conv => (double)conv.PoidsMGC.Value / 1000), 3) + " t";

            //Notes
            form.listNotes.ItemsSource = esc.NOTE;

            //Statut de l'escale
            form.statutDechargement = vsomAcc.GetStatutDechargementEsc(esc.IdEsc);
            if (form.statutDechargement.NbVehicule == esc.VEHICULE.Count && form.statutDechargement.NbConteneur == esc.CONTENEUR.Count && form.statutDechargement.NbMafi == esc.MAFI.Count && form.statutDechargement.NbConventionnel == esc.CONVENTIONNEL.Count && esc.CONNAISSEMENT.Count != 0)
            {
                form.lblDechargement.Content = "Déchargement terminé";
                //form.txtDateArrReelle.IsEnabled = false;
            }
            else if (form.statutDechargement.NbVehicule == 0 && form.statutDechargement.NbConteneur == 0 && form.statutDechargement.NbMafi == 0 && form.statutDechargement.NbConventionnel == 0)
            {
                form.lblDechargement.Content = "Déchargement non démarré";
                //form.txtDateArrReelle.IsEnabled = true;
            }
            else
            {
                form.lblDechargement.Content = "Déchargement en cours";
                //form.txtDateArrReelle.IsEnabled = false;
            }

            //if (esc.CONNAISSEMENT.Count(con => con.ELEMENT_FACTURATION.Count(ef => ef.IdFD.HasValue) != 0) != 0)
            //{
            //    form.txtDateArrReelle.IsEnabled = false;
            //}

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsEscale(esc.IdEsc);

            form.lblStatut.Content = "Statut de l'escale : " + esc.StatEsc;

            form.Title = "Escale - " + esc.NumEsc + " - " + esc.NAVIRE.NomNav;
        }

        public void LoadEscaleForm(SummaryOPForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
            form.txtDateArrReelle.SelectedDate = esc.DRAEsc;
            form.cbNavire.SelectedItem = esc.NAVIRE.NomNav;
            form.cbArmateur.SelectedItem = esc.ARMATEUR.NomArm;

            form.opArms = vsomAcc.GetOperationArmOfEscale(esc.IdEsc);
            form.ops = new List<string>();
            form.lignesOpArm = new List<ElementLigneOpArm>();
            foreach (OPERATION_ARMATEUR opArm in form.opArms)
            {
                LIGNE_PRIX lp = vsomAcc.GetLignePrixOpArm(esc.IdArm.Value, opArm.IdTypeOp);

                form.ops.Add(opArm.TYPE_OPERATION.LibTypeOp);

                ElementLigneOpArm eltOpArm = new ElementLigneOpArm();
                eltOpArm.StatutOp = opArm.TYPE_OPERATION.Statut;
                eltOpArm.IdTypeOp = opArm.IdTypeOp;
                eltOpArm.Operation = opArm.TYPE_OPERATION.LibTypeOp;
                eltOpArm.Poids = opArm.Poids.HasValue ? opArm.Poids.Value : 0;
                eltOpArm.Volume = opArm.Volume.HasValue ? opArm.Volume.Value : 0;
                eltOpArm.Qte = opArm.QTE.HasValue ? opArm.QTE.Value : 0;
                eltOpArm.PrixUnitaire = lp.PU1LP.Value;

                if (eltOpArm.Operation == "Discharging General Cargo")
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Poids, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "Loading General Cargo")
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Poids, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "Discharging Pallets")
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Poids, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "Loading Sawn Timber")
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Volume, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "Sawn timber") // Karim
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Volume, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "Lashing operations") // Karim
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Volume, 0, MidpointRounding.AwayFromZero);
                }
                else if (eltOpArm.Operation == "General Cargo") // Karim
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Poids, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    eltOpArm.PrixTotal = (int)Math.Round(eltOpArm.PrixUnitaire * eltOpArm.Qte, 0, MidpointRounding.AwayFromZero);
                }

                form.lignesOpArm.Add(eltOpArm);

            }
            form.cbOperation.ItemsSource = null;
            form.cbOperation.ItemsSource = form.ops;

            //Valeur stevedoring operations
            form.lblStatutStevedoring.Content = form.lignesOpArm.Sum(lg => lg.PrixTotal);

            form.dataGridEltOpArm.ItemsSource = null;
            System.Windows.Data.ListCollectionView collection = new System.Windows.Data.ListCollectionView(form.lignesOpArm);
            collection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("StatutOp"));
            form.dataGridEltOpArm.ItemsSource = collection;

            //Statut de l'escale
            form.statutDechargement = vsomAcc.GetStatutDechargementEsc(esc.IdEsc);
            if (form.statutDechargement.NbVehicule == esc.VEHICULE.Count && form.statutDechargement.NbConteneur == esc.CONTENEUR.Count && form.statutDechargement.NbMafi == esc.MAFI.Count && form.statutDechargement.NbConventionnel == esc.CONVENTIONNEL.Count && esc.CONNAISSEMENT.Count != 0)
            {
                form.lblDechargement.Content = "Déchargement terminé";
                form.txtDateArrReelle.IsEnabled = false;
            }
            else if (form.statutDechargement.NbVehicule == 0 && form.statutDechargement.NbConteneur == 0 && form.statutDechargement.NbMafi == 0 && form.statutDechargement.NbConventionnel == 0)
            {
                form.lblDechargement.Content = "Déchargement non démarré";
                form.txtDateArrReelle.IsEnabled = true;
            }
            else
            {
                form.lblDechargement.Content = "Déchargement en cours";
                form.txtDateArrReelle.IsEnabled = false;
            }

            if (esc.SOP == "C")
            {
                form.btnEnregistrer.IsEnabled = false;
                form.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                form.btnMAJ.IsEnabled = false;
            }
            else
            {
                form.btnEnregistrer.IsEnabled = true;
                form.actionsBorder.Visibility = System.Windows.Visibility.Visible;
                form.btnMAJ.IsEnabled = true;
            }

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsEscale(esc.IdEsc);

            // valeur de l'escale
            form.eltsValeurEsc = vsomAcc.GetValeurEscale(esc.IdEsc);
            form.montantHTCpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantHT);
            form.montantTVACpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteEscale.Content = form.eltsValeurEsc.Sum(elt => elt.MontantTTC);

            // valeur armateur
            form.eltsFact = vsomAcc.GetElementFacturationEsc(esc.IdEsc);
            form.montantHTCpteArm.Content = form.eltsFact.Sum(elt => elt.MontantHT);
            form.montantTVACpteArm.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteArm.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            ////compte du DIT
            //form.eltsCompteDIT = vsomAcc.GetCompteDITEscale(esc.IdEsc);
            //form.montantFactureCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantFacture);
            //form.montantDITCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantDIT);

            //compte SEPBC
            form.eltsCompteSEPBC = vsomAcc.GetCompteSEPBCEscale(esc.IdEsc);
            form.montantFactureCpteSEPBC.Content = form.eltsCompteSEPBC.Sum(elt => elt.MontantFacture);

            //Compte du PAD

            // statut de déchargement
            form.statutDechargement = vsomAcc.GetStatutDechargementEsc(esc.IdEsc);
            form.nbBL.Content = esc.CONNAISSEMENT.Count(bl => bl.SensBL == "I");
            form.nbVeh.Content = form.statutDechargement.NbVehicule + "/" + esc.VEHICULE.Count;
            form.nbCont.Content = form.statutDechargement.NbConteneur + "/" + esc.CONTENEUR.Count;
            form.nbMafi.Content = form.statutDechargement.NbMafi + "/" + esc.MAFI.Count;
            form.nbCargos.Content = form.statutDechargement.NbConventionnel + "/" + esc.CONVENTIONNEL.Count;
            form.volTonVeh.Content = Math.Round(esc.VEHICULE.Sum(veh => veh.VolMVeh.Value), 3) + " m³ / " + Math.Round(esc.VEHICULE.Sum(veh => (double)veh.PoidsMVeh.Value / 1000), 3) + " t";
            form.volTonCtr.Content = esc.CONTENEUR.Where(ctr => ctr.SensCtr == "I").Sum(ctr => ctr.VolMCtr) + " m³ / " + Math.Round(esc.CONTENEUR.Where(ctr => ctr.SensCtr == "I").Sum(ctr => (double)ctr.PoidsMCtr.Value / 1000), 3) + " t";
            form.volTonMafi.Content = esc.MAFI.Where(mf => mf.SensMafi == "I").Sum(mf => mf.VolMMafi) + " m³ / " + Math.Round(esc.MAFI.Where(mf => mf.SensMafi == "I").Sum(mf => (double)mf.PoidsMMafi.Value / 1000), 3) + " t";
            form.volTonCargo.Content = Math.Round(esc.CONVENTIONNEL.Where(conv => conv.SensGC == "I").Sum(conv => conv.VolMGC.Value), 3) + " m³ / " + Math.Round(esc.CONVENTIONNEL.Where(conv => conv.SensGC == "I").Sum(conv => (double)conv.PoidsMGC.Value / 1000), 3) + " t";

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsEscale(esc.IdEsc);

            form.lblStatut.Content = "Statut de l'escale : " + esc.StatEsc;

            form.Title = "Summary of operations : Escale : " + esc.NumEsc.ToString() + " - Armateur : " + esc.ARMATEUR.NomArm;
        }

        public void LoadEscaleForm(OrdreServiceForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbEscale.SelectedItem = esc.NumEsc;
            form.txtNumVoyage.Text = esc.NumVoySCR;
            form.txtArmateur.Text = esc.ARMATEUR.NomArm;
            form.txtNavire.Text = esc.NAVIRE.NomNav;
        }

        public void LoadEscaleForm(BookingForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbEscale.ItemsSource = null;
            form.cbEscale.ItemsSource = form.escs;
            form.cbEscale.SelectedItem = esc.NumEsc;
            form.txtNumVoyage.Text = esc.NumVoySCR;
            form.cbNavire.SelectedItem = esc.NAVIRE.NomNav;
        }

        public void LoadEscaleForm(CubageForm form, ESCALE esc)
        {
            vsomAcc = new VSOMAccessors();
            ////VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
            form.txtVoyage.Text = esc.NumVoySCR;
            form.txtNavire.Text = esc.NAVIRE.NomNav;
            form.txtAcconier.Text = esc.ACCONIER.NomAcc;
            form.txtArmateur.Text = esc.ARMATEUR.NomArm;

            if (form.cbIdCub.IsEnabled == false)
            {
                // il s'agit d'un nouveau projet de cubage
                form.dataGridVehicules.ItemsSource = vsomAcc.GetVehiculesEscalePourCubage(esc.IdEsc);
            }
            else
            {
                // lister les véhicules du cubage en cours
                // les véhicules cubés doivent être surlignés en rouge
            }
        }

        public void LoadEscaleForm(ManifestExportForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
        }

        public void LoadEscaleForm(RapportDebarquementForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            ////VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursDebarques(esc.IdEsc);

            form.dataGrid.ItemsSource = listCtrs.GroupBy(c => c.NumCtr).Select(c => c.First());
            form.lblStatut.Content = listCtrs.Count + " conteneur débarqués";

        }

        public void LoadEscaleForm(RapportDebarquementExcelForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursDebarques(esc.IdEsc);

            form.dataGrid.ItemsSource = listCtrs.GroupBy(c => c.NumCtr).Select(c => c.First());
            form.lblStatut.Content = listCtrs.Count + " conteneur débarqués";

        }

        public void LoadEscaleForm(DischargingReportExcelForm form, ESCALE esc)
        {
            vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursDebarques(esc.IdEsc);

            form.dataGrid.ItemsSource = listCtrs.GroupBy(c => c.NumCtr).Select(c => c.First());
            form.lblStatut.Content = listCtrs.Count + " conteneur débarqués";

        }

        public void LoadEscaleForm(LoadingReportExcelForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursEmbarques(esc.IdEsc);// esc.CONTENEUR.Where<CONTENEUR>(ct => /*ct.SensCtr == "I" &&*/ ct.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283) != null).ToList<CONTENEUR>();

            form.dataGrid.ItemsSource = listCtrs;
            form.lblStatut.Content = listCtrs.Count + " conteneur embarqués";

        }

        public void LoadEscaleForm(RapportEmbarquementForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursEmbarques(esc.IdEsc);// esc.CONTENEUR.Where<CONTENEUR>(ct => /*ct.SensCtr == "I" &&*/ ct.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283) != null).ToList<CONTENEUR>();

            form.dataGrid.ItemsSource = listCtrs;
            form.lblStatut.Content = listCtrs.Count + " conteneur embarqués";

        }

        public void LoadEscaleForm(RapportEmbarquementExcelForm form, ESCALE esc)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();

            List<CONTENEUR> listCtrs = vsomAcc.GetConteneursEmbarques(esc.IdEsc);// esc.CONTENEUR.Where<CONTENEUR>(ct => /*ct.SensCtr == "I" &&*/ ct.CONTENEUR_TC.FirstOrDefault<CONTENEUR_TC>().MOUVEMENT_TC.FirstOrDefault<MOUVEMENT_TC>(op => op.IdTypeOp == 283) != null).ToList<CONTENEUR>();

            form.dataGrid.ItemsSource = listCtrs;
            form.lblStatut.Content = listCtrs.Count + " conteneur embarqués";

        }

        public void LoadEscaleForm(ExtractPrevImportDITForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
        }

        public void LoadEscaleForm(ExtractPrevExportDITForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
        }

        public void LoadEscaleForm(TransfertBookingEscaleForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtEscaleSysID.Text = esc.IdEsc.ToString();
        }

        public void LoadEscaleForm(ManifesteForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = esc.NumEsc;
            form.txtNumEscale.Text = esc.IdEsc.ToString();
        }

        public void LoadEscaleForm(ConnaissementForm form, ESCALE esc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.escales = new List<ESCALE>();
            form.escales.Add(esc);
            form.escs = new List<int>();
            form.escs.Add(esc.NumEsc.Value);
            form.cbEscale.ItemsSource = null;
            form.cbEscale.ItemsSource = form.escs;
            form.cbEscale.SelectedItem = esc.NumEsc;
            form.txtNumVoyage.Text = esc.NumVoySCR;

            form.manifestes = new List<MANIFESTE>();
            form.manifestes.AddRange(esc.MANIFESTE);
            form.manifs = new List<int>();
            foreach (MANIFESTE man in esc.MANIFESTE)
            {
                form.manifs.Add(man.IdMan);
                form.cbManifeste.ItemsSource = null;
                form.cbManifeste.ItemsSource = form.manifs;
            }
        }

        public void LoadManifesteForm(ManifesteForm form, MANIFESTE man)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdMan.Text = man.IdMan.ToString();

            form.escales = new List<ESCALE>();
            form.escales.Add(man.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(man.ESCALE.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = man.ESCALE.NumEsc;

            form.cbPort.SelectedItem = man.PORT.NomPort;
            form.txtDateCreation.SelectedDate = man.DCMan;
            form.txtNbPrevBL.Text = man.NPBLMan.ToString();
            form.txtNbEffBL.Text = man.CONNAISSEMENT.Count.ToString();
            form.txtNbPrevVeh.Text = man.NPVMan.ToString();
            form.txtNbEffVeh.Text = man.VEHICULE.Count.ToString();
            form.txtNbPrevCont.Text = man.NPCMan.ToString();
            form.txtNbEffCont.Text = man.CONTENEUR.Count.ToString();
            form.txtNbPrevMafi.Text = man.NPMMan.ToString();
            form.txtNbEffMafi.Text = man.MAFI.Count.ToString();
            form.txtNbPrevGC.Text = man.NPGCMan.ToString();
            form.txtNbEffGC.Text = man.CONVENTIONNEL.Count.ToString();
            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(man.AIMan)));
            form.cbFormat.SelectedIndex = Convert.ToInt32(man.FormatMan);
            form.txtCheminFichier.Text = man.CAFMan;

            form.connaissements = man.CONNAISSEMENT.ToList<CONNAISSEMENT>();
            form.dataGridConnaissement.ItemsSource = form.connaissements;

            form.vehicules = man.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                if (form.vehTab.IsSelected == true)
                {
                    form.compteManTab.IsSelected = true;
                }
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
            }

            form.conteneurs = man.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                if (form.conTab.IsSelected == true)
                {
                    form.compteManTab.IsSelected = true;
                }
            }
            else
            {
                form.dataGridCont.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
            }

            form.mafis = man.MAFI.ToList<MAFI>();
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
                if (form.mafiTab.IsSelected == true)
                {
                    form.compteManTab.IsSelected = true;
                }
            }
            else
            {
                form.dataGridMafi.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
            }

            form.conventionnels = man.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                if (form.gcTab.IsSelected == true)
                {
                    form.compteManTab.IsSelected = true;
                }
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
            }

            if (man.DVMan.HasValue)
            {
                form.btnValidMan.Visibility = System.Windows.Visibility.Collapsed;
                form.btnEnregistrer.IsEnabled = false;
                form.gridDetails.IsEnabled = false;
            }
            else
            {
                form.btnValidMan.Visibility = System.Windows.Visibility.Visible;
                form.btnEnregistrer.IsEnabled = true;
                form.gridDetails.IsEnabled = true;
            }

            if (man.StatutMan == "En cours")
            {
                form.btnCloturer.Visibility = System.Windows.Visibility.Visible;
                form.gridDetails.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }
            else if (man.StatutMan == "Clôturé")
            {
                form.btnCloturer.Visibility = System.Windows.Visibility.Collapsed;
                form.gridDetails.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsManifeste(man.IdMan);

            //valeur du manifeste
            form.eltsValeurMan = vsomAcc.GetCompteManifeste(man.IdMan);
            form.dataGridValeurMan.ItemsSource = form.eltsValeurMan;
            form.montantHTCpteManifeste.Content = form.eltsValeurMan.Sum(elt => elt.MontantHT);
            form.montantTVACpteManifeste.Content = form.eltsValeurMan.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteManifeste.Content = form.eltsValeurMan.Sum(elt => elt.MontantTTC);

            //Compte du manifeste
            form.dataGridCompteMan.ItemsSource = vsomAcc.GetCompteMan(man.IdMan);

            // statut de déchargement
            form.statutDechargement = vsomAcc.GetStatutDechargementMan(man.IdMan);
            form.nbVeh.Content = form.statutDechargement.NbVehicule + "/" + man.VEHICULE.Count;
            form.nbCont.Content = form.statutDechargement.NbConteneur + "/" + man.CONTENEUR.GroupBy(c => c.NumCtr).ToList().Count;
            form.nbMafi.Content = form.statutDechargement.NbMafi + "/" + man.MAFI.GroupBy(m => m.NumMafi).ToList().Count;
            form.nbCargos.Content = form.statutDechargement.NbConventionnel + "/" + man.CONVENTIONNEL.Count;
            form.volTonVeh.Content = Math.Round(man.VEHICULE.Sum(veh => veh.VolMVeh.Value), 3) + " m³ - " + Math.Round(man.VEHICULE.Sum(veh => (double)veh.PoidsMVeh.Value / 1000), 3) + " t";
            form.volTonCtr.Content = man.CONTENEUR.Sum(ctr => ctr.VolMCtr) + " m³ - " + Math.Round(man.CONTENEUR.Sum(ctr => (double)ctr.PoidsMCtr.Value / 1000), 3) + " t";
            form.volTonMafi.Content = man.MAFI.Sum(mf => mf.VolMMafi) + " m³ - " + Math.Round(man.MAFI.Sum(mf => (double)mf.PoidsMMafi.Value / 1000), 3) + " t";
            form.volTonCargo.Content = Math.Round(man.CONVENTIONNEL.Sum(conv => conv.VolMGC.Value), 3) + " m³ - " + Math.Round(man.CONVENTIONNEL.Sum(conv => (double)conv.PoidsMGC.Value / 1000), 3) + " t";

            ////compte du DIT
            //form.eltsCompteDIT = vsomAcc.GetCompteDITManifeste(man.IdMan);
            //form.dataGridCompteDIT.ItemsSource = form.eltsCompteDIT;
            //form.montantFactureCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantFacture);
            //form.montantDITCpteDIT.Content = form.eltsCompteDIT.Sum(elt => elt.MontantDIT);

            //Synthèse des marchandises
            form.nbAttelle.Content = man.VEHICULE.Count(veh => veh.IdVehAP.HasValue);
            form.nbCle.Content = man.VEHICULE.Count(veh => veh.VehCle == "Y");
            form.nbDemarre.Content = man.VEHICULE.Count(veh => veh.VehStart == "Y");
            form.nbMinutie.Content = man.VEHICULE.Count(veh => veh.VehChargé == "Y");
            form.nbPortes.Content = man.VEHICULE.Count(veh => veh.IdVehAP.HasValue);

            //Notes
            form.listNotes.ItemsSource = man.NOTE;

            form.txtRechercher.IsEnabled = true;

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsManifeste(man.IdMan);

            form.Title = "Manifeste - " + man.PORT.CodePort + " - Escale - " + Convert.ToInt32(man.ESCALE.NumEsc);
        }

        public void LoadConnaissementForm(ConnaissementForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.cbClient.IsEnabled = con.ELEMENT_FACTURATION.Count(el => el.IdFD.HasValue) == 0 ? true : false;

            //
            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            foreach (ESCALE esc in form.escales)
            {
                form.escs.Add(esc.NumEsc.Value);
            }
            form.cbEscale.ItemsSource = null;
            form.cbEscale.ItemsSource = form.escs;
            form.cbEscale.SelectedItem = con.ESCALE.NumEsc;

            form.manifestes = new List<MANIFESTE>();
            form.manifs = new List<Int32>();
            form.manifestes.Add(con.MANIFESTE);
            foreach (MANIFESTE man in form.manifestes)
            {
                form.manifs.Add(man.IdMan);
            }
            form.cbManifeste.ItemsSource = null;
            form.cbManifeste.ItemsSource = form.manifs;
            form.cbManifeste.SelectedItem = con.MANIFESTE.IdMan;
            //

            form.txtNumSydonia.Text = con.ESCALE.NumManifestSydonia;

            form.txtPortProv.Text = con.LPBL;
            form.cbDestFinale.Text = con.DestBL;
            if (con.CONTENEUR.Count(ctr => ctr.IdPay.HasValue) == con.CONTENEUR.Count)
            {
                form.cbDestFinale.IsEnabled = false;
            }
            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtNotify.Text = con.NotifyBL;
            form.txtEmail.Text = con.EmailBL;
            form.txtNomChargeur.Text = con.NomCharger;
            form.txtAdresseChargeur.Text = con.AdresseCharger;

            if (con.StatutBL == "Cloturé")
            {
                form.btnEnregistrer.IsEnabled = false;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
                form.btnProforma.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.btnEnregistrer.IsEnabled = true;
                form.btnAccomplissement.Visibility = (!con.DVBL.HasValue || (operationsUser.Where(op => op.NomOp == "Connaissement : Modification des informations de base").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            }

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BLLT.Equals("Y"))
            {
                form.chkLineTerm.IsChecked = true;
            }
            else
            {
                form.chkLineTerm.IsChecked = false;
            }

            if (con.BLFO.Equals("Y"))
            {
                form.chkFreeOut.IsChecked = true;
            }
            else
            {
                form.chkFreeOut.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBLBloque.IsChecked = true;
                form.txtBLBloqueNote.Text = con.BLBloqueNote;
            }
            else
            {
                form.chkBLBloque.IsChecked = false;
                form.txtBLBloqueNote.Text = con.BLBloqueNote;
            }

            if (con.BLSocar.Equals("Y"))
            {
                form.chkBLSOCAR.IsChecked = true;
                form.txtNumSocar.IsEnabled = true;
                form.txtNumSocar.Text = con.NumSocar;
            }
            else
            {
                form.chkBLSOCAR.IsChecked = false;
                form.txtNumSocar.IsEnabled = false;
                form.txtNumSocar.Text = "";
            }

            if (con.CCBL.Equals("Y"))
            {
                form.chkFretCollecter.IsChecked = true;
                form.txtFretCollecter.IsEnabled = true;
                form.txtFretCollecter.Text = con.CCBLMontant.ToString();
            }
            else
            {
                form.chkFretCollecter.IsChecked = false;
                form.txtFretCollecter.IsEnabled = false;
                form.txtFretCollecter.Text = "0";
            }

            if (con.BLDette.Equals("Y"))
            {
                form.chkDetteCollecter.IsChecked = true;
                form.txtDetteCollecter.IsEnabled = true;
                form.txtDetteCollecter.Text = con.DetteMontant.ToString();
            }
            else
            {
                form.chkDetteCollecter.IsChecked = false;
                form.txtDetteCollecter.IsEnabled = false;
                form.txtDetteCollecter.Text = "0";
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkBLER.IsChecked = true;
                form.txtBLER.Text = con.BLERNote;
            }
            else
            {
                form.chkBLER.IsChecked = false;
                form.txtBLER.Text = con.BLERNote;
            }

            form.proformas = vsomAcc.GetProformasOfConnaissement(con.IdBL);
            if (form.proformas.Count == 0)
            {
                form.profTab.Visibility = System.Windows.Visibility.Collapsed;
                form.eltTab.IsSelected = true;
            }
            else
            {
                form.dataGridProfs.ItemsSource = form.proformas;
                form.profTab.Visibility = System.Windows.Visibility.Visible;
            }

            form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.mafis = con.MAFI.ToList<MAFI>();
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridMafi.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.listStatuts.ItemsSource = vsomAcc.GetStatutsConnaissement(con.IdBL);

            //Valeur du BL
            form.eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteBL.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            // statut de déchargement
            form.statutDechargement = vsomAcc.GetStatutDechargementBL(con.IdBL);
            form.nbVeh.Content = form.statutDechargement.NbVehicule + "/" + con.VEHICULE.Count;
            form.nbCont.Content = form.statutDechargement.NbConteneur + "/" + con.CONTENEUR.Where(c => c.SensCtr == "I").GroupBy(c => c.NumCtr).ToList().Count;
            form.nbMafi.Content = form.statutDechargement.NbMafi + "/" + con.MAFI.GroupBy(m => m.NumMafi).ToList().Count;
            form.nbCargos.Content = form.statutDechargement.NbConventionnel + "/" + con.CONVENTIONNEL.Count(c => c.SensGC == "I");
            form.volTonVeh.Content = Math.Round(con.VEHICULE.Sum(veh => veh.VolMVeh.Value), 3) + " m³ - " + Math.Round(con.VEHICULE.Sum(veh => (double)veh.PoidsMVeh.Value / 1000), 3) + " t";
            form.volTonCtr.Content = con.CONTENEUR.Sum(ctr => ctr.VolMCtr) + " m³ - " + Math.Round(con.CONTENEUR.Sum(ctr => (double)ctr.PoidsMCtr.Value / 1000), 3) + " t";
            form.volTonMafi.Content = con.MAFI.Sum(mf => mf.VolMMafi) + " m³ - " + Math.Round(con.MAFI.Sum(mf => (double)mf.PoidsMMafi.Value / 1000), 3) + " t";
            form.volTonCargo.Content = Math.Round(con.CONVENTIONNEL.Sum(conv => conv.VolMGC.Value), 3) + " m³ - " + Math.Round(con.CONVENTIONNEL.Sum(conv => (double)conv.PoidsMGC.Value / 1000), 3) + " t";

            //Notes
            form.listNotes.ItemsSource = con.NOTE;

            if (con.DVBL.HasValue)
            {
                form.btnValidBL.Visibility = System.Windows.Visibility.Collapsed;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
                form.btnExonererTVA.Visibility = System.Windows.Visibility.Collapsed;
                form.lblStatut.Content = "Connaissement validé le " + con.DVBL;
            }
            else
            {
                form.btnValidBL.Visibility = con.ESCALE.IdAcc != 1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
                form.btnExonererTVA.Visibility = System.Windows.Visibility.Visible;
                form.lblStatut.Content = "Connaissement non validé";
            }

            form.gridInfosBL.IsEnabled = (operationsUser.Where(op => op.NomOp == "Connaissement : Modification des informations de base").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin") ? false : true;
            form.groupIncoterms.IsEnabled = (operationsUser.Where(op => op.NomOp == "Connaissement : Modification des incoterms").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin") ? false : true;
            form.groupInfosChargeur.IsEnabled = (operationsUser.Where(op => op.NomOp == "Connaissement : Modification des informations du chargeur").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin") ? false : true;

            form.chkBLSOCAR.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (elt.StatutEF == "Proforma" || elt.IdFD != null) && (elt.LibEF.Contains("Débours SOCAR : BESC") || elt.LibEF.Contains("Débours SOCAR : Fret à collecter") || elt.LibEF.Contains("Débours SOCAR : Dette à collecter"))) == 0 ? true : false;
            form.chkFretCollecter.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (elt.StatutEF == "Proforma" || elt.IdFD != null) && elt.LibEF.Contains("Débours SOCAR : Fret à collecter")) == 0 /*&& form.chkBLSOCAR.IsChecked == true*/ ? true : false;
            form.txtFretCollecter.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (elt.StatutEF == "Proforma" || elt.IdFD != null) && elt.LibEF.Contains("Débours SOCAR : Fret à collecter")) == 0 /*&& form.chkBLSOCAR.IsChecked == true*/ && form.chkFretCollecter.IsChecked == true ? true : false;
            form.chkDetteCollecter.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (elt.StatutEF == "Proforma" || elt.IdFD != null) && elt.LibEF.Contains("Débours SOCAR : Dette à collecter")) == 0 && form.chkBLSOCAR.IsChecked == true ? true : false;
            form.txtDetteCollecter.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (elt.StatutEF == "Proforma" || elt.IdFD != null) && elt.LibEF.Contains("Débours SOCAR : Dette à collecter")) == 0 && form.chkBLSOCAR.IsChecked == true && form.chkDetteCollecter.IsChecked == true ? true : false;
            form.groupIncoterms.IsEnabled = con.ELEMENT_FACTURATION.Count(elt => (/*elt.StatutEF == "Proforma" || */elt.IdFD != null && elt.StatutEF !="Annule")) == 0 ? true : false;

            form.btnProforma.Visibility = con.DVBL.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            //form.cbClient.IsEnabled = con.PROFORMA.Count == 0 ? true : false;

            //Compte BL
            form.dataGridCompteBL.ItemsSource = vsomAcc.GetCompteBL(con.IdBL);
            double debit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Debit);
            double credit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Credit);
            form.lblStatutPaiement.Content = "Debit : " + debit + " / Crédit : " + credit + ". Solde du BL : " + (credit - debit).ToString();
            form.lblStatut.Content = "Statut du BL : " + con.StatutBL;
            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DDechEsc.HasValue ? con.ESCALE.DDechEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());

            form.actionsBorder.Visibility = System.Windows.Visibility.Visible;

            form.btnValidBL.Visibility = (form.listStatuts.Items.OfType<OPERATION_CONNAISSEMENT>().SingleOrDefault<OPERATION_CONNAISSEMENT>(op => op.IdTypeOp == 34).DateOp.HasValue && !con.DVBL.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.Title = "BL - " + con.NumBL + " - Consignée - " + con.ConsigneeBL;
        }

        public void LoadConnaissementForm(VAEForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            foreach (ESCALE esc in form.escales)
            {
                form.escs.Add(esc.NumEsc.Value);
            }
            form.cbEscale.ItemsSource = null;
            form.cbEscale.ItemsSource = form.escs;
            form.cbEscale.SelectedItem = con.ESCALE.NumEsc;

            form.manifestes = new List<MANIFESTE>();
            form.manifs = new List<Int32>();
            form.manifestes.Add(con.MANIFESTE);
            foreach (MANIFESTE man in form.manifestes)
            {
                form.manifs.Add(man.IdMan);
            }
            form.cbManifeste.ItemsSource = null;
            form.cbManifeste.ItemsSource = form.manifs;
            form.cbManifeste.SelectedItem = con.MANIFESTE.IdMan;
            //

            form.txtPortProv.Text = con.LPBL;
            form.cbDestFinale.Text = con.DestBL;
            if (con.CONTENEUR.Count(ctr => ctr.IdPay.HasValue) == con.CONTENEUR.Count)
            {
                form.cbDestFinale.IsEnabled = false;
            }
            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtNotify.Text = con.NotifyBL;
            form.txtEmail.Text = con.EmailBL;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BLLT.Equals("Y"))
            {
                form.chkLineTerm.IsChecked = true;
            }
            else
            {
                form.chkLineTerm.IsChecked = false;
            }

            if (con.BLFO.Equals("Y"))
            {
                form.chkFreeOut.IsChecked = true;
            }
            else
            {
                form.chkFreeOut.IsChecked = false;
            }

            form.lblStatut.Content = "Statut du BL : " + con.StatutBL;

            form.Title = "BL - " + con.NumBL + " - Consignée - " + con.ConsigneeBL;
        }

        public void LoadSinistreForm(SinistreForm form, OPERATION_SINISTRE sin)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.cbIdSinistre.Text = sin.IdOpSinistre.ToString();
            form.txtDateSinistre.SelectedDate = sin.DateSinistre.Value;

            this.LoadVehiculeForm(form, sin.VEHICULE);
        }

        public void LoadVehiculeForm(VehiculeForm form, VEHICULE veh)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.btnIdentifier.IsEnabled = false;
            form.btnCuber.IsEnabled = false;
            form.btnConstatSinistre.IsEnabled = false;
            form.btnReceptionner.IsEnabled = false;
            form.btnTransfertZoneSortie.IsEnabled = false;
            form.btnTransfEmpl.IsEnabled = false;
            form.btnSortir.IsEnabled = false;
            form.btnEnGC.IsEnabled = false;

            if (veh.StatVeh == "Non initié")
            {
                form.btnEnGC.IsEnabled = true;
                form.btnIdentifier.IsEnabled = true;
                form.btnReceptionner.IsEnabled = true;
            }
            else if (veh.StatVeh == "Initié")
            {
                form.btnIdentifier.IsEnabled = true;
                form.btnReceptionner.IsEnabled = true;
            }
            else if (veh.StatVeh == "Traité")
            {
                form.btnEnGC.IsEnabled = true;
                form.btnIdentifier.IsEnabled = true;
            }
            else if (veh.StatVeh == "Manifesté")
            {
                form.btnIdentifier.IsEnabled = true;
            }
            else if (veh.StatVeh == "Identifié/Déchargé")
            {
                form.btnIdentifier.IsEnabled = true;
                form.btnCuber.IsEnabled = true;
                form.btnConstatSinistre.IsEnabled = true;
                form.btnReceptionner.IsEnabled = true;
            }
            else if (veh.StatVeh == "Parqué")
            {
                form.btnTransfEmpl.IsEnabled = true;
                form.btnCuber.IsEnabled = true;
                form.btnConstatSinistre.IsEnabled = true;
            }
            else if (veh.StatVeh == "Enlèvement")
            {
                form.btnTransfEmpl.IsEnabled = true;
                form.btnConstatSinistre.IsEnabled = true;
            }
            else if (veh.StatVeh == "Livraison")
            {
                form.btnTransfEmpl.IsEnabled = true;
                form.btnTransfertZoneSortie.IsEnabled = true;
                form.btnConstatSinistre.IsEnabled = true;
            }
            else if (veh.StatVeh == "Sortie en cours" || veh.StatVeh == "Zone Sortie")
            {
                form.btnSortir.IsEnabled = true;
            }
            else if (veh.StatVeh == "Livré")
            {

            }

            form.borderEtat.Visibility = veh.IdBS.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            form.btnDuplicataBonSortie.Visibility = veh.DSRVeh.HasValue  ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
            form.btnImprimerBonSortie.Visibility = veh.DSRVeh.HasValue  ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            form.cbNumChassis.Text = veh.NumChassis;
            form.txtIdChassis.Text = veh.IdVeh.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(veh.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(veh.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.SelectedItem = veh.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = veh.DescVeh;
            form.txtCreeLe.SelectedDate = veh.DCVeh;
            form.txtBarCode.Text = veh.BarCode;
            form.txtLongM.Text = veh.LongMVeh.ToString();
            form.txtLargM.Text = veh.LargMVeh.ToString();
            form.txtHautM.Text = veh.HautMVeh.ToString();
            form.txtVolM.Text = veh.VolMVeh.ToString();
            form.txtPoidsM.Text = veh.PoidsMVeh.ToString();
            form.txtEtatM.Text = veh.StatutVeh;
            form.cbTypeVehM.SelectedIndex = veh.VolMVeh < 16 ? 0 : (veh.VolMVeh < 50 ? 1 : 2);

            form.txtLongC.Text = veh.LongCVeh.ToString();
            form.txtLargC.Text = veh.LargCVeh.ToString();
            form.txtHautC.Text = veh.HautCVeh.ToString();
            form.txtVolC.Text = veh.VolCVeh.ToString();
            form.txtPoidsC.Text = veh.PoidsCVeh.ToString();
            form.txtEtatC.Text = veh.StatutCVeh;
            form.cbTypeVehC.SelectedItem = veh.TypeCVeh;

            form.txtNumSydonia.Text = veh.ESCALE.NumManifestSydonia;

            form.chkCle.IsChecked = veh.VehCle == "Y" ? true : false;
            form.chkAttelle.IsChecked = veh.VehAttelle == "Y" ? true : false;
            form.chkDemarre.IsChecked = veh.VehStart == "Y" ? true : false;
            form.chkMinutie.IsChecked = veh.VehChargé == "Y" ? true : false;
            form.chkPorte.IsChecked = veh.VehPorte == "Y" ? true : false;
            if (veh.IdVehAP.HasValue)
            {
                form.txtChassisAP.Text = vsomAcc.GetVehiculePrincipal(veh.IdVehAP.Value).NumChassis;
                form.txtIdChassisAP.Text = veh.IdVehAP.ToString();
                //form.btnCalculerSejour.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.txtChassisAP.Text = "";
                form.txtIdChassisAP.Text = "";
                //form.btnCalculerSejour.Visibility = System.Windows.Visibility.Visible;
            }

            form.dataGridVehiculesAP.ItemsSource = vsomAcc.GetVehiculesAPByIdVeh(veh.IdVeh);

            form.numAttDed.Content = veh.NumADDVeh;
            form.numCIVIO.Content = veh.NumCIVIOveh;
            form.numDeclDouane.Content = veh.NumDDVeh;
            form.numQuittDouane.Content = veh.NumQDVeh;
            form.numFactPAD.Content = veh.NumFPADVeh;
            form.numQuittPAD.Content = veh.NumQPADVeh;
            form.numBAEPAD.Content = veh.NumAEPADVeh;
            form.numBESC.Content = veh.NumBESCVeh;
            form.numSydonia.Content = veh.NumSydoniaVeh;

            if (veh.CONNAISSEMENT.DVBL.HasValue)
            {
                form.groupInfosManif.IsEnabled = false;
                form.groupInfosCub.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.groupInfosManif.IsEnabled = true;
                form.groupInfosCub.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            //Notes
            form.listNotes.ItemsSource = veh.NOTE;

            form.eltsFact = vsomAcc.GetElementFacturationVeh(veh.IdVeh);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteVeh.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.debutSejour.Content = veh.ESCALE.DRAEsc;
            form.finFranchise.Content = veh.FFVeh;
            form.finStationnement.Content = veh.FSVeh;

            form.lblStatut.Content = "Statut du véhicule : " + veh.StatVeh;
            form.lblStatutBL.Content = "Statut du BL : " + veh.CONNAISSEMENT.StatutBL;
            form.lblAcconier.Content = "Acconier : " + veh.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + veh.ESCALE.NumEsc + " du " + (veh.ESCALE.DDechEsc.HasValue ? veh.ESCALE.DDechEsc.Value.ToShortDateString() : veh.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + veh.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = veh.NumADDVeh != null ? "Numéro d'attestation douane : " + veh.NumADDVeh : "Numéro d'attestation douane : Non dédouané";
            //List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
            //if (occupations.Count != 0)
            //{
            //    form.lblParking.Content = "Parking : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc + " : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
            //}
            //else
            //{
            //    form.lblParking.Content = "Parking : Non parqué";
            //}

            form.lblParking.Content = "Parking : " + vsomAcc.GetPositionVehicule(veh.IdVeh);

            if (utilisateur.IdAcc == 1)
            {
                form.actionsBorder.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                form.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
                form.btnEnregistrer.IsEnabled = false;
            }

            //form.btnCalculerSejour.Visibility = veh.IdAcc.Value == 1 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.Title = "Chassis - " + veh.NumChassis + " - Consignée - " + veh.CONNAISSEMENT.ConsigneeBL;

            // ajouter un blocage pour qu'on ne puisse pas facturer le séjour tant que le véhicule n'a pas été identifié
        }

        public void LoadVehiculeForm(SinistreForm form, VEHICULE veh)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
            form.cbNumChassis.Text = veh.NumChassis;
            form.txtIdChassis.Text = veh.IdVeh.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(veh.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(veh.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.SelectedItem = veh.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = veh.DescVeh;
            form.txtCreeLe.SelectedDate = veh.DCVeh;
            form.txtBarCode.Text = veh.BarCode;
            form.txtLongM.Text = veh.LongMVeh.ToString();
            form.txtLargM.Text = veh.LargMVeh.ToString();
            form.txtHautM.Text = veh.HautMVeh.ToString();
            form.txtVolM.Text = veh.VolMVeh.ToString();
            form.txtPoidsM.Text = veh.PoidsMVeh.ToString();
            form.txtEtatM.Text = veh.StatutVeh;
            form.cbTypeVehM.SelectedIndex = veh.VolMVeh < 16 ? 0 : (veh.VolMVeh < 50 ? 1 : 2);

            form.txtLongC.Text = veh.LongCVeh.ToString();
            form.txtLargC.Text = veh.LargCVeh.ToString();
            form.txtHautC.Text = veh.HautCVeh.ToString();
            form.txtVolC.Text = veh.VolCVeh.ToString();
            form.txtPoidsC.Text = veh.PoidsCVeh.ToString();
            form.txtEtatC.Text = veh.StatutCVeh;
            form.cbTypeVehC.SelectedItem = veh.TypeCVeh;

            form.chkCle.IsChecked = veh.VehCle == "Y" ? true : false;
            form.chkAttelle.IsChecked = veh.VehAttelle == "Y" ? true : false;
            form.chkDemarre.IsChecked = veh.VehStart == "Y" ? true : false;
            form.chkMinutie.IsChecked = veh.VehChargé == "Y" ? true : false;
            form.chkPorte.IsChecked = veh.VehPorte == "Y" ? true : false;
            if (veh.IdVehAP.HasValue)
            {
                form.txtChassisAP.Text = vsomAcc.GetVehiculePrincipal(veh.IdVehAP.Value).NumChassis;
                form.txtIdChassisAP.Text = veh.IdVehAP.ToString();
            }
            else
            {
                form.txtChassisAP.Text = "";
                form.txtIdChassisAP.Text = "";
            }

            if (veh.CONNAISSEMENT.DVBL.HasValue)
            {
                form.groupInfosManif.IsEnabled = false;
                form.groupInfosCub.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.groupInfosManif.IsEnabled = true;
                form.groupInfosCub.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            form.eltsFact = vsomAcc.GetElementFacturationVeh(veh.IdVeh);
            form.montantHTCpteVeh.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.lblStatut.Content = "Statut du véhicule : " + veh.StatVeh;
            form.lblAcconier.Content = "Acconier : " + veh.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + veh.ESCALE.NumEsc + " du " + (veh.ESCALE.DRAEsc.HasValue ? veh.ESCALE.DRAEsc.Value.ToShortDateString() : veh.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + veh.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = veh.NumADDVeh != null ? "Numéro d'attestation douane : " + veh.NumADDVeh : "Numéro d'attestation douane : Non dédouané";
            List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();
            if (occupations.Count != 0)
            {
                form.lblParking.Content = "Parking : " + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
            }
            else
            {
                form.lblParking.Content = "Parking : Non parqué";
            }

            form.Title = "Chassis - " + veh.NumChassis + " - Consignée - " + veh.CONNAISSEMENT.ConsigneeBL;

            form.typesSinistres = vsomAcc.GetTypesSinistreVeh();
            form.dataGridSinistres.ItemsSource = form.typesSinistres;

            // ajouter un blocage pour qu'on ne puisse pas facturer le séjour tant que le véhicule n'a pas été identifié

        }

        public void LoadConteneurForm(ConteneurForm form, CONTENEUR cont)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumCtr.Text = cont.NumCtr;
            form.txtIdCtr.Text = cont.IdCtr.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(cont.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(cont.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;

            form.cbNumBL.SelectedItem = cont.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = cont.DescCtr;
            form.txtCreeLe.SelectedDate = cont.DCCtr;
            form.txtIMDGCode.Text = cont.IMDGCode;
            form.txtDescMses.Text = cont.DescMses;

            form.txtPoids.Text = cont.PoidsMCtr.ToString();
            form.txtSeal1.Text = cont.Seal1Ctr;
            form.txtSeal2.Text = cont.Seal2Ctr;
            form.cbEtatM.SelectedIndex = cont.StatutCtr == "F" ? 0 : 1;
            form.cbTypeCtrM.SelectedItem = cont.TypeMCtr;

            form.txtNumSydonia.Text = cont.ESCALE.NumManifestSydonia;

            if (cont.TypeMses == "N")
            {
                form.cbTypeMses.SelectedIndex = 0;
            }
            else if (cont.TypeMses == "P")
            {
                form.cbTypeMses.SelectedIndex = 1;
            }
            else if (cont.TypeMses == "D")
            {
                form.cbTypeMses.SelectedIndex = 2;
            }
            else if (cont.TypeMses == "Coton/Banane")
            {
                form.cbTypeMses.SelectedIndex = 3;
            }
            else if (cont.TypeMses == "Café/Cacao")
            {
                form.cbTypeMses.SelectedIndex = 4;
            }
            else if (cont.TypeMses == "Bois débité")
            {
                form.cbTypeMses.SelectedIndex = 5;
            }
            else if (cont.TypeMses == "Autres produits")
            {
                form.cbTypeMses.SelectedIndex = 6;
            }

            form.cbTypeCtrC.SelectedItem = cont.TypeCCtr;
            form.txtPoidsC.Text = cont.PoidsCCtr.ToString();
            form.radioLigneMar.IsChecked = cont.PropCtr == 1 ? true : false;
            form.radioConsignee.IsChecked = cont.PropCtr == 2 ? true : false;
            form.txtCaution.Text = cont.MCCtr.ToString();

            form.eltsFact = vsomAcc.GetElementFacturationCtr(cont.IdCtr);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteCtr.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.franchiseSurestaries.Content = cont.FFCtr;
            form.franchiseSejour.Content = cont.FFSCtr;
            form.finSejour.Content = cont.FSCtr;

            //if (operationsUser.Where(op => op.NomOp == "Mise à jour des prix DIT").FirstOrDefault<OPERATION>() != null || utilisateur.LU == "Admin")
            //{
            //    form.btnDIT.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    form.btnDIT.Visibility = System.Windows.Visibility.Collapsed;
            //}

            form.numAttDed.Content = cont.NumADDCtr;
            form.numAVI.Content = cont.NumAVICtr;
            form.numDeclDouane.Content = cont.NumDDCtr;
            form.numQuittDouane.Content = cont.NumQDCtr;
            form.numFactPAD.Content = cont.NumFPADCtr;
            form.numQuittPAD.Content = cont.NumQPADCtr;
            form.numBAEPAD.Content = cont.NumAEPADCtr;
            form.numBESC.Content = cont.NumBESCCtr;
            form.numSydonia.Content = cont.NumSydoniaCtr;
            form.nbDet.Content = cont.NbDet.ToString() + " jour(s)";

            //Notes
            form.listNotes.ItemsSource = cont.NOTE;

            form.lblStatut.Content = "Statut du conteneur : " + cont.StatCtr;
            form.lblStatutBL.Content = "Statut du BL : " + cont.CONNAISSEMENT.StatutBL;
            form.lblAcconier.Content = "Acconier : " + cont.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + cont.ESCALE.NumEsc + " du " + (cont.ESCALE.DDechEsc.HasValue ? cont.ESCALE.DDechEsc.Value.ToShortDateString() : cont.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + cont.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = cont.NumADDCtr != null ? "Numéro d'attestation douane : " + cont.NumADDCtr : "Numéro d'attestation douane : Non dédouané";

            form.actionsBorder.Visibility = System.Windows.Visibility.Visible;

            form.groupInfosCub.IsEnabled = true;

            if (cont.SensCtr == "E")
            {
                form.btnEnregistrer.IsEnabled = false;
                form.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
            }

            form.Title = "Conteneur - " + cont.NumCtr + " - Consignée - " + cont.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadConteneurTCForm(ConteneurTCForm form, CONTENEUR_TC contTC)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumCtr.Text = contTC.CONTENEUR.NumCtr;
            form.txtIdCtr.Text = contTC.IdCtr.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(contTC.CONTENEUR.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(contTC.CONTENEUR.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;

            form.cbNumBL.SelectedItem = contTC.CONTENEUR.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = contTC.CONTENEUR.DescCtr;
            form.txtCreeLe.SelectedDate = contTC.CONTENEUR.DCCtr;
            form.txtIMDGCode.Text = contTC.CONTENEUR.IMDGCode;
            form.txtDescMses.Text = contTC.CONTENEUR.DescMses;

            form.txtPoids.Text = contTC.CONTENEUR.PoidsMCtr.ToString();
            form.txtSeal1.Text = contTC.CONTENEUR.Seal1Ctr;
            form.txtSeal2.Text = contTC.CONTENEUR.Seal2Ctr;
            form.cbEtatM.SelectedIndex = contTC.CONTENEUR.StatutCtr == "F" ? 0 : 1;
            form.cbTypeCtrM.SelectedItem = contTC.CONTENEUR.TypeMCtr;

            if (contTC.CONTENEUR.TypeMses == "N")
            {
                form.cbTypeMses.SelectedIndex = 0;
            }
            else if (contTC.CONTENEUR.TypeMses == "P")
            {
                form.cbTypeMses.SelectedIndex = 1;
            }
            else if (contTC.CONTENEUR.TypeMses == "D")
            {
                form.cbTypeMses.SelectedIndex = 2;
            }
            else if (contTC.CONTENEUR.TypeMses == "Coton/Banane")
            {
                form.cbTypeMses.SelectedIndex = 3;
            }
            else if (contTC.CONTENEUR.TypeMses == "Café/Cacao")
            {
                form.cbTypeMses.SelectedIndex = 4;
            }
            else if (contTC.CONTENEUR.TypeMses == "Bois débité")
            {
                form.cbTypeMses.SelectedIndex = 5;
            }
            else if (contTC.CONTENEUR.TypeMses == "Autres produits")
            {
                form.cbTypeMses.SelectedIndex = 6;
            }

            form.cbTypeCtrC.SelectedItem = contTC.CONTENEUR.TypeCCtr;
            form.txtPoidsC.Text = contTC.CONTENEUR.PoidsCCtr.ToString();
            form.radioLigneMar.IsChecked = contTC.CONTENEUR.PropCtr == 1 ? true : false;
            form.radioConsignee.IsChecked = contTC.CONTENEUR.PropCtr == 2 ? true : false;
            form.txtCaution.Text = contTC.CONTENEUR.MCCtr.ToString();

            if (contTC.Reparation == "Y")
            {
                form.chkReparation.IsChecked = true;
            }
            else
            {
                form.chkReparation.IsChecked = false;
            }

            if (contTC.Nettoyage == "Y")
            {
                form.chkNettoyage.IsChecked = true;
            }
            else
            {
                form.chkNettoyage.IsChecked = false;
            }

            if (contTC.Lavage == "Y")
            {
                form.chkLavage.IsChecked = true;
            }
            else
            {
                form.chkLavage.IsChecked = false;
            }

            if (contTC.Irreparable == "Y")
            {
                form.chkIrrep.IsChecked = true;
            }
            else
            {
                form.chkIrrep.IsChecked = false;
            }

            if (contTC.IdCtrExport.HasValue)
            {
                form.btnRetourPlein.IsEnabled = true;
                form.btnEmbarquer.IsEnabled = true;
            }
            else
            {
                form.btnRetourPlein.IsEnabled = false;
                form.btnEmbarquer.IsEnabled = false;
            }

            if (contTC.IdParcParquing.HasValue)
            {
                form.btnRetourArmateur.IsEnabled = true;
            }
            else
            {
                form.btnRetourArmateur.IsEnabled = false;
            }

            form.btnRestitution.IsEnabled = contTC.StatutTC == "Cargo Loaded" ? true : false;
            form.eltsFact = vsomAcc.GetElementFacturationCtr(contTC.CONTENEUR.IdCtr);
            form.montantHTCpteCtr.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.eltsMvt = contTC.MOUVEMENT_TC.OrderBy(mvt => mvt.DateMvt).ToList<MOUVEMENT_TC>();
            form.dataGridMvt.ItemsSource = form.eltsMvt;

            form.franchiseSurestaries.Content = contTC.CONTENEUR.FFCtr;
            form.franchiseSejour.Content = contTC.CONTENEUR.FFSCtr;
            form.finSejour.Content = contTC.CONTENEUR.FSCtr;

            form.numAttDed.Content = contTC.CONTENEUR.NumADDCtr;
            form.numAVI.Content = contTC.CONTENEUR.NumAVICtr;
            form.numDeclDouane.Content = contTC.CONTENEUR.NumDDCtr;
            form.numQuittDouane.Content = contTC.CONTENEUR.NumQDCtr;
            form.numFactPAD.Content = contTC.CONTENEUR.NumFPADCtr;
            form.numQuittPAD.Content = contTC.CONTENEUR.NumQPADCtr;
            form.numBAEPAD.Content = contTC.CONTENEUR.NumAEPADCtr;
            form.numBESC.Content = contTC.CONTENEUR.NumBESCCtr;
            form.numSydonia.Content = contTC.CONTENEUR.NumSydoniaCtr;
            form.nbDet.Content = contTC.CONTENEUR.NbDet.ToString() + " jour(s)";

            //Notes
            form.listNotes.ItemsSource = contTC.CONTENEUR.NOTE;

            form.lblStatut.Content = "Statut du conteneur : " + contTC.StatutTC;
            form.lblStatutBL.Content = "Statut du BL : " + contTC.CONTENEUR.CONNAISSEMENT.StatutBL;
            form.lblAcconier.Content = "Acconier : " + contTC.CONTENEUR.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + contTC.CONTENEUR.ESCALE.NumEsc + " du " + (contTC.CONTENEUR.ESCALE.DDechEsc.HasValue ? contTC.CONTENEUR.ESCALE.DDechEsc.Value.ToShortDateString() : contTC.CONTENEUR.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + contTC.CONTENEUR.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = contTC.CONTENEUR.NumADDCtr != null ? "Numéro d'attestation douane : " + contTC.CONTENEUR.NumADDCtr : "Numéro d'attestation douane : Non dédouané";
            form.lblParking.Content = "Parking : " + vsomAcc.GetPositionConteneur(contTC.CONTENEUR.IdCtr);
            form.lblStatutParking.Content = "Etat Parking : " + contTC.EtatRetourVide;

            form.actionsBorder.Visibility = System.Windows.Visibility.Visible;

            form.groupInfosCub.IsEnabled = true;

            //if (contTC.CONTENEUR.SensCtr == "E")
            //{
            //    form.btnEnregistrer.IsEnabled = false;
            //    form.actionsBorder.Visibility = System.Windows.Visibility.Collapsed;
            //}

            form.Title = "Conteneur - " + contTC.CONTENEUR.NumCtr + " - Consignée - " + contTC.CONTENEUR.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadMafiForm(MafiForm form, MAFI mafi)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumMafi.Text = mafi.NumMafi;
            form.txtIdMafi.Text = mafi.IdMafi.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(mafi.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(mafi.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;

            form.cbNumBL.SelectedItem = mafi.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = mafi.DescMafi;
            form.txtCreeLe.SelectedDate = mafi.DCMafi;
            form.txtIMDGCode.Text = mafi.IMDGCode;
            form.txtDescMses.Text = mafi.DescMses;

            form.txtNumSydonia.Text = mafi.ESCALE.NumManifestSydonia;

            form.txtPoids.Text = mafi.PoidsMMafi.ToString();
            form.txtTypeMMafi.Text = mafi.TypeMMafi;

            if (mafi.CONNAISSEMENT.DVBL.HasValue)
            {
                form.groupInfosManif.IsEnabled = false;
                form.groupInfosCub.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.groupInfosManif.IsEnabled = true;
                form.groupInfosCub.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            form.numAttDed.Content = mafi.NumADDMafi;
            form.numAVI.Content = mafi.NumAVIMafi;
            form.numDeclDouane.Content = mafi.NumDDMafi;
            form.numQuittDouane.Content = mafi.NumQDMafi;
            form.numFactPAD.Content = mafi.NumFPADMafi;
            form.numQuittPAD.Content = mafi.NumQPADMafi;
            form.numBAEPAD.Content = mafi.NumAEPADMafi;
            form.numBESC.Content = mafi.NumBESCMafi;
            form.numSydonia.Content = mafi.NumSydoniaMafi;

            //Notes
            form.listNotes.ItemsSource = mafi.NOTE;

            form.txtTypeCMafi.Text = mafi.TypeCMafi;
            form.txtPoidsC.Text = mafi.PoidsCMafi.ToString();
            form.txtVolMafi.Text = mafi.VolMMafi.ToString();

            form.eltsFact = vsomAcc.GetElementFacturationMafi(mafi.IdMafi);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteCtr.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteCtr.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.franchiseSurestaries.Content = mafi.FFMafi;
            form.franchiseSejour.Content = mafi.FFSMafi;
            form.finSejour.Content = mafi.FSMafi;

            form.lblStatut.Content = "Statut du mafi : " + mafi.StatMafi;
            form.lblStatutBL.Content = "Statut du BL : " + mafi.CONNAISSEMENT.StatutBL;
            form.lblAcconier.Content = "Acconier : " + mafi.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + mafi.ESCALE.NumEsc + " du " + (mafi.ESCALE.DDechEsc.HasValue ? mafi.ESCALE.DDechEsc.Value.ToShortDateString() : mafi.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + mafi.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = mafi.NumADDMafi != null ? "Numéro d'attestation douane : " + mafi.NumADDMafi : "Numéro d'attestation douane : Non dédouané";

            form.actionsBorder.Visibility = System.Windows.Visibility.Visible;

            form.groupInfosCub.IsEnabled = true;

            form.Title = "Mafi - " + mafi.NumMafi + " - Consignée - " + mafi.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadConventionnelForm(ConventionnelForm form, CONVENTIONNEL conv)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumGC.Text = conv.NumGC;
            form.txtIdGC.Text = conv.IdGC.ToString();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(conv.CONNAISSEMENT);

            form.cons = new List<string>();
            form.cons.Add(conv.CONNAISSEMENT.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.SelectedItem = conv.CONNAISSEMENT.NumBL;
            form.txtDescription.Text = conv.DescGC;
            form.txtCreeLe.SelectedDate = conv.DCGC;
            form.txtBarCode.Text = conv.BarCode;
            form.txtLongM.Text = conv.LongMGC.ToString();
            form.txtLargM.Text = conv.LargMGC.ToString();
            form.txtHautM.Text = conv.HautMGC.ToString();
            form.txtVolM.Text = conv.VolMGC.ToString();
            form.txtPoidsM.Text = conv.PoidsMGC.ToString();
            form.cbTypeConvM.SelectedItem = conv.TYPE_CONVENTIONNEL.LibTypeGC;

            form.txtLongC.Text = conv.LongCGC.ToString();
            form.txtLargC.Text = conv.LargCGC.ToString();
            form.txtHautC.Text = conv.HautCGC.ToString();
            form.txtVolC.Text = conv.VolCGC.ToString();
            form.txtPoidsC.Text = conv.PoidsCGC.ToString();
            form.cbTypeConvC.SelectedItem = conv.TYPE_CONVENTIONNEL1.LibTypeGC;

            form.txtNumSydonia.Text = conv.ESCALE.NumManifestSydonia;

            if (conv.CONNAISSEMENT.DVBL.HasValue)
            {
                form.groupInfosManif.IsEnabled = false;
                form.groupInfosCub.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.groupInfosManif.IsEnabled = true;
                form.groupInfosCub.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            form.numAttDed.Content = conv.NumADDGC;
            form.numCIVIO.Content = conv.NumCIVIOGC;
            form.numDeclDouane.Content = conv.NumDDGC;
            form.numQuittDouane.Content = conv.NumQDGC;
            form.numFactPAD.Content = conv.NumFPADGC;
            form.numQuittPAD.Content = conv.NumQPADGC;
            form.numBAEPAD.Content = conv.NumAEPADGC;
            form.numBESC.Content = conv.NumBESCGC;
            form.numSydonia.Content = conv.NumSydoniaGC;

            //Notes
            form.listNotes.ItemsSource = conv.NOTE;

            form.eltsFact = vsomAcc.GetElementFacturationConv(conv.IdGC);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteVeh.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteVeh.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.debutSejour.Content = conv.FFGC;
            form.finFranchise.Content = conv.FFGC;
            form.finStationnement.Content = conv.FSGC;

            form.lblStatut.Content = "Statut du conventionnel : " + conv.StatGC;
            form.lblStatutBL.Content = "Statut du BL : " + conv.CONNAISSEMENT.StatutBL;
            form.lblAcconier.Content = "Acconier : " + conv.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + conv.ESCALE.NumEsc + " du " + (conv.ESCALE.DDechEsc.HasValue ? conv.ESCALE.DDechEsc.Value.ToShortDateString() : conv.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + conv.ESCALE.NAVIRE.NomNav;
            form.lblNumAttDouane.Content = conv.NumADDGC != null ? "Numéro d'attestation douane : " + conv.NumADDGC : "Numéro d'attestation douane : Non dédouané";

            form.actionsBorder.Visibility = System.Windows.Visibility.Visible;

            form.groupInfosCub.IsEnabled = true;

            form.Title = "General Cargo - " + conv.NumGC + " - Consignée - " + conv.CONNAISSEMENT.ConsigneeBL;

            // ajouter un blocage pour qu'on ne puisse pas facturer le séjour tant que le véhicule n'a pas été identifié
        }

        public void LoadCubageForm(CubageForm form, CUBAGE cub)
        {
            vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdCub.Text = cub.IdCubage.ToString();

            form.escales = new List<ESCALE>();
            form.escales.Add(cub.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(cub.ESCALE.NumEsc.Value);
            form.cbNumEscale.ItemsSource = null;
            form.cbNumEscale.ItemsSource = form.escs;
            form.cbNumEscale.SelectedItem = cub.ESCALE.NumEsc;
            form.txtEscaleSysID.Text = cub.ESCALE.IdEsc.ToString();
            form.txtAcconier.Text = cub.ESCALE.ACCONIER.CodeAcc;
            form.txtArmateur.Text = cub.ESCALE.ARMATEUR.CodeArm;
            form.txtNavire.Text = cub.ESCALE.NAVIRE.CodeNav;
            form.txtVoyage.Text = cub.ESCALE.NumVoySCR;
            form.txtEnregistreLe.SelectedDate = cub.DateCubage;
            form.txtDateExecution.SelectedDate = cub.DateExCubage;
            form.txtDateCloture.SelectedDate = cub.DateCloCubage;
            form.txtRemarques.Document.Blocks.Clear();
            form.txtRemarques.Document.Blocks.Add(new Paragraph(new Run(cub.AICubage)));

            form.vehicules = vsomAcc.GetVehiculesEnCubage(cub.IdCubage);
            form.dataGridVehicules.ItemsSource = form.vehicules;

            form.nbVehTotal.Content = form.vehicules.Count;
            form.nbVehCubes.Content = cub.CUBAGE_VEHICULE.Count(cb => cb.DateCV.HasValue);
            form.nbVehRestant.Content = form.vehicules.Count - cub.CUBAGE_VEHICULE.Count(cb => cb.DateCV.HasValue);

            form.lblEscale.Content = "Escale : " + cub.ESCALE.NumEsc + " du " + (cub.ESCALE.DRAEsc.HasValue ? cub.ESCALE.DRAEsc.Value.ToShortDateString() : cub.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblEtat.Content = "Statut du cubage sur le terrain : " + cub.CUBAGE_VEHICULE.Count(cb => cb.DateCV.HasValue) + "/" + cub.CUBAGE_VEHICULE.Count;
            form.lblStatut.Content = cub.DateCloCubage.HasValue ? "Cubage clôturé" : "Cubage en cours";

            form.btnCloturerCub.IsEnabled = cub.DateCloCubage.HasValue ? false : true;
            form.btnCloturerCub.Visibility = cub.CUBAGE_VEHICULE.Count(cb => cb.DateVal.HasValue) == cub.CUBAGE_VEHICULE.Count ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.Title = "Cubage - " + form.cbIdCub.Text + " - Escale - " + Convert.ToInt32(cub.ESCALE.NumEsc);
        }

        public void LoadBonEnleverForm(BonEnleverForm form, BON_ENLEVEMENT bae)
        {
            VsomParameters vsomAcc = new VsomParameters();

            form.cbIdBAE.Text = bae.IdBAE.ToString();
            form.cbIdBAE.IsEnabled = true;

            this.LoadConnaissementForm(form, bae, bae.CONNAISSEMENT);

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(bae.AIBAE)));

            if (bae.DVBAE.HasValue)
            {
                form.lblStatut.Content = "BAE validé le : " + bae.DVBAE + " par " + vsomAcc.GetUtilisateursByIdU(bae.IdUV.Value).NU;
                form.btnValiderBAE.Visibility = System.Windows.Visibility.Collapsed;
                form.btnEnregistrer.IsEnabled = false;
                form.btnDetailsGC.IsEnabled = false;
                form.btnDetailsCtr.IsEnabled = false;
                form.btnDetailsVeh.IsEnabled = false;
                form.btnDetailsMafi.IsEnabled = false;
                form.borderEtat.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                form.lblStatut.Content = "BAE crée le : " + bae.DateBAE + " par " + vsomAcc.GetUtilisateursByIdU(bae.IdU.Value).NU;
                form.btnValiderBAE.Visibility = System.Windows.Visibility.Visible;
                form.btnEnregistrer.IsEnabled = true;
                form.btnDetailsGC.IsEnabled = true;
                form.btnDetailsCtr.IsEnabled = true;
                form.btnDetailsVeh.IsEnabled = true;
                form.btnDetailsMafi.IsEnabled = true;
                form.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (bae.CONTENEUR.Count == 0)
            {
                form.btnImprimerBAD.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.btnImprimerBAD.Visibility = System.Windows.Visibility.Visible;
            }

            form.Title = "Bon à enlever - " + bae.IdBAE + " - Consignée - " + bae.CONNAISSEMENT.ConsigneeBL;

        }

        /// <summary>
        /// charge les element de la vue bookingform a partir des données deja enregistrer sur le booking
        /// </summary>
        /// <param name="form">objet bookingform courant</param>
        /// <param name="book">objet du connaissement a charger</param>
        public void LoadBookingForm(BookingForm form, CONNAISSEMENT book)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBooking.IsEnabled = true;
            form.cbNumBooking.Text = book.NumBooking;
            form.txtSysId.Text = book.IdBL.ToString();
            form.txtNumBL.Text = book.NumBL;
            form.cbClient.SelectedItem = book.CLIENT.NomClient;
            form.escales = new List<ESCALE>();
            form.escales.Add(book.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(book.ESCALE.NumEsc.Value);
            form.cbEscale.ItemsSource = null;
            form.cbEscale.ItemsSource = form.escs;
            form.cbEscale.SelectedItem = book.ESCALE.NumEsc;
            form.txtNumVoyage.Text = book.ESCALE.NumVoySCR;
            form.cbNavire.SelectedItem = book.ESCALE.NAVIRE.NomNav;
            form.txtNatMses.Text = book.DescBL;
            form.cbDestination.SelectedItem = book.DPBL;
            form.cbVia.SelectedItem = book.LPBL;
            form.txtShipper.Text = book.ConsigneeBL;
            form.txtAdresseShipper.Text = book.AdresseBL;
            form.txtEmailShipper.Text = book.EmailBL;
            form.txtTelephoneShipper.Text = book.PhoneManBL;
            form.txtConsignee.Text = book.ConsigneeBooking;
            form.txtAdresseConsigee.Text = book.AdresseConsignee;
            form.txtNotify.Text = book.NotifyBL;
            form.txtAdresseNotify.Text = book.AdresseNotify;
            form.txtEmailNotify.Text = book.EmailNotify;
            form.txtTelephoneNotify.Text = book.TelNotify;
            form.txtNotify2.Text = book.NotifyBL2;
            form.txtAdresseNotify2.Text = book.AdresseNotify2;
            form.txtEmailNotify2.Text = book.EmailNotify2;
            form.txtTelephoneNotify2.Text = book.TelNotify2;
            form.txtPayor.Text = book.Payor;
            form.txtClearAgent.Text = book.ClearAgent;
            form.chkSEPBC.IsChecked = book.NoSEPBC == "Y" ? true : false;
            form.chkHinterland.IsChecked = book.BLIL == "Y" ? true : false;
            form.chkGN.IsChecked = book.BLGN == "Y" ? true : false;
            form.chkFretPrepaid.IsChecked = book.CCBL == "Y" ? true : false;
            form.btnValidClearance.Visibility = book.StatutBL == "Clearance" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.eltsBookingGC = vsomAcc.GetConventionnelsOfBooking(book.IdBL);
            form.dataGridGC.ItemsSource = null;
            form.dataGridGC.ItemsSource = form.eltsBookingGC;

            form.eltsBookingCtr = vsomAcc.GetConteneursOfBooking(book.IdBL);
            form.dataGridConteneurs.ItemsSource = null;
            form.dataGridConteneurs.ItemsSource = form.eltsBookingCtr;

            form.eltsDispositionCtr = vsomAcc.GetDispositionCtr(book.IdBL);
            form.dataGridCtrsDispo.ItemsSource = null;
            form.dataGridCtrsDispo.ItemsSource = form.eltsDispositionCtr;

            //if (form.eltsBookingCtr.Count == 0)
            //{
            //    form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            //    form.gcTab.Visibility = System.Windows.Visibility.Visible;
            //    form.eltsBookingGC = vsomAcc.GetConventionnelsOfBooking(book.IdBL);
            //    form.dataGridGC.ItemsSource = null;
            //    form.dataGridGC.ItemsSource = form.eltsBookingGC;
            //    form.gcTab.IsSelected = true;
            //}
            //else
            //{
            //    form.dataGridConteneurs.ItemsSource = null;
            //    form.dataGridConteneurs.ItemsSource = form.eltsBookingCtr;
            //    form.contTab.Visibility = System.Windows.Visibility.Visible;
            //    form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            //    form.contTab.IsSelected = true;
            //}

            form.proformas = vsomAcc.GetProformasOfConnaissement(book.IdBL);
            if (form.proformas.Count == 0)
            {
                form.profTab.Visibility = System.Windows.Visibility.Collapsed;
                form.eltTab.IsSelected = true;
            }
            else
            {
                form.dataGridProfs.ItemsSource = form.proformas;
                form.profTab.Visibility = System.Windows.Visibility.Visible;
            }

            if (!book.DCLBL.HasValue)
            {
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (book.CONTENEUR.Count != 0)
            {
                form.btnGenererCoparn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                form.btnGenererCoparn.Visibility = System.Windows.Visibility.Collapsed;
            }

            form.btnProforma.Visibility = book.DCLBL.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            //Valeur du booking
            form.eltsFact = vsomAcc.GetElementFacturationBooking(book.IdBL);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteBooking.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBooking.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBooking.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            // statut de chargement
            form.statutChargement = vsomAcc.GetStatutChargementBooking(book.IdBL);
            form.nbCont.Content = form.statutChargement.NbConteneur + "/" + book.CONTENEUR.Count;
            form.nbCargos.Content = form.statutChargement.NbConventionnel + "/" + book.CONVENTIONNEL.Count;

            //Notes
            form.listNotes.ItemsSource = book.NOTE;

            form.btnGenererBL.Visibility = book.StatutBL == "Cargo Loaded" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            if (book.CONTENEUR.Count(ctr => ctr.StatCtr != "Cargo Loaded") == book.CONTENEUR.Count && book.CONVENTIONNEL.Count(gc => gc.StatGC != "Cargo Loaded") == book.CONVENTIONNEL.Count)
            {
                form.btnTransfertEscale.IsEnabled = true;
            }
            else
            {
                form.btnTransfertEscale.IsEnabled = false;
            }

            form.borderEtat.Visibility = book.StatutBL == "BL généré" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.btnEnregistrer.IsEnabled = (book.StatutBL == "Booking" || book.StatutBL == "Clearance") ? true : false;
            //form.btnImprimerDraftBL.Visibility = book.DVCBLI.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.btnAnnuler.IsEnabled = book.StatutBL == "Annulé" ? false : true;
            form.btnTransfertEscale.IsEnabled = book.StatutBL == "Annulé" ? false : true;

            //Compte BL
            form.dataGridCompteBooking.ItemsSource = vsomAcc.GetCompteBL(book.IdBL);
            double debit = form.dataGridCompteBooking.Items.OfType<ElementCompte>().Sum(el => el.Debit);
            double credit = form.dataGridCompteBooking.Items.OfType<ElementCompte>().Sum(el => el.Credit);
            form.lblStatutPaiement.Content = "Debit : " + debit + " / Crédit : " + credit + ". Solde du BL : " + (credit - debit).ToString();

            //Chargement de la barre d'état
            form.lblStatut.Content = "Statut du Booking : " + book.StatutBL;
            form.lblAcconier.Content = "Acconier : " + book.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + book.ESCALE.NumEsc + " du " + (book.ESCALE.DRAEsc.HasValue ? book.ESCALE.DRAEsc.Value.ToShortDateString() : book.ESCALE.DPAEsc.Value.ToShortDateString());

            form.Title = "Booking - " + book.NumBooking + " - Consignée - " + book.ConsigneeBooking;

        }

        public void LoadBookingForm(TransfertBookingGCForm form, CONNAISSEMENT book)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.bookings = new List<CONNAISSEMENT>();
            form.bookings.Add(book);
            form.books = new List<string>();
            form.books.Add(book.NumBooking);
            form.cbNumBooking.ItemsSource = null;
            form.cbNumBooking.ItemsSource = form.books;
            form.cbNumBooking.SelectedItem = book.NumBooking;
            form.txtIdBL.Text = book.IdBL.ToString();
        }

        public void LoadBookingForm(TransfertBookingCtrForm form, CONNAISSEMENT book)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.bookings = new List<CONNAISSEMENT>();
            form.bookings.Add(book);
            form.books = new List<string>();
            form.books.Add(book.NumBooking);
            form.cbNumBooking.ItemsSource = null;
            form.cbNumBooking.ItemsSource = form.books;
            form.cbNumBooking.SelectedItem = book.NumBooking;
            form.txtIdBL.Text = book.IdBL.ToString();
        }

        public void LoadOrdreServiceForm(OrdreServiceForm form, ORDRE_SERVICE os)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdOS.Text = os.IdOS.ToString();
            form.txtDateCreation.SelectedDate = os.DCrOS;
            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(os.ObsOS)));
            if (os.DClOS.HasValue)
            {
                form.txtDateCloture.SelectedDate = os.DClOS;
            }
            form.txtDateExecPrevue.SelectedDate = os.DPExOS;
            if (os.DRExOS.HasValue)
            {
                form.txtDateExecReelle.SelectedDate = os.DRExOS;
            }
            form.txtObjetOS.Text = os.LibOS;
            form.cbFsseur.SelectedItem = os.FOURNISSEUR.NomFsseur;
            //form.escales = new List<ESCALE>();
            //form.escales.Add(os.ESCALE);
            //form.escs = new List<int>();
            //form.escs.Add(os.ESCALE.NumEsc.Value);
            //form.cbEscale.SelectedItem = os.ESCALE.NumEsc;
            this.LoadEscaleForm(form, os.ESCALE);
            if (os.IdBL.HasValue)
            {
                //form.connaissements = new List<CONNAISSEMENT>();
                //form.connaissements.Add(os.CONNAISSEMENT);
                //form.cons = new List<string>();
                //form.cons.Add(os.CONNAISSEMENT.NumBL);
                //form.cbNumBL.SelectedItem = os.CONNAISSEMENT.NumBL;
                this.LoadConnaissementForm(form, os.CONNAISSEMENT);
            }
            if (os.IdVeh.HasValue)
            {
                form.chkChassis.IsChecked = true;
                form.vehicules = new List<VEHICULE>();
                form.vehicules.Add(os.VEHICULE);
                form.vehs = new List<string>();
                form.vehs.Add(os.VEHICULE.NumChassis);
                form.cbMarch.ItemsSource = null;
                form.cbMarch.ItemsSource = form.vehs;
                form.cbMarch.SelectedItem = os.VEHICULE.NumChassis;
            }
            if (os.IdCtr.HasValue)
            {
                form.chkConteneur.IsChecked = true;
                form.conteneurs = new List<CONTENEUR>();
                form.conteneurs.Add(os.CONTENEUR);
                form.ctrs = new List<string>();
                form.ctrs.Add(os.CONTENEUR.NumCtr);
                form.cbMarch.ItemsSource = null;
                form.cbMarch.ItemsSource = form.ctrs;
                form.cbMarch.SelectedItem = os.CONTENEUR.NumCtr;
            }
            if (os.IdMafi.HasValue)
            {
                form.chkMafi.IsChecked = true;
                form.mafis = new List<MAFI>();
                form.mafis.Add(os.MAFI);
                form.mfs = new List<string>();
                form.mfs.Add(os.MAFI.NumMafi);
                form.cbMarch.ItemsSource = null;
                form.cbMarch.ItemsSource = form.mfs;
                form.cbMarch.SelectedItem = os.MAFI.NumMafi;
            }
            if (os.IdGC.HasValue)
            {
                form.chkGC.IsChecked = true;
                form.conventionnels = new List<CONVENTIONNEL>();
                form.conventionnels.Add(os.CONVENTIONNEL);
                form.gcs = new List<string>();
                form.gcs.Add(os.CONVENTIONNEL.NumGC);
                form.cbMarch.ItemsSource = null;
                form.cbMarch.ItemsSource = form.gcs;
                form.cbMarch.SelectedItem = os.CONVENTIONNEL.NumGC;
            }
            form.eltsLigneOS = vsomAcc.GetLignesOS(os.IdOS);
            form.dataGridEltOS.ItemsSource = form.eltsLigneOS;
            form.btnEnregistrer.IsEnabled = os.DVOS.HasValue ? false : true; // A revoir
            form.btnValider.Visibility = os.DVOS.HasValue ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            if (os.DVOS.HasValue)
            {
                if (os.DClOS.HasValue)
                {
                    form.btnCloturer.IsEnabled = false;
                    form.btnCloturer.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    form.btnCloturer.IsEnabled = true;
                    form.btnCloturer.Visibility = System.Windows.Visibility.Visible;
                }
                form.cbNumBL.IsEnabled = false;
                form.cbEscale.IsEnabled = false;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.btnCloturer.Visibility = System.Windows.Visibility.Collapsed;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
                form.cbNumBL.IsEnabled = true;
                form.cbEscale.IsEnabled = true;
            }
            if (os.StatutOS == "Annulé")
            {
                form.btnEnregistrer.IsEnabled = false;
                form.btnValider.IsEnabled = false;
                form.btnAnnuler.IsEnabled = false;
            }
            form.groupDetailLignes.IsEnabled = os.DVOS.HasValue ? false : true;

            form.lblStatut.Content = "Statut de l'OS : " + os.StatutOS;
            form.lblAcconier.Content = "Acconier : " + os.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + os.ESCALE.NumEsc + " du " + (os.ESCALE.DRAEsc.HasValue ? os.ESCALE.DRAEsc.Value.ToShortDateString() : os.ESCALE.DPAEsc.Value.ToShortDateString());

            form.Title = "Ordre de service N° " + os.IdOS + " - " + os.ESCALE.NumEsc;
        }

        public void LoadConnaissementForm(ProformaForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;
            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtClientAFacturer.Text = con.CLIENT.NomClient;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(con.ESCALE.NumEsc.Value);
            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            //if (form.cbIdProf.IsEnabled == false)
            //{
            //    // il s'agit d'une nouvelle proforma
            //    // Sélection des éléments de factures ne faisant pas l'objet d'une facture
            //    form.eltsFact = vsomAcc.GetElementNonFactureBL(con.IdBL);
            //    form.dataGridEltsFact.ItemsSource = form.eltsFact;
            //}
            //else
            //{
            //    form.eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
            //    form.dataGridEltsFact.ItemsSource = form.eltsFact;
            //}

            form.eltsFact = vsomAcc.GetElementNonFactureBL(con.IdBL);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DDechEsc.HasValue ? con.ESCALE.DDechEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(FactureDITForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;
            form.txtConsignee.Text = con.ConsigneeBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (form.cbIdFact.IsEnabled == false)
            {
                // Sélection des éléments de factures ne faisant pas l'objet d'une facture
                form.eltsFact = vsomAcc.GetElementDITByIdBLNonFacture(con.IdBL);
                form.dataGridEltsFact.ItemsSource = form.eltsFact;
            }
            else
            {
                form.eltsFact = vsomAcc.GetElementDITByIdBL(con.IdBL);
                form.dataGridEltsFact.ItemsSource = form.eltsFact;
            }

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            form.dataGridConteneurs.ItemsSource = form.conteneurs;

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + con.ESCALE.DPAEsc.Value.ToShortDateString();
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadFactureSpotForm(FactureSpot form, FACTURE _facture)
        {
             vsomAcc = new VSOMAccessors(); 
            //VsomParameters vsp = new VsomParameters();

            form.eltsLigneOS = vsomAcc.GetFactureSpotElement(_facture.IdFD);
            form.txtIdFact.Text = _facture.IdDocSAP.ToString();

            form.dataGridEltOS.ItemsSource = null; form.dataGridEltOS.ItemsSource = form.eltsLigneOS;
            form.btnEnregistrer.IsEnabled = false; form.lblStatut.Content = "Facture émise le " + _facture.DCFD.Value;
            form.borderActions.Visibility = System.Windows.Visibility.Visible;
            if (_facture.StatutFD == "A") form.btnAnnuler.IsEnabled = false;

            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(_facture.AIFD)));
            form.cbClient.SelectedItem = _facture.CLIENT;

            form.montantHTCpteFact.Content = _facture.MHT.ToString();
            form.montantTVACpteFact.Content = _facture.MTVA.ToString();
            form.montantTTCCpteFact.Content = _facture.MTTC.ToString();


            form.btnAjoutLS.IsEnabled = form.btnModifierLS.IsEnabled = form.btnSupprimerLS.IsEnabled = false;
            if (_facture.StatutFD == "A")
            { form.statutCpteFact.Content = "Annulée"; }
            if (_facture.StatutFD == "O")
            { form.statutCpteFact.Content = "En Cours"; }
            if (_facture.StatutFD == "P")
            { form.statutCpteFact.Content = "Payée"; }
            //form.statutCpteFact.Content = "En Cours";
        }

        public void LoadFactureForm(FactureForm form, FACTURE fact)
        {
            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(fact.PROFORMA.CONNAISSEMENT);
            form.escales = new List<ESCALE>();
            form.escales.Add(fact.PROFORMA.CONNAISSEMENT.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(form.escales.FirstOrDefault<ESCALE>().NumEsc.Value);
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbIdFact.Text = fact.IdFD.ToString();
            form.txtIdFact.Text = fact.IdDocSAP.ToString();
            form.txtDateCreation.SelectedDate = fact.DCFD;
            form.txtDateComptable.SelectedDate = fact.DateComptable;
            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(fact.AIFD)));
            form.cbNumEsc.SelectedIndex = 0;

            this.LoadProformaForm(form, fact.PROFORMA);
            form.btnEnregistrer.IsEnabled = false;

            form.btnPaiement.Visibility = fact.IdPay.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            form.lblStatut.Content = "Facture émise le " + fact.DCFD.Value;

            //AH 10/juillet/16 affichage statut facture
            string statut=string.Empty;
            if(fact.StatutFD=="O") statut="En cours";
            if(fact.StatutFD=="A") statut="Annulée";
            if(fact.StatutFD=="P") statut="Payée";
            form.statutCpteFact.Content = statut;

            form.Title = "Facture - " + fact.IdFD + " - Consignée - " + fact.PROFORMA.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadConnaissementForm(FactureForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;
            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            List<ElementFacturation> eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
            form.montantHTCpteBL.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = eltsFact.Sum(elt => elt.MontantTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DDechEsc.HasValue ? con.ESCALE.DDechEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;

        }

        public void LoadProformaForm(ProformaForm form, PROFORMA prof)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdProf.Text = prof.IdFP.ToString();
            form.txtDateCreation.SelectedDate = prof.DCFP;

            this.LoadConnaissementForm(form, prof.CONNAISSEMENT);
            form.cbNumBL.IsEnabled = false;
            form.cbClient.SelectedItem = prof.CLIENT.NomClient;
            form.txtClientAFacturer.Text = prof.ClientFacture;

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(prof.AIFP)));

            if (form.cbFiltres.SelectedIndex != 2)
            {
                form.cbFiltres.SelectedIndex = 2;
            }
            else
            {
                form.eltsFact = vsomAcc.GetLignesProf(prof.IdFP);
                form.dataGridEltsFact.ItemsSource = form.eltsFact;
            }

            List<ElementFacturation> eltsFact = vsomAcc.GetElementFacturationProf(prof.IdFP);
            form.montantHTCpteProf.Content = eltsFact.Sum(elt => elt.PrixUnitaire * Math.Abs(elt.Qte));
            form.montantTVACpteProf.Content = eltsFact.Sum(elt => Math.Abs(elt.MontantTVA));
            form.montantTTCCpteProf.Content = eltsFact.Sum(elt => Math.Abs(elt.MontantTTC));

            //form.montantHTCpteProf.Content = prof.MHT;
            //form.montantTVACpteProf.Content = prof.MTVA;
            //form.montantTTCCpteProf.Content = prof.MTTC;

            //form.btnValider.Visibility = prof.DVFP.HasValue ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            form.btnValider.Visibility = prof.CLIENT.CodeClient == "C0200" ? System.Windows.Visibility.Collapsed : (!prof.DVFP.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);

            if (prof.StatutFP == "A")
            {
                form.borderActions.Visibility = System.Windows.Visibility.Visible;
                form.btnValider.Visibility = System.Windows.Visibility.Collapsed;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
                form.btnAnnuler.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
                form.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
                form.eltBorder.IsEnabled = false;
            }
            else
            {
                form.btnEnregistrer.IsEnabled = true;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
                form.btnAnnuler.IsEnabled = true;
                form.borderEtat.Visibility = System.Windows.Visibility.Visible;
                form.eltBorder.IsEnabled = true;
            }

            if (prof.DVFP.HasValue)
            {
                form.btnEnregistrer.IsEnabled = false;
                form.borderEtat.Visibility = System.Windows.Visibility.Visible;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
                form.eltBorder.IsEnabled = false;
                form.btnValider.Visibility = System.Windows.Visibility.Collapsed;
                form.cbFiltres.IsEnabled = false;
            }

            if (prof.CONNAISSEMENT.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.btnFacture.Visibility = prof.DVFP.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

            List<ElementFacturation> eltsBL = vsomAcc.GetElementFacturationBL(prof.IdBL.Value);
            form.montantHTCpteBL.Content = eltsBL.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = eltsBL.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = eltsBL.Sum(elt => elt.MontantTTC);

            form.lblStatut.Content = prof.DVFP.HasValue ? "Proforma validée le " + prof.DVFP.Value : (prof.StatutFP == "A" ? "Proforma annulée" : "Proforma en attente de validation");

            form.Title = "Proforma - " + prof.IdFP + " - Consignée - " + prof.CONNAISSEMENT.ConsigneeBL;

        }

        public void LoadFactureDITForm(FactureDITForm form, FACTURE_DIT fact)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdFact.Text = fact.IdFactDIT.ToString();
            form.txtDateFacture.SelectedDate = fact.DateFact;

            form.txtNumFactDIT.Text = fact.NumFactDIT;

            this.LoadConnaissementForm(form, fact.CONNAISSEMENT);
            form.cbNumBL.IsEnabled = false;

            form.eltsFact = vsomAcc.GetLignesFactDIT(fact.IdFactDIT);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;

            form.montantHTCpteFact.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteFact.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteFact.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.btnEnregistrer.IsEnabled = false;

            if (fact.CONNAISSEMENT.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (fact.CONNAISSEMENT.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (fact.CONNAISSEMENT.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (fact.CONNAISSEMENT.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.lblStatut.Content = "Facture DIT enregistrée le " + fact.DateTrans;

            form.Title = "Facture DIT - " + fact.NumFactDIT + " - Consignée - " + fact.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadAvoirForm(Finance.AvoirCustom form, AVOIR avoir)
        {
            vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
             

            form.cbTypeAvoir.Text = avoir.TypeAvoir;

            form.cbIdAvoir.Text = avoir.IdFA.ToString();
            form.txtIdAvoir.Text = avoir.IdDocSAP.ToString();
            form.txtDateCreation.SelectedDate = avoir.DCFA;
            form.cbTypeAvoir.Text= avoir.FACTURE==null ? "Spot" : "Facture Spot";

            form.txtNumFact.Text = avoir.FACTURE==null ? "N/D" : avoir.FACTURE.IdDocSAP.Value.ToString();
             
            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(avoir.AIFA)));

            if (avoir.FACTURE == null)
                form.eltsFact = vsomAcc.GetLignesAvoirSpot(avoir.IdFA);
            else
                form.eltsFact = vsomAcc.GetLignesAvoirFSpot(avoir.IdFA);

            form.dataGridEltsFact.ItemsSource = form.eltsFact;

            form.montantHTCpteAvoir.Content = avoir.MHT;
            form.montantTVACpteAvoir.Content = avoir.MTVA;
            form.montantTTCCpteAvoir.Content = avoir.MTTC;

            form.cbClient.ItemsSource = form.clts;
            form.cbClient.Text = avoir.CLIENT.NomClient;
            form.txtCodeClient.Text = avoir.CLIENT.CodeClient;

            //List<ElementFacturation> eltsFactProf = vsomAcc.GetLignesProf(avoir.FACTURE.PROFORMA.IdFP);
            //form.montantHTCpteFact.Content = eltsFactProf.Sum(elt => elt.PrixUnitaire * elt.Qte);
            //form.montantTVACpteFact.Content = eltsFactProf.Sum(elt => elt.MontantTVA);
            //form.montantTTCCpteFact.Content = eltsFactProf.Sum(elt => elt.MontantTTC);
 
            form.borderEtat.Visibility = System.Windows.Visibility.Visible;

            form.lblStatut.Content = "Avoir crée le " + avoir.DCFA.Value;

            form.Title = "Avoir - " + avoir.IdFA;
        }

        public void LoadAvoirForm(AvoirForm form, AVOIR avoir)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(avoir.CONNAISSEMENT.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(avoir.CONNAISSEMENT.ESCALE.NumEsc.Value);
            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = avoir.CONNAISSEMENT.ESCALE;

            form.cbTypeAvoir.Text = avoir.TypeAvoir;

            form.cbIdAvoir.Text = avoir.IdFA.ToString();
            form.txtIdAvoir.Text = avoir.IdDocSAP.ToString();
            form.txtDateCreation.SelectedDate = avoir.DCFA;
            form.cbNumFact.Text = avoir.FACTURE.IdDocSAP.ToString();

            this.LoadFactureForm(form, avoir.FACTURE, 1);

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(avoir.AIFA)));

            form.eltsFact = vsomAcc.GetLignesAvoir(avoir.IdFA);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;

            form.montantHTCpteAvoir.Content = avoir.MHT;
            form.montantTVACpteAvoir.Content = avoir.MTVA;
            form.montantTTCCpteAvoir.Content = avoir.MTTC;

            List<ElementFacturation> eltsFactProf = vsomAcc.GetLignesProf(avoir.FACTURE.PROFORMA.IdFP);
            form.montantHTCpteFact.Content = eltsFactProf.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteFact.Content = eltsFactProf.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteFact.Content = eltsFactProf.Sum(elt => elt.MontantTTC);

            form.borderActions.Visibility = System.Windows.Visibility.Visible;
            form.btnEnregistrer.IsEnabled = false;
            form.borderEtat.Visibility = System.Windows.Visibility.Visible;

            form.lblStatut.Content = "Avoir crée le " + avoir.DCFA.Value;

            form.Title = "Avoir - " + avoir.IdFA + " - Consignée - " + avoir.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadFactureForm(AvoirForm form, FACTURE fact, int typeForm)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.txtNumBL.Text = fact.PROFORMA.CONNAISSEMENT.NumBL;

            form.cbClient.SelectedItem = fact.PROFORMA.CLIENT.NomClient;
            form.txtConsignee.Text = fact.PROFORMA.CONNAISSEMENT.ConsigneeBL;
            form.txtClientAvoir.Text = fact.PROFORMA.ClientFacture;
            form.cbNumEsc.SelectedItem = fact.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc;

            form.conventionnels = fact.PROFORMA.CONNAISSEMENT.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = fact.PROFORMA.CONNAISSEMENT.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = fact.PROFORMA.CONNAISSEMENT.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            if (typeForm == 0)
            {
                form.dataGridEltsFact.ItemsSource = vsomAcc.GetLignesProfPourAvoir(fact.IdFP.Value);

                List<ElementFacturation> eltsFact = vsomAcc.GetLignesProf(fact.PROFORMA.IdFP);
                form.montantHTCpteFact.Content = eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
                form.montantTVACpteFact.Content = eltsFact.Sum(elt => elt.MontantTVA);
                form.montantTTCCpteFact.Content = eltsFact.Sum(elt => elt.MontantTTC);
            }

            form.lblAcconier.Content = "Acconier : " + fact.PROFORMA.CONNAISSEMENT.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + fact.PROFORMA.CONNAISSEMENT.ESCALE.NumEsc + " du " + (fact.PROFORMA.CONNAISSEMENT.ESCALE.DDechEsc.HasValue ? fact.PROFORMA.CONNAISSEMENT.ESCALE.DDechEsc.Value.ToShortDateString() : fact.PROFORMA.CONNAISSEMENT.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + fact.PROFORMA.CONNAISSEMENT.ESCALE.NAVIRE.NomNav;
        }

        private void LoadProformaForm(FactureForm form, PROFORMA prof)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdProf.Text = prof.IdFP.ToString();
            this.LoadConnaissementForm(form, prof.CONNAISSEMENT);
            form.txtClientAFacturer.Text = prof.ClientFacture;
            form.eltsFact = vsomAcc.GetLignesProf(prof.IdFP);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;

            List<ElementFacturation> eltsFact = vsomAcc.GetElementFacturationProf(prof.IdFP);
            form.montantHTCpteFact.Content = eltsFact.Sum(elt => elt.PrixUnitaire * Math.Abs(elt.Qte));
            form.montantTVACpteFact.Content = eltsFact.Sum(elt => Math.Abs(elt.MontantTVA));
            form.montantTTCCpteFact.Content = eltsFact.Sum(elt => Math.Abs(elt.MontantTTC));

            //form.montantHTCpteFact.Content = prof.MHT;
            //form.montantTVACpteFact.Content = prof.MTVA;
            //form.montantTTCCpteFact.Content = prof.MTTC;

            if (prof.CONNAISSEMENT.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (prof.CONNAISSEMENT.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }
        }

        public void LoadDemandeVisiteForm(DemandeVisiteForm form, DEMANDE_VISITE visite)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.demandesVisite = new List<DEMANDE_VISITE>();
            form.demandesVisite.Add(visite);
            form.visites = new List<int>();
            form.visites.Add(visite.IdDV);
            form.cbIdDV.ItemsSource = null;
            form.cbIdDV.ItemsSource = form.visites;
            form.cbIdDV.SelectedItem = visite.IdDV;

            this.LoadConnaissementForm(form, visite.CONNAISSEMENT, 1);

            form.cbTypeVisite.SelectedItem = visite.TYPE_VISITE.LibTypeVisite;

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(visite.AIDV)));

            form.factures = vsomAcc.GetFacturesOfConnaissement(visite.IdBL.Value);
            form.dataGridFacts.ItemsSource = form.factures;

            if (visite.DVDV.HasValue)
            {
                form.lblStatut.Content = "Demande de visite validé le : " + visite.DVDV + " par " + vsomAcc.GetUtilisateursByIdU(visite.IdUV.Value).NU;
                form.btnValiderDV.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.lblStatut.Content = "Demande de visite créée le : " + visite.DateDV + " par " + vsomAcc.GetUtilisateursByIdU(visite.IdU.Value).NU;
                form.btnValiderDV.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            form.borderActions.Visibility = System.Windows.Visibility.Visible;
            form.Title = "Demande de visite - " + visite.IdDV + " - Consignée - " + visite.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadDemandeCautionForm(DemandeRestitutionCautionForm form, DEMANDE_CAUTION caution)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.demandesCaution = new List<DEMANDE_CAUTION>();
            form.demandesCaution.Add(caution);
            form.cautions = new List<int>();
            form.cautions.Add(caution.IdDRC);
            form.cbIdDRC.ItemsSource = null;
            form.cbIdDRC.ItemsSource = form.cautions;
            form.cbIdDRC.SelectedItem = caution.IdDRC;

            this.LoadConnaissementForm(form, caution.CONNAISSEMENT, 1);

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(caution.AIVDRC)));

            form.factures = vsomAcc.GetFacturesOfConnaissement(caution.IdBL.Value);
            form.dataGridFacts.ItemsSource = form.factures;

            if (caution.DVDRC.HasValue)
            {
                form.lblStatut.Content = "Demande de restitution de caution validée le : " + caution.DVDRC + " par " + vsomAcc.GetUtilisateursByIdU(caution.IdUV.Value).NU;
                form.btnValider.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
            }
            else
            {
                form.lblStatut.Content = "Demande de restitution de caution créée le : " + caution.DateDRC + " par " + vsomAcc.GetUtilisateursByIdU(caution.IdU.Value).NU;
                form.btnValider.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
            }

            form.borderActions.Visibility = System.Windows.Visibility.Visible;
            form.Title = "Demande de restitution de caution - " + caution.IdDRC + " - Consignée - " + caution.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadConnaissementForm(PaiementForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(form.escales.FirstOrDefault<ESCALE>().NumEsc.Value);
            form.cbNumEsc.ItemsSource = form.escs;

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;
            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (form.btnEnregistrer.IsEnabled)
            {
                form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
                if (form.conventionnels.Count == 0)
                {
                    form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    form.dataGridGC.ItemsSource = form.conventionnels;
                    form.gcTab.Visibility = System.Windows.Visibility.Visible;
                    form.gcTab.IsSelected = true;
                }

                form.conteneurs = con.CONTENEUR.Where(ctr => !ctr.IdPay.HasValue).ToList<CONTENEUR>();
                if (con.CONTENEUR.Count == 0)
                {
                    form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    form.dataGridConteneurs.ItemsSource = con.CONTENEUR.ToList();
                    form.contTab.Visibility = System.Windows.Visibility.Visible;
                    form.contTab.IsSelected = true;
                }

                //form.conteneurs = con.CONTENEUR.Where(ctr => !ctr.IdPay.HasValue).ToList<CONTENEUR>();
                if (con.CONTENEUR.Count(ctr => !ctr.IdPay.HasValue) == 0)
                {
                    form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    form.dataGridCautions.ItemsSource = vsomAcc.GetConteneursImportCautionImpayee(con.IdBL);
                    form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                }

                if (con.CONTENEUR.Count(ctr => ctr.IdPay.HasValue) == 0)
                {
                    form.restCautionTab.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    form.dataGridRestCautions.ItemsSource = vsomAcc.GetCautionNonRestitueeByIdBL(con.IdBL);
                    form.restCautionTab.Visibility = System.Windows.Visibility.Visible;
                }

                form.vehicules = con.VEHICULE.ToList<VEHICULE>();
                if (form.vehicules.Count == 0)
                {
                    form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    form.dataGridVehicules.ItemsSource = form.vehicules;
                    form.vehTab.Visibility = System.Windows.Visibility.Visible;
                    form.vehTab.IsSelected = true;
                }

                if (con.CLIENT.CodeClient == "C0200")
                {
                    form.proformas = vsomAcc.GetProformasOfConnaissementEnAttente(con.IdBL);
                    form.dataGridProfs.ItemsSource = form.proformas;
                    form.profTab.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    form.profTab.Visibility = System.Windows.Visibility.Collapsed;
                    form.factTab.IsSelected = true;
                }

                form.factures = vsomAcc.GetFacturesNonPayeesOfConnaissement(con.IdBL);
                form.dataGridFacts.ItemsSource = form.factures;

                if (utilisateur.Caisse == "5711101")
                {
                    form.cbCompteCaisse.SelectedIndex = 0;
                }
                else if (utilisateur.Caisse == "5711102")
                {
                    form.cbCompteCaisse.SelectedIndex = 1;
                }
                else if (utilisateur.Caisse == "5711103")
                {
                    form.cbCompteCaisse.SelectedIndex = 2;
                }
                else if (utilisateur.Caisse == "5711104")
                {
                    form.cbCompteCaisse.SelectedIndex = 3;
                }
                //if (utilisateur.LU == "Admin")
                //{
                //    form.cbCompteCaisse.IsEnabled = true;
                //}
                //else
                //{
                //    form.cbCompteCaisse.IsEnabled = false;
                //}
                
            }

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DDechEsc.HasValue ? con.ESCALE.DDechEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadPaiementForm(PaiementForm form, PAYEMENT pay)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbIdPay.Text = pay.IdPay.ToString();
            form.txtIdPaySAP.Text = pay.IdPaySAP.ToString();

            form.escales = new List<ESCALE>();
            form.escales.Add(pay.CONNAISSEMENT.ESCALE);
            form.escs = new List<int>();
            form.escs.Add(pay.CONNAISSEMENT.ESCALE.NumEsc.Value);
            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = pay.CONNAISSEMENT.ESCALE.NumEsc;

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(pay.CONNAISSEMENT);
            form.cons = new List<string>();
            form.cons.Add(form.connaissements.FirstOrDefault<CONNAISSEMENT>().NumBL);
            form.cbNumBL.ItemsSource = form.cons;

            this.LoadConnaissementForm(form, pay.CONNAISSEMENT);
            if (pay.ModePay.Value == 1) form.cbModePay.SelectedIndex = 0;
            if (pay.ModePay.Value == 2) form.cbModePay.SelectedIndex = 1;
            if (pay.ModePay.Value == 3) form.cbModePay.SelectedIndex = 3;
            if (pay.ModePay.Value == 4) form.cbModePay.SelectedIndex = 2;

            //form.cbModePay.SelectedIndex = pay.ModePay.Value - 1;
            form.txtComptePay.Text = pay.CCPay;
            form.txtMAPay.Text = pay.MAPay.ToString();
            form.txtMRPay.Text = pay.MRPay.ToString();
            form.txtMRendrePay.Text = (Convert.ToInt32(form.txtMRPay.Text) - Convert.ToInt32(form.txtMAPay.Text)).ToString();

            form.txtNumCheque.Text = pay.NumCheque;
            form.txtNumCompte.Text = pay.NumCompte;
            form.txtAgence.Text = pay.Agence;
            form.txtBanque.Text = pay.Banque;

            form.txtRefVirt.Text = pay.RefVirement;
            if (pay.IdBanque != null && pay.IdBanque != 0)
            {
                form.cbBanque.SelectedItem = pay.BANQUE1.CodeBanque;
            }

            form.chkRetenueIS.IsChecked = pay.RetIS == "Y" ? true : false;

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(pay.AIPay)));

            form.dataGridFacts.ItemsSource = vsomAcc.GetFacturesOfConnaissementPayees(pay.CONNAISSEMENT.IdBL, pay.IdPay);
            form.dataGridCautions.ItemsSource = vsomAcc.GetConteneursImportCautionPayee(pay.CONNAISSEMENT.IdBL, pay.IdPay);
            form.dataGridRestCautions.ItemsSource = vsomAcc.GetRestitutionCautionOfConnaissement(pay.CONNAISSEMENT.IdBL);

            form.profTab.Visibility = System.Windows.Visibility.Collapsed;

            if (form.dataGridRestCautions.Items.Count != 0)
            {
                form.restCautionTab.Visibility = System.Windows.Visibility.Visible;
                form.restCautionTab.IsSelected = true;
            }
            else
            {
                form.restCautionTab.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (form.dataGridCautions.Items.Count != 0)
            {
                form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                form.cautionsTab.IsSelected = true;
            }
            else
            {
                form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (form.dataGridFacts.Items.Count != 0)
            {
                form.factTab.Visibility = System.Windows.Visibility.Visible;
                form.factTab.IsSelected = true;
            }
            else
            {
                form.factTab.Visibility = System.Windows.Visibility.Collapsed;
            }

            form.btnAnnuler.Visibility = pay.StatutPay == "A" ? System.Windows.Visibility.Collapsed : (pay.FACTURE.Count != 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);

            form.lblStatut.Content = "Crée le " + pay.DatePay + " par " + pay.UTILISATEUR.NU + " - Statut : " + (pay.StatutPay == "O" ? "Valide" : "Annulé");

            form.Title = "Paiement - " + pay.IdPay + " - Consignée - " + pay.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadExtensionFranchiseForm(ExtensionFranchiseForm form, EXTENSION_FRANCHISE ext)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsomAcc = new VsomParameters();

            form.cbIdExt.Text = ext.IdDEXT.ToString();

            this.LoadConnaissementForm(form, ext.CONNAISSEMENT);

            form.txtNbSej.Text = ext.NbreSej.ToString();
            //form.txtNbStat.Text = ext.NbreStat.ToString();
            form.txtNbSures.Text = ext.NbreSures.ToString();
            form.txtNbDet.Text = ext.NbreDet.ToString();

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(ext.ObsDEXT)));

            if (ext.DatevDEXT.HasValue)
            {
                form.lblStatut.Content = "Demande d'extension de franchise validée le : " + ext.DatevDEXT + " par " + vsomAcc.GetUtilisateursByIdU(ext.IdUV.Value).NU;
                form.btnValider.IsEnabled = false;
                form.btnEnregistrer.IsEnabled = false;
                form.groupExtention.IsEnabled = false;
            }
            else
            {
                form.lblStatut.Content = "Demande d'extension de franchise créée le : " + ext.DateDEXT + " par " + vsomAcc.GetUtilisateursByIdU(ext.IdU.Value).NU;
                form.btnValider.IsEnabled = true;
                form.btnEnregistrer.IsEnabled = true;
                form.groupExtention.IsEnabled = true;
            }

            form.Title = "Demande d'extension de séjour - " + ext.IdDEXT + " - Consignée - " + ext.CONNAISSEMENT.ConsigneeBL;

        }

        public void LoadDemandeReductionForm(DemandeReductionForm form, DEMANDE_REDUCTION reduc)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsomAcc = new VsomParameters();

            form.cbIdRed.Text = reduc.IdDDR.ToString();
            form.cbIdRed.IsEnabled = true;

            this.LoadConnaissementForm(form, reduc.CONNAISSEMENT);

            form.cbArticle.SelectedItem = reduc.ARTICLE.LibArticle;
            form.txtPourcent.Text = reduc.Pourcent.Value.ToString();
            form.txtMHT.Text = reduc.MHT.ToString();
            form.txtLibelle.Text = reduc.LibDDR;

            form.txtMTVA.Text = (Math.Round((Convert.ToDouble(form.txtMHT.Text) * reduc.TauxTVA.Value) / 100, 0, MidpointRounding.AwayFromZero)).ToString();
            form.txtMTTC.Text = (Convert.ToDouble(form.txtMHT.Text) + Convert.ToDouble(form.txtMTVA.Text)).ToString();

            form.txtMTTCAvant.Text = ((Convert.ToDouble(form.txtMTTC.Text) * 100) / reduc.Pourcent.Value).ToString();

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(reduc.ObsDDR)));

            form.btnEnregistrer.IsEnabled = reduc.DatevDDR.HasValue ? false : true;
            if (reduc.DatevDDR.HasValue)
            {
                form.btnValider.IsEnabled = false;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.btnValider.IsEnabled = true;
                form.btnAnnuler.Visibility = System.Windows.Visibility.Visible;
            }
            if (reduc.StatutRed == "Annulé")
            {
                form.btnEnregistrer.IsEnabled = false;
                form.btnValider.IsEnabled = false;
                form.btnAnnuler.IsEnabled = false;
            }

            if (reduc.DatevDDR.HasValue)
            {
                form.lblStatut.Content = "Demande de réduction validée le : " + reduc.DatevDDR + " par " + vsomAcc.GetUtilisateursByIdU(reduc.IdUV.Value).NU;
            }
            else
            {
                if (reduc.StatutRed != "Annulé")
                {
                    form.lblStatut.Content = "Demande de réduction créée le : " + reduc.DateDDR + " par " + vsomAcc.GetUtilisateursByIdU(reduc.IdU.Value).NU;
                }
                else
                {
                    form.lblStatut.Content = "Statut : " + reduc.StatutRed;
                }
            }

            //Notes
            form.listNotes.ItemsSource = reduc.NOTE;

            form.Title = "Demande de réduction - " + reduc.IdDDR + " - Consignée - " + reduc.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadConnaissementForm(DemandeVisiteForm form, CONNAISSEMENT con, int type)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            if (type == 0)
            {
                form.vehicules = con.VEHICULE.ToList<VEHICULE>();
                form.dataGridVehicules.ItemsSource = form.vehicules;
            }
            else
            {
                form.vehicules = vsomAcc.GetVehiculesOfVisite(Convert.ToInt32(form.cbIdDV.Text));
                form.dataGridVehicules.ItemsSource = form.vehicules;
            }

            form.factures = vsomAcc.GetFacturesOfConnaissement(con.IdBL);
            form.dataGridFacts.ItemsSource = form.factures;

            form.montantHTCpteBL.Content = form.factures.Sum(fact => fact.MHT);
            form.montantTVACpteBL.Content = form.factures.Sum(fact => fact.MTVA);
            form.montantTTCCpteBL.Content = form.factures.Sum(fact => fact.MTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + con.ESCALE.DPAEsc.Value.ToShortDateString();
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(DemandeRestitutionCautionForm form, CONNAISSEMENT con, int type)
        {
            vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            if (type == 0)
            {
                form.conteneurs = con.CONTENEUR.Where(ctr => ctr.MCCtr.HasValue && ctr.MCCtr.Value > 0).ToList<CONTENEUR>();
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
            }
            else
            {
                form.conteneurs = vsomAcc.GetConteneursOfDemandeCaution(con.IdBL);
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
            }

            form.factures = vsomAcc.GetFacturesOfConnaissement(con.IdBL);
            form.dataGridFacts.ItemsSource = form.factures;

            form.montantHTCpteBL.Content = form.factures.Sum(fact => fact.MHT);
            form.montantTVACpteBL.Content = form.factures.Sum(fact => fact.MTVA);
            form.montantTTCCpteBL.Content = form.factures.Sum(fact => fact.MTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + con.ESCALE.DPAEsc.Value.ToShortDateString();
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(ExtensionFranchiseForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.eltsFact = vsomAcc.GetElementNonFactureBL(con.IdBL);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteBL.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(DemandeReductionForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.eltsFact = vsomAcc.GetElementNonFactureBL(con.IdBL);
            form.dataGridEltsFact.ItemsSource = form.eltsFact;
            form.montantHTCpteBL.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.articles = vsomAcc.GetArticlesFacturablesByIdBL(con.IdBL);
            form.arts = new List<string>();
            foreach (ARTICLE art in form.articles)
            {
                form.arts.Add(art.LibArticle);
            }
            form.cbArticle.ItemsSource = null;
            form.cbArticle.ItemsSource = form.arts;

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadDemandeLivraisonForm(DemandeLivraisonForm form, DEMANDE_LIVRAISON liv)
        {
             //vsp = new VsomParameters();
             vsomAcc = new VSOMAccessors();

            form.cbIdDL.Text = liv.IdDBL.ToString();
            form.cbIdDL.IsEnabled = true;

            //this.LoadConnaissementForm(form, liv.CONNAISSEMENT, 1);
            this.LoadConnaissementForm(form, liv, liv.CONNAISSEMENT);

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(liv.AIDBL)));

            form.factures = vsomAcc.GetFacturesOfConnaissement(liv.IdBL.Value);
            form.dataGridFacts.ItemsSource = form.factures;

            if (liv.DVDBL.HasValue)
            {
                form.lblStatut.Content = "Demande de livraison validée le : " + liv.DVDBL + " par " + vsomAcc.GetUtilisateursByIdU(liv.IdUV.Value).NU;
                form.btnValiderDL.Visibility = System.Windows.Visibility.Collapsed;
                form.btnEnregistrer.IsEnabled = false;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Collapsed;
                form.btnMAJDateDepot.IsEnabled = false;
                form.borderEtat.Visibility = System.Windows.Visibility.Visible;
                if (liv.CONTENEUR.Count > 0)
                {
                    form.btnGenererCoreor.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    form.btnGenererCoreor.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                form.lblStatut.Content = "Demande de livraison crée le : " + liv.DateDBL + " par " + vsomAcc.GetUtilisateursByIdU(liv.IdU.Value).NU; ;
                form.btnValiderDL.Visibility = System.Windows.Visibility.Visible;
                form.btnEnregistrer.IsEnabled = true;
                form.btnDetailsCtr.IsEnabled = true;
                form.btnDetailsVeh.IsEnabled = true;
                form.btnMAJDateDepot.IsEnabled = true;
                form.borderEtat.Visibility = System.Windows.Visibility.Collapsed;
            }

            form.borderActions.Visibility = System.Windows.Visibility.Visible;

            form.Title = "Demande de livraison - " + liv.IdDBL + " - Consignée - " + liv.CONNAISSEMENT.ConsigneeBL;
        }

        public void LoadBonSortieForm(BonSortieForm form, BON_SORTIE bs)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();
            form.cbIdBS.Text = bs.IdBS.ToString();

            CONNAISSEMENT bl = vsomAcc.GetConnaissementByIdBL(bs.IdBL.Value);

            this.LoadConnaissementForm(form, bl);

            form.txtObservations.Document.Blocks.Clear();
            form.txtObservations.Document.Blocks.Add(new Paragraph(new Run(bs.AIBS)));

            form.factures = vsomAcc.GetFacturesOfConnaissement(bs.IdBL.Value);
            form.dataGridFacts.ItemsSource = form.factures;

            form.btnEnregistrer.IsEnabled = false;
            form.borderActions.Visibility = System.Windows.Visibility.Visible;

            form.lblStatut.Content = "Bon de sortie crée le : " + bs.DateBS + " par " + vsomAcc.GetUtilisateursByIdU(bs.IdU.Value).NU;

            form.Title = "Bon de sortie - " + bs.IdBS + " - Consignée - " + bl.ConsigneeBL;
        }

        public void LoadConnaissementForm(DemandeLivraisonForm form, DEMANDE_LIVRAISON liv, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.Where(gc => gc.IdDBL == liv.IdDBL).ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.mafis = con.MAFI.Where(mf => mf.IdDBL == liv.IdDBL).ToList<MAFI>();
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Collapsed;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridMafis.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.Where(ctr => ctr.IdDBL == liv.IdDBL).ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Collapsed;
                form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.dataGridCautions.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.Where(veh => veh.IdDBL == liv.IdDBL).ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.factures = vsomAcc.GetFacturesOfConnaissement(con.IdBL);
            form.dataGridFacts.ItemsSource = form.factures;

            form.borderActions.Visibility = System.Windows.Visibility.Visible;

            form.montantHTCpteBL.Content = form.factures.Sum(fact => fact.MHT) + form.conteneurs.Sum(ctr => ctr.MCCtr);
            form.montantTVACpteBL.Content = form.factures.Sum(fact => fact.MTVA);
            form.montantTTCCpteBL.Content = form.factures.Sum(fact => fact.MTTC) + form.conteneurs.Sum(ctr => ctr.MCCtr);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(DemandeLivraisonForm form, CONNAISSEMENT con, int type)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            if (type == 0)
            {
                form.conventionnels = vsomAcc.GetConventionnelsImportLivraisonOfConnaissement(con.IdBL);
            }
            else
            {
                form.conventionnels = vsomAcc.GetConventionnelsImportOfDBL(Convert.ToInt32(form.cbIdDL.Text));
            }

            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            if (type == 0)
            {
                form.mafis = vsomAcc.GetMafisImportLivraisonOfConnaissement(con.IdBL);
            }
            else
            {
                form.mafis = vsomAcc.GetMafisImportOfDBL(Convert.ToInt32(form.cbIdDL.Text));
            }

            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridMafis.ItemsSource = form.mafis;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            if (type == 0)
            {
                form.conteneurs = vsomAcc.GetConteneursImportLivraisonOfConnaissement(con.IdBL);
            }
            else
            {
                form.conteneurs = vsomAcc.GetConteneursImportOfDBL(Convert.ToInt32(form.cbIdDL.Text));
            }

            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Collapsed;
                form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
                //form.factTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.dataGridCautions.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            if (con.ESCALE.ACCONIER.NomAcc == "Socomar")
            {
                if (type == 0)
                {
                    form.vehicules = vsomAcc.GetVehiculesLivraisonOfConnaissement(con.IdBL);
                }
                else
                {
                    form.vehicules = vsomAcc.GetVehiculesImportOfDBL(Convert.ToInt32(form.cbIdDL.Text));
                }
            }
            else
            {
                if (type == 0)
                {
                    form.vehicules = vsomAcc.GetVehiculesOfConnaissement(con.IdBL);
                }
                else
                {
                    form.vehicules = vsomAcc.GetVehiculesImportOfDBL(Convert.ToInt32(form.cbIdDL.Text));
                }                
            }

            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Collapsed;
                //form.factTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.factures = vsomAcc.GetFacturesOfConnaissement(con.IdBL);
            form.dataGridFacts.ItemsSource = form.factures;

            form.borderActions.Visibility = System.Windows.Visibility.Visible;

            form.montantHTCpteBL.Content = form.factures.Sum(fact => fact.MHT) + form.conteneurs.Sum(ctr => ctr.MCCtr);
            form.montantTVACpteBL.Content = form.factures.Sum(fact => fact.MTVA);
            form.montantTTCCpteBL.Content = form.factures.Sum(fact => fact.MTTC) + form.conteneurs.Sum(ctr => ctr.MCCtr);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(BonSortieForm form, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = vsomAcc.GetConventionnelsImportSortieOfConnaissement(con.IdBL);
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.conteneurs = vsomAcc.GetConteneursImportSortieOfConnaissement(con.IdBL);
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.mafis = vsomAcc.GetMafisImportSortieOfConnaissement(con.IdBL);
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridMafis.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            if (con.ESCALE.ACCONIER.NomAcc == "Socomar")
            {
                form.vehicules = vsomAcc.GetVehiculesSortieOfConnaissement(con.IdBL);
            }
            else
            {
                form.vehicules = vsomAcc.GetVehiculesOfConnaissement(con.IdBL);
            }

            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.factures = vsomAcc.GetFacturesOfConnaissement(con.IdBL);
            form.dataGridFacts.ItemsSource = form.factures;

            form.montantHTCpteBL.Content = form.factures.Sum(fact => fact.MHT) + form.conteneurs.Sum(ctr => ctr.MCCtr);
            form.montantTVACpteBL.Content = form.factures.Sum(fact => fact.MTVA);
            form.montantTTCCpteBL.Content = form.factures.Sum(fact => fact.MTTC) + form.conteneurs.Sum(ctr => ctr.MCCtr);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(OrdreServiceForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.txtClient.Text = con.CLIENT.NomClient;
            form.txtCodeClient.Text = con.CLIENT.CodeClient;

            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(VehiculeForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(con);
            form.cons = new List<string>();
            form.cons.Add(con.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();
            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(ConteneurForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(con);
            form.cons = new List<string>();
            form.cons.Add(con.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();
            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(ConteneurTCForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(con);
            form.cons = new List<string>();
            form.cons.Add(con.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();
            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(MafiForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(con);
            form.cons = new List<string>();
            form.cons.Add(con.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();
            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(ConventionnelForm form, CONNAISSEMENT con)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();

            form.connaissements = new List<CONNAISSEMENT>();
            form.connaissements.Add(con);
            form.cons = new List<string>();
            form.cons.Add(con.NumBL);
            form.cbNumBL.ItemsSource = null;
            form.cbNumBL.ItemsSource = form.cons;
            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();
            form.txtConsignee.Text = con.ConsigneeBL;
        }

        public void LoadConnaissementForm(BonEnleverForm form, CONNAISSEMENT con)
        {
            vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.Where(c => !c.IdBAE.HasValue).ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.mafis = con.MAFI.Where(m => !m.IdBAE.HasValue).ToList<MAFI>();
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridMafis.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.Where(c => !c.IdBAE.HasValue).ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Collapsed;
                form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.dataGridCautions.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.Where(v => !v.IdBAE.HasValue).ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.borderActions.Visibility = System.Windows.Visibility.Visible;

            form.dataGridCompteBL.ItemsSource = vsomAcc.GetCompteBL(con.IdBL);
            double debit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Debit);
            double credit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Credit);
            form.lblStatutPaiement.Content = "Debit : " + debit + " / Crédit : " + credit + ". Solde du BL : " + (credit - debit).ToString();

            //Valeur du BL
            form.eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
            form.montantHTCpteBL.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadConnaissementForm(BonEnleverForm form, BON_ENLEVEMENT bae, CONNAISSEMENT con)
        {
             vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            form.cbNumBL.Text = con.NumBL;
            form.txtIdBL.Text = con.IdBL.ToString();

            form.cbClient.SelectedItem = con.CLIENT.NomClient;

            form.txtConsignee.Text = con.ConsigneeBL;
            form.txtAdresse.Text = con.AdresseBL;
            form.txtContrib.Text = con.NContribBL;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;
            form.txtNomMand.Text = con.NomManBL;
            form.txtCNIMand.Text = con.CNIManBL;
            form.txtTelMand.Text = con.PhoneManBL;

            form.escales = new List<ESCALE>();
            form.escales.Add(con.ESCALE);
            form.escs = new List<Int32>();
            form.escs.Add(con.ESCALE.NumEsc.Value);

            form.cbNumEsc.ItemsSource = null;
            form.cbNumEsc.ItemsSource = form.escs;
            form.cbNumEsc.SelectedItem = con.ESCALE.NumEsc;

            if (con.BLIL.Equals("Y"))
            {
                form.chkHinterland.IsChecked = true;
            }
            else
            {
                form.chkHinterland.IsChecked = false;
            }

            if (con.BLGN.Equals("Y"))
            {
                form.chkGN.IsChecked = true;
            }
            else
            {
                form.chkGN.IsChecked = false;
            }

            if (con.BlBloque.Equals("Y"))
            {
                form.chkBloque.IsChecked = true;
            }
            else
            {
                form.chkBloque.IsChecked = false;
            }

            if (con.BLER.Equals("Y"))
            {
                form.chkExpressRelease.IsChecked = true;
            }
            else
            {
                form.chkExpressRelease.IsChecked = false;
            }

            form.conventionnels = con.CONVENTIONNEL.Where(gc => gc.IdBAE == bae.IdBAE).ToList<CONVENTIONNEL>();
            if (form.conventionnels.Count == 0)
            {
                form.gcTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                form.dataGridGC.ItemsSource = form.conventionnels;
                form.btnDetailsGC.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.Visibility = System.Windows.Visibility.Visible;
                form.gcTab.IsSelected = true;
            }

            form.mafis = con.MAFI.Where(mf => mf.IdBAE == bae.IdBAE).ToList<MAFI>();
            if (form.mafis.Count == 0)
            {
                form.mafiTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Collapsed;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridMafis.ItemsSource = form.mafis;
                form.mafiTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsMafi.Visibility = System.Windows.Visibility.Visible;
                form.mafiTab.IsSelected = true;
            }

            form.conteneurs = con.CONTENEUR.Where(ctr => ctr.IdBAE == bae.IdBAE).ToList<CONTENEUR>();
            if (form.conteneurs.Count == 0)
            {
                form.contTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Collapsed;
                form.cautionsTab.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridConteneurs.ItemsSource = form.conteneurs;
                form.dataGridCautions.ItemsSource = form.conteneurs;
                form.contTab.Visibility = System.Windows.Visibility.Visible;
                form.cautionsTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsCtr.Visibility = System.Windows.Visibility.Visible;
                form.contTab.IsSelected = true;
            }

            form.vehicules = con.VEHICULE.Where(veh => veh.IdBAE == bae.IdBAE).ToList<VEHICULE>();
            if (form.vehicules.Count == 0)
            {
                form.vehTab.Visibility = System.Windows.Visibility.Collapsed;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Collapsed;
                //form.compteBLTab.IsSelected = true;
                form.marchTab.IsSelected = true;
            }
            else
            {
                form.dataGridVehicules.ItemsSource = form.vehicules;
                form.vehTab.Visibility = System.Windows.Visibility.Visible;
                form.btnDetailsVeh.Visibility = System.Windows.Visibility.Visible;
                form.vehTab.IsSelected = true;
            }

            form.dataGridCompteBL.ItemsSource = vsomAcc.GetCompteBL(con.IdBL);
            double debit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Debit);
            double credit = form.dataGridCompteBL.Items.OfType<ElementCompte>().Sum(el => el.Credit);
            form.lblStatutPaiement.Content = "Debit : " + debit + " / Crédit : " + credit + ". Solde du BL : " + (credit - debit).ToString();

            //Valeur du BL
            form.eltsFact = vsomAcc.GetElementFacturationBL(con.IdBL);
            form.montantHTCpteBL.Content = form.eltsFact.Sum(elt => elt.PrixUnitaire * elt.Qte);
            form.montantTVACpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTVA);
            form.montantTTCCpteBL.Content = form.eltsFact.Sum(elt => elt.MontantTTC);

            form.lblAcconier.Content = "Acconier : " + con.ESCALE.ACCONIER.NomAcc;
            form.lblEscale.Content = "Escale : " + con.ESCALE.NumEsc + " du " + (con.ESCALE.DRAEsc.HasValue ? con.ESCALE.DRAEsc.Value.ToShortDateString() : con.ESCALE.DPAEsc.Value.ToShortDateString());
            form.lblNavire.Content = "Navire : " + con.ESCALE.NAVIRE.NomNav;
        }

        public void LoadUtilisateurForm(UtilisateurForm form, UTILISATEUR user)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            //VsomParameters vsp = new VsomParameters();

            //form.userOfForm = user;
            form.txtLogin.Text = user.LU;
            form.txtLogin.IsEnabled = false;
            form.txtNomUser.Text = user.NU;
            PwdHash pwdhash = new PwdHash(user.MPU);
            form.txtPassword.Password = pwdhash.Decrypt();
            form.txtPasswordConfirmation.Password = pwdhash.Decrypt();
            form.cbCaisse.Text = user.Caisse;
            form.cbAcconier.SelectedItem = user.ACCONIER.NomAcc;
            if (user.IdParc.HasValue)
            {
                form.cbParc.SelectedItem = user.PARC.NomParc;
            }
            form.chkStatut.IsChecked = user.EU == "A" ? true : false;
            form.operationOfUser = vsomAcc.GetOperationsUtilisateur(user.IdU);
            form.dataGridOperationsUser.ItemsSource = form.operationOfUser;
            form.Title = "Utilisateur : " + user.NU;
            form.lblStatut.Content = user.EU == "A" ? "Actif" : "Inactif";
        }
    }
}

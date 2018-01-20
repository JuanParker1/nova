using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VesselStopOverData;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour ManifesteForm.xaml
    /// </summary>
    public partial class ManifesteForm : Window
    {
        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<MANIFESTE> manifestes { get; set; }
        public List<Int32> manifs { get; set; }

        private List<PORT> ports;
        public List<string> prts { get; set; }

        public List<CONNAISSEMENT> connaissements { get; set; }
        public List<VEHICULE> vehicules { get; set; }
        public List<CONTENEUR> conteneurs { get; set; }
        public List<MAFI> mafis { get; set; }
        public List<CONVENTIONNEL> conventionnels { get; set; }
        public List<ElementCompteManifeste> eltsValeurMan { get; set; }
        public List<ElementCompteDIT> eltsCompteDIT { get; set; }
        public StatutLoadUnload statutDechargement { get; set; }

        public EscaleForm escForm { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public ManifestePanel manifPanel { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public ManifesteForm(string type, ManifestePanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;
                InitializeCombos();

                manifPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                actionsBorder.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ManifesteForm(EscaleForm form, MANIFESTE man, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                escForm = form;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);
                
                formLoader = new FormLoader(utilisateur);

                formLoader.LoadManifesteForm(this, man);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ManifesteForm(string type, EscaleForm form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                escForm = form;

                ESCALE esc = vsp.GetEscaleById(Convert.ToInt32(form.txtEscaleSysID.Text));
                escales = new List<ESCALE>();
                escales.Add(esc);
                escs = new List<int>();
                escs.Add(esc.NumEsc.Value);
                cbNumEscale.ItemsSource = null;
                cbNumEscale.ItemsSource = escs;
                cbNumEscale.SelectedItem = esc.NumEsc;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                actionsBorder.Visibility = System.Windows.Visibility.Hidden;

                formLoader = new FormLoader(utilisateur);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        public ManifesteForm(ManifestePanel panel, MANIFESTE man, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                manifPanel = panel;
                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                formLoader = new FormLoader(utilisateur);

                formLoader.LoadManifesteForm(this, man);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void InitializeCombos()
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();

                ports = vsp.GetPortsOrderByNom();
                prts = new List<string>();
                foreach (PORT prt in ports)
                {
                    prts.Add(prt.NomPort);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void cbNumEscale_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtNumEscale.Text = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc.ToString();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void cbPort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodePort.Text = ports.ElementAt<PORT>(cbPort.SelectedIndex).CodePort;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        private void txtNbPrev_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            //VSOMAccessors vsomAcc = new VSOMAccessors();
            VsomMarchal vsomAcc = new VsomMarchal();

            if (cbIdMan.IsEnabled == false)
            {
                // procedure d'enregistrement d'un nouveau manifeste
                if (cbNumEscale.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer l'escale à laquelle ce manifeste est rattaché.", "N° escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbPort.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer le port d'embarquement.", "Port d'embarquement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridConnaissement.Items.Count == 0 && cbFormat.SelectedIndex != 5)
                {
                    if (MessageBox.Show("Il n'y a actuellement aucun connaissement importé pour ce manifeste voulez-vous le faire maintenant ?", "Importer connaissement ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        btnSelectFile_Click(sender, e);
                    }
                }
                else
                {
                    try
                    {
                        if (connaissements == null)
                        {
                            connaissements = new List<CONNAISSEMENT>();
                        }
                        if (vehicules == null)
                        {
                            vehicules = new List<VEHICULE>();
                        }
                        if (conteneurs == null)
                        {
                            conteneurs = new List<CONTENEUR>();
                        }
                        if (mafis == null)
                        {
                            mafis = new List<MAFI>();
                        }
                        if (conventionnels == null)
                        {
                            conventionnels = new List<CONVENTIONNEL>();
                        }
                        MANIFESTE man = vsomAcc.InsertManifeste(Convert.ToInt32(txtNumEscale.Text), txtCodePort.Text, Convert.ToInt16(txtNbPrevBL.Text), Convert.ToInt16(txtNbPrevVeh.Text), Convert.ToInt16(txtNbPrevCont.Text), Convert.ToInt16(txtNbPrevMafi.Text), Convert.ToInt16(txtNbPrevGC.Text), (short)cbFormat.SelectedIndex, txtCheminFichier.Text, connaissements, vehicules, conteneurs, mafis, conventionnels, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);

                        //Raffraîchir les informations
                        formLoader.LoadManifesteForm(this, man);

                        cbIdMan.IsEnabled = true;
                        actionsBorder.Visibility = System.Windows.Visibility.Visible;

                        if (this.escForm != null)
                        {
                            formLoader.LoadEscaleForm(this.escForm, man.ESCALE);
                        }
                        else if (this.manifPanel != null)
                        {
                            if (manifPanel.cbFiltres.SelectedIndex != 1)
                            {
                                manifPanel.cbFiltres.SelectedIndex = 1;
                            }
                            else
                            {
                                manifPanel.manifestes = vsp.GetManifestesByStatut("En cours");
                                manifPanel.dataGrid.ItemsSource = manifPanel.manifestes;
                                manifPanel.lblStatut.Content = manifPanel.manifestes.Count + " Manifeste(s) en cours";
                            }
                        }

                        MessageBox.Show("Manifeste enregistré avec avec succès.", "Manifeste enregistré !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (EnregistrementInexistant ex)
                    {
                        MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (HabilitationException ex)
                    {
                        MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                if (cbNumEscale.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer l'escale à laquelle ce manifeste est rattaché.", "N° escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (cbPort.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer le port d'embarquement.", "Port d'embarquement ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (dataGridConnaissement.Items.Count == 0)
                {
                    if (MessageBox.Show("Il n'y a actuellement aucun connaissement importé pour ce manifeste voulez-vous le faire maintenant ?", "Importer connaissement ?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        btnSelectFile_Click(sender, e);
                    }
                }
                else
                {
                    try
                    {
                        List<CONNAISSEMENT> cons = new List<CONNAISSEMENT>();
                        foreach (CONNAISSEMENT c in connaissements)
                        {
                            cons.Add(new CONNAISSEMENT
                            {
                                NumBL = c.NumBL,
                                ESCALE = c.ESCALE,
                                BLIL = c.BLIL,
                                BLGN = c.BLGN,
                                CCBL = c.CCBL,
                                TypeBL = c.TypeBL,
                                BLDette = c.BLDette,
                                DetteMontant = c.DetteMontant,
                                ConsigneeBL = c.ConsigneeBL,
                                AdresseBL = c.AdresseBL,
                                NotifyBL = c.NotifyBL,
                                LPBL = c.LPBL,
                                DPBL = c.DPBL,
                                DCBL = c.DCBL,
                                BLFO = c.BLFO,
                                BLLT = c.BLLT,
                                BlBloque = c.BlBloque,
                                BLER = c.BLER,
                                BLSocar = c.BLSocar,
                                SensBL = c.SensBL,
                                CodeTVA = c.CodeTVA,
                                StatutBL = c.StatutBL,
                                EtatBL = c.EtatBL,
                                CCBLMontant = c.CCBLMontant,
                                IdAcc = c.IdAcc,
                                PoidsBL = c.PoidsBL,
                                VolBL = c.VolBL,
                                DestBL = c.DestBL,
                                LastModif = DateTime.Now,
                                IdU = utilisateur.IdU,
                                NomCharger = c.NomCharger,
                                AdresseCharger = c.AdresseCharger
                            });
                        }

                        List<VEHICULE> vehs = new List<VEHICULE>();
                        foreach (VEHICULE v in vehicules)
                        {
                            var bl = (from con in cons
                                      where con.NumBL == v.CONNAISSEMENT.NumBL
                                      select con).FirstOrDefault<CONNAISSEMENT>();
                            vehs.Add(new VEHICULE
                            {
                                CONNAISSEMENT = bl,
                                ESCALE = v.ESCALE,
                                MANIFESTE = v.MANIFESTE,
                                IdBL = bl.IdBL,
                                NumChassis = v.NumChassis,
                                DescVeh = v.DescVeh,
                                TypeMVeh = v.TypeMVeh,
                                TypeCVeh = v.TypeCVeh,
                                StatutVeh = v.StatutVeh,
                                PoidsMVeh = v.PoidsMVeh,
                                PoidsCVeh = v.PoidsCVeh,
                                VolMVeh = v.VolMVeh,
                                VolCVeh = v.VolCVeh,
                                LongMVeh = v.LongMVeh,
                                LongCVeh = v.LongCVeh,
                                LargMVeh = v.LargMVeh,
                                LargCVeh = v.LargCVeh,
                                HautMVeh = v.HautMVeh,
                                HautCVeh = v.HautCVeh,
                                NumItem = v.NumItem,
                                InfoMan = v.InfoMan,
                                BarCode = v.BarCode,
                                DCVeh = v.DCVeh,
                                StatVeh = v.StatVeh,
                                SensVeh = v.SensVeh,
                                StatutCVeh = v.StatutCVeh,
                                IdAcc = v.IdAcc
                            });
                        }

                        List<CONTENEUR> ctrs = new List<CONTENEUR>();
                        foreach (CONTENEUR c in conteneurs)
                        {
                            var bl = (from con in cons
                                      where con.NumBL == c.CONNAISSEMENT.NumBL
                                      select con).FirstOrDefault<CONNAISSEMENT>();
                            ctrs.Add(new CONTENEUR
                            {
                                CONNAISSEMENT = bl,
                                IdBL = bl.IdBL,
                                ESCALE = c.ESCALE,
                                MANIFESTE = c.MANIFESTE,
                                NumCtr = c.NumCtr,
                                DescCtr = c.DescCtr,
                                TypeMCtr = c.TypeMCtr,
                                TypeCCtr = c.TypeCCtr,
                                StatutCtr = c.StatutCtr,
                                PoidsMCtr = c.PoidsMCtr,
                                PoidsCCtr = c.PoidsCCtr,
                                CatMseCtr = c.CatMseCtr,
                                NumItem = c.NumItem,
                                InfoMan = c.InfoMan,
                                Seal1Ctr = c.Seal1Ctr,
                                Seal2Ctr = c.Seal2Ctr,
                                IMDGCode = c.IMDGCode,
                                DescMses = c.DescMses,
                                PropCtr = c.PropCtr,
                                MCCtr = c.MCCtr,
                                DCCtr = c.DCCtr,
                                StatCtr = c.StatCtr,
                                SensCtr = c.SensCtr
                            });
                        }

                        List<MAFI> mfs = new List<MAFI>();
                        foreach (MAFI m in mafis)
                        {
                            var bl = (from con in cons
                                      where con.NumBL == m.CONNAISSEMENT.NumBL
                                      select con).FirstOrDefault<CONNAISSEMENT>();
                            mfs.Add(new MAFI
                            {
                                CONNAISSEMENT = bl,
                                IdBL = bl.IdBL,
                                ESCALE = m.ESCALE,
                                MANIFESTE = m.MANIFESTE,
                                NumMafi = m.NumMafi,
                                DescMafi = m.DescMafi,
                                TypeMMafi = m.TypeMMafi,
                                TypeCMafi = m.TypeCMafi,
                                PoidsMMafi = m.PoidsMMafi,
                                PoidsCMafi = m.PoidsCMafi,
                                CatMseMafi = m.CatMseMafi,
                                NumItem = m.NumItem,
                                InfoMan = m.InfoMan,
                                IMDGCode = m.IMDGCode,
                                DescMses = m.DescMses,
                                //PropMafi = m.PropMafi,
                                //MCMafi = m.MCMafi,
                                DCMafi = m.DCMafi,
                                StatMafi = m.StatMafi,
                                SensMafi = m.SensMafi
                            });
                        }

                        List<CONVENTIONNEL> gcs = new List<CONVENTIONNEL>();
                        foreach (CONVENTIONNEL c in conventionnels)
                        {
                            var bl = (from con in cons
                                      where con.NumBL == c.CONNAISSEMENT.NumBL
                                      select con).FirstOrDefault<CONNAISSEMENT>();
                            gcs.Add(new CONVENTIONNEL
                            {
                                CONNAISSEMENT = bl,
                                IdBL = bl.IdBL,
                                ESCALE = c.ESCALE,
                                MANIFESTE = c.MANIFESTE,
                                NumGC = c.NumGC,
                                DescGC = c.DescGC,
                                TypeMGC = c.TypeMGC,
                                TypeCGC = c.TypeCGC,
                                PoidsMGC = c.PoidsMGC,
                                PoidsCGC = c.PoidsCGC,
                                VolMGC = c.VolMGC,
                                VolCGC = c.VolCGC,
                                LongMGC = c.LongMGC,
                                LongCGC = c.LongCGC,
                                LargMGC = c.LargMGC,
                                LargCGC = c.LargCGC,
                                HautMGC = c.HautMGC,
                                HautCGC = c.HautCGC,
                                NumItem = c.NumItem,
                                InfoMan = c.InfoMan,
                                DCGC = c.DCGC,
                                StatGC = c.StatGC,
                                SensGC = c.SensGC
                            });
                        }

                        MANIFESTE man = vsomAcc.UpdateManifeste(Convert.ToInt32(cbIdMan.Text), Convert.ToInt32(txtNumEscale.Text), txtCodePort.Text, Convert.ToInt16(txtNbPrevBL.Text), Convert.ToInt16(txtNbPrevVeh.Text), Convert.ToInt16(txtNbPrevCont.Text), Convert.ToInt16(txtNbPrevMafi.Text), Convert.ToInt16(txtNbPrevGC.Text), (short)cbFormat.SelectedIndex, txtCheminFichier.Text, cons, vehs, ctrs, mfs, gcs, new TextRange(txtObservations.Document.ContentStart, txtObservations.Document.ContentEnd).Text, utilisateur.IdU);
                        MessageBox.Show("Mise à jour effectuée avec succès.", "Mise à jour effectuée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (EnregistrementInexistant ex)
                    {
                        MessageBox.Show(ex.Message, "Enregistrement inexistant !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (HabilitationException ex)
                    {
                        MessageBox.Show(ex.Message, "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    catch (ApplicationException ex)
                    {
                        MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            System.IO.StreamReader file = null;
            try
            {
                if (cbNumEscale.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez indiquer l'escale à laquelle ce manifeste est rattaché.", "N° escale ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                    dlg.DefaultExt = ".txt";
                    dlg.Filter = "EXTRACT ALL Files (*.txt)|*.txt|EXTRACT ALL Files Extended (*.txt)|*.txt|Excel Véhicules Autres Acconiers (*.xls)|*.xls|Excel Véhicules Autres Acconiers (*.xlsx)|*.xlsx|Manifeste Conteneurs NDS (*.xlsx)|*.xlsx";
                    Nullable<bool> result = dlg.ShowDialog();
                    if (result == true)
                    {
                        string filename = dlg.FileName;
                        txtCheminFichier.Text = filename;
                        cbFormat.SelectedIndex = dlg.FilterIndex - 1;
                    }
                    else
                    {
                        return;
                    }

                    if (cbFormat.SelectedIndex == 0)
                    {
                        // vider les listes
                        #region extract All
                        dataGridConnaissement.ItemsSource = null;
                        dataGridVehicules.ItemsSource = null;
                        dataGridCont.ItemsSource = null;
                        dataGridMafi.ItemsSource = null;
                        dataGridGC.ItemsSource = null;

                        file = new System.IO.StreamReader(txtCheminFichier.Text);
                        // re-enregistrer le fichier avec l'encodage ANSI

                        connaissements = new List<CONNAISSEMENT>();
                        vehicules = new List<VEHICULE>();
                        conteneurs = new List<CONTENEUR>();
                        mafis = new List<MAFI>();
                        conventionnels = new List<CONVENTIONNEL>();

                        //string line = file.ReadLine();
                        string line = file.ReadLine();
                        while (line != null && line.Length <= 796 && line.Length >= 790)
                        {
                            // ajout du connaissement
                            if (line.Substring(19, 17).Trim() != "")
                            {
                                #region BL
                                CONNAISSEMENT bl = new CONNAISSEMENT();
                                bl.NumBL = line.Substring(19, 17).Trim();
                                if (line.Substring(630, 2).Trim() == "TR")
                                {
                                    bl.BLIL = "Y";
                                }
                                else
                                {
                                    bl.BLIL = "N";
                                }
                                bl.BLGN = "N";
                                bl.BlBloque = "N";
                                bl.BLER = "N";
                                bl.BLSocar = "N";
                                bl.SensBL = "I";
                                bl.CCBL = line.Substring(613, 1).Trim();
                                bl.TypeBL = line.Substring(327, 1).Trim();
                                bl.ConsigneeBL = line.Substring(97, 34).Trim().ToUpper();
                                bl.AdresseBL = (line.Substring(132, 34).Trim() + " " + line.Substring(167, 34).Trim() + " " + line.Substring(202, 34).Trim() + " " + line.Substring(237, 34).Trim()).Trim().ToUpper();
                                bl.NotifyBL = line.Substring(396, 34).Trim().ToUpper();
                                bl.LPBL = line.Substring(7, 5).Trim();
                                bl.DPBL = line.Substring(13, 5).Trim();
                                bl.DCBL = DateTime.Now;
                                bl.StatutBL = "Non initié";
                                bl.EtatBL = "O";
                                bl.CCBLMontant = 0;
                                bl.BLDette = "N";
                                bl.DetteMontant = 0;
                                bl.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;
                                bl.IdEsc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc;
                                bl.PoidsBL = 0;
                                bl.VolBL = 0;
                                bl.DestBL = bl.TypeBL == "C" ? "HINT" : "CMR";
                                bl.SensBL = "I";
                                bl.LastModif = DateTime.Now;
                                bl.IdU = utilisateur.IdU;
                                bl.NomCharger = "GRIMALDI";
                                bl.AdresseCharger = "GRIMALDI";

                                if (connaissements.FirstOrDefault<CONNAISSEMENT>(connaissement => connaissement.NumBL == bl.NumBL) == null)
                                {
                                    connaissements.Add(bl);
                                }

                                #endregion
                                // ajout de marchandises
                                if (line.Substring(327, 1).Equals("C"))
                                {
                                    // ajout conteneur
                                    #region conteneur
                                    CONTENEUR ctr = new CONTENEUR();
                                    ctr.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    ctr.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    ctr.NumCtr = line.Substring(42, 17).Trim();
                                    ctr.DescCtr = line.Substring(60, 36).Trim();
                                    ctr.TypeMCtr = line.Substring(329, 20).Trim().Replace("BX", "DV");
                                    ctr.TypeCCtr = line.Substring(329, 20).Trim().Replace("BX", "DV");
                                    ctr.StatutCtr = line.Substring(394, 1).Trim();
                                    ctr.PoidsMCtr = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    ctr.PoidsCCtr = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    ctr.CatMseCtr = line.Substring(633, 2).Trim();
                                    ctr.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    ctr.InfoMan = line;
                                    ctr.Seal1Ctr = line.Substring(350, 10).Trim();
                                    ctr.Seal2Ctr = line.Substring(361, 10).Trim();
                                    ctr.IMDGCode = line.Substring(636, 5).Trim();
                                    ctr.TypeMses = ctr.IMDGCode == "" ? "N" : "D";
                                    ctr.DescMses = line.Substring(641, 134).Trim();
                                    ctr.MCCtr = 6000000;
                                    ctr.PropCtr = 1;
                                    ctr.VolMCtr = 0;
                                    ctr.DCCtr = DateTime.Now;
                                    ctr.StatCtr = "Non initié";
                                    ctr.SensCtr = "I";

                                    conteneurs.Add(ctr);
                                    #endregion
                                }
                                else if (line.Substring(327, 1).Equals("B"))
                                {
                                    // ajout Mafi
                                    #region mafi
                                    MAFI mafi = new MAFI();
                                    mafi.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    mafi.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    mafi.NumMafi = line.Substring(42, 17).Trim();
                                    mafi.DescMafi = line.Substring(60, 36).Trim();
                                    mafi.TypeMMafi = line.Substring(329, 20).Trim();
                                    mafi.TypeCMafi = line.Substring(329, 20).Trim();
                                    mafi.PoidsMMafi = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    mafi.PoidsCMafi = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    mafi.CatMseMafi = line.Substring(633, 2).Trim();
                                    mafi.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    mafi.InfoMan = line;
                                    //mafi.Seal1Ctr = line.Substring(350, 10).Trim();
                                    //mafi.Seal2Ctr = line.Substring(361, 10).Trim();
                                    mafi.IMDGCode = line.Substring(636, 5).Trim();
                                    mafi.DescMses = line.Substring(641, 134).Trim();
                                    //mafi.MCMafi = 6000000;
                                    //mafi.PropMafi= 1;
                                    mafi.VolMMafi = 0;
                                    mafi.DCMafi = DateTime.Now;
                                    mafi.StatMafi = "Non initié";
                                    mafi.SensMafi = "I";

                                    mafis.Add(mafi);
                                    #endregion
                                }
                                else if (line.Substring(42, 17).Trim() != "" && line.Substring(791, 1).Trim() != "")
                                {
                                    // ajout de véhicule
                                    #region vehicule
                                    VEHICULE veh = new VEHICULE();
                                    veh.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    veh.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    veh.NumChassis = line.Substring(42, 17).Trim();
                                    veh.DescVeh = line.Substring(60, 36).Trim();
                                    veh.TypeMVeh = line.Substring(273, 1).Trim();
                                    veh.TypeCVeh = line.Substring(273, 1).Trim();
                                    veh.StatutVeh = line.Substring(791, 1).Trim();
                                    veh.PoidsMVeh = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    veh.PoidsCVeh = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    veh.VolMVeh = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    veh.VolCVeh = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    veh.LongMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    veh.LongCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    veh.LargMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    veh.LargCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    veh.HautMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    veh.HautCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    veh.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    veh.InfoMan = line;
                                    veh.BarCode = line.Substring(571, 30).Trim();
                                    veh.DCVeh = DateTime.Now;
                                    veh.StatVeh = "Non initié";
                                    veh.SensVeh = "I";
                                    veh.StatutCVeh = veh.StatutVeh;
                                    veh.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;

                                    vehicules.Add(veh);
                                    #endregion
                                }
                                else
                                {
                                    // ajout general cargo
                                    #region conventionel
                                    CONVENTIONNEL conv = new CONVENTIONNEL();
                                    conv.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    conv.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    conv.NumGC = line.Substring(42, 17).Trim();
                                    conv.DescGC = line.Substring(60, 36).Trim();
                                    conv.TypeMGC = "I404";
                                    conv.TypeCGC = "I404";
                                    conv.PoidsMGC = Math.Round(Convert.ToDouble(line.Substring(275, 7).Trim()) / 1000, 3);
                                    conv.PoidsCGC = Math.Round(Convert.ToDouble(line.Substring(275, 7).Trim()) / 1000, 3);
                                    conv.VolMGC = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    conv.VolCGC = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    conv.LongMGC = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    conv.LongCGC = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    conv.LargMGC = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    conv.LargCGC = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    conv.HautMGC = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    conv.HautCGC = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    conv.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    conv.InfoMan = line;
                                    conv.DCGC = DateTime.Now;
                                    conv.StatGC = "Non initié";
                                    conv.SensGC = "I";

                                    conventionnels.Add(conv);
                                    #endregion
                                }
                            }
                            line = file.ReadLine();
                        }

                        //file.Close();

                        dataGridConnaissement.ItemsSource = connaissements;
                        txtNbEffBL.Text = connaissements.Count.ToString();

                        dataGridVehicules.ItemsSource = vehicules;
                        txtNbEffVeh.Text = vehicules.Count.ToString();

                        dataGridCont.ItemsSource = conteneurs;
                        txtNbEffCont.Text = conteneurs.Count.ToString();

                        dataGridMafi.ItemsSource = mafis;
                        txtNbEffMafi.Text = mafis.Count.ToString();

                        dataGridGC.ItemsSource = conventionnels;
                        txtNbEffGC.Text = conventionnels.Count.ToString(); 
                        #endregion
                    }
                    else if (cbFormat.SelectedIndex == 1)
                    {
                        // vider les listes
                        #region EXTRACT ALL Files Extended
                        dataGridConnaissement.ItemsSource = null;
                        dataGridVehicules.ItemsSource = null;
                        dataGridCont.ItemsSource = null;
                        dataGridMafi.ItemsSource = null;
                        dataGridGC.ItemsSource = null;

                        file = new System.IO.StreamReader(txtCheminFichier.Text);
                        // re-enregistrer le fichier avec l'encodage ANSI

                        connaissements = new List<CONNAISSEMENT>();
                        vehicules = new List<VEHICULE>();
                        conteneurs = new List<CONTENEUR>();
                        mafis = new List<MAFI>();
                        conventionnels = new List<CONVENTIONNEL>();

                        //string line = file.ReadLine();
                        string line = file.ReadLine();
                        while (line != null && line.Length <= 900 && line.Length >= 800)
                        {
                            // ajout du connaissement
                            if (line.Substring(19, 17).Trim() != "")
                            {
                                #region BL
                                CONNAISSEMENT bl = new CONNAISSEMENT();
                                bl.NumBL = line.Substring(19, 17).Trim();
                                if (line.Substring(630, 2).Trim() == "TR")
                                {
                                    bl.BLIL = "Y";
                                }
                                else
                                {
                                    bl.BLIL = "N";
                                }
                                bl.BLGN = "N";
                                bl.BlBloque = "N";
                                bl.BLER = "N";
                                bl.BLSocar = "N";
                                bl.SensBL = "I";
                                bl.CCBL = line.Substring(613, 1).Trim();
                                bl.TypeBL = line.Substring(327, 1).Trim();
                                bl.ConsigneeBL = line.Substring(97, 34).Trim().ToUpper();
                                bl.AdresseBL = (line.Substring(132, 34).Trim() + " " + line.Substring(167, 34).Trim() + " " + line.Substring(202, 34).Trim() + " " + line.Substring(237, 34).Trim()).Trim().ToUpper();
                                bl.NotifyBL = line.Substring(396, 34).Trim().ToUpper();
                                bl.LPBL = line.Substring(7, 5).Trim();
                                bl.DPBL = line.Substring(13, 5).Trim();
                                bl.DCBL = DateTime.Now;
                                bl.StatutBL = "Non initié";
                                bl.EtatBL = "O";
                                bl.CCBLMontant = 0;
                                bl.BLDette = "N";
                                bl.DetteMontant = 0;
                                bl.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;
                                bl.IdEsc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc;
                                bl.PoidsBL = 0;
                                bl.VolBL = 0;
                                bl.DestBL = bl.TypeBL == "C" ? "HINT" : "CMR";
                                bl.SensBL = "I";
                                bl.LastModif = DateTime.Now;
                                bl.IdU = utilisateur.IdU;
                                bl.NomCharger = "GRIMALDI";
                                bl.AdresseCharger = "GRIMALDI";

                                if (connaissements.FirstOrDefault<CONNAISSEMENT>(connaissement => connaissement.NumBL == bl.NumBL) == null)
                                {
                                    connaissements.Add(bl);
                                }

                                #endregion
                                // ajout de marchandises
                                if (line.Substring(327, 1).Equals("C"))
                                {
                                    // ajout conteneur
                                    #region conteneur
                                    CONTENEUR ctr = new CONTENEUR();
                                    ctr.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    ctr.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    ctr.NumCtr = line.Substring(42, 17).Trim();
                                    ctr.DescCtr = line.Substring(60, 36).Trim();
                                    ctr.TypeMCtr = line.Substring(329, 20).Trim().Replace("BX", "DV");
                                    ctr.TypeCCtr = line.Substring(329, 20).Trim().Replace("BX", "DV");
                                    ctr.StatutCtr = line.Substring(394, 1).Trim();
                                    ctr.PoidsMCtr = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    ctr.PoidsCCtr = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    ctr.CatMseCtr = line.Substring(633, 2).Trim();
                                    ctr.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    ctr.InfoMan = line;
                                    ctr.Seal1Ctr = line.Substring(350, 10).Trim();
                                    ctr.Seal2Ctr = line.Substring(361, 10).Trim();
                                    ctr.IMDGCode = line.Substring(636, 5).Trim();
                                    ctr.TypeMses = ctr.IMDGCode == "" ? "N" : "D";
                                    ctr.DescMses = line.Substring(641, 134).Trim();
                                    ctr.MCCtr = 6000000;
                                    ctr.PropCtr = 1;
                                    ctr.VolMCtr = 0;
                                    ctr.DCCtr = DateTime.Now;
                                    ctr.StatCtr = "Non initié";
                                    ctr.SensCtr = "I";

                                    conteneurs.Add(ctr);
                                    #endregion
                                }
                                else if (line.Substring(327, 1).Equals("B"))
                                {
                                    // ajout Mafi
                                    #region mafi
                                    MAFI mafi = new MAFI();
                                    mafi.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    mafi.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    mafi.NumMafi = line.Substring(42, 17).Trim();
                                    mafi.DescMafi = line.Substring(60, 36).Trim();
                                    mafi.TypeMMafi = line.Substring(329, 20).Trim();
                                    mafi.TypeCMafi = line.Substring(329, 20).Trim();
                                    mafi.PoidsMMafi = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    mafi.PoidsCMafi = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    mafi.CatMseMafi = line.Substring(633, 2).Trim();
                                    mafi.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    mafi.InfoMan = line;
                                    //mafi.Seal1Ctr = line.Substring(350, 10).Trim();
                                    //mafi.Seal2Ctr = line.Substring(361, 10).Trim();
                                    mafi.IMDGCode = line.Substring(636, 5).Trim();
                                    mafi.DescMses = line.Substring(641, 134).Trim();
                                    //mafi.MCMafi = 6000000;
                                    //mafi.PropMafi = 1;
                                    mafi.VolMMafi = 0;
                                    mafi.DCMafi = DateTime.Now;
                                    mafi.StatMafi = "Non initié";
                                    mafi.SensMafi = "I";

                                    mafis.Add(mafi);
                                    #endregion
                                }
                                else if (line.Substring(42, 17).Trim() != "" && line.Substring(791, 1).Trim() != "")
                                {
                                    // ajout de véhicule
                                    #region vehicule
                                    VEHICULE veh = new VEHICULE();
                                    veh.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    veh.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    veh.NumChassis = line.Substring(42, 17).Trim();
                                    veh.DescVeh = line.Substring(60, 36).Trim();
                                    veh.TypeMVeh = line.Substring(273, 1).Trim();
                                    veh.TypeCVeh = line.Substring(273, 1).Trim();
                                    veh.StatutVeh = line.Substring(791, 1).Trim();
                                    veh.PoidsMVeh = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    veh.PoidsCVeh = Convert.ToInt32(line.Substring(275, 7).Trim());
                                    veh.VolMVeh = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    veh.VolCVeh = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    veh.LongMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    veh.LongCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    veh.LargMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    veh.LargCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    veh.HautMVeh = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    veh.HautCVeh = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    veh.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    veh.InfoMan = line;
                                    veh.BarCode = line.Substring(571, 30).Trim();
                                    veh.DCVeh = DateTime.Now;
                                    veh.StatVeh = "Non initié";
                                    veh.SensVeh = "I";
                                    veh.StatutCVeh = veh.StatutVeh;
                                    veh.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;

                                    vehicules.Add(veh);
                                    #endregion
                                }
                                else
                                {
                                    // ajout general cargo
                                    #region conventionel
                                    CONVENTIONNEL conv = new CONVENTIONNEL();
                                    conv.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim());
                                    conv.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == line.Substring(19, 17).Trim()).IdBL;
                                    conv.NumGC = line.Substring(42, 17).Trim();
                                    conv.DescGC = line.Substring(60, 36).Trim();
                                    conv.TypeMGC = "I404";
                                    conv.TypeCGC = "I404";
                                    conv.PoidsMGC = Math.Round(Convert.ToDouble(line.Substring(275, 7).Trim()) / 1000, 3);
                                    conv.PoidsCGC = Math.Round(Convert.ToDouble(line.Substring(275, 7).Trim()) / 1000, 3);
                                    conv.VolMGC = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    conv.VolCGC = Math.Round(Convert.ToDouble(line.Substring(283, 11).Trim()) / 1000, 3);
                                    conv.LongMGC = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    conv.LongCGC = (float)Math.Round(Convert.ToDouble(line.Substring(615, 4).Trim()) / 100, 3);
                                    conv.LargMGC = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    conv.LargCGC = (float)Math.Round(Convert.ToDouble(line.Substring(620, 4).Trim()) / 100, 3);
                                    conv.HautMGC = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    conv.HautCGC = (float)Math.Round(Convert.ToDouble(line.Substring(625, 4).Trim()) / 100, 3);
                                    conv.NumItem = Convert.ToInt16(line.Substring(37, 4).Trim());
                                    conv.InfoMan = line;
                                    conv.DCGC = DateTime.Now;
                                    conv.StatGC = "Non initié";
                                    conv.SensGC = "I";

                                    conventionnels.Add(conv);
                                    #endregion
                                }
                            }
                            line = file.ReadLine();
                        }

                        //file.Close();

                        dataGridConnaissement.ItemsSource = connaissements;
                        txtNbEffBL.Text = connaissements.Count.ToString();

                        dataGridVehicules.ItemsSource = vehicules;
                        txtNbEffVeh.Text = vehicules.Count.ToString();

                        dataGridCont.ItemsSource = conteneurs;
                        txtNbEffCont.Text = conteneurs.Count.ToString();

                        dataGridMafi.ItemsSource = mafis;
                        txtNbEffMafi.Text = mafis.Count.ToString();

                        dataGridGC.ItemsSource = conventionnels;
                        txtNbEffGC.Text = conventionnels.Count.ToString(); 
                        #endregion
                    }
                    else if (cbFormat.SelectedIndex == 2)
                    {
                        // vider les listes
                        #region Excel Véhicules Autres Acconiers (Excel xls)
                        dataGridConnaissement.ItemsSource = null;
                        dataGridVehicules.ItemsSource = null;
                        dataGridCont.ItemsSource = null;
                        dataGridMafi.ItemsSource = null;
                        dataGridGC.ItemsSource = null;

                        connaissements = new List<CONNAISSEMENT>();
                        vehicules = new List<VEHICULE>();
                        conteneurs = new List<CONTENEUR>();
                        mafis = new List<MAFI>();
                        conventionnels = new List<CONVENTIONNEL>();

                        excelLoadVehicule(txtCheminFichier.Text);

                        dataGridConnaissement.ItemsSource = connaissements;
                        txtNbEffBL.Text = connaissements.Count.ToString();

                        dataGridVehicules.ItemsSource = vehicules;
                        txtNbEffVeh.Text = vehicules.Count.ToString();

                        dataGridCont.ItemsSource = conteneurs;
                        txtNbEffCont.Text = conteneurs.Count.ToString();

                        dataGridMafi.ItemsSource = mafis;
                        txtNbEffMafi.Text = mafis.Count.ToString();

                        dataGridGC.ItemsSource = conventionnels;
                        txtNbEffGC.Text = conventionnels.Count.ToString(); 
                        #endregion
                    }
                    else if (cbFormat.SelectedIndex == 3)
                    {
                        // vider les listes
                        #region Excel Véhicules Autres Acconiers (Excel xlsx)
                        dataGridConnaissement.ItemsSource = null;
                        dataGridVehicules.ItemsSource = null;
                        dataGridCont.ItemsSource = null;
                        dataGridMafi.ItemsSource = null;
                        dataGridGC.ItemsSource = null;

                        connaissements = new List<CONNAISSEMENT>();
                        vehicules = new List<VEHICULE>();
                        conteneurs = new List<CONTENEUR>();
                        mafis = new List<MAFI>();
                        conventionnels = new List<CONVENTIONNEL>();

                        excelLoadVehicule(txtCheminFichier.Text);

                        dataGridConnaissement.ItemsSource = connaissements;
                        txtNbEffBL.Text = connaissements.Count.ToString();

                        dataGridVehicules.ItemsSource = vehicules;
                        txtNbEffVeh.Text = vehicules.Count.ToString();

                        dataGridCont.ItemsSource = conteneurs;
                        txtNbEffCont.Text = conteneurs.Count.ToString();

                        dataGridMafi.ItemsSource = mafis;
                        txtNbEffMafi.Text = mafis.Count.ToString();

                        dataGridGC.ItemsSource = conventionnels;
                        txtNbEffGC.Text = conventionnels.Count.ToString(); 
                        #endregion
                    }
                    else if (cbFormat.SelectedIndex == 4)
                    {
                        // vider les listes
                        #region Excel Manifeste Conteneurs NDS (Excel xlsx)
                        dataGridConnaissement.ItemsSource = null;
                        dataGridVehicules.ItemsSource = null;
                        dataGridCont.ItemsSource = null;
                        dataGridMafi.ItemsSource = null;
                        dataGridGC.ItemsSource = null;

                        connaissements = new List<CONNAISSEMENT>();
                        vehicules = new List<VEHICULE>();
                        conteneurs = new List<CONTENEUR>();
                        mafis = new List<MAFI>();
                        conventionnels = new List<CONVENTIONNEL>();

                        excelLoadConteneur(txtCheminFichier.Text);

                        dataGridConnaissement.ItemsSource = connaissements;
                        txtNbEffBL.Text = connaissements.Count.ToString();

                        dataGridVehicules.ItemsSource = vehicules;
                        txtNbEffVeh.Text = vehicules.Count.ToString();

                        dataGridCont.ItemsSource = conteneurs;
                        txtNbEffCont.Text = conteneurs.Count.ToString();

                        dataGridMafi.ItemsSource = mafis;
                        txtNbEffMafi.Text = mafis.Count.ToString();

                        dataGridGC.ItemsSource = conventionnels;
                        txtNbEffGC.Text = conventionnels.Count.ToString(); 
                        #endregion
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }

        private void excelLoadVehicule(string path)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                List<TYPE_VEHICULE> typesVeh = vsp.GetTypesVehicules();

                while (((string)(range.Cells[rCnt, 5] as Excel.Range).Value2) != null)
                {
                    //Connaissements
                    CONNAISSEMENT bl = new CONNAISSEMENT();
                    bl.NumBL = (string)(range.Cells[rCnt, 5] as Excel.Range).Value2;
                    if ((string)(range.Cells[rCnt, 12] as Excel.Range).Value2 != "CAMEROUN")
                    {
                        bl.BLIL = "Y";
                    }
                    else
                    {
                        bl.BLIL = "N";
                    }
                    bl.BLGN = "N";
                    bl.BlBloque = "N";
                    bl.BLER = "N";
                    bl.BLSocar = "N";
                    bl.SensBL = "I";
                    bl.CCBL = "N";
                    bl.TypeBL = "R";
                    bl.ConsigneeBL = (string)(range.Cells[rCnt, 21] as Excel.Range).Value2;
                    bl.AdresseBL = "";
                    bl.NotifyBL = (string)(range.Cells[rCnt, 22] as Excel.Range).Value2;
                    bl.LPBL = (string)(range.Cells[rCnt, 9] as Excel.Range).Value2;
                    bl.DPBL = (string)(range.Cells[rCnt, 10] as Excel.Range).Value2;
                    bl.DCBL = DateTime.Now;
                    bl.StatutBL = "Non initié";
                    bl.EtatBL = "O";
                    bl.CCBLMontant = 0;
                    bl.BLDette = "N";
                    bl.DetteMontant = 0;
                    bl.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;
                    bl.IdEsc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc;
                    bl.PoidsBL = 0;
                    bl.VolBL = 0;
                    bl.DestBL = "HINT";
                    bl.LastModif = DateTime.Now;
                    bl.IdU = utilisateur.IdU;
                    bl.NomCharger = "GRIMALDI";
                    bl.AdresseCharger = "GRIMALDI";

                    if (connaissements.FirstOrDefault<CONNAISSEMENT>(connaissement => connaissement.NumBL == bl.NumBL) == null)
                    {
                        connaissements.Add(bl);
                    }

                    // ajout de véhicule
                    VEHICULE veh = new VEHICULE();
                    veh.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == bl.NumBL);
                    veh.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == bl.NumBL).IdBL;
                    veh.NumChassis = Convert.ToString((range.Cells[rCnt, 6] as Excel.Range).Value2);
                    veh.DescVeh = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;
                    veh.StatutVeh = (string)(range.Cells[rCnt, 18] as Excel.Range).Value2;
                    if (veh.StatutVeh.Trim() == "")
                    {
                        veh.StatutVeh = "N";
                    }
                    veh.PoidsMVeh = Convert.ToInt32((double)(range.Cells[rCnt, 19] as Excel.Range).Value2);
                    veh.PoidsCVeh = veh.PoidsMVeh;
                    if ((double)(range.Cells[rCnt, 15] as Excel.Range).Value2 <= 1000)
                    {
                        veh.VolMVeh = Convert.ToDouble((double)(range.Cells[rCnt, 15] as Excel.Range).Value2);
                    }
                    else
                    {
                        veh.VolMVeh = Math.Round(Convert.ToDouble((double)(range.Cells[rCnt, 15] as Excel.Range).Value2) / 1000, 3);
                    }
                    veh.VolCVeh = veh.VolMVeh;
                    veh.LongMVeh = 0;
                    veh.LongCVeh = 0;
                    veh.LargMVeh = 0;
                    veh.LargCVeh = 0;
                    veh.HautMVeh = 0;
                    veh.HautCVeh = 0;
                    veh.NumItem = 1;
                    veh.InfoMan = "";
                    veh.BarCode = "";
                    veh.TypeMVeh = typesVeh.FirstOrDefault<TYPE_VEHICULE>(tp => tp.DeVol <= veh.VolMVeh && tp.LimVol > veh.VolMVeh).CodeTypeVeh;
                    veh.TypeCVeh = veh.TypeMVeh;
                    veh.DCVeh = DateTime.Now;
                    veh.StatVeh = "Non initié";
                    veh.SensVeh = "I";
                    veh.StatutCVeh = veh.StatutVeh;
                    veh.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;

                    vehicules.Add(veh);

                    rCnt++;
                }
                
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void excelLoadConteneur(string path)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                int rCnt = 2;

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                List<TYPE_CONTENEUR> typesCtr = vsp.GetTypesConteneurs();

                while (((string)(range.Cells[rCnt, 4] as Excel.Range).Value2) != null)
                {
                    //Connaissements
                    CONNAISSEMENT bl = new CONNAISSEMENT();
                    bl.NumBL = (string)(range.Cells[rCnt, 4] as Excel.Range).Value2;
                    if ((string)(range.Cells[rCnt, 13] as Excel.Range).Value2 != "DOUALA" && (string)(range.Cells[rCnt, 13] as Excel.Range).Value2 != "YAOUNDE" && (string)(range.Cells[rCnt, 13] as Excel.Range).Value2 != "BAFOUSSAM")
                    {
                        bl.BLIL = "Y";
                    }
                    else
                    {
                        bl.BLIL = "N";
                    }
                    bl.BLGN = "N";
                    bl.BlBloque = "N";
                    bl.BLER = "N";
                    bl.BLSocar = "N";
                    bl.SensBL = "I";
                    bl.CCBL = "N";
                    bl.TypeBL = "C";
                    bl.ConsigneeBL = (string)(range.Cells[rCnt, 3] as Excel.Range).Value2;
                    bl.AdresseBL = "";
                    bl.NotifyBL = bl.ConsigneeBL;
                    bl.LPBL = (string)(range.Cells[rCnt, 5] as Excel.Range).Value2;
                    bl.DPBL = "CMDLA";
                    bl.DCBL = DateTime.Now;
                    bl.StatutBL = "Non initié";
                    bl.EtatBL = "O";
                    bl.CCBLMontant = 0;
                    bl.BLDette = "N";
                    bl.DetteMontant = 0;
                    bl.IdAcc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).ACCONIER.IdAcc;
                    bl.IdEsc = escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc;
                    bl.PoidsBL = 0;
                    bl.VolBL = 0;
                    bl.DestBL = "HINT";
                    bl.LastModif = DateTime.Now;
                    bl.IdU = utilisateur.IdU;
                    bl.NomCharger = "GRIMALDI";
                    bl.AdresseCharger = "GRIMALDI";

                    if (connaissements.FirstOrDefault<CONNAISSEMENT>(connaissement => connaissement.NumBL == bl.NumBL) == null)
                    {
                        connaissements.Add(bl);
                    }

                    // ajout conteneur
                    CONTENEUR ctr = new CONTENEUR();
                    ctr.CONNAISSEMENT = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == bl.NumBL);
                    ctr.IdBL = connaissements.FirstOrDefault<CONNAISSEMENT>(con => con.NumBL == bl.NumBL).IdBL;
                    ctr.NumCtr = (string)(range.Cells[rCnt, 6] as Excel.Range).Value2;
                    ctr.DescCtr = (string)(range.Cells[rCnt, 11] as Excel.Range).Value2;
                    ctr.TypeMCtr = ((double)(range.Cells[rCnt, 9] as Excel.Range).Value2).ToString() + (((string)(range.Cells[rCnt, 14] as Excel.Range).Value2).Contains("FRIGO") ? "RF" : "DV");
                    ctr.TypeCCtr = ctr.TypeMCtr;
                    ctr.StatutCtr = (((string)(range.Cells[rCnt, 14] as Excel.Range).Value2).Trim().StartsWith("V") || ((string)(range.Cells[rCnt, 14] as Excel.Range).Value2).Trim().StartsWith("E")) ? "E" : "F";
                    ctr.PoidsMCtr = Convert.ToInt32(((double)(range.Cells[rCnt, 12] as Excel.Range).Value2) * 1000);
                    ctr.PoidsCCtr = ctr.PoidsMCtr;
                    ctr.CatMseCtr = "";
                    ctr.NumItem = 1;
                    ctr.InfoMan = "";
                    ctr.Seal1Ctr = (string)(range.Cells[rCnt, 7] as Excel.Range).Value2;
                    ctr.Seal2Ctr = (string)(range.Cells[rCnt, 8] as Excel.Range).Value2;
                    ctr.IMDGCode = "";
                    ctr.TypeMses = ctr.IMDGCode == "" ? "N" : "D";
                    ctr.DescMses = (string)(range.Cells[rCnt, 11] as Excel.Range).Value2;
                    ctr.MCCtr = 6000000;
                    ctr.PropCtr = 1;
                    ctr.VolMCtr = 0;
                    ctr.DCCtr = DateTime.Now;
                    ctr.StatCtr = "Non initié";
                    ctr.SensCtr = "I";

                    conteneurs.Add(ctr);

                    rCnt++;
                }

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet != null)
                {
                    releaseObject(xlWorkSheet);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                //MessageBox.Show("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void dataGridConnaissement_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbIdMan.IsEnabled == true && dataGridConnaissement.SelectedIndex != -1)
                {
                    CONNAISSEMENT con = vsp.GetConnaissementByIdBL(((CONNAISSEMENT)dataGridConnaissement.SelectedItem).IdBL);
                    if (con.StatutBL == "Del")
                    {
                        MessageBox.Show("Cet objet n'est pas disponible", "Escale");
                    }
                    else
                    {
                        ConnaissementForm conForm = new ConnaissementForm(this, con, utilisateur);
                        conForm.Show();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridVehicules_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbIdMan.IsEnabled == true && dataGridVehicules.SelectedIndex != -1)
                {
                    VEHICULE veh = vsp.GetVehiculeByIdVeh(((VEHICULE)dataGridVehicules.SelectedItem).IdVeh);
                    VehiculeForm vehForm = new VehiculeForm(this, veh, utilisateur);
                    vehForm.Show();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridCont_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbIdMan.IsEnabled == true && dataGridCont.SelectedIndex != -1)
                {
                    CONTENEUR ctr = vsp.GetConteneurImportByIdCtr(((CONTENEUR)dataGridCont.SelectedItem).IdCtr);
                    ConteneurForm ctrForm = new ConteneurForm(this, ctr, utilisateur);
                    ctrForm.Show();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cpteManifeste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            compteManTab.IsSelected = true;
        }

        private void cpteDIT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //compteDITTab.IsSelected = true;
        }

        private void txtRechercher_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return)
                {
                    if (cbFiltres.SelectedIndex != 11)
                    {
                        dataGridConnaissement.ItemsSource = vsp.GetConnaissementByNumBL(Convert.ToInt32(cbIdMan.Text), txtRechercher.Text);
                    }
                }
                else if (e.Key == Key.Escape)
                {
                    txtRechercher.Text = null;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbIdMan_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbIdMan_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbIdMan.Text.Trim() != "")
                {
                    int result;

                    manifestes = new List<MANIFESTE>();

                    MANIFESTE mn = vsp.GetManifesteByIdMan(Int32.TryParse(cbIdMan.Text.Trim(), out result) ? result : -1);

                    if (mn != null)
                    {
                        manifestes.Add(mn);
                    }

                    if (manifestes.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucun manifeste portant ce numéro", "Manifeste introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (manifestes.Count == 1)
                    {
                        MANIFESTE man = manifestes.FirstOrDefault<MANIFESTE>();
                        formLoader.LoadManifesteForm(this, man);
                    }
                    else
                    {
                        ListManifesteForm listManForm = new ListManifesteForm(this, manifestes, utilisateur);
                        listManForm.Title = "Choix multiples : Sélectionnez un manifeste";
                        listManForm.ShowDialog();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void listStatuts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listStatuts.SelectedIndex != -1)
                {
                    OPERATION_MANIFESTE opMan = (OPERATION_MANIFESTE)listStatuts.SelectedItem;
                    if (opMan.DateOp.HasValue)
                    {
                        StatutForm statutForm = new StatutForm(opMan);
                        statutForm.Title = "Statut - " + opMan.TYPE_OPERATION.LibTypeOp;
                        statutForm.ShowDialog();
                    }
                }

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridCompteMan_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                if (dataGridCompteMan.SelectedIndex != -1)
                {
                    ElementCompte eltCompte = (ElementCompte)dataGridCompteMan.SelectedItem;
                    if (eltCompte.TypeDoc == "FA")
                    {
                        FactureForm factForm = new FactureForm(this, vsp.GetFactureByIdFact(eltCompte.Id), utilisateur);
                        factForm.Show();
                    }
                    else if (eltCompte.TypeDoc == "PA")
                    {
                        PaiementForm payForm = new PaiementForm(this, vsp.GetPaiementByIdPay(eltCompte.Id), utilisateur);
                        payForm.btnEnregistrer.IsEnabled = false;
                        payForm.Show();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnAjoutNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NoteForm noteForm = new NoteForm("Nouveau", this, utilisateur);
                noteForm.Title = "Création de note - Manifeste " + cbPort.Text;
                noteForm.lblStatut.Content = "Note crée par : " + utilisateur.NU;
                noteForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void listNotes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (listNotes.SelectedIndex != -1)
                {
                    NOTE note = (NOTE)listNotes.SelectedItem;
                    NoteForm noteForm = new NoteForm(this, note, utilisateur);
                    noteForm.Title = "Note - " + note.IdNote + " - Manifeste - " + note.MANIFESTE.IdMan;
                    noteForm.lblStatut.Content = "Note crée le : " + note.DateNote + " par " + note.UTILISATEUR.NU;
                    noteForm.ShowDialog();
                }

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void dataGridGC_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                CONVENTIONNEL conv = vsp.GetConventionnelByIdGC(((CONVENTIONNEL)dataGridGC.SelectedItem).IdGC);
                if (dataGridGC.SelectedIndex != -1)
                {
                    ConventionnelForm convForm = new ConventionnelForm(this, conv, utilisateur);
                    convForm.Show();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnValidMan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ValiderManifesteForm validForm = new ValiderManifesteForm(this, utilisateur);
                validForm.Title = "Validation du manifeste : " + cbIdMan.Text + " - Escale " + cbNumEscale.Text;
                validForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbFormat.SelectedIndex == 5)
                {
                    btnSelectFile.IsEnabled = false;
                }
                else
                {
                    btnSelectFile.IsEnabled = true;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbFiltres_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbFiltres.SelectedIndex == 0)
                {
                    connaissements = vsp.GetConnaissementsImportByIdMan(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 1)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Non initié", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 2)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Initié", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 3)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Traité", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 4)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Manifesté", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 5)
                {
                    connaissements = vsp.GetConnaissementsAFacturer(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 6)
                {
                    connaissements = vsp.GetConnaissementPourEnlevementByIdMan(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 7)
                {
                    connaissements = vsp.GetConnaissementPourLivraisonByIdMan(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 8)
                {
                    connaissements = vsp.GetConnaissementPourSortieByIdMan(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 9)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Accompli", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 10)
                {
                    connaissements = vsp.GetConnaissementByStatutAndIdMan("Cloturé", Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
                else if (cbFiltres.SelectedIndex == 11)
                {
                    connaissements = vsp.GetConnaissementSOCARByIdMan(Convert.ToInt32(cbIdMan.Text));
                    dataGridConnaissement.ItemsSource = connaissements;
                    lblStatut.Content = connaissements.Count + " Connaissement(s)";
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void dataGridMafi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbIdMan.IsEnabled == true && dataGridMafi.SelectedIndex != -1)
                {
                    MAFI mf = vsp.GetMafiImportByIdMafi(((MAFI)dataGridMafi.SelectedItem).IdMafi);
                    MafiForm mafiForm = new MafiForm(this, mf, utilisateur);
                    mafiForm.Show();
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbNumEscale_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (e.Key == Key.Return && cbNumEscale.Text.Trim() != "")
                {
                    int result;
                    escales = vsp.GetEscalesByNumEscale(Int32.TryParse(cbNumEscale.Text.Trim(), out result) ? result : -1);

                    if (escales.Count == 0)
                    {
                        MessageBox.Show("Il n'existe aucune escale portant ce numéro", "Escale introuvable", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    }
                    else if (escales.Count == 1)
                    {
                        ESCALE esc = escales.FirstOrDefault<ESCALE>();
                        formLoader.LoadEscaleForm(this, esc);
                    }
                    else
                    {
                        ListEscaleForm listEscForm = new ListEscaleForm(this, escales, utilisateur);
                        listEscForm.Title = "Choix multiples : Sélectionnez une escale";
                        listEscForm.ShowDialog();
                    }
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }
    }
}

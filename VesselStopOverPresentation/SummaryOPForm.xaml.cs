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
using System.IO.Compression;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour SummaryOPForm.xaml
    /// </summary>
    public partial class SummaryOPForm : Window
    {
        private List<NAVIRE> navires;
        public List<string> navs { get; set; }

        private List<ACCONIER> acconiers;
        public List<string> accs { get; set; }

        private List<ARMATEUR> armateurs;
        public List<string> arms { get; set; }

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        public List<OPERATION_ARMATEUR> opArms { get; set; }
        public List<string> ops { get; set; }
        public List<ElementLigneOpArm> lignesOpArm { get; set; }

        public EscaleForm escForm { get; set; }
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;
        public StatutLoadUnload statutDechargement { get; set; }

        public List<ElementCompteEscale> eltsValeurEsc { get; set; }
        public List<ElementCompteDIT> eltsCompteDIT { get; set; }
        public List<ElementCompteSEPBC> eltsCompteSEPBC { get; set; }
        public List<ElementFacturation> eltsFact { get; set; }

        private string typeForm;

        private FormLoader formLoader;
        //private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public SummaryOPForm(EscaleForm form, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                InitializeCombos();

                //opArms = vsomAcc.GetOperationArmOfEscale(Convert.ToInt32(form.txtEscaleSysID.Text));
                //ops = new List<string>();
                //lignesOpArm = new List<ElementLigneOpArm>();

                escForm = form;
                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void InitializeCombos()
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsp = new VsomParameters();
                navires = vsomAcc.GetNaviresActifs();
                navs = new List<string>();
                foreach (NAVIRE nav in navires)
                {
                    navs.Add(nav.NomNav);
                }

                armateurs = vsomAcc.GetArmateursActifs();
                arms = new List<string>();
                foreach (ARMATEUR arm in armateurs)
                {
                    arms.Add(arm.NomArm);
                }

                acconiers = vsomAcc.GetAcconiersActifs();
                accs = new List<string>();
                foreach (ACCONIER acc in acconiers)
                {
                    accs.Add(acc.NomAcc);
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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                ESCALE esc = vsomAcc.UpdateSummaryOperations(escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc, dataGridEltOpArm.Items.OfType<ElementLigneOpArm>().ToList<ElementLigneOpArm>(), utilisateur.IdU);

                formLoader.LoadEscaleForm(this, esc);

                formLoader.LoadEscaleForm(escForm, esc);

                MessageBox.Show("Le summary of operations a été mis à jour avec succès", "Summary of operations mis à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
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

            }
        }

        private void cbNavire_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeNavire.Text = navires.ElementAt<NAVIRE>(cbNavire.SelectedIndex).CodeNav;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbArmateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeArmateur.Text = armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).CodeArm;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }

        }

        private void cbNumEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void dataGridEltOpArm_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dataGridEltOpArm.Items.Count != 0)
                {
                    ElementLigneOpArm elt = (ElementLigneOpArm)dataGridEltOpArm.SelectedItem;

                    cbOperation.SelectedItem = elt.Operation;
                    txtQte.Text = elt.Qte.ToString();
                    txtVolume.Text = elt.Volume.ToString();
                    txtPoids.Text = elt.Poids.ToString();
                    txtRemarques.Text = elt.Remarques;
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

        private void txtDim_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnMAJ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dataGridEltOpArm.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Veuillez sélectionner un ligne à modifier", "Ligne ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtQte.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la quantité", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtVolume.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le volume", "Volume ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (txtPoids.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le poids", "Poids ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    ElementLigneOpArm elt = (ElementLigneOpArm)dataGridEltOpArm.SelectedItem;

                    elt.Operation = cbOperation.Text;
                    elt.Qte = Convert.ToDouble(txtQte.Text.Replace(" ", "").Replace(".", ","));
                    elt.Volume = Convert.ToDouble(txtVolume.Text.Replace(" ", "").Replace(".", ","));
                    elt.Poids = Convert.ToDouble(txtPoids.Text.Replace(" ", "").Replace(".", ","));
                    if (elt.Operation == "Discharging General Cargo")
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Poids, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "Loading General Cargo")
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Poids, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "Discharging Pallets")
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Poids, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "Loading Sawn Timber")
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Volume, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "Sawn timber") // Karim
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Volume, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "Lashing operations") // Karim
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Volume, 0, MidpointRounding.AwayFromZero);
                    }
                    else if (elt.Operation == "General Cargo") // Karim
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Poids, 0, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        elt.PrixTotal = (int)Math.Round(elt.PrixUnitaire * elt.Qte, 0, MidpointRounding.AwayFromZero);
                    }
                    elt.Remarques = txtRemarques.Text;

                    //Valeur stevedoring operations
                    lblStatutStevedoring.Content = lignesOpArm.Sum(lg => lg.PrixTotal);

                    dataGridEltOpArm.ItemsSource = null;
                    System.Windows.Data.ListCollectionView collection = new System.Windows.Data.ListCollectionView(lignesOpArm);
                    collection.GroupDescriptions.Add(new System.Windows.Data.PropertyGroupDescription("StatutOp"));
                    dataGridEltOpArm.ItemsSource = collection;
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

        private void btnCloturer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //if (MessageBox.Show("Il ne sera plus possible d'apporter des modifications sur les informations du summary of operation, voulez-vous vraiment lancer cette clôture maintenant ?", "Clôture du summary of operation !", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                //{
                //    ESCALE esc = vsomAcc.CloturerSummaryOperations(escales.ElementAt<ESCALE>(cbNumEscale.SelectedIndex).IdEsc, utilisateur.IdU);

                //    formLoader.LoadEscaleForm(this, esc);

                //    formLoader.LoadEscaleForm(escForm, esc);

                //    MessageBox.Show("Le summary of operations a été clôturé avec succès et les éléments de facturation y afférant générés", "Summary of operations clôturé !", MessageBoxButton.OK, MessageBoxImage.Information);
                //}
                ClotureSOPForm clotureForm = new ClotureSOPForm(this, utilisateur);
                clotureForm.Title = "Clôture SOP - Escale N° : " + cbNumEscale.Text;
                clotureForm.ShowDialog();
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

        private void btnExtractExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                ESCALE esc = vsomAcc.GetEscaleById(Convert.ToInt32(txtEscaleSysID.Text));

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//ProformaStevedoring.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                (range.Cells[1, 2] as Excel.Range).Value2 = esc.ARMATEUR.NomArm;
                (range.Cells[3, 3] as Excel.Range).Value2 = esc.NAVIRE.NomNav;
                (range.Cells[5, 3] as Excel.Range).Value2 = "Douala";
                (range.Cells[3, 8] as Excel.Range).Value2 = esc.NumVoySCR;
                (range.Cells[5, 8] as Excel.Range).Value2 = esc.NumEsc;
                range.Range[range.Cells[3, 13], range.Cells[3, 16]].Merge();
                (range.Cells[3, 13] as Excel.Range).Value2 = esc.DRAEsc.HasValue ? esc.DRAEsc.Value.ToShortDateString() : esc.DPAEsc.Value.ToShortDateString();

                int i = 8;

                xlWorkSheet.Columns[9].NumberFormat = "0,000;@";
                xlWorkSheet.Columns[10].NumberFormat = "0,000;@";
                xlWorkSheet.Columns[11].NumberFormat = "# ###;@";
                xlWorkSheet.Columns[13].NumberFormat = "# ###;@";

                foreach (ElementLigneOpArm elt in dataGridEltOpArm.Items.OfType<ElementLigneOpArm>().Where(el => el.Qte != 0 && el.PrixUnitaire != 0).ToList<ElementLigneOpArm>())
                {
                    range.Range[range.Cells[i, 1], range.Cells[i, 6]].Merge();
                    range.Range[range.Cells[i, 7], range.Cells[i, 8]].Merge();
                    range.Range[range.Cells[i, 11], range.Cells[i, 12]].Merge();
                    range.Range[range.Cells[i, 13], range.Cells[i, 16]].Merge();
                    (range.Cells[i, 1] as Excel.Range).Value2 = elt.Operation;
                    //if (elt.Operation == "Discharging General Cargo")
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Poids;
                    //}
                    //else if (elt.Operation == "Loading General Cargo")
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Poids;
                    //}
                    //else if (elt.Operation == "Discharging Pallets")
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Poids;
                    //}
                    //else if (elt.Operation == "Loading Sawn Timber")
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Volume;
                    //}
                    //else if (elt.Operation == "Sawn timber") // Karim
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Volume;
                    //}
                    //else if (elt.Operation == "Lashing operations") // Karim
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Volume;
                    //}
                    //else if (elt.Operation == "General Cargo") // Karim
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Volume;
                    //}
                    //else
                    //{
                    //    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Qte;
                    //}
                    (range.Cells[i, 7] as Excel.Range).Value2 = elt.Qte;
                    (range.Cells[i, 9] as Excel.Range).Value2 = elt.Poids;
                    (range.Cells[i, 10] as Excel.Range).Value2 = elt.Volume;
                    (range.Cells[i, 11] as Excel.Range).Value2 = elt.PrixUnitaire;
                    (range.Cells[i, 13] as Excel.Range).Value2 = elt.PrixTotal;

                    i++;
                }

                (range.Cells[i, 10] as Excel.Range).Value2 = "TOTAL";
                (range.Cells[i, 13] as Excel.Range).Value2 = "=SUM(M8:M" + (i-1) + ")";
                (range.Cells[i, 10] as Excel.Range).Font.Bold = true;
                (range.Cells[i, 13] as Excel.Range).Font.Bold = true;

                range.Range[range.Cells[i, 10], range.Cells[i, 12]].Merge();
                range.Range[range.Cells[i, 13], range.Cells[i, 16]].Merge();
                range.Range[range.Cells[i, 10], range.Cells[i, 16]].Columns.Borders.LineStyle = 1;
                range.Range[range.Cells[8, 1], range.Cells[i-1, 16]].Columns.Borders.LineStyle = 1;

                xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Proforma Stevedoring - Escale - " + esc.NumEsc.ToString() + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                MessageBox.Show("Edition de la proforma de Stevedoring terminée", "Edition de la proforma de stevedoring terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}

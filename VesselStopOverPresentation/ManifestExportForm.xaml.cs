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
    /// Logique d'interaction pour ManifestExportForm.xaml
    /// </summary>
    public partial class ManifestExportForm : Window
    {

        private ExportControlPanel exportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ManifestExportForm(ExportControlPanel form, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                exportPanel = form;

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

        private void btnManifest_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                if (escales.Count > 0)
                {
                    VSOMAccessors vsomAcc = new VSOMAccessors();

                    ESCALE esc = escales.FirstOrDefault<ESCALE>();

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Export.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    (range.Cells[2, 4] as Excel.Range).Value2 = esc.NAVIRE.NomNav + " - " + esc.NumVoySCR;
                    (range.Cells[4, 7] as Excel.Range).Value2 = "Douala";
                    (range.Cells[4, 16] as Excel.Range).Value2 = esc.DRAEsc.HasValue ? esc.DRAEsc.Value : esc.DPAEsc.Value;

                    int i = 7;

                    List<CONNAISSEMENT> listBL = new List<CONNAISSEMENT>();

                    if (chkProvisoire.IsChecked == true)
                    {
                        listBL = esc.CONNAISSEMENT.Where(c => c.SensBL == "E").OrderBy(c => c.LDBL).ThenBy(c => c.LPBL).ThenBy(c => c.NumBL).ToList<CONNAISSEMENT>();
                    }
                    else
                    {
                        listBL = esc.CONNAISSEMENT.Where(c => c.DVCBLI.HasValue).OrderBy(c => c.LDBL).ThenBy(c => c.LPBL).ThenBy(c => c.NumBL).ThenBy(c => c.DVCBLI).ToList<CONNAISSEMENT>();
                    }

                    foreach (CONNAISSEMENT bl in listBL)
                    {
                        i++;
                        (range.Cells[i, 1] as Excel.Range).Value2 = "SH";
                        (range.Cells[i, 2] as Excel.Range).Value2 = bl.ConsigneeBL;
                        (range.Cells[i + 1, 2] as Excel.Range).Value2 = bl.AdresseBL;
                        (range.Cells[i + 2, 2] as Excel.Range).Value2 = bl.PhoneManBL;

                        (range.Cells[i + 3, 1] as Excel.Range).Value2 = "CO";
                        (range.Cells[i + 3, 2] as Excel.Range).Value2 = bl.ConsigneeBooking;
                        (range.Cells[i + 4, 2] as Excel.Range).Value2 = bl.AdresseConsignee;

                        (range.Cells[i + 5, 1] as Excel.Range).Value2 = "NF";
                        (range.Cells[i + 5, 2] as Excel.Range).Value2 = bl.NotifyBL;
                        (range.Cells[i + 6, 2] as Excel.Range).Value2 = bl.AdresseNotify;
                        (range.Cells[i + 7, 2] as Excel.Range).Value2 = bl.TelNotify;

                        (range.Cells[i, 4] as Excel.Range).Value2 = bl.NumBL;
                        (range.Cells[i, 3] as Excel.Range).Value2 = bl.LDBL;

                        int j = i;
                        i = i + 2;
                        //if (bl.TypeBL == "FF")
                        //{
                        //    foreach (CONTENEUR ctr in bl.CONTENEUR)
                        //    {
                        //        (range.Cells[i, 5] as Excel.Range).Value2 = ctr.TypeCCtr;
                        //        (range.Cells[i, 6] as Excel.Range).Value2 = ctr.NumCtr;
                        //        (range.Cells[i, 7] as Excel.Range).Value2 = ctr.Seal1Ctr;
                        //        (range.Cells[i, 8] as Excel.Range).Value2 = ctr.DescCtr;
                        //        (range.Cells[i, 9] as Excel.Range).Value2 = Math.Round((double)ctr.PoidsCCtr.Value / 1000, 3);
                        //        (range.Cells[i, 10] as Excel.Range).Value2 = ctr.VolMCtr;
                        //        (range.Cells[i, 11] as Excel.Range).Value2 = ctr.IMDGCode;
                        //        i++;
                        //    }
                        //    (range.Cells[j, 9] as Excel.Range).Value2 = bl.CONTENEUR.Sum(ctr => Math.Round((double)ctr.PoidsCCtr.Value / 1000, 3));
                        //    (range.Cells[j, 10] as Excel.Range).Value2 = bl.CONTENEUR.Sum(ctr => ctr.VolMCtr);
                        //}
                        //else if (bl.TypeBL == "BB")
                        //{
                        //    foreach (CONVENTIONNEL conv in bl.CONVENTIONNEL)
                        //    {
                        //        (range.Cells[i, 5] as Excel.Range).Value2 = conv.TypeCGC;
                        //        (range.Cells[i, 6] as Excel.Range).Value2 = conv.NumGC;
                        //        (range.Cells[i, 7] as Excel.Range).Value2 = conv.NumItem;
                        //        (range.Cells[i, 8] as Excel.Range).Value2 = conv.DescGCEmbarq;
                        //        (range.Cells[i, 9] as Excel.Range).Value2 = conv.PoidsCGC;
                        //        (range.Cells[i, 10] as Excel.Range).Value2 = conv.VolCGC;
                        //        i++;
                        //    }
                        //    (range.Cells[j, 9] as Excel.Range).Value2 = bl.CONVENTIONNEL.Sum(conv => conv.PoidsCGC.Value);
                        //    (range.Cells[j, 10] as Excel.Range).Value2 = bl.CONVENTIONNEL.Sum(conv => conv.VolCGC);
                        //}

                        //
                        foreach (CONTENEUR ctr in bl.CONTENEUR)
                        {
                            (range.Cells[i, 5] as Excel.Range).Value2 = ctr.TypeCCtr;
                            (range.Cells[i, 6] as Excel.Range).Value2 = ctr.NumCtr;
                            (range.Cells[i, 7] as Excel.Range).Value2 = ctr.Seal1Ctr;
                            (range.Cells[i, 8] as Excel.Range).Value2 = ctr.DescCtr;
                            (range.Cells[i, 9] as Excel.Range).Value2 = Math.Round((double)ctr.PoidsCCtr.Value / 1000, 3);
                            (range.Cells[i, 10] as Excel.Range).Value2 = ctr.VolMCtr;
                            (range.Cells[i, 11] as Excel.Range).Value2 = ctr.IMDGCode;
                            i++;
                        }

                        foreach (CONVENTIONNEL conv in bl.CONVENTIONNEL)
                        {
                            (range.Cells[i, 5] as Excel.Range).Value2 = conv.TypeCGC;
                            (range.Cells[i, 6] as Excel.Range).Value2 = conv.NumGC;
                            (range.Cells[i, 7] as Excel.Range).Value2 = conv.NumItem;
                            (range.Cells[i, 8] as Excel.Range).Value2 = conv.DescGCEmbarq;
                            (range.Cells[i, 9] as Excel.Range).Value2 = conv.PoidsCGC;
                            (range.Cells[i, 10] as Excel.Range).Value2 = conv.VolCGC;
                            i++;
                        }
                        (range.Cells[j, 9] as Excel.Range).Value2 = bl.CONVENTIONNEL.Sum(conv => conv.PoidsCGC.Value) + bl.CONTENEUR.Sum(ctr => Math.Round((double)ctr.PoidsCCtr.Value / 1000, 3));
                        (range.Cells[j, 10] as Excel.Range).Value2 = bl.CONVENTIONNEL.Sum(conv => conv.VolCGC) + bl.CONTENEUR.Sum(ctr => ctr.VolMCtr);
                        //

                        (range.Cells[j, 5] as Excel.Range).Value2 = "CT : " + bl.NumCtrBL + " / Ex: " + bl.NumDEBL + " / BESC: " + bl.NumBESCBL + " / HSCode: " + bl.NumHSCode;

                        //(range[16 + i, 1 + i] as Excel.Range).RowHeight = 5;
                        //(range[16 + i, 1 + i] as Excel.Range).Merge();
                        //(range[16 + i, 1 + i] as Excel.Range).Columns.Borders.LineStyle = 1;
                        //(range[16 + i, 1 + i] as Excel.Range).Columns.Borders.Weight = 2;

                        if (i < j + 8)
                        {
                            i += 5;
                        }
                        else
                        {
                            i += 2;
                        }

                    }

                    if (i > 10)
                    {
                        i++;
                        (range[16 + i, 1 + i] as Excel.Range).Font.Bold = true;
                        (range[16 + i, 1 + i] as Excel.Range).Font.Italic = true;
                        //(range[16 + i, 1 + i] as Excel.Range).Interior.Color = Excel.XlRgbColor.rgbRed;

                        //(range.Cells[i, 9] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(bl => bl.SensBL == "E").Sum(bl => bl.PoidsBL);
                        //(range.Cells[i, 10] as Excel.Range).Value2 = esc.CONNAISSEMENT.Where(bl => bl.SensBL == "E").Sum(bl => bl.VolBL);

                        (range.Cells[i, 9] as Excel.Range).Value2 = listBL.Sum(bl => bl.PoidsBL);
                        (range.Cells[i, 10] as Excel.Range).Value2 = listBL.Sum(bl => bl.VolBL);
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Manifeste - Export - Escale " + esc.NumEsc.ToString() + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Edition du manifeste export terminée", "Edition du manifeste terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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

        private void cbNumEscale_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        
    }
}

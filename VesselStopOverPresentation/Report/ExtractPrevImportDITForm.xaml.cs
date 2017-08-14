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
    /// Logique d'interaction pour ExtractPrevImportDITForm.xaml
    /// </summary>
    public partial class ExtractPrevImportDITForm : Window
    {

        private ReportingControlPanel reportControlPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ExtractPrevImportDITForm(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                reportControlPanel = panel;

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

        private void btnExtractPrevImport_Click(object sender, RoutedEventArgs e)
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
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Extract Prev Import DIT.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    int i = 2;

                    foreach (CONTENEUR ctr in esc.CONTENEUR.Where(c => c.SensCtr == "I").ToList<CONTENEUR>())
                    {
                        //(range.Cells[i, 1] as Excel.Range).Value2 = ctr.CONNAISSEMENT.LDBL;
                        (range.Cells[i, 1] as Excel.Range).Value2 = "CMDLA";
                        (range.Cells[i, 2] as Excel.Range).Value2 = ctr.CONNAISSEMENT.LPBL;
                        (range.Cells[i, 6] as Excel.Range).Value2 = ctr.NumCtr;
                        (range.Cells[i, 7] as Excel.Range).Value2 = ctr.TypeCCtr.Substring(0, 2);
                        (range.Cells[i, 8] as Excel.Range).Value2 = ctr.PoidsCCtr;
                        (range.Cells[i, 9] as Excel.Range).Value2 = ctr.TypeCCtr;
                        if (ctr.TypeCCtr == "20BX")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "22G1";
                        }
                        else if (ctr.TypeCCtr == "40BX")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "42G1";
                        }
                        else if (ctr.TypeCCtr == "40HC")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "45G1";
                        }
                        else if (ctr.TypeCCtr == "40OT")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "42U1";
                        }
                        else if (ctr.TypeCCtr == "40FL")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "45P3";
                        }
                        else if (ctr.TypeCCtr == "20OT")
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = "22U1";
                        }
                        (range.Cells[i, 11] as Excel.Range).Value2 = ctr.StatutCtr == "E" ? "MY" : "Full";
                        //(range.Cells[i, 16] as Excel.Range).Value2 = ctr.CONNAISSEMENT.DPBL;
                        (range.Cells[i, 16] as Excel.Range).Value2 = "CMDLA";
                        i++;
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Previsions - Import Ctr - Escale " + esc.NumEsc.ToString() + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Edition des prévisions d'import de conteneur terminée", "Edition des prévisions import conteneur terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

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
    /// Logique d'interaction pour DischargingReportExcelForm.xaml
    /// </summary>
    public partial class DischargingReportExcelForm : Window
    {
        public List<CONTENEUR> conteneurs { get; set; }

        private TrackingControlPanel trackingPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        public List<ESCALE> escales { get; set; }
        public List<Int32> escs { get; set; }

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DischargingReportExcelForm(TrackingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                trackingPanel = panel;

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

        private void btnExtraire_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
                {
                    MessageBox.Show("Vous n'avez pas les droits nécessaires pour éditer le rapport de embarquement. Veuillez contacter un administrateur", "Droits insuffisants !", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {

                    ESCALE esc = escales.FirstOrDefault<ESCALE>();

                    DateTime dte = DateTime.Now;

                    List<CONTENEUR> listCtrs = vsp.GetConteneursDebarques(esc.IdEsc);
                    
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//DischargingNDS.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    int i = 2;

                    foreach (CONTENEUR ctr in listCtrs)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = ctr.NumCtr;

                        if (ctr.TypeCCtr == "20BX")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "22G1";
                        }
                        else if (ctr.TypeCCtr == "40BX")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42G1";
                        }
                        else if (ctr.TypeCCtr == "40HC")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "45G1";
                        }
                        else if (ctr.TypeCCtr == "40OT")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42U1";
                        }
                        else if (ctr.TypeCCtr == "40FL")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "45P3";
                        }
                        else if (ctr.TypeCCtr == "20OT")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42U1";
                        }
                        else
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = ctr.TypeCCtr;
                        }

                        if (ctr.StatutCtr.StartsWith("F"))
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "DIF";
                        }
                        else if (ctr.StatutCtr.StartsWith("E"))
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "DIE";
                        }

                        (range.Cells[i, 4] as Excel.Range).Value2 = ctr.StatutCtr.Substring(0, 1);
                        (range.Cells[i, 5] as Excel.Range).Value2 = ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12).DateOp.Value.Year.ToString() + FormatChiffre(ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12).DateOp.Value.Month) + FormatChiffre(ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12).DateOp.Value.Day);

                        (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                        (range.Cells[i, 7] as Excel.Range).Value2 = "DIT DLA";
                        (range.Cells[i, 8] as Excel.Range).Value2 = "DLA";
                        (range.Cells[i, 9] as Excel.Range).Value2 = "";
                        (range.Cells[i, 10] as Excel.Range).Value2 = "DLA";
                        (range.Cells[i, 11] as Excel.Range).Value2 = "DLA";
                        (range.Cells[i, 12] as Excel.Range).Value2 = ctr.ESCALE.NAVIRE.CodeNDS;
                        (range.Cells[i, 13] as Excel.Range).Value2 = ctr.ESCALE.NumVoySCR;

                        i++;
                    }
                    range.Range[range.Cells[1, 1], range.Cells[i - 1, 16]].Columns.Borders.LineStyle = 1;

                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Discharging Report - " + cbNumEscale.Text + ".xlsx";
                    dlg.DefaultExt = ".xlsx"; // Default file extension
                    dlg.Filter = "Excel Documents (.xlsx)|*.xlsx"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    string filename = "";
                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        filename = dlg.FileName;

                        xlWorkBook.SaveAs(filename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        MessageBox.Show("Discharging Report NDS édité avec succès", "Discharging Report édité !", MessageBoxButton.OK, MessageBoxImage.Information);
                        this.Close();
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

        private static string FormatChiffre(int entier)
        {
            Int32 i = entier;
            if (i >= 10)
            {
                return i.ToString();
            }
            else
            {
                return "0" + i.ToString();
            }
        }

    }
}

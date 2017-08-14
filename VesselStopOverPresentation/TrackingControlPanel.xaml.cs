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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VesselStopOverData;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Drawing;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour TrackingControlPanel.xaml
    /// </summary>
    public partial class TrackingControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc;
        //VsomParameters vsp = new VsomParameters();

        public TrackingControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
             vsomAcc=new VSOMAccessors();
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Conteneur TC : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Mise à disposition : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnMiseDisposition.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Bilan Mise à disposition").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBilanMiseDisposition.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport debarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRapportDeb.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRapportEmb.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport debarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRapportDebExcel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnRapportEmbExcel.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport debarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDischargingReportNDS.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Rapport embarquement").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnLoadingReportNDS.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Stock conteneurs").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnStockConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Bilan Opérations Tracking").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnBilanTracking.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : Daily Moves").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnDailyMoves.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Report : WAF Report").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnWAFReport.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Importation opérations DIT").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnImportDIT.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnConteneur_Click(object sender, RoutedEventArgs e)
        {
            ConteneurTCPanel ctrPanel = new ConteneurTCPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Conteneur";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = ctrPanel;
        }

        private void btnMiseDisposition_Click(object sender, RoutedEventArgs e)
        {
            MiseDispositionPanel dispoCtrPanel = new MiseDispositionPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Mise à disposition de conteneur";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = dispoCtrPanel;
        }

        private void btnRapportDeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RapportDebarquementForm rapDebForm = new RapportDebarquementForm(this, utilisateur);
                rapDebForm.Title = "Rapport débarquement";
                rapDebForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRapportEmb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RapportEmbarquementForm rapEmbForm = new RapportEmbarquementForm(this, utilisateur);
                rapEmbForm.Title = "Rapport embarquement";
                rapEmbForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDailyMoves_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DailyMovesForm dailyMovesForm = new DailyMovesForm(this, utilisateur);
                dailyMovesForm.Title = "Daily Moves";
                dailyMovesForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnImportDIT_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            int rCnt = 7;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsm = new VsomMarchal();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Infos DIT (*.xls)|*.xls|Infos DIT (*.xlsx)|*.xlsx";
                Nullable<bool> result = dlg.ShowDialog();
                string filename = "";
                if (result == true)
                {
                    filename = dlg.FileName;
                }
                else
                {
                    return;
                }

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    StringBuilder sb = new StringBuilder();
                    string ophour = string.Empty;
                    DateTime tempDate;
                    for (rCnt = 7; (rCnt < range.Cells.Rows.Count && (range.Cells[rCnt, 6] as Excel.Range).Value2 != null); rCnt++)
                    {

                        OperationDIT opDIT = new OperationDIT();

                        opDIT.NumCtr = ((string)(range.Cells[rCnt, 6] as Excel.Range).Value2).Trim();
                        //opDIT.DateOp = (DateTime)(range.Cells[rCnt, 1] as Excel.Range).Value2;
                        //opDIT.DateOp = DateTime.FromOADate((double)(range.Cells[rCnt, 1] as Excel.Range).Value2);
                        if ((range.Cells[rCnt, 1] as Excel.Range).MergeCells)
                        {
                            opDIT.DateOp = DateTime.ParseExact(((Excel.Range)(range.Cells[rCnt, 1] as Excel.Range).MergeArea[1, 1]).Value2 /* + " " + (range.Cells[rCnt, 17] as Excel.Range).Value2*/, "dd/MM/yyyy"/*"dd/MM/yyyy hh:mm:ss"*/, System.Globalization.CultureInfo.InvariantCulture);
                            ophour = string.Format("{0} {1}",opDIT.DateOp.ToShortDateString(),
                                                  ((string)(range.Cells[rCnt, 16] as Excel.Range).Value2).Trim());

                            try { tempDate = DateTime.Parse(ophour); opDIT.DateOp = DateTime.Parse(ophour); }
                            catch (Exception ex) { ophour = opDIT.DateOp.ToShortDateString() + " 05:59:59"; opDIT.DateOp = DateTime.Parse(ophour); }
                        }
                        if ((range.Cells[rCnt, 5] as Excel.Range).MergeCells)
                        {
                            opDIT.CodeOperation = ((string)((Excel.Range)(range.Cells[rCnt, 5] as Excel.Range).MergeArea[1, 1]).Value2).Trim();
                        }
                        //if ((range.Cells[rCnt, 4] as Excel.Range).MergeCells)
                        //{
                        //    opDIT.Cycle = ((string)((Excel.Range)(range.Cells[rCnt, 4] as Excel.Range).MergeArea[1, 1]).Value2).Trim();
                        //}
                        //opDIT.Plomb = (string)(range.Cells[rCnt, 14] as Excel.Range).Value2;
                        //opDIT.Observations = (string)(range.Cells[rCnt, 13] as Excel.Range).Value2;

                        CONTENEUR_TC ctr = vsomAcc.UpdateInfosTrackingDIT(opDIT, utilisateur.IdU);

                        if (ctr != null)
                        {
                            sb.Append("Mise à jour OK : Ligne - ").Append(rCnt.ToString()).Append(" ").Append(ctr.CONTENEUR.ESCALE.NumEsc).Append(" ").Append(ctr.NumTC).Append(Environment.NewLine);
                            //range.Range[range.Cells[rCnt, 1], range.Cells[rCnt, 17]].Interior.Color = System.Drawing.Color.LightGreen;
                        }
                        else
                        {
                            sb.Append("Mise à jour KO : Ligne - ").Append(rCnt.ToString()).Append(" ").Append(opDIT.NumCtr).Append(Environment.NewLine);
                            //range.Range[range.Cells[rCnt, 1], range.Cells[rCnt, 17]].Interior.Color = System.Drawing.Color.Magenta;
                        }
                    }

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log Importation Tracking DIT.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    //xlWorkBook.SaveAs(parentDir.FullName + "\\Tracking DIT Report.xls", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(rCnt + "\n" + ex.Message + "\n" + ex.StackTrace);
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

        private void btnBilanMiseDisposition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatBilanMiseDispositionReport elementsBilanMADReport = new EtatBilanMiseDispositionReport(this);
                vesselWindow.mainPanel.Header = "Bilan Mise à disposition";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = elementsBilanMADReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnStockConteneur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatStockConteneurReport stockConteneurReport = new EtatStockConteneurReport(this);
                vesselWindow.mainPanel.Header = "Stock de conteneurs";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = stockConteneurReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnBilanTracking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EtatBilanTrackingReport bilanTrackingReport = new EtatBilanTrackingReport(this);
                vesselWindow.mainPanel.Header = "Bilan Tracking";
                vesselWindow.mainPanel.Content = null;
                vesselWindow.mainPanel.Content = bilanTrackingReport;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRapportDebExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RapportDebarquementExcelForm rapdEBExcelForm = new RapportDebarquementExcelForm(this, utilisateur);
                rapdEBExcelForm.Title = "Rapport débarquement (Excel)";
                rapdEBExcelForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnRapportEmbExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RapportEmbarquementExcelForm rapEmbExcelForm = new RapportEmbarquementExcelForm(this, utilisateur);
                rapEmbExcelForm.Title = "Rapport embarquement (Excel)";
                rapEmbExcelForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnDailyMovesExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DailyMovesFormExcel dailyMovesExcelForm = new DailyMovesFormExcel(this, utilisateur);
                dailyMovesExcelForm.Title = "Daily Moves (Excel)";
                dailyMovesExcelForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnWAFReport_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                xlApp = new Excel.Application();
                xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//WAF Report.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                range = xlWorkSheet.UsedRange;

                (range.Cells[4, 2] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20DV").Count;
                (range.Cells[4, 3] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20OT").Count;
                (range.Cells[4, 4] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20VT").Count;
                (range.Cells[4, 5] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20FR").Count;
                (range.Cells[4, 6] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20PL").Count;
                (range.Cells[4, 7] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "20RF").Count;
                (range.Cells[4, 8] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40DV").Count;
                (range.Cells[4, 9] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40OT").Count;
                (range.Cells[4, 10] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40HC").Count;
                (range.Cells[4, 11] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40FL").Count;
                (range.Cells[4, 12] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40FR").Count;
                (range.Cells[4, 13] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Débarqué", "40RF").Count;
                //(range.Cells[4, 14] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutForWAF("Débarqué").Count;

                (range.Cells[5, 2] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20DV").Count;
                (range.Cells[5, 3] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20OT").Count;
                (range.Cells[5, 4] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20VT").Count;
                (range.Cells[5, 5] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20FR").Count;
                (range.Cells[5, 6] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20PL").Count;
                (range.Cells[5, 7] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "20RF").Count;
                (range.Cells[5, 8] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40DV").Count;
                (range.Cells[5, 9] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40OT").Count;
                (range.Cells[5, 10] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40HC").Count;
                (range.Cells[5, 11] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40FL").Count;
                (range.Cells[5, 12] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40FR").Count;
                (range.Cells[5, 13] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Sorti", "40RF").Count;
                //(range.Cells[5, 14] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutForWAF("Sorti").Count;

                (range.Cells[10, 2] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20DV").Count;
                (range.Cells[10, 3] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20OT").Count;
                (range.Cells[10, 4] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20VT").Count;
                (range.Cells[10, 5] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20FR").Count;
                (range.Cells[10, 6] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20PL").Count;
                (range.Cells[10, 7] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "20RF").Count;
                (range.Cells[10, 8] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40DV").Count;
                (range.Cells[10, 9] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40OT").Count;
                (range.Cells[10, 10] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40HC").Count;
                (range.Cells[10, 11] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40FL").Count;
                (range.Cells[10, 12] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40FR").Count;
                (range.Cells[10, 13] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Mis à disposition", "40RF").Count;
                //(range.Cells[10, 14] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutForWAF("Mis à disposition").Count;

                (range.Cells[11, 2] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20DV").Count;
                (range.Cells[11, 3] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20OT").Count;
                (range.Cells[11, 4] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20VT").Count;
                (range.Cells[11, 5] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20FR").Count;
                (range.Cells[11, 6] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20PL").Count;
                (range.Cells[11, 7] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "20RF").Count;
                (range.Cells[11, 8] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40DV").Count;
                (range.Cells[11, 9] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40OT").Count;
                (range.Cells[11, 10] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40HC").Count;
                (range.Cells[11, 11] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40FL").Count;
                (range.Cells[11, 12] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40FR").Count;
                (range.Cells[11, 13] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutAndTypeForWAF("Cargo Loading", "40RF").Count;
                //(range.Cells[11, 14] as Excel.Range).Value2 = vsomAcc.GetConteneurTCsByStatutForWAF("Cargo Loading").Count;


                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\WAF Report - " + DateTime.Now.ToShortDateString().Replace("/", "") + ".xlsx";
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
                    MessageBox.Show("Edition du WAF Report terminée", "Edition du WAF Report terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void btnDischargingReportNDS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DischargingReportExcelForm dischargingReportExcelForm = new DischargingReportExcelForm(this, utilisateur);
                dischargingReportExcelForm.Title = "Discharging Report (NDS)";
                dischargingReportExcelForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnLoadingReportNDS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadingReportExcelForm loadingReportExcelForm = new LoadingReportExcelForm(this, utilisateur);
                loadingReportExcelForm.Title = "Loading Report (NDS)";
                loadingReportExcelForm.ShowDialog();
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

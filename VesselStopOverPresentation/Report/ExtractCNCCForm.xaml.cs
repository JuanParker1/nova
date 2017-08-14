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
    /// Logique d'interaction pour ExtractCNCCForm.xaml
    /// </summary>
    public partial class ExtractCNCCForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public ExtractCNCCForm(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                utilisateur = user;
                operationsUser = vsp.GetOperationsUtilisateur(utilisateur.IdU);

                reportPanel = panel;

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

        private void btnExtractCNCC_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (!txtDateDebut.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de début", "Date début ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateFin.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de fin", "Date fin ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    List<VEHICULE> listVehicules = vsp.GetVehiculesExtractSortiesCNCC(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Extract CNCC.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    int i = 2;

                    xlWorkSheet.Columns[2].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[3].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[4].NumberFormat = "[$-00040C]jj/mm/aaaa hh:mm:ss;@";
                    xlWorkSheet.Columns[5].NumberFormat = "[$-00040C]jj/mm/aaaa hh:mm:ss;@";
                    xlWorkSheet.Columns[6].NumberFormat = "[$-00040C]jj/mm/aaaa hh:mm:ss;@";
                    xlWorkSheet.Columns[7].NumberFormat = "[$-00040C]jj/mm/aaaa hh:mm:ss;@";

                    foreach (VEHICULE veh in listVehicules)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = "'" + veh.NumChassis;
                        if (veh.ESCALE.DRAEsc.HasValue)
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = veh.ESCALE.DRAEsc.Value;
                        }
                        if (veh.ESCALE.DDechEsc.HasValue)
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = veh.ESCALE.DDechEsc.Value;
                        }
                        (range.Cells[i, 4] as Excel.Range).Value2 = veh.OPERATION_VEHICULE.FirstOrDefault<OPERATION_VEHICULE>(op => op.IdTypeOp == 1).DateOp.Value;
                        if (veh.IdDBL.HasValue)
                        {
                            (range.Cells[i, 5] as Excel.Range).Value2 = veh.DEMANDE_LIVRAISON.DateDBL.Value;
                        }
                        (range.Cells[i, 6] as Excel.Range).Value2 = veh.BON_SORTIE.DateBS.Value;
                        (range.Cells[i, 7] as Excel.Range).Value2 = veh.OPERATION_VEHICULE.FirstOrDefault<OPERATION_VEHICULE>(op => op.IdTypeOp == 10).DateOp.Value;
                        (range.Cells[i, 8] as Excel.Range).Value2 = "'" + veh.CONNAISSEMENT.NumBL;
                        (range.Cells[i, 9] as Excel.Range).Value2 = veh.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 10] as Excel.Range).Value2 = "'" + veh.ESCALE.NumVoySCR;
                        (range.Cells[i, 11] as Excel.Range).Value2 = veh.StatutVeh == "U" ? "USAGE" : "NEUF";
                        (range.Cells[i, 12] as Excel.Range).Value2 = veh.DescVeh;
                        
                        i++;
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Extract Véhicules sortis du - " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Extract des véhicules sortis du parc auto pour CNCC", "Edition de l'extract des véhicules terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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

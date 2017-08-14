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
    /// Logique d'interaction pour ExtractManutForm.xaml
    /// </summary>
    public partial class ExtractManutForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ExtractManutForm(ReportingControlPanel panel, UTILISATEUR user)
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

        private void btnExtractManut_Click(object sender, RoutedEventArgs e)
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
                    List<VEHICULE> listVehicules = vsp.GetVehiculesExtractManutAutresAcconiers(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value, chkRoulant.IsChecked.Value);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Manutention Parc autos.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    int i = 2;

                    xlWorkSheet.Columns[2].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[7].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[13].NumberFormat = "[$-00040C]jj/mm/aaaa;@";

                    foreach (VEHICULE veh in listVehicules)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = veh.ESCALE.NumManifestSydonia;
                        (range.Cells[i, 2] as Excel.Range).Value2 = veh.ESCALE.DRAEsc.Value;
                        (range.Cells[i, 3] as Excel.Range).Value2 = veh.ESCALE.DRAEsc.Value.Year;
                        (range.Cells[i, 4] as Excel.Range).Value2 = veh.ESCALE.NAVIRE.CodeNav;
                        (range.Cells[i, 5] as Excel.Range).Value2 = veh.CONNAISSEMENT.NumBL;
                        (range.Cells[i, 6] as Excel.Range).Value2 = veh.NumChassis;
                        (range.Cells[i, 7] as Excel.Range).Value2 = veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3) != null ? veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3).DateOp.Value.ToString("dd/MM/yyyy") : "";
                        (range.Cells[i, 8] as Excel.Range).Value2 = veh.DescVeh;
                        (range.Cells[i, 9] as Excel.Range).Value2 = veh.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 10] as Excel.Range).Value2 = veh.CONNAISSEMENT.LPBL;
                        (range.Cells[i, 11] as Excel.Range).Value2 = veh.CONNAISSEMENT.DPBL;
                        (range.Cells[i, 12] as Excel.Range).Value2 = veh.CONNAISSEMENT.BLIL == "Y" ? "HINTERLAND" : (veh.CONNAISSEMENT.BLGN == "Y" ? "GRAND NORD" : "CAMEROUN");
                        (range.Cells[i, 13] as Excel.Range).Value2 = veh.FFVeh.Value;
                        (range.Cells[i, 14] as Excel.Range).Value2 = veh.TypeCVeh;
                        (range.Cells[i, 15] as Excel.Range).Value2 = veh.VolCVeh.Value;
                        (range.Cells[i, 16] as Excel.Range).Value2 = veh.ESCALE.ACCONIER.CodeAcc;
                        (range.Cells[i, 17] as Excel.Range).Value2 = veh.ESCALE.NumEsc;
                        (range.Cells[i, 18] as Excel.Range).Value2 = veh.StatutCVeh;
                        (range.Cells[i, 19] as Excel.Range).Value2 = veh.PoidsCVeh;
                        (range.Cells[i, 20] as Excel.Range).Value2 = "";
                        (range.Cells[i, 21] as Excel.Range).Value2 = veh.CONNAISSEMENT.ConsigneeBL;
                        (range.Cells[i, 22] as Excel.Range).Value2 = veh.CONNAISSEMENT.NotifyBL;
                        (range.Cells[i, 23] as Excel.Range).Value2 = veh.CONNAISSEMENT.BLIL == "Y" ? "HINTERLAND" : (veh.CONNAISSEMENT.BLGN == "Y" ? "GRAND NORD" : "CAMEROUN");
                        (range.Cells[i, 24] as Excel.Range).Value2 = veh.ESCALE.NumVoySCR;

                        List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();

                        if (occupations.Count != 0)
                        {
                            (range.Cells[i, 25] as Excel.Range).Value2 = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc;
                            (range.Cells[i, 26] as Excel.Range).Value2 = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
                        }
                        
                        i++;
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Extract Manutention parc auto du - " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Extract des véhicules du parc auto pour manutention terminée", "Edition de l'extract des véhicules terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}

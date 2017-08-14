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
    /// Logique d'interaction pour ExtractSejourForm.xaml
    /// </summary>
    public partial class ExtractSejourForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ExtractSejourForm(ReportingControlPanel panel, UTILISATEUR user)
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

        private void btnExtractSejour_Click(object sender, RoutedEventArgs e)
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
                    List<VEHICULE> listVehicules = vsp.GetVehiculesExtractSejourAutresAcconiers(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value, chkRoulant.IsChecked.Value);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Sejour Parc Autos.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    int i = 2;

                    xlWorkSheet.Columns[12].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[13].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[20].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    foreach (VEHICULE veh in listVehicules)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = veh.NumChassis;
                        (range.Cells[i, 2] as Excel.Range).Value2 = veh.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 3] as Excel.Range).Value2 = veh.ESCALE.NumVoySCR;
                        (range.Cells[i, 4] as Excel.Range).Value2 = veh.CONNAISSEMENT.NumBL;
                        (range.Cells[i, 5] as Excel.Range).Value2 = veh.DescVeh;
                        (range.Cells[i, 6] as Excel.Range).Value2 = veh.VolCVeh;
                        (range.Cells[i, 7] as Excel.Range).Value2 = veh.PoidsCVeh;
                        (range.Cells[i, 8] as Excel.Range).Value2 = veh.TypeCVeh;
                        (range.Cells[i, 9] as Excel.Range).Value2 = veh.MANIFESTE.PORT.CodePort;
                        (range.Cells[i, 10] as Excel.Range).Value2 = veh.StatutCVeh;
                        (range.Cells[i, 11] as Excel.Range).Value2 = veh.ESCALE.ACCONIER.NomAcc;
                        (range.Cells[i, 12] as Excel.Range).Value2 = veh.ESCALE.DRAEsc.Value;
                        if(veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3) != null)
                        {
                            (range.Cells[i, 13] as Excel.Range).Value2 = veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3).DateOp.Value;
                        }
                        else
                        {
                            (range.Cells[i, 13] as Excel.Range).Value2 = "";
                        }
                        (range.Cells[i, 14] as Excel.Range).Value2 = "";
                        (range.Cells[i, 15] as Excel.Range).Value2 = "";
                        (range.Cells[i, 16] as Excel.Range).Value2 = veh.NumSydoniaVeh;
                        (range.Cells[i, 17] as Excel.Range).Value2 = veh.NumDDVeh;
                        (range.Cells[i, 18] as Excel.Range).Value2 = veh.ESCALE.NumEsc;
                        (range.Cells[i, 19] as Excel.Range).Value2 = veh.IdBS.Value;
                        (range.Cells[i, 20] as Excel.Range).Value2 = veh.DSRVeh.Value;
                        (range.Cells[i, 21] as Excel.Range).Value2 = veh.CONNAISSEMENT.BLIL == "Y" ? "HINTERLAND" : (veh.CONNAISSEMENT.BLGN == "Y" ? "GRAND NORD" : "CAMEROUN");
                        (range.Cells[i, 22] as Excel.Range).Value2 = veh.CONNAISSEMENT.ConsigneeBL;
                        (range.Cells[i, 23] as Excel.Range).Value2 = veh.NomEnVeh;
                        (range.Cells[i, 24] as Excel.Range).Value2 = veh.CNIEnVeh;

                        OCCUPATION occup = veh.OCCUPATION.Skip(veh.OCCUPATION.Count -1).FirstOrDefault<OCCUPATION>();

                        if (occup != null)
                        {
                            (range.Cells[i, 25] as Excel.Range).Value2 = occup.EMPLACEMENT.PARC.NomParc;
                            (range.Cells[i, 26] as Excel.Range).Value2 = occup.EMPLACEMENT.LigEmpl + occup.EMPLACEMENT.ColEmpl;
                        }
                        
                        i++;
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Extract Sejour parc auto du - " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Extract des véhicules du parc auto pour séjour terminée", "Edition de l'extract des véhicules terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

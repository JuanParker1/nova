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
    /// Logique d'interaction pour ExtractParcAutoExcelForm.xaml
    /// </summary>
    public partial class ExtractParcAutoExcelForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<PARC> parcs;
        public List<string> prcs { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public ExtractParcAutoExcelForm(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                parcs = vsp.GetParcs("V");
                prcs = new List<string>();
                foreach (PARC p in parcs)
                {
                    prcs.Add(p.NomParc);
                }

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

        private void btnExtractParcAutoExcel_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbParc.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un parc", "Parc ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    List<VEHICULE> listVehicules = vsp.GetVehiculesExtractParques(parcs.ElementAt<PARC>(cbParc.SelectedIndex).IdParc);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//Extract Parc Auto.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    int i = 2;

                    xlWorkSheet.Columns[2].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    xlWorkSheet.Columns[6].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    foreach (VEHICULE veh in listVehicules)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = veh.ESCALE.NumEsc;
                        if(veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3) != null)
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = veh.OPERATION_VEHICULE.FirstOrDefault(op => op.IdTypeOp == 3).DateOp.Value;
                        }
                        (range.Cells[i, 3] as Excel.Range).Value2 = veh.NumChassis;
                        (range.Cells[i, 4] as Excel.Range).Value2 = veh.ESCALE.NAVIRE.CodeNav;
                        (range.Cells[i, 5] as Excel.Range).Value2 = veh.ESCALE.NumVoySCR;
                        if (veh.ESCALE.DRAEsc.HasValue)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = veh.ESCALE.DRAEsc.Value;
                        }
                        (range.Cells[i, 7] as Excel.Range).Value2 = veh.CONNAISSEMENT.NumBL;
                        (range.Cells[i, 8] as Excel.Range).Value2 = veh.DescVeh;
                        (range.Cells[i, 9] as Excel.Range).Value2 = veh.CONNAISSEMENT.LPBL;
                        (range.Cells[i, 10] as Excel.Range).Value2 = veh.ESCALE.ACCONIER.NomAcc;
                        (range.Cells[i, 11] as Excel.Range).Value2 = veh.CONNAISSEMENT.ConsigneeBL;
                        (range.Cells[i, 12] as Excel.Range).Value2 = veh.CONNAISSEMENT.BLIL == "Y" ? "HINTERLAND" : (veh.CONNAISSEMENT.BLGN == "Y" ? "GRAND NORD" : "CAMEROUN");

                        List<OCCUPATION> occupations = veh.OCCUPATION.Where(occ => !occ.DateFin.HasValue).ToList<OCCUPATION>();

                        if (occupations.Count != 0 && !veh.IdVehAP.HasValue)
                        {
                            (range.Cells[i, 13] as Excel.Range).Value2 = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.PARC.NomParc;
                            (range.Cells[i, 14] as Excel.Range).Value2 = occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.LigEmpl + occupations.FirstOrDefault<OCCUPATION>().EMPLACEMENT.ColEmpl;
                        }
                        else if (veh.IdVehAP.HasValue)
                        {
                            (range.Cells[i, 13] as Excel.Range).Value2 = "";
                            (range.Cells[i, 14] as Excel.Range).Value2 = vsp.GetPosVeh(veh.IdVeh);
                            range.Range[range.Cells[i, 1], range.Cells[i, 14]].Interior.Color = System.Drawing.Color.LightGreen;
                        }

                        i++;
                    }

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Extract Stock - " + cbParc.Text + " " + DateTime.Now.ToShortDateString().Replace("/", "") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Extract des véhicules au parc auto terminée", "Edition de l'extract des véhicules parc auto terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

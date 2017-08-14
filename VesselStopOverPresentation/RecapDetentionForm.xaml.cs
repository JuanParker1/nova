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
    /// Logique d'interaction pour RecapDetentionForm.xaml
    /// </summary>
    public partial class RecapDetentionForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public RecapDetentionForm(ReportingControlPanel panel, UTILISATEUR user)
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

        private void btnRecapDetention_Click(object sender, RoutedEventArgs e)
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
                    List<CONTENEUR> listConteneurs = vsp.GetConteneursPourRecapDetention(txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//RecapDetention.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;
                    int i = 2;

                    if (System.Globalization.CultureInfo.CurrentCulture.Name == "en-US")
                    {
                        xlWorkSheet.Columns[6].NumberFormat = "[$-00040C]dd/mm/yyyy;@";
                        xlWorkSheet.Columns[7].NumberFormat = "[$-00040C]dd/mm/yyyy;@";
                        xlWorkSheet.Columns[9].NumberFormat = "[$-00040C]dd/mm/yyyy;@";
                        xlWorkSheet.Columns[10].NumberFormat = "[$-00040C]dd/mm/yyyy;@";
                    }
                    else if (System.Globalization.CultureInfo.CurrentCulture.Name == "fr-FR")
                    {
                        xlWorkSheet.Columns[6].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                        xlWorkSheet.Columns[7].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                        xlWorkSheet.Columns[9].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                        xlWorkSheet.Columns[10].NumberFormat = "[$-00040C]jj/mm/aaaa;@";
                    }

                    foreach (CONTENEUR ctr in listConteneurs)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = ctr.NumCtr;
                        (range.Cells[i, 2] as Excel.Range).Value2 = ctr.ESCALE.NAVIRE.NomNav;
                        (range.Cells[i, 3] as Excel.Range).Value2 = "'" + ctr.ESCALE.NumVoySCR;
                        (range.Cells[i, 4] as Excel.Range).Value2 = ctr.TypeCCtr.Substring(0, 2);
                        (range.Cells[i, 5] as Excel.Range).Value2 = ctr.TypeCCtr.Substring(2, 2);
                        if (ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12) != null && ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12).DateOp.HasValue)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = ctr.OPERATION_CONTENEUR.FirstOrDefault(op => op.IdTypeOp == 12).DateOp.Value;
                        }
                        if (ctr.DSCtr.HasValue)
                        {
                            (range.Cells[i, 7] as Excel.Range).Value2 = ctr.DSCtr.Value;
                        }
                        if (ctr.NbDet.HasValue)
                        {
                            (range.Cells[i, 8] as Excel.Range).Value2 = ctr.NbDet;
                        }
                        if (ctr.NbDet.HasValue && ctr.DSCtr.HasValue)
                        {
                            (range.Cells[i, 9] as Excel.Range).Value2 = ctr.DSCtr.Value.AddDays(ctr.NbDet.Value);
                        }
                        if (ctr.DRCtr.HasValue)
                        {
                            (range.Cells[i, 10] as Excel.Range).Value2 = ctr.DRCtr.Value;
                        }
                        if (ctr.DRCtr.HasValue && ctr.DSCtr.HasValue)
                        {
                            (range.Cells[i, 11] as Excel.Range).Value2 = (ctr.DRCtr.Value - ctr.DSCtr.Value).Days;
                        }
                        (range.Cells[i, 12] as Excel.Range).Value2 = ctr.ELEMENT_FACTURATION.Where(el => el.LibEF.Contains("Détention") && el.QTEEF > 0).Sum(el => (el.PUEFBase * el.QTEEF.Value));
                        (range.Cells[i, 13] as Excel.Range).Value2 = ctr.ELEMENT_FACTURATION.Where(el => el.LibEF.Contains("Détention") && el.QTEEF > 0).Sum(el => (el.PUEFBase - el.PUEF) * el.QTEEF.Value);
                        (range.Cells[i, 14] as Excel.Range).Value2 = ctr.ELEMENT_FACTURATION.Where(el => el.LibEF.Contains("Détention") && el.QTEEF > 0).Sum(el => el.PUEF * el.QTEEF.Value);

                        StringBuilder sbProfs = new StringBuilder();

                        List<PROFORMA> profs = vsp.GetProformaByIdCtrAndArticle(ctr.IdCtr, 1807);

                        foreach(PROFORMA prof in profs.Distinct())
                        {
                            sbProfs.Append(prof.IdFP).Append("; ");
                        }

                        (range.Cells[i, 15] as Excel.Range).Value2 = sbProfs.ToString();

                        StringBuilder sbFacts = new StringBuilder();

                        List<FACTURE> facts = vsp.GetFactureByIdCtrAndCodeArticle(ctr.IdCtr, 1807);

                        foreach (FACTURE fact in facts.Distinct())
                        {
                            sbFacts.Append(fact.IdDocSAP).Append("; ");
                        }

                        (range.Cells[i, 16] as Excel.Range).Value2 = sbFacts.ToString();

                        (range.Cells[i, 17] as Excel.Range).Value2 = ctr.CONNAISSEMENT.NumBL;
                        (range.Cells[i, 18] as Excel.Range).Value2 = ctr.CONNAISSEMENT.ConsigneeBL;

                        i++;
                    }

                    (range.Cells[i, 12] as Excel.Range).Value2 = "=SUM(L2:L" + (i - 1).ToString() + ")";
                    (range.Cells[i, 13] as Excel.Range).Value2 = "=SUM(M2:M" + (i - 1).ToString() + ")";
                    (range.Cells[i, 14] as Excel.Range).Value2 = "=SUM(N2:N" + (i - 1).ToString() + ")";

                    range.Range[range.Cells[2, 1], range.Cells[i, 18]].Columns.Borders.LineStyle = 1;
                    range.Range[range.Cells[2, 1], range.Cells[i, 18]].Font.Name = "Tw Cen MT";

                    xlWorkBook.SaveAs(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Recap Détention du - " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Edition du recap détention terminée", "Edition du recap détention terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
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

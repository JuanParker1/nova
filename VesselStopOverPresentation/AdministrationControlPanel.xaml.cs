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
    /// Logique d'interaction pour AdministrationControlPanel.xaml
    /// </summary>
    public partial class AdministrationControlPanel : StackPanel
    {
        private VesselStopOverWindow vesselWindow;
        public UTILISATEUR utilisateur { get; set; }
        private List<OPERATION> operationsUser;

        private VSOMAccessors vsomAcc = new VSOMAccessors();
        //VsomParameters vsp = new VsomParameters();

        public AdministrationControlPanel(VesselStopOverWindow window, UTILISATEUR user)
        {
            InitializeComponent();
            vesselWindow = window;
            utilisateur = user;
            operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);
            if (operationsUser.Where(op => op.NomOp == "Acconier : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnAcconier.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Armateur : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnArmateur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Navire : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnNavire.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Port : Visualisation des éléments existants").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnPort.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Importation CIVIO").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnMAJSGS.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (operationsUser.Where(op => op.NomOp == "Opération sur les comptes utilisateurs").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnUtilisateur.Visibility = System.Windows.Visibility.Collapsed;
            }
            //if (operationsUser.Where(op => op.NomOp == "Opération sur le journal système").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            //{
            //    btnJournal.Visibility = System.Windows.Visibility.Collapsed;
            //}
            if (utilisateur.LU != "Admin")
            {
                btnCorrectionParc3000.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (utilisateur.LU != "Admin")
            {
                btnCorrectionSortieConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }
            if (utilisateur.LU != "Admin")
            {
                btnCorrectionDoublonConteneur.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (operationsUser.Where(op => op.NomOp == "Articles: Opération").FirstOrDefault<OPERATION>() == null && utilisateur.LU != "Admin")
            {
                btnArticles.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void btnMotDePasse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MotDePasseForm passwordForm = new MotDePasseForm(this, utilisateur);
                passwordForm.Title = "Changement de mot de passe";
                passwordForm.ShowDialog();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void btnNavire_Click(object sender, RoutedEventArgs e)
        {
            NavirePanel navirePanel = new NavirePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Navire";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = navirePanel;
        }

        private void btnAcconier_Click(object sender, RoutedEventArgs e)
        {
            AcconierPanel acconierPanel = new AcconierPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Acconier";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = acconierPanel;
        }

        private void btnArmateur_Click(object sender, RoutedEventArgs e)
        {
            ArmateurPanel armateurPanel = new ArmateurPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Armateur";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = armateurPanel;
        }

        private void btnPort_Click(object sender, RoutedEventArgs e)
        {
            PortPanel portPanel = new PortPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Port";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = portPanel;
        }

        private void btnMAJSGS_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet1 = null;
            Excel.Worksheet xlWorkSheet2 = null;
            Excel.Range range;

            int rCnt1 = 2;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsm = new VsomMarchal();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Infos CIVIO (*.xls)|*.xls|Infos CIVIO (*.xlsx)|*.xlsx";
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

                List<SGS> listSGSs = new List<SGS>();

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet1.UsedRange;

                    List<string> voyages = new List<string>();
                    List<string> navires = new List<string>();

                    StringBuilder sb = new StringBuilder();

                    while (((string)(range.Cells[rCnt1, 1] as Excel.Range).Value2) != null)
                    {
                        SGS sgs = new SGS();
                        sgs.NumChassis = (string)(range.Cells[rCnt1, 1] as Excel.Range).Value2;
                        sgs.NumCIVIO = (string)(range.Cells[rCnt1, 5] as Excel.Range).Value2;
                        sgs.CouleurVeh = (string)(range.Cells[rCnt1, 4] as Excel.Range).Value2;
                        sgs.MarqueVeh = ((string)(range.Cells[rCnt1, 2] as Excel.Range).Value2).Trim() + " " + (((string)(range.Cells[rCnt1, 3] as Excel.Range).Value2) != null && ((string)(range.Cells[rCnt1, 3] as Excel.Range).Value2).Split(' ').Length > 0 ? ((string)(range.Cells[rCnt1, 3] as Excel.Range).Value2).Split(' ')[0] : "");
                        sgs.Acconier = (string)(range.Cells[rCnt1, 8] as Excel.Range).Value2;

                        VEHICULE veh = vsomAcc.UpdateInfosCIVIO(sgs, utilisateur.IdU);

                        if (veh != null)
                        {
                            sb.Append("Mise à jour OK : Ligne - ").Append(rCnt1.ToString()).Append(" ").Append(veh.NumChassis).Append(" ").Append(veh.DescVeh).Append(Environment.NewLine);
                            range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.LightGreen;
                        }
                        else
                        {
                            sb.Append("Mise à jour KO : Ligne - ").Append(rCnt1.ToString()).Append(" ").Append(sgs.NumChassis).Append(" ").Append(sgs.MarqueVeh).Append(Environment.NewLine);
                            range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.Magenta;
                        }

                        rCnt1++;
                    }

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log Importation CIVIO.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    xlWorkBook.SaveAs(parentDir.FullName + "\\CIVIO Report.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(rCnt1 + "\n" + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet1 != null)
                {
                    releaseObject(xlWorkSheet1);
                }

                if (xlWorkSheet2 != null)
                {
                    releaseObject(xlWorkSheet2);
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

        private void btnUtilisateur_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurPanel userPanel = new UtilisateurPanel(utilisateur);
            vesselWindow.mainPanel.Header = "Utilisateur";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = userPanel;
        }

        private void btnImportSAP_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet1 = null;
            Excel.Worksheet xlWorkSheet2 = null;
            Excel.Range range1;
            Excel.Range range2;

            int rCnt1 = 3;
            int rCnt2 = 3;

            double batchNum = 0;

            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Ecritures SAP (*.xls)|*.xls|Ecritures SAP (*.xlsx)|*.xlsx";
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

                List<EcritureJournal> listEcritures = new List<EcritureJournal>();

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                    xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

                    range1 = xlWorkSheet1.UsedRange;
                    range2 = xlWorkSheet2.UsedRange;

                    //xlWorkSheet2.Columns[5].NumberFormat = "@";
                    //xlWorkSheet2.Columns[6].NumberFormat = "@";
                    //xlWorkSheet2.Columns[8].NumberFormat = "@";

                    while (((range1.Cells[rCnt1, 1] as Excel.Range).Value2) != null)
                    {
                        batchNum = ((double)(range1.Cells[rCnt1, 1] as Excel.Range).Value2);

                        EcritureJournal ecriture = new EcritureJournal();
                        ecriture.batchNum = batchNum;
                        string refdate = ((double)(range1.Cells[rCnt1, 3] as Excel.Range).Value2).ToString();
                        ecriture.refDate = refdate.Substring(0, 4) + "-" + refdate.Substring(4, 2) + "-" + refdate.Substring(6, 2);
                        string taxdate = ((double)(range1.Cells[rCnt1, 9] as Excel.Range).Value2).ToString();
                        ecriture.taxDate = taxdate.Substring(0, 4) + "-" + taxdate.Substring(4, 2) + "-" + taxdate.Substring(6, 2);
                        string duedate = ((double)(range1.Cells[rCnt1, 10] as Excel.Range).Value2).ToString();
                        ecriture.dueDate = duedate.Substring(0, 4) + "-" + duedate.Substring(4, 2) + "-" + duedate.Substring(6, 2);
                        ecriture.memo = ((string)(range1.Cells[rCnt1, 4] as Excel.Range).Value2).Replace("/", "-");
                        ecriture.ref1 = ((double)(range1.Cells[rCnt1, 5] as Excel.Range).Value2).ToString();
                        ecriture.ref2 = ((double)(range1.Cells[rCnt1, 6] as Excel.Range).Value2).ToString();
                        if (ecriture.ref2 == "0")
                        {
                            ecriture.ref2 = "";
                        }
                        ecriture.ref3 = "";

                        ecriture.lignes = new List<VesselStopOverData.SocSAPWS.LigneJournal>();

                        while (((double)(range2.Cells[rCnt2, 1] as Excel.Range).Value2) == batchNum)
                        {
                            VesselStopOverData.SocSAPWS.LigneJournal ligne = new VesselStopOverData.SocSAPWS.LigneJournal();

                            ligne.AccountCode = ((double)(range2.Cells[rCnt2, 4] as Excel.Range).Value2).ToString();
                            ligne.ShortName = ((range2.Cells[rCnt2, 8] as Excel.Range).Value2).ToString();
                            if (ligne.ShortName == "0")
                            {
                                ligne.ShortName = ligne.AccountCode;
                            }
                            ligne.ControlAccount = ((double)(range2.Cells[rCnt2, 4] as Excel.Range).Value2).ToString();
                            ligne.Credit = (double)((range2.Cells[rCnt2, 6] as Excel.Range).Value2);
                            ligne.Debit = (double)((range2.Cells[rCnt2, 5] as Excel.Range).Value2);
                            ligne.DueDate = ecriture.dueDate;
                            ligne.LineMemo = ((string)(range2.Cells[rCnt2, 9] as Excel.Range).Value2).ToString().Trim();
                            ligne.Reference1 = ((double)(range2.Cells[rCnt2, 11] as Excel.Range).Value2).ToString();
                            ligne.Reference2 = ((double)(range2.Cells[rCnt2, 12] as Excel.Range).Value2).ToString();
                            if (ligne.Reference2 == "0")
                            {
                                ligne.Reference2 = "";
                            }

                            ligne.CostingCode = ((double)(range2.Cells[rCnt2, 16] as Excel.Range).Value2).ToString().Trim();
                            if (ligne.CostingCode == "0")
                            {
                                ligne.CostingCode = "";
                            }

                            ligne.CostingCode2 = ((double)(range2.Cells[rCnt2, 17] as Excel.Range).Value2).ToString().Trim();
                            if (ligne.CostingCode2 == "0")
                            {
                                ligne.CostingCode2 = "";
                            }

                            ligne.CostingCode3 = ((double)(range2.Cells[rCnt2, 18] as Excel.Range).Value2).ToString().Trim();
                            if (ligne.CostingCode3 == "0")
                            {
                                ligne.CostingCode3 = "";
                            }

                            ligne.CostingCode4 = ((double)(range2.Cells[rCnt2, 19] as Excel.Range).Value2).ToString().Trim();
                            if (ligne.CostingCode4 == "0")
                            {
                                ligne.CostingCode4 = "";
                            }

                            ligne.CostingCode5 = ((string)(range2.Cells[rCnt2, 20] as Excel.Range).Value2).ToString().Trim();
                            if (ligne.CostingCode5 == "0")
                            {
                                ligne.CostingCode5 = "";
                            }

                            ligne.ReferenceDate1 = ecriture.refDate;
                            ligne.TaxDate = ecriture.taxDate;
                            ligne.VATGroup = "";
                            ecriture.lignes.Add(ligne);

                            rCnt2++;
                        }

                        listEcritures.Add(ecriture);

                        rCnt1++;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb = vsomAcc.ImportationSAP(listEcritures);

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log Importation SAP.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Batch " + batchNum.ToString() + " - Ligne 1 : " + rCnt1.ToString() + " - Ligne 2 : " + rCnt2.ToString() + Environment.NewLine + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet1 != null)
                {
                    releaseObject(xlWorkSheet1);
                }

                if (xlWorkSheet2 != null)
                {
                    releaseObject(xlWorkSheet2);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE *32").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void btnCorrectionLignesSAP_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet2 = null;
            Excel.Range range2;

            int rCnt2 = 3;

            double batchNum = 0;

            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Ecritures SAP (*.xls)|*.xls|Ecritures SAP (*.xlsx)|*.xlsx";
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

                List<VesselStopOverData.SocSAPWS.LigneJournal> listLignes = new List<VesselStopOverData.SocSAPWS.LigneJournal>();

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);

                    range2 = xlWorkSheet2.UsedRange;

                    while (((range2.Cells[rCnt2, 1] as Excel.Range).Value2) != null)
                    {
                        batchNum = ((double)(range2.Cells[rCnt2, 1] as Excel.Range).Value2);

                        VesselStopOverData.SocSAPWS.LigneJournal ligne = new VesselStopOverData.SocSAPWS.LigneJournal();

                        ligne.ShortName = ((range2.Cells[rCnt2, 3] as Excel.Range).Value2).ToString();
                        ligne.ControlAccount = batchNum.ToString();

                        ligne.Reference1 = ((double)(range2.Cells[rCnt2, 11] as Excel.Range).Value2).ToString();
                        ligne.LineMemo = ((string)(range2.Cells[rCnt2, 9] as Excel.Range).Value2).ToString().Trim();

                        listLignes.Add(ligne);

                        rCnt2++;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb = vsomAcc.CorrectionLigneEcriture(listLignes);

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log Correction Lignes.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Batch " + batchNum.ToString() + " - Ligne : " + rCnt2.ToString() + Environment.NewLine + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet2 != null)
                {
                    releaseObject(xlWorkSheet2);
                }

                if (xlWorkBook != null)
                {
                    xlWorkBook.Close(true, Type.Missing, Type.Missing);
                    releaseObject(xlWorkBook);
                }

                bool excelWasRunning = System.Diagnostics.Process.GetProcessesByName("EXCEL.EXE *32").Length > 0;

                if (excelWasRunning)
                {
                    xlApp.Quit();
                    releaseObject(xlApp);
                }
            }
        }

        private void btnCorrectionSorties_Click(object sender, RoutedEventArgs e)
        {
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet1 = null;
            Excel.Range range;

            int rCnt1 = 2;

            try
            {
                //VSOMAccessors vsomAcc = new VSOMAccessors();

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.DefaultExt = ".xlsx";
                dlg.Filter = "Infos Sorties (*.xls)|*.xls|Infos Sorties (*.xlsx)|*.xlsx";
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

                List<SGS> listSGSs = new List<SGS>();

                if (filename != "")
                {
                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(filename, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet1.UsedRange;

                    StringBuilder sb = new StringBuilder();

                    while (((double)(range.Cells[rCnt1, 1] as Excel.Range).Value2) != 0)
                    {
                        string numChassis = (string)(range.Cells[rCnt1, 6] as Excel.Range).Value2;
                        string numBL = (string)(range.Cells[rCnt1, 5] as Excel.Range).Value2;

                        VEHICULE veh = vsomAcc.GetVehiculeByNumChassisAndNumBL(numChassis, numBL);

                        if (veh != null)
                        {
                            if (veh.StatVeh == "Livraison")
                            {
                                vsomAcc.TransfertZoneSortie(veh.IdVeh, 3, DateTime.FromOADate((double)(range.Cells[rCnt1, 16] as Excel.Range).Value2), "Régularisation", 1);
                                vsomAcc.SortirVehicule(veh.IdVeh, 3, DateTime.FromOADate((double)(range.Cells[rCnt1, 16] as Excel.Range).Value2), "Régularisation", 1);
                                sb.Append("Mise à jour OK : Ligne - ").Append(rCnt1.ToString()).Append(" ").Append(veh.NumChassis).Append(" ").Append(veh.DescVeh).Append(Environment.NewLine);
                                range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.LightGreen;
                            }
                            else if (veh.StatVeh == "Sortie en cours")
                            {
                                vsomAcc.SortirVehicule(veh.IdVeh, 3, DateTime.FromOADate((double)(range.Cells[rCnt1, 16] as Excel.Range).Value2), "Régularisation", 1);
                                sb.Append("Mise à jour OK : Ligne - ").Append(rCnt1.ToString()).Append(" ").Append(veh.NumChassis).Append(" ").Append(veh.DescVeh).Append(Environment.NewLine);
                                range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.LightGreen;
                            }
                            else
                            {
                                sb.Append("Mise à jour KO : Ligne - ").Append(rCnt1.ToString()).Append(numChassis).Append(" ").Append(numBL).Append(Environment.NewLine);
                                range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.Magenta;
                            }
                        }
                        else
                        {
                            sb.Append("Mise à jour KO : Ligne - ").Append(rCnt1.ToString()).Append(numChassis).Append(" ").Append(numBL).Append(Environment.NewLine);
                            range.Range[range.Cells[rCnt1, 1], range.Cells[rCnt1, 9]].Interior.Color = System.Drawing.Color.Magenta;
                        }

                        rCnt1++;
                    }

                    DirectoryInfo parentDir = Directory.GetParent(filename);
                    System.IO.File.WriteAllText(parentDir.FullName + "\\Log MAJ Sorties.txt", sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));
                    xlWorkBook.SaveAs(parentDir.FullName + "\\SortiesReport.xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                    MessageBox.Show("Importation terminée", "Importation terminée !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(rCnt1 + "\n" + ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(rCnt1 + "\n" + ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                if (xlWorkSheet1 != null)
                {
                    releaseObject(xlWorkSheet1);
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

        private void btnCorrectionParc3000_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                MessageBox.Show(vsomAcc.CorrectionParc30000().ToString());
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

        private void btnCorrectionSortieConteneur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                MessageBox.Show(vsomAcc.CorrectionSortieConteneur().ToString());
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

        private void btnCorrectionDoublonConteneur_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();
                MessageBox.Show(vsomAcc.CorrectionDoublon().ToString());
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

        private void btnCorrectionImportDIT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomMarchal vsomAcc = new VsomMarchal();

                MessageBox.Show(vsomAcc.CorrectionImportDIT().ToString());
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

        private void btnArticles_Click_1(object sender, RoutedEventArgs e)
        {
            ArticlePanel userPanel = new ArticlePanel(utilisateur);
            vesselWindow.mainPanel.Header = "Articles";
            vesselWindow.mainPanel.Content = null;
            vesselWindow.mainPanel.Content = userPanel;
        }

    }
}

internal class NOVA_SAGE2
{
    private string _code; private string _invoicedate; private string _fc; private string _codetitle;
    private string _x; private string _customercode; private string _invoicenumber; private string _description;
    private string _paytype; private string _datepay; private string _debitcredit; private string _grossamount; private string _n;
    public string Code { get { return _code.PadRight(3); } set { _code = value; } }
    public string InvoiceDate { get { return _invoicedate; } set { _invoicedate = value; } }
    public string FC { get { return _fc; } set { _fc = value; } }
    public string CodeTitle { get { return _codetitle.PadRight(13); } set { _codetitle = value; } }
    public string X { get { return _x; } set { _x = value; } }
    public string CustomerCode { get { return _customercode.PadRight(13); } set { _customercode = value; } }
    public string InvoiceNumber { get { return _invoicenumber.PadRight(13); } set { _invoicenumber = value; } }
    public string Description { get { return _description.PadRight(25); } set { _description = value; } }
    public string PayType { get { return _paytype; } set { _paytype = value; } }
    public string DatePay { get { return _datepay; } set { _datepay = value; } }
    public string DebitCredit { get { return _debitcredit; } set { _debitcredit = value; } }
    public string GrossAmount { get { return _grossamount.PadLeft(20); } set { _grossamount = value; } }
    public string N { get { return _n; } set { _n = value; } }

}
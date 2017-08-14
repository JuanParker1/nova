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
    /// Logique d'interaction pour DailyMovesFormExcel.xaml
    /// </summary>
    public partial class DailyMovesFormExcel : Window
    {

        private ReportingControlPanel reportPanel;
        private TrackingControlPanel trackingPanel;

        private List<ARMATEUR> armateurs;
        public List<string> arms { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public DailyMovesFormExcel(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                armateurs = vsprm.GetArmateursActifs();
                arms = new List<string>();
                foreach (ARMATEUR arm in armateurs)
                {
                    arms.Add(arm.NomArm);
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

        public DailyMovesFormExcel(TrackingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsprm = new VsomParameters();

                InitializeComponent();
                this.DataContext = this;

                armateurs = vsprm.GetArmateursActifs();
                arms = new List<string>();
                foreach (ARMATEUR arm in armateurs)
                {
                    arms.Add(arm.NomArm);
                }

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

        private void btnDailyMoves_Click(object sender, RoutedEventArgs e)
        {

            Excel.Application xlApp = null;
            Excel.Workbook xlWorkBook = null;
            Excel.Worksheet xlWorkSheet = null;
            Excel.Range range;

            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();

                if (cbArmateur.SelectedIndex == -1)
                {
                    MessageBox.Show("Veuillez sélectionner un armateur", "Armateur ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateDebut.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de début", "Date début ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!txtDateFin.SelectedDate.HasValue)
                {
                    MessageBox.Show("Veuillez saisir la date de fin", "Date fin ?", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {

                    List<OPERATION_CONTENEUR> listDailyMoves = vsp.GetDailyMovesExcel(armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value);

                    xlApp = new Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Open(Environment.CurrentDirectory + "//Ressources//DailyMovesNDS.xlsx", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
                    xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                    range = xlWorkSheet.UsedRange;

                    int i = 2;

                    foreach (OPERATION_CONTENEUR opCtr in listDailyMoves)
                    {
                        (range.Cells[i, 1] as Excel.Range).Value2 = opCtr.CONTENEUR.NumCtr;

                        if (opCtr.CONTENEUR.TypeCCtr == "20BX")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "22G1";
                        }
                        else if (opCtr.CONTENEUR.TypeCCtr == "40BX")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42G1";
                        }
                        else if (opCtr.CONTENEUR.TypeCCtr == "40HC")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "45G1";
                        }
                        else if (opCtr.CONTENEUR.TypeCCtr == "40OT")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42U1";
                        }
                        else if (opCtr.CONTENEUR.TypeCCtr == "40FL")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "45P3";
                        }
                        else if (opCtr.CONTENEUR.TypeCCtr == "20OT")
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = "42U1";
                        }
                        else
                        {
                            (range.Cells[i, 2] as Excel.Range).Value2 = opCtr.CONTENEUR.TypeCCtr;
                        }

                        if (opCtr.IdTypeOp == 18)
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "GOF";
                        }
                        else if (opCtr.IdTypeOp == 19)
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "GIE";
                        }
                        else if (opCtr.IdTypeOp == 281)
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "GOE";
                        }
                        else if (opCtr.IdTypeOp == 282)
                        {
                            (range.Cells[i, 3] as Excel.Range).Value2 = "GIF";
                        }
                        //else if (opCtr.IdTypeOp == 12)
                        //{
                        //    if (opCtr.CONTENEUR.StatutCtr.StartsWith("F"))
                        //    {
                        //        (range.Cells[i, 3] as Excel.Range).Value2 = "DIF";
                        //    }
                        //    else if (opCtr.CONTENEUR.StatutCtr.StartsWith("E"))
                        //    {
                        //        (range.Cells[i, 3] as Excel.Range).Value2 = "DIE";
                        //    }
                        //}
                        //else if (opCtr.IdTypeOp == 283)
                        //{
                        //    if (opCtr.CONTENEUR.StatutCtr.StartsWith("F"))
                        //    {
                        //        (range.Cells[i, 3] as Excel.Range).Value2 = "LOF";
                        //    }
                        //    else if (opCtr.CONTENEUR.StatutCtr.StartsWith("E"))
                        //    {
                        //        (range.Cells[i, 3] as Excel.Range).Value2 = "LOE";
                        //    }
                        //}

                        (range.Cells[i, 4] as Excel.Range).Value2 = opCtr.CONTENEUR.StatutCtr.Substring(0, 1);
                        (range.Cells[i, 5] as Excel.Range).Value2 = opCtr.DateOp.Value.Year.ToString() + FormatChiffre(opCtr.DateOp.Value.Month) + FormatChiffre(opCtr.DateOp.Value.Day);

                        if (opCtr.IdTypeOp == 18)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                            (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                        }
                        else if (opCtr.IdTypeOp == 19)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                            (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                        }
                        else if (opCtr.IdTypeOp == 281)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 7] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 8] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 9] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 10] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                            (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                        }
                        else if (opCtr.IdTypeOp == 282)
                        {
                            (range.Cells[i, 6] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 7] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 8] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 9] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 10] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                            (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                            (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                        }
                        else if (opCtr.IdTypeOp == 12)
                        {
                            if (opCtr.CONTENEUR.StatutCtr.StartsWith("F"))
                            {
                                (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 7] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 8] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 10] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                                (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                            }
                            else if (opCtr.CONTENEUR.StatutCtr.StartsWith("E"))
                            {
                                (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 7] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 8] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 10] as Excel.Range).Value2 = "DLA";
                                (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                                (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                            }
                        }
                        else if (opCtr.IdTypeOp == 283)
                        {
                            if (opCtr.CONTENEUR.StatutCtr.StartsWith("F"))
                            {
                                (range.Cells[i, 6] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 7] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 8] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 9] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 10] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                                (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                            }
                            else if (opCtr.CONTENEUR.StatutCtr.StartsWith("E"))
                            {
                                (range.Cells[i, 6] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 7] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 8] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 9] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 10] as Excel.Range).Value2 = opCtr.CONTENEUR.CONNAISSEMENT.DPBL;
                                (range.Cells[i, 11] as Excel.Range).Value2 = opCtr.CONTENEUR.CONTENEUR_TC.FirstOrDefault().MOUVEMENT_TC.FirstOrDefault(m => m.IdTypeOp == opCtr.IdTypeOp).ESCALE.NAVIRE.CodeNDS;
                                (range.Cells[i, 12] as Excel.Range).Value2 = opCtr.CONTENEUR.ESCALE.NumVoySCR;
                            }
                        }

                        if ((string)((range.Cells[i, 3] as Excel.Range).Value2) == "GIF")
                        {
                            (range.Cells[i, 4] as Excel.Range).Value2 = "F";
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "SHIPPER";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "DIT";
                        }
                        else if ((string)((range.Cells[i, 3] as Excel.Range).Value2) == "GOF")
                        {
                            (range.Cells[i, 4] as Excel.Range).Value2 = "F";
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "DIT";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "CONSIGNEE";
                        }
                        else if ((string)((range.Cells[i, 3] as Excel.Range).Value2) == "GIE")
                        {
                            (range.Cells[i, 4] as Excel.Range).Value2 = "E";
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "CONSIGNEE";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "SOCOMAR";
                        }
                        else if ((string)((range.Cells[i, 3] as Excel.Range).Value2) == "GOE")
                        {
                            (range.Cells[i, 4] as Excel.Range).Value2 = "E";
                            (range.Cells[i, 6] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 7] as Excel.Range).Value2 = "SHIPPER";
                            (range.Cells[i, 8] as Excel.Range).Value2 = "";
                            (range.Cells[i, 9] as Excel.Range).Value2 = "DLA";
                            (range.Cells[i, 10] as Excel.Range).Value2 = "DIT";
                        }

                        i++;
                    }
                    range.Range[range.Cells[1, 1], range.Cells[i - 1, 16]].Columns.Borders.LineStyle = 1;

                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Daily Moves - " + cbArmateur.Text + " - du " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".xlsx";
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
                        MessageBox.Show("Rapport Daily Moves édité avec succès", "Rapport Daily Moves édité !", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void cbArmateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                txtCodeArmateur.Text = armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).CodeArm;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

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

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
    /// Logique d'interaction pour DailyMovesForm.xaml
    /// </summary>
    public partial class DailyMovesForm : Window
    {

        private ReportingControlPanel reportPanel;
        private TrackingControlPanel trackingPanel;

        private List<ARMATEUR> armateurs;
        public List<string> arms { get; set; }

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();
        public DailyMovesForm(ReportingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                //VsomParameters vsprm = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                armateurs = vsp.GetArmateursActifs();
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

        public DailyMovesForm(TrackingControlPanel panel, UTILISATEUR user)
        {
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();
                InitializeComponent();
                this.DataContext = this;

                armateurs = vsp.GetArmateursActifs();
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
            try
            {
                VSOMAccessors vsomAcc = new VSOMAccessors();
                VsomParameters vsp = new VsomParameters();

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

                    StringBuilder sb = new StringBuilder();
                    DateTime dte = DateTime.Now;

                    List<MOUVEMENT_TC> listDailyMoves = vsp.GetDailyMoves(armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm, txtDateDebut.SelectedDate.Value, txtDateFin.SelectedDate.Value);
                    int numMessage = vsomAcc.GetNumMessageGR();

                    int i = 1;
                    sb.Append("SOCOMAR").Append(Environment.NewLine);
                    sb.Append("B.P:12351 Douala Cameroun").Append(Environment.NewLine);
                    sb.Append("Til:(237) 33424550. Fax:(237) 33424936").Append(Environment.NewLine);
                    sb.Append("Nr.Message: ").Append(numMessage).Append(" Date ").Append(dte.Day + "." + dte.Month + "." + dte.Year).Append(Environment.NewLine);
                    sb.Append("Operation: Daily Movements from ").Append(txtDateDebut.SelectedDate.Value.Day + "." + txtDateDebut.SelectedDate.Value.Month + "." + txtDateDebut.SelectedDate.Value.Year).Append(" to ").Append(txtDateFin.SelectedDate.Value.Day + "." + txtDateFin.SelectedDate.Value.Month + "." + txtDateFin.SelectedDate.Value.Year).Append(Environment.NewLine);

                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);


                    if (armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm == 1)
                    {
                        //vsomAcc.GetPortsByCodePort(book.LDBL).FirstOrDefault<PORT>().NomPortEN;

                        sb.Append("0004043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append(numMessage.ToString()).Append("GR").Append(Environment.NewLine);
                        foreach (MOUVEMENT_TC mvtTC in listDailyMoves)
                        {
                            if (mvtTC.IdTypeOp == 281)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append(vsp.GetPortsByCodePort(mvtTC.CONNAISSEMENT.DPBL).FirstOrDefault<PORT>().CodeRadio).Append(Environment.NewLine);
                            }
                            else if (mvtTC.IdTypeOp == 282)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append(vsp.GetPortsByCodePort(mvtTC.CONNAISSEMENT.DPBL).FirstOrDefault<PORT>().CodeRadio).Append(Environment.NewLine);
                            }
                            else
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append(mvtTC.IdTypeOp == 19 ? "SVU" : "077").Append(Environment.NewLine);
                            }
                            i++;
                        }
                        sb.Append("9994043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append("000").Append((listDailyMoves.Count + 2).ToString().PadLeft(3, '0')).Append(Environment.NewLine);
                    }
                    else if (armateurs.ElementAt<ARMATEUR>(cbArmateur.SelectedIndex).IdArm == 2)
                    {
                        sb.Append("0004043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append(numMessage.ToString()).Append("GR").Append(Environment.NewLine);
                        foreach (MOUVEMENT_TC mvtTC in listDailyMoves)
                        {
                            if (mvtTC.IdTypeOp == 18)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("GOF").Append(Environment.NewLine);
                            }
                            else if (mvtTC.IdTypeOp == 19)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("GIE").Append(Environment.NewLine);
                            }
                            else if (mvtTC.IdTypeOp == 281)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("GOE").Append(Environment.NewLine);
                            }
                            else if (mvtTC.IdTypeOp == 282)
                            {
                                sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("GIF").Append(Environment.NewLine);
                            }
                            else if (mvtTC.IdTypeOp == 12)
                            {
                                if (mvtTC.CONTENEUR_TC.CONTENEUR.StatutCtr.StartsWith("F"))
                                {
                                    sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("DIF").Append(Environment.NewLine);
                                }
                                else if (mvtTC.CONTENEUR_TC.CONTENEUR.StatutCtr.StartsWith("E"))
                                {
                                    sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("DIE").Append(Environment.NewLine);
                                }
                            }
                            else if (mvtTC.IdTypeOp == 283)
                            {
                                if (mvtTC.CONTENEUR_TC.CONTENEUR.StatutCtr.StartsWith("F"))
                                {
                                    sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("LOF").Append(Environment.NewLine);
                                }
                                else if (mvtTC.CONTENEUR_TC.CONTENEUR.StatutCtr.StartsWith("E"))
                                {
                                    sb.Append(mvtTC.TYPE_OPERATION.CodeMvt).Append(mvtTC.CONTENEUR_TC.NumTC.Replace("-Doublon", "")).Append("077").Append(FormatChiffre(mvtTC.DateMvt.Value.Year).Substring(2, 2) + FormatChiffre(mvtTC.DateMvt.Value.Month) + FormatChiffre(mvtTC.DateMvt.Value.Day)).Append("LOE").Append(Environment.NewLine);
                                }
                            }
                            
                            i++;
                        }
                        sb.Append("9994043077").Append(FormatChiffre(dte.Year).Substring(2, 2) + FormatChiffre(dte.Month) + FormatChiffre(dte.Day) + FormatChiffre(dte.Hour) + FormatChiffre(dte.Minute)).Append("000").Append((listDailyMoves.Count + 2).ToString().PadLeft(3, '0')).Append(Environment.NewLine);
                    }
                    
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.FileName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Daily Moves - " + cbArmateur.Text + " - du " + txtDateDebut.SelectedDate.Value.ToShortDateString().Replace("/", "") + " au " + txtDateFin.SelectedDate.Value.ToShortDateString().Replace("/", "") + ".txt";
                    dlg.DefaultExt = ".txt"; // Default file extension
                    dlg.Filter = "Text Documents (.txt)|*.txt"; // Filter files by extension

                    // Show save file dialog box
                    Nullable<bool> result = dlg.ShowDialog();

                    string filename = "";
                    // Process save file dialog box results
                    if (result == true)
                    {
                        // Save document
                        filename = dlg.FileName;

                        System.IO.File.WriteAllText(filename, sb.ToString(), Encoding.GetEncoding("ISO-8859-1"));

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

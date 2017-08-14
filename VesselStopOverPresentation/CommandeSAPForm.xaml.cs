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
    /// Logique d'interaction pour CommandeSAPForm.xaml
    /// </summary>
    public partial class CommandeSAPForm : Window
    {

        private ReportingControlPanel reportPanel;

        private List<OPERATION> operationsUser;
        private UTILISATEUR utilisateur;

        private FormLoader formLoader;
        private VsomParameters vsp = new VsomParameters();

        public CommandeSAPForm(ReportingControlPanel panel, UTILISATEUR user)
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

        private void txtNumCommande_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Return && txtNumCommande.Text.Trim() != "")
                {
                    //VSOMAccessors vsomAcc = new VSOMAccessors();
                    VsomMarchal vsomAcc = new VsomMarchal();

                    if (vsomAcc.GetMontantCommande(Convert.ToInt32(txtNumCommande.Text.Trim())) != 0)
                    {
                        CommandeSAPReport commandeSAPReport = new CommandeSAPReport(this, Convert.ToInt32(txtNumCommande.Text.Trim()), NumberToWords(vsomAcc.GetMontantCommande(Convert.ToInt32(txtNumCommande.Text.Trim()))));
                        commandeSAPReport.Title = "Impression de la commande SAP : " + txtNumCommande.Text.Trim();
                        commandeSAPReport.Show();
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

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zéro";

            if (number < 0)
                return "moins " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + ((number / 1000000) > 1 ? " millions " : " million ");
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " mille ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " cent ";
                number %= 100;
            }

            if (number > 0)
            {
                //if (words != "")
                //    words += " ";

                var unitsMap = new[] { "zéro", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "dix", "onze", "douze", "treize", "quatorze", "quinze", "seize", "dix sept", "dix huit", "dix neuf" };
                var tensMap = new[] { "zéro", "dix", "vingt", "trente", "quarante", "cinquante", "soixante", "soixante dix", "quatre vingt", "quatre vingt dix" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Replace("dix un", "onze").Replace("dix deux", "douze").Replace("dix trois", "treize").Replace("dix quatre", "quatorze").Replace("dix cinq", "quinze").Replace("dix six", "seize").Replace("un cent", "cent");
        }

        private void btnImprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtNumCommande.Text.Trim() != "")
                {
                    //VSOMAccessors vsomAcc = new VSOMAccessors();
                    VsomMarchal vsomAcc = new VsomMarchal();

                    if (vsomAcc.GetMontantCommande(Convert.ToInt32(txtNumCommande.Text.Trim())) != 0)
                    {
                        CommandeSAPReport commandeSAPReport = new CommandeSAPReport(this, Convert.ToInt32(txtNumCommande.Text.Trim()), NumberToWords(vsomAcc.GetMontantCommande(Convert.ToInt32(txtNumCommande.Text.Trim()))));
                        commandeSAPReport.Title = "Impression de la commande SAP : " + txtNumCommande.Text.Trim();
                        commandeSAPReport.Show();
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
    }
}

using System;
using System.Net;
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
using Microsoft.Reporting.WinForms;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour AvoirReport.xaml
    /// </summary>
    public partial class AvoirReport : Window
    {
        private AvoirForm avoirForm;
        private Finance.AvoirCustom avoirForm2;

        public AvoirReport(AvoirForm form)
        {
            try
            {
                InitializeComponent();
                avoirForm = form;

                repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                repViewer.ShowParameterPrompts = false;
                repViewer.ServerReport.ReportPath = "/VSOMReports/Avoir";

                NetworkCredential myCred = new NetworkCredential("novareports", "novareports", "siege.local");
                repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                ReportParameter[] parameters = new ReportParameter[1];
                parameters[0] = new ReportParameter("RefAvoir", avoirForm.txtIdAvoir.Text);
                repViewer.ServerReport.SetParameters(parameters);
                repViewer.ServerReport.Refresh();
                repViewer.RefreshReport();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        public AvoirReport(Finance.AvoirCustom form)
        {
            try
            {
                InitializeComponent();
                avoirForm2 = form;

                if (avoirForm2._avoir.TypeAvoir == "Spot")
                {
                    repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                    repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                    repViewer.ShowParameterPrompts = false;
                    repViewer.ServerReport.ReportPath = "/VSOMReports/AvoirSpotArmateur";

                    NetworkCredential myCred = new NetworkCredential("novareports", "novareports", "siege.local");
                    repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                    ReportParameter[] parameters = new ReportParameter[1];
                    parameters[0] = new ReportParameter("refAvoir", avoirForm2._avoir.IdFA.ToString());
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();
                }
                else
                {
                    repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                    repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                    repViewer.ShowParameterPrompts = false;
                    repViewer.ServerReport.ReportPath = "/VSOMReports/AvoirFactureSpot";

                    NetworkCredential myCred = new NetworkCredential("novareports", "novareports", "siege.local");
                    repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                    ReportParameter[] parameters = new ReportParameter[1];
                    parameters[0] = new ReportParameter("RefAvoir", avoirForm.txtIdAvoir.Text);
                    repViewer.ServerReport.SetParameters(parameters);
                    repViewer.ServerReport.Refresh();
                    repViewer.RefreshReport();
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

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
    /// Logique d'interaction pour LivraisonReport.xaml
    /// </summary>
    public partial class LivraisonReport : Window
    {
        private DemandeLivraisonForm livForm;

        public LivraisonReport(DemandeLivraisonForm form)
        {
            try
            {
                InitializeComponent();
                livForm = form;

                repViewer.ProcessingMode = Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                repViewer.ServerReport.ReportServerUrl = new Uri("http://192.168.0.28/ReportServer");
                repViewer.ShowParameterPrompts = false;
                repViewer.ServerReport.ReportPath = "/VSOMReports/Livraison";

                NetworkCredential myCred = new NetworkCredential("novareports", "novareports", "siege.local");
                repViewer.ServerReport.ReportServerCredentials.NetworkCredentials = myCred;

                ReportParameter[] parameters = new ReportParameter[1];
                parameters[0] = new ReportParameter("RefDBL", livForm.cbIdDL.Text);
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
    }
}

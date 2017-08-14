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
using System.ComponentModel;
using VesselStopOverData;

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour NavigationWindow.xaml
    /// </summary>
    public partial class Container : NavigationWindow
    {
        public Container()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                base.OnClosing(e);
                //Application.Current.MainWindow.Close();
                if (MessageBox.Show("Etes vous sûr(e) de vouloir fermer l'application ?", "Etes vous sûr(e)", MessageBoxButton.YesNo, MessageBoxImage.Question)
                    == MessageBoxResult.No)
                    e.Cancel = true;
                Application.Current.MainWindow.Close();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
            
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}

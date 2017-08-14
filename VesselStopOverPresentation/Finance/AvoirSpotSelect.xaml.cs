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
namespace VesselStopOverPresentation.Finance
{
    /// <summary>
    /// Logique d'interaction pour AvoirSpotSelect.xaml
    /// </summary>
    public partial class AvoirSpotSelect : Window
    {
        public List<ElementFacturation> elementFactureArmateur { get; set; }
        public List<string> lstesc { get; set; }
        private Finance.AvoirSpot _frm;
        public AvoirSpotSelect(UTILISATEUR user, Finance.AvoirSpot frm )
        {
            InitializeComponent();
            elementFactureArmateur = new List<ElementFacturation>(); lstesc = new List<string>();
            this.DataContext = this;
            try
            {
                _frm = frm;

            }
            catch (Exception ex)
            { 
             //MessageBox.Show("
            }
        }

        private void btnselection_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Etes vous sûr de vouloir ajouter ces élements?", "Avoir Spot", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>().Count == 0)
                {
                    MessageBox.Show("Veuillez selectionner au moins un element", "Avoir Spot");
                }
                else
                {
                    _frm.SetCompulsoryElement(dataGridEltsFact.SelectedItems.OfType<ElementFacturation>().ToList<ElementFacturation>());
                    this.Close();
                    
                }
            }
        }

        private void btnLoad_Click_1(object sender, RoutedEventArgs e)
        {
            if (txtEscal.Text.Trim() == "")
            {

                MessageBox.Show("Veullez saisir un numero d'escale", "Avoir Spot");
            }
            else
            {
                int nbr = lstesc.Where(s => s.Equals(txtEscal.Text.Trim())).Count();
                if (nbr == 0)
                {
                    VsomParameters data = new VsomParameters();
                    if (txtEscal.Text.Trim() == "2997")
                    {
                        List<ElementFacturation> lst = data.GetElementFacturationMaintenanceCtr(int.Parse(txtEscal.Text.Trim()));
                        if (lst.Count == 0)
                        { MessageBox.Show("Aucun élement trouvé", "Avoir Spot"); }
                        else
                        {
                            elementFactureArmateur.AddRange(lst);
                            dataGridEltsFact.ItemsSource = null;
                            dataGridEltsFact.ItemsSource = elementFactureArmateur;
                            lstesc.Add(txtEscal.Text.Trim());
                        }
                    }
                    else
                    {
                        List<ElementFacturation> lst = data.GetElementFacturationCompulsory(int.Parse(txtEscal.Text.Trim()));
                        if (lst.Count == 0)
                        { MessageBox.Show("Aucun élement trouvé", "Avoir Spot"); }
                        else
                        {
                            elementFactureArmateur.AddRange(lst);
                            dataGridEltsFact.ItemsSource = null;
                            dataGridEltsFact.ItemsSource = elementFactureArmateur;
                            lstesc.Add(txtEscal.Text.Trim());
                        }
                    }
                }
                else
                {
                    MessageBox.Show("les lignes de cet escale sont déja chargées dans la liste", "Avoir Spot");
                }
            }
            
        }
    }
}

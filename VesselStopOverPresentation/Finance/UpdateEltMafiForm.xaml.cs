﻿using System;
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

namespace VesselStopOverPresentation
{
    /// <summary>
    /// Logique d'interaction pour UpdateEltMafiForm.xaml
    /// </summary>
    public partial class UpdateEltMafiForm : Window
    {
        private MafiForm mafiForm;
        private List<ELEMENT_FACTURATION> elements;
        public List<string> elts { get; set; }

        private UTILISATEUR utilisateur;
        private List<OPERATION> operationsUser;

        private FormLoader formLoader;
       // private VsomParameters vsp = new VsomParameters();
        private VSOMAccessors vsomAcc;
        public UpdateEltMafiForm(MafiForm form, int idMafi, UTILISATEUR user)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                InitializeComponent();
                this.DataContext = this;

                elements = vsomAcc.GetElementFacturationMafiByIdMafi(idMafi);
                elts = new List<string>();
                foreach (ELEMENT_FACTURATION elt in elements)
                {
                    elts.Add(elt.LibEF);
                }

                mafiForm = form;

                utilisateur = user;
                operationsUser = vsomAcc.GetOperationsUtilisateur(utilisateur.IdU);

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

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                 vsomAcc = new VSOMAccessors();

                if (cbElt.SelectedIndex == -1)
                {
                    MessageBox.Show("Vous devez saisir sélectionner un élément de facturation", "Elément de facturation ?", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (txtElt.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le libellé pour cet élément de facture", "Libellé ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtPrixUnitaire.Focus();
                }
                else if (txtPrixUnitaire.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le prix unitaire pour cet élément de facture", "Prix unitaire ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtPrixUnitaire.Focus();
                }
                else if (txtQuantite.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir la quantité pour cet élément de facture", "Quantité ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtQuantite.Focus();
                }
                else if (txtCpte.Text.Trim() == "")
                {
                    MessageBox.Show("Vous devez saisir le compte comptable pour cet élément de facture", "Compte comptable ?", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtQuantite.Focus();
                }
                else
                {
                    txtQuantite.Text = txtQuantite.Text.Replace(" ", "").Replace(".", ",");

                    ELEMENT_FACTURATION elt = vsomAcc.UpdateEltFactMafi(elements.ElementAt<ELEMENT_FACTURATION>(cbElt.SelectedIndex).IdJEF, txtElt.Text.Trim(), Convert.ToDouble(txtPrixUnitaire.Text), Convert.ToDouble(txtQuantite.Text), txtCpte.Text.Trim(), utilisateur.IdU);

                    elements = vsomAcc.GetElementFacturationMafiByIdMafi(elt.IdMafi.Value);
                    elts = new List<string>();
                    foreach (ELEMENT_FACTURATION el in elements)
                    {
                        elts.Add(el.LibEF);
                    }
                    cbElt.ItemsSource = null;
                    cbElt.ItemsSource = elts;
                    cbElt.SelectedItem = elt.LibEF;

                    formLoader.LoadMafiForm(mafiForm, vsomAcc.GetMafiImportByIdMafi(elt.IdMafi.Value));

                    MessageBox.Show("Le prix unitaire et la quantité ont été mis à jour sur cette ligne de facturation", "Ligne de facturation mise à jour !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (EnregistrementInexistant ei)
            {
                MessageBox.Show(ei.Message, "Enregistrement inexistant", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Echec de l'opération !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {

            }
        }

        private void cbElt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbElt.SelectedIndex != -1)
                {
                    int indexElt = cbElt.SelectedIndex;
                    ELEMENT_FACTURATION elt = elements.ElementAt<ELEMENT_FACTURATION>(indexElt);
                    txtElt.Text = elt.LibEF;
                    txtPrixUnitaire.Text = elt.PUEF.HasValue ? elt.PUEF.ToString() : "";
                    txtQuantite.Text = elt.QTEEF.HasValue ? elt.QTEEF.ToString() : "";
                    txtCpte.Text = elt.CCArticle;
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

        private void txtMontant_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void txtQte_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}

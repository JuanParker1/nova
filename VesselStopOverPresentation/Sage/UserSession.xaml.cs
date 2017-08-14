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

using System.Data.Sql;
using System.Data.SqlClient;
namespace VesselStopOverPresentation.Sage
{
    /// <summary>
    /// Logique d'interaction pour UserSession.xaml
    /// </summary>
    public partial class UserSession : Window
    {
         SqlConnection con;
        List<SageUserSession> users;
        public UserSession()
        {
            InitializeComponent();
            SqlDataReader dr = null;
            try
            {
                con = new SqlConnection(); users = new List<SageUserSession>();
                con.ConnectionString = @"Data Source=192.168.0.28\SVR2012;Initial Catalog=SOCOMARPROD; User id=sa; Password=P@ssw0rd2012;";
                con.Open();
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de connexion à la base de donnée : \n " + ex.Message, "Lecture de donnée SAGE");
            } 
        }

        private void Refresh()
        {
            SqlDataReader dr = null;
            try
            {
                 
                SqlCommand cmd = new SqlCommand("select * from cbUserSession ", con);

                dr = cmd.ExecuteReader();
                string hash = string.Empty;
                users = new List<SageUserSession>();
                while (dr.Read())
                {

                    if (!(dr.IsDBNull(dr.GetOrdinal("cbUserName"))) && !(dr.IsDBNull(dr.GetOrdinal("CB_Type"))))
                    {
                        users.Add(new SageUserSession { NOM = dr.GetString(dr.GetOrdinal("cbUserName")), CB_TYPE = dr.GetString(dr.GetOrdinal("CB_Type")) });
                    }
                }
                dataGridEltOS.ItemsSource = null;
                dataGridEltOS.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Echec de l'opération " + ex.Message, "Lecture de donnée SAGE");
            }
            finally
            {
                if (dr != null) { dr.Dispose(); }
            }
        }

        private void dataGridEltOS_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if (dataGridEltOS.SelectedIndex != -1)
            {
                SageUserSession sus = (SageUserSession)dataGridEltOS.SelectedItem;
                if (MessageBox.Show("Confirmez vous la deconnexion de " + sus.NOM + "?", "Sage", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        SqlCommand cmdupd;
                        cmdupd = new SqlCommand("delete from cbUserSession where cbUserName=@nom", con);
                        cmdupd.Parameters.AddWithValue("@nom", sus.NOM);
                        cmdupd.ExecuteNonQuery();
                        MessageBox.Show("Opération effectuée");
                        Refresh();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Echec de l'opération : \n" + ex.Message, "Sage");
                    }
                }
            }
        }
    }

  public class SageUserSession
    {
        private string _nom; private string cb_type;
        public string NOM { get { return _nom;} set{_nom=value;} }
        public string CB_TYPE { get { return cb_type; } set { cb_type = value; } }
        public SageUserSession()
        { }
    }
}

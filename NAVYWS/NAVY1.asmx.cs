using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using VesselStopOverDataEF;
namespace NAVYWS
{
    /// <summary>
    /// Description résumée de NAVY1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Pour autoriser l'appel de ce service Web depuis un script à l'aide d'ASP.NET AJAX, supprimez les marques de commentaire de la ligne suivante. 
    // [System.Web.Script.Services.ScriptService]
    public class NAVY1 : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }
        [WebMethod]
        public string InsertBAE(int idBL, string emetteurLettre, string destinataireLettre, DateTime dateEmissionLettre, DateTime dateValiditeLettre, List<VEHICULE> vehs, string autresInfos, int idUser)
        {
            try
            {
                BON_ENLEVEMENT be = VSOM_EF_Marchal.InsertBonEnlevement(idBL, emetteurLettre, destinataireLettre, dateEmissionLettre, dateValiditeLettre, vehs, autresInfos, idUser);

            }
            catch (Exception ex)
            {
            }
            return "ok";
        }
    }
}

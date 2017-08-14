using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VesselStopOverDataEF;
namespace VSOM_API.Controllers
{
    public class WPFController : ApiController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/Navy/Test/")]
        public IHttpActionResult Test()
        { return Ok("ok"); }

        public IHttpActionResult InsertBAE(int idBL, string emetteurLettre, string destinataireLettre, DateTime dateEmissionLettre, DateTime dateValiditeLettre, List<VesselStopOverDataEF.VEHICULE> vehs, string autresInfos, int idUser)
        {
            try
            {

                BON_ENLEVEMENT bae = new VSOM_EF_Marchal().InsertBonEnlevement(idBL, emetteurLettre, destinataireLettre, dateEmissionLettre, dateValiditeLettre, vehs, autresInfos, idUser);
                return Ok(bae);
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(String.Format("E:\\enavy\\wpf_insert_bae_{0}_{1}_{2}_{3}_{4}_{5}_{6}_{7}.txt", idBL, idUser, DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second)))
                {
                    sw.WriteLine("message : " + ex.Message); sw.WriteLine("innerexception : " + ex.StackTrace);
                }


                var message = new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Demande non traitée : " + ex.Message)
                };
                throw new HttpResponseException(message);
            }
        }
    }
}

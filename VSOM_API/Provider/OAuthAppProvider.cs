using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security.OAuth;
using System.Threading.Tasks;
using System.Security.Claims;
namespace VSOM_API.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        /// <summary>
        /// methode qui retournera le token de connexion
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //recherche si le code dacces existe et correspond au compte 
            int nbr=0;
            using (var ctx = new RM_VSOMEntities1())
            {
                int idcompte = int.Parse(context.Password);
                 nbr = ctx.acces.Where(s => s.CODE==context.UserName && s.ID_COMPTE==idcompte).Count();
            }

            /*if (context.UserName == "admin" && context.Password == "pass")
            {
                identity.AddClaim(new Claim( ClaimTypes.Role,"admin"));
                identity.AddClaim(new Claim("username","admin"));
                identity.AddClaim(new Claim(ClaimTypes.Name,"administrateur"));
                context.Validated(identity);
            }
            else if ( context.Password == "1") // doit controler le pass word correctement
            {
            identity.AddClaim(new Claim( ClaimTypes.Role,"user"));
                identity.AddClaim(new Claim("username","user"));
                identity.AddClaim(new Claim(ClaimTypes.Name,"utilisateur"));
                context.Validated(identity);
            }*/
            if (nbr == 0)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, "user"));
                identity.AddClaim(new Claim("username", "user"));
                identity.AddClaim(new Claim(ClaimTypes.Name, "utilisateur"));
                context.Validated(identity);
            }
            else
            {
                context.SetError("invalid_grant", "user name and password are incorrect"); return;
            }


        }
    }
}
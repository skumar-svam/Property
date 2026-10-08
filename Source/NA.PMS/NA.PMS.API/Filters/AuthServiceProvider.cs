using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using NA.PMS.Service;

namespace NA.PMS.API.Filters
{
    public class AuthServiceProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var user = false;
            string userName = string.Empty;
            string password = string.Empty;

            if (!string.IsNullOrEmpty(context.UserName) && !string.IsNullOrEmpty(context.Password))
            {
                ILoginService _loginservice = new LoginService();
                user = _loginservice.ValidateUser(context.UserName, context.Password);
            }

            if (user == false)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Role, context.UserName));
            identity.AddClaim(new Claim("username", context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Name, context.Password));
            context.Validated(identity);
        }
    }
}
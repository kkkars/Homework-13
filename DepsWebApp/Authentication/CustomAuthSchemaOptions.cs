using Microsoft.AspNetCore.Authentication;

namespace DepsWebApp.Authentication
{
    public class CustomAuthSchemaOptions : AuthenticationSchemeOptions
    {
        public CustomAuthSchemaOptions()
        {
            ClaimsIssuer = CustomAuthSchema.Issuer;
        }
    }
}

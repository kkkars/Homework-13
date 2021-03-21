using System.Collections.Generic;
using System.Security.Claims;

namespace DepsWebApp.Authentication
{
    public class AccountIdentity : ClaimsIdentity
    {
        public string Login { get; set; }
        public string Password { get; set; }

        public AccountIdentity()
        {
        }

        public AccountIdentity(string login, string password)
            : base(CreateClaimsIdentity(login, password), CustomAuthSchema.Type)
        {
            Login = login;
            Password = password;
        }

        private static IEnumerable<Claim> CreateClaimsIdentity(string login, string password)
        {
            var result = new List<Claim>();

            if (login != null)
            {
                result.Add(new Claim(DefaultNameClaimType, login));
            }

            if (password != null)
            {
                result.Add(new Claim("Password", password));
            }

            return result;
        }
    }
}

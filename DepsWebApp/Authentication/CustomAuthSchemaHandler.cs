using DepsWebApp.Models;
using DepsWebApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace DepsWebApp.Authentication
{
    public class CustomAuthSchemaHandler : AuthenticationHandler<CustomAuthSchemaOptions>
    {
        private readonly IDbService _dbService;

        public CustomAuthSchemaHandler(
            IDbService dbService,
            IOptionsMonitor<CustomAuthSchemaOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _dbService = dbService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!TryGetEncodedDataFromRequest(Request, out var encodedData))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var account = DecodeModel(encodedData);

                var isExist = await _dbService.Find(account);

                if (!isExist) return AuthenticateResult.NoResult();

                return AuthenticateResult.Success(
                    new AuthenticationTicket(
                        new ClaimsPrincipal(new AccountIdentity(account.LoginId, account.Password)),
                        CustomAuthSchema.Name));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error during authentication");
                return AuthenticateResult.Fail(ex);
            }
        }

        private static bool TryGetEncodedDataFromRequest(HttpRequest request, out string encodedData)
        {
            encodedData = null;
            if (request.Headers.ContainsKey(HeaderNames.Authorization))
            {
                encodedData = request.Headers[HeaderNames.Authorization].FirstOrDefault();
            }
            return !string.IsNullOrEmpty(encodedData);
        }

        private Account DecodeModel(string encodedData)
        {
            var authHeaderValue = AuthenticationHeaderValue.Parse(encodedData);
            Account account = new Account();

            if (authHeaderValue.Scheme.Equals(AuthenticationSchemes.Basic.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                var credentials = Encoding.UTF8
                                    .GetString(Convert.FromBase64String(authHeaderValue.Parameter ?? string.Empty))
                                    .Split(':', 2);
                account.LoginId = credentials[0];
                account.Password = credentials[1];
            }

            return account;
        }
    }
}

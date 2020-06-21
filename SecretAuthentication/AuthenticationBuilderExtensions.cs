using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecretAuthentication;

namespace AspNetCore.Advanced.SecretAuthentication
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddSecretAuthentication(this AuthenticationBuilder builder, Action<SecretAuthenticationOptions> configureOptions)
        {
            builder.AddScheme<SecretAuthenticationOptions, SecretAuthenticationHandler>(SecretAuthenticationDefaults.AuthenticationSchema, configureOptions);
            return builder;
        }
    }

    public class SecretAuthenticationOptions: AuthenticationSchemeOptions
    {
    }

    public class SecretAuthenticationHandler: AuthenticationHandler<SecretAuthenticationOptions>
    {
        private static string s_authenticationHeaderName = "Authorization";

        public SecretAuthenticationHandler(IOptionsMonitor<SecretAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(s_authenticationHeaderName, out var requestApiKey))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }


            if (!_apiKeyConfiguration.GetApiKeyValue()
                .Equals(requestApiKey[0], StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Wrong Api Key"));
            }
            var identity = new ClaimsIdentity(Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}

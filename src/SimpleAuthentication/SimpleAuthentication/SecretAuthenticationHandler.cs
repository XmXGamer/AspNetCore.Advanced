﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SecretAuthentication;

namespace AspNetCore.Advanced.SecretAuthentication
{

    public class SecretAuthenticationHandler : AuthenticationHandler<SecretAuthenticationOptions>
    {
        private static string s_authenticationHeaderName = "Authorization";

        public SecretAuthenticationHandler(IOptionsMonitor<SecretAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(s_authenticationHeaderName, out var secrets))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (Options.Secret is null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!secrets[0].StartsWith(SecretAuthenticationDefaults.AuthenticationSchema))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            var secret = secrets[0].Replace($"{SecretAuthenticationDefaults.AuthenticationSchema} ", "");

            if (!Options.Secret
                .Equals(secret, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(AuthenticateResult.Fail("Wrong Secret"));
            }
            var identity = new ClaimsIdentity(Scheme.Name);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, secret));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
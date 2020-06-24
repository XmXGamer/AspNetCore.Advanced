using System;
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
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddSecretAuthentication(this AuthenticationBuilder builder, Action<SecretAuthenticationOptions> configureOptions)
        {
            builder.AddScheme<SecretAuthenticationOptions, SecretAuthenticationHandler>(SecretAuthenticationDefaults.AuthenticationSchema, configureOptions);
            return builder;
        }

        public static AuthenticationBuilder AddEmptySchemaSecretAuthentication(this AuthenticationBuilder builder, Action<SecretAuthenticationOptions> configureOptions)
        {
            builder.AddScheme<SecretAuthenticationOptions, EmptySchemaSecretAuthenticationHandler>(SecretAuthenticationDefaults.EmptyAuthenticationSchema, configureOptions);
            return builder;
        }
    }
}
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

    public class SecretAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string? Secret { get; set; }
    }
}
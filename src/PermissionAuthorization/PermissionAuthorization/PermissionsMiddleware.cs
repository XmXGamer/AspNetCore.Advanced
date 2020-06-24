using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PermissionAuthorization
{
    public class PermissionsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IPermissionSource> _permissionSources;

        public PermissionsMiddleware(RequestDelegate next, IEnumerable<IPermissionSource> permissionSources)
        {
            _next = next;
            _permissionSources = permissionSources;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context?.User is null || !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var permissionSets = _permissionSources.Select(x => x.GetPermissions(context));

            var identities = permissionSets.Select(x => new ClaimsIdentity(x.Select(y => new Claim("permissions", y))));
            context.User.AddIdentities(identities);
            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
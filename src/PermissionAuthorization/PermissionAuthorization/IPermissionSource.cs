using System.Collections.Immutable;
using Microsoft.AspNetCore.Http;

namespace PermissionAuthorization
{
    public interface IPermissionSource
    {
        IImmutableList<string> GetPermissions(HttpContext? context);
    }
}
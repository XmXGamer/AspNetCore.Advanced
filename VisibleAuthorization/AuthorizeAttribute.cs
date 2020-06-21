using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCore.Advanced.Authorization
{
    [AttributeUsage(validOn:AttributeTargets.Property)]
    public class VisibleAuthorizeAttribute: AuthorizeAttribute
    {
       
    }
}

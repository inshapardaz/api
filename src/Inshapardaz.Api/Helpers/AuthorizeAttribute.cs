using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Helpers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly IList<Role> _roles;

        public AuthorizeAttribute(params Role[] roles)
        {
            _roles = roles ?? new Role[] { };
        }

        public string LibraryKey { get; set; } = "libraryId";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var account = (AccountModel)context.HttpContext.Items["Account"];
            if (!_roles.Any() && account != null) return;

            var libraries = (IEnumerable<LibraryModel>)context.HttpContext.Items["Libraries"];
            if (account == null)
            {
                // not logged in
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                if (account.IsSuperAdmin)
                {
                    return;
                }
                else if (_roles.Any() && context.RouteData.Values.ContainsKey(LibraryKey))
                {
                    int libraryId;
                    if (int.TryParse(context.RouteData.Values[LibraryKey].ToString(), out libraryId))
                    {
                        var library = libraries.SingleOrDefault(l => l.Id == libraryId);

                        if (library != null && _roles.Contains(library.Role))
                        {
                            return;
                        }
                    }
                }

                context.Result = new JsonResult(new { message = "Forbidden." }) { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }
}

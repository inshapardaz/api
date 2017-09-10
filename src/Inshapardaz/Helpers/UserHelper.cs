using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => _contextAccessor.HttpContext.User.Identity.IsAuthenticated;

        // public bool IsAdmin => IsAuthenticated && _contextAccessor.HttpContext.User.IsInRole(UserRoles.Admin.ToDescription());
        // public bool IsContributor => IsAdmin || (IsAuthenticated && _contextAccessor.HttpContext.User.IsInRole(UserRoles.Contributor.ToDescription()));
        // public bool IsReader => IsContributor || (IsAuthenticated && _contextAccessor.HttpContext.User.IsInRole(UserRoles.Reader.ToDescription()));

        public bool IsAdmin => IsAuthenticated;

        public bool IsContributor => IsAuthenticated;

        public bool IsReader => IsAuthenticated;

        public Guid GetUserId()
        {
            var nameIdentifier = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (nameIdentifier != null)
            {
                return Guid.Parse(nameIdentifier);
            }
            
            return Guid.Empty;
        }

    }
}
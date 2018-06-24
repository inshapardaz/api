using System;
using System.Linq;
using System.Security.Claims;
using Inshapardaz.Domain.Helpers;
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

        public bool IsAdmin => IsAuthenticated; // && (IsUserInRole("Administrator") ||IsUserInRole("Owner"));

        public bool IsContributor => IsAdmin;  //|| (IsAuthenticated && IsUserInRole("Contributor"));

        public bool IsReader => IsAdmin;// || (IsAuthenticated && IsUserInRole("Reader"));
        
        public Guid GetUserId()
        {
            var nameIdentifier = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
            if (nameIdentifier != null)
            {
                return Guid.Parse(nameIdentifier);
            }
            
            return Guid.Empty;
        }

        private bool IsUserInRole(string role)
        {
            return _contextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, role);
        }
    }
}
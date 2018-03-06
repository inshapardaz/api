﻿using System;
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

        public bool IsAdmin => IsAuthenticated && _contextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, "Administrator");

        public bool IsContributor => IsAdmin || (IsAuthenticated && _contextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, "Contributor"));

        public bool IsReader => IsAdmin || (IsAuthenticated && _contextAccessor.HttpContext.User.HasClaim(ClaimTypes.Role, "Reader"));
        
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
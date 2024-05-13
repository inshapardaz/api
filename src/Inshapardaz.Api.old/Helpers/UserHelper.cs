using Microsoft.AspNetCore.Http;
using Inshapardaz.Api.Entities;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using System.Collections.Generic;
using System.Linq;

namespace Inshapardaz.Api.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public bool IsAuthenticated => Account != null;

        public bool IsAdmin => IsAuthenticated && IsUserInRole(Role.Admin);

        public bool IsLibraryAdmin(int libraryId) => IsAuthenticated && (IsAdmin || IsUserInRole(Role.LibraryAdmin, libraryId));

        public bool IsWriter(int libraryId) => IsAuthenticated && (IsLibraryAdmin(libraryId) || IsUserInRole(Role.Writer, libraryId));

        public bool IsReader(int libraryId) => IsAuthenticated && (IsWriter(libraryId) || IsUserInRole(Role.Reader, libraryId));

        public AccountModel Account => (AccountModel)_contextAccessor.HttpContext.Items["Account"];

        public bool IsUserInRole(Role role, int? libraryId = null)
        {
            var account = (AccountModel)_contextAccessor.HttpContext.Items["Account"];
            if (role == Role.Admin && account.IsSuperAdmin) return true;

            var libraries = (IEnumerable<LibraryModel>)_contextAccessor.HttpContext.Items["Libraries"];
            return libraries.Any(l => l.Id == libraryId && l.Role == role);
        }
    }
}

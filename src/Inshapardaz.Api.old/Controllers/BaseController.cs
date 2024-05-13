using Microsoft.AspNetCore.Mvc;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Exception;
using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Api.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        public int Account => (int)HttpContext.Items["Account"];
        public IEnumerable<LibraryModel> Libraries => (IEnumerable<LibraryModel>)HttpContext.Items["Libraries"];
    }
}

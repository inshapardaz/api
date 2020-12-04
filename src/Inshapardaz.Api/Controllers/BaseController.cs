using Microsoft.AspNetCore.Mvc;
using Inshapardaz.Api.Entities;

namespace Inshapardaz.Api.Controllers
{
    [Controller]
    public abstract class BaseController : ControllerBase
    {
        public Account Account => (Account)HttpContext.Items["Account"];
    }
}

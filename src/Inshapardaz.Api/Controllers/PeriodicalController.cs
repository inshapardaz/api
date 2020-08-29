using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Controllers
{
    public class PeriodicalController : Controller
    {
        public IActionResult GetPeriodicals(int libraryId)
        {
            return View();
        }
    }
}

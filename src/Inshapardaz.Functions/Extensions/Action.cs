using Inshapardaz.Domain.Exception;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Extensions
{
    public static class Action
    {
        public static async Task<IActionResult> Execute(Func<Task<IActionResult>> action, ClaimsPrincipal principal)
        {
            try
            {
                return await action();
            }
            catch (UnauthorizedException ex)
            {
                if (principal == null)
                {
                    return new UnauthorizedResult();
                }

                return new ForbidResult();
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }
    }
}

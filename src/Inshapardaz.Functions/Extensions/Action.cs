using Inshapardaz.Domain.Exception;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Extensions
{
    public static class Action
    {
        public static async Task<IActionResult> Execute(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (UnauthorizedException)
            {
                return new UnauthorizedResult();
            }
            catch (ForbiddenException)
            {
                return new ForbidResult();
            }
            catch
            {
                return new StatusCodeResult(500);
            }
        }
    }
}

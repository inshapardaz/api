using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Converters;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Domain.Models.Handlers;
using Microsoft.AspNetCore.Mvc;
using Paramore.Darker;

namespace Inshapardaz.Api.Controllers
{
    public class LibraryController : Controller
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderLibrary _libraryRenderer;
        private readonly IUserHelper _userHelper;

        public LibraryController(IQueryProcessor queryProcessor, IRenderLibrary libraryRenderer, IUserHelper userHelper)
        {
            _queryProcessor = queryProcessor;
            _libraryRenderer = libraryRenderer;
            _userHelper = userHelper;
        }

        [HttpGet("library/{libraryId}")]
        public async Task<IActionResult> GetLibraryById(int libraryId, CancellationToken cancellationToken)
        {
            var query = new GetLibraryQuery(libraryId, _userHelper.Claims);
            var library = await _queryProcessor.ExecuteAsync(query, cancellationToken);
            return new OkObjectResult(_libraryRenderer.Render(library));
        }
    }
}

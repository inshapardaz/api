using Inshapardaz.Api.Controllers;
using Inshapardaz.Api.Mappings;
using Inshapardaz.Api.Views;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Converters;

public interface IRenderFile
{
    FileView Render(int libraryId, FileModel source);
}

public class FileRenderer(IRenderLink linkRenderer, IUserHelper userHelper) : IRenderFile
{
    public FileView Render(int libraryId, FileModel source)
    {
        var result = source.Map();
        var links = new List<LinkView>
        {
            linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.GetLibraryFile),
                Method = HttpMethod.Get,
                Rel = RelTypes.Self,
                Parameters = new { libraryId = libraryId, fileId = source.Id },
            })
        };

        if (userHelper.IsAdmin)
        {
            links.Add(linkRenderer.Render(new Link
            {
                ActionName = nameof(FileController.DeleteFile),
                Method = HttpMethod.Delete,
                Rel = RelTypes.Delete,
                Parameters = new { fileId = source.Id },
            }));
        }

        result.Links = links;

        return result;
    }
}

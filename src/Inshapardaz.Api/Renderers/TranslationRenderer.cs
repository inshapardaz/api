using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderTranslation
    {
        TranslationView Render(Translation source);
    }

    public class TranslationRenderer : IRenderTranslation
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderEnum _enumRenderer;
        private readonly IUserHelper _userHelper;

        public TranslationRenderer(IRenderLink linkRenderer,
            IRenderEnum enumRenderer,
            IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public TranslationView Render(Translation source)
        {
            var result = source.Map<Translation, TranslationView>();

            result.Language = _enumRenderer.Render(source.Language);

            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetTranslationById", "self", new { id = source.Id }),
                _linkRenderer.Render("GetWordDetailsById", "worddetail", new { id = source.WordDetailId })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateTranslation", "update", new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteTranslation", "delete", new { id = source.Id }));
            }
            result.Links = links;
            return result;
        }
    }
}
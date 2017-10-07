using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public class TranslationRenderer : RendrerBase, IRenderResponseFromObject<Translation, TranslationView>
    {
        private readonly IRenderEnum _enumRenderer;
        private readonly IUserHelper _userHelper;

        public TranslationRenderer(IRenderLink linkRenderer,
            IRenderEnum enumRenderer,
            IUserHelper userHelper)
            : base(linkRenderer)
        {
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public TranslationView Render(Translation source)
        {
            var result = source.Map<Translation, TranslationView>();

            result.Language = _enumRenderer.Render((Languages)source.Language);

            var links = new List<LinkView>
            {
                LinkRenderer.Render("GetTranslationById", "self", new { id = source.Id }),
                LinkRenderer.Render("GetWordDetailsById", "worddetail", new { id = source.WordDetailId })
            };

            if (_userHelper.IsContributor)
            {
                links.Add(LinkRenderer.Render("UpdateTranslation", "update", new { id = source.Id }));
                links.Add(LinkRenderer.Render("DeleteTranslation", "delete", new { id = source.Id }));
            }
            result.Links = links;
            return result;
        }
    }
}
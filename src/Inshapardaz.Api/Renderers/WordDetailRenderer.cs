using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderWordDetail
    {
        WordDetailView Render(WordDetail source);
    }

    public class WordDetailRenderer : IRenderWordDetail
    {
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderEnum _enumRenderer;

        private readonly IUserHelper _userHelper;

        public WordDetailRenderer(IRenderLink linkRenderer, IRenderEnum enumRenderer, IUserHelper userHelper)
        {
            _linkRenderer = linkRenderer;
            _enumRenderer = enumRenderer;
            _userHelper = userHelper;
        }

        public WordDetailView Render(WordDetail source)
        {
            var result = source.Map<WordDetail, WordDetailView>();

            result.Attributes = _enumRenderer.RenderFlags(source.Attributes).Trim(',');
            result.Language = _enumRenderer.Render((Languages)source.Language);
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetDetailsById", "self", new {id = source.Id}),
                _linkRenderer.Render("GetWordById", "word", new {id = source.WordInstanceId}),
                _linkRenderer.Render("GetWordTranslationsByDetailId", "translations", new {id = source.Id}),
                _linkRenderer.Render("GetWordMeaningByWordDetailId", "meanings", new {id = source.Id})
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateWordDetail", "update", new { id = source.Id }));
                links.Add(_linkRenderer.Render("DeleteWordDetail", "delete", new { id = source.Id }));
                links.Add(_linkRenderer.Render("AddTranslation", "addTranslation", new { id = source.Id }));
                links.Add(_linkRenderer.Render("AddMeaning", "addMeaning", new { id = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
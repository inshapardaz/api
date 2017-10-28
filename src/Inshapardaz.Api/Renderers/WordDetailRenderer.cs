using System.Collections.Generic;
using Inshapardaz.Api.Helpers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderWordDetail
    {
        WordDetailView Render(WordDetail source, int dictionaryId);
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

        public WordDetailView Render(WordDetail source, int dictionaryId)
        {
            var result = source.Map<WordDetail, WordDetailView>();

            result.Attributes = _enumRenderer.RenderFlags(source.Attributes).Trim(',');
            result.Language = _enumRenderer.Render((Languages)source.Language);
            var links = new List<LinkView>
            {
                _linkRenderer.Render("GetDetailsById", RelTypes.Self, new {id = dictionaryId, detailId = source.Id}),
                _linkRenderer.Render("GetWordById", "word", new {id = dictionaryId, wordId =source.WordInstanceId}),
                _linkRenderer.Render("GetWordTranslationsByDetailId", "translations", new {id = dictionaryId, wordId = source.Id}),
                _linkRenderer.Render("GetWordMeaningByWordDetailId", "meanings", new {id = dictionaryId, detailId = source.Id})
            };

            if (_userHelper.IsContributor)
            {
                links.Add(_linkRenderer.Render("UpdateWordDetail", RelTypes.Update, new { id = dictionaryId, detailId = source.Id }));
                links.Add(_linkRenderer.Render("DeleteWordDetail", RelTypes.Delete, new { id = dictionaryId, detailId = source.Id }));
                links.Add(_linkRenderer.Render("AddTranslation", RelTypes.AddTranslation, new { id = dictionaryId, detailId = source.Id }));
                links.Add(_linkRenderer.Render("AddMeaning", RelTypes.AddMeaning, new { id = dictionaryId, detailId = source.Id }));
            }

            result.Links = links;
            return result;
        }
    }
}
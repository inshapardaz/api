using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Api.Renderers
{
    public interface IRenderWordMeanings
    {
        IEnumerable<MeaningContextView> Render(Word source, int dictionaryId);
    }

    public class WordMeaningsRenderer : IRenderWordMeanings
    {
        private const string DefaultContext = "Default";
        private readonly IRenderLink _linkRenderer;
        private readonly IRenderMeaning _meaningRenderer;

        public WordMeaningsRenderer(IRenderLink linkRenderer, IRenderMeaning meaningRenderer)
        {
            _linkRenderer = linkRenderer;
            _meaningRenderer = meaningRenderer;
        }

        public IEnumerable<MeaningContextView> Render(Word source, int dictionaryId)
        {
            return source
                   .Meaning
                   .GroupBy(m => m.Context)
                   .Select(
                    group => new MeaningContextView
                    {
                        Context = !string.IsNullOrWhiteSpace(group.Key) ? group.Key : DefaultContext,
                        Links =
                            new[]
                                {
                                    _linkRenderer.Render("GetWordMeaningByContext", RelTypes.Self,
                                        new { id = dictionaryId, wordId = source.Id, context = !string.IsNullOrWhiteSpace(group.Key) ? group.Key : DefaultContext})
                                },
                        Meanings = group.ToList().Select(v => _meaningRenderer.Render(v, dictionaryId))
                    }).ToList();
        }
    }
}
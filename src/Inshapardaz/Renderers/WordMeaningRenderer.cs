using System.Collections.Generic;
using System.Linq;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class WordMeaningRenderer : RendrerBase, IRenderResponseFromObject<WordDetail, IEnumerable<MeaningContextView>>
    {
        private const string DefaultContext = "Default";
        private readonly IRenderResponseFromObject<Meaning, MeaningView> _meaningRenderer;

        public WordMeaningRenderer(IRenderLink linkRenderer, IRenderResponseFromObject<Meaning, MeaningView> meaningRenderer)
            : base(linkRenderer)
        {
            _meaningRenderer = meaningRenderer;
        }

        public IEnumerable<MeaningContextView> Render(WordDetail source)
        {
            return source
                   .Meaning
                   .GroupBy(m => m.Context)
                   .Select(
                    @group => new MeaningContextView
                    {
                        Context = !string.IsNullOrWhiteSpace(@group.Key) ? @group.Key : DefaultContext,
                        Links =
                            new[]
                                {
                                    LinkRenderer.Render(
                                        "GetWordMeaningByContext",
                                        "self",
                                        new { id = source.Id, context = !string.IsNullOrWhiteSpace(@group.Key) ? @group.Key : DefaultContext})
                                },
                        Meanings = @group.ToList().Select(v => _meaningRenderer.Render(v))
                    }).ToList();
        }
    }
}
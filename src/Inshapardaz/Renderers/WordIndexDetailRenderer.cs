using System;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Api.Renderers
{
    public class WordIndexDetailRenderer : RendrerBase, IRenderResponseFromObject<Word, WordDetailView>
    {
        private readonly IRenderResponseFromObject<Word, WordView> _wordRenderer;

        public WordIndexDetailRenderer(IRenderLink linkrenderer, IRenderResponseFromObject<Word, WordView> wordRenderer)
            : base(linkrenderer)
        {
            _wordRenderer = wordRenderer;
        }

        public WordDetailView Render(Word source)
        {
            throw new NotImplementedException();
            //var result = source.Map<Word, WordDetailView>();

            //result.Links = new[] { LinkRenderer.Render("GetWordIndexById", "self", new { id = result.Id }) };

            //var words = new List<WordView>();
            //if (source.Words != null)
            //{
            //    words.AddRange(source.Words.Select(_wordRenderer.Render));
            //}

            //result.Words = words;

            //return result;
        }
    }
}
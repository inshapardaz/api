using System.Collections.Generic;
using Inshapardaz.Model;

namespace Inshapardaz.Renderers
{
    public class DictionaryIndexRenderer : RendrerBase, IRenderResponse<IndexView>
    {
        public DictionaryIndexRenderer(IRenderLink linkRenderer)
            : base(linkRenderer)
        {
        }

        public IndexView Render()
        {
            return new IndexView
                       {
                            Links = new List<LinkView>
                                        {
                                           LinkRenderer.Render("DictionaryIndex", "self", null)
                                        },
                           Indexes = GetWordLinks()
                       };
        }

        private IEnumerable<LinkView> GetWordLinks()
        {
            return new List<LinkView>
            {
                LinkRenderer.Render("GetWordsListStartWith", "آ", new { title = "آ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ا", new { title = "ا" }),
                LinkRenderer.Render("GetWordsListStartWith", "ب", new { title = "ب" }),
                LinkRenderer.Render("GetWordsListStartWith", "پ", new { title = "پ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ت", new { title = "ت" }),
                LinkRenderer.Render("GetWordsListStartWith", "ٹ", new { title = "ٹ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ث", new { title = "ث" }),
                LinkRenderer.Render("GetWordsListStartWith", "ج", new { title = "ج" }),
                LinkRenderer.Render("GetWordsListStartWith", "چ", new { title = "چ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ح", new { title = "ح" }),
                LinkRenderer.Render("GetWordsListStartWith", "خ", new { title = "خ" }),
                LinkRenderer.Render("GetWordsListStartWith", "د", new { title = "د" }),
                LinkRenderer.Render("GetWordsListStartWith", "ڈ", new { title = "ڈ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ذ", new { title = "ذ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ر", new { title = "ر" }),
                LinkRenderer.Render("GetWordsListStartWith", "ڑ", new { title = "ڑ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ز", new { title = "ز" }),
                LinkRenderer.Render("GetWordsListStartWith", "ژ", new { title = "ژ" }),
                LinkRenderer.Render("GetWordsListStartWith", "س", new { title = "س" }),
                LinkRenderer.Render("GetWordsListStartWith", "ش", new { title = "ش" }),
                LinkRenderer.Render("GetWordsListStartWith", "ص", new { title = "ص" }),
                LinkRenderer.Render("GetWordsListStartWith", "ض", new { title = "ض" }),
                LinkRenderer.Render("GetWordsListStartWith", "ط", new { title = "ط" }),
                LinkRenderer.Render("GetWordsListStartWith", "ظ", new { title = "ظ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ع", new { title = "ع" }),
                LinkRenderer.Render("GetWordsListStartWith", "غ", new { title = "غ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ف", new { title = "ف" }),
                LinkRenderer.Render("GetWordsListStartWith", "ق", new { title = "ق" }),
                LinkRenderer.Render("GetWordsListStartWith", "ک", new { title = "ک" }),
                LinkRenderer.Render("GetWordsListStartWith", "گ", new { title = "گ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ل", new { title = "ل" }),
                LinkRenderer.Render("GetWordsListStartWith", "م", new { title = "م" }),
                LinkRenderer.Render("GetWordsListStartWith", "ن", new { title = "ن" }),
                LinkRenderer.Render("GetWordsListStartWith", "و", new { title = "و" }),
                LinkRenderer.Render("GetWordsListStartWith", "ہ", new { title = "ہ" }),
                LinkRenderer.Render("GetWordsListStartWith", "ء", new { title = "ء" }),
                LinkRenderer.Render("GetWordsListStartWith", "ی", new { title = "ی" }),
                LinkRenderer.Render("GetWordsListStartWith", "ے", new { title = "ے" }),
            };
        }
    }
}
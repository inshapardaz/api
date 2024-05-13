using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using MarkdownSharp;
using HtmlToOpenXml;

namespace Inshapardaz.Domain.Adapters
{
    public interface IWriteWordDocument
    {
        byte[] ConvertMarkdownToWord(IEnumerable<string> chapters);
    }

    public class WordDocumentWriter : IWriteWordDocument
    {
        private readonly ILogger<WordDocumentWriter> _logger;

        public WordDocumentWriter(ILogger<WordDocumentWriter> logger)
        {
            _logger = logger;
        }
        public byte[] ConvertMarkdownToWord(IEnumerable<string> chapters)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();

                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    foreach (string chapter in chapters)
                    {
                        Markdown markdown = new Markdown();
                        string htmlContent = markdown.Transform(chapter);

                        HtmlConverter converter = new HtmlConverter(mainPart);
                        IEnumerable<OpenXmlCompositeElement> elements = converter.Parse(htmlContent);

                        foreach (var element in elements)
                        {
                            body.AppendChild(element);
                        }

                        Paragraph paragraph = body.AppendChild(new Paragraph());

                        paragraph.AppendChild(new Break() { Type = BreakValues.Page });
                    }
                }

                return stream.ToArray();
            }
        }
    }
}

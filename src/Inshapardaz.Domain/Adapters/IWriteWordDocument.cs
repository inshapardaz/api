using System.Linq;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.ExtendedProperties;
using Inshapardaz.Domain.Helpers;
using MarkdownSharp;
using HtmlToOpenXml;

namespace Inshapardaz.Domain.Adapters;

public interface IWriteWordDocument
{
    byte[] ConvertMarkdownToWord(IEnumerable<string> chapters, string publisher = null);
}

public class WordDocumentWriter(ILogger<WordDocumentWriter> logger) : IWriteWordDocument
{
    private readonly ILogger<WordDocumentWriter> _logger = logger;

    public byte[] ConvertMarkdownToWord(IEnumerable<string> chapters, string publisher = null)
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

                    body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));
                }

                ReplacePageBreakMarkersWithBookmarks(body);

                var extendedProperties = doc.AddExtendedFilePropertiesPart();
                extendedProperties.Properties = new Properties(new Company(string.IsNullOrWhiteSpace(publisher) ? "Nawishta" : publisher));
            }

            return stream.ToArray();
        }
    }

    // Turns each `::pagebreak` marker left in the converted text into an invisible Word bookmark
    // (no text, no break) so the original page boundary can be located without any visual effect.
    private static void ReplacePageBreakMarkersWithBookmarks(Body body)
    {
        var pageNumber = 0;

        foreach (var text in body.Descendants<Text>().Where(t => t.Text.Contains(MarkdownHelper.PageBreakMarker)).ToList())
        {
            var run = text.Parent as Run;
            var parent = run?.Parent;
            if (parent == null)
            {
                continue;
            }

            var segments = text.Text.Split(MarkdownHelper.PageBreakMarker);
            OpenXmlElement anchor = run;

            for (var i = 0; i < segments.Length; i++)
            {
                if (i > 0)
                {
                    pageNumber++;
                    var bookmarkId = pageNumber.ToString();
                    anchor = parent.InsertAfter(new BookmarkStart { Id = bookmarkId, Name = $"page{pageNumber}" }, anchor);
                    anchor = parent.InsertAfter(new BookmarkEnd { Id = bookmarkId }, anchor);
                }

                if (segments[i].Length > 0)
                {
                    var newRun = (Run)run.CloneNode(true);
                    var newText = newRun.GetFirstChild<Text>();
                    newText.Text = segments[i];
                    newText.Space = SpaceProcessingModeValues.Preserve;
                    anchor = parent.InsertAfter(newRun, anchor);
                }
            }

            run.Remove();
        }
    }
}

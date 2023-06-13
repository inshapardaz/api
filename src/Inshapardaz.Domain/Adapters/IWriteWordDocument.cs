using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Inshapardaz.Domain.Adapters
{
    public interface IWriteWordDocument
    {
        byte[] ConvertTextToWord(IEnumerable<string >chapters);
    }

    public class WordDocumentWriter : IWriteWordDocument
    {
        private readonly ILogger<WordDocumentWriter> _logger;

        public WordDocumentWriter(ILogger<WordDocumentWriter> logger)
        {
            _logger = logger;
        }
        public byte[] ConvertTextToWord(IEnumerable<string> chapters)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                // Create a Word document
                using (WordprocessingDocument doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    // Add a main part to the document
                    MainDocumentPart mainPart = doc.AddMainDocumentPart();

                    // Create a new document and body
                    mainPart.Document = new Document();
                    Body body = mainPart.Document.AppendChild(new Body());

                    // Loop through each chapter
                    foreach (string chapter in chapters)
                    {
                        // Add a new paragraph for the chapter
                        Paragraph paragraph = body.AppendChild(new Paragraph());
                        Run run = paragraph.AppendChild(new Run());
                        run.AppendChild(new Text(chapter));

                        // Add a new page break after each chapter
                        paragraph.AppendChild(new Break() { Type = BreakValues.Page });
                    }
                }

                // Return the byte array of the Word document
                return stream.ToArray();
            }
        }

        private static void ReleaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (System.Exception ex)
            {
                obj = null;
                Console.WriteLine("Unable to release the Object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
    }
}

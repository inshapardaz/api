using PDFiumSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Inshapardaz.Domain.Adapters;

public interface IConvertPdf
{
    Dictionary<string, byte[]> ExtractImagePages(byte[] pdfContent);
}

public class PdfConverter : IConvertPdf
{
    public Dictionary<string, byte[]> ExtractImagePages(byte[] pdfContent)
    {
        var pageImages = new Dictionary<string, byte[]>();
        using (var doc = new PdfDocument(pdfContent))
        {
            int i = 0;
            foreach (var page in doc.Pages)
            {
                using (var bitmap = new PDFiumBitmap((int)page.Width, (int)page.Height, false))
                {
                    bitmap.Fill(new PDFiumSharp.Types.FPDF_COLOR(255, 255, 255));
                    page.Render(bitmap);
                    var bmp = Image.FromStream(bitmap.AsBmpStream());
                    using (var ms = new MemoryStream())
                    {
                        bmp.Save(ms, ImageFormat.Jpeg);
                        pageImages.Add($"{i++.ToString("000")}.jpg", ms.ToArray());
                    }
                }
            }
        }

        return pageImages;
    }
}

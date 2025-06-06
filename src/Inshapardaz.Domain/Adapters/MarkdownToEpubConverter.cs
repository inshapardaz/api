using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Markdig;

namespace Inshapardaz.Domain.Adapters;

public class MarkdownToEpubConverter
{
    public record Chapter(string Title, string MarkdownContent);
    
    public byte[] CreateEpub(
        BookModel book,
        List<Chapter> chapters,
        string outputFilePath,
        byte[] coverImage = null)
    {
        var direction = book.Language == "ur" ? "rtl" : "ltr";
        string epubFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        string oebpsDir = Path.Combine(epubFolder, "OEBPS");
        string metaInfDir = Path.Combine(epubFolder, "META-INF");
        Directory.CreateDirectory(epubFolder);
        Directory.CreateDirectory(oebpsDir);
        Directory.CreateDirectory(metaInfDir);
        
        var manifest = new StringBuilder();
        var spine = new StringBuilder();
        var navItems = new StringBuilder();

        WriteMimeType(epubFolder);

        WriteContainerXml(metaInfDir);
        
        WriteCss(direction, oebpsDir, manifest);
        
        var coverItem = WriteCoverImage(coverImage, oebpsDir, manifest, spine);
        
        WriteChapters(chapters, book.Language, direction, oebpsDir, manifest, spine, navItems);

        WriteNavigation(chapters, oebpsDir, manifest, spine);

        WriteLegacyNavigation(book, chapters, oebpsDir, manifest, spine);

        WriteContentOpf(book, coverItem, manifest, direction, spine, oebpsDir);

        var epubBytes = CreateEPubFile(outputFilePath, epubFolder);

        Directory.Delete(epubFolder, true);

        return epubBytes;
    }

    private static void WriteMimeType(string epubFolder)
    {
      File.WriteAllText(Path.Combine(epubFolder, "mimetype"), "application/epub+zip", new UTF8Encoding(false));
    }

    private static void WriteContainerXml(string metaInfDir)
    {
      File.WriteAllText(Path.Combine(metaInfDir, "container.xml"), 
        @"<?xml version='1.0' encoding='utf-8'?>
          <container version='1.0' xmlns='urn:oasis:names:tc:opendocument:xmlns:container'>
            <rootfiles>
              <rootfile full-path='OEBPS/content.opf' media-type='application/oebps-package+xml'/>
            </rootfiles>
          </container>");
    }
    
    private static void WriteCss(string direction, string oebpsDir, StringBuilder manifest)
    {
      var rtlBullets = direction == "rtl"
        ? @"ul, ol { 
              direction: ltr;
              unicode-bidi: embed;
              text-align: left;
              margin-left: 1.5em;
            }
            ul li, ol li {
              direction: rtl;
              text-align: right;
            }"
        : "";
      // Step 4: styles.css
      File.WriteAllText(Path.Combine(oebpsDir, "styles.css"), $@"
body {{
  direction: {direction};
  unicode-bidi: embed;
  font-family: serif;
  line-height: 1.6;
  text-align: right;
}}
{rtlBullets}
");

      manifest.AppendLine("    <item id='css' href='styles.css' media-type='text/css'/>");
    }
    
    private static string WriteCoverImage(byte[] coverImage, string oebpsDir, StringBuilder manifest,
      StringBuilder spine)
    {
      string coverItem = "";
      if (coverImage != null)
      {
        var mimeType = coverImage.GetImageMimeType();
        string fileName = $"cover.{mimeType.GetFileExtensionFromMimeType()}";
        File.WriteAllBytes(Path.Combine(oebpsDir, fileName), coverImage);
        manifest.AppendLine($"    <item id='cover' href='{fileName}' media-type='{mimeType}' properties='cover-image'/>");
        coverItem = "<meta name='cover' content='cover'/>";

        string titlePageContent = $@"<?xml version='1.0' encoding='utf-8'?>
          <html xmlns='http://www.w3.org/1999/xhtml' xmlns:epub='http://www.idpf.org/2007/ops' lang='ur' xml:lang='ur' dir='rtl'>
            <head>
              <title>Cover</title>
              <style type='text/css' title='override_css'>
                      @page {{padding: 0pt; margin:0pt}}
                      body {{ text-align: center; padding:0pt; margin: 0pt; }}
                  </style>
              <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>
            </head>
            <body>
                  <div>
                      <svg xmlns='http://www.w3.org/2000/svg' xmlns:xlink='http://www.w3.org/1999/xlink' version='1.1' width='100%' height='100%' viewBox='0 0 1118 1588' preserveAspectRatio='none'>
                          <image width='1118' height='1588' xlink:href='{fileName}'/>
                      </svg>
                  </div>
              </body>
          </html>";   
        File.WriteAllText(Path.Combine(oebpsDir, "titlepage.xhtml"), titlePageContent, Encoding.UTF8);
        
        manifest.AppendLine($"    <item id='titlepage' href='titlepage.xhtml' media-type='application/xhtml+xml'/>");
        
        spine.AppendLine("    <itemref idref=\"titlepage\" />\n");
      }

      
      return coverItem;
    }
    
    private static void WriteChapters(List<Chapter> chapters, string language, string direction, string oebpsDir,
      StringBuilder manifest, StringBuilder spine, StringBuilder navItems)
    {
      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      for (int i = 0; i < chapters.Count; i++)
      {
        var chapter = chapters[i];
        string html = Markdown.ToHtml(chapter.MarkdownContent, pipeline);

        string xhtml = $@"<?xml version='1.0' encoding='utf-8'?>
            <!DOCTYPE html>
            <html xmlns='http://www.w3.org/1999/xhtml' xml:lang='{language}' dir='{direction}'>
              <head>
                <title>{chapter.Title}</title>
                <link rel='stylesheet' type='text/css' href='styles.css'/>
              </head>
              <body>
                {html}
              </body>
            </html>";

        string fileName = $"chapter{i + 1}.xhtml";
        File.WriteAllText(Path.Combine(oebpsDir, fileName), xhtml, Encoding.UTF8);

        manifest.AppendLine($"    <item id='chap{i + 1}' href='{fileName}' media-type='application/xhtml+xml'/>");
        spine.AppendLine($"    <itemref idref='chap{i + 1}'/>");
        navItems.AppendLine($"      <li><a href='{fileName}'>{chapter.Title}</a></li>");
      }
    }

    private static void WriteNavigation(List<Chapter> chapters, string oebpsDir, StringBuilder manifest,
      StringBuilder spine)
    {
      StringBuilder toc = new StringBuilder();
      toc.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
          <!DOCTYPE html>
          <html xmlns=""http://www.w3.org/1999/xhtml"" 
                xmlns:epub=""http://www.idpf.org/2007/ops""
                xml:lang=""ur"" dir=""rtl"">
          <head><title>TOC</title></head>
          <body>
          <nav epub:type=""toc"" id=""toc"">
          <h1>فہرست مضامین</h1>
          <ol>");

      for (int i = 0; i < chapters.Count; i++)
      {
        toc.AppendLine($"<li><a href=\"chapter{i + 1}.xhtml\">{chapters[i].Title}</a></li>");
      }

      toc.AppendLine("</ol></nav></body></html>");
      File.WriteAllText(Path.Combine(oebpsDir, "nav.xhtml"), toc.ToString(), Encoding.UTF8);

      manifest.AppendLine("    <item id='nav' href='nav.xhtml' media-type='application/xhtml+xml' properties='nav' />");
      spine.AppendLine("    <itemref idref=\"nav\" />\n");
    }
    
    private static void WriteLegacyNavigation(BookModel book, List<Chapter> chapters, string oebpsDir, StringBuilder manifest,
      StringBuilder spine)
    {
      StringBuilder toc = new StringBuilder();
      toc.AppendLine(@$"<ncx xmlns=""http://www.daisy.org/z3986/2005/ncx/"" version=""2005-1"" xml:lang=""{book.Language}"" >
             <head>
              <meta content=""50414388-9c89-4506-b7da-a33e4c46c07b"" name=""dtb:uid""/>
              <meta content=""2"" name=""dtb:depth""/>
              <meta content=""Nawishta"" name=""dtb:generator""/>
            </head>
            <docTitle>
              <text>{book.Title}</text>
            </docTitle>
            <navMap>");

      for (int i = 0; i < chapters.Count; i++)
      {
        toc.AppendLine(@$"<navPoint id=""{StringExtentions.GenerateEpubUniqueId()}"" playOrder=""{i+1}"">
                <navLabel>
                  <text>{chapters[i].Title}</text>
                </navLabel>
                <content src=""chapter{i + 1}.xhtml""/>
            </navPoint>");
      }

      toc.AppendLine("</navMap>\n</ncx>");
      File.WriteAllText(Path.Combine(oebpsDir, "toc.ncx"), toc.ToString(), Encoding.UTF8);

      manifest.AppendLine("    <item href=\"toc.ncx\" id=\"ncx\" media-type=\"application/x-dtbncx+xml\"/>");
    }
    
    private static void WriteContentOpf(BookModel book, string coverItem,
      StringBuilder manifest, string direction, StringBuilder spine, string oebpsDir)
    {
      var authors = book.Authors.Select(x => x.Name).ToArray();
      string authorsXml = string.Join("\n", authors.Select(a => $"<dc:creator>{a}</dc:creator>"));
      string categoriesXml = book.Categories != null
        ? string.Join("\n", book.Categories.Select(c => $"<dc:subject>{c.Name}</dc:subject>"))
        : "";
      string publisherXml = !string.IsNullOrEmpty(book.Publisher)
        ? $"<dc:publisher>{book.Publisher}</dc:publisher>"
        : "";
      string seriesMeta = !string.IsNullOrEmpty(book.SeriesName)
        ? $"<meta property='belongs-to-collection'>{book.SeriesName}</meta>"
        : "";
      
      string seriesIndexMeta = book.SeriesIndex.HasValue
        ? $"<meta property='group-position'>{book.SeriesIndex.Value}</meta>"
        : "";
      string generatorMeta = "<meta name='generator' content='Nawishta'/>";
      
      string rightsXml = book.Copyrights != CopyrightStatuses.Open
        ? $"<dc:rights>{System.Security.SecurityElement.Escape(book.Copyrights.ToString())}</dc:rights>"
        : "";
      
      var uniqueIdentifier = StringExtentions.GenerateEpubUniqueId();
      string opfContent = $@"<?xml version='1.0' encoding='utf-8'?>
<package version='3.0' xmlns='http://www.idpf.org/2007/opf' unique-identifier='{uniqueIdentifier}' xml:lang='{book.Language}'>
  <metadata xmlns:dc='http://purl.org/dc/elements/1.1/'>
    <dc:identifier id='bookid'>urn:uuid:{Guid.NewGuid()}</dc:identifier>
    <dc:identifier xmlns:opf=""http://www.idpf.org/2007/opf"" id=""{uniqueIdentifier}"" opf:scheme=""uuid"">{uniqueIdentifier}</dc:identifier>
    <dc:title>{book.Title}</dc:title>
    <dc:language>{book.Language}</dc:language>
    {authorsXml}
    {categoriesXml}
    {publisherXml}
    {rightsXml}
    {seriesMeta}
    {seriesIndexMeta}
    {generatorMeta}
    {coverItem}
  </metadata>
  <manifest>
{manifest.ToString().TrimEnd()}
  </manifest>
  <spine toc=""ncx"" page-progression-direction='{direction}'>
{spine.ToString().TrimEnd()}
  </spine>
</package>";

      File.WriteAllText(Path.Combine(oebpsDir, "content.opf"), opfContent, Encoding.UTF8);
    }
    
    private static byte[] CreateEPubFile(string outputFilePath, string epubFolder)
    {
      if (File.Exists(outputFilePath))
        File.Delete(outputFilePath);

      ZipFile.CreateFromDirectory(epubFolder, outputFilePath, CompressionLevel.Optimal, false);

      byte []  epubBytes = File.ReadAllBytes(outputFilePath);
      return epubBytes;
    }
}


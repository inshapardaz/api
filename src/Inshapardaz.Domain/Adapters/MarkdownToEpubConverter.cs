using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Markdig;
using SkiaSharp;

namespace Inshapardaz.Domain.Adapters;

public class MarkdownToEpubConverter
{
    public record Chapter(string Title, string MarkdownContent);
    
    public byte[] CreateEpub(
        string outputFilePath,
        List<Chapter> chapters,
        string title,
        string[] authors,
        string language = "ur",
        byte[] coverImage = null)
    {
        var direction = language == "ur" ? "rtl" : "ltr";
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
        
        WriteChapters(chapters, language, direction, oebpsDir, manifest, spine, navItems);

        WriteNavigation(chapters, oebpsDir, manifest, spine);

        WriteContentOpf(title, authors, language, coverItem, manifest, direction, spine, oebpsDir);

        var epubBytes = CreateEPubFile(outputFilePath, epubFolder);

        Directory.Delete(epubFolder, true);

        return epubBytes;
    }

    private static byte[] CreateEPubFile(string outputFilePath, string epubFolder)
    {
      if (File.Exists(outputFilePath))
        File.Delete(outputFilePath);

      ZipFile.CreateFromDirectory(epubFolder, outputFilePath, CompressionLevel.Optimal, false);

      byte []  epubBytes = File.ReadAllBytes(outputFilePath);
      return epubBytes;
    }

    private static void WriteContentOpf(string title, string[] authors, string language, string coverItem,
      StringBuilder manifest, string direction, StringBuilder spine, string oebpsDir)
    {
      string authorsXml = string.Join("\n", authors.Select(a => $"<dc:creator>{a}</dc:creator>"));
      string opfContent = $@"<?xml version='1.0' encoding='utf-8'?>
<package version='3.0' xmlns='http://www.idpf.org/2007/opf' unique-identifier='bookid' xml:lang='{language}'>
  <metadata xmlns:dc='http://purl.org/dc/elements/1.1/'>
    <dc:identifier id='bookid'>urn:uuid:{Guid.NewGuid()}</dc:identifier>
    <dc:title>{title}</dc:title>
    <dc:language>{language}</dc:language>
    {authorsXml}
    {coverItem}
  </metadata>
  <manifest>
{manifest.ToString().TrimEnd()}
  </manifest>
  <spine page-progression-direction='{direction}'>
{spine.ToString().TrimEnd()}
  </spine>
</package>";

      File.WriteAllText(Path.Combine(oebpsDir, "content.opf"), opfContent, Encoding.UTF8);
    }

    private static string WriteCoverImage(byte[] coverImage, string oebpsDir, StringBuilder manifest,
      StringBuilder spine)
    {
      string coverItem = "";
      if (coverImage != null)
      {
        var mimeType = GetImageMimeType(coverImage);
        string fileName = $"cover.{GetFileExtensionFromMimeType(mimeType)}";
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
      spine.AppendLine("    <itemref idref=\"nav\" media-type=\"application/xhtml+xml\" properties=\"nav\"/>\n");
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
  
    public static string GetImageMimeType(byte[] imageBytes)
    {
      using var codec = SKCodec.Create(new SKMemoryStream(imageBytes));
      if (codec == null) return "application/octet-stream";
      return codec.EncodedFormat switch
      {
        SKEncodedImageFormat.Jpeg => "image/jpeg",
        SKEncodedImageFormat.Png => "image/png",
        SKEncodedImageFormat.Gif => "image/gif",
        SKEncodedImageFormat.Bmp => "image/bmp",
        SKEncodedImageFormat.Webp => "image/webp",
        SKEncodedImageFormat.Wbmp => "image/vnd.wap.wbmp",
        SKEncodedImageFormat.Pkm => "image/x-pkm",
        SKEncodedImageFormat.Ktx => "image/ktx",
        SKEncodedImageFormat.Heif => "image/heif",
        SKEncodedImageFormat.Ico => "image/x-icon",
        SKEncodedImageFormat.Avif => "image/avif",
        _ => "application/octet-stream"
      };
    }
    
    private static string GetFileExtensionFromMimeType(string mimeType)
    {
      return mimeType switch
      {
        "image/jpeg" => "jpg",
        "image/png" => "png",
        "image/gif" => "gif",
        "image/bmp" => "bmp",
        "image/webp" => "webp",
        "image/vnd.wap.wbmp" => "wbmp",
        "image/x-pkm" => "pkm",
        "image/ktx" => "ktx",
        "image/heif" => "heif",
        "image/x-icon" => "ico",
        "image/avif" => "avif",
        _ => throw new NotSupportedException($"Mime type '{mimeType}' is not supported.")
      };
    }
}


using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Markdig;

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
        string epubFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(epubFolder);

        string oebpsDir = Path.Combine(epubFolder, "OEBPS");
        string metaInfDir = Path.Combine(epubFolder, "META-INF");
        Directory.CreateDirectory(oebpsDir);
        Directory.CreateDirectory(metaInfDir);

        // Step 1: Write mimetype
        File.WriteAllText(Path.Combine(epubFolder, "mimetype"), "application/epub+zip", new UTF8Encoding(false));

        // Step 2: container.xml
        File.WriteAllText(Path.Combine(metaInfDir, "container.xml"), @"<?xml version='1.0' encoding='utf-8'?>
<container version='1.0' xmlns='urn:oasis:names:tc:opendocument:xmlns:container'>
  <rootfiles>
    <rootfile full-path='OEBPS/content.opf' media-type='application/oebps-package+xml'/>
  </rootfiles>
</container>");

        var manifest = new StringBuilder();
        var spine = new StringBuilder();
        var navItems = new StringBuilder();

        // Step 3: Generate XHTML from Markdown chapters
        var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        for (int i = 0; i < chapters.Count; i++)
        {
            var chapter = chapters[i];
            string html = Markdown.ToHtml(chapter.MarkdownContent, pipeline);

            string xhtml = $@"<?xml version='1.0' encoding='utf-8'?>
<!DOCTYPE html>
<html xmlns='http://www.w3.org/1999/xhtml' xml:lang='{language}' dir='rtl'>
  <head>
    <title>{chapter.Title}</title>
    <link rel='stylesheet' type='text/css' href='styles.css'/>
  </head>
  <body>
    <h1>{chapter.Title}</h1>
    {html}
  </body>
</html>";

            string fileName = $"chapter{i + 1}.xhtml";
            File.WriteAllText(Path.Combine(oebpsDir, fileName), xhtml, Encoding.UTF8);

            manifest.AppendLine($"    <item id='chap{i + 1}' href='{fileName}' media-type='application/xhtml+xml'/>");
            spine.AppendLine($"    <itemref idref='chap{i + 1}'/>");
            navItems.AppendLine($"      <li><a href='{fileName}'>{chapter.Title}</a></li>");
        }

        // Step 4: styles.css
        File.WriteAllText(Path.Combine(oebpsDir, "styles.css"), @"
body {
  direction: rtl;
  unicode-bidi: embed;
  font-family: serif;
  line-height: 1.6;
  text-align: right;
}");

        manifest.AppendLine("    <item id='css' href='styles.css' media-type='text/css'/>");

        // Step 5: Optional Cover
        string coverItem = "";
        if (coverImage != null)
        {
            string fileName = "cover.jpg";
            File.WriteAllBytes(Path.Combine(oebpsDir, fileName), coverImage);
            manifest.AppendLine($"    <item id='cover' href='{fileName}' media-type='image/jpeg' properties='cover-image'/>");
            coverItem = "<meta name='cover' content='cover'/>";
        }

        string authorsXml = string.Join("\n", authors.Select(a => $"<dc:creator>{a}</dc:creator>"));
        // Step 6: content.opf
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
  <spine page-progression-direction='rtl'>
{spine.ToString().TrimEnd()}
  </spine>
</package>";

        File.WriteAllText(Path.Combine(oebpsDir, "content.opf"), opfContent, Encoding.UTF8);

        // Step 7: Create final EPUB zip
        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);

        ZipFile.CreateFromDirectory(epubFolder, outputFilePath, CompressionLevel.Optimal, false);

        byte []  epubBytes = File.ReadAllBytes(outputFilePath);
        
        // Cleanup
        Directory.Delete(epubFolder, true);

        return epubBytes;
    }
}


namespace Inshapardaz.Domain.Helpers;

// Page break marker from the Likhari markdown dialect (https://github.com/inshapardaz/likhari/blob/main/docs/markdown-dialect.md),
// a leaf directive written as `::pagebreak`. Inserted inline (no surrounding blank lines) at the
// boundary between two originally-scanned pages, so it carries no visual effect of its own -
// renderers turn it into an invisible marker (an epub pagebreak anchor, a Word bookmark) used
// only to let a reader track/count progress through the original page numbering.
public static class MarkdownHelper
{
    public const string PageBreakMarker = "::pagebreak";
}

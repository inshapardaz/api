using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

// Also used as the PUT request body (UserController.UpsertUserNote) - see BookmarkView's own doc
// comment for why Id here is output-only.
public class NoteView
{
    public string Id { get; set; }

    [Required]
    public string ChapterId { get; set; }

    public int StartOffset { get; set; }

    public int EndOffset { get; set; }

    [Required]
    public string Text { get; set; }

    public string Comment { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}

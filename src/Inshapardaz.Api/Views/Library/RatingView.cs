using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

// Also used as the PUT request body (UserController.UpsertUserBookRating /
// UpsertUserIssueRating) - unlike BookmarkView/NoteView there is no client-generated Id, since a
// rating is naturally unique per account per book/issue.
public class RatingView
{
    [Range(1, 5)]
    public int Value { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}

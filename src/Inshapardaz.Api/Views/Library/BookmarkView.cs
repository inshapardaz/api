using System.ComponentModel.DataAnnotations;

namespace Inshapardaz.Api.Views.Library;

// Also used as the PUT request body (UserController.UpsertUserBookmark) - the client's own
// bookmark id travels in the route (clientId), not this body, so Id here is only ever populated on
// the way *out* (== BookmarkModel.ClientId, not the internal bigint PK - see that model's own doc
// comment) and is ignored if a caller sends one on the way in.
public class BookmarkView
{
    public string Id { get; set; }

    [Required]
    public string ChapterId { get; set; }

    public double Position { get; set; }

    [Required]
    public string Name { get; set; }

    public DateTime DateAdded { get; set; }

    public DateTime? DateUpdated { get; set; }
}

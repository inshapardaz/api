# Author

Author schema defines an author resource. This author represents a person who can produce any type of literary work.

``` javascript

{
    links : [{
        href: "...",
        rel : "self"
    },{
        href: "...",
        rel : "books"
    }, {
        href : "...",
        rel : "image"
    }, {
        href : "...",
        rel : "update",
        method : "PUT"
    }, {
        href : "...",
        rel : "delete",
        method : "DELETE"
    }, {
        href : "...",
        rel : "image-upload",
        method : "PUT"
    }],
    id : 1,
    name : "John Doe",
    bookCount : 23
}
```

### Links

| Link | Details |
| ---- | ---- |
| self | Link to author |
| books | Link to books written by author |
| Image | Link to image of this author |
| update | Link to update the author, only rendered if call made as a writer |
| delete | Link to delete the author, only rendered if call made as a writer |
| image-upload | Link to upload author image, only rendered if call made as a writer |

### Properties

| Property | Details |
| -------- | ------- |
| id | Id of the author |
| name | Name of author |
| bookCount | Number of books authored by author |

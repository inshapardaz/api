# Library

This is the root of the library. Any client trying to use the library will start here. This resource will give links to other resources that can be used by the client to access them.

## GET

`/library/{libraryId}`


### Request
```
GET /library/{libraryId} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
{
    links : [{
        href : "..."
        rel : "self"
    }, {
        href : "..."
        rel : "authors"
    }, {
    
      href: "...",
      rel: "categories"
    },
    {
      href: "...",
      rel: "series"
    },
    {
      href: "...",
      rel: "books"
    },
    {
      href: "...",
      rel: "periodicals"  
    },
    {
      href: "...",
      rel: "recents"
    },
    {
      href: "...",
      rel: "favorites"
    },
    {
      href: "...",
      rel: "create-author",
      method: "POST"
    },
    {
      href: "...",
      rel: "create-book",
      method: "POST"
    },
    {
      href: "...",
      rel: "create-series",
      method: "POST"
    },
    {
      href: "...",
      rel: "create-category",
      method: "POST"
    }

    ]
}
```

### Status codes

See [status codes](statuscodes.md).

### Links

| Link | Details |
| ---- | ---- |
| self | Link to current resource |
| [authors](authors.md) | Link to authors |
| [categories](categories.md) | Link to categories resource |
| [series](series.md) | Link to word attribute resource |
| [books](books.md) | Link to book resource |
| [periodicals](periodicals.md) | Link to periodicals resource. This link is sent only if the dictionary supports the feature |
| [recents](recents.md)| Link to recent reads by the user. Link only available if user is making the call|
| [favorites](favorites.md)| Link to favorites books by the user. Link only available if user is making the call|
| [create-book](books.md)| Link to create new books. Link only available if a writer user is making the call|
| [create-author](authors.md)| Link to create new author. Link only available if a writer user is making the call|
| [create-series](series.md)| Link to create new series. Link only available if a writer user is making the call|
| [create-category](category.md)| Link to create new category. Link only available if an admin user is making the call|

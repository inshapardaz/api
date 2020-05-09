# Authors

Author resource represents an [author](schema/author.md).

## Get Authors

`/library/{libraryId}/authors`


### Request
```
GET /library/{libraryId}/authors HTTP/1.1
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
    }, 
    {
      href: "...",
      rel: "next"
    }, 
    {
      href: "...",
      rel: "previous"
    }, 
    {
      href: "...",
      rel: "create-author",
      method: "POST"
    }],
    pageSize : 10,
    pageCount: 10,
    currentPageIndex: 1,
    totalCount: 100,
    data : [...]
}
```

### Status codes 

See [status codes](statuscodes.md).

### Properties

For all properties see the [pagination](schema/pagination.md) schema. 

### Other Links

| Link | Details |
| ---- | ---- |
| [create-author](authors.md)| Link to create new author. Link only available if a writer user is making the call|

### Data

The response contains a list of [author](schema/author.md) 


## Get Author

`/library/{libraryId}/authors/{authorId}`


### Request
```
GET /library/{libraryId}/authors/{authorId} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

For details of body please refer to [auhtor](schema/author.md) resource.

### Status codes 

See [status codes](statuscodes.md).

## Add Author

`/library/{libraryId}/authors`

### Request
```
POST /library/{libraryId}/authors HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

```
{
  name : "some name"
}
````

### Response
```
HTTP/1.1 201 CREATED
Content-Type: application/json
LOCATION: http://......
```

For details of body please refer to [author](schema/author.md) resource.

### Headers

Location header gives the uri to newly created resource.

### Status codes 

See [status codes](statuscodes.md) for other codes.

## Update Author

`/library/{libraryId}/authors/{authorId}`

### Request
```
PUT /library/{libraryId}/authors/{authorId} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

```
{
  name : "some name"
}
````

### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

### Status codes 

See [status codes](statuscodes.md) for other codes.

## Delete Author

`/library/{libraryId}/authors/{authorId}`

### Request
```
DELETE /library/{libraryId}/authors/{authorId} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Status codes 

See [status codes](statuscodes.md) for other codes.

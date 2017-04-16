# Entry

Entry is the root of the api. It provide links to resources available for user.

## GET

`/api/entry`


### Request
```
GET /api/entry HTTP/1.1
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
        rel : "dictionaries"
    }, {
    
      href: "...",
      rel: "languages"
    },
    {
      href: "...",
      rel: "attributes"
    },
    {
      href: "...",
      rel: "relationshiptypes"
    },
    {
      href: "...",
      rel: "thesaurus"
    }
    // Other links can be added later
    ]
}
```


### Links

| Link | Details |
| ---- | ---- |
| self | Link to current resource |
| [dictionaries](dictionary/dictionaries.md) | Link to resource for all dictionaries available to user |
| [languages](languages.md) | Link to language resource |
| [attributes](attributes.md) | Link to word attribute resource |
| [relationshiptypes](relationshipTypes.md) | Link to relationship types resource |
| thesaurus | Link to thesaurus resource |

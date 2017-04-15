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
        href = "..."
        rel = "self"
    }, {
        href = "..."
        rel = "dictionary"
    } 
    // Other links can be added later
    ]
}
```

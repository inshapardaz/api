Gets List of all languages used in application. This is a lookup resource and represent the language id values returned in various resources.

## Gets
`api/languages`


### Request
```
GET /api/languages HTTP/1.1
Accept: application/json
```

### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
    [
        {
            key: "None",
            value: 0
        }, {
            key: "Unknown",
            value: 1
        }, {
            key: "Urdu",
            value: 2
        },
        // Other values here
    ]
```
Gets List of all attribute types used in application. This is a lookup resource and represent the attribute values returned in various resources. This resource should be used as resource. Each bit represent a different type of attribute.

## Gets
`api/attributes`


### Request
```
GET /api/attributes HTTP/1.1
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
        },{
            key: "Male",
            value: 1
        },{
            key: "Female",
            value: 16
        },{
            key: "Singular",
            value: 256
        },{
            key: "Plural",
            value: 4096
        },{
            key: "Ism",
            value: 65536
        },{
            key: "Sift",
            value: 131072
        },
        // Other values here
    ]
```
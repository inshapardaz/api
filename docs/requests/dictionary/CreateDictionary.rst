..  index:: requests; Dictionary, CreateDictionary

Create Dictionary
=====================

### Uri 
` /api/dictionaries`

#### Request
```
POST /api/dictionaries HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    name : 'dictionary name',
    description : 'dictionary description',
    language : 34,
    isPublic : true
}
```

#### Response
```
HTTP/1.1 201 CREATED
Content-Type: application/json
```

``` javascript
{
    id : 12,
    name : 'dictionary name',
    description : 'dictionary description',
    language : 34,
    isPublic : true,
    links : [{
        href : '...',
        rel : 'self'
    },{
        href : '...'
        rel : 'update'
    },{
        href : '...',
        rel : 'delete'
    }, {
        href : '...',
        rel : 'create-word'
    }, {
        href : '...',
        rel : 'reference-types'
    }]
}
```

- If a dictioanry is created successfully, a object representing newly created dictionary is returned in response. Please review the [Dictioanry Resource](DictioanryResource) for detials on object.

- By default all dictionaries are public unless specified.

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to create dictioanry                 | `403 Forbidden`   | Unauthorised                  |
| No name specified                                     | `400 Bad Request` | Invalid name                  |
| Name not specified                                    | `400 Bad Request` | Name required                 |
| Invalid language                                      | `400 Bad Request` | Invalid Language              |
| Posted data not correct format                        | `400 Bad Request` | Invlaid data                  |

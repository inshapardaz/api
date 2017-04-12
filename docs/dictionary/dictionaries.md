## Dictionaries

## GET all dictionaries

### Uri
` /api/dictionaries`

Retrieves all dictionaries available to user.

### Request
```
GET /api/dictionaries HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

### Response

Returns the list of all dictionaries available to user. Please review [Dictionary Resource](DictoanryResource) for details on the individual items in array.

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
        rel = "create"
    }],
    items : [{
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
    }]
}
```
 
### Create new dictionary

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

### GET dictionary

### Uri

`/api/dictionaries/{dictionaryId}`

### Request
```
GET /api/dictionary/{id} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of dictionary to get   |


#### Response
```
HTTP/1.1 200 OK
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

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to get dictioanry                 | `403 Forbidden`   | Unauthorised                  |
| No matching dictionary found                          | `404 Not Found`   | Dictionary Not Found          |

### Update dictionary data

#### Uri

`/api/dictionaries/{dictionaryId}`

#### Request

```
PUT /api/dictionaries/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    id : 12,
    name : 'dictionary name',
    description : 'dictionary description',
    language : 34,
    isPublic : true
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of dictionary to update|

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
{
    id : 12,
    name : 'dictionary name',
    description : 'dictionary description',
    language : 34,
    isPublic : true
}
```

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to update dictioanry                 | `403 Forbidden`   | Unauthorised                  |
| No matching dictionary found                          | `404 Not Found`   | Dictionary Not Found          |
| Title missing                                         | `400 Bad Request` | Title required                |
| Language id not valid                                 | `400 Bad Request` | Invlaid language              |
| Posted data not correct format                        | `400 Bad Request` | Invlaid data                  |

### Delete dictionary

#### Uri
`/api/dictionaries/{id}`

#### Request

```
DELETE /api/dictionaries/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of dictionary to delete|

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to delete dictioanry                 | `403 Forbidden`   | Unauthorised                  |
| No matching dictionary found                          | `404 Not Found`   | Dictionary Not Found          |
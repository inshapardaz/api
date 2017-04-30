### Get Details For Word

Get the details resources associated with word 

#### Uri

`api/words/{id}/details`

#### Request

```
GET /api/words/{id}/details HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                     |
|----------------|-------------|----------------------------------|
| id             | number      | Id of word to get detail for     |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
[{
        id : 1,
        attributes : "some attribute",
        attributeValue : 2345,
        language : "English",
        languageValue : 3,
        links : [
            // links
        ]
    },{
        // other word details
    }
]
```

See [word detail resource](../resources/wordDetail.md) for details on items in response list

### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to get word                          | `403 Forbidden`   | Unauthorised                  |
| No matching word found                                | `404 Not Found`   | Word Not Found                |

### Get Word Detail

Get the word detail resource 

#### Uri

`api/details/{id}`

#### Request

```
GET /api/details/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                     |
|----------------|-------------|----------------------------------|
| id             | number      | Id of word detail to retrieve    |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
{
    id : 1,
    attributes : "some attribute",
    attributeValue : 2345,
    language : "English",
    languageValue : 3,
    links : [{
        href : '...',
        rel : 'self'
    },{
        href : '...',
        rel : 'word'
    },{
        href : '...',
        rel : 'meanings'
    },{
        href : '...',
        rel : 'translation'
    },{
        href : '...',
        rel : 'update'
    },{
        href : '...',
        rel : 'delete'
    },{
        href : '...',
        rel : 'add-meaning'
    },{
        href : '...',
        rel : 'add-translation'
    }]
}
```

See [word detail resource](../resources/wordDetail.md) for details on response
### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to get word                          | `403 Forbidden`   | Unauthorised                  |
| No matching word detail found                         | `404 Not Found`   | Word Detail Not Found         |

### Add Detail to word

Add another detail resource to word

#### Uri

`api/words/{id}/details`

#### Request

```
POST /api/words/{id}/details HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    attributes : "some attribute",
    attributeValue : 2345,
    language : "English",
    languageValue : 3,
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                      |
|----------------|-------------|-----------------------------------|
| id             | number      | Id of word to add details         |

#### Response
```
HTTP/1.1 201 CREATED
Content-Type: application/json
Location : http://....
```

``` javascript
{
    id : 1,
    attributes : "some attribute",
    attributeValue : 2345,
    language : "English",
    languageValue : 3,
    links : [
        // links here
    ]
}
```


### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to update resource                   | `403 Forbidden`   | Unauthorised                  |
| Bad Request                            | `403 Bad Request`   | Data not found            |

### Update Word

Update metadata for [word detail resource](../resources/wordDetail.md)

#### Uri

`/api/details/{id}`

#### Request

```
PUT /api/details/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    attributes : "some attribute",
    attributeValue : 2345,
    language : "English",
    languageValue : 3,
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                      |
|----------------|-------------|-----------------------------------|
| id             | number      | Id of word details to update      |

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to update resource                   | `403 Forbidden`   | Unauthorised                  |
| No matching resource found                            | `404 Not Found`   | Resource Not Found            |


### Delete Word Detail

Removes the [word detail resource](../resources/wordDetail.md). All data associated with the word detail is deleted. Data is removed permanently and cannot be undone.

#### Uri
`/api/details/{id}`

#### Request

```
DELETE /api/details/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of resource to delete  |

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to delete resource                   | `403 Forbidden`   | Unauthorised                  |
| No matching resource found                            | `404 Not Found`   | Resource Not Found            |
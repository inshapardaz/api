### Get Word

Get the word resource 

#### Uri

`api/words/{id}`

#### Request

```
GET /api/words/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of word to retrieve    |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
{
    id : 1,
    title : "some title",
    titleWithMovements : "some title with movement",
    pronunciation : "...",
    description : "...",
    links : [{
        href : '...',
        rel : 'self'
    },{
        href : '...',
        rel : 'details'
    },{
        href : '...',
        rel : 'relations'
    },{
        href : '...',
        rel : 'update'
    },{
        href : '...',
        rel : 'delete'
    },{
        href : '...',
        rel : 'add-detail'
    },{
        href : '...',
        rel : 'add-relation'
    }]
}
```

See [word resource](../resources/word.md) for details on response
### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to get word                          | `403 Forbidden`   | Unauthorised                  |
| No matching word found                                | `404 Not Found`   | Word Not Found                |

### Add word to dictionary

Add another word resource to dictionary

#### Uri

`api/dictionaries/{id}/words`

#### Request

```
POST /api/dictionaries/{id}/words HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    title : "some title",
    titleWithMovements : "some title with movement",
    pronunciation : "...",
    description : "...",
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                      |
|----------------|-------------|-----------------------------------|
| id             | number      | Id of dictionary to add word      |

#### Response
```
HTTP/1.1 201 CREATED
Content-Type: application/json
Location : http://....
```

``` javascript
{
    id : 1,
    title : "some title",
    titleWithMovements : "some title with movement",
    pronunciation : "...",
    description : "...",
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

Update metadata for [word resource](../resources/word.md)

#### Uri

`/api/words/{id}`

#### Request

```
PUT /api/words/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    id : 12,
    title : "some title",
    titleWithMovements : "some title with movement",
    pronunciation : "...",
    description : "..."
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of word to update      |

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to update word                       | `403 Forbidden`   | Unauthorised                  |
| No matching word found                                | `404 Not Found`   | Word Not Found                |
| Title missing                                         | `400 Bad Request` | Title required                |


### Delete Word

Removes the [word resource](../resources/word.md). All data associated with the word is deleted. Data is removed permanently and cannot be undone.

#### Uri
`/api/words/{id}`

#### Request

```
DELETE /api/words/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description              |
|----------------|-------------|---------------------------|
| id             | number      | Id of word to delete|

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses ###

| **Case**                                              | **Response Code** |      **Error Code**           |
|-------------------------------------------------------|-------------------|-------------------------------|
| User not allowed to delete word                       | `403 Forbidden`   | Unauthorised                  |
| No matching word found                                | `404 Not Found`   | Word Not Found                |
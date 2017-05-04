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

Returns the list of all dictionaries available to user. Please review [Dictionary Resource](../resources/dictionary.md) for details on the individual items in array.

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
        rel : "create"
    }],
    items : [
        // Dictionary resources
    ]
    }]
}
```
 
### GET dictionary

Returns metadata of a [dictionary resource](../resources/dictionary.md)

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

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to get dictionary                    | `403 Forbidden`   |
| No matching dictionary found                          | `404 Not Found`   |

### Create new dictionary

Creates a new [dictionary resource](../resources/dictionary.md)

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
Location : http://....
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

- If a dictionary is created successfully, a object representing newly created dictionary is returned in response. Please review the [Dictionary Resource](../resources/dictionary.md) for details on object.

- By default all dictionaries are public unless specified.

### Error Responses ###

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to create dictionary                 | `403 Forbidden`   |
| No name specified                                     | `400 Bad Request` |
| Name not specified                                    | `400 Bad Request` |
| Invalid language                                      | `400 Bad Request` |
| Posted data not correct format                        | `400 Bad Request` |


### Update dictionary data

Update metadata for [dictionary resource](../resources/dictionary.md)

#### Uri

`/api/dictionaries/{id}`

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
HTTP/1.1 204 NoContent
Content-Type: application/json
```

#### Response

When the dictionary mentioned does not exist

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
    isPublic : true
}
```

### Error Responses

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to update dictionary                 | `403 Forbidden`   |
| No matching dictionary found                          | `404 Not Found`   |
| Title missing                                         | `400 Bad Request` |
| Language id not valid                                 | `400 Bad Request` |
| Posted data not correct format                        | `400 Bad Request` |

### Delete dictionary

Removes the [dictionary resource](../resources/dictionary.md). All data associated with the dictionary is deleted. Data is removed permanently and cannot be undone.

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

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to delete dictionary                 | `403 Forbidden`   |
| No matching dictionary found                          | `404 Not Found`   |
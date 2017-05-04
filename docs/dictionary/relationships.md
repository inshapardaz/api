## Relationships

Relationships represents how two different word resources are linked to each other

### Get Relationships for a word

### Uri
`/api/words/{id}/relationships`

### Request
```
GET /api/words/{id}/relationships HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                            |
|----------------|-------------|-----------------------------------------|
| id             | number      | Id of word whose relationships to get   |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

```javascript
[{
    id : 7,
    relatedWordId : 11,
    relatedWord :  "title" ,
    relationType :  "sometype" ,
    relationTypeId : 0,
    links : [{
        href :  "...",
        rel : "self"
    },{
        href : "...",
        rel : "source-word"
    },{
        href : "...",
        rel : "related-word"
    }]},
    // other relationship resources
]
```

See [relationship resource](../resources/relationship.md) for details on response
### Error Responses

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to get resource                      | `403 Forbidden`   |
| No matching word found                                | `404 Not Found`   |


### Get Relationships by id

### Uri
`/api/relationships/{id}`

### Request
```
GET /api/relationships/{id} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                |
|----------------|-------------|-----------------------------|
| id             | number      | Id of relationship to get   |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

```javascript
{
     id : 7,
     relatedWordId : 11,
     relatedWord :  "title" ,
     relationType :  "sometype" ,
     relationTypeId : 0,
     links : [
        {
         href :  "...",
         rel : "self"
        },{
         href : "...",
         rel : "source-word"
        },{
         href : "...",
         rel : "related-word"
        }
    ]
}
```

See [relationship resource](../resources/relationship.md) for details on response
### Error Responses

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to get resource                      | `403 Forbidden`   |
| No matching word found                                | `404 Not Found`   |


### Create new relationship

Creates a new [relationship](../resources/relationship.md) between words

### Uri 
`/api/word/{id}/Relation`

#### Request
```
POST /api/word/{id}/Relation HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                  |
|----------------|-------------|-------------------------------|
| id             | number      | Id of word to add relation to |

``` javascript
{
    relatedWordId : 11,
    relatedWord : "title",
    relationType : "sometype",
    relationTypeId : 0,
}
```

#### Response
```
HTTP/1.1 201 CREATED
Content-Type: application/json
Location : http://....
```

```javascript
{
     id : 7,
     relatedWordId : 11,
     relatedWord :  "title" ,
     relationType :  "sometype" ,
     relationTypeId : 0,
     links : [
        {
         href :  "...",
         rel : "self"
        },{
         href : "...",
         rel : "source-word"
        },{
         href : "...",
         rel : "related-word"
        }
    ]
}
```

- If a relationship is created successfully, a object representing newly created resource is returned in response. Please review the [relationship Resource](../resources/relationship.md) for details on object.

- By default all dictionaries are public unless specified.

### Error Responses ###

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to create relationship               | `403 Forbidden`   |
| source word not found                                 | `404 Not Found`   |
| related word not found                                | `400 Bad Request` |

### Update relationship 

Update metadata for [relationship resource](../resources/relationship.md)

#### Uri

`/api/relationship/{id}`

#### Request

```
PUT /api/relationship/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

``` javascript
{
    relatedWordId : 11,
    relatedWord : "title",
    relationType : "sometype",
    relationTypeId : 0,
}
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                |
|----------------|-------------|-----------------------------|
| id             | number      | Id of relationship to update|

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to update resource                   | `403 Forbidden`   |
| No matching relationship found                        | `404 Not Found`   |
| No matching relationship found                        | `400 Bad Request` |
| Related word not part of same dictionary              | `400 Bad Request` |
| Posted data not correct format                        | `400 Bad Request` |

### Delete relationship

Removes the [relationship resource](../resources/relationship.md. Source and destination words will not be deleted when deleting relationship. Data is removed permanently and cannot be undone.

#### Uri
`/api/relationship/{id}`

#### Request

```
DELETE /api/relationship/{id} HTTP/1.1
Content-Type: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                |
|----------------|-------------|-----------------------------|
| id             | number      | Id of relationship to delete|

#### Response
```
HTTP/1.1 204 NoContent
Content-Type: application/json
```

### Error Responses ###

| **Case**                                              | **Response Code** |
|-------------------------------------------------------|-------------------|
| User not allowed to delete resource                   | `403 Forbidden`   |
| No matching relationship found                        | `404 Not Found`   |
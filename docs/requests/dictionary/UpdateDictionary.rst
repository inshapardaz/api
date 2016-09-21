..  index:: requests; Dictionary, UpdateDictionary

Update Dictionary
=====================

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
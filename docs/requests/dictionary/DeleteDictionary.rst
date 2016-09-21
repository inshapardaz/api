..  index:: requests; Dictionary, DeleteDictionary

Delete Dictionary
=================

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
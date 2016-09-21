..  index:: requests; Dictionary, GetDictionary


..  |operation| replace:: GetDictionary

GET dictionary
=================

..  default-domain:: http

..  get:: /api/dictionaries/(dictionaryId)

Request
--------

GET /api/dictionary/{id} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

Request Parameters

+---------------+--------------+---------------------------+
| Parameter Name |  Data Type  |  Description              |
+================+=============+===========================+
| id             | number      | Id of dictionary to get   |
+---------------+--------------+---------------------------+

Response
---------


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


    :code 200: Success
    :code 403: Unauthorised
    :code 404: Dictionary not found
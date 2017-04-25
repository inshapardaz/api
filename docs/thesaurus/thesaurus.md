# Thesaurus

## GET 

Allows the consumer to retrueve list of word that can be used instead of the source word, for same meaning or opposite.

### Uri
` /api/thesaurus/{word}`

Retrieves all dictionaries available to user.

### Request
```
GET /api//thesaurus/{word} HTTP/1.1
Accept: application/json
Authorization: OAuth2 ...
```

##### Request Parameters

| Parameter Name |  Data Type  |  Description                            |
|----------------|-------------|-----------------------------------------|
| word           | string      | title of word to get replacements for   |

#### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
{
    title : 'word',
    links : [{
        href : '...',
        rel : 'self'
    },{
        href : '...'
        rel : 'add'
    }],
    synonyms : [{
        title : 'alternate 1',
        id : 23,
        context : ''
    }],
    acronyms : [{
        title : 'alternate 1',
        id : 23,
        context : ''
    }]
}
```
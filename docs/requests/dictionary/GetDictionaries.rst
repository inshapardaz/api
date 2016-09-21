..  index:: requests; Dictionary, GetDictionaries

Get Dictionaries
============

..  default-domain:: http

..  get:: /api/dictionaries

Retrieves all dictionaries available to user.

Request
-------

..  GET /api/dictionaries HTTP/1.1
    Accept: application/json
    Authorization: OAuth2 ...

Response
---------

Returns the list of all dictionaries available to user. Please review [Dictionary Resource](DictoanryResource) for details on the individual items in array.

..  HTTP/1.1 200 OK
    Content-Type: application/json

.. code-block:: javascript
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


Response Codes 
----------------

    :code 200: Success

# Chapter 

Represents a chapter resource in book. Chapter is an independent entity that can exist inside a book but can be references outside.

``` json
{
    "id" : 1,
    "chapterNumber" : 1,
    "title" : "abc",
    "language" : "ur-pk",
    "bookId" : 1,
    "links" : [
        { "relType" : "self", "href" : "...", "method" : "GET" },
        { "relType" : "book", "href" : "...", "method" : "GET" },
        { "relType" : "update", "href" : "...", "method" : "PUT" },
        { "relType" : "delete", "href" : "...", "method" : "DELETE" },
        { "relType" : "add-contents", "href" : "...", "method" : "POST" },
        { "relType" : "content", "href" : "...", "method" : "GET", "accept-language": "ur", "accept": "text/plain" },
        { "relType" : "content", "href" : "...", "method" : "GET", "accept-language": "hi", "accept": "text/plain" }
    ],
}
```

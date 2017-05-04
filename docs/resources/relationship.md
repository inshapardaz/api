## Relationship

``` javascript
{
    id : 7,
    sourceWordId : 22,
    sourceWord : "title1",
    relatedWordId : 11,
    relatedWord : "title",
    relationType : "sometype",
    relationTypeId : 0,
    links: [{
        href : "...",
        rel : "self"
    },{
        href : "...",
        rel : "source-word"
    },{
        href : "...",
        rel : "related-word"
    },{
        href : "...",
        rel : "update"
    },{
        href : "...",
        rel : "delete"
    }]
}
```

### Fields

#### id

Identifiers for the relationship

#### source word id

Identifier of the word the relation is associated with

#### source word

Value of word the relation is associated with

### related word id

Identifier of the source word relate to

#### related word

Value of the source word relate to

#### relationship type id

Type of relationship. See [relationship types](./relationTypes.md) for details.

#### relationship type id

Description of relationship type . See [relationship types](./relationTypes.md) for details.

### links

#### self

Link to relationship itself

#### source-word

Link to the source word

#### related-word

Link to the related word

#### update

Link to update this relationship

#### delete

Link to delete this relationship
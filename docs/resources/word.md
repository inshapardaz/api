Represents a word resource


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

### Fields

#### id
 
Unique identifier for the word.

#### title

Word contents

#### titleWithMovements

Word contents with movements

#### pronunciation

Represents how word is pronounced

#### description

Short description of word


### Links

#### self

Link to dictionary

#### details

Link to get word details. See [Word Details](../dictionary/wordDetail.md) resource for further details. 

#### relations

Link to get words relationships. See [Relations](../dictionary/relationships.md) for further details. 

#### update

Link to update the word resource. Link only returned if user making call have permissions to edit container dictionary.

#### delete

Link to delete the word. Link only returned if user making call have permissions to edit container dictionary.

#### add-detail

Link to add a new details for word. Link only returned if user making call have permissions to edit container dictionary data.

#### add-relation

Link to add a new relation for word. Link only returned if user making call have permissions to edit container dictionary data.
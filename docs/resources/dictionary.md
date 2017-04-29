Represents a dictionary resource.


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
        rel : 'index'
    },{
        href : '...'
        rel : 'update'
    },{
        href : '...',
        rel : 'delete'
    }, {
        href : '...',
        rel : 'create-word'
    }
}
```

### Fields

#### id
 
Unique identifier for the dictionary.

#### name

Name of the dictionary

#### description

Details about this dictionary

#### language

Primary language of dictionary. For details of language types see [Languages Resource](../Languages.md)

#### isPublic

Boolean flag representing if this dictionary is public or private. Private  dictionary data is available to owner only. Public dictionary data is available to everyone (authenticated and un-authenticated calls) but only owner can update the data in dictionary.


### Links

#### self

Link to dictionary

#### index

Link to word index for this dictionary

#### update

Link to update the dictionary resource. Link only returned if user making call have permissions to edit dictionary resource.

#### delete

Link to delete the dictionary. Link only returned if user making call have permissions to edit dictionary resource.

#### create-word

Link to create a new word in dictionary. Link only returned if user making call have permissions to edit dictionary data. Details on creating word can be found [here](../dictionary/words.md)
## Structure of Word Detail

``` javascript
{
    id : 1,
    attributes : "some attribute",
    attributeValue : 2345,
    language : "English",
    languageValue : 3,
    links : [{
        href : '...',
        rel : 'self'
    },{
        href : '...',
        rel : 'word'
    },{
        href : '...',
        rel : 'meanings'
    },{
        href : '...',
        rel : 'translation'
    },{
        href : '...',
        rel : 'update'
    },{
        href : '...',
        rel : 'delete'
    },{
        href : '...',
        rel : 'add-meaning'
    },{
        href : '...',
        rel : 'add-translation'
    }]
}
```

## Fields

### id

identifier for the detail resource.

### attributes

string representation of the attribute. This is a comma separated list of attribute. For example if a word is used for singular male nouns it attribute will be "Singular, Male, Noun". The value can be requested to be in english or urdu `(This part is not implemented yet)`.

### attributeValue

numeric representation of attribute for this detail. See [attributes](./attributes.md) for details.

### language 

string representing the source language of word detail.

### languageValue

numeric representation of source language of word. See [languages](./languages.md) for details.

### Links

#### self

link to the word detail resource itself.

#### word

link to parent word for this word detail

#### meaning

link to retrieve linked meaning resources. See [meaning resource](../dictionary/meaning.md) for details.

#### translation

link to retrieve linked translations resources. See [languages](../languages.md) for details.

#### update

link to update this word detail resource.

#### delete 

link to delete this word detail resource. This operation is irreversible and cannot be undone.

#### add-meaning

link to add meaning to this word detail resource.

#### add-translation

link to add translation resources to this word detail resource
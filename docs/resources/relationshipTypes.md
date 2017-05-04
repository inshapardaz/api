Gets List of all relationship types used in application. This is a lookup resource and represent the relationship id values returned in various resources.

## Gets
`api/relationtypes`


### Request
```
GET /api/relationtypes HTTP/1.1
Accept: application/json
```

### Response
```
HTTP/1.1 200 OK
Content-Type: application/json
```

``` javascript
    [
        {
            key: "Synonym",
            value: 0
        },{
            key: "Acronym",
            value: 1
        },{
            key: "Compund",
            value: 2
        },
        // Other values here
    ]
```

## Relationship Types

| Relationship   |
|----------------|
| Synonym        |
| Acronym        |
| Compound       |
| Variation      |
| Singular       |
| Plural         | 
| JamaGhairNadai |
| WahidGhairNadai|
| JamaIstasnai   |
| OppositeGender |
| FormOfFeal     |
| Halat          |
| HalatMafooli   |
| HalatIzafi     |
| HalatTafseeli  |
| JamaNadai      |
| HalatFaili     |
| HalatTakhsis   |
| WahidNadai     | 
| Takabuli       | 
| Usage          |
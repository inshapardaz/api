# Inshapardaz APIs #

Inshapardaz APIs are a way to consume inshapardaz data store. The basic aim this API to let any application access the processed entities stored in the application.

Service is a standard REST API using [HATEAOS](https://en.wikipedia.org/wiki/HATEOAS). The main entry point is the [entry](docs/entry.md) you can follow  to get access to other resources.

# Rationale

The main aim for these apis is to provide a service that can be used to develop language resources for future. The platform will be designed to support multiple languages and would allow linking of different language resources but our focus is on Urdu in the start and adding other lanuages as we go. The services provided by the apis would include:

- Dictionaries
- Thesaurus
- Spell Checker
- Grammar Checker
- Transpiler (for example converting roman to unicode urdu or shah-mukhi to gurmukhi script for punjabi)
- Translations
- Content Editors


It will allow us to create a service to define data for languages that can be exported in various formats (xml, json, third-party binary) to be used in different applications. If you are the author of an application that is based on data provided by application, it should help you updating the data for applications here and roll it out to your clients.

For example if you have defined a dictionary of medical terms and want to ship a product (free or paid) based on your this dictionary. This service should be able to define the dictionary and provide data either as apis so client applications can query it online or download in form of json, sqlite db or anything else to allow offline access to data. Mechanism can be created to allow publishers to update the repository and puclish updates to your client apps easily throught our services.

## [Security](docs/security.md)


## Contribution

This project is in a very early phase and would he beneficial to get some help on the project. Just fork the repo and create a pull request on the project to get started.
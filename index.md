# Inshapardaz APIs #

Inshapardaz APIs are a way to consume inshapardaz data store. The basic aim this API to let any application access the processed entities stored in the application.

Service is a standard REST API using [HATEAOS](https://en.wikipedia.org/wiki/HATEOAS). The main entry point is the [entry](docs/entry.md) you can follow  to get access to other resources.

## Authorisation

API will expose only the links that can be allowed for the user in context. If no user context is provided a guest user is assumed.

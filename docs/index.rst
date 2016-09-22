..  _index:

Inshapardaz REST API
=======================

..  sidebar:: Note

    This documentation is a work in progress.

Inshapardaz APIs are a way to consume inshapardaz data store. The basic aim this API to let any application access the processed entities stored in the application.

Service is a standard REST API using HATEAOS. The main entry point is the [entry] (entry) you can follow  to get access to other resources.

API Key
=======================

Currently this api do not require any key but this can change in future.

Authorisation
=======================

API will expose only the links that can be allowed for the user in context. If no user context is provided a guest user is assumed.

..  Navigation/TOC

..  toctree::
    :maxdepth: 2
    :hidden:
    :glob:
    :caption: Inshapardaz REST API

    gettingStarted
    Authentication
    standard

..  toctree::
    :maxdepth: 3
    :hidden:
    :glob:
    :caption: Requests

    /requests/dictionary/GetDictionaries
    /requests/dictionary/CreateDictionary
    /requests/dictionary/GetDictionary
    /requests/dictionary/UpdateDictionary
    /requests/dictionary/DeleteDictionary


..  toctree::
    :maxdepth: 2
    :hidden:
    :glob:
    :caption: Resources
    
    /resources/*

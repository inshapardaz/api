# Authentication

We aim to support as many popular authentication providers to make it easy for users to user the service and remove need to remember another password.

We aim to provide authentication using following:

- Google
- Facebook
- Microsoft Account
- Twitter

This feature is under development. Please stop by late to get update on the topic.

# Authorisation

Authorisation mechanism in API is role based. Roles of a user are not explicitly given by the APIs. Instead the API response would render the results based on user authorisation. For example if a user is an administrator he will get extra links in [entry](./entry.md) call to administrative functions. If user is a reader the create dictionary link would not be returned in entry response etc... 

There are three levels of a user can have:

## Administrator

Administrator user can perform administrative tasks but not able to modify the contents of private resources.

## Contributor

Contributors are the users who have access to write contents in the given context. e.g. A user is a contributor in his private dictionary. There are plans to allow multile users to act as contributors within one context. This allows collaboration on editing a single resource.

## Reader

Reader is a user who has read only access to the resource. It is similar to un-authenticated access for public resource. Plans are in place to allow people to be readers for private resources.
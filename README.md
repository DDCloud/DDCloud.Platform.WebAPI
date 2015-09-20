# DDCloud.Platform.WebAPI

Dimension Data's core framework for HTTP client APIs (creating [HttpClient](https://msdn.microsoft.com/en-us/library/system.net.http.httpclient(v=vs.110).aspx)s and creating / invoking [HTTP requests](https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessage(v=vs.118).aspx)).

It is based on the concept of an immutable template (with lazily-resolved parameters) that acts as the definition for an HTTP request. Any change to this template makes a copy of it, only capturing the differences between the old and the new template. This allows you to progressively build up a hierarchy of templates that increasingly specialise requests until they refer to a specific API operation.

For example, you might have a base request with a URL of "/api", and then extend that with a relative URI "{apiVersion}", which would effectively be "/api/{apiVersion}". Then you might extend that with "{entitySet}" (let's call this template "entity list"), and extend *that* with "{entityId}" (let's call this template "entity by Id").

So if all the entity sets exposed by your API follow this pattern then you can simply customise these templates by giving them values for "apiVersion" and "entitySet" (e.g. "user list" = "entity list with entitySet=user" and "user by Id" = "entity by Id" with "entitySet = user"). If the overall pattern for your API changes (e.g. "/api/{apiVersion}" becomes "/api/public/versions/{apiVersion}") then you simply modify the base template definition.

Note that not all template parameters have to be deferred (lazily evaluated). Some can be given constant values as part of the template definition. See the tests for examples of how the API can be used.

The HttpClient builder performs a similar role; it is a template describing how to create and configure an HttpClient (most of its power comes from the ability to define extension methods to encapsulate special configuration or custom message handlers in the pipeline). Again, see the tests for examples of how it can be used.

###### Copyright (c) 2015, Dimension Data Cloud

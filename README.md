# EngineBay.Authentication

[![NuGet version](https://badge.fury.io/nu/EngineBay.Authentication.svg)](https://badge.fury.io/nu/EngineBay.Authentication)
[![Maintainability](https://api.codeclimate.com/v1/badges/02ff0e1d109a5b09710f/maintainability)](https://codeclimate.com/github/engine-bay/authentication/maintainability)
[![Test Coverage](https://api.codeclimate.com/v1/badges/02ff0e1d109a5b09710f/test_coverage)](https://codeclimate.com/github/engine-bay/authentication/test_coverage)

Authentication module for EngineBay published to [EngineBay.Authentication](https://www.nuget.org/packages/EngineBay.Authentication/) on NuGet.

## About

Learn about [Authentication](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-7.0) and [Authorization](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-7.0).

This module provides middleware and services for both authentication and authorization. The behaviour of the authentication depends heavily on your configuration options. The module can be configured to use JWT auth, Basic auth, and no auth. JWT is recommended. The latter two should only be used during local testing - not in a production environment.

Simple endpoints to register a user or get the current user are also provided.

This module also provides implementations of [ICurrentIdentity](https://github.com/engine-bay/core/blob/main/EngineBay.Core/Interfaces/ICurrentIdentity.cs), which can be used by modules such as [EngineBay.Auditing](https://github.com/engine-bay/auditing), to get information about the current user from the HttpContext.

**Not implemented yet**: _With the authorization middleware, you can specify authorization policies on endpoints to limit access to users with the appropriate permissions._

## Usage

When this module is registered and configured, all endpoints will only allow authenticated users by default. To allow unauthenticated users to access an endpoint, you can use the `.AllowAnonymous()` method on the route builder. For an example, see [AuthenticationModule](EngineBay.Authentication/AuthenticationModule.cs).

**Not implemented yet**: _To restrict access to endpoints to users with specific permissions, you can use `.RequireAuthorization("Policy.Name")` on the route builder. For an example, see [AuditEntryEndpoint](https://github.com/engine-bay/auditing/blob/main/EngineBay.Auditing/AuditEntry/AuditEntryEndpoints.cs)._

### Registration

This module cannot run on its own. You will need to register it in your application to use its functionality. See the [Demo API registration guide](https://github.com/engine-bay/demo-api).

Since this module uses [HttpContextAccessor](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.httpcontextaccessor?view=aspnetcore-7.0) in custom components, you will need to register it on the service collection for dependency injection. This can be done with the line `builder.Services.AddHttpContextAccessor();`, as seen [here](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-context?view=aspnetcore-7.0#access-httpcontext-from-custom-components).

### Environment Variables

See the [Documentation Portal](https://github.com/engine-bay/documentation-portal/blob/main/EngineBay.DocumentationPortal/DocumentationPortal/docs/documentation/configuration/environment-variables.md#authentication).

## Dependencies

* [EngineBay.Core](https://github.com/engine-bay/core)
* [EngineBay.Persistence](https://github.com/engine-bay/persistence)
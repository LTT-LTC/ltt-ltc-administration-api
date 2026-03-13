# LTC.AdministrationService.AuthServer

## Purpose

Hosts authentication and token issuance using OpenIddict/ABP account modules.

## Responsibilities

- User authentication endpoints.
- OAuth2/OpenID Connect token issuance.
- Client configuration for Swagger or external consumers.

## Key Files

- `Program.cs`
- `AdministrationServiceAuthServerModule.cs`
- `appsettings*.json`

## Run

```bash
dotnet run --project src/LTC.AdministrationService.AuthServer/LTC.AdministrationService.AuthServer.csproj
```

## Notes

If API host validates JWT against this server, keep issuer, audience, and signing settings aligned across both projects.
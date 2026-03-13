# LTC.AdministrationService.HttpApi.Client

## Purpose

Contains typed client/proxy setup for calling administration HTTP APIs from other services/apps.

## Responsibilities

- Register remote service endpoints.
- Provide generated or typed client-side contracts.
- Centralize API client configuration.

## Key Files

- `AdministrationServiceHttpApiClientModule.cs`

## Typical Use

Reference this project from another service and register client module to call Administration APIs through typed proxies.
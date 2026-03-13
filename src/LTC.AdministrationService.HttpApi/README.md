# LTC.AdministrationService.HttpApi

## Purpose

Defines HTTP API surface for the administration service.

## Responsibilities

- Expose application services via REST endpoints.
- Configure API conventions and contracts.
- Integrate with ABP HTTP API modules.

## Key Files

- `AdministrationServiceHttpApiModule.cs`
- Controllers or conventional API exposure components in this project.

## When to Change This Project

- Add new endpoint.
- Adjust route shape or request/response binding.
- Apply endpoint-level authorization attributes.

## Typical Flow

Controller/Endpoint -> Application Service -> Domain/Persistence.
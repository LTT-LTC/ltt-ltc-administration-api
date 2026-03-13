# LTC.AdministrationService.Application.Contracts

## Purpose

Defines service contracts, DTOs, and permission constants used by clients and server implementations.

## Responsibilities

- Declare interfaces for application services.
- Define request/response DTOs.
- Define permission names and permission definition provider.

## Key Files

- `AdministrationServiceApplicationContractsModule.cs`
- `Permissions/AdministrationServicePermissions.cs`
- `Permissions/AdministrationServicePermissionDefinitionProvider.cs`

## When to Change This Project

- Add new API contract or DTO.
- Add new permission names.
- Version contracts while keeping implementations separate.

## Notes

Keep this project free of infrastructure or persistence logic.
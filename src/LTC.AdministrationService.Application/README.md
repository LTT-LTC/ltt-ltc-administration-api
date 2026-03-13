# LTC.AdministrationService.Application

## Purpose

Implements application use cases, orchestration logic, and permission checks.

## Responsibilities

- Implement service methods used by API endpoints.
- Coordinate domain objects, repositories, and external integrations.
- Apply authorization and business-level validation.

## Key Files

- `AdministrationServiceApplicationModule.cs`: DI and module setup.
- `Permission/PermissionAppService.cs`: permission query and evaluation APIs.
- Feature folders under this project: use-case implementations.

## When to Change This Project

- Add new business use case.
- Change workflow logic without changing transport protocol.
- Add application-level permission checks.

## Typical Flow

HTTP API -> Application Service -> Domain/Repository -> DTO Result
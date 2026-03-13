# LTC.AdministrationService.Domain

## Purpose

Contains core domain entities, domain services, and business rules.

## Responsibilities

- Define aggregates/entities and invariants.
- Implement domain-level operations and events.
- Remain infrastructure-agnostic.

## Key Files

- `AdministrationServiceDomainModule.cs`
- `Entities/*`
- `Events/*`

## When to Change This Project

- Add or modify business entities.
- Introduce new domain rules that should not depend on transport or persistence.

## Notes

Persistence details belong in EntityFrameworkCore project; DTOs belong in Application.Contracts.
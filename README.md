# SmartDesk AI
 
**SaaS de Ticketing & Support Client intelligent, Multi-Tenant et Cloud-Native**
 
> Plateforme de support client où chaque entreprise (tenant) dispose d'un espace totalement isolé, avec un copilote IA qui catégorise et priorise les tickets à la création — sans jamais bloquer l'agent si l'IA est indisponible.
 
---

## Pourquoi ce projet
 
Après un énième projet (GAMEVERSE, gestion de bibliothèque de jeux vidéo en .NET), j'ai voulu construire quelque chose qui pose de vraies questions d'architecture logicielle plutôt qu'un simple CRUD : isolation stricte des données entre clients d'un SaaS, découplage d'un traitement IA du flux principal, et une base de code organisée pour rester maintenable même en solo.
 
Ce dépôt documente l'intégralité du cycle de conception : cahier des charges, architecture technique (C4, ADR), avant la moindre ligne de code métier.

## Documentation

| Document | Contenu |
|---|---|
| [`docs/cahierDesChargesFonctionnel.md`](docs/cahierDesChargesFonctionnel.md) | Cahier des charges fonctionnel : contexte, personas, périmètre, exigences, MVP/roadmap |

## Stack technique

- **Backend** .NET 10, C#, Clean Architecture, CQRS (MediatR)
- **Frontend** Blazor WebAssembly
- **Base de données** SQL Server, multi-tenant via Global Query Filters EF Core
- **Messagerie asynchrone** RabbitMQ (découplage du traitement IA)
- **Orchestration** .NET Aspire (conteneurs, logs, traces)
- **Tests** xUnit dont une suite dédiée à la non-régression sur l'isolation multi-tenant



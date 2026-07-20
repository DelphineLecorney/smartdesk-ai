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
| [`docs/CahierDesChargesFonctionnel.md`](docs/CahierDesChargesFonctionnel.md) | Cahier des charges fonctionnel : contexte, personas, périmètre, exigences, MVP/roadmap |
| [`docs/ConceptionTechnique.md`](docs/ConceptionTechnique.md) | Architecture (modèle C4), modèle de données, ADR, structure de solution |
## Stack technique

- **Backend** .NET 10, C#, Clean Architecture, CQRS (MediatR)
- **Frontend** Blazor WebAssembly
- **Base de données** SQL Server, multi-tenant via Global Query Filters EF Core
- **Messagerie asynchrone** RabbitMQ (découplage du traitement IA)
- **Orchestration** .NET Aspire (conteneurs, logs, traces)
- **Tests** xUnit dont une suite dédiée à la non-régression sur l'isolation multi-tenant

## Points d'architecture

- **Isolation multi-tenant réelle** : le `TenantId` d'un utilisateur provient exclusivement de ses claims d'authentification, jamais d'un paramètre de requête, testé par des scénarios d'intégration qui tentent explicitement l'accès croisé entre tenants.
- **IA non bloquante** : la catégorisation ou l'analyse de sentiment se fait de manière asynchrone via RabbitMQ. Si le service IA est indisponible, le ticket reste pleinement en mode manuel.
- **Décisions documentées** : chaque choix technique significatif est tracé dans un ADR (voir `docs/adr/`), avec les alternatives écartées et pourquoi.

## Etat du projet

En cours de développement, voir le découpage MVP/V2/V3 dans le [Cahier des charges](docs/CahierDesChargesFonctionnel.md#8-decoupage-mvp--roadmap).

- [ ] Module A : Utilisateurs & Multi-Tenancy
- [ ] Module B : Coeur métier (Ticketing)
- [ ] Module C : Copilote IA (catégorisation)
- [ ] Tests d'intégration isolation multi-tenant
- [ ] Déploiement de démo

## Lancer le projet en local

```bash
git clone https://github.com/<votre-user>/smartdesk-ai.git
cd smartdesk-ai
dotnet restore
# Orchestration complète (API + DB + RabbitMQ) via Aspire
dotnet run --project aspire/SmartDeskAI.AppHost
```

## Auteur

Développeuse en reconversion vers le développement .NET/C#.

Ce projet illustre une approche de conception complète, du cahier des charges au code plutôt qu'un exercice technique isolé.
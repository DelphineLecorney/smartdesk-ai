# ADR-0001 : Stratégie multi-tenant, Single Database + Discriminator Column

- **Statut** : Accepté
- **Date** : Juillet 2026

## Contexte
Besoin d'isoler les données de plusieurs entreprises clientes.

## Options considérées
  - Option 1 : base de données dédiée par tenant (isolation maximale, coût opérationnel élevé)
  - Option 2 : schéma dédié par tenant (isolation moyenne, complexité de migration)
  - Option 3 : single database + colonne discriminante `TenantId` (isolation logique, coût faible)

## Décision
Option 3, via des Global Query Filters EF Core.

## Conséquences
Nécessite une discipline stricte de tests d'intégration pour garantir qu'aucune requête ne contourne le filtre (ex : requêtes SQL brutes à proscrire ou à sécuriser manuellement).

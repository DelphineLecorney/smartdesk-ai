# ADR-0005 : Verrouillage optimiste pour la concurrence sur un ticket

- **Statut** : accepté
- **Date** : Juillet 2026

## Contexte
Deux agents peuvent modifier le même ticket simultanément.

## Options considérées
  - Option 1 : verrouillage pessimiste, lock en base et complexité opérationnelle, risque de deadlock.
  - Option 2 : verrouillage optimiste (`RowVersion`, `ConcurrencyToken` EF Core) standard, peu de code.
  - Option 3 : indicateur de présence en temps réel (SignalR), meilleure UX mais infra supplémentaire non justifié pour un MVP

## Décision
Option 2 pour le MVP. Option 3 envisageable en V3.

## Conséquences
En cas de modification concurrente, le second agent qui tente d'enregistrer ses changements reçoit une erreur de concurrence et doit recharger le ticket.

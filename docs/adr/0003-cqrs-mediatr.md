# ADR-0003 : CQRS avec MediatR

- **Statut** : accepté.
- **Date** : Juillet 2026

## Contexte
Séparer les cas d'usage de lecture (queries, souvent optimisés, DTO plats) des cas d'usage d'écriture (commands avec validation et règles métier).

## Décision
Un handler MediatR par Command/Query, validation via FluentValidation en pipeline behavior.

## Conséquences
Plus de fichiers qu'une approche "service classique" mais chaque cas d'usage est isolé, testable unitairement et le code rese lisible même quand le projet grossit.

# ADR-0006 : Abstraction du fournisseur IA (`IAIAnalysisService`)

- **Statut** : accepté
- **Date** : Juillet 2026

## Contexte
Projet mené sans budget, besoin de pouvoir utiliser un LLM gratuit ou local (Ollama, Mistral) pendant le développement, sans figer un fournisseur payant dans le code métier.

## Décision
Toute interaction avec un LLM passe par l'interface `IAIAnalysisService`, définie côté Application. L'implémentation concrète (Infrastructure) est substituable sans impact sur le reste du système.

## Conséquences
Léger surcoût de conception au départ mais permet de changer de fournisseur (Ollama en local, Mistral API, ou un fournisseur payant plus tard) sans toucher au Domain ni à l'Application. Facilite aussi les tests (mock de l'interface).

Point d'attention RGPD, si un fournisseur externe est utilisé, où les données ne sortent jamais de la machine, cela constitue une sous-tratance de données à documenter dans les CGU.
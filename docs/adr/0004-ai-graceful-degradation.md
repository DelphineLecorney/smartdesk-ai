# ADR-0004 : Dégradation gracieuse en cas d'échec ou de latence IA

- **Statut** : accepté
- **Date** : Juillet 2026

## Contexte
Un appel au LLM peut échouer ou traîner indéfiniment, le service de ticketing ne dois jamais dépendre de la disponibilité de l'IA pour fonctionner.

## Décision
Timeout de 10 secondes sur l'appel au LLM, un retry automatique (backoff 2s) puis abandon avec enregistrement d'audit. Le ticket reste pleinement utilisable en mode manuel en cas d'échec.

## Conséquences
L'agent peut se retrouver sans suggestion IA sur certains tickets, cela reste acceptable mais le service principal, le ticketing n'est jamais bloqué par un tiers externe.

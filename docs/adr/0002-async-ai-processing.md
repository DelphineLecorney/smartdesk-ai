# ADR-0002 : Traitement IA asynchrone via message broker

- **Statut** : accepté.
- **Date** : Juillet 2026

## Contexte
L'appel à un LLM externe peut prendre plusieurs secondes, il ne doit pas bloquer la création d'un ticket.

## Options considérées

  - Option 1 : appel synchrone dans la requête HTTP (latence subie par l'utilisateur)
  - Option 2 : background job in-process (`IHostedService`) sans broker (perte de messages si le process redémarre)
  - Option 3 : message broker (RabbitMQ) + worker dédié

## Décision
Option 3.

## Conséquences
Ajoute un composant d'infrastructure supplémentaire mais garantit la résilience (les messages non traités restent dans la queue) et la scalabilité indépendante du worker.
 
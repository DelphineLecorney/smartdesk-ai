# Cahier des Charges Fonctionnel : SmartDesk AI

**Saas de Ticketing & Support Client intelligent, Mutli-Tenant et Cloud-Native**

Version 1.0 (juillet 2026)

---

## Table des matières

1. Contexte & Objectifs Business

---

## 1. Contexte & Objectifs Business

**Le Problème** : Les services de support client des PME sont débordés par des demandes répétitives, mal catégorisées, ce qui augmente le temps de résolution et frustre les clients.

**La Solution** : Une plateforme SaaS d'assistance où chaque entreprise cliente (Tenant) dispose de son espace isolé. L'IA intervient comme un copilote pour catégoriser les tickets dès leur réception et suggérer des réponses aux agents humains.

**Objectif Business** : Réduire de 40% le temps de traitement initial d'un ticket grâce à l'assistance IA, tout en garantissant une étanchéité absolue des données entre les clients du Saas.

**Objectifs secondaires** : 
- Démontrer une maîtrise de la Clean Architecture + CQRS sur un cas d'usage réaliste, pas artificiel.
- Illustrer une problématique multi-tenant avec de vrais enjeux de sécurité (pas juste un "hello word" avec un `TenantId` en colonne).
- Montrer une intégration IA pragmatique (pas un simple appel d'API sans réflexion sur l'asynchrone, le coût, les fallbacks).
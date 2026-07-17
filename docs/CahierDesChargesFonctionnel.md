# Cahier des Charges Fonctionnel : SmartDesk AI

**Saas de Ticketing & Support Client intelligent, Mutli-Tenant et Cloud-Native**

Version 1.0 (juillet 2026)

---

## Table des matières

1. Contexte & Objectifs Business
2. Personas & Parcours Utilisateurs

---

## 1. Contexte & Objectifs Business

**Le Problème** : Les services de support client des PME sont débordés par des demandes répétitives, mal catégorisées, ce qui augmente le temps de résolution et frustre les clients.

**La Solution** : Une plateforme SaaS d'assistance où chaque entreprise cliente (Tenant) dispose de son espace isolé. L'IA intervient comme un copilote pour catégoriser les tickets dès leur réception et suggérer des réponses aux agents humains.

**Objectif Business** : Réduire de 40% le temps de traitement initial d'un ticket grâce à l'assistance IA, tout en garantissant une étanchéité absolue des données entre les clients du Saas.

**Objectifs secondaires** : 
- Démontrer une maîtrise de la Clean Architecture + CQRS sur un cas d'usage réaliste, pas artificiel.
- Illustrer une problématique multi-tenant avec de vrais enjeux de sécurité (pas juste un "hello word" avec un `TenantId` en colonne).
- Montrer une intégration IA pragmatique (pas un simple appel d'API sans réflexion sur l'asynchrone, le coût, les fallbacks).

---

## 2.Personas & Parcours Utilisateurs

### 2.1 Personas

| Persona | Rôle | Objectif principal | Frustration actuelle |
|---|---|---|---|
| **Mathilde** | Administratice client (PME e-commerce) | Superviser son équipe support, maîtriser les coûts | Perd du temps à réassigner manuellement les tickets mal triés |
| **Ismaël** | Agent de support | Résoudre un maximum de tickets avec qualité | Doit lire chaque ticket en entier avant de savoir quoi répondre |
| **Philippe** | Client final | Obtenir une réponse rapide à son problème | Ne sait jamais où est son ticket, doit relancer par email |

### 2.2 User Stories (je le remplirais au fur et à mesure)

**Module A :  Utilisateurs & Multi-tenancy**
- En tant qu'Administratice client, je veux inviter un nouvel agent par email, pour qu'il rejoigne mon espace sans que j'aie à créer son mot de passe.
- En tant qu'Agent, je ne dois jamais pouvoir accéder à une URL de ticket d'un autre tenant même en devinant ou en forçant (test de non-régression obligatoire).

**Module B : Ticketing**
- En tant que Client Final, je veux créer un ticket avec pièce jointe, pour illustrer mon problème, comme la capture d'écran d'un bug.
- En tant qu'Agent, je veux laisser une note interne invisible au client, pour échanger avec un collègue sans polluer le fil visible.
- En tant qu'Administratice, je veux voir un ticket réassigné automatiquement si l'agent ne répond pas sous X heures pour éviter qu'un ticket reste orphelin.

**Module C : IA Copilote**
- En tant qu'Agent, je veux voir la catégorie et le sentiment suggérés par l'IA dès l'ouverture du ticket, pour prioriser sans tout relire.
- En tant qu'Agent, je veux pouvoir rejeter ou corriger une suggestion IA, avoir une trace de cette correction pour l'audit et l'amélioration future.
- En tant qu'Administratice, je veux voir un indicateur de fiabilité de l'IA  par rapport aux suggestions acceptées et celles corrigées pour les évaluer.

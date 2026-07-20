# Cahier des Charges Fonctionnel : SmartDesk AI

**Saas de Ticketing & Support Client intelligent, Mutli-Tenant et Cloud-Native**

Version 1.0 (juillet 2026)

---

## Table des matières

1. Contexte & Objectifs Business
2. Personas & Parcours Utilisateurs
3. Périmètre Fonctionnel
4. Règles de Gestion & Cas Limites
5. Exigences Non-fonctionnelles
6. Modèle de données (vue conceptuelle)
7. Contraintes Techniques & Architecture
8. Découpage MVP / Roadmap
9. Risques & Mitigations
10. Glossaire

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
| **Mathilde** | Administratrice client (PME e-commerce) | Superviser son équipe support, maîtriser les coûts | Perd du temps à réassigner manuellement les tickets mal triés |
| **Ismaël** | Agent de support | Résoudre un maximum de tickets avec qualité | Doit lire chaque ticket en entier avant de savoir quoi répondre |
| **Philippe** | Client final | Obtenir une réponse rapide à son problème | Ne sait jamais où est son ticket, doit relancer par email |

### 2.2 User Stories

**Module A :  Utilisateurs & Multi-tenancy**
- En tant qu'Administratrice client, je veux inviter un nouvel agent par email, pour qu'il rejoigne mon espace sans que j'aie à créer son mot de passe.
- En tant qu'Agent, je ne dois jamais pouvoir accéder à une URL de ticket d'un autre tenant même en devinant ou en forçant (test de non-régression obligatoire).

**Module B : Ticketing**
- En tant que Client Final, je veux créer un ticket avec pièce jointe, pour illustrer mon problème, comme la capture d'écran d'un bug.
- En tant qu'Agent, je veux laisser une note interne invisible au client, pour échanger avec un collègue sans polluer le fil visible.
- En tant qu'Administratrice, je veux voir un ticket réassigné automatiquement si l'agent ne répond pas sous X heures pour éviter qu'un ticket reste orphelin.

**Module C : IA Copilote**
- En tant qu'Agent, je veux voir la catégorie et le sentiment suggérés par l'IA dès l'ouverture du ticket, pour prioriser sans tout relire.
- En tant qu'Agent, je veux pouvoir rejeter ou corriger une suggestion IA, avoir une trace de cette correction pour l'audit et l'amélioration future.
- En tant qu'Administratrice, je veux voir un indicateur de fiabilité de l'IA  par rapport aux suggestions acceptées et celles corrigées pour les évaluer.

---

## 3. Périmètre Fonctionnel

### Module A :  Gestion des utilisateurs & Multi-tenancy (sécurité)

- **Isolation des données** : un utilisteur du Tenant A ne peut sous aucun prétexte afficher, modifier ou même déduire l'existence d'une ressource du Tenant B (y compris via les messages d'erreur, pas de 403 qui confirme l'existence d'un ID).
- **Rôles** : 
  - *Administratrice client* : gère l'abonnement, créé et désactive les comptes agents, configure les règles de routage.
  - *Agent de support* : traite les tickets, utilise l'assistance IA.
  - *Client final* : créé des tickets, suit leur résolution via un portail simplifié.
- **Onboading tenant** : création d'un tenant = provisioning automatique (pas d'intervention manuelle en base).
- **Gestion de session** : authentification, expiration, révocation d'accès immédiate si un agent est désactivé (pas d'attente d'expiration de token).

### Module B : Coeur Métier (Ticketing)

- **Cycle de vie d'un ticket** : `Nouveau` -> `En cours` -> `En attente d'informations` -> `Résolu` -> `Clôturé` (avec possibilité de réouverture sous conditions, voir 4).
- **Priorités** : `Basse`, `Moyenne`, `Haute`, `Critique` modifiables manuellement même après suggestion IA.
- **Attribution** : assignation à un agent spécifique ou à un groupe, une file d'attente.
- **Fil de discussion** : échanges chronologiques client/agent + notes internes.
- **Pièces jointes** : upload sécurisé (types de fichiers limités, taille max).
- **Historique et audit** : toute action significative (changement de statut, réassignation, correction IA) est tracée avec horodatage et auteur.

### Module C : L'Intelligence Artificielle (Le Copilote)

- **Catégorisation automatique** : analyse asynchrone du texte à la création -> tags (`#Bug`, `#Facturation`, `#RGPD`).
- **Analyse de sentiment** : détection de frustration, d'urgence -> ajustement suggéré de la priorité (jamais automatique et silencieux, l'agent est notifié du changement et peut l'annuler).
- **Suggestion de réponse** : ébauche modifiable par l'agent avant envoi.
- **Traçabilité IA** : chaque suggestion (catégorie, sentiment, réponse) est stockée avec la décision de l'agent (acceptée, modifiée, rejetée) pour permettre un futur entraînement ou audit.
- **Dégradation gracieuse** : si le service IA est indisponible ou timeout, le ticket reste utilisable normalement en mode manuel (l'IA ne doit jamais être un point de blocage).

---

## 4. Règles de gestion & cas limites

- **Réouverture d'un ticket clôturé** : autorisée dans les 7 jours suivant la clôture. Au-delà, le client doit créer un nouveau ticket référençant l'ancien.
- **Désactivation d'un compte agent avec tickets non résolus** : ses tickets sont automatiquement réassignés vers la file d'attente non assignée du tenant (le statut du ticket n'est pas modifié, seule l'assignation change). Un enregistrement d'audit est créé : `"Ticket désassigné suite à la désactivation de {agent}"`.
- **Un tenant qui dépasse son quota de tickets/mois** : la création de tickets reste possible sans limitation, seul le traitement IA est désactivé jusqu'au renouvellement du quota, cohérent avec le principe que l'IA ne doit jamais être bloquante pour le service. Le client est informé du dépassement.
- **Concurrence** : deux agents ouvrent ou modifient le même ticket en même temps -> verrouillage optimiste (colonne `RowVersion`, `ConcurrencyToken` côté EF Core). Le second agent à sauvegarder reçoit une erreur explicite et doit recharger le ticket avant de réessayer. Un indicateur de présence en temps réel (SignalR) sera mis pour la V3.
- **Latence IA** : timeout de 10 secondes de l'appel au LLM avec un retry automatique de 2 secondes avant abandon. En cas d'échec définitif, le ticket reste utilisable en mode manuel (sans catégorie ou sentiment IA) et un enregistrement d'audit `"Analyse IA échouée pour ce ticket"` est créé pour permettre le monitoring du taux d'échec.
- **Langue** : mono-langue pour la V1, français, pas de détection de langue ni de traduction, la suggestion de réponse est toujours générée en français. Le support multilingue est pour la V3.
- **RGPD et droit à l'oubli** : anonymisation, pas suppression. Le contenu du ticket (`Subject`, `Message.Content`) reste en base pour l'historique et les statistiques, les données identifiantes du client (`Email`, `nom`) sont remplacées par des valeurs génériques. Le compte `User` passe au statut distinct `Anonymized` (voir `UserStatus`), différent de `Deactivated` pour ne pas confondre une désactivation classique avec une demande RGPD. Un enregistrement d'audit trace l'opération (date, demandeur).
- **Multi-tenant + IA partagée** : le traitement IA repose sur un LLM stateless, accédé via une abstraction (`IAIAnalysisService`) permettant de changer de fournisseur sans impact sur le reste du système, choix motivé à la fois par une bonne pratique d'architecture et par une contrainte budgétaire réelle (fournisseurs gratuits, locaux comme Ollama ou Mistral en phase de développement, projet mené sans budget). Aucun risque de fuite d'apprentissage entre tenants (modèle figé et pas de mémoire persistante). Le contenu envoyé au LLM se limite strictement au ticket concerné, jamais d'historique cross-tenant. Si un fournisseur externe est utilisé, il sera encadré et détaillé dans nos CGU.

## 5. Exigences Non-fonctionnelles

| Catégorie | Exigence |
|---|---|
| **Performance** | Temps de réponse API < 300ms hors appel IA, traitement IA asynchrone, non bloquant pour l'utilisateur |
| **Sécurité** | Authentification OIDC, autorisation par claims incluant le `TenantId`, chiffrement des données sensibles au repos, protection contre l'IDOR (accès direct par ID) |
| **Disponibilité** | Cible 99% en environnement de démo, retry policy sur les appels RabbitMQ/IA avec backoff exponentiel |
| **Scalabilité** | Architecture stateless côté API pour permettre un scaling horizontal |
| **Conformité RGPD** | Anonymisation possible d'un client final, export de ses données sur demande, durée de rétention documentée |
| **Observabilité** | Logs structurés et traces distribués via .NET Aspire, corrélation par `TenantId` + `TicketId` |
| **Testabilité** | Couverture cible sur le Domain et l'Application layer (Clean Architecture), tests d'intégration dédiés à l'isolation multi-tenant |
| **Accessibilité** | Portail client conforme aux bases WCAG (contrastes, navigation clavier) |

---

## 6. Modèle de données (vue conceptuelle)

Entités principales et relations clés (le détail des colonnes sera affiné en phase de conception technique) :

- **Tenant** (1) -> (N) **User** : chaque `User` porte un `TenantId` et un `Role`.
- **Tenant** (1) -> (N) **Ticket** : chaque `Ticket` porte un `TenantId` (filtré via Global Query Filter EF Core).
- **Ticket** (1) -> (N) **Message** : chaque `Message` a un flag `IsInternalNote`.
- **Ticket** (1) -> (0..1) **AIAnalysis** : catégorie, sentiment, priorité suggérée, réponse suggérée, statut de la décision agent (acceptée, modifiée ou rejetée).
- **Ticket** (1) -> (N) **Attachment**.
- **Ticket** (1) -> (N) **AuditLogEntry** : traçabilité des changements de statut, d'assignation.

> POint d'attention architecture : le `TenantId` doit être injecté depuis le contexte d'authentification (claims), jamais depuis une valeur envoyée par le client dans la requête sinon l'isolation multi-tenant est cassée par design.

---

## 7. Contraintes Techniques & Architecture

- **Backend** : .NET 10 (Web API, C#).
- **Frontend** : Blazor WebAssembly.
- **Base de données** : SQL Server, stratégie multi-tenant "Single Database, Discriminator Column" via Global Query Filters EF COre.
- **Architecture logicielle** : Clean Architecture (Domain, Application, Infrastructure, Presentation) + CQRS (MediatR).
- **Messagerie asynchrone** : RabbitMQ pour découpler le traitement IA de la création de ticket.
- **Orchestration & Observabilité** : .NET Aspire (conteneurs, logs, traces centralisées).
- **IA** : appel à un LLM externe (API OpenAI, Ollama, un modèle léger pour la démo, encapsulé derrière une interface `IAInalysisService` pour rester substituable et testable (mocks en tests unitaires).

---

## 8. Découpage MVP / Roadmap

**MVP (livrable présentable)** : 
- Module A : cas d'usage retenus : `InviteUser`, `ActivateUser`, `DeactivateUser` (avec réassignation des tickets), `GetUsersByTenant`, `GetUserById`, authentification (`Login`). La création de tenant (`CreateTenant`) sera en V2, la V1 étant un MVP, les tenants de test sont créés directement en base (seed).
- Module B complet (cycle de vie, priorités, fil de discussion, notes internes).
- Module C réduit : catégorisation automatique uniquement (pas encore sentiment ni suggestion de réponse).
- Tests d'intégration sur l'isolation multi-tenant.

**V2** :
- Création de tenant par l'utilisateur (`CreateTenantCommand`), sans intervention manuelle en base.
- Analyse de sentiment + ajustement de priorité.
- Suggestion de réponse par l'IA.
- Dashboard analytics pour l'administratrice client (taux de résolution, fiabilité IA).

**V3** :

- Portail client final en marque blanche (personnalisé à l'identité visuelle de chaque client).
- Modifications temps réel (SignalR).
- Export de rapports (PDF, Excel).

---

## 9. Riques & Mitigations

| Risque | Mitigation |
|---|---|
| Fuite de données entre tenants (bug dans unQuery Filter)| Tests d'intégration systématiques, revue de code ciblée sur toute requête EF Core |
| Dépendance forte à un service IA externe (coût, disponibilité) | Interface abstraite + mode dégradé fonctionnel sans IA |
| Périmètre qui explose (feature creep) | Respect strict du découpage MVP/V2/V3 ci-dessus |
| Projet solo, motivation sur la durée | Découpage en petis jalons livrables et démontrables |

---

## 10. Glossaire

- **Tenant** : entreprise cliente du Saas, disposant d'un espace isolé.
- **SLA** : Service Level Agreement, engagement de délai de traitement.
- **Query Filter Global (EF Core)** : filtre automatique appliqué à toutes les requêtes pour restreindre les résultats au tenant courant.
- **IDOR** : Insecure Direct Object Reference, faille où un utilisateur accède à une ressource d'un autre en devinant ou en modifiant un identifiant.

---

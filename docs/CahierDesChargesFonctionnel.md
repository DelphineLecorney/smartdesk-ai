# Cahier des Charges Fonctionnel : SmartDesk AI

**Saas de Ticketing & Support Client intelligent, Mutli-Tenant et Cloud-Native**

Version 1.0 (juillet 2026)

---

## Table des matières

1. Contexte & Objectifs Business
2. Personas & Parcours Utilisateurs
3. Périmètre Fonctionnel
4. Règles de Gestion & Cas Limites

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

### 2.2 User Stories (je le remplirais au fur et à mesure)

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
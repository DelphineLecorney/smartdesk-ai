# Dossier de conception technique : SmartDesk AI

Version 1.0, suite du Cahier des Charges Fonctionnel (CDCF:CahierDesChargesFonctionnel.md)

---

## Table des matières

1. Vue d'ensemble architecturale (modèle C4)
2. Modèle de données détaillé
3. Architecture Decision Records (ADR)
4. Flux clés (séquence)
5. Structure de solution .NET

---

## 1. Vue d'ensemble architecturale (modèle C4)

Le modèle C4 permet de documenter l'architecture à différents niveaux de zoom.

### 1.1 Niveau 1 — Contexte système

Qui utilise le système et avec quoi interagit-il ?

```mermaid
graph LR
    %% --- Déclarations des blocs ---
    Client["👤 Client Final<br>(Tickets et suivi)"]
    Agent["🎧 Agent de Support<br>(Traitement et IA)"]
    Admin["⚙️ Administrateur<br>(Gestion du Tenant)"]
    
    SmartDesk["🚀 SmartDesk AI<br>(SaaS Ticketing Multi-tenant)"]
    
    LLM["🧠 Service LLM externe<br>(Analyse et suggestions)"]
    Mail["✉️ Service Email<br>(Notifications)"]

    %% --- Liens de flux avec texte ---
    Client -->|HTTPS| SmartDesk
    Agent -->|HTTPS| SmartDesk
    Admin -->|HTTPS| SmartDesk
    
    SmartDesk -->|API / HTTPS| LLM
    SmartDesk -->|SMTP / API| Mail
```

<hr style="width: 25%; margin: 50px auto;" />

### 1.2 Niveau 2 — Conteneurs

```mermaid
graph LR
    User["👤 Utilisateur<br>(Client, Agent ou Admin)"]
 
    Web["💻 Frontend<br>Blazor WebAssembly"]
    Api["🔗 API<br>.NET 10 Web API"]
    Bus["📨 Message Broker<br>RabbitMQ"]
    Worker["⚙️ Worker IA<br>.NET Worker Service"]
    Db[("🗄️ Base de données<br>SQL Server multi-tenant")]
 
    LLM["🧠 LLM externe"]
 
    User -->|HTTPS| Web
    Web -->|"HTTPS / JSON"| Api
    Api -->|"EF Core"| Db
    Api -->|"Publie TicketCreated (AMQP)"| Bus
    Bus -->|"Consomme (AMQP)"| Worker
    Worker -->|HTTPS| LLM
    Worker -->|"EF Core"| Db
```
 
<hr style="width: 25%; margin: 50px auto;" />

### 1.3 Niveau 3 — Composants (zoom sur l'API)

 
```mermaid
graph LR
    Presentation["🌐 Presentation<br>Controllers Minimal API<br>Points d'entrée HTTP"]
    Application["🧩 Application<br>MediatR Handlers<br>Commands/Queries"]
    Domain["🎯 Domain<br>Entités, Value Objects<br>Règles métier pures"]
    Infrastructure["🔧 Infrastructure<br>EF Core, Repos, Clients externes"]
 
    Presentation -->|"Envoie Commands/Queries"| Application
    Application -->|"Manipule"| Domain
    Application -.->|"Utilise via interfaces (DIP)"| Infrastructure
```

> **Principe clé (Clean Architecture)** : les flèches de dépendance pointent toujours vers le `Domain`. L'`Infrastructure` dépend de l'`Application` via des interfaces définies dans l'`Application`, jamais l'inverse. C'est ce qui permet de tester le métier sans base de données ni appel réseau.

---

## 2. Modèle de données détaillé.

### 2.1 Diagramme entité-relation

```mermaid
erDiagram
    TENANT ||--o{ USER : possede
    TENANT ||--o{ TICKET : possede
    USER ||--o{ TICKET : assigne_a
    USER ||--o{ MESSAGE : redige
    TICKET ||--o{ MESSAGE : contient
    TICKET ||--o| AI_ANALYSIS : analyse_par
    TICKET ||--o{ ATTACHMENT : possede
    TICKET ||--o{ AUDIT_LOG_ENTRY : trace

    TENANT {
        guid Id PK
        string Name
        string Subdomain
        datetime CreatedAt
        string SubscriptionPlan
    }

    USER {
        guid Id PK
        guid TenantId FK
        string Email
        string Role
        bool IsActive
        datetime CreatedAt
    }

    TICKET {
        guid Id PK
        guid TenantId FK
        guid CreatedByUserId FK
        guid AssignedAgentId FK
        string Subject
        string Status
        string Priority
        datetime CreatedAt
        datetime ClosedAt
    }

    MESSAGE {
        guid Id PK
        guid TicketId FK
        guid AuthorId FK
        string Content
        bool IsInternalNote
        datetime CreatedAt
    }

    AI_ANALYSIS {
        guid Id PK
        guid TicketId FK
        string SuggestedCategory
        string DetectedSentiment
        string SuggestedPriority
        string SuggestedReply
        string AgentDecision
        datetime AnalyzedAt
    }

    ATTACHMENT {
        guid Id PK
        guid TicketId FK
        string FileName
        string StorageUrl
        int SizeBytes
    }

    AUDIT_LOG_ENTRY {
        guid Id PK
        guid TicketId FK
        guid ActorId FK
        string Action
        string OldValue
        string NewValue
        datetime OccurredAt
    }
```

<hr style="width: 25%; margin: 50px auto;" />

### 2.2 Point d'attention : isolation multi-tenant

Toutes les entités portant un `TenantId` doivent avoir un **Global Query Filter** EF Core configuré dans `OnModelCreating` :

```csharp
modelBuilder.Entity<Ticket>()
    .HasQueryFilter(t => t.TenantId == _currentTenantService.TenantId);
```

Le `_currentTenantService.TenantId` doit provenir **exclusivement** des claims du token d'authentification, jamais d'un paramètre de requête ou de route.

---

## 3. Architecture Decision Records (ADR)

Un ADR documente une décision technique, son contexte et ses alternatives écartées.

### ADR-001 : Stratégie multi-tenant, Single Database + Discriminator Column

- **Statut** : Accepté
- **Contexte** : besoin d'isoler les données de plusieurs entreprises clientes.
- **Options considérées** :
  - Option 1 : Base de données dédiée par tenant (isolation maximale, coût opérationnel élevé)
  - Option 2 : Schéma dédié par tenant (isolation moyenne, complexité de migration)
  - Option 3 : Single database + colonne discriminante `TenantId` (isolation logique, coût faible)
- **Décision** : Option 3, via Global Query Filters EF Core.
- **Conséquences** : nécessite une discipline stricte de tests d'intégration pour garantir qu'aucune requête ne contourne le filtre (ex : requêtes SQL brutes à proscrire ou à sécuriser manuellement).

<hr style="width: 25%; margin: 50px auto;" />


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
 
Chaque ADR est un fichier séparé dans [`docs/adr/`](adr/) (convention standard : un fichier par décision, numéroté) pour rester consultable et versionné indépendamment de ce document.
 
| ADR | Décision |
|---|---|
| [0001](adr/0001-multi-tenant-strategy.md) | Stratégie multi-tenant, Single Database + Discriminator Column |
| [0002](adr/0002-async-ai-processing.md) | Traitement IA asynchrone via message broker |
| [0003](adr/0003-cqrs-mediatr.md) | CQRS avec MediatR |
| [0004](adr/0004-ai-graceful-degradation.md) | Dégradation gracieuse en cas d'échec ou de latence IA |
| [0005](adr/0005-optimistic-concurrency.md) | Verrouillage optimiste pour la concurrence sur un ticket |
| [0006](adr/0006-ai-provider-abstraction.md) | Abstraction du fournisseur IA (`IAIAnalysisService`) |
 
> D'autres ADR seront ajoutés au fil du développement (ex : mécanisme d'authentification, stratégie de cache, gestion des migrations EF Core en multi-tenant).
Un nouveau fichier `000N-titre.md` à chaque nouvelle décision structurante.

 
---


## 4. Flux clés (séquences)

### 4.1 Création d'un ticket avec analyse IA asynchrone

```mermaid
sequenceDiagram
    actor Client
    participant API
    participant DB as Base de données
    participant Bus as RabbitMQ
    participant Worker as WOrker IA
    participant LLM as Service LLM

    Client ->>API: POST / tickets
    API->>DB: INSERT Ticket (Status=NOuveau)
    API->>Bus: Publie TicketCreatedEvent
    API->>Client: 201 Created (ticket visible immédiatement)

    Bus->>Worker: Consomme TicketCreatedEvent
    Worker->>LLM: Analyse (catégorie, sentiment)
    LLM-->>Worker: Résultat
    Worker->>DB: Insert AIAnalysis
    Worker->>DB: Update Ticket.Priority (si sentiment critique)
```

<hr style="width: 25%; margin: 50px auto;" />


> Le client obtient une réponse immédiate, l'enrichissement IA arrive quelques secondes après, sans bloquer personne.


## 5. Structure de la solution
 
```
SmartDeskAI/
├── src/
│   ├── SmartDeskAI.Domain/           # Entités, Value Objects, règles métier pures
│   ├── SmartDeskAI.Application/      # Commands, Queries, Handlers, interfaces
│   ├── SmartDeskAI.Infrastructure/   # EF Core, repos, client LLM, RabbitMQ
│   ├── SmartDeskAI.Api/              # Minimal API, controllers, mapping DTO
│   ├── SmartDeskAI.Worker/           # Worker service consommant la queue IA
│   └── SmartDeskAI.Web/              # Blazor WebAssembly
├── tests/
│   ├── SmartDeskAI.Domain.Tests/
│   ├── SmartDeskAI.Application.Tests/
│   └── SmartDeskAI.IntegrationTests/ # Dont tests d'isolation multi-tenant  
├── docs/
│   ├── CahierDesChargesFonctionnel.md
│   ├── ConceptionTechnique.md
│   └── adr/
│   └── images/
├── README.md
└── SmartDeskAI.sln
```
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
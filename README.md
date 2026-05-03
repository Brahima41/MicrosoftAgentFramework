# Pipeline d'analyse de tickets support à traiter

Système de traitement automatisé de tickets support utilisant **MAF (Microsoft Agents Framework)**, **MCP (Model Context Protocol)** et **Ollama**
pour orchestrer un workflow d'agents intelligents.

## Description

Dans ce projet vous découvrirez comment créer un workflow d'agents AI collaboratifs pour traiter automatiquement des tickets de support client.
Le système utilise trois agents spécialisés qui travaillent en séquence :

1. **Agent de Triage** - Analyse et structure le ticket initial
2. **Agent Analyste** - Consulte la base de connaissances et calcule la priorité
3. **Agent Rédacteur** - Génère les réponses client et notes internes

## Architecture

```
Ticket Support
     ↓
Agent Triage (Analyse)
     ↓
Agent Analyste (Base de connaissances MCP + Outils locaux)
     ↓
Agent Rédacteur (Communication)
     ↓
Réponse finale (Client + Equipe interne)
```

### Technologies utilisées

- **Microsoft Agents AI** - Framework d'orchestration d'agents
- **Ollama** - LLM local (modèle qwen3:4b)
- **MCP (Model Context Protocol)** - Accès à la base de connaissances SQLite
- **SQLite** - Base de données des problèmes connus
- **.NET 10** / **C# 14.0**

## Prérequis

### 1. Ollama
Installez Ollama et téléchargez le modèle :
```bash
# Installation : https://ollama.ai
ollama pull qwen3:4b
```

### 2. MCP Server SQLite
Installez le serveur MCP pour SQLite :
[https://github.com/astral-sh/uv](https://github.com/astral-sh/uv)

## Installation

1. **Cloner le repository**
```bash
git clone https://github.com/Brahima41/MicrosoftAgentFramework.git
cd MicrosoftAgentFramework
```

2. **Restaurer les packages NuGet**
```bash
dotnet restore
```

3. **Lancer l'application**
```bash
dotnet run --project ConsoleApp1
```

## Structure du projet

```
ConsoleApp1/
├── Program.cs                      # Point d'entrée, workflow principal
├── KnowledgeBaseSeeder.cs          # Initialisation de la base SQLite
├── SupportTools.cs                 # Outils locaux (CalculatePriority)
├── AgentMiddlewareExtensions.cs    # Extensions pour les middlewares
└── knowledge_base.db               # Base de données SQLite
```

## Licence

Ce projet est sous licence MIT.

## Auteur

**Brahima Baré**
- GitHub: [@Brahima41](https://github.com/Brahima41)
- Repository: [MicrosoftAgentFramework](https://github.com/Brahima41/MicrosoftAgentFramework)

  ---

**Note** : Ce projet est un exemple pédagogique démontrant l'utilisation du Microsoft Agents Framework avec MCP et Ollama. Il n'est pas destiné à une utilisation en production sans adaptations supplémentaires.

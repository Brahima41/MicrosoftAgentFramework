using ConsoleApp1;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Client;
using OllamaSharp;

KnowledgeBaseSeeder.Seed("knowledge_base.db");

// Connexion au serveur Création du client MCP avec transport stdio
await using McpClient mcpClient = await McpClient.CreateAsync(
    new StdioClientTransport(new StdioClientTransportOptions
    {
        Name = "KnowledgeBase",
        Command = "uvx",
        Arguments = ["mcp-server-sqlite", "--db-path", "knowledge_base.db"]
    }));

// Les outils sont découverts automatiquement
var mcpTools = await mcpClient.ListToolsAsync();

Console.WriteLine($"Outils MCP : {string.Join(", ", mcpTools.Select(t => t.Name))}");

// Création du client Ollama
var chatClient = new OllamaApiClient(
    new Uri("http://localhost:11434"),
    "qwen3:4b");


var localTool = AIFunctionFactory.Create(SupportTools.CalculatePriority);

// Agent 1 : Triage — raisonne sur le texte, pas d'outils
ChatClientAgent triageAgent = new(chatClient, """
    Tu es un agent de triage support.
    Lis le ticket, vérifie qu'il est complet et pertinent.
    Résume le problème en 2-3 phrases claires.
    Identifie : le type d'erreur, l'ID client, le niveau d'urgence.
    """, "Triage");

// Agent 2 : Analyste — consulte l'historique et la base de connaissance
ChatClientAgent analysteAgent = new(chatClient, """
    Tu es un analyste support expérimenté. Suis ces étapes dans l'ordre :
    1. Identifie le type d'erreur du ticket (ex: "403", "performance", "billing").
    2. Appelle read_query avec : SELECT * FROM known_issues WHERE type = '<type_identifié>'
       puis analyse la réponse pour vérifier si ce problème est déjà répertorié et résolu.
    3. Appelle CalculatePriority selon la gravité du ticket.

    Fournis : si le problème est connu (oui/non), la solution trouvée, la priorité.
    """, "Analyste",
    tools: [localTool, .. mcpTools]);

// Agent 3 : Rédacteur — traduit l'analyse en communication client
ChatClientAgent redacteurAgent = new(chatClient, """
    Tu es un expert en communication client.
    À partir de l'analyse fournie, rédige OBLIGATOIREMENT ces deux blocs :

    [CLIENT]
    (réponse claire et rassurante, sans jargon technique, pour Jean Dupont)

    [INTERNE]
    (note technique pour l'équipe support : cause, priorité, solution, actions à faire)
    """, "Redacteur");


var workflowAgent = AgentWorkflowBuilder.BuildSequential(
    triageAgent,
    analysteAgent,
    redacteurAgent
).AsAIAgent();

var ticket = """
    De : jean.dupont@acme.fr
    Objet : Impossible de me connecter depuis ce matin
 
    Salut,
    Je n'arrive plus à accéder à mon dashboard.
    J'obtiens une erreur "403". Mon ID client est ACME-00421. C'est urgent, j'ai une démo client dans 2 heures !
    """;

var response = await workflowAgent.RunAsync(ticket);
Console.WriteLine("\n ********* Réponse finale au client :");
Console.WriteLine(response);

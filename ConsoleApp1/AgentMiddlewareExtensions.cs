using Microsoft.Agents.AI;

namespace ConsoleApp1;

public static class AgentMiddlewareExtensions
{
    public static AIAgent WithLogging(this AIAgent agent, string name) =>
        new AIAgentBuilder(agent)
            .Use(
                async (messages, session, options, innerAgent, ct) =>
                {
                    Console.WriteLine($"[{name}] ? Exécution...");
                    var result = await innerAgent.RunAsync(messages, session, options, ct);
                    Console.WriteLine($"[{name}] ?");
                    return result;
                },
                null!)
            .Build(null!);
}

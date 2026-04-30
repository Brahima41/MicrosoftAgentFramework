using System.ComponentModel;

namespace ConsoleApp1
{
    public static class SupportTools
    {
        [Description("Calcule la priorité d'un ticket selon sa criticité.")]
        public static string CalculatePriority(
            [Description("Niveau de criticité : low, medium, high, critical")] string severity)
        {
            return severity switch
            {
                "critical" => "P0 - Intervention immédiate",
                "high" => "P1 - Traitement sous 4h",
                "medium" => "P2 - Traitement sous 8h",
                _ => "P3 - Traitement sous 24h"
            };
        }
    }
}

using Microsoft.Data.Sqlite;

namespace ConsoleApp1
{
    public static class KnowledgeBaseSeeder
    {
        public static void Seed(string dbPath)
        {
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = """
                CREATE TABLE IF NOT EXISTS known_issues (
                    id          INTEGER PRIMARY KEY AUTOINCREMENT,
                    type        TEXT NOT NULL,
                    error_code  TEXT,
                    description TEXT NOT NULL,
                    solution    TEXT NOT NULL,
                    introduced  TEXT
                )
                """;
                command.ExecuteNonQuery();
            }

            // Idempotent : on ne re-seede pas si des données existent déjà
            using (var countCommand = connection.CreateCommand())
            {
                countCommand.CommandText = "SELECT COUNT(*) FROM known_issues";
                var result = countCommand.ExecuteScalar();
                var count = result != null ? (long)result : 0;
                if (count > 0) return;
            }

            var entries = new[]
            {
                new {
                    Type        = "403",
                    ErrorCode   = "AUTH_TOKEN_EXPIRED",
                    Description = "Erreur 403 Forbidden après la mise à jour du 10/03/2026. " +
                                  "Les tokens OAuth expirent prématurément (bug #4521).",
                    Solution    = "Se déconnecter et vider le cache navigateur. " +
                                  "Si persistant : POST /auth/refresh. Fix en v2.4.1.",
                    Introduced  = "2026-03-10"
                },
                new {
                    Type        = "performance",
                    ErrorCode   = "DASHBOARD_SLOW",
                    Description = "Lenteurs dashboard pour comptes > 500 entrées. " +
                                  "Requête N+1 sur /metrics (ticket #4389).",
                    Solution    = "Contournement : activer 'Vue allégée' dans les paramètres.",
                    Introduced  = "2026-02-01"
                },
                new {
                    Type        = "billing",
                    ErrorCode   = "INVOICE_MISMATCH",
                    Description = "Écart de facturation sur changement de plan en cours de mois.",
                    Solution    = "Remboursement manuel via backoffice. Contacter l'équipe finance.",
                    Introduced  = "2026-01-15"
                }
            };

            foreach (var entry in entries)
            {
                using (var insertCommand = connection.CreateCommand())
                {
                    insertCommand.CommandText = """
                    INSERT INTO known_issues (type, error_code, description, solution, introduced)
                    VALUES (@Type, @ErrorCode, @Description, @Solution, @Introduced)
                    """;
                    insertCommand.Parameters.AddWithValue("@Type", entry.Type);
                    insertCommand.Parameters.AddWithValue("@ErrorCode", entry.ErrorCode ?? (object)DBNull.Value);
                    insertCommand.Parameters.AddWithValue("@Description", entry.Description);
                    insertCommand.Parameters.AddWithValue("@Solution", entry.Solution);
                    insertCommand.Parameters.AddWithValue("@Introduced", entry.Introduced);
                    insertCommand.ExecuteNonQuery();
                }
            }

            Console.WriteLine($"Base de connaissance initialisée ({entries.Length} entrées)");
        }
    }
}

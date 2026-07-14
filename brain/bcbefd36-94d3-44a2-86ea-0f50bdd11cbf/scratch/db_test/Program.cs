using System;
using Npgsql;

namespace db_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string password = "MiFer2121092001";
            string refId = "bajbvebkmacdnllnxvkv";
            string connStr = $"Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.{refId};Password={password};SSL Mode=Require;Trust Server Certificate=true;Timeout=15;";

            using (var conn = new NpgsqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    Console.WriteLine("Listing all users with detailed fields:");
                    using (var cmd = new NpgsqlCommand("SELECT \"Email\", \"Activo\", \"EmailConfirmed\", \"LockoutEnabled\", \"AccessFailedCount\" FROM \"AspNetUsers\";", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($" - Email: {reader.GetValue(0)} | Activo: {reader.GetValue(1)} | EmailConfirmed: {reader.GetValue(2)} | LockoutEnabled: {reader.GetValue(3)} | AccessFailed: {reader.GetValue(4)}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}

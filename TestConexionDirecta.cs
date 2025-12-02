using System;
using Npgsql;

namespace SistemaBanco
{
    public class TestConexionDirecta
    {
        public static void ProbarConexionDirecta()
        {
            Console.WriteLine("=== PRUEBA DE CONEXIÓN DIRECTA ===\n");

            // Probar con la cadena de conexión directa
            string connectionString = "Host=db.ovfaxfhvcjrvujtgiaaf.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ModuloBanco2025;SSL Mode=Require;Trust Server Certificate=true;Timeout=30;";

            Console.WriteLine("Cadena de conexión:");
            Console.WriteLine(connectionString.Replace("Password=ModuloBanco2025", "Password=***"));
            Console.WriteLine();

            try
            {
                Console.WriteLine("Intentando conectar...");
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    Console.WriteLine("✓ Conexión exitosa!");
                    Console.WriteLine($"  Estado: {conn.State}");
                    Console.WriteLine($"  Base de datos: {conn.Database}");
                    Console.WriteLine($"  Servidor: {conn.Host}");
                    Console.WriteLine();

                    // Probar una consulta simple
                    Console.WriteLine("Probando consulta...");
                    using (var cmd = new NpgsqlCommand("SELECT version()", conn))
                    {
                        var version = cmd.ExecuteScalar();
                        Console.WriteLine($"✓ Versión PostgreSQL: {version}");
                    }

                    // Probar consulta de usuarios
                    Console.WriteLine("\nProbando consulta de usuarios...");
                    using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM usuarios", conn))
                    {
                        var count = cmd.ExecuteScalar();
                        Console.WriteLine($"✓ Total de usuarios: {count}");
                    }

                    Console.WriteLine("\n=== CONEXIÓN EXITOSA ===");
                }
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine($"\n✗ ERROR DE POSTGRESQL:");
                Console.WriteLine($"  Mensaje: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Detalle: {ex.InnerException.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR GENERAL:");
                Console.WriteLine($"  Tipo: {ex.GetType().Name}");
                Console.WriteLine($"  Mensaje: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"  Detalle: {ex.InnerException.Message}");
                }
                Console.WriteLine($"\n  Stack Trace:");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPrueba completada. Cerrando en 5 segundos...");
            System.Threading.Thread.Sleep(5000);
        }
    }
}

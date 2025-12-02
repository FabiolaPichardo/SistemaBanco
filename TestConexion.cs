using System;
using System.Data;
using Npgsql;

namespace SistemaBanco
{
    public class TestConexion
    {
        public static void ProbarConexion()
        {
            try
            {
                Console.WriteLine("=== PRUEBA DE CONEXIÓN A BASE DE DATOS ===\n");

                // Probar conexión básica
                Console.WriteLine("1. Probando conexión...");
                string query = "SELECT version()";
                DataTable dt = Database.ExecuteQuery(query);
                Console.WriteLine("✓ Conexión exitosa!");
                Console.WriteLine($"   Versión PostgreSQL: {dt.Rows[0][0]}\n");

                // Verificar tabla usuarios
                Console.WriteLine("2. Verificando tabla usuarios...");
                query = "SELECT COUNT(*) FROM usuarios";
                dt = Database.ExecuteQuery(query);
                int totalUsuarios = Convert.ToInt32(dt.Rows[0][0]);
                Console.WriteLine($"✓ Tabla usuarios existe");
                Console.WriteLine($"   Total de usuarios: {totalUsuarios}\n");

                // Listar usuarios
                Console.WriteLine("3. Listando usuarios:");
                query = "SELECT usuario, nombre_completo, email, estatus FROM usuarios";
                dt = Database.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    string usuario = row["usuario"].ToString();
                    string nombre = row["nombre_completo"].ToString();
                    string email = row["email"].ToString();
                    bool estatus = Convert.ToBoolean(row["estatus"]);
                    Console.WriteLine($"   - Usuario: {usuario}");
                    Console.WriteLine($"     Nombre: {nombre}");
                    Console.WriteLine($"     Email: {email}");
                    Console.WriteLine($"     Estatus: {(estatus ? "ACTIVO" : "INACTIVO")}\n");
                }

                // Probar login con admin
                Console.WriteLine("4. Probando login con usuario 'admin'...");
                query = "SELECT id_usuario, contraseña, estatus FROM usuarios WHERE usuario = @user";
                dt = Database.ExecuteQuery(query, new NpgsqlParameter("@user", "admin"));
                
                if (dt.Rows.Count > 0)
                {
                    Console.WriteLine("✓ Usuario 'admin' encontrado");
                    Console.WriteLine($"   ID: {dt.Rows[0]["id_usuario"]}");
                    Console.WriteLine($"   Contraseña: {dt.Rows[0]["contraseña"]}");
                    Console.WriteLine($"   Estatus: {(Convert.ToBoolean(dt.Rows[0]["estatus"]) ? "ACTIVO" : "INACTIVO")}\n");
                }
                else
                {
                    Console.WriteLine("✗ Usuario 'admin' NO encontrado\n");
                }

                // Verificar preguntas de seguridad
                Console.WriteLine("5. Verificando preguntas de seguridad...");
                query = @"SELECT usuario, 
                         CASE WHEN pregunta_seguridad_1 IS NOT NULL THEN 'SI' ELSE 'NO' END as tiene_preguntas
                         FROM usuarios";
                dt = Database.ExecuteQuery(query);
                foreach (DataRow row in dt.Rows)
                {
                    Console.WriteLine($"   - {row["usuario"]}: Preguntas = {row["tiene_preguntas"]}");
                }

                Console.WriteLine("\n=== PRUEBA COMPLETADA ===");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ ERROR: {ex.Message}");
                Console.WriteLine($"\nDetalle completo:\n{ex}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}

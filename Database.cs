using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace SistemaBanco
{
    public class Database
    {
        private static string connectionString;

        static Database()
        {
            try
            {
                connectionString = ConfigurationManager.ConnectionStrings["BancoDB"]?.ConnectionString;
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("No se encontró la cadena de conexión 'BancoDB' en App.config");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cargar la configuración de base de datos: {ex.Message}");
            }
        }

        public static DataTable ExecuteQuery(string query, params NpgsqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Mensajes de error más específicos
                string errorMsg = "Error de base de datos";
                
                if (ex.Message.Contains("Host") || ex.Message.Contains("host"))
                {
                    errorMsg = "No se puede conectar al servidor de base de datos.\n\nVerifique:\n1. Que tenga conexión a Internet\n2. Que la configuración en App.config sea correcta\n3. Que el servidor de Supabase esté disponible";
                }
                else if (ex.Message.Contains("authentication") || ex.Message.Contains("password"))
                {
                    errorMsg = "Error de autenticación con la base de datos.\n\nVerifique el usuario y contraseña en App.config";
                }
                else if (ex.Message.Contains("column") || ex.Message.Contains("columna"))
                {
                    errorMsg = $"Error en la estructura de la base de datos.\n\nEjecute el script EJECUTAR_PRIMERO.sql en Supabase.\n\nDetalle: {ex.Message}";
                }
                else
                {
                    errorMsg = $"Error de base de datos: {ex.Message}";
                }
                
                throw new Exception(errorMsg);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar consulta: {ex.Message}");
            }
            return dt;
        }

        public static int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                string errorMsg = "Error de base de datos";
                
                if (ex.Message.Contains("Host") || ex.Message.Contains("host"))
                {
                    errorMsg = "No se puede conectar al servidor de base de datos.\n\nVerifique su conexión a Internet y la configuración en App.config";
                }
                else if (ex.Message.Contains("duplicate") || ex.Message.Contains("duplicado"))
                {
                    errorMsg = "El registro ya existe en la base de datos";
                }
                else
                {
                    errorMsg = $"Error de base de datos: {ex.Message}";
                }
                
                throw new Exception(errorMsg);
            }
        }

        public static object ExecuteScalar(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                string errorMsg = "Error de base de datos";
                
                if (ex.Message.Contains("Host") || ex.Message.Contains("host"))
                {
                    errorMsg = "No se puede conectar al servidor de base de datos.\n\nVerifique su conexión a Internet y la configuración en App.config";
                }
                else
                {
                    errorMsg = $"Error de base de datos: {ex.Message}";
                }
                
                throw new Exception(errorMsg);
            }
        }
    }
}

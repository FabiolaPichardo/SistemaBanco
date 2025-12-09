using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace SistemaBanco
{
    /// <summary>
    /// Clase estática que gestiona todas las operaciones de conexión y consultas a la base de datos PostgreSQL.
    /// Proporciona métodos para ejecutar consultas SELECT, INSERT, UPDATE y DELETE de forma segura.
    /// </summary>
    public class Database
    {
        // Cadena de conexión a la base de datos, cargada desde App.config
        private static string connectionString;

        /// <summary>
        /// Constructor estático que se ejecuta una sola vez al cargar la clase.
        /// Inicializa la cadena de conexión desde el archivo de configuración.
        /// </summary>
        static Database()
        {
            try
            {
                // Obtener la cadena de conexión desde App.config
                connectionString = ConfigurationManager.ConnectionStrings["BancoDB"]?.ConnectionString;
                
                // Validar que la cadena de conexión exista
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

        /// <summary>
        /// Ejecuta una consulta SELECT y devuelve los resultados en un DataTable.
        /// Utilizado para obtener datos de la base de datos.
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar (SELECT)</param>
        /// <param name="parameters">Parámetros opcionales para la consulta (previene SQL Injection)</param>
        /// <returns>DataTable con los resultados de la consulta</returns>
        public static DataTable ExecuteQuery(string query, params NpgsqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                // Crear conexión a la base de datos
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Crear comando SQL
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Agregar parámetros si existen (seguridad contra SQL Injection)
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        
                        // Ejecutar consulta y llenar DataTable
                        using (var adapter = new NpgsqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Manejo de errores específicos de PostgreSQL
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
                    errorMsg = $"Error en la estructura de la base de datos.\n\nEjecute el script setup_completo.sql.\n\nDetalle: {ex.Message}";
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

        /// <summary>
        /// Ejecuta una consulta que no devuelve resultados (INSERT, UPDATE, DELETE).
        /// Utilizado para modificar datos en la base de datos.
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar (INSERT, UPDATE, DELETE)</param>
        /// <param name="parameters">Parámetros opcionales para la consulta (previene SQL Injection)</param>
        /// <returns>Número de filas afectadas por la consulta</returns>
        public static int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                // Crear conexión a la base de datos
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Crear y ejecutar comando SQL
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Agregar parámetros si existen
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        
                        // Ejecutar y devolver número de filas afectadas
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Manejo de errores específicos
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

        /// <summary>
        /// Ejecuta una consulta y devuelve un único valor (primera columna de la primera fila).
        /// Utilizado para consultas como COUNT, SUM, MAX, o para obtener un solo dato.
        /// </summary>
        /// <param name="query">Consulta SQL a ejecutar</param>
        /// <param name="parameters">Parámetros opcionales para la consulta</param>
        /// <returns>Valor único obtenido de la consulta (puede ser null)</returns>
        public static object ExecuteScalar(string query, params NpgsqlParameter[] parameters)
        {
            try
            {
                // Crear conexión a la base de datos
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    
                    // Crear y ejecutar comando SQL
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        // Agregar parámetros si existen
                        if (parameters != null) cmd.Parameters.AddRange(parameters);
                        
                        // Ejecutar y devolver el primer valor
                        return cmd.ExecuteScalar();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                // Manejo de errores
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

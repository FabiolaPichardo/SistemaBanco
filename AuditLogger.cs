using System;
using System.IO;
using System.Text.Json;
using Npgsql;

namespace SistemaBanco
{
    /// <summary>
    /// Clase estática que gestiona el sistema de auditoría del sistema bancario.
    /// Registra todas las acciones importantes en base de datos y archivos de log.
    /// Proporciona trazabilidad completa de operaciones para cumplimiento normativo.
    /// </summary>
    public static class AuditLogger
    {
        // Directorio donde se almacenan los archivos de log
        private static readonly string LogDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

        /// <summary>
        /// Enumeración de acciones auditables en el sistema.
        /// </summary>
        public enum AuditAction
        {
            Login,
            LoginFailed,
            Logout,
            ConsultaSaldo,
            RegistroMovimiento,
            EdicionMovimiento,
            EliminacionMovimiento,
            Transferencia,
            AutorizacionMovimiento,
            RechazoMovimiento,
            CambioConfiguracion,
            ConsultaHistorial,
            ExportacionDatos,
            CreacionUsuario,
            EdicionUsuario,
            EliminacionUsuario
        }

        /// <summary>
        /// Niveles de severidad para los logs.
        /// </summary>
        public enum LogLevel
        {
            INFO,
            WARNING,
            ERROR,
            CRITICAL
        }

        /// <summary>
        /// Constructor estático que crea el directorio de logs si no existe.
        /// </summary>
        static AuditLogger()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        /// <summary>
        /// Registra una acción en el sistema de auditoría.
        /// Guarda el registro tanto en base de datos como en archivos de log.
        /// </summary>
        /// <param name="action">Tipo de acción realizada</param>
        /// <param name="detalles">Detalles adicionales de la acción</param>
        /// <param name="nivel">Nivel de severidad del log</param>
        /// <param name="tipoMovimiento">Tipo de movimiento si aplica</param>
        /// <param name="ipAddress">Dirección IP del usuario</param>
        /// <param name="nombreEquipo">Nombre del equipo desde donde se realizó la acción</param>
        public static void Log(
            AuditAction action,
            string detalles = "",
            LogLevel nivel = LogLevel.INFO,
            string tipoMovimiento = "",
            string ipAddress = "",
            string nombreEquipo = "")
        {
            try
            {

                string usuario = FormLogin.UsuarioActual ?? "SYSTEM";
                string email = ObtenerEmailUsuario(usuario);

                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = ObtenerIPLocal();
                }

                if (string.IsNullOrEmpty(nombreEquipo))
                {
                    nombreEquipo = Environment.MachineName;
                }

                DateTime timestamp = DateTime.Now;

                RegistrarEnBD(usuario, email, action.ToString(), detalles, timestamp, 
                             ipAddress, nombreEquipo, tipoMovimiento);

                RegistrarEnArchivo(usuario, email, action.ToString(), detalles, timestamp, 
                                 ipAddress, nombreEquipo, tipoMovimiento, nivel);
            }
            catch (Exception ex)
            {

                LogEmergencia($"Error en AuditLogger: {ex.Message}");
            }
        }

        private static void RegistrarEnBD(string usuario, string email, string accion, 
            string detalles, DateTime timestamp, string ip, string equipo, string tipoMovimiento)
        {
            try
            {
                string query = @"
                    INSERT INTO auditoria_sistema 
                    (usuario, email, accion, detalles, fecha_hora, ip_address, nombre_equipo, tipo_movimiento)
                    VALUES (@usuario, @email, @accion, @detalles, @timestamp, @ip, @equipo, @tipo)";

                Database.ExecuteNonQuery(query,
                    new NpgsqlParameter("@usuario", usuario),
                    new NpgsqlParameter("@email", email ?? ""),
                    new NpgsqlParameter("@accion", accion),
                    new NpgsqlParameter("@detalles", detalles ?? ""),
                    new NpgsqlParameter("@timestamp", timestamp),
                    new NpgsqlParameter("@ip", ip),
                    new NpgsqlParameter("@equipo", equipo),
                    new NpgsqlParameter("@tipo", tipoMovimiento ?? ""));
            }
            catch (Exception ex)
            {
                LogEmergencia($"Error al registrar en BD: {ex.Message}");
            }
        }

        private static void RegistrarEnArchivo(string usuario, string email, string accion,
            string detalles, DateTime timestamp, string ip, string equipo, string tipoMovimiento, LogLevel nivel)
        {
            try
            {
                string fecha = timestamp.ToString("yyyy-MM-dd");
                string appLogFile = Path.Combine(LogDirectory, $"app-{fecha}.log");
                string dbLogFile = Path.Combine(LogDirectory, $"db-{fecha}.log");

                string logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss.fff} | {nivel} | AuditLogger | {accion} | " +
                                $"usuario={usuario} email={email} ip={ip} equipo={equipo} tipo={tipoMovimiento} detalles={detalles}";

                var jsonEntry = new
                {
                    timestamp = timestamp.ToString("o"),
                    nivel = nivel.ToString(),
                    componente = "AuditLogger",
                    accion,
                    usuario,
                    email,
                    ip,
                    equipo,
                    tipo_movimiento = tipoMovimiento,
                    detalles
                };
                string jsonLine = JsonSerializer.Serialize(jsonEntry);

                File.AppendAllText(appLogFile, logEntry + Environment.NewLine);
                File.AppendAllText(appLogFile, jsonLine + Environment.NewLine);

                if (accion.Contains("Movimiento") || accion.Contains("Transferencia"))
                {
                    File.AppendAllText(dbLogFile, logEntry + Environment.NewLine);
                    File.AppendAllText(dbLogFile, jsonLine + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                LogEmergencia($"Error al escribir archivo: {ex.Message}");
            }
        }

        private static string ObtenerEmailUsuario(string usuario)
        {
            try
            {
                string query = "SELECT email FROM usuarios WHERE usuario = @usuario";
                var result = Database.ExecuteScalar(query, new NpgsqlParameter("@usuario", usuario));
                return result?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static string ObtenerIPLocal()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "127.0.0.1";
            }
            catch
            {
                return "127.0.0.1";
            }
        }

        private static void LogEmergencia(string mensaje)
        {
            try
            {
                string emergencyLog = Path.Combine(LogDirectory, "emergency.log");
                string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | EMERGENCY | {mensaje}{Environment.NewLine}";
                File.AppendAllText(emergencyLog, entry);
            }
            catch
            {

                Console.WriteLine($"EMERGENCY LOG: {mensaje}");
            }
        }

        /// <summary>
        /// Elimina logs antiguos tanto de archivos como de base de datos.
        /// Útil para cumplir con políticas de retención de datos.
        /// </summary>
        /// <param name="diasRetencion">Número de días a retener los logs (por defecto 90)</param>
        public static void LimpiarLogsAntiguos(int diasRetencion = 90)
        {
            try
            {
                DateTime fechaLimite = DateTime.Now.AddDays(-diasRetencion);

                // Eliminar archivos de log antiguos
                var archivos = Directory.GetFiles(LogDirectory, "*.log");
                foreach (var archivo in archivos)
                {
                    var info = new FileInfo(archivo);
                    if (info.CreationTime < fechaLimite)
                    {
                        File.Delete(archivo);
                    }
                }

                // Eliminar registros antiguos de la base de datos
                string query = "DELETE FROM auditoria_sistema WHERE fecha_hora < @fecha";
                Database.ExecuteNonQuery(query, new NpgsqlParameter("@fecha", fechaLimite));
            }
            catch (Exception ex)
            {
                LogEmergencia($"Error al limpiar logs antiguos: {ex.Message}");
            }
        }
    }
}

using System;
using System.Data;
using Npgsql;

namespace SistemaBanco
{

    public static class SuspiciousActivityDetector
    {

        private static decimal MontoAtipicoMinimo = 50000m; // $50,000 MXN
        private static int TransaccionesRepetitivasMax = 5; // 5 transacciones en 1 hora
        private static decimal DesviacionPorcentual = 200m; // 200% del promedio histórico

        public enum EstadoAlerta
        {
            Abierta,
            EnRevision,
            Escalada,
            Cerrada
        }

        public static void AnalizarMovimiento(int idMovimiento, string tipoMovimiento, 
            decimal importe, int idUsuario, string cuentaOrdenante)
        {
            try
            {
                bool esSospechoso = false;
                string razon = "";

                if (importe >= MontoAtipicoMinimo)
                {
                    esSospechoso = true;
                    razon += $"Monto atípico (${importe:N2} >= ${MontoAtipicoMinimo:N2}). ";
                }

                int transaccionesRecientes = ContarTransaccionesRecientes(idUsuario, cuentaOrdenante);
                if (transaccionesRecientes >= TransaccionesRepetitivasMax)
                {
                    esSospechoso = true;
                    razon += $"Transacciones repetitivas ({transaccionesRecientes} en 1 hora). ";
                }

                decimal promedioHistorico = ObtenerPromedioHistorico(idUsuario);
                if (promedioHistorico > 0 && importe > (promedioHistorico * (DesviacionPorcentual / 100)))
                {
                    esSospechoso = true;
                    razon += $"Desviación del perfil histórico ({(importe / promedioHistorico * 100):N0}% del promedio). ";
                }

                if (esSospechoso)
                {
                    GenerarAlerta(idMovimiento, tipoMovimiento, importe, idUsuario, razon);
                    NotificarFinanzas(idMovimiento, razon);
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion, 
                    $"Error al analizar movimiento: {ex.Message}", 
                    AuditLogger.LogLevel.ERROR);
            }
        }

        private static void GenerarAlerta(int idMovimiento, string tipoMovimiento, 
            decimal importe, int idUsuario, string razon)
        {
            try
            {
                string query = @"
                    INSERT INTO alertas_movimientos_sospechosos 
                    (id_movimiento, tipo_movimiento, importe, id_usuario, razon, estado, 
                     fecha_generacion, notificado_finanzas)
                    VALUES (@idMov, @tipo, @importe, @idUser, @razon, @estado, @fecha, TRUE)
                    RETURNING id_alerta";

                var result = Database.ExecuteScalar(query,
                    new NpgsqlParameter("@idMov", idMovimiento),
                    new NpgsqlParameter("@tipo", tipoMovimiento),
                    new NpgsqlParameter("@importe", importe),
                    new NpgsqlParameter("@idUser", idUsuario),
                    new NpgsqlParameter("@razon", razon),
                    new NpgsqlParameter("@estado", EstadoAlerta.Abierta.ToString()),
                    new NpgsqlParameter("@fecha", DateTime.Now));

                int idAlerta = Convert.ToInt32(result);

                AuditLogger.Log(AuditLogger.AuditAction.RegistroMovimiento,
                    $"Alerta generada ID: {idAlerta} - Movimiento: {idMovimiento} - Razón: {razon}",
                    AuditLogger.LogLevel.WARNING,
                    tipoMovimiento);
            }
            catch (Exception ex)
            {
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"Error al generar alerta: {ex.Message}",
                    AuditLogger.LogLevel.ERROR);
            }
        }

        private static void NotificarFinanzas(int idMovimiento, string razon)
        {
            try
            {

                string query = @"
                    SELECT email FROM usuarios 
                    WHERE rol IN ('Gerente', 'Administrador') 
                    AND estatus = TRUE";

                DataTable dt = Database.ExecuteQuery(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string email = row["email"].ToString();

                        string asunto = $"⚠️ ALERTA: Movimiento Sospechoso Detectado - ID {idMovimiento}";
                        string cuerpo = $@"
                            <h2>Alerta de Movimiento Sospechoso</h2>
                            <p><strong>ID Movimiento:</strong> {idMovimiento}</p>
                            <p><strong>Razón:</strong> {razon}</p>
                            <p><strong>Fecha:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
                            <p><strong>Sistema:</strong> Módulo Bancario</p>
                            <hr>
                            <p>Por favor, revise este movimiento en el sistema para su análisis.</p>
                            <p><em>Este es un mensaje automático del sistema de detección de fraudes.</em></p>
                        ";

                        EmailService.EnviarCorreo(email, asunto, cuerpo);
                    }
                }
            }
            catch (Exception ex)
            {
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"Error al notificar finanzas: {ex.Message}",
                    AuditLogger.LogLevel.ERROR);
            }
        }

        private static int ContarTransaccionesRecientes(int idUsuario, string cuenta)
        {
            try
            {
                DateTime hace1Hora = DateTime.Now.AddHours(-1);

                string query = @"
                    SELECT COUNT(*) 
                    FROM movimientos_financieros 
                    WHERE id_usuario = @idUser 
                    AND cuenta_ordenante = @cuenta 
                    AND fecha_hora >= @fecha";

                var result = Database.ExecuteScalar(query,
                    new NpgsqlParameter("@idUser", idUsuario),
                    new NpgsqlParameter("@cuenta", cuenta),
                    new NpgsqlParameter("@fecha", hace1Hora));

                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        private static decimal ObtenerPromedioHistorico(int idUsuario)
        {
            try
            {
                DateTime hace30Dias = DateTime.Now.AddDays(-30);

                string query = @"
                    SELECT AVG(importe) 
                    FROM movimientos_financieros 
                    WHERE id_usuario = @idUser 
                    AND fecha_hora >= @fecha";

                var result = Database.ExecuteScalar(query,
                    new NpgsqlParameter("@idUser", idUsuario),
                    new NpgsqlParameter("@fecha", hace30Dias));

                return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        public static void ActualizarEstadoAlerta(int idAlerta, EstadoAlerta nuevoEstado, 
            string comentarios = "", bool esFalsoPositivo = false)
        {
            try
            {
                string query = @"
                    UPDATE alertas_movimientos_sospechosos 
                    SET estado = @estado,
                        fecha_actualizacion = @fecha,
                        usuario_actualizacion = @usuario,
                        comentarios = @comentarios,
                        es_falso_positivo = @falsoPositivo
                    WHERE id_alerta = @id";

                Database.ExecuteNonQuery(query,
                    new NpgsqlParameter("@estado", nuevoEstado.ToString()),
                    new NpgsqlParameter("@fecha", DateTime.Now),
                    new NpgsqlParameter("@usuario", FormLogin.UsuarioActual ?? "SYSTEM"),
                    new NpgsqlParameter("@comentarios", comentarios),
                    new NpgsqlParameter("@falsoPositivo", esFalsoPositivo),
                    new NpgsqlParameter("@id", idAlerta));

                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"Alerta {idAlerta} actualizada a {nuevoEstado}. Comentarios: {comentarios}",
                    AuditLogger.LogLevel.INFO);
            }
            catch (Exception ex)
            {
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"Error al actualizar alerta: {ex.Message}",
                    AuditLogger.LogLevel.ERROR);
            }
        }

        public static void EstablecerFechaExpiracion(int idAlerta, DateTime fechaExpiracion)
        {
            try
            {
                string query = @"
                    UPDATE alertas_movimientos_sospechosos 
                    SET fecha_expiracion = @fecha
                    WHERE id_alerta = @id";

                Database.ExecuteNonQuery(query,
                    new NpgsqlParameter("@fecha", fechaExpiracion),
                    new NpgsqlParameter("@id", idAlerta));
            }
            catch (Exception ex)
            {
                AuditLogger.Log(AuditLogger.AuditAction.CambioConfiguracion,
                    $"Error al establecer fecha de expiración: {ex.Message}",
                    AuditLogger.LogLevel.ERROR);
            }
        }
    }
}

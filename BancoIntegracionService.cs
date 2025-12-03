using System;
using System.Data;
using System.Collections.Generic;
using Npgsql;

namespace SistemaBanco
{
    /// <summary>
    /// Servicio de Integración del Módulo Banco
    /// Actúa como núcleo central para exponer información financiera a otros módulos (ERP, CRM, Proveedores)
    /// </summary>
    public class BancoIntegracionService
    {
        #region Singleton Pattern
        private static BancoIntegracionService _instance;
        private static readonly object _lock = new object();

        public static BancoIntegracionService Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new BancoIntegracionService();
                        }
                    }
                }
                return _instance;
            }
        }

        private BancoIntegracionService() { }
        #endregion

        #region Consulta de Saldos en Tiempo Real

        /// <summary>
        /// Obtiene el saldo actual de una cuenta específica
        /// </summary>
        public SaldoResponse ObtenerSaldo(int idUsuario)
        {
            try
            {
                string query = @"
                    SELECT 
                        u.id_usuario,
                        u.nombre_usuario,
                        u.nombre_completo,
                        c.numero_cuenta,
                        c.tipo_cuenta,
                        c.saldo,
                        c.fecha_apertura,
                        c.estado,
                        c.fecha_ultima_actualizacion
                    FROM usuarios u
                    INNER JOIN cuentas c ON u.id_usuario = c.id_usuario
                    WHERE u.id_usuario = @idUsuario";

                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@idUsuario", idUsuario));

                if (dt.Rows.Count == 0)
                    return new SaldoResponse { Exito = false, Mensaje = "Usuario o cuenta no encontrada" };

                DataRow row = dt.Rows[0];

                return new SaldoResponse
                {
                    Exito = true,
                    IdUsuario = Convert.ToInt32(row["id_usuario"]),
                    NombreUsuario = row["nombre_usuario"].ToString(),
                    NombreCompleto = row["nombre_completo"].ToString(),
                    NumeroCuenta = row["numero_cuenta"].ToString(),
                    TipoCuenta = row["tipo_cuenta"].ToString(),
                    Saldo = Convert.ToDecimal(row["saldo"]),
                    Estado = row["estado"].ToString(),
                    FechaUltimaActualizacion = Convert.ToDateTime(row["fecha_ultima_actualizacion"]),
                    Mensaje = "Saldo obtenido exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new SaldoResponse
                {
                    Exito = false,
                    Mensaje = $"Error al obtener saldo: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtiene saldos de múltiples cuentas
        /// </summary>
        public List<SaldoResponse> ObtenerSaldosMultiples(List<int> idsUsuarios)
        {
            var saldos = new List<SaldoResponse>();
            foreach (int idUsuario in idsUsuarios)
            {
                saldos.Add(ObtenerSaldo(idUsuario));
            }
            return saldos;
        }

        #endregion

        #region Consulta de Movimientos

        /// <summary>
        /// Obtiene movimientos de una cuenta en un rango de fechas
        /// </summary>
        public MovimientosResponse ObtenerMovimientos(int idUsuario, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                string query = @"
                    SELECT 
                        m.id_movimiento,
                        m.tipo_movimiento,
                        m.monto,
                        m.concepto,
                        m.fecha_movimiento,
                        m.saldo_anterior,
                        m.saldo_nuevo,
                        u.nombre_completo,
                        c.numero_cuenta
                    FROM movimientos m
                    INNER JOIN usuarios u ON m.id_usuario = u.id_usuario
                    INNER JOIN cuentas c ON u.id_usuario = c.id_usuario
                    WHERE m.id_usuario = @idUsuario
                    AND m.fecha_movimiento BETWEEN @fechaInicio AND @fechaFin
                    ORDER BY m.fecha_movimiento DESC";

                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@idUsuario", idUsuario),
                    new NpgsqlParameter("@fechaInicio", fechaInicio),
                    new NpgsqlParameter("@fechaFin", fechaFin));

                var movimientos = new List<MovimientoDetalle>();
                foreach (DataRow row in dt.Rows)
                {
                    movimientos.Add(new MovimientoDetalle
                    {
                        IdMovimiento = Convert.ToInt32(row["id_movimiento"]),
                        TipoMovimiento = row["tipo_movimiento"].ToString(),
                        Monto = Convert.ToDecimal(row["monto"]),
                        Concepto = row["concepto"].ToString(),
                        FechaMovimiento = Convert.ToDateTime(row["fecha_movimiento"]),
                        SaldoAnterior = Convert.ToDecimal(row["saldo_anterior"]),
                        SaldoNuevo = Convert.ToDecimal(row["saldo_nuevo"])
                    });
                }

                return new MovimientosResponse
                {
                    Exito = true,
                    IdUsuario = idUsuario,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    CantidadMovimientos = movimientos.Count,
                    Movimientos = movimientos,
                    Mensaje = "Movimientos obtenidos exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new MovimientosResponse
                {
                    Exito = false,
                    Mensaje = $"Error al obtener movimientos: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Obtiene el último movimiento de una cuenta
        /// </summary>
        public MovimientoDetalle ObtenerUltimoMovimiento(int idUsuario)
        {
            try
            {
                string query = @"
                    SELECT 
                        m.id_movimiento,
                        m.tipo_movimiento,
                        m.monto,
                        m.concepto,
                        m.fecha_movimiento,
                        m.saldo_anterior,
                        m.saldo_nuevo
                    FROM movimientos m
                    WHERE m.id_usuario = @idUsuario
                    ORDER BY m.fecha_movimiento DESC
                    LIMIT 1";

                DataTable dt = Database.ExecuteQuery(query, new NpgsqlParameter("@idUsuario", idUsuario));

                if (dt.Rows.Count == 0)
                    return null;

                DataRow row = dt.Rows[0];
                return new MovimientoDetalle
                {
                    IdMovimiento = Convert.ToInt32(row["id_movimiento"]),
                    TipoMovimiento = row["tipo_movimiento"].ToString(),
                    Monto = Convert.ToDecimal(row["monto"]),
                    Concepto = row["concepto"].ToString(),
                    FechaMovimiento = Convert.ToDateTime(row["fecha_movimiento"]),
                    SaldoAnterior = Convert.ToDecimal(row["saldo_anterior"]),
                    SaldoNuevo = Convert.ToDecimal(row["saldo_nuevo"])
                };
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Registro de Operaciones (Para ERP/CRM/Proveedores)

        /// <summary>
        /// Registra un cargo en la cuenta (usado por módulos externos)
        /// </summary>
        public OperacionResponse RegistrarCargo(int idUsuario, decimal monto, string concepto, string moduloOrigen)
        {
            try
            {
                // Validar saldo suficiente
                var saldoActual = ObtenerSaldo(idUsuario);
                if (!saldoActual.Exito)
                    return new OperacionResponse { Exito = false, Mensaje = "No se pudo obtener el saldo actual" };

                if (saldoActual.Saldo < monto)
                    return new OperacionResponse { Exito = false, Mensaje = "Saldo insuficiente" };

                // Registrar movimiento
                string query = @"
                    INSERT INTO movimientos (id_usuario, tipo_movimiento, monto, concepto, saldo_anterior, saldo_nuevo)
                    VALUES (@idUsuario, 'Cargo', @monto, @concepto, @saldoAnterior, @saldoNuevo)
                    RETURNING id_movimiento";

                decimal nuevoSaldo = saldoActual.Saldo - monto;

                var parametros = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("@idUsuario", idUsuario),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", $"[{moduloOrigen}] {concepto}"),
                    new NpgsqlParameter("@saldoAnterior", saldoActual.Saldo),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldo)
                };

                DataTable dt = Database.ExecuteQuery(query, parametros);
                int idMovimiento = Convert.ToInt32(dt.Rows[0]["id_movimiento"]);

                // Actualizar saldo en cuenta
                string queryUpdate = "UPDATE cuentas SET saldo = @nuevoSaldo, fecha_ultima_actualizacion = CURRENT_TIMESTAMP WHERE id_usuario = @idUsuario";
                Database.ExecuteNonQuery(queryUpdate,
                    new NpgsqlParameter("@nuevoSaldo", nuevoSaldo),
                    new NpgsqlParameter("@idUsuario", idUsuario));

                // Registrar en auditoría
                AuditLogger.Log(AuditLogger.AuditAction.RegistroMovimiento,
                    $"INTEGRACION_CARGO - Módulo: {moduloOrigen}, Usuario: {idUsuario}, Monto: {monto:C2}, ID Movimiento: {idMovimiento}");

                return new OperacionResponse
                {
                    Exito = true,
                    IdMovimiento = idMovimiento,
                    SaldoAnterior = saldoActual.Saldo,
                    SaldoNuevo = nuevoSaldo,
                    Mensaje = "Cargo registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = $"Error al registrar cargo: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Registra un abono en la cuenta (usado por módulos externos)
        /// </summary>
        public OperacionResponse RegistrarAbono(int idUsuario, decimal monto, string concepto, string moduloOrigen)
        {
            try
            {
                var saldoActual = ObtenerSaldo(idUsuario);
                if (!saldoActual.Exito)
                    return new OperacionResponse { Exito = false, Mensaje = "No se pudo obtener el saldo actual" };

                // Registrar movimiento
                string query = @"
                    INSERT INTO movimientos (id_usuario, tipo_movimiento, monto, concepto, saldo_anterior, saldo_nuevo)
                    VALUES (@idUsuario, 'Abono', @monto, @concepto, @saldoAnterior, @saldoNuevo)
                    RETURNING id_movimiento";

                decimal nuevoSaldo = saldoActual.Saldo + monto;

                var parametros = new NpgsqlParameter[]
                {
                    new NpgsqlParameter("@idUsuario", idUsuario),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@concepto", $"[{moduloOrigen}] {concepto}"),
                    new NpgsqlParameter("@saldoAnterior", saldoActual.Saldo),
                    new NpgsqlParameter("@saldoNuevo", nuevoSaldo)
                };

                DataTable dt = Database.ExecuteQuery(query, parametros);
                int idMovimiento = Convert.ToInt32(dt.Rows[0]["id_movimiento"]);

                // Actualizar saldo en cuenta
                string queryUpdate = "UPDATE cuentas SET saldo = @nuevoSaldo, fecha_ultima_actualizacion = CURRENT_TIMESTAMP WHERE id_usuario = @idUsuario";
                Database.ExecuteNonQuery(queryUpdate,
                    new NpgsqlParameter("@nuevoSaldo", nuevoSaldo),
                    new NpgsqlParameter("@idUsuario", idUsuario));

                // Registrar en auditoría
                AuditLogger.Log(AuditLogger.AuditAction.RegistroMovimiento,
                    $"INTEGRACION_ABONO - Módulo: {moduloOrigen}, Usuario: {idUsuario}, Monto: {monto:C2}, ID Movimiento: {idMovimiento}");

                return new OperacionResponse
                {
                    Exito = true,
                    IdMovimiento = idMovimiento,
                    SaldoAnterior = saldoActual.Saldo,
                    SaldoNuevo = nuevoSaldo,
                    Mensaje = "Abono registrado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new OperacionResponse
                {
                    Exito = false,
                    Mensaje = $"Error al registrar abono: {ex.Message}"
                };
            }
        }

        #endregion

        #region Consultas para ERP (Contabilidad)

        /// <summary>
        /// Obtiene resumen contable para el módulo ERP
        /// </summary>
        public ResumenContableResponse ObtenerResumenContable(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                string query = @"
                    SELECT 
                        COUNT(DISTINCT m.id_usuario) as total_cuentas,
                        SUM(CASE WHEN m.tipo_movimiento = 'Cargo' THEN m.monto ELSE 0 END) as total_cargos,
                        SUM(CASE WHEN m.tipo_movimiento = 'Abono' THEN m.monto ELSE 0 END) as total_abonos,
                        COUNT(CASE WHEN m.tipo_movimiento = 'Cargo' THEN 1 END) as cantidad_cargos,
                        COUNT(CASE WHEN m.tipo_movimiento = 'Abono' THEN 1 END) as cantidad_abonos,
                        SUM(c.saldo) as saldo_total_sistema
                    FROM movimientos m
                    CROSS JOIN cuentas c
                    WHERE m.fecha_movimiento BETWEEN @fechaInicio AND @fechaFin";

                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@fechaInicio", fechaInicio),
                    new NpgsqlParameter("@fechaFin", fechaFin));

                DataRow row = dt.Rows[0];

                return new ResumenContableResponse
                {
                    Exito = true,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                    TotalCuentas = row["total_cuentas"] != DBNull.Value ? Convert.ToInt32(row["total_cuentas"]) : 0,
                    TotalCargos = row["total_cargos"] != DBNull.Value ? Convert.ToDecimal(row["total_cargos"]) : 0,
                    TotalAbonos = row["total_abonos"] != DBNull.Value ? Convert.ToDecimal(row["total_abonos"]) : 0,
                    CantidadCargos = row["cantidad_cargos"] != DBNull.Value ? Convert.ToInt32(row["cantidad_cargos"]) : 0,
                    CantidadAbonos = row["cantidad_abonos"] != DBNull.Value ? Convert.ToInt32(row["cantidad_abonos"]) : 0,
                    SaldoTotalSistema = row["saldo_total_sistema"] != DBNull.Value ? Convert.ToDecimal(row["saldo_total_sistema"]) : 0,
                    Mensaje = "Resumen contable generado exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ResumenContableResponse
                {
                    Exito = false,
                    Mensaje = $"Error al generar resumen contable: {ex.Message}"
                };
            }
        }

        #endregion

        #region Consultas para CRM (Información al Cliente)

        /// <summary>
        /// Obtiene información financiera del cliente para el módulo CRM
        /// </summary>
        public ClienteFinancieroResponse ObtenerInformacionCliente(int idUsuario)
        {
            try
            {
                var saldo = ObtenerSaldo(idUsuario);
                if (!saldo.Exito)
                    return new ClienteFinancieroResponse { Exito = false, Mensaje = saldo.Mensaje };

                // Obtener estadísticas de movimientos del último mes
                var movimientos = ObtenerMovimientos(idUsuario, DateTime.Now.AddMonths(-1), DateTime.Now);

                decimal totalCargos = 0;
                decimal totalAbonos = 0;
                int cantidadTransacciones = 0;

                if (movimientos.Exito)
                {
                    foreach (var mov in movimientos.Movimientos)
                    {
                        if (mov.TipoMovimiento == "Cargo")
                            totalCargos += mov.Monto;
                        else if (mov.TipoMovimiento == "Abono")
                            totalAbonos += mov.Monto;
                        cantidadTransacciones++;
                    }
                }

                return new ClienteFinancieroResponse
                {
                    Exito = true,
                    IdUsuario = idUsuario,
                    NombreCompleto = saldo.NombreCompleto,
                    NumeroCuenta = saldo.NumeroCuenta,
                    SaldoActual = saldo.Saldo,
                    TotalCargosUltimoMes = totalCargos,
                    TotalAbonosUltimoMes = totalAbonos,
                    CantidadTransaccionesUltimoMes = cantidadTransacciones,
                    EstadoCuenta = saldo.Estado,
                    FechaUltimaActividad = saldo.FechaUltimaActualizacion,
                    Mensaje = "Información del cliente obtenida exitosamente"
                };
            }
            catch (Exception ex)
            {
                return new ClienteFinancieroResponse
                {
                    Exito = false,
                    Mensaje = $"Error al obtener información del cliente: {ex.Message}"
                };
            }
        }

        #endregion

        #region Consultas para Proveedores (Conciliación de Pagos)

        /// <summary>
        /// Verifica si un pago fue procesado (para conciliación con proveedores)
        /// </summary>
        public VerificacionPagoResponse VerificarPago(int idUsuario, decimal monto, DateTime fechaAproximada, string conceptoBusqueda)
        {
            try
            {
                // Buscar en un rango de ±2 días
                DateTime fechaInicio = fechaAproximada.AddDays(-2);
                DateTime fechaFin = fechaAproximada.AddDays(2);

                string query = @"
                    SELECT 
                        m.id_movimiento,
                        m.tipo_movimiento,
                        m.monto,
                        m.concepto,
                        m.fecha_movimiento
                    FROM movimientos m
                    WHERE m.id_usuario = @idUsuario
                    AND m.monto = @monto
                    AND m.fecha_movimiento BETWEEN @fechaInicio AND @fechaFin
                    AND m.concepto ILIKE @concepto
                    ORDER BY m.fecha_movimiento DESC
                    LIMIT 1";

                DataTable dt = Database.ExecuteQuery(query,
                    new NpgsqlParameter("@idUsuario", idUsuario),
                    new NpgsqlParameter("@monto", monto),
                    new NpgsqlParameter("@fechaInicio", fechaInicio),
                    new NpgsqlParameter("@fechaFin", fechaFin),
                    new NpgsqlParameter("@concepto", $"%{conceptoBusqueda}%"));

                if (dt.Rows.Count == 0)
                {
                    return new VerificacionPagoResponse
                    {
                        Exito = true,
                        PagoEncontrado = false,
                        Mensaje = "No se encontró el pago en el sistema"
                    };
                }

                DataRow row = dt.Rows[0];

                return new VerificacionPagoResponse
                {
                    Exito = true,
                    PagoEncontrado = true,
                    IdMovimiento = Convert.ToInt32(row["id_movimiento"]),
                    TipoMovimiento = row["tipo_movimiento"].ToString(),
                    Monto = Convert.ToDecimal(row["monto"]),
                    Concepto = row["concepto"].ToString(),
                    FechaProcesamiento = Convert.ToDateTime(row["fecha_movimiento"]),
                    Mensaje = "Pago encontrado y verificado"
                };
            }
            catch (Exception ex)
            {
                return new VerificacionPagoResponse
                {
                    Exito = false,
                    Mensaje = $"Error al verificar pago: {ex.Message}"
                };
            }
        }

        #endregion

        #region Notificaciones de Cambios (Para Suscriptores)

        /// <summary>
        /// // Evento que se dispara cdo hay un cambio en saldos o movimientos
        /// Los módulos externos pueden suscribirse a este evento
        /// </summary>
        public event EventHandler<CambioFinancieroEventArgs> CambioFinanciero;

        protected virtual void OnCambioFinanciero(CambioFinancieroEventArgs e)
        {
            CambioFinanciero?.Invoke(this, e);
        }

        /// <summary>
        /// Notifica a los módulos suscritos sobre un cambio financiero
        /// </summary>
        public void NotificarCambio(int idUsuario, string tipoOperacion, decimal monto, string moduloOrigen)
        {
            OnCambioFinanciero(new CambioFinancieroEventArgs
            {
                IdUsuario = idUsuario,
                TipoOperacion = tipoOperacion,
                Monto = monto,
                ModuloOrigen = moduloOrigen,
                FechaHora = DateTime.Now
            });
        }

        #endregion
    }

    #region Clases de Respuesta (DTOs)

    public class SaldoResponse
    {
        public bool Exito { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroCuenta { get; set; }
        public string TipoCuenta { get; set; }
        public decimal Saldo { get; set; }
        public string Estado { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public string Mensaje { get; set; }
    }

    public class MovimientosResponse
    {
        public bool Exito { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int CantidadMovimientos { get; set; }
        public List<MovimientoDetalle> Movimientos { get; set; }
        public string Mensaje { get; set; }
    }

    public class MovimientoDetalle
    {
        public int IdMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Monto { get; set; }
        public string Concepto { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
    }

    public class OperacionResponse
    {
        public bool Exito { get; set; }
        public int IdMovimiento { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal SaldoNuevo { get; set; }
        public string Mensaje { get; set; }
    }

    public class ResumenContableResponse
    {
        public bool Exito { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int TotalCuentas { get; set; }
        public decimal TotalCargos { get; set; }
        public decimal TotalAbonos { get; set; }
        public int CantidadCargos { get; set; }
        public int CantidadAbonos { get; set; }
        public decimal SaldoTotalSistema { get; set; }
        public string Mensaje { get; set; }
    }

    public class ClienteFinancieroResponse
    {
        public bool Exito { get; set; }
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroCuenta { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal TotalCargosUltimoMes { get; set; }
        public decimal TotalAbonosUltimoMes { get; set; }
        public int CantidadTransaccionesUltimoMes { get; set; }
        public string EstadoCuenta { get; set; }
        public DateTime FechaUltimaActividad { get; set; }
        public string Mensaje { get; set; }
    }

    public class VerificacionPagoResponse
    {
        public bool Exito { get; set; }
        public bool PagoEncontrado { get; set; }
        public int IdMovimiento { get; set; }
        public string TipoMovimiento { get; set; }
        public decimal Monto { get; set; }
        public string Concepto { get; set; }
        public DateTime FechaProcesamiento { get; set; }
        public string Mensaje { get; set; }
    }

    public class CambioFinancieroEventArgs : EventArgs
    {
        public int IdUsuario { get; set; }
        public string TipoOperacion { get; set; }
        public decimal Monto { get; set; }
        public string ModuloOrigen { get; set; }
        public DateTime FechaHora { get; set; }
    }

    #endregion
} 
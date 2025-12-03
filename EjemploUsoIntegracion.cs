using System;
using System.Collections.Generic;

namespace SistemaBanco
{
    /// <summary>
    /// Ejemplos prácticos de uso del servicio de integración
    /// Demuestra cómo los módulos ERP, CRM y Proveedores pueden interactuar con el Módulo Banco
    /// </summary>
    public class EjemploUsoIntegracion
    {
        private static BancoIntegracionService banco = BancoIntegracionService.Instance;

        #region Ejemplos para Módulo ERP

        /// <summary>
        /// Ejemplo 1: ERP registra un gasto de nómina
        /// </summary>
        public static void ERP_RegistrarNomina(int idCuentaEmpresa, decimal montoNomina)
        {
            Console.WriteLine("=== ERP: Registrando Nómina ===");

            var resultado = banco.RegistrarCargo(
                idUsuario: idCuentaEmpresa,
                monto: montoNomina,
                concepto: "Pago de nómina quincenal - " + DateTime.Now.ToString("MMM yyyy"),
                moduloOrigen: "ERP"
            );

            if (resultado.Exito)
            {
                Console.WriteLine($"✅ Nómina registrada exitosamente");
                Console.WriteLine($"   ID Movimiento: {resultado.IdMovimiento}");
                Console.WriteLine($"   Saldo anterior: {resultado.SaldoAnterior:C2}");
                Console.WriteLine($"   Saldo nuevo: {resultado.SaldoNuevo:C2}");
                
                // ERP puede actualizar su contabilidad
                ActualizarContabilidadERP(resultado.IdMovimiento, montoNomina);
            }
            else
            {
                Console.WriteLine($"❌ Error: {resultado.Mensaje}");
            }
        }

        /// <summary>
        /// Ejemplo 2: ERP genera reporte contable mensual
        /// </summary>
        public static void ERP_GenerarReporteContable()
        {
            Console.WriteLine("\n=== ERP: Generando Reporte Contable ===");

            DateTime inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime finMes = DateTime.Now;

            var resumen = banco.ObtenerResumenContable(inicioMes, finMes);

            if (resumen.Exito)
            {
                Console.WriteLine($"Período: {resumen.FechaInicio:dd/MM/yyyy} - {resumen.FechaFin:dd/MM/yyyy}");
                Console.WriteLine($"Total cuentas: {resumen.TotalCuentas}");
                Console.WriteLine($"Total cargos: {resumen.TotalCargos:C2} ({resumen.CantidadCargos} ops)");
                Console.WriteLine($"Total abonos: {resumen.TotalAbonos:C2} ({resumen.CantidadAbonos} ops)");
                Console.WriteLine($"Saldo total sistema: {resumen.SaldoTotalSistema:C2}");
                
                decimal flujoNeto = resumen.TotalAbonos - resumen.TotalCargos;
                Console.WriteLine($"Flujo neto: {flujoNeto:C2}");
            }
        }

        private static void ActualizarContabilidadERP(int idMovimiento, decimal monto)
        {
            // Simulación de actualización en ERP
            Console.WriteLine($"   [ERP] Contabilidad actualizada - Asiento: {idMovimiento}");
        }

        #endregion

        #region Ejemplos para Módulo CRM

        /// <summary>
        /// Ejemplo 3: CRM muestra perfil financiero del cliente
        /// </summary>
        public static void CRM_MostrarPerfilCliente(int idCliente)
        {
            Console.WriteLine("\n=== CRM: Perfil Financiero del Cliente ===");

            var info = banco.ObtenerInformacionCliente(idCliente);

            if (info.Exito)
            {
                Console.WriteLine($"Cliente: {info.NombreCompleto}");
                Console.WriteLine($"Cuenta: {info.NumeroCuenta}");
                Console.WriteLine($"Saldo actual: {info.SaldoActual:C2}");
                Console.WriteLine($"Estado: {info.EstadoCuenta}");
                Console.WriteLine("\nActividad último mes:");
                Console.WriteLine($"  Cargos: {info.TotalCargosUltimoMes:C2}");
                Console.WriteLine($"  Abonos: {info.TotalAbonosUltimoMes:C2}");
                Console.WriteLine($"  Transacciones: {info.CantidadTransaccionesUltimoMes}");
                Console.WriteLine($"  Última actividad: {info.FechaUltimaActividad:dd/MM/yyyy HH:mm}");
                
                // CRM puede clasificar al cliente
                ClasificarCliente(info);
            }
        }

        /// <summary>
        /// Ejemplo 4: CRM registra bonificación para cliente VIP
        /// </summary>
        public static void CRM_AplicarBonificacion(int idCliente, decimal montoBonificacion)
        {
            Console.WriteLine("\n=== CRM: Aplicando Bonificación ===");

            var resultado = banco.RegistrarAbono(
                idUsuario: idCliente,
                monto: montoBonificacion,
                concepto: "Bonificación cliente VIP - Programa de lealtad",
                moduloOrigen: "CRM"
            );

            if (resultado.Exito)
            {
                Console.WriteLine($"✅ Bonificación aplicada: {montoBonificacion:C2}");
                Console.WriteLine($"   Nuevo saldo: {resultado.SaldoNuevo:C2}");
                
                // CRM actualiza puntos de lealtad
                ActualizarPuntosLealtad(idCliente, montoBonificacion);
            }
        }

        private static void ClasificarCliente(ClienteFinancieroResponse info)
        {
            string categoria = "Regular";
            if (info.SaldoActual > 50000) categoria = "Premium";
            if (info.SaldoActual > 100000) categoria = "VIP";
            
            Console.WriteLine($"\n[CRM] Categoría del cliente: {categoria}");
        }

        private static void ActualizarPuntosLealtad(int idCliente, decimal monto)
        {
            int puntos = (int)(monto / 100); // 1 punto por cada $100
            Console.WriteLine($"   [CRM] Puntos de lealtad agregados: {puntos}");
        }

        #endregion

        #region Ejemplos para Módulo Proveedores

        /// <summary>
        /// Ejemplo 5: Proveedores verifica si un pago fue procesado
        /// /// </summary>
        public static void Proveedores_VerificarPago(int idCuenta, decimal monto, string numFactura)
        {
            Console.WriteLine("\n=== PROVEEDORES: Verificando Pago ===");

            var verificacion = banco.VerificarPago(
                idUsuario: idCuenta,
                monto: monto,
                fechaAproximada: DateTime.Now.AddDays(-1),
                conceptoBusqueda: $"Factura #{numFactura}"
            );

            if (verificacion.Exito && verificacion.PagoEncontrado)
            {
                Console.WriteLine($"✅ Pago encontrado y verificado");
                Console.WriteLine($"   ID Movimiento: {verificacion.IdMovimiento}");
                Console.WriteLine($"   Fecha procesamiento: {verificacion.FechaProcesamiento:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"   Monto: {verificacion.Monto:C2}");
                Console.WriteLine($"   Concepto: {verificacion.Concepto}");
                
                // Proveedores marca la factura como pagada
                MarcarFacturaPagada(numFactura);
            }
            else if (verificacion.Exito && !verificacion.PagoEncontrado)
            {
                Console.WriteLine($"❌ Pago no encontrado en el sistema");
                Console.WriteLine($"   Factura #{numFactura} aún pendiente");
            }
        }

        /// <summary>
        /// Ejemplo 6: Proveedores registra pago recibido
        /// </summary>
        public static void Proveedores_RegistrarPagoRecibido(int idCuenta, decimal monto, string nombreProveedor, string numOrden)
        {
            Console.WriteLine("\n=== PROVEEDORES: Registrando Pago Recibido ===");

            var resultado = banco.RegistrarAbono(
                idUsuario: idCuenta,
                monto: monto,
                concepto: $"Pago de {nombreProveedor} - Orden #{numOrden}",
                moduloOrigen: "PROVEEDORES"
            );

            if (resultado.Exito)
            {
                Console.WriteLine($"✅ Pago registrado: {monto:C2}");
                Console.WriteLine($"   Nuevo saldo: {resultado.SaldoNuevo:C2}");
                
                // Proveedores actualiza estado de orden
                ActualizarEstadoOrden(numOrden, "Pagada");
            }
        }

        private static void MarcarFacturaPagada(string numFactura)
        {
            Console.WriteLine($"   [PROVEEDORES] Factura #{numFactura} marcada como PAGADA");
        }

        private static void ActualizarEstadoOrden(string numOrden, string estado)
        {
            Console.WriteLine($"   [PROVEEDORES] Orden #{numOrden} actualizada a: {estado}");
        }

        #endregion

        #region Ejemplo Completo: Proceso de Venta Integrado

        /// <summary>
        /// Ejemplo 7: Proceso completo de venta integrando todos los módulos
        /// </summary>
        public static void ProcesoVentaCompleto(int idCliente, decimal montoVenta, string detalleVenta)
        {
            Console.WriteLine("\n=== PROCESO DE VENTA INTEGRADO ===");
            Console.WriteLine($"Monto: {montoVenta:C2}");
            Console.WriteLine($"Detalle: {detalleVenta}\n");

            // 1. CRM: Obtener información del cliente
            Console.WriteLine("1. [CRM] Obteniendo información del cliente...");
            var infoCliente = banco.ObtenerInformacionCliente(idCliente);
            if (!infoCliente.Exito)
            {
                Console.WriteLine("❌ Error: Cliente no encontrado");
                return;
            }
            Console.WriteLine($"   Cliente: {infoCliente.NombreCompleto}");

            // 2. BANCO: Verificar saldo suficiente
            Console.WriteLine("\n2. [BANCO] Verificando saldo...");
            var saldo = banco.ObtenerSaldo(idCliente);
            if (saldo.Saldo < montoVenta)
            {
                Console.WriteLine($"❌ Error: Saldo insuficiente");
                Console.WriteLine($"   Saldo actual: {saldo.Saldo:C2}");
                Console.WriteLine($"   Requerido: {montoVenta:C2}");
                return;
            }
            Console.WriteLine($"   Saldo suficiente: {saldo.Saldo:C2}");

            // 3. BANCO: Registrar cargo
            Console.WriteLine("\n3. [BANCO] Procesando pago...");
            var cargo = banco.RegistrarCargo(
                idCliente,
                montoVenta,
                $"Venta: {detalleVenta}",
                "ERP"
            );

            if (!cargo.Exito)
            {
                Console.WriteLine($"❌ Error al procesar pago: {cargo.Mensaje}");
                return;
            }
            Console.WriteLine($"   ✅ Pago procesado - ID: {cargo.IdMovimiento}");
            Console.WriteLine($"   Nuevo saldo: {cargo.SaldoNuevo:C2}");

            // 4. ERP: Registrar en contabilidad
            Console.WriteLine("\n4. [ERP] Registrando en contabilidad...");
            ActualizarContabilidadERP(cargo.IdMovimiento, montoVenta);

            // 5. CRM: Actualizar perfil del cliente
            Console.WriteLine("\n5. [CRM] Actualizando perfil del cliente...");
            ActualizarPuntosLealtad(idCliente, montoVenta);

            // 6. Notificar a todos los módulos
            Console.WriteLine("\n6. [BANCO] Notificando a módulos suscritos...");
            banco.NotificarCambio(idCliente, "Venta", montoVenta, "ERP");

            Console.WriteLine("\n✅ VENTA PROCESADA EXITOSAMENTE");
            Console.WriteLine($"   Total: {montoVenta:C2}");
            Console.WriteLine($"   Saldo final: {cargo.SaldoNuevo:C2}");
        }

        #endregion

        #region Sistema de Notificaciones

        /// <summary>
        /// Ejemplo 8: Suscripción a notificaciones de cambios financieros
        /// </summary>
        public static void ConfigurarNotificaciones()
        {
            Console.WriteLine("\n=== Configurando Sistema de Notificaciones ===");

            // Suscribir módulos a cambios financieros
            banco.CambioFinanciero += OnCambioFinanciero_ERP;
            banco.CambioFinanciero += OnCambioFinanciero_CRM;
            banco.CambioFinanciero += OnCambioFinanciero_Proveedores;

            Console.WriteLine("✅ Módulos suscritos a notificaciones");
        }

        private static void OnCambioFinanciero_ERP(object sender, CambioFinancieroEventArgs e)
        {
            Console.WriteLine($"[ERP] Notificación recibida:");
            Console.WriteLine($"  Operación: {e.TipoOperacion}");
            Console.WriteLine($"  Monto: {e.Monto:C2}");
            Console.WriteLine($"  Origen: {e.ModuloOrigen}");
            // ERP actualiza contabilidad automáticamente
        }

        private static void OnCambioFinanciero_CRM(object sender, CambioFinancieroEventArgs e)
        {
            Console.WriteLine($"[CRM] Notificación recibida:");
            Console.WriteLine($"  Cliente: {e.IdUsuario}");
            Console.WriteLine($"  Operación: {e.TipoOperacion} de {e.Monto:C2}");
            // CRM actualiza perfil del cliente automáticamente
        }

        private static void OnCambioFinanciero_Proveedores(object sender, CambioFinancieroEventArgs e)
        {
            Console.WriteLine($"[PROVEEDORES] Notificación recibida:");
            Console.WriteLine($"  Operación: {e.TipoOperacion}");
            Console.WriteLine($"  Monto: {e.Monto:C2}");
            // Proveedores actualiza estado de órdenes automáticamente
        }

        #endregion

        #region Método Principal de Demostración

        /// <summary>
        /// Ejecuta todos los ejemplos de integración
        /// </summary>
        public static void EjecutarTodosLosEjemplos()
        {
            Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║  DEMOSTRACIÓN DE INTEGRACIÓN - MÓDULO BANCO COMO NÚCLEO   ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝\n");

            // Configurar notificaciones
            ConfigurarNotificaciones();

            // Ejemplos ERP
            ERP_RegistrarNomina(idCuentaEmpresa: 1, montoNomina: 50000.00m);
            ERP_GenerarReporteContable();

            // Ejemplos CRM
            CRM_MostrarPerfilCliente(idCliente: 2);
            CRM_AplicarBonificacion(idCliente: 2, montoBonificacion: 500.00m);

            // Ejemplos Proveedores
            Proveedores_RegistrarPagoRecibido(idCuenta: 1, monto: 10000.00m, nombreProveedor: "ABC Corp", numOrden: "ORD-12345");
            Proveedores_VerificarPago(idCuenta: 1, monto: 10000.00m, numFactura: "FAC-98765");

            // Proceso completo integrado
            ProcesoVentaCompleto(idCliente: 2, montoVenta: 1500.00m, detalleVenta: "Producto XYZ - Cantidad: 3");

            Console.WriteLine("\n╔════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║              DEMOSTRACIÓN COMPLETADA                       ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        }

        #endregion
    }
}

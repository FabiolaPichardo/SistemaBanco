# Sistema de Integración - Módulo Banco como Núcleo Central

## Fecha: Diciembre 2025

---

## 🎯 OBJETIVO

El módulo Banco actúa como **núcleo central** del sistema de paquetería, siendo la fuente oficial de información financiera para todos los demás módulos (ERP, CRM, Proveedores). Proporciona servicios de integración estandarizados para consultar y registrar operaciones financieras en tiempo real.

---

## 🏗️ ARQUITECTURA

```
┌─────────────────────────────────────────────────────────────┐
│                    MÓDULO BANCO (NÚCLEO)                     │
│                 BancoIntegracionService                      │
│                                                              │
│  ┌────────────────────────────────────────────────────┐    │
│  │  • Consulta de Saldos en Tiempo Real              │    │
│  │  • Consulta de Movimientos                        │    │
│  │  • Registro de Operaciones (Cargos/Abonos)       │    │
│  │  • Resúmenes Contables                            │    │
│  │  • Información de Clientes                        │    │
│  │  • Verificación de Pagos                          │    │
│  │  • Sistema de Notificaciones                      │    │
│  └────────────────────────────────────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
        ┌───────────────────┼───────────────────┐
        │                   │                   │
        ▼                   ▼                   ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────┐
│  MÓDULO ERP  │    │  MÓDULO CRM  │    │   MÓDULO     │
│              │    │              │    │  PROVEEDORES │
│ • Contabilidad│   │ • Info Cliente│   │ • Conciliación│
│ • Reportes   │    │ • Historial  │    │ • Pagos      │
│ • Auditoría  │    │ • Estadísticas│   │ • Facturas   │
└──────────────┘    └──────────────┘    └──────────────┘
```

---

## 📋 SERVICIOS DISPONIBLES

### 1. Consulta de Saldos en Tiempo Real

#### Obtener Saldo de una Cuenta
```csharp
// Uso desde cualquier módulo
var servicio = BancoIntegracionService.Instance;
var saldo = servicio.ObtenerSaldo(idUsuario);

if (saldo.Exito)
{
    Console.WriteLine($"Saldo actual: {saldo.Saldo:C2}");
    Console.WriteLine($"Cuenta: {saldo.NumeroCuenta}");
    Console.WriteLine($"Estado: {saldo.Estado}");
}
```

**Respuesta**:
```json
{
  "Exito": true,
  "IdUsuario": 123,
  "NombreUsuario": "jperez",
  "NombreCompleto": "Juan Pérez",
  "NumeroCuenta": "1234567890",
  "TipoCuenta": "Ahorro",
  "Saldo": 15000.50,
  "Estado": "Activa",
  "FechaUltimaActualizacion": "2025-12-03T10:30:00",
  "Mensaje": "Saldo obtenido exitosamente"
}
```

#### Obtener Saldos Múltiples
```csharp
var idsUsuarios = new List<int> { 1, 2, 3, 4, 5 };
var saldos = servicio.ObtenerSaldosMultiples(idsUsuarios);

foreach (var saldo in saldos)
{
    if (saldo.Exito)
        Console.WriteLine($"{saldo.NombreCompleto}: {saldo.Saldo:C2}");
}
```

---

### 2. Consulta de Movimientos

#### Obtener Movimientos en Rango de Fechas
```csharp
DateTime inicio = DateTime.Now.AddMonths(-1);
DateTime fin = DateTime.Now;

var movimientos = servicio.ObtenerMovimientos(idUsuario, inicio, fin);

if (movimientos.Exito)
{
    Console.WriteLine($"Total movimientos: {movimientos.CantidadMovimientos}");
    
    foreach (var mov in movimientos.Movimientos)
    {
        Console.WriteLine($"{mov.FechaMovimiento:dd/MM/yyyy} - {mov.TipoMovimiento}: {mov.Monto:C2}");
        Console.WriteLine($"  Concepto: {mov.Concepto}");
        Console.WriteLine($"  Saldo: {mov.SaldoAnterior:C2} → {mov.SaldoNuevo:C2}");
    }
}
```

#### Obtener Último Movimiento
```csharp
var ultimoMov = servicio.ObtenerUltimoMovimiento(idUsuario);

if (ultimoMov != null)
{
    Console.WriteLine($"Última operación: {ultimoMov.TipoMovimiento} de {ultimoMov.Monto:C2}");
}
```

---

### 3. Registro de Operaciones (Para Módulos Externos)

#### Registrar Cargo desde ERP
```csharp
// Ejemplo: ERP registra un cargo por compra de inventario
var resultado = servicio.RegistrarCargo(
    idUsuario: 123,
    monto: 5000.00m,
    concepto: "Compra de inventario - Factura #12345",
    moduloOrigen: "ERP"
);

if (resultado.Exito)
{
    Console.WriteLine($"Cargo registrado. ID Movimiento: {resultado.IdMovimiento}");
    Console.WriteLine($"Saldo anterior: {resultado.SaldoAnterior:C2}");
    Console.WriteLine($"Saldo nuevo: {resultado.SaldoNuevo:C2}");
}
else
{
    Console.WriteLine($"Error: {resultado.Mensaje}");
}
```

#### Registrar Abono desde Proveedores
```csharp
// Ejemplo: Módulo Proveedores registra un pago recibido
var resultado = servicio.RegistrarAbono(
    idUsuario: 123,
    monto: 10000.00m,
    concepto: "Pago de proveedor ABC - Orden #98765",
    moduloOrigen: "PROVEEDORES"
);

if (resultado.Exito)
{
    Console.WriteLine($"Abono registrado exitosamente");
    // El módulo Proveedores puede actualizar su estado de pago
}
```

**Características**:
- ✅ Validación automática de saldo suficiente (para cargos)
- ✅ Actualización automática del saldo en cuenta
- ✅ Registro en auditoría con módulo de origen
- ✅ Transacciones atómicas (todo o nada)

---

### 4. Resumen Contable para ERP

```csharp
// Obtener resumen contable del mes actual
DateTime inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
DateTime finMes = DateTime.Now;

var resumen = servicio.ObtenerResumenContable(inicioMes, finMes);

if (resumen.Exito)
{
    Console.WriteLine("=== RESUMEN CONTABLE ===");
    Console.WriteLine($"Período: {resumen.FechaInicio:dd/MM/yyyy} - {resumen.FechaFin:dd/MM/yyyy}");
    Console.WriteLine($"Total cuentas activas: {resumen.TotalCuentas}");
    Console.WriteLine($"Total cargos: {resumen.TotalCargos:C2} ({resumen.CantidadCargos} operaciones)");
    Console.WriteLine($"Total abonos: {resumen.TotalAbonos:C2} ({resumen.CantidadAbonos} operaciones)");
    Console.WriteLine($"Saldo total sistema: {resumen.SaldoTotalSistema:C2}");
}
```

**Uso en ERP**:
- Generación de reportes contables
- Balances generales
- Estados de resultados
- Conciliaciones bancarias

---

### 5. Información de Cliente para CRM

```csharp
// Obtener información financiera completa del cliente
var infoCliente = servicio.ObtenerInformacionCliente(idUsuario);

if (infoCliente.Exito)
{
    Console.WriteLine($"Cliente: {infoCliente.NombreCompleto}");
    Console.WriteLine($"Cuenta: {infoCliente.NumeroCuenta}");
    Console.WriteLine($"Saldo actual: {infoCliente.SaldoActual:C2}");
    Console.WriteLine($"Estado: {infoCliente.EstadoCuenta}");
    Console.WriteLine("\n=== Actividad Último Mes ===");
    Console.WriteLine($"Total cargos: {infoCliente.TotalCargosUltimoMes:C2}");
    Console.WriteLine($"Total abonos: {infoCliente.TotalAbonosUltimoMes:C2}");
    Console.WriteLine($"Transacciones: {infoCliente.CantidadTransaccionesUltimoMes}");
    Console.WriteLine($"Última actividad: {infoCliente.FechaUltimaActividad:dd/MM/yyyy HH:mm}");
}
```

**Uso en CRM**:
- Perfil financiero del cliente
- Historial de transacciones
- Análisis de comportamiento
- Segmentación de clientes

---

### 6. Verificación de Pagos para Proveedores

```csharp
// Verificar si un pago fue procesado (conciliación)
var verificacion = servicio.VerificarPago(
    idUsuario: 123,
    monto: 5000.00m,
    fechaAproximada: new DateTime(2025, 12, 1),
    conceptoBusqueda: "Factura #12345"
);

if (verificacion.Exito && verificacion.PagoEncontrado)
{
    Console.WriteLine("✅ Pago encontrado y verificado");
    Console.WriteLine($"ID Movimiento: {verificacion.IdMovimiento}");
    Console.WriteLine($"Fecha procesamiento: {verificacion.FechaProcesamiento:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Monto: {verificacion.Monto:C2}");
    Console.WriteLine($"Concepto: {verificacion.Concepto}");
}
else if (verificacion.Exito && !verificacion.PagoEncontrado)
{
    Console.WriteLine("❌ Pago no encontrado en el sistema");
}
```

**Características**:
- Búsqueda en rango de ±2 días de la fecha aproximada
- Búsqueda por monto exacto
- Búsqueda por concepto (parcial)
- Útil para conciliación automática

---

### 7. Sistema de Notificaciones (Eventos)

```csharp
// Los módulos externos pueden suscribirse a cambios financieros
var servicio = BancoIntegracionService.Instance;

// Suscribirse al evento
servicio.CambioFinanciero += (sender, e) =>
{
    Console.WriteLine($"[{e.ModuloOrigen}] Cambio financiero detectado:");
    Console.WriteLine($"  Usuario: {e.IdUsuario}");
    Console.WriteLine($"  Operación: {e.TipoOperacion}");
    Console.WriteLine($"  Monto: {e.Monto:C2}");
    Console.WriteLine($"  Fecha/Hora: {e.FechaHora:dd/MM/yyyy HH:mm:ss}");
    
    // El módulo puede reaccionar al cambio
    // Ejemplo: ERP actualiza contabilidad, CRM actualiza perfil cliente
};

// Cuando se registra una operación, se notifica automáticamente
servicio.NotificarCambio(123, "Cargo", 5000.00m, "ERP");
```

**Beneficios**:
- Sincronización en tiempo real
- Arquitectura desacoplada
- Reacción automática a cambios
- Trazabilidad completa

---

## 🔐 SEGURIDAD Y AUDITORÍA

### Registro Automático en Auditoría

Todas las operaciones realizadas a través del servicio de integración se registran automáticamente:

```
INTEGRACION_CARGO - Módulo: ERP, Usuario: 123, Monto: $5,000.00, ID Movimiento: 456
INTEGRACION_ABONO - Módulo: PROVEEDORES, Usuario: 123, Monto: $10,000.00, ID Movimiento: 457
```

### Identificación de Módulo de Origen

Cada operación incluye el módulo de origen en el concepto:
```
[ERP] Compra de inventario - Factura #12345
[PROVEEDORES] Pago de proveedor ABC - Orden #98765
[CRM] Ajuste por promoción - Cliente VIP
```

---

## 📊 CASOS DE USO POR MÓDULO

### Módulo ERP (Contabilidad)

```csharp
// 1. Registrar gasto de nómina
servicio.RegistrarCargo(idUsuario, montoNomina, "Pago de nómina quincenal", "ERP");

// 2. Registrar ingreso por venta
servicio.RegistrarAbono(idUsuario, montoVenta, "Venta - Factura #" + numFactura, "ERP");

// 3. Obtener resumen para cierre contable
var resumen = servicio.ObtenerResumenContable(inicioMes, finMes);

// 4. Generar reporte de flujo de efectivo
var movimientos = servicio.ObtenerMovimientos(idUsuario, inicioMes, finMes);
```

### Módulo CRM (Gestión de Clientes)

```csharp
// 1. Mostrar saldo en perfil del cliente
var saldo = servicio.ObtenerSaldo(idCliente);
lblSaldoCliente.Text = $"Saldo: {saldo.Saldo:C2}";

// 2. Obtener historial financiero
var info = servicio.ObtenerInformacionCliente(idCliente);

// 3. Registrar bonificación o promoción
servicio.RegistrarAbono(idCliente, montoBonificacion, "Bonificación cliente VIP", "CRM");

// 4. Verificar capacidad de pago
var saldo = servicio.ObtenerSaldo(idCliente);
bool puedeComprar = saldo.Saldo >= montoCompra;
```

### Módulo Proveedores (Pagos y Conciliación)

```csharp
// 1. Registrar pago recibido de proveedor
servicio.RegistrarAbono(idCuenta, montoPago, $"Pago proveedor {nombreProveedor}", "PROVEEDORES");

// 2. Verificar si un pago fue procesado
var verificacion = servicio.VerificarPago(idCuenta, monto, fecha, "Factura #" + numFactura);

// 3. Conciliación automática
if (verificacion.PagoEncontrado)
{
    // Marcar factura como pagada en módulo Proveedores
    ActualizarEstadoFactura(numFactura, "Pagada");
}

// 4. Registrar pago a proveedor
servicio.RegistrarCargo(idCuenta, montoPago, $"Pago a proveedor {nombreProveedor}", "PROVEEDORES");
```

---

## 🚀 VENTAJAS DEL SISTEMA

### 1. Fuente Única de Verdad
- ✅ Todos los módulos consultan la misma información
- ✅ No hay duplicidad de datos
- ✅ Consistencia garantizada

### 2. Tiempo Real
- ✅ Cambios reflejados inmediatamente
- ✅ Sincronización automática
- ✅ Notificaciones instantáneas

### 3. Trazabilidad Completa
- ✅ Registro en auditoría de todas las operaciones
- ✅ Identificación del módulo de origen
- ✅ Historial completo de cambios

### 4. Desacoplamiento
- ✅ Módulos independientes
- ✅ Fácil mantenimiento
- ✅ Escalabilidad

### 5. Estandarización
- ✅ API uniforme para todos los módulos
- ✅ Respuestas consistentes
- ✅ Manejo de errores estandarizado

---

## 📝 EJEMPLO COMPLETO DE INTEGRACIÓN

```csharp
// Ejemplo: Proceso de venta completo integrando ERP, CRM y Banco

public class ProcesoVenta
{
    private BancoIntegracionService banco = BancoIntegracionService.Instance;
    
    public void ProcesarVenta(int idCliente, decimal montoVenta, string detalleVenta)
    {
        // 1. CRM: Verificar información del cliente
        var infoCliente = banco.ObtenerInformacionCliente(idCliente);
        if (!infoCliente.Exito)
        {
            Console.WriteLine("Error: Cliente no encontrado");
            return;
        }
        
        Console.WriteLine($"Procesando venta para: {infoCliente.NombreCompleto}");
        
        // 2. BANCO: Verificar saldo suficiente
        var saldo = banco.ObtenerSaldo(idCliente);
        if (saldo.Saldo < montoVenta)
        {
            Console.WriteLine("Error: Saldo insuficiente");
            return;
        }
        
        // 3. BANCO: Registrar cargo por la venta
        var cargo = banco.RegistrarCargo(
            idCliente, 
            montoVenta, 
            $"Venta: {detalleVenta}", 
            "ERP"
        );
        
        if (!cargo.Exito)
        {
            Console.WriteLine($"Error al procesar pago: {cargo.Mensaje}");
            return;
        }
        
        // 4. ERP: Registrar en contabilidad
        RegistrarEnContabilidad(cargo.IdMovimiento, montoVenta, detalleVenta);
        
        // 5. CRM: Actualizar perfil del cliente
        ActualizarPerfilCliente(idCliente, montoVenta);
        
        // 6. Notificar a todos los módulos
        banco.NotificarCambio(idCliente, "Venta", montoVenta, "ERP");
        
        Console.WriteLine("✅ Venta procesada exitosamente");
        Console.WriteLine($"Nuevo saldo: {cargo.SaldoNuevo:C2}");
    }
    
    private void RegistrarEnContabilidad(int idMovimiento, decimal monto, string detalle)
    {
        // Lógica del ERP para registrar en contabilidad
        Console.WriteLine($"[ERP] Registrado en contabilidad: {monto:C2}");
    }
    
    private void ActualizarPerfilCliente(int idCliente, decimal montoCompra)
    {
        // Lógica del CRM para actualizar perfil
        Console.WriteLine($"[CRM] Perfil actualizado - Nueva compra: {montoCompra:C2}");
    }
}
```

---

## 🔧 INSTALACIÓN Y CONFIGURACIÓN

### 1. Agregar el Servicio al Proyecto

El archivo `BancoIntegracionService.cs` ya está incluido en el proyecto.

### 2. Uso desde Otros Módulos

```csharp
// Obtener instancia del servicio (Singleton)
var servicio = BancoIntegracionService.Instance;

// Usar cualquier método disponible
var saldo = servicio.ObtenerSaldo(idUsuario);
```

### 3. Suscribirse a Notificaciones (Opcional)

```csharp
// En la inicialización del módulo
BancoIntegracionService.Instance.CambioFinanciero += OnCambioFinanciero;

private void OnCambioFinanciero(object sender, CambioFinancieroEventArgs e)
{
    // Reaccionar al cambio
    Console.WriteLine($"Cambio detectado: {e.TipoOperacion} de {e.Monto:C2}");
}
```

---

## 📚 DOCUMENTACIÓN ADICIONAL

### Métodos Disponibles

| Método | Descripción | Módulos Objetivo |
|--------|-------------|------------------|
| `ObtenerSaldo()` | Consulta saldo actual | Todos |
| `ObtenerMovimientos()` | Consulta movimientos | Todos |
| `RegistrarCargo()` | Registra cargo | ERP, Proveedores |
| `RegistrarAbono()` | Registra abono | ERP, Proveedores, CRM |
| `ObtenerResumenContable()` | Resumen contable | ERP |
| `ObtenerInformacionCliente()` | Info financiera cliente | CRM |
| `VerificarPago()` | Verifica pago procesado | Proveedores |

### Códigos de Respuesta

- `Exito = true`: Operación exitosa
- `Exito = false`: Error en la operación (ver `Mensaje`)

---

## ✅ ESTADO

**Implementación**: ✅ Completada  
**Pruebas**: ⏳ Pendientes  
**Documentación**: ✅ Completada  
**Integración**: 🔄 Lista para usar  

---

**Última actualización**: Diciembre 2025  
**Versión**: 1.0  
**Autor**: Sistema Banco - Núcleo Central

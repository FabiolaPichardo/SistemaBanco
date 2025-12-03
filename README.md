# Sistema Banco - Módulo de Gestión Bancaria

## 🏦 Descripción

Sistema integral de gestión bancaria desarrollado en C# con Windows Forms y PostgreSQL. Incluye gestión de cuentas, movimientos financieros, transferencias, autorización de divisas, auditoría y administración de usuarios.

## ✨ Características Principales

### Gestión de Cuentas
- ✅ Consulta de saldos en tiempo real
- ✅ Indicadores visuales de estado de cuenta
- ✅ Actualización automática cada 30 segundos
- ✅ Filtros por período histórico

### Movimientos Financieros
- ✅ Registro de cargos y abonos
- ✅ Autocompletado de beneficiarios
- ✅ Generación automática de folios y referencias
- ✅ Validación de campos obligatorios
- ✅ Sistema de estados (Pendiente, Autorizado, Rechazado)

### Transferencias
- ✅ Transferencias entre cuentas
- ✅ Autocompletado de cuentas destino
- ✅ Validación de saldos
- ✅ Confirmación antes de ejecutar

### Historial y Reportes
- ✅ Búsqueda en tiempo real
- ✅ Filtros por tipo de movimiento
- ✅ Exportación a PDF, Word y Excel
- ✅ Colores por tipo de operación

### Autorización de Divisas
- ✅ Gestión de operaciones en moneda extranjera
- ✅ Sistema de autorización por roles
- ✅ Fechas de expiración
- ✅ Exportación de reportes

### Administración
- ✅ Gestión de usuarios
- ✅ Sistema de roles (Cliente, Cajero, Ejecutivo, Gerente, Administrador)
- ✅ Auditoría completa de operaciones
- ✅ Detección de actividades sospechosas

## 🎨 Interfaz de Usuario

### Panel de Control Adaptativo
La interfaz se adapta según el rol del usuario:

**Cliente/Cajero:** Operaciones básicas
**Ejecutivo:** + Autorización de divisas
**Gerente/Administrador:** + Administración de usuarios

### Características de UX
- ✅ Placeholders en todos los campos
- ✅ Autocompletado inteligente
- ✅ Confirmaciones antes de acciones críticas
- ✅ Mensajes claros y descriptivos
- ✅ Botón de cerrar sesión visible para todos

## 📁 Estructura del Proyecto

```
Banco/
├── imagenes/              # Logo y recursos visuales
│   ├── logo.png
│   └── logo.ico
├── scripts_sql/           # Scripts de base de datos
│   ├── EJECUTAR_PRIMERO.sql
│   ├── database_setup.sql
│   └── [otros scripts...]
├── documentacion/         # Documentación del proyecto
│   ├── README_CORRECCIONES_FINALES.md
│   ├── CAMBIOS_FINALES_INTERFAZ.md
│   ├── RESUMEN_VISUAL_CAMBIOS.md
│   └── [otros documentos...]
├── Form*.cs              # Formularios de la aplicación
├── *.cs                  # Clases del sistema
└── SistemaBanco.csproj   # Archivo del proyecto
```

## 🚀 Instalación y Configuración

### Requisitos Previos
- .NET 8.0 SDK
- PostgreSQL 12 o superior
- Windows 10/11

### Configuración de Base de Datos

1. Ejecutar scripts en orden:
```sql
-- 1. Crear estructura base
scripts_sql/EJECUTAR_PRIMERO.sql

-- 2. Configurar base de datos
scripts_sql/database_setup.sql

-- 3. Actualizar roles
scripts_sql/actualizar_roles.sql

-- 4. Crear tablas de movimientos
scripts_sql/crear_movimientos_financieros.sql

-- 5. Configurar auditoría
scripts_sql/crear_sistema_auditoria_completo.sql
```

2. Configurar cadena de conexión en `App.config`:
```xml
<connectionStrings>
  <add name="BancoConnection" 
       connectionString="Host=tu-servidor;Database=tu-bd;Username=tu-usuario;Password=tu-password" 
       providerName="Npgsql" />
</connectionStrings>
```

### Compilación

```bash
dotnet build SistemaBanco.csproj
```

### Ejecución

```bash
dotnet run
```

O ejecutar el archivo generado:
```
bin/Debug/net8.0-windows/SistemaBanco.exe
```

## 👥 Sistema de Roles

### Cliente
- Consultar saldo
- Ver historial
- Realizar transferencias
- Generar estado de cuenta

### Cajero
- Todo lo de Cliente
- Registrar movimientos financieros básicos

### Ejecutivo
- Todo lo de Cajero
- Autorización de divisas
- Revisión de movimientos
- Exportación completa de reportes

### Gerente
- Todo lo de Ejecutivo
- Configuración de roles de divisas
- Acceso a auditoría completa

### Administrador
- Acceso total al sistema
- Administración de usuarios
- Configuración del sistema
- Gestión de permisos

## 📊 Exportación de Datos

### Formatos Disponibles

**PDF (HTML):**
- Se genera archivo HTML
- Se abre en navegador
- Guardar como PDF con Ctrl+P

**Word (.doc):**
- Formato compatible con Microsoft Word
- Diseño profesional
- Incluye metadatos

**Excel (CSV):**
- Compatible con Excel y LibreOffice
- Incluye descripciones
- Fácil de importar

## 🔒 Seguridad

- ✅ Autenticación de usuarios
- ✅ Sistema de roles y permisos
- ✅ Auditoría completa de operaciones
- ✅ Detección de actividades sospechosas
- ✅ Validación de accesos en backend
- ✅ Registro de intentos fallidos

## 🎯 Características Destacadas

### Autocompletado Inteligente
- Cuentas beneficiarias en movimientos
- Cuentas destino en transferencias
- IDs de transacción en divisas

### Búsqueda en Tiempo Real
- Historial de movimientos
- Administración de usuarios
- Auditoría de operaciones

### Validaciones Robustas
- Campos obligatorios
- Formatos de datos
- Saldos disponibles
- Permisos de usuario

### Exportación Flexible
- Selección de ubicación
- Múltiples formatos
- Datos completos sin paginación

## 📝 Documentación

Consulte la carpeta `documentacion/` para:
- Guías de instalación
- Manuales de usuario
- Documentación técnica
- Resúmenes de implementación
- Guías de pruebas

## 🛠️ Tecnologías Utilizadas

- **Framework:** .NET 8.0
- **UI:** Windows Forms
- **Base de Datos:** PostgreSQL
- **ORM:** Npgsql
- **Lenguaje:** C# 12

## 📞 Soporte

Para soporte o consultas, consulte la documentación en la carpeta `documentacion/` o revise los comentarios en el código fuente.

## 📄 Licencia

Este proyecto es parte de un sistema académico/empresarial. Todos los derechos reservados.

## 🎉 Versión Actual

**Versión:** 1.0.0
**Fecha:** Diciembre 2025
**Estado:** Producción

---

© 2025 Módulo Banco - Sistema de Gestión Bancaria

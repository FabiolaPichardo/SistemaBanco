# 🚀 Guía de Despliegue - Banco Premier

## Preparación para Producción

Esta guía te ayudará a desplegar el sistema bancario en un entorno de producción.

---

## 📋 Pre-requisitos

### Software Necesario:
- ✅ Windows Server 2019/2022 o Windows 10/11 Pro
- ✅ .NET 8.0 Runtime (Desktop)
- ✅ PostgreSQL 14+ Server
- ✅ 4GB RAM mínimo (8GB recomendado)
- ✅ 500MB espacio en disco

### Accesos Requeridos:
- ✅ Acceso administrativo al servidor
- ✅ Permisos para crear base de datos
- ✅ Puerto 5432 disponible (PostgreSQL)

---

## 🗄️ 1. Configuración de Base de Datos

### Paso 1: Instalar PostgreSQL

```bash
# Descargar desde: https://www.postgresql.org/download/windows/
# Instalar con configuración por defecto
# Puerto: 5432
# Usuario: postgres
# Contraseña: [tu_contraseña_segura]
```

### Paso 2: Crear Base de Datos

```bash
# Abrir pgAdmin o psql
psql -U postgres

# Ejecutar:
CREATE DATABASE banco_db;
\c banco_db
```

### Paso 3: Ejecutar Script de Configuración

```bash
# Desde psql:
\i database_setup.sql

# O desde pgAdmin:
# Tools → Query Tool → Abrir database_setup.sql → Ejecutar
```

### Paso 4: Verificar Instalación

```sql
-- Verificar tablas
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public';

-- Verificar datos
SELECT COUNT(*) FROM usuarios;
SELECT COUNT(*) FROM cuentas;
SELECT COUNT(*) FROM movimientos;
```

### Paso 5: Crear Usuario de Aplicación (Recomendado)

```sql
-- Crear usuario específico para la aplicación
CREATE USER banco_app WITH PASSWORD 'contraseña_segura_aqui';

-- Otorgar permisos
GRANT CONNECT ON DATABASE banco_db TO banco_app;
GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO banco_app;
GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO banco_app;
```

---

## 🔧 2. Configuración de la Aplicación

### Paso 1: Compilar en Modo Release

```bash
cd SistemaBanco
dotnet publish -c Release -r win-x64 --self-contained false
```

### Paso 2: Configurar Cadena de Conexión

Editar `App.config` en la carpeta de publicación:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="BancoDB" 
         connectionString="Host=localhost;Port=5432;Database=banco_db;Username=banco_app;Password=contraseña_segura_aqui;SSL Mode=Prefer;Trust Server Certificate=true" 
         providerName="Npgsql" />
  </connectionStrings>
</configuration>
```

### Paso 3: Copiar Archivos

```bash
# Copiar carpeta de publicación a:
C:\Program Files\BancoPremier\

# Estructura:
BancoPremier/
├── SistemaBanco.exe
├── SistemaBanco.dll
├── App.config
├── Npgsql.dll
└── [otros archivos necesarios]
```

---

## 🔒 3. Configuración de Seguridad

### Firewall de Windows

```powershell
# Permitir PostgreSQL
New-NetFirewallRule -DisplayName "PostgreSQL" -Direction Inbound -LocalPort 5432 -Protocol TCP -Action Allow

# Permitir aplicación
New-NetFirewallRule -DisplayName "Banco Premier" -Direction Inbound -Program "C:\Program Files\BancoPremier\SistemaBanco.exe" -Action Allow
```

### Permisos de Archivos

```powershell
# Dar permisos de lectura/ejecución
icacls "C:\Program Files\BancoPremier" /grant Users:(OI)(CI)RX /T
```

### SSL/TLS para PostgreSQL (Recomendado)

```bash
# En postgresql.conf:
ssl = on
ssl_cert_file = 'server.crt'
ssl_key_file = 'server.key'

# Reiniciar PostgreSQL
```

---

## 🎯 4. Configuración de Producción

### Cambiar Contraseñas por Defecto

```sql
-- Cambiar contraseñas de usuarios de prueba
UPDATE usuarios SET contraseña = 'nueva_contraseña_segura' WHERE usuario = 'admin';
UPDATE usuarios SET contraseña = 'nueva_contraseña_segura' WHERE usuario = 'jperez';

-- O eliminar usuarios de prueba
DELETE FROM usuarios WHERE usuario IN ('mlopez', 'cgarcia', 'arodriguez');
```

### Configurar Límites

```sql
-- Ajustar límites de retiro según política del banco
UPDATE cuentas SET limite_retiro_diario = 5000.00;
```

### Habilitar Auditoría

```sql
-- Crear tabla de auditoría
CREATE TABLE auditoria (
    id_auditoria SERIAL PRIMARY KEY,
    id_usuario INTEGER,
    accion VARCHAR(100),
    tabla VARCHAR(50),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(50),
    detalles TEXT
);

-- Crear trigger de auditoría (ejemplo para movimientos)
CREATE OR REPLACE FUNCTION auditar_movimientos()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO auditoria (id_usuario, accion, tabla, detalles)
    VALUES (
        (SELECT id_usuario FROM cuentas WHERE id_cuenta = NEW.id_cuenta),
        TG_OP,
        'movimientos',
        'Tipo: ' || NEW.tipo || ', Monto: ' || NEW.monto
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_auditar_movimientos
AFTER INSERT ON movimientos
FOR EACH ROW EXECUTE FUNCTION auditar_movimientos();
```

---

## 📊 5. Monitoreo y Mantenimiento

### Backup Automático

```bash
# Crear script de backup (backup_banco.bat)
@echo off
set PGPASSWORD=tu_contraseña
set BACKUP_DIR=C:\Backups\BancoPremier
set FECHA=%date:~-4,4%%date:~-7,2%%date:~-10,2%

"C:\Program Files\PostgreSQL\14\bin\pg_dump.exe" -U postgres -h localhost banco_db > "%BACKUP_DIR%\banco_db_%FECHA%.sql"

echo Backup completado: %FECHA%
```

### Programar Backup en Windows

```powershell
# Crear tarea programada
$action = New-ScheduledTaskAction -Execute "C:\Scripts\backup_banco.bat"
$trigger = New-ScheduledTaskTrigger -Daily -At 2am
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "Backup Banco Premier" -Description "Backup diario de base de datos"
```

### Monitoreo de PostgreSQL

```sql
-- Ver conexiones activas
SELECT * FROM pg_stat_activity WHERE datname = 'banco_db';

-- Ver tamaño de base de datos
SELECT pg_size_pretty(pg_database_size('banco_db'));

-- Ver tablas más grandes
SELECT 
    schemaname,
    tablename,
    pg_size_pretty(pg_total_relation_size(schemaname||'.'||tablename)) AS size
FROM pg_tables
WHERE schemaname = 'public'
ORDER BY pg_total_relation_size(schemaname||'.'||tablename) DESC;
```

### Logs de Aplicación

```csharp
// Agregar logging en Database.cs (ejemplo)
public static void LogError(string message, Exception ex)
{
    string logPath = @"C:\Logs\BancoPremier\";
    Directory.CreateDirectory(logPath);
    
    string logFile = Path.Combine(logPath, $"error_{DateTime.Now:yyyyMMdd}.log");
    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n{ex.Message}\n{ex.StackTrace}\n\n";
    
    File.AppendAllText(logFile, logEntry);
}
```

---

## 🔄 6. Actualización de Versiones

### Proceso de Actualización:

1. **Backup completo**
```bash
pg_dump -U postgres banco_db > backup_pre_update.sql
```

2. **Detener aplicación**
```powershell
Stop-Process -Name "SistemaBanco" -Force
```

3. **Actualizar archivos**
```bash
# Copiar nuevos archivos
xcopy /E /Y "nueva_version\*" "C:\Program Files\BancoPremier\"
```

4. **Ejecutar scripts de migración** (si hay cambios en BD)
```sql
-- migration_v2.sql
ALTER TABLE usuarios ADD COLUMN ultimo_cambio_password TIMESTAMP;
-- etc.
```

5. **Verificar configuración**
```bash
# Verificar App.config
# Verificar permisos
```

6. **Iniciar aplicación**
```bash
cd "C:\Program Files\BancoPremier"
SistemaBanco.exe
```

7. **Verificar funcionamiento**
- Login exitoso
- Consulta de saldo
- Registro de movimiento de prueba

---

## 🌐 7. Configuración Multi-Usuario

### Para Red Local:

1. **Configurar PostgreSQL para aceptar conexiones remotas**

Editar `postgresql.conf`:
```
listen_addresses = '*'
```

Editar `pg_hba.conf`:
```
# IPv4 local connections:
host    banco_db    banco_app    192.168.1.0/24    md5
```

2. **Reiniciar PostgreSQL**
```bash
net stop postgresql-x64-14
net start postgresql-x64-14
```

3. **Actualizar App.config en clientes**
```xml
<add name="BancoDB" 
     connectionString="Host=192.168.1.100;Port=5432;Database=banco_db;Username=banco_app;Password=contraseña" 
     providerName="Npgsql" />
```

---

## 📱 8. Instalador (Opcional)

### Crear Instalador con Inno Setup:

```pascal
[Setup]
AppName=Banco Premier
AppVersion=1.0
DefaultDirName={pf}\BancoPremier
DefaultGroupName=Banco Premier
OutputDir=Output
OutputBaseFilename=BancoPremierSetup

[Files]
Source: "bin\Release\net8.0-windows\*"; DestDir: "{app}"; Flags: recursesubdirs

[Icons]
Name: "{group}\Banco Premier"; Filename: "{app}\SistemaBanco.exe"
Name: "{commondesktop}\Banco Premier"; Filename: "{app}\SistemaBanco.exe"

[Run]
Filename: "{app}\SistemaBanco.exe"; Description: "Iniciar Banco Premier"; Flags: postinstall nowait skipifsilent
```

---

## ✅ 9. Checklist de Despliegue

### Pre-Despliegue:
- [ ] PostgreSQL instalado y configurado
- [ ] Base de datos creada y poblada
- [ ] Usuario de aplicación creado
- [ ] Aplicación compilada en Release
- [ ] Cadena de conexión configurada
- [ ] Contraseñas por defecto cambiadas

### Despliegue:
- [ ] Archivos copiados a ubicación final
- [ ] Permisos de archivos configurados
- [ ] Firewall configurado
- [ ] Aplicación inicia correctamente
- [ ] Login funciona
- [ ] Todas las funciones operativas

### Post-Despliegue:
- [ ] Backup automático configurado
- [ ] Monitoreo habilitado
- [ ] Logs funcionando
- [ ] Documentación entregada
- [ ] Usuarios capacitados

---

## 🆘 10. Solución de Problemas

### Error: "No se puede conectar a la base de datos"

```bash
# Verificar que PostgreSQL esté corriendo
net start postgresql-x64-14

# Verificar puerto
netstat -an | findstr 5432

# Verificar cadena de conexión en App.config
```

### Error: "Acceso denegado"

```sql
-- Verificar permisos del usuario
SELECT * FROM information_schema.role_table_grants 
WHERE grantee = 'banco_app';

-- Otorgar permisos faltantes
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO banco_app;
```

### Aplicación lenta

```sql
-- Analizar queries lentas
SELECT * FROM pg_stat_statements 
ORDER BY total_time DESC LIMIT 10;

-- Reindexar tablas
REINDEX TABLE movimientos;
REINDEX TABLE cuentas;

-- Actualizar estadísticas
ANALYZE;
```

---

## 📞 11. Soporte Post-Despliegue

### Contactos:
- **Soporte Técnico**: soporte@bancopremier.com
- **Emergencias**: 555-0000 (24/7)

### Recursos:
- Documentación: Ver README.md y GUIA_USUARIO.md
- Base de conocimiento: [URL]
- Tickets: [Sistema de tickets]

---

## 📈 12. Métricas de Éxito

### KPIs a Monitorear:
- Tiempo de respuesta promedio
- Número de transacciones por día
- Tasa de errores
- Disponibilidad del sistema (uptime)
- Satisfacción de usuarios

### Herramientas Recomendadas:
- PostgreSQL logs
- Windows Event Viewer
- Custom logging en aplicación
- Monitoreo de recursos (CPU, RAM, Disco)

---

**¡Despliegue Exitoso!** 🎉

*Banco Premier - Sistema Bancario Profesional*

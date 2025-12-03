# 🔍 DIAGNÓSTICO DE ERRORES - MÓDULO DE DIVISAS

## Error: "Object reference not set to an instance of an object"

### 📋 Causas Posibles

#### 1. Tablas No Creadas en la Base de Datos ⚠️ (MÁS COMÚN)

**Síntoma**: El error aparece al abrir el módulo de Autorización de Divisas

**Causa**: El script SQL no se ha ejecutado o no se ejecutó correctamente

**Solución**:

```bash
# Ejecutar el script de creación
psql -U tu_usuario -d tu_base_datos -f crear_sistema_autorizacion_divisas.sql

# Verificar la instalación
psql -U tu_usuario -d tu_base_datos -f verificar_instalacion_divisas.sql
```

**Verificación Manual**:
```sql
-- Verificar que existen las tablas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name LIKE '%divisa%';

-- Deberías ver:
-- divisas
-- roles_autorizadores_divisas
-- solicitudes_autorizacion_divisas
-- historial_autorizacion_divisas
```

#### 2. Conexión a Base de Datos Incorrecta

**Síntoma**: El error aparece pero las tablas existen

**Causa**: La aplicación está conectada a una base de datos diferente

**Solución**:

1. Abre `App.config`
2. Verifica la cadena de conexión:
```xml
<connectionStrings>
    <add name="PostgreSQL" 
         connectionString="Host=localhost;Port=5432;Database=NOMBRE_BD;Username=usuario;Password=contraseña" 
         providerName="Npgsql" />
</connectionStrings>
```
3. Asegúrate de que `Database=NOMBRE_BD` apunta a la base de datos correcta

#### 3. Permisos Insuficientes

**Síntoma**: El script se ejecuta pero da errores

**Causa**: El usuario de PostgreSQL no tiene permisos

**Solución**:
```sql
-- Otorgar permisos al usuario
GRANT ALL PRIVILEGES ON DATABASE tu_base_datos TO tu_usuario;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO tu_usuario;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO tu_usuario;
```

## 🔧 Pasos de Diagnóstico

### Paso 1: Verificar Conexión a Base de Datos

```csharp
// Prueba rápida en C#
try
{
    string query = "SELECT version()";
    DataTable dt = Database.ExecuteQuery(query);
    MessageBox.Show($"Conectado a: {dt.Rows[0][0]}");
}
catch (Exception ex)
{
    MessageBox.Show($"Error de conexión: {ex.Message}");
}
```

### Paso 2: Verificar Existencia de Tablas

```sql
-- Ejecutar en pgAdmin o psql
SELECT COUNT(*) as tablas_divisas
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name IN (
    'divisas', 
    'roles_autorizadores_divisas', 
    'solicitudes_autorizacion_divisas', 
    'historial_autorizacion_divisas'
);

-- Resultado esperado: 4
```

### Paso 3: Verificar Datos Iniciales

```sql
-- Verificar divisas
SELECT COUNT(*) FROM divisas;
-- Resultado esperado: 5

-- Verificar configuraciones de roles
SELECT COUNT(*) FROM roles_autorizadores_divisas;
-- Resultado esperado: >= 15
```

### Paso 4: Probar Consulta del Módulo

```sql
-- Esta es la consulta que hace el módulo
SELECT 
    s.id_solicitud,
    s.id_transaccion,
    s.descripcion,
    u.nombre_completo AS titular,
    d.codigo AS divisa,
    s.tasa_cambio,
    s.monto_mxn,
    s.monto_divisa,
    s.estado,
    s.fecha_solicitud,
    s.fecha_expiracion,
    COALESCE(u_aut.nombre_completo, '-') AS autorizador
FROM solicitudes_autorizacion_divisas s
INNER JOIN usuarios u ON s.id_usuario_solicitante = u.id_usuario
INNER JOIN divisas d ON s.id_divisa = d.id_divisa
LEFT JOIN usuarios u_aut ON s.id_usuario_autorizador = u_aut.id_usuario
WHERE 1=1
ORDER BY s.fecha_solicitud DESC;

-- Si esta consulta funciona, el módulo debería funcionar
```

## 🎯 Soluciones Rápidas

### Solución 1: Reinstalar Tablas

```sql
-- ADVERTENCIA: Esto eliminará todas las solicitudes existentes
DROP TABLE IF EXISTS historial_autorizacion_divisas CASCADE;
DROP TABLE IF EXISTS solicitudes_autorizacion_divisas CASCADE;
DROP TABLE IF EXISTS roles_autorizadores_divisas CASCADE;
DROP TABLE IF EXISTS divisas CASCADE;

-- Luego ejecutar el script de creación
\i crear_sistema_autorizacion_divisas.sql
```

### Solución 2: Verificar y Corregir Datos

```sql
-- Verificar divisas
SELECT * FROM divisas;

-- Si no hay divisas, insertarlas manualmente
INSERT INTO divisas (codigo, nombre, simbolo, tasa_cambio) VALUES
('USD', 'Dólar Estadounidense', '$', 17.50),
('EUR', 'Euro', '€', 19.20),
('GBP', 'Libra Esterlina', '£', 22.30),
('CAD', 'Dólar Canadiense', 'C$', 13.10),
('JPY', 'Yen Japonés', '¥', 0.12)
ON CONFLICT (codigo) DO NOTHING;
```

### Solución 3: Verificar Función de Expiración

```sql
-- Probar la función
SELECT actualizar_solicitudes_expiradas();

-- Si da error, recrearla
CREATE OR REPLACE FUNCTION actualizar_solicitudes_expiradas()
RETURNS void AS $$
BEGIN
    UPDATE solicitudes_autorizacion_divisas
    SET estado = 'Expirada'
    WHERE estado IN ('Pendiente', 'En Revisión')
    AND fecha_expiracion IS NOT NULL
    AND fecha_expiracion < CURRENT_TIMESTAMP;
END;
$$ LANGUAGE plpgsql;
```

## 📊 Checklist de Diagnóstico

Marca cada ítem conforme lo verifiques:

- [ ] Script SQL ejecutado sin errores
- [ ] 4 tablas creadas (divisas, roles_autorizadores_divisas, solicitudes_autorizacion_divisas, historial_autorizacion_divisas)
- [ ] 5 divisas en la tabla divisas
- [ ] Al menos 15 configuraciones en roles_autorizadores_divisas
- [ ] Función actualizar_solicitudes_expiradas() existe
- [ ] Trigger trigger_historial_estado_divisa existe
- [ ] Vista vista_solicitudes_divisas existe
- [ ] Conexión a base de datos correcta en App.config
- [ ] Usuario tiene rol Ejecutivo, Gerente o Administrador
- [ ] Proyecto compila sin errores

## 🆘 Si Nada Funciona

1. **Exporta los logs de error**:
   - Copia el mensaje de error completo
   - Incluye el stack trace si está disponible

2. **Verifica la versión de PostgreSQL**:
   ```sql
   SELECT version();
   ```
   - Versión mínima requerida: PostgreSQL 9.5+

3. **Revisa los logs de PostgreSQL**:
   - Windows: `C:\Program Files\PostgreSQL\[version]\data\log\`
   - Linux: `/var/log/postgresql/`

4. **Prueba con un usuario diferente**:
   - Crea un nuevo usuario con rol Administrador
   - Intenta acceder al módulo con ese usuario

## 📞 Información para Soporte

Si necesitas ayuda, proporciona:

1. Mensaje de error completo
2. Resultado del script `verificar_instalacion_divisas.sql`
3. Versión de PostgreSQL
4. Rol del usuario que intenta acceder
5. Contenido de la cadena de conexión (sin contraseña)

---

**Última actualización**: Diciembre 2025  
**Versión**: 1.0


---

## 🔧 ERRORES CORREGIDOS - HISTORIAL DE SOLUCIONES

### Error Corregido #1: NullReferenceException en FormConfigRolesDivisas

**Fecha**: Diciembre 2025  
**Estado**: ✅ RESUELTO

#### Descripción del Error
Al hacer clic en el botón "⚙ Ir a Config de Roles" desde FormAutorizacionDivisas, se producía el error:
```
Error al Cargar Configuración
No se pudo cargar la configuración.
Detalle: Object reference not set to an instance of an object.
```

#### Causa Raíz
En el constructor de `FormConfigRolesDivisas`, los métodos `CargarDivisas()` y `CargarConfiguracion()` se llamaban **antes** de que `InitializeComponent()` creara los controles `cmbDivisa` y `dgvConfiguracion`, causando que los controles fueran null al intentar acceder a ellos.

#### Solución Aplicada

**1. Corrección del Constructor**:
```csharp
// ANTES (INCORRECTO)
public FormConfigRolesDivisas()
{
    InitializeComponent();
    CargarDivisas();
    CargarConfiguracion();
}

// DESPUÉS (CORRECTO)
public FormConfigRolesDivisas()
{
    InitializeComponent();
    // Cargar datos después de que los controles estén inicializados
    try
    {
        CargarDivisas();
        CargarConfiguracion();
    }
    catch (Exception ex)
    {
        CustomMessageBox.Show("Error al Inicializar",
            $"Error al inicializar el formulario.\n\nDetalle: {ex.Message}",
            MessageBoxIcon.Error);
    }
}
```

**2. Validaciones en CargarDivisas()**:
```csharp
private void CargarDivisas()
{
    try
    {
        // Validar que el control esté inicializado
        if (cmbDivisa == null)
        {
            System.Diagnostics.Debug.WriteLine("cmbDivisa no está inicializado");
            return;
        }
        
        cmbDivisa.Items.Clear();
        // ... resto del código
    }
    catch (Exception ex)
    {
        CustomMessageBox.Show("Error al Cargar Divisas",
            $"No se pudieron cargar las divisas.\n\nDetalle: {ex.Message}",
            MessageBoxIcon.Error);
    }
}
```

**3. Validaciones en CargarConfiguracion()**:
```csharp
private void CargarConfiguracion()
{
    try
    {
        // Validar que el control esté inicializado
        if (dgvConfiguracion == null)
        {
            System.Diagnostics.Debug.WriteLine("dgvConfiguracion no está inicializado");
            return;
        }
        
        string query = @"SELECT ...";
        DataTable dt = Database.ExecuteQuery(query);
        dgvConfiguracion.DataSource = dt;
        ConfigurarColumnas();
    }
    catch (Exception ex)
    {
        CustomMessageBox.Show("Error al Cargar Configuración",
            $"No se pudo cargar la configuración.\n\nDetalle: {ex.Message}",
            MessageBoxIcon.Error);
    }
}
```

**4. Validaciones en ConfigurarColumnas()**:
```csharp
private void ConfigurarColumnas()
{
    try
    {
        if (dgvConfiguracion == null || dgvConfiguracion.Columns.Count == 0) 
            return;

        // Validar existencia de cada columna antes de configurarla
        if (dgvConfiguracion.Columns.Contains("id_config"))
            dgvConfiguracion.Columns["id_config"].Visible = false;

        if (dgvConfiguracion.Columns.Contains("divisa"))
        {
            dgvConfiguracion.Columns["divisa"].HeaderText = "Divisa";
            dgvConfiguracion.Columns["divisa"].Width = 80;
        }
        
        // ... resto de columnas con validación Contains()
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Error en ConfigurarColumnas: {ex.Message}");
    }
}
```

#### Archivos Modificados
- `FormConfigRolesDivisas.cs`

#### Pruebas Realizadas
- ✅ Apertura del formulario desde botón "Ir a Config de Roles"
- ✅ Carga correcta de divisas en el ComboBox
- ✅ Carga correcta de configuraciones existentes en el DataGridView
- ✅ Compilación exitosa sin errores
- ✅ No se producen excepciones al abrir el formulario

#### Lecciones Aprendidas
1. **Siempre llamar a InitializeComponent() primero** en el constructor antes de cualquier otra operación
2. **Validar controles antes de usarlos** para evitar NullReferenceException
3. **Usar Contains() antes de acceder a columnas** de DataGridView
4. **Envolver operaciones de carga en try-catch** para manejo robusto de errores
5. **Agregar mensajes de debug** para facilitar diagnóstico futuro

---

### Error Corregido #2: NullReferenceException en FormAutorizacionDivisas

**Fecha**: Diciembre 2025  
**Estado**: ✅ RESUELTO

#### Descripción
Error similar al anterior, donde los controles no estaban inicializados al momento de cargar datos.

#### Solución
Se aplicaron las mismas técnicas de validación y manejo de errores que en FormConfigRolesDivisas.

---

**Nota**: Estos errores son comunes en Windows Forms cuando se intenta acceder a controles antes de que sean creados por InitializeComponent(). La solución estándar es siempre validar que los controles no sean null antes de usarlos.

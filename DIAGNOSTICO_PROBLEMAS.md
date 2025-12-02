# 🔍 DIAGNÓSTICO DE PROBLEMAS - Sistema Bancario

## ❌ Problemas Reportados

1. **Login no funciona** - Dice "nombre de usuario o contraseña incorrectos"
2. **Recuperación no funciona** - Dice "el usuario no está registrado"
3. **Registro no funciona** - Dice "el formulario está en mantenimiento"

## 🔧 SOLUCIÓN PASO A PASO

### PASO 1: Probar la Conexión a la Base de Datos

He creado un programa de prueba para verificar la conexión. Sigue estos pasos:

1. Abre el archivo `Program.cs`
2. Descomenta estas líneas (quita los `//`):
   ```csharp
   // TestConexion.ProbarConexion();
   // return;
   ```
   
   Debe quedar así:
   ```csharp
   TestConexion.ProbarConexion();
   return;
   ```

3. Ejecuta el programa con:
   ```
   dotnet run
   ```

4. El programa mostrará:
   - ✓ Si la conexión funciona
   - ✓ Cuántos usuarios hay en la base de datos
   - ✓ Lista de usuarios con sus datos
   - ✓ Si el usuario 'admin' existe y su contraseña
   - ✓ Si los usuarios tienen preguntas de seguridad

### PASO 2: Verificar la Base de Datos Directamente

Ejecuta el archivo `test_conexion.sql` en tu cliente PostgreSQL (Supabase):

1. Abre Supabase SQL Editor
2. Copia y pega el contenido de `test_conexion.sql`
3. Ejecuta el script
4. Verifica los resultados:
   - ¿Existe la tabla usuarios?
   - ¿Hay usuarios en la tabla?
   - ¿Los usuarios tienen preguntas de seguridad?

### PASO 3: Actualizar Usuarios con Preguntas de Seguridad

Si los usuarios NO tienen preguntas de seguridad, ejecuta:

1. Abre Supabase SQL Editor
2. Copia y pega el contenido de `actualizar_usuarios.sql`
3. Ejecuta el script
4. Verifica que se actualizaron correctamente

### PASO 4: Verificar Credenciales de Prueba

Después de ejecutar `actualizar_usuarios.sql`, usa estas credenciales:

**Usuario Admin:**
- Usuario: `admin`
- Contraseña: `Admin123!`
- Preguntas de seguridad:
  - Pregunta 1: firulais
  - Pregunta 2: mexico
  - Pregunta 3: azul

**Usuario jperez:**
- Usuario: `jperez`
- Contraseña: `Pass123!`
- Preguntas de seguridad:
  - Pregunta 1: max
  - Pregunta 2: guadalajara
  - Pregunta 3: verde

### PASO 5: Probar el Sistema

1. Vuelve a comentar las líneas en `Program.cs`:
   ```csharp
   // TestConexion.ProbarConexion();
   // return;
   ```

2. Ejecuta el programa normalmente:
   ```
   dotnet run
   ```

3. Prueba:
   - **Login**: admin / Admin123!
   - **Recuperación**: admin con respuestas: firulais, mexico, azul
   - **Registro**: Crea un nuevo usuario

## 🐛 Posibles Causas del Problema

### Causa 1: Base de Datos Vacía
- **Síntoma**: No hay usuarios en la base de datos
- **Solución**: Ejecuta `database_setup.sql` para crear la estructura y datos iniciales

### Causa 2: Usuarios Sin Preguntas de Seguridad
- **Síntoma**: Los usuarios existen pero no tienen preguntas de seguridad
- **Solución**: Ejecuta `actualizar_usuarios.sql`

### Causa 3: Contraseñas Incorrectas
- **Síntoma**: Las contraseñas en la base de datos no coinciden
- **Solución**: Verifica con `test_login.sql` y actualiza si es necesario

### Causa 4: Problema de Conexión
- **Síntoma**: No se puede conectar a Supabase
- **Solución**: Verifica la cadena de conexión en `App.config`

## 📝 Archivos de Ayuda Creados

1. **TestConexion.cs** - Programa para probar la conexión desde C#
2. **test_conexion.sql** - Script para verificar la base de datos
3. **actualizar_usuarios.sql** - Script para agregar preguntas de seguridad
4. **test_login.sql** - Script para verificar credenciales de login

## 🔍 Información de Depuración

### Cadena de Conexión Actual:
```
Host=db.ovfaxfhvcjrvujtgiaaf.supabase.co
Port=5432
Database=postgres
Username=postgres
Password=ModuloBanco2025
SSL Mode=Require
Trust Server Certificate=true
```

### Estructura Esperada de la Tabla usuarios:
- id_usuario (serial)
- usuario (varchar)
- contraseña (varchar)
- nombre_completo (varchar)
- email (varchar)
- estatus (boolean)
- bloqueado_hasta (timestamp)
- intentos_fallidos (integer)
- pregunta_seguridad_1 (text)
- respuesta_seguridad_1 (text)
- pregunta_seguridad_2 (text)
- respuesta_seguridad_2 (text)
- pregunta_seguridad_3 (text)
- respuesta_seguridad_3 (text)
- ultima_sesion (timestamp)
- fecha_registro (timestamp)

## 📞 Siguiente Paso

**EJECUTA EL PASO 1** y comparte los resultados que veas en la consola. Eso me dirá exactamente cuál es el problema.

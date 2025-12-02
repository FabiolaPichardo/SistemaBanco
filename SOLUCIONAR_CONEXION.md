# 🔧 Guía para Solucionar Problemas de Conexión

## ❌ Error: "Host desconocido"

Este error indica que la aplicación no puede conectarse a la base de datos de Supabase.

---

## ✅ Soluciones Paso a Paso

### 1️⃣ Verificar Conexión a Internet
- Asegúrate de tener conexión a Internet activa
- Intenta abrir https://supabase.com en tu navegador
- Si no tienes Internet, conéctate antes de usar la aplicación

### 2️⃣ Verificar Configuración en App.config

Abre el archivo `App.config` y verifica que tenga esta estructura:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <connectionStrings>
    <add name="BancoDB" 
         connectionString="Host=db.ovfaxfhvcjrvujtgiaaf.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=ModuloBanco2025;SSL Mode=Require;Trust Server Certificate=true;Timeout=30;Command Timeout=30;" 
         providerName="Npgsql" />
  </connectionStrings>
</configuration>
```

**Importante:** Verifica que:
- El Host sea: `db.ovfaxfhvcjrvujtgiaaf.supabase.co`
- El Port sea: `5432`
- El Username sea: `postgres`
- El Password sea: `ModuloBanco2025`

### 3️⃣ Verificar que la Base de Datos Esté Configurada

1. Ve a https://supabase.com
2. Inicia sesión en tu proyecto
3. Ve a "SQL Editor"
4. Ejecuta el script `VERIFICAR_CONEXION.sql`
5. Verifica que veas las tablas: `usuarios`, `cuentas`, `movimientos`

### 4️⃣ Ejecutar Script de Actualización

Si la columna `rol` no existe en la tabla usuarios:

1. Abre Supabase SQL Editor
2. Copia y pega el contenido de `EJECUTAR_PRIMERO.sql`
3. Presiona "Run" o "Ejecutar"
4. Verifica que no haya errores

### 5️⃣ Probar Conexión desde la Aplicación

1. Compila el proyecto: `dotnet build`
2. Ejecuta la aplicación
3. Intenta registrar un nuevo usuario
4. Si funciona, el problema está resuelto ✅

---

## 🔍 Diagnóstico Avanzado

### Verificar Firewall
- Asegúrate de que tu firewall no esté bloqueando la conexión al puerto 5432
- Temporalmente desactiva el firewall para probar

### Verificar DNS
- El error "Host desconocido" puede ser un problema de DNS
- Intenta usar Google DNS (8.8.8.8) o Cloudflare DNS (1.1.1.1)

### Verificar Supabase
- Ve a https://status.supabase.com para verificar si hay problemas con el servicio
- Si Supabase está caído, espera a que se restablezca

---

## 📝 Cambios Realizados

### ✅ Formulario de Registro
- ✅ Altura reducida de 800px a 650px
- ✅ ID de usuario se asigna automáticamente (SERIAL)
- ✅ Se incluye el rol al registrar usuario
- ✅ Se muestra el ID de usuario en el mensaje de confirmación

### ✅ Manejo de Errores Mejorado
- ✅ Mensajes de error más claros y específicos
- ✅ Detección de problemas de conexión
- ✅ Detección de problemas de autenticación
- ✅ Detección de problemas de estructura de BD

### ✅ Base de Datos
- ✅ Script de verificación creado (VERIFICAR_CONEXION.sql)
- ✅ Columna `rol` agregada a usuarios
- ✅ Valores por defecto configurados
- ✅ Constraints de validación agregados

---

## 🆘 Si Nada Funciona

1. **Verifica que tengas Internet:** Abre un navegador y visita cualquier sitio web
2. **Reinicia tu computadora:** A veces ayuda reiniciar
3. **Verifica la configuración de Supabase:** Asegúrate de que el proyecto esté activo
4. **Contacta soporte:** Si el problema persiste, puede ser un problema con Supabase

---

## 📞 Información de Contacto

Si necesitas ayuda adicional:
- Revisa la documentación de Supabase: https://supabase.com/docs
- Verifica el estado del servicio: https://status.supabase.com
- Revisa los logs de la aplicación para más detalles

---

## ✨ Próximos Pasos

Una vez que la conexión funcione:

1. ✅ Registra un nuevo usuario
2. ✅ Inicia sesión con ese usuario
3. ✅ Prueba la recuperación de contraseña
4. ✅ Verifica que todos los módulos funcionen correctamente

---

**Última actualización:** Diciembre 2, 2025

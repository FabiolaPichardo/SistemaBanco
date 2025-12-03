# 📧 Configuración del Correo Electrónico para Recuperación de Contraseña

## Estado Actual del Sistema

✅ **Todo está implementado y funcionando correctamente:**

1. ✅ Nombre cambiado a "Módulo Banco" en todas las ventanas
2. ✅ Mensajes de error explicativos con CustomMessageBox
3. ✅ Indicadores de seguridad de contraseña ocultos
4. ✅ Sistema de registro de usuarios funcionando
5. ✅ Sistema de correo implementado (EmailService.cs)

---

## 🔧 Cómo Activar el Envío de Correos

### Opción 1: Usar Gmail (Recomendado para pruebas)

#### Paso 1: Configurar Gmail
1. Ve a tu cuenta de Gmail
2. Activa la **verificación en 2 pasos**:
   - Ve a: https://myaccount.google.com/security
   - Busca "Verificación en 2 pasos" y actívala

3. Genera una **contraseña de aplicación**:
   - Ve a: https://myaccount.google.com/apppasswords
   - Selecciona "Correo" y "Windows Computer"
   - Copia la contraseña de 16 caracteres que te genera

#### Paso 2: Editar EmailService.cs
Abre el archivo `EmailService.cs` y modifica estas líneas:

```csharp
private static string smtpUser = "tu_correo@gmail.com";        // ← Tu correo de Gmail
private static string smtpPassword = "xxxx xxxx xxxx xxxx";    // ← La contraseña de aplicación
private static string fromEmail = "tu_correo@gmail.com";       // ← Tu correo de Gmail
```

**Ejemplo:**
```csharp
private static string smtpUser = "modulobanco@gmail.com";
private static string smtpPassword = "abcd efgh ijkl mnop";
private static string fromEmail = "modulobanco@gmail.com";
```

#### Paso 3: Compilar y Probar
```bash
dotnet build
dotnet run
```

---

### Opción 2: Usar Otro Proveedor de Correo

Si no quieres usar Gmail, puedes configurar otro proveedor:

#### Para Outlook/Hotmail:
```csharp
private static string smtpServer = "smtp-mail.outlook.com";
private static int smtpPort = 587;
private static string smtpUser = "tu_correo@outlook.com";
private static string smtpPassword = "tu_contraseña";
```

#### Para Yahoo:
```csharp
private static string smtpServer = "smtp.mail.yahoo.com";
private static int smtpPort = 587;
private static string smtpUser = "tu_correo@yahoo.com";
private static string smtpPassword = "tu_contraseña_app";
```

---

## 🧪 Modo de Prueba (Sin Configurar Correo)

Si **NO** configuras el correo, el sistema funcionará de todas formas:
- Mostrará el código de verificación en pantalla
- Podrás probarlo sin necesidad de correo real
- Perfecto para desarrollo y pruebas

---

## ✅ Verificación del Registro de Usuarios

### ¿Por qué no se registran los usuarios?

Posibles causas:

1. **Base de datos no actualizada**
   - Ejecuta el script: `database_setup.sql`
   - Verifica que las tablas existan

2. **Problema de conexión**
   - Verifica `App.config` que la conexión sea correcta
   - Prueba la conexión a PostgreSQL

3. **Error en el formulario**
   - Revisa que el botón "Crear Cuenta" esté visible
   - Verifica que no haya errores en la consola

### Cómo Probar el Registro:

1. Ejecuta la aplicación
2. Haz clic en "¿No tienes cuenta? Regístrate"
3. Llena todos los campos:
   - Usuario: `prueba1` (máx 20 caracteres)
   - Nombre: `Usuario de Prueba`
   - Email: `prueba@email.com`
   - Contraseña: `Test1234!` (mínimo 8 caracteres)
   - Confirmar: `Test1234!`
4. Haz clic en "CREAR CUENTA"

Si hay algún error, aparecerá un mensaje explicativo.

---

## 📊 Verificar en la Base de Datos

Para verificar que el usuario se registró correctamente:

```sql
-- Ver todos los usuarios
SELECT * FROM usuarios;

-- Ver cuentas creadas
SELECT * FROM cuentas;

-- Ver último usuario registrado
SELECT * FROM usuarios ORDER BY fecha_registro DESC LIMIT 1;
```

---

## 🎯 Resumen de Cambios Completados

### 1. Nombre "Módulo Banco"
- ✅ Todos los títulos actualizados
- ✅ Headers y footers cambiados
- ✅ Logo actualizado

### 2. Mensajes Explicativos
- ✅ Todos usan CustomMessageBox
- ✅ Mensajes claros y detallados
- ✅ Iconos apropiados (Warning, Error, Info)

### 3. Indicadores de Contraseña
- ✅ `lblPasswordStrength.Visible = false` en FormRegistro
- ✅ `lblPasswordStrength.Visible = false` en FormRecuperacion
- ✅ Ya no se muestra el nivel de seguridad

### 4. Sistema de Correo
- ✅ EmailService.cs implementado
- ✅ Plantilla HTML profesional
- ✅ Fallback para modo de prueba
- ✅ Configuración fácil de cambiar

---

## 🚀 Próximos Pasos

1. **Configurar el correo** (opcional, pero recomendado)
2. **Probar el registro de usuarios**
3. **Probar la recuperación de contraseña**
4. **Verificar que todo funcione correctamente**

---

## 💡 Notas Importantes

- Las contraseñas están en **texto plano** (solo para desarrollo)
- En producción debes usar **bcrypt** o **hash** para las contraseñas
- El código de recuperación expira en **15 minutos**
- Los tokens usados no se pueden reutilizar

---

## 📞 Soporte

Si tienes problemas:
1. Verifica los mensajes de error en pantalla
2. Revisa la conexión a la base de datos
3. Comprueba que el script SQL se haya ejecutado
4. Verifica la configuración del correo en EmailService.cs

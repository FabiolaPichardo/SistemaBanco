# 🧪 Prueba Rápida del Sistema

## ⚡ Verificación Rápida (5 minutos)

### Paso 1: Verificar Conexión a Internet
```
✅ Abre tu navegador
✅ Ve a https://supabase.com
✅ Si carga, tienes Internet ✓
```

### Paso 2: Verificar Base de Datos
1. Inicia sesión en Supabase
2. Ve a tu proyecto
3. Abre "SQL Editor"
4. Ejecuta este comando:

```sql
SELECT COUNT(*) as total_usuarios FROM usuarios;
```

**Resultado esperado:** Un número (puede ser 0 si no hay usuarios)
**Si hay error:** Ejecuta `EJECUTAR_PRIMERO.sql`

---

### Paso 3: Verificar Columna ROL
Ejecuta en Supabase SQL Editor:

```sql
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'usuarios' AND column_name = 'rol';
```

**Resultado esperado:** Debe mostrar "rol"
**Si está vacío:** Ejecuta este comando:

```sql
ALTER TABLE usuarios ADD COLUMN IF NOT EXISTS rol VARCHAR(20) DEFAULT 'Cliente';
```

---

### Paso 4: Compilar Aplicación
```bash
dotnet build
```

**Resultado esperado:** "Compilación correcta"
**Si hay errores:** Revisa los mensajes y corrige

---

### Paso 5: Ejecutar Aplicación
```bash
dotnet run
```

**Resultado esperado:** Se abre la ventana de Login

---

## 🧪 Pruebas Funcionales

### Prueba 1: Registrar Usuario
1. Haz clic en "REGISTRARSE"
2. Llena todos los campos:
   - Email: test@ejemplo.com
   - Rol: Cliente
   - Nombre: Usuario Prueba
   - Usuario: testuser123!
   - Contraseña: Test123!@#
   - Confirmar: Test123!@#
   - Responde las 3 preguntas de seguridad
3. Haz clic en "CONTINUAR"

**Resultado esperado:**
```
✅ Mensaje: "Registrado correctamente"
✅ Muestra: Usuario, ID Usuario, Número de cuenta
✅ Cierra el formulario de registro
```

**Si hay error "Host desconocido":**
- Verifica tu conexión a Internet
- Revisa App.config
- Consulta SOLUCIONAR_CONEXION.md

---

### Prueba 2: Iniciar Sesión
1. En la pantalla de Login
2. Ingresa:
   - Usuario: testuser123!
   - Contraseña: Test123!@#
3. Haz clic en "CONTINUAR"

**Resultado esperado:**
```
✅ Se abre el Dashboard/Menú principal
✅ Muestra el nombre del usuario
✅ Muestra las opciones del menú
```

**Si hay error "Usuario no registrado":**
- Verifica que el registro fue exitoso
- Ejecuta en Supabase: `SELECT * FROM usuarios;`
- Verifica que el usuario existe

---

### Prueba 3: Recuperar Contraseña
1. En Login, haz clic en "¿Olvidaste tu contraseña?"
2. Ingresa: testuser123!
3. Responde las 3 preguntas de seguridad
4. Ingresa nueva contraseña: NewPass123!@#
5. Confirma: NewPass123!@#
6. Haz clic en "CONTINUAR"

**Resultado esperado:**
```
✅ Mensaje: "La contraseña se ha actualizado correctamente"
✅ Muestra el email donde se envió confirmación
✅ Cierra el formulario
```

**Si hay error "Usuario no registrado":**
- Verifica que escribiste bien el usuario
- Ejecuta en Supabase: `SELECT usuario FROM usuarios;`
- Verifica que el usuario existe

---

## 🔍 Verificación en Base de Datos

### Ver usuarios registrados
```sql
SELECT 
    id_usuario,
    usuario,
    nombre_completo,
    email,
    rol,
    estatus,
    fecha_registro
FROM usuarios
ORDER BY id_usuario DESC
LIMIT 10;
```

### Ver cuentas creadas
```sql
SELECT 
    c.id_cuenta,
    u.usuario,
    c.numero_cuenta,
    c.tipo_cuenta,
    c.saldo
FROM cuentas c
JOIN usuarios u ON c.id_usuario = u.id_usuario
ORDER BY c.id_cuenta DESC
LIMIT 10;
```

---

## ✅ Checklist de Verificación

### Antes de Usar
- [ ] Tengo conexión a Internet
- [ ] Supabase está disponible
- [ ] App.config está configurado correctamente
- [ ] Ejecuté EJECUTAR_PRIMERO.sql
- [ ] La columna `rol` existe en usuarios
- [ ] El proyecto compila sin errores

### Funcionalidades Básicas
- [ ] Puedo registrar un usuario nuevo
- [ ] Puedo iniciar sesión
- [ ] Puedo recuperar contraseña
- [ ] Se muestra el Dashboard
- [ ] Puedo consultar saldo
- [ ] Puedo hacer transferencias

### Si Algo Falla
- [ ] Revisé SOLUCIONAR_CONEXION.md
- [ ] Ejecuté VERIFICAR_CONEXION.sql
- [ ] Verifiqué los logs de error
- [ ] Revisé la configuración de Supabase
- [ ] Reinicié la aplicación

---

## 🆘 Errores Comunes

### Error: "Host desconocido"
**Solución rápida:**
1. Verifica Internet: `ping google.com`
2. Verifica App.config tiene el Host correcto
3. Verifica Supabase: https://status.supabase.com

### Error: "Usuario no registrado"
**Solución rápida:**
1. Ejecuta en Supabase: `SELECT * FROM usuarios;`
2. Si está vacío, registra un usuario
3. Verifica que el registro fue exitoso

### Error: "Error en estructura de BD"
**Solución rápida:**
1. Ejecuta EJECUTAR_PRIMERO.sql en Supabase
2. Verifica con: `SELECT column_name FROM information_schema.columns WHERE table_name = 'usuarios';`
3. Debe incluir la columna `rol`

---

## 📞 Soporte

Si después de estas pruebas sigues teniendo problemas:

1. **Revisa los documentos:**
   - SOLUCIONAR_CONEXION.md
   - CAMBIOS_REALIZADOS.md
   - CONFIGURAR_CONEXION.md

2. **Ejecuta los scripts:**
   - VERIFICAR_CONEXION.sql
   - EJECUTAR_PRIMERO.sql

3. **Verifica logs:**
   - Mensajes de error en la aplicación
   - Logs de Supabase
   - Eventos de Windows

---

**Tiempo estimado:** 5-10 minutos
**Última actualización:** Diciembre 2, 2025

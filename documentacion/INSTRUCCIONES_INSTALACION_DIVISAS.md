# 📋 INSTRUCCIONES DE INSTALACIÓN - MÓDULO DE DIVISAS

## ⚠️ IMPORTANTE - LEER ANTES DE USAR

El módulo de **Autorización de Divisas** requiere que se ejecute un script SQL en la base de datos **ANTES** de poder utilizarlo. Si intentas acceder al módulo sin ejecutar el script, verás un error.

## 🔧 Pasos de Instalación

### 1. Ejecutar el Script SQL

Debes ejecutar el archivo `crear_sistema_autorizacion_divisas.sql` en tu base de datos PostgreSQL.

#### Opción A: Usando pgAdmin
1. Abre pgAdmin
2. Conéctate a tu base de datos
3. Haz clic derecho en tu base de datos → Query Tool
4. Abre el archivo `crear_sistema_autorizacion_divisas.sql`
5. Haz clic en el botón "Execute" (▶️)
6. Verifica que no haya errores

#### Opción B: Usando línea de comandos
```bash
psql -U tu_usuario -d nombre_base_datos -f crear_sistema_autorizacion_divisas.sql
```

Reemplaza:
- `tu_usuario` con tu usuario de PostgreSQL
- `nombre_base_datos` con el nombre de tu base de datos

### 2. Verificar la Instalación

Después de ejecutar el script, verifica que las tablas se crearon correctamente:

```sql
-- Verificar que existen las tablas
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name IN (
    'divisas', 
    'roles_autorizadores_divisas', 
    'solicitudes_autorizacion_divisas', 
    'historial_autorizacion_divisas'
);

-- Verificar que hay divisas cargadas
SELECT * FROM divisas;
```

Deberías ver 5 divisas: USD, EUR, GBP, CAD, JPY

### 3. Usar el Módulo

Una vez ejecutado el script:

1. Inicia sesión en el sistema con un usuario que tenga rol **Ejecutivo**, **Gerente** o **Administrador**
2. En el menú principal, haz clic en la tarjeta **"Autorización Divisas"** (💱)
3. El módulo debería abrirse sin errores

## 🎯 Tablas Creadas

El script crea las siguientes tablas:

| Tabla | Descripción |
|-------|-------------|
| `divisas` | Catálogo de divisas disponibles |
| `roles_autorizadores_divisas` | Configuración de roles por divisa |
| `solicitudes_autorizacion_divisas` | Solicitudes de autorización |
| `historial_autorizacion_divisas` | Historial de cambios |

## 🔐 Permisos Necesarios

Para usar el módulo necesitas uno de estos roles:
- **Ejecutivo**: Puede consultar y autorizar hasta $50,000 USD
- **Gerente**: Puede consultar, autorizar hasta $200,000 USD y configurar roles
- **Administrador**: Acceso completo sin límites

## ❌ Solución de Problemas

### Error: "Object reference not set to an instance of an object"
**Causa**: No se ha ejecutado el script SQL  
**Solución**: Ejecuta el script `crear_sistema_autorizacion_divisas.sql`

### Error: "relation 'divisas' does not exist"
**Causa**: El script no se ejecutó correctamente  
**Solución**: Verifica que estás conectado a la base de datos correcta y vuelve a ejecutar el script

### Error: "No se pudieron cargar las divisas"
**Causa**: Problema de conexión a la base de datos  
**Solución**: Verifica tu cadena de conexión en `App.config`

### No aparece la tarjeta "Autorización Divisas"
**Causa**: Tu usuario no tiene los permisos necesarios  
**Solución**: Inicia sesión con un usuario Ejecutivo, Gerente o Administrador

## 📞 Soporte

Si después de seguir estos pasos sigues teniendo problemas:

1. Verifica los logs de PostgreSQL
2. Revisa que tu usuario de base de datos tenga permisos para crear tablas
3. Asegúrate de que la versión de PostgreSQL sea compatible (9.5+)

## ✅ Checklist de Instalación

- [ ] Script SQL ejecutado sin errores
- [ ] Tablas creadas verificadas
- [ ] 5 divisas cargadas en la tabla `divisas`
- [ ] Usuario con rol adecuado (Ejecutivo/Gerente/Administrador)
- [ ] Tarjeta "Autorización Divisas" visible en el menú
- [ ] Módulo abre sin errores

---

**Fecha de creación**: Diciembre 2025  
**Versión**: 1.0

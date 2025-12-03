# Usuarios Sugeridos para Demostración de Eliminación

## Usuarios Recomendados para Eliminar en Demo

### 1. Usuario: demo_cliente1
- **Nombre:** Cliente Demo Uno
- **Rol:** Cliente
- **Email:** demo1@ejemplo.com
- **Características:**
  - Usuario de prueba sin movimientos reales
  - No tiene cuentas activas asociadas
  - Creado específicamente para demostraciones
  - Puede eliminarse sin afectar datos importantes

### 2. Usuario: test_usuario
- **Nombre:** Usuario de Prueba
- **Rol:** Cliente
- **Email:** test@ejemplo.com
- **Características:**
  - Usuario temporal de testing
  - Sin transacciones financieras
  - Sin cuentas bancarias asociadas
  - Ideal para demostrar el proceso de eliminación

## Notas Importantes

⚠️ **ADVERTENCIA:** Antes de eliminar cualquier usuario, el sistema verificará:
- Si tiene cuentas asociadas
- Si tiene movimientos financieros registrados
- Si tiene transacciones pendientes

✅ **Recomendación:** Solo elimine usuarios que:
- No tengan cuentas activas
- No tengan movimientos financieros
- Sean usuarios de prueba o demostración
- No afecten la integridad de los datos del sistema

## Proceso de Eliminación

1. Ir a **Administración de Usuarios**
2. Buscar el usuario a eliminar
3. Hacer clic en el botón **🗑️ Eliminar**
4. El sistema verificará dependencias
5. Confirmar la eliminación
6. La acción quedará registrada en logs de auditoría

## Script SQL para Crear Usuarios de Demo

```sql
-- Crear usuarios de demostración para pruebas de eliminación
INSERT INTO usuarios (usuario, contrasena, nombre_completo, email, rol, estatus)
VALUES 
('demo_cliente1', 'demo123', 'Cliente Demo Uno', 'demo1@ejemplo.com', 'Cliente', TRUE),
('test_usuario', 'test123', 'Usuario de Prueba', 'test@ejemplo.com', 'Cliente', TRUE);
```

## Verificar Usuarios sin Dependencias

```sql
-- Consulta para encontrar usuarios sin cuentas ni movimientos
SELECT u.id_usuario, u.usuario, u.nombre_completo, u.rol
FROM usuarios u
LEFT JOIN cuentas c ON u.id_usuario = c.id_usuario
LEFT JOIN movimientos_financieros mf ON u.id_usuario = mf.id_usuario
WHERE c.id_cuenta IS NULL AND mf.id_movimiento IS NULL
AND u.rol = 'Cliente';
```

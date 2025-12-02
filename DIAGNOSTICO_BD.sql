-- ============================================
-- SCRIPT DE DIAGNÓSTICO
-- ============================================
-- Ejecuta este script para ver el estado actual de tu base de datos

-- 1. Ver si la columna 'rol' existe
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'usuarios'
ORDER BY ordinal_position;

-- 2. Ver todos los usuarios
SELECT id_usuario, usuario, nombre_completo, email, estatus
FROM usuarios;

-- 3. Intentar ver roles (si la columna existe)
-- Si este query falla, significa que la columna 'rol' NO existe
SELECT usuario, nombre_completo, rol
FROM usuarios
ORDER BY usuario;

-- 4. Ver si la tabla logs_auditoria existe
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
AND table_name = 'logs_auditoria';

-- 5. Contar usuarios
SELECT COUNT(*) as total_usuarios FROM usuarios;

-- 6. Ver cuentas
SELECT c.numero_cuenta, u.usuario, c.saldo, c.tipo_cuenta
FROM cuentas c
JOIN usuarios u ON c.id_usuario = u.id_usuario;

-- Script para verificar la conexión y datos en la base de datos
-- Ejecuta esto en tu cliente PostgreSQL (Supabase)

-- 1. Verificar que la tabla usuarios existe
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' AND table_name = 'usuarios';

-- 2. Ver estructura de la tabla usuarios
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'usuarios'
ORDER BY ordinal_position;

-- 3. Contar usuarios en la base de datos
SELECT COUNT(*) as total_usuarios FROM usuarios;

-- 4. Ver todos los usuarios con sus datos básicos
SELECT id_usuario, usuario, nombre_completo, email, estatus, 
       CASE WHEN pregunta_seguridad_1 IS NOT NULL THEN 'SI' ELSE 'NO' END as tiene_preguntas
FROM usuarios;

-- 5. Verificar usuario específico 'admin'
SELECT * FROM usuarios WHERE usuario = 'admin';

-- 6. Verificar si hay usuarios activos
SELECT usuario, estatus, bloqueado_hasta, intentos_fallidos
FROM usuarios
WHERE estatus = TRUE;

-- 7. Ver cuentas bancarias
SELECT c.id_cuenta, c.numero_cuenta, u.usuario, c.saldo
FROM cuentas c
LEFT JOIN usuarios u ON c.id_usuario = u.id_usuario;

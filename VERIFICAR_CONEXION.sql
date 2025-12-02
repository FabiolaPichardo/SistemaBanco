-- ============================================
-- SCRIPT DE VERIFICACIÓN DE CONEXIÓN
-- ============================================
-- Ejecuta este script en Supabase SQL Editor para verificar
-- que la base de datos está configurada correctamente

-- 1. Verificar que la tabla usuarios existe y tiene la estructura correcta
SELECT 
    column_name, 
    data_type, 
    is_nullable, 
    column_default
FROM information_schema.columns
WHERE table_name = 'usuarios'
ORDER BY ordinal_position;

-- 2. Verificar que la columna rol existe
SELECT EXISTS (
    SELECT 1 
    FROM information_schema.columns 
    WHERE table_name = 'usuarios' 
    AND column_name = 'rol'
) as columna_rol_existe;

-- 3. Verificar usuarios existentes
SELECT 
    id_usuario,
    usuario,
    nombre_completo,
    email,
    rol,
    estatus,
    intentos_fallidos,
    bloqueado_hasta,
    fecha_registro
FROM usuarios
ORDER BY id_usuario;

-- 4. Verificar cuentas existentes
SELECT 
    c.id_cuenta,
    c.id_usuario,
    u.usuario,
    c.numero_cuenta,
    c.tipo_cuenta,
    c.saldo,
    c.estatus
FROM cuentas c
LEFT JOIN usuarios u ON c.id_usuario = u.id_usuario
ORDER BY c.id_cuenta;

-- 5. Verificar secuencia de id_usuario
SELECT 
    last_value as ultimo_id_usuario,
    is_called
FROM usuarios_id_usuario_seq;

-- 6. Verificar secuencia de id_cuenta
SELECT 
    last_value as ultimo_id_cuenta,
    is_called
FROM cuentas_id_cuenta_seq;

-- ============================================
-- SI LA COLUMNA ROL NO EXISTE, EJECUTA ESTO:
-- ============================================

-- Agregar columna rol si no existe
ALTER TABLE usuarios 
ADD COLUMN IF NOT EXISTS rol VARCHAR(20) DEFAULT 'Cliente';

-- Agregar constraint
ALTER TABLE usuarios 
DROP CONSTRAINT IF EXISTS usuarios_rol_check;

ALTER TABLE usuarios 
ADD CONSTRAINT usuarios_rol_check 
CHECK (rol IN ('Cliente', 'Cajero', 'Ejecutivo', 'Gerente', 'Administrador'));

-- Actualizar usuarios sin rol
UPDATE usuarios 
SET rol = 'Cliente' 
WHERE rol IS NULL;

-- ============================================
-- RESULTADO ESPERADO
-- ============================================
-- Deberías ver:
-- - Todas las columnas de la tabla usuarios incluyendo 'rol'
-- - Lista de usuarios con sus datos
-- - Lista de cuentas asociadas a usuarios
-- - Secuencias funcionando correctamente
-- ============================================

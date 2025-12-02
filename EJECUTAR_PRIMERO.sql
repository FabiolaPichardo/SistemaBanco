-- ============================================
-- SCRIPT DE ACTUALIZACIÓN - EJECUTAR PRIMERO
-- ============================================
-- Copia y pega este script completo en Supabase SQL Editor
-- y presiona "Run" o "Ejecutar"

-- PASO 1: Agregar columna de rol
ALTER TABLE usuarios 
ADD COLUMN IF NOT EXISTS rol VARCHAR(20) DEFAULT 'Cliente';

-- PASO 2: Eliminar constraint anterior si existe
ALTER TABLE usuarios 
DROP CONSTRAINT IF EXISTS usuarios_rol_check;

-- PASO 3: Agregar nuevo constraint
ALTER TABLE usuarios 
ADD CONSTRAINT usuarios_rol_check 
CHECK (rol IN ('Cliente', 'Cajero', 'Ejecutivo', 'Gerente', 'Administrador'));

-- PASO 4: Actualizar usuarios existentes con roles
UPDATE usuarios SET rol = 'Administrador' WHERE usuario = 'admin';
UPDATE usuarios SET rol = 'Cliente' WHERE usuario IN ('jperez', 'mlopez');
UPDATE usuarios SET rol = 'Cajero' WHERE usuario = 'arodriguez';
UPDATE usuarios SET rol = 'Ejecutivo' WHERE usuario = 'cgarcia';

-- PASO 5: Actualizar usuarios sin rol
UPDATE usuarios SET rol = 'Cliente' WHERE rol IS NULL;

-- PASO 6: Crear índice
CREATE INDEX IF NOT EXISTS idx_usuarios_rol ON usuarios(rol);

-- PASO 7: Crear tabla de logs de auditoría
CREATE TABLE IF NOT EXISTS logs_auditoria (
    id_log SERIAL PRIMARY KEY,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    accion VARCHAR(100) NOT NULL,
    detalle TEXT,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(50)
);

-- PASO 8: Crear índices para logs
CREATE INDEX IF NOT EXISTS idx_logs_usuario ON logs_auditoria(id_usuario);
CREATE INDEX IF NOT EXISTS idx_logs_fecha ON logs_auditoria(fecha DESC);
CREATE INDEX IF NOT EXISTS idx_logs_accion ON logs_auditoria(accion);

-- ============================================
-- VERIFICACIÓN
-- ============================================
-- Estos SELECT te mostrarán si todo funcionó correctamente

-- Ver usuarios con sus roles
SELECT usuario, nombre_completo, rol, estatus 
FROM usuarios 
ORDER BY rol, usuario;

-- Ver estructura de la tabla usuarios
SELECT column_name, data_type, is_nullable, column_default
FROM information_schema.columns
WHERE table_name = 'usuarios' AND column_name = 'rol';

-- Contar logs (debería ser 0 al inicio)
SELECT COUNT(*) as total_logs FROM logs_auditoria;

-- ============================================
-- RESULTADO ESPERADO
-- ============================================
-- Deberías ver:
-- - admin: Administrador
-- - jperez: Cliente
-- - mlopez: Cliente
-- - arodriguez: Cajero
-- - cgarcia: Ejecutivo
-- ============================================

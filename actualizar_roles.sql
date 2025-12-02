-- Script para agregar sistema de roles a la base de datos

-- Agregar columna de rol a la tabla usuarios
ALTER TABLE usuarios 
ADD COLUMN IF NOT EXISTS rol VARCHAR(20) DEFAULT 'Cliente' 
CHECK (rol IN ('Cliente', 'Cajero', 'Ejecutivo', 'Gerente', 'Administrador'));

-- Actualizar usuarios existentes con roles
UPDATE usuarios SET rol = 'Administrador' WHERE usuario = 'admin';
UPDATE usuarios SET rol = 'Cliente' WHERE usuario IN ('jperez', 'mlopez');
UPDATE usuarios SET rol = 'Cajero' WHERE usuario = 'arodriguez';
UPDATE usuarios SET rol = 'Ejecutivo' WHERE usuario = 'cgarcia';

-- Crear índice para optimizar consultas por rol
CREATE INDEX IF NOT EXISTS idx_usuarios_rol ON usuarios(rol);

-- Crear tabla de logs de auditoría para seguridad (BAN-21)
CREATE TABLE IF NOT EXISTS logs_auditoria (
    id_log SERIAL PRIMARY KEY,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    accion VARCHAR(100) NOT NULL,
    detalle TEXT,
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(50)
);

-- Crear índice para optimizar consultas de auditoría
CREATE INDEX IF NOT EXISTS idx_logs_usuario ON logs_auditoria(id_usuario);
CREATE INDEX IF NOT EXISTS idx_logs_fecha ON logs_auditoria(fecha DESC);
CREATE INDEX IF NOT EXISTS idx_logs_accion ON logs_auditoria(accion);

-- Verificar que se aplicaron los cambios
SELECT usuario, nombre_completo, rol FROM usuarios ORDER BY rol, usuario;

-- Verificar que la tabla de logs se creó
SELECT COUNT(*) as total_logs FROM logs_auditoria;

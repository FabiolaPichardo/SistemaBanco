-- ============================================
-- TABLAS: BENEFICIARIOS Y NOTIFICACIONES
-- Implementa BAN-28, BAN-30, BAN-31
-- ============================================

-- BAN-28: Tabla de Beneficiarios
CREATE TABLE IF NOT EXISTS beneficiarios (
    id_beneficiario SERIAL PRIMARY KEY,
    nombre_completo VARCHAR(100) NOT NULL,
    numero_cuenta VARCHAR(50) NOT NULL,
    banco VARCHAR(100),
    rfc VARCHAR(13),
    email VARCHAR(100),
    telefono VARCHAR(20),
    activo BOOLEAN DEFAULT TRUE,
    id_usuario_registro INTEGER REFERENCES usuarios(id_usuario),
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_beneficiarios_nombre ON beneficiarios(nombre_completo);
CREATE INDEX IF NOT EXISTS idx_beneficiarios_cuenta ON beneficiarios(numero_cuenta);

-- Datos de ejemplo
INSERT INTO beneficiarios (nombre_completo, numero_cuenta, banco, rfc, email, telefono, id_usuario_registro)
VALUES 
('Proveedor ABC SA de CV', '014012345678901234', 'Santander', 'ABC123456789', 'contacto@abc.com', '5551234567', 1),
('Cliente XYZ SA de CV', '002123456789012345', 'Banamex', 'XYZ987654321', 'ventas@xyz.com', '5559876543', 1),
('Servicios Generales SA', '012345678901234567', 'BBVA', 'SGE456789012', 'info@servicios.com', '5555555555', 1);

-- BAN-30, BAN-31: Tabla de Notificaciones
CREATE TABLE IF NOT EXISTS notificaciones (
    id_notificacion SERIAL PRIMARY KEY,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    tipo_movimiento VARCHAR(20) NOT NULL,
    monto DECIMAL(15,2) NOT NULL,
    saldo_actualizado DECIMAL(15,2),
    canal VARCHAR(20) CHECK (canal IN ('EMAIL', 'SMS', 'AMBOS')),
    estado VARCHAR(20) DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE', 'ENVIADA', 'FALLIDA')),
    leida BOOLEAN DEFAULT FALSE,
    fecha_envio TIMESTAMP,
    fecha_lectura TIMESTAMP,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_notif_usuario ON notificaciones(id_usuario);
CREATE INDEX IF NOT EXISTS idx_notif_fecha ON notificaciones(created_at DESC);
CREATE INDEX IF NOT EXISTS idx_notif_leida ON notificaciones(leida);

-- BAN-31: Tabla de Configuración de Notificaciones
CREATE TABLE IF NOT EXISTS configuracion_notificaciones (
    id_config SERIAL PRIMARY KEY,
    id_usuario INTEGER UNIQUE REFERENCES usuarios(id_usuario),
    notificaciones_activas BOOLEAN DEFAULT TRUE,
    canal_preferido VARCHAR(20) DEFAULT 'EMAIL' CHECK (canal_preferido IN ('EMAIL', 'SMS', 'AMBOS')),
    notificar_cargos BOOLEAN DEFAULT TRUE,
    notificar_abonos BOOLEAN DEFAULT TRUE,
    monto_minimo_notificacion DECIMAL(15,2) DEFAULT 0.00,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Configuración por defecto para usuarios existentes
INSERT INTO configuracion_notificaciones (id_usuario, notificaciones_activas, canal_preferido)
SELECT id_usuario, TRUE, 'EMAIL'
FROM usuarios
WHERE id_usuario NOT IN (SELECT id_usuario FROM configuracion_notificaciones WHERE id_usuario IS NOT NULL);

-- BAN-29: Tabla de Historial de Cambios de Estado
CREATE TABLE IF NOT EXISTS historial_movimientos (
    id_historial SERIAL PRIMARY KEY,
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    estado_anterior VARCHAR(20),
    estado_nuevo VARCHAR(20) NOT NULL,
    motivo TEXT,
    id_usuario_accion INTEGER REFERENCES usuarios(id_usuario),
    tipo_accion VARCHAR(50) CHECK (tipo_accion IN ('CREACION', 'AUTORIZACION', 'RECHAZO', 'ANULACION', 'PROCESAMIENTO')),
    fecha_accion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_historial_movimiento ON historial_movimientos(id_movimiento);
CREATE INDEX IF NOT EXISTS idx_historial_fecha ON historial_movimientos(fecha_accion DESC);

-- Trigger para registrar cambios de estado (BAN-29)
CREATE OR REPLACE FUNCTION registrar_cambio_estado()
RETURNS TRIGGER AS $$
BEGIN
    IF OLD.estado IS DISTINCT FROM NEW.estado THEN
        INSERT INTO historial_movimientos (id_movimiento, estado_anterior, estado_nuevo, id_usuario_accion, tipo_accion)
        VALUES (NEW.id_movimiento, OLD.estado, NEW.estado, NEW.id_usuario, 
                CASE NEW.estado
                    WHEN 'PROCESADO' THEN 'PROCESAMIENTO'
                    WHEN 'RECHAZADO' THEN 'RECHAZO'
                    WHEN 'ANULADO' THEN 'ANULACION'
                    ELSE 'AUTORIZACION'
                END);
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_cambio_estado
AFTER UPDATE ON movimientos_financieros
FOR EACH ROW
EXECUTE FUNCTION registrar_cambio_estado();

-- Agregar columna de estado ANULADO si no existe
ALTER TABLE movimientos_financieros 
DROP CONSTRAINT IF EXISTS movimientos_financieros_estado_check;

ALTER TABLE movimientos_financieros 
ADD CONSTRAINT movimientos_financieros_estado_check 
CHECK (estado IN ('PENDIENTE', 'PROCESADO', 'RECHAZADO', 'ANULADO'));

-- Agregar campos para anulación (BAN-29)
ALTER TABLE movimientos_financieros 
ADD COLUMN IF NOT EXISTS motivo_anulacion TEXT,
ADD COLUMN IF NOT EXISTS usuario_anulacion INTEGER REFERENCES usuarios(id_usuario),
ADD COLUMN IF NOT EXISTS fecha_anulacion TIMESTAMP;

-- Vista completa de movimientos con historial (BAN-29)
CREATE OR REPLACE VIEW vista_movimientos_completa AS
SELECT 
    mf.id_movimiento,
    mf.folio,
    mf.fecha_hora,
    mf.tipo_operacion,
    mf.beneficiario,
    mf.importe,
    mf.moneda,
    mf.estado,
    mf.concepto,
    u_registro.nombre_completo as usuario_registro,
    u_autorizacion.nombre_completo as usuario_autorizacion,
    u_anulacion.nombre_completo as usuario_anulacion,
    mf.motivo_anulacion,
    mf.fecha_anulacion,
    COUNT(h.id_historial) as total_cambios
FROM movimientos_financieros mf
LEFT JOIN usuarios u_registro ON mf.id_usuario = u_registro.id_usuario
LEFT JOIN usuarios u_autorizacion ON mf.usuario_autorizacion = u_autorizacion.id_usuario
LEFT JOIN usuarios u_anulacion ON mf.usuario_anulacion = u_anulacion.id_usuario
LEFT JOIN historial_movimientos h ON mf.id_movimiento = h.id_movimiento
GROUP BY mf.id_movimiento, u_registro.nombre_completo, u_autorizacion.nombre_completo, u_anulacion.nombre_completo
ORDER BY mf.fecha_hora DESC;

SELECT 'Tablas de beneficiarios, notificaciones e historial creadas exitosamente!' as mensaje;

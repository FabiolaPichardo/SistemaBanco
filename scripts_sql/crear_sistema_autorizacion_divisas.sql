-- =====================================================
-- SISTEMA DE AUTORIZACIÓN DE OPERACIONES EN DIVISAS
-- =====================================================
-- Este script crea las tablas necesarias para gestionar
-- autorizaciones de transacciones en moneda extranjera
-- =====================================================

-- Tabla de configuración de divisas
CREATE TABLE IF NOT EXISTS divisas (
    id_divisa SERIAL PRIMARY KEY,
    codigo VARCHAR(3) NOT NULL UNIQUE, -- USD, EUR, GBP, etc.
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(5) NOT NULL,
    tasa_cambio DECIMAL(10, 4) NOT NULL DEFAULT 1.0,
    activa BOOLEAN DEFAULT TRUE,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_tasa_positiva CHECK (tasa_cambio > 0)
);

-- Tabla de configuración de roles autorizadores por divisa
CREATE TABLE IF NOT EXISTS roles_autorizadores_divisas (
    id_config SERIAL PRIMARY KEY,
    id_divisa INTEGER NOT NULL REFERENCES divisas(id_divisa),
    rol VARCHAR(50) NOT NULL,
    monto_minimo DECIMAL(15, 2) DEFAULT 0,
    monto_maximo DECIMAL(15, 2),
    activo BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT chk_montos CHECK (monto_minimo >= 0 AND (monto_maximo IS NULL OR monto_maximo >= monto_minimo))
);

-- Tabla de solicitudes de autorización de divisas
CREATE TABLE IF NOT EXISTS solicitudes_autorizacion_divisas (
    id_solicitud SERIAL PRIMARY KEY,
    id_transaccion VARCHAR(50) UNIQUE NOT NULL,
    id_cuenta INTEGER NOT NULL REFERENCES cuentas(id_cuenta),
    id_usuario_solicitante INTEGER NOT NULL REFERENCES usuarios(id_usuario),
    id_divisa INTEGER NOT NULL REFERENCES divisas(id_divisa),
    descripcion TEXT,
    monto_mxn DECIMAL(15, 2) NOT NULL,
    tasa_cambio DECIMAL(10, 4) NOT NULL,
    monto_divisa DECIMAL(15, 2) NOT NULL,
    estado VARCHAR(20) DEFAULT 'Pendiente',
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP,
    id_usuario_autorizador INTEGER REFERENCES usuarios(id_usuario),
    fecha_autorizacion TIMESTAMP,
    comentarios_autorizacion TEXT,
    motivo_rechazo TEXT,
    CONSTRAINT chk_estado CHECK (estado IN ('Pendiente', 'En Revisión', 'Autorizada', 'Rechazada', 'Expirada')),
    CONSTRAINT chk_montos_positivos CHECK (monto_mxn > 0 AND monto_divisa > 0 AND tasa_cambio > 0)
);

-- Tabla de historial de cambios de estado
CREATE TABLE IF NOT EXISTS historial_autorizacion_divisas (
    id_historial SERIAL PRIMARY KEY,
    id_solicitud INTEGER NOT NULL REFERENCES solicitudes_autorizacion_divisas(id_solicitud),
    estado_anterior VARCHAR(20),
    estado_nuevo VARCHAR(20) NOT NULL,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    comentario TEXT,
    fecha_cambio TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices para mejorar rendimiento
CREATE INDEX IF NOT EXISTS idx_solicitudes_estado ON solicitudes_autorizacion_divisas(estado);
CREATE INDEX IF NOT EXISTS idx_solicitudes_fecha ON solicitudes_autorizacion_divisas(fecha_solicitud);
CREATE INDEX IF NOT EXISTS idx_solicitudes_divisa ON solicitudes_autorizacion_divisas(id_divisa);
CREATE INDEX IF NOT EXISTS idx_solicitudes_cuenta ON solicitudes_autorizacion_divisas(id_cuenta);
CREATE INDEX IF NOT EXISTS idx_historial_solicitud ON historial_autorizacion_divisas(id_solicitud);

-- Insertar divisas principales
INSERT INTO divisas (codigo, nombre, simbolo, tasa_cambio) VALUES
('USD', 'Dólar Estadounidense', '$', 17.50),
('EUR', 'Euro', '€', 19.20),
('GBP', 'Libra Esterlina', '£', 22.30),
('CAD', 'Dólar Canadiense', 'C$', 13.10),
('JPY', 'Yen Japonés', '¥', 0.12)
ON CONFLICT (codigo) DO NOTHING;

-- Configuración inicial de roles autorizadores
-- Ejecutivos pueden autorizar hasta $50,000 USD
INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Ejecutivo', 0, 50000 
FROM divisas 
WHERE codigo IN ('USD', 'EUR', 'GBP', 'CAD', 'JPY')
ON CONFLICT DO NOTHING;

-- Gerentes pueden autorizar hasta $200,000 USD
INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Gerente', 0, 200000 
FROM divisas 
WHERE codigo IN ('USD', 'EUR', 'GBP', 'CAD', 'JPY')
ON CONFLICT DO NOTHING;

-- Administradores pueden autorizar cualquier monto
INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Administrador', 0, NULL 
FROM divisas 
WHERE codigo IN ('USD', 'EUR', 'GBP', 'CAD', 'JPY')
ON CONFLICT DO NOTHING;

-- Función para actualizar estado a Expirada automáticamente
CREATE OR REPLACE FUNCTION actualizar_solicitudes_expiradas()
RETURNS void AS $$
BEGIN
    UPDATE solicitudes_autorizacion_divisas
    SET estado = 'Expirada'
    WHERE estado IN ('Pendiente', 'En Revisión')
    AND fecha_expiracion IS NOT NULL
    AND fecha_expiracion < CURRENT_TIMESTAMP;
END;
$$ LANGUAGE plpgsql;

-- Trigger para registrar cambios en el historial
CREATE OR REPLACE FUNCTION registrar_cambio_estado_divisa()
RETURNS TRIGGER AS $$
BEGIN
    IF (TG_OP = 'UPDATE' AND OLD.estado != NEW.estado) THEN
        INSERT INTO historial_autorizacion_divisas (
            id_solicitud, 
            estado_anterior, 
            estado_nuevo, 
            id_usuario,
            comentario
        ) VALUES (
            NEW.id_solicitud,
            OLD.estado,
            NEW.estado,
            NEW.id_usuario_autorizador,
            NEW.comentarios_autorizacion
        );
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_historial_estado_divisa
AFTER UPDATE ON solicitudes_autorizacion_divisas
FOR EACH ROW
EXECUTE FUNCTION registrar_cambio_estado_divisa();

-- Vista para consultas simplificadas
CREATE OR REPLACE VIEW vista_solicitudes_divisas AS
SELECT 
    s.id_solicitud,
    s.id_transaccion,
    c.numero_cuenta,
    u_sol.nombre_completo AS solicitante,
    d.codigo AS divisa,
    d.nombre AS nombre_divisa,
    d.simbolo,
    s.descripcion,
    s.monto_mxn,
    s.tasa_cambio,
    s.monto_divisa,
    s.estado,
    s.fecha_solicitud,
    s.fecha_expiracion,
    u_aut.nombre_completo AS autorizador,
    s.fecha_autorizacion,
    s.comentarios_autorizacion,
    s.motivo_rechazo,
    CASE 
        WHEN s.fecha_expiracion IS NOT NULL AND s.fecha_expiracion < CURRENT_TIMESTAMP 
        THEN TRUE 
        ELSE FALSE 
    END AS esta_expirada
FROM solicitudes_autorizacion_divisas s
INNER JOIN cuentas c ON s.id_cuenta = c.id_cuenta
INNER JOIN usuarios u_sol ON s.id_usuario_solicitante = u_sol.id_usuario
INNER JOIN divisas d ON s.id_divisa = d.id_divisa
LEFT JOIN usuarios u_aut ON s.id_usuario_autorizador = u_aut.id_usuario;

-- Comentarios para documentación
COMMENT ON TABLE divisas IS 'Catálogo de divisas disponibles para operaciones';
COMMENT ON TABLE roles_autorizadores_divisas IS 'Configuración de roles autorizados por divisa y rango de montos';
COMMENT ON TABLE solicitudes_autorizacion_divisas IS 'Solicitudes de autorización para operaciones en divisas';
COMMENT ON TABLE historial_autorizacion_divisas IS 'Historial de cambios de estado de solicitudes';

COMMENT ON COLUMN solicitudes_autorizacion_divisas.estado IS 'Estados: Pendiente, En Revisión, Autorizada, Rechazada, Expirada';
COMMENT ON COLUMN solicitudes_autorizacion_divisas.monto_mxn IS 'Monto en pesos mexicanos';
COMMENT ON COLUMN solicitudes_autorizacion_divisas.monto_divisa IS 'Monto convertido en la divisa extranjera';
COMMENT ON COLUMN solicitudes_autorizacion_divisas.tasa_cambio IS 'Tasa de cambio aplicada al momento de la solicitud';

-- Mensaje de confirmación
DO $$
BEGIN
    RAISE NOTICE 'Sistema de autorización de divisas creado exitosamente';
    RAISE NOTICE 'Tablas creadas: divisas, roles_autorizadores_divisas, solicitudes_autorizacion_divisas, historial_autorizacion_divisas';
    RAISE NOTICE 'Vista creada: vista_solicitudes_divisas';
END $$;

-- ============================================
-- TABLA: MOVIMIENTOS FINANCIEROS
-- Implementa BAN-23, BAN-24, BAN-25, BAN-26
-- ============================================

CREATE TABLE IF NOT EXISTS movimientos_financieros (
    id_movimiento SERIAL PRIMARY KEY,
    folio VARCHAR(50) UNIQUE NOT NULL,                    -- BAN-23: Folio único
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,       -- BAN-26: Fecha/hora automática
    tipo_operacion VARCHAR(20) NOT NULL CHECK (tipo_operacion IN ('CARGO', 'ABONO')), -- BAN-25
    cuenta_ordenante VARCHAR(100) NOT NULL,               -- BAN-26
    cuenta_beneficiaria VARCHAR(50) NOT NULL,             -- BAN-26
    beneficiario VARCHAR(100) NOT NULL,                   -- BAN-26
    importe DECIMAL(15,2) NOT NULL CHECK (importe > 0),   -- BAN-26
    moneda VARCHAR(3) NOT NULL DEFAULT 'MXN',             -- BAN-26
    concepto TEXT NOT NULL,                               -- BAN-26
    referencia VARCHAR(50),                               -- BAN-26: PO/Factura
    cuenta_contable VARCHAR(100) NOT NULL,                -- BAN-26: Catálogo ERP
    estado VARCHAR(20) DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE', 'PROCESADO', 'RECHAZADO', 'ELIMINADO')), -- BAN-23, BAN-44
    id_usuario INTEGER REFERENCES usuarios(id_usuario),  -- BAN-26: Usuario responsable
    fecha_procesamiento TIMESTAMP,
    usuario_autorizacion INTEGER REFERENCES usuarios(id_usuario),
    comentarios_autorizacion TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices para optimización
CREATE INDEX IF NOT EXISTS idx_movfin_folio ON movimientos_financieros(folio);
CREATE INDEX IF NOT EXISTS idx_movfin_fecha ON movimientos_financieros(fecha_hora DESC);
CREATE INDEX IF NOT EXISTS idx_movfin_tipo ON movimientos_financieros(tipo_operacion);
CREATE INDEX IF NOT EXISTS idx_movfin_estado ON movimientos_financieros(estado);
CREATE INDEX IF NOT EXISTS idx_movfin_usuario ON movimientos_financieros(id_usuario);

-- Trigger para actualizar updated_at
CREATE OR REPLACE FUNCTION actualizar_movfin_timestamp()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_movfin_timestamp
BEFORE UPDATE ON movimientos_financieros
FOR EACH ROW
EXECUTE FUNCTION actualizar_movfin_timestamp();

-- Vista para auditoría y reportes
CREATE OR REPLACE VIEW vista_movimientos_financieros AS
SELECT 
    mf.id_movimiento,
    mf.folio,
    mf.fecha_hora,
    mf.tipo_operacion,
    mf.cuenta_ordenante,
    mf.cuenta_beneficiaria,
    mf.beneficiario,
    mf.importe,
    mf.moneda,
    mf.concepto,
    mf.referencia,
    mf.cuenta_contable,
    mf.estado,
    u.nombre_completo as usuario_registro,
    u.usuario as username_registro,
    ua.nombre_completo as usuario_autorizacion,
    mf.fecha_procesamiento,
    mf.comentarios_autorizacion
FROM movimientos_financieros mf
LEFT JOIN usuarios u ON mf.id_usuario = u.id_usuario
LEFT JOIN usuarios ua ON mf.usuario_autorizacion = ua.id_usuario
ORDER BY mf.fecha_hora DESC;

-- Datos de ejemplo
INSERT INTO movimientos_financieros 
(folio, tipo_operacion, cuenta_ordenante, cuenta_beneficiaria, beneficiario, 
 importe, moneda, concepto, referencia, cuenta_contable, estado, id_usuario)
VALUES 
('MOV-20251202120000', 'CARGO', 'BBVA - 012345678901234567', '014012345678901234', 
 'Proveedor ABC SA de CV', 15000.00, 'MXN', 'Pago de factura servicios', 'FAC-2024-001', 
 '5101 - Gastos Operativos', 'PROCESADO', 1),
 
('MOV-20251202120100', 'ABONO', 'Santander - 014012345678901234', '002123456789012345', 
 'Cliente XYZ SA de CV', 25000.00, 'MXN', 'Pago de cliente por servicios', 'PO-2024-050', 
 '4101 - Ventas', 'PROCESADO', 1);

SELECT 'Tabla movimientos_financieros creada exitosamente!' as mensaje;
SELECT COUNT(*) as total_movimientos FROM movimientos_financieros;


-- ============================================
-- BAN-44: TABLA DE AUDITORÍA
-- Historial de cambios en movimientos
-- ============================================

CREATE TABLE IF NOT EXISTS historial_movimientos (
    id_historial SERIAL PRIMARY KEY,
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    folio VARCHAR(50) NOT NULL,
    accion VARCHAR(50) NOT NULL, -- 'CREADO', 'EDITADO', 'ELIMINADO', 'ESTADO_CAMBIADO'
    campo_modificado VARCHAR(100),
    valor_anterior TEXT,
    valor_nuevo TEXT,
    usuario VARCHAR(100) NOT NULL,
    fecha_accion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    comentarios TEXT
);

CREATE INDEX IF NOT EXISTS idx_historial_folio ON historial_movimientos(folio);
CREATE INDEX IF NOT EXISTS idx_historial_fecha ON historial_movimientos(fecha_accion DESC);

-- Trigger para auditoría automática
CREATE OR REPLACE FUNCTION registrar_auditoria_movimiento()
RETURNS TRIGGER AS $$
BEGIN
    IF (TG_OP = 'UPDATE') THEN
        -- Registrar cambio de estado
        IF OLD.estado != NEW.estado THEN
            INSERT INTO historial_movimientos (id_movimiento, folio, accion, campo_modificado, valor_anterior, valor_nuevo, usuario, comentarios)
            VALUES (NEW.id_movimiento, NEW.folio, 'ESTADO_CAMBIADO', 'estado', OLD.estado, NEW.estado, CURRENT_USER, NEW.comentarios_autorizacion);
        END IF;
        
        -- Registrar cambio de concepto
        IF OLD.concepto != NEW.concepto THEN
            INSERT INTO historial_movimientos (id_movimiento, folio, accion, campo_modificado, valor_anterior, valor_nuevo, usuario)
            VALUES (NEW.id_movimiento, NEW.folio, 'EDITADO', 'concepto', OLD.concepto, NEW.concepto, CURRENT_USER);
        END IF;
        
        -- Registrar cambio de referencia
        IF OLD.referencia != NEW.referencia THEN
            INSERT INTO historial_movimientos (id_movimiento, folio, accion, campo_modificado, valor_anterior, valor_nuevo, usuario)
            VALUES (NEW.id_movimiento, NEW.folio, 'EDITADO', 'referencia', OLD.referencia, NEW.referencia, CURRENT_USER);
        END IF;
    END IF;
    
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_auditoria_movimientos
AFTER UPDATE ON movimientos_financieros
FOR EACH ROW
EXECUTE FUNCTION registrar_auditoria_movimiento();

SELECT 'Sistema de auditoría configurado exitosamente!' as mensaje;

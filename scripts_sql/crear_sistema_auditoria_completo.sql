-- ============================================================================
-- SISTEMA DE AUDITORÍA Y SEGURIDAD COMPLETO (BAN-56 a BAN-59)
-- ============================================================================

-- ============================================================================
-- BAN-56: TABLA DE AUDITORÍA DEL SISTEMA
-- ============================================================================
CREATE TABLE IF NOT EXISTS auditoria_sistema (
    id_auditoria SERIAL PRIMARY KEY,
    usuario VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    accion VARCHAR(100) NOT NULL,
    detalles TEXT,
    fecha_hora TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(50),
    nombre_equipo VARCHAR(100),
    tipo_movimiento VARCHAR(50),
    protegido BOOLEAN DEFAULT TRUE, -- Registro inalterable
    INDEX idx_auditoria_usuario (usuario),
    INDEX idx_auditoria_fecha (fecha_hora),
    INDEX idx_auditoria_accion (accion)
);

-- Trigger para proteger registros de auditoría (inalterables)
CREATE OR REPLACE FUNCTION proteger_auditoria()
RETURNS TRIGGER AS $$
BEGIN
    IF TG_OP = 'UPDATE' OR TG_OP = 'DELETE' THEN
        RAISE EXCEPTION 'Los registros de auditoría no pueden ser modificados o eliminados';
    END IF;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_proteger_auditoria
BEFORE UPDATE OR DELETE ON auditoria_sistema
FOR EACH ROW
EXECUTE FUNCTION proteger_auditoria();

-- ============================================================================
-- BAN-57: TABLA DE ALERTAS DE ACTIVIDAD SOSPECHOSA
-- ============================================================================
CREATE TABLE IF NOT EXISTS alertas_sospechosas (
    id_alerta SERIAL PRIMARY KEY,
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    nombre_titular VARCHAR(200) NOT NULL,
    rfc VARCHAR(20),
    monto DECIMAL(15,2) NOT NULL,
    tipo_alerta VARCHAR(100) NOT NULL, -- 'MONTO_ATIPICO', 'TRANSACCIONES_REPETITIVAS', 'PATRON_INUSUAL'
    descripcion TEXT,
    estado VARCHAR(50) DEFAULT 'Abierta', -- 'Abierta', 'En revisión', 'Escalada', 'Cerrada'
    es_falso_positivo BOOLEAN DEFAULT FALSE,
    fecha_alerta TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP, -- SLA para atender la alerta
    fecha_cierre TIMESTAMP,
    usuario_cierre VARCHAR(100),
    comentarios_cierre TEXT,
    notificado_finanzas BOOLEAN DEFAULT FALSE,
    fecha_notificacion TIMESTAMP,
    INDEX idx_alertas_estado (estado),
    INDEX idx_alertas_fecha (fecha_alerta),
    INDEX idx_alertas_titular (nombre_titular)
);

-- ============================================================================
-- BAN-58: TABLA DE AUTORIZACIONES DE DIVISAS
-- ============================================================================
CREATE TABLE IF NOT EXISTS autorizaciones_divisas (
    id_autorizacion SERIAL PRIMARY KEY,
    id_transaccion INTEGER REFERENCES movimientos_financieros(id_movimiento),
    descripcion TEXT,
    nombre_titular VARCHAR(200) NOT NULL,
    divisa VARCHAR(10) NOT NULL, -- 'USD', 'EUR', 'GBP', etc.
    tasa_cambio DECIMAL(10,4) NOT NULL,
    monto_mxn DECIMAL(15,2) NOT NULL,
    monto_divisa DECIMAL(15,2) NOT NULL,
    estado VARCHAR(50) DEFAULT 'Pendiente', -- 'Pendiente', 'En revisión', 'Autorizada', 'Rechazada', 'Expirada'
    fecha_solicitud TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP, -- SLA para autorizar
    fecha_resolucion TIMESTAMP,
    usuario_autorizador VARCHAR(100),
    rol_autorizador VARCHAR(50),
    comentarios TEXT,
    INDEX idx_autorizaciones_estado (estado),
    INDEX idx_autorizaciones_divisa (divisa),
    INDEX idx_autorizaciones_fecha (fecha_solicitud)
);

-- Tabla de configuración de roles autorizadores por divisa
CREATE TABLE IF NOT EXISTS config_autorizadores_divisas (
    id_config SERIAL PRIMARY KEY,
    divisa VARCHAR(10) NOT NULL UNIQUE,
    roles_autorizados TEXT[], -- Array de roles que pueden autorizar
    monto_minimo_autorizacion DECIMAL(15,2) DEFAULT 0,
    requiere_doble_autorizacion BOOLEAN DEFAULT FALSE,
    activo BOOLEAN DEFAULT TRUE
);

-- Insertar configuración por defecto
INSERT INTO config_autorizadores_divisas (divisa, roles_autorizados, monto_minimo_autorizacion)
VALUES 
    ('USD', ARRAY['Gerente', 'Administrador'], 10000.00),
    ('EUR', ARRAY['Gerente', 'Administrador'], 10000.00),
    ('GBP', ARRAY['Gerente', 'Administrador'], 10000.00),
    ('JPY', ARRAY['Gerente', 'Administrador'], 10000.00),
    ('CAD', ARRAY['Gerente', 'Administrador'], 10000.00)
ON CONFLICT (divisa) DO NOTHING;

-- ============================================================================
-- BAN-59: TABLA DE LÍMITES DE TRANSACCIÓN
-- ============================================================================
CREATE TABLE IF NOT EXISTS limites_transaccion (
    id_limite SERIAL PRIMARY KEY,
    id_cuenta INTEGER REFERENCES cuentas(id_cuenta),
    moneda VARCHAR(10) NOT NULL DEFAULT 'MXN',
    limite_diario DECIMAL(15,2) NOT NULL,
    limite_mensual DECIMAL(15,2) NOT NULL,
    accion_exceso VARCHAR(50) DEFAULT 'RECHAZAR', -- 'RECHAZAR' o 'AUTORIZAR'
    activo BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(id_cuenta, moneda)
);

-- Tabla de seguimiento de transacciones contra límites
CREATE TABLE IF NOT EXISTS seguimiento_limites (
    id_seguimiento SERIAL PRIMARY KEY,
    id_cuenta INTEGER REFERENCES cuentas(id_cuenta),
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    moneda VARCHAR(10) NOT NULL,
    monto DECIMAL(15,2) NOT NULL,
    limite_aplicado DECIMAL(15,2) NOT NULL,
    excede_limite BOOLEAN DEFAULT FALSE,
    accion_tomada VARCHAR(50), -- 'APROBADO', 'RECHAZADO', 'ENVIADO_AUTORIZACION'
    fecha_transaccion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_seguimiento_cuenta (id_cuenta),
    INDEX idx_seguimiento_fecha (fecha_transaccion)
);

-- ============================================================================
-- BAN-60: TABLA DE LOGS DE INTEGRACIÓN API
-- ============================================================================
CREATE TABLE IF NOT EXISTS logs_integracion_api (
    id_log SERIAL PRIMARY KEY,
    modulo_origen VARCHAR(50) NOT NULL, -- 'ERP', 'CRM', 'PROVEEDORES'
    endpoint VARCHAR(200) NOT NULL,
    metodo VARCHAR(10) NOT NULL, -- 'GET', 'POST', 'PUT', 'DELETE'
    parametros TEXT,
    respuesta TEXT,
    codigo_estado INTEGER,
    tiempo_respuesta_ms INTEGER,
    ip_cliente VARCHAR(50),
    token_autorizacion VARCHAR(100),
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    exitoso BOOLEAN DEFAULT TRUE,
    mensaje_error TEXT,
    INDEX idx_logs_api_modulo (modulo_origen),
    INDEX idx_logs_api_fecha (fecha_hora),
    INDEX idx_logs_api_exitoso (exitoso)
);

-- Tabla de tokens de API para módulos externos
CREATE TABLE IF NOT EXISTS tokens_api (
    id_token SERIAL PRIMARY KEY,
    modulo VARCHAR(50) NOT NULL UNIQUE,
    token VARCHAR(200) NOT NULL UNIQUE,
    descripcion TEXT,
    permisos TEXT[], -- Array de permisos: 'READ_SALDOS', 'READ_MOVIMIENTOS', etc.
    activo BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP,
    ultimo_uso TIMESTAMP,
    ip_permitidas TEXT[] -- Array de IPs autorizadas
);

-- ============================================================================
-- VISTAS ÚTILES PARA REPORTES
-- ============================================================================

-- Vista de auditoría con información resumida
CREATE OR REPLACE VIEW v_auditoria_resumen AS
SELECT 
    DATE(fecha_hora) as fecha,
    usuario,
    accion,
    COUNT(*) as total_acciones,
    COUNT(DISTINCT ip_address) as ips_distintas
FROM auditoria_sistema
GROUP BY DATE(fecha_hora), usuario, accion
ORDER BY fecha DESC, total_acciones DESC;

-- Vista de alertas activas
CREATE OR REPLACE VIEW v_alertas_activas AS
SELECT 
    a.*,
    m.folio,
    m.tipo_operacion,
    CASE 
        WHEN a.fecha_expiracion < CURRENT_TIMESTAMP THEN 'VENCIDA'
        ELSE a.estado
    END as estado_actual
FROM alertas_sospechosas a
LEFT JOIN movimientos_financieros m ON a.id_movimiento = m.id_movimiento
WHERE a.estado != 'Cerrada'
ORDER BY a.fecha_alerta DESC;

-- Vista de autorizaciones pendientes
CREATE OR REPLACE VIEW v_autorizaciones_pendientes AS
SELECT 
    a.*,
    m.folio,
    CASE 
        WHEN a.fecha_expiracion < CURRENT_TIMESTAMP THEN 'EXPIRADA'
        ELSE a.estado
    END as estado_actual,
    EXTRACT(EPOCH FROM (a.fecha_expiracion - CURRENT_TIMESTAMP))/3600 as horas_restantes
FROM autorizaciones_divisas a
LEFT JOIN movimientos_financieros m ON a.id_transaccion = m.id_movimiento
WHERE a.estado IN ('Pendiente', 'En revisión')
ORDER BY a.fecha_solicitud ASC;

-- ============================================================================
-- FUNCIONES ÚTILES
-- ============================================================================

-- Función para verificar si una transacción excede límites
CREATE OR REPLACE FUNCTION verificar_limite_transaccion(
    p_id_cuenta INTEGER,
    p_moneda VARCHAR,
    p_monto DECIMAL
) RETURNS TABLE(
    excede_limite BOOLEAN,
    limite_aplicado DECIMAL,
    accion_recomendada VARCHAR
) AS $$
DECLARE
    v_limite_diario DECIMAL;
    v_suma_dia DECIMAL;
    v_accion VARCHAR;
BEGIN
    -- Obtener límite configurado
    SELECT limite_diario, accion_exceso 
    INTO v_limite_diario, v_accion
    FROM limites_transaccion
    WHERE id_cuenta = p_id_cuenta 
    AND moneda = p_moneda 
    AND activo = TRUE;
    
    -- Si no hay límite configurado, permitir
    IF v_limite_diario IS NULL THEN
        RETURN QUERY SELECT FALSE, 0::DECIMAL, 'APROBADO'::VARCHAR;
        RETURN;
    END IF;
    
    -- Calcular suma de transacciones del día
    SELECT COALESCE(SUM(monto), 0)
    INTO v_suma_dia
    FROM seguimiento_limites
    WHERE id_cuenta = p_id_cuenta
    AND moneda = p_moneda
    AND DATE(fecha_transaccion) = CURRENT_DATE
    AND accion_tomada = 'APROBADO';
    
    -- Verificar si excede
    IF (v_suma_dia + p_monto) > v_limite_diario THEN
        RETURN QUERY SELECT TRUE, v_limite_diario, v_accion;
    ELSE
        RETURN QUERY SELECT FALSE, v_limite_diario, 'APROBADO'::VARCHAR;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Función para detectar patrones sospechosos
CREATE OR REPLACE FUNCTION detectar_patron_sospechoso(
    p_id_cuenta INTEGER,
    p_monto DECIMAL,
    p_tipo_operacion VARCHAR
) RETURNS BOOLEAN AS $$
DECLARE
    v_promedio_historico DECIMAL;
    v_desviacion DECIMAL;
    v_transacciones_similares INTEGER;
BEGIN
    -- Calcular promedio histórico
    SELECT AVG(importe), STDDEV(importe)
    INTO v_promedio_historico, v_desviacion
    FROM movimientos_financieros
    WHERE cuenta_ordenante IN (
        SELECT numero_cuenta FROM cuentas WHERE id_cuenta = p_id_cuenta
    )
    AND tipo_operacion = p_tipo_operacion
    AND fecha >= CURRENT_DATE - INTERVAL '90 days';
    
    -- Si el monto es 3 veces la desviación estándar, es sospechoso
    IF p_monto > (v_promedio_historico + (3 * v_desviacion)) THEN
        RETURN TRUE;
    END IF;
    
    -- Verificar transacciones repetitivas (más de 5 en 1 hora)
    SELECT COUNT(*)
    INTO v_transacciones_similares
    FROM movimientos_financieros
    WHERE cuenta_ordenante IN (
        SELECT numero_cuenta FROM cuentas WHERE id_cuenta = p_id_cuenta
    )
    AND tipo_operacion = p_tipo_operacion
    AND fecha >= CURRENT_TIMESTAMP - INTERVAL '1 hour';
    
    IF v_transacciones_similares > 5 THEN
        RETURN TRUE;
    END IF;
    
    RETURN FALSE;
END;
$$ LANGUAGE plpgsql;

-- ============================================================================
-- ÍNDICES ADICIONALES PARA RENDIMIENTO
-- ============================================================================
CREATE INDEX IF NOT EXISTS idx_movimientos_fecha ON movimientos_financieros(fecha);
CREATE INDEX IF NOT EXISTS idx_movimientos_cuenta ON movimientos_financieros(cuenta_ordenante);
CREATE INDEX IF NOT EXISTS idx_movimientos_tipo ON movimientos_financieros(tipo_operacion);
CREATE INDEX IF NOT EXISTS idx_movimientos_estado ON movimientos_financieros(estado);

-- ============================================================================
-- COMENTARIOS EN TABLAS
-- ============================================================================
COMMENT ON TABLE auditoria_sistema IS 'Registro completo de auditoría del sistema (BAN-56)';
COMMENT ON TABLE alertas_sospechosas IS 'Alertas de actividad sospechosa (BAN-57)';
COMMENT ON TABLE autorizaciones_divisas IS 'Flujo de autorización para divisas extranjeras (BAN-58)';
COMMENT ON TABLE limites_transaccion IS 'Límites máximos de transacción por cuenta y moneda (BAN-59)';
COMMENT ON TABLE logs_integracion_api IS 'Logs de integración con módulos externos (BAN-60)';

-- ============================================================================
-- PERMISOS (ajustar según necesidad)
-- ============================================================================
-- GRANT SELECT ON auditoria_sistema TO auditor_role;
-- GRANT SELECT ON alertas_sospechosas TO finanzas_role;
-- GRANT SELECT, UPDATE ON autorizaciones_divisas TO gerente_role;

COMMIT;

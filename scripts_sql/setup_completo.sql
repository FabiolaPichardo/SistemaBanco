-- ============================================
-- MÓDULO BANCO - BASE DE DATOS
-- Estructuta creada con PostgreSQL 
-- ============================================

-- ============================================
-- 1. TABLAS PRINCIPALES
-- ============================================

DROP TABLE IF EXISTS tokens_recuperacion CASCADE;
DROP TABLE IF EXISTS movimientos CASCADE;
DROP TABLE IF EXISTS cuentas CASCADE;
DROP TABLE IF EXISTS usuarios CASCADE;

CREATE TABLE usuarios (
    id_usuario SERIAL PRIMARY KEY,
    usuario VARCHAR(20) UNIQUE NOT NULL,
    contraseña VARCHAR(255) NOT NULL,
    nombre_completo VARCHAR(100) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    telefono VARCHAR(20),
    rol VARCHAR(20) DEFAULT 'Cliente' CHECK (rol IN ('Cliente', 'Cajero', 'Ejecutivo', 'Gerente', 'Administrador')),
    pregunta_seguridad_1 TEXT,
    respuesta_seguridad_1 TEXT,
    pregunta_seguridad_2 TEXT,
    respuesta_seguridad_2 TEXT,
    pregunta_seguridad_3 TEXT,
    respuesta_seguridad_3 TEXT,
    estatus BOOLEAN DEFAULT TRUE,
    fecha_registro TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ultima_sesion TIMESTAMP,
    intentos_fallidos INTEGER DEFAULT 0,
    bloqueado_hasta TIMESTAMP
);

CREATE TABLE cuentas (
    id_cuenta SERIAL PRIMARY KEY,
    id_usuario INTEGER NOT NULL REFERENCES usuarios(id_usuario) ON DELETE CASCADE,
    numero_cuenta VARCHAR(20) UNIQUE NOT NULL,
    tipo_cuenta VARCHAR(20) DEFAULT 'AHORRO' CHECK (tipo_cuenta IN ('AHORRO', 'CORRIENTE', 'INVERSION')),
    saldo DECIMAL(15,2) DEFAULT 0.00 CHECK (saldo >= 0),
    limite_retiro_diario DECIMAL(15,2) DEFAULT 10000.00,
    estatus BOOLEAN DEFAULT TRUE,
    fecha_apertura TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE movimientos (
    id_movimiento SERIAL PRIMARY KEY,
    id_cuenta INTEGER NOT NULL REFERENCES cuentas(id_cuenta) ON DELETE CASCADE,
    tipo VARCHAR(50) NOT NULL CHECK (tipo IN ('DEPOSITO', 'RETIRO', 'CARGO', 'ABONO', 'TRANSFERENCIA ENVIADA', 'TRANSFERENCIA RECIBIDA')),
    monto DECIMAL(15,2) NOT NULL CHECK (monto > 0),
    concepto TEXT,
    saldo_anterior DECIMAL(15,2) NOT NULL,
    saldo_nuevo DECIMAL(15,2) NOT NULL,
    referencia VARCHAR(50),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ============================================
-- 2. MOVIMIENTOS FINANCIEROS
-- ============================================

CREATE TABLE movimientos_financieros (
    id_movimiento SERIAL PRIMARY KEY,
    folio VARCHAR(50) UNIQUE NOT NULL,
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    tipo_operacion VARCHAR(20) NOT NULL CHECK (tipo_operacion IN ('CARGO', 'ABONO')),
    cuenta_ordenante VARCHAR(100) NOT NULL,
    cuenta_beneficiaria VARCHAR(50) NOT NULL,
    beneficiario VARCHAR(100) NOT NULL,
    importe DECIMAL(15,2) NOT NULL CHECK (importe > 0),
    moneda VARCHAR(3) NOT NULL DEFAULT 'MXN',
    concepto TEXT NOT NULL,
    referencia VARCHAR(50),
    cuenta_contable VARCHAR(100) NOT NULL,
    estado VARCHAR(20) DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE', 'PROCESADO', 'RECHAZADO', 'ELIMINADO')),
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    fecha_procesamiento TIMESTAMP,
    usuario_autorizacion INTEGER REFERENCES usuarios(id_usuario),
    comentarios_autorizacion TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE historial_movimientos (
    id_historial SERIAL PRIMARY KEY,
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    folio VARCHAR(50) NOT NULL,
    accion VARCHAR(50) NOT NULL,
    campo_modificado VARCHAR(100),
    valor_anterior TEXT,
    valor_nuevo TEXT,
    usuario VARCHAR(100) NOT NULL,
    fecha_accion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    comentarios TEXT
);

-- ============================================
-- 3. SISTEMA DE AUDITORÍA
-- ============================================

CREATE TABLE auditoria (
    id_auditoria SERIAL PRIMARY KEY,
    id_usuario INTEGER REFERENCES usuarios(id_usuario),
    accion VARCHAR(100) NOT NULL,
    detalles TEXT,
    fecha_hora TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ip_address VARCHAR(50),
    nombre_equipo VARCHAR(100)
);

CREATE TABLE alertas_sospechosas (
    id_alerta SERIAL PRIMARY KEY,
    id_movimiento INTEGER REFERENCES movimientos_financieros(id_movimiento),
    nombre_titular VARCHAR(200) NOT NULL,
    rfc VARCHAR(20),
    monto DECIMAL(15,2) NOT NULL,
    tipo_alerta VARCHAR(100) NOT NULL,
    descripcion TEXT,
    estado VARCHAR(50) DEFAULT 'Abierta',
    fecha_alerta TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_cierre TIMESTAMP,
    usuario_cierre VARCHAR(100),
    comentarios_cierre TEXT
);

-- ============================================
-- 4. SISTEMA DE DIVISAS
-- ============================================

CREATE TABLE divisas (
    id_divisa SERIAL PRIMARY KEY,
    codigo VARCHAR(3) NOT NULL UNIQUE,
    nombre VARCHAR(50) NOT NULL,
    simbolo VARCHAR(5) NOT NULL,
    tasa_cambio DECIMAL(10, 4) NOT NULL DEFAULT 1.0,
    activa BOOLEAN DEFAULT TRUE,
    fecha_actualizacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE solicitudes_autorizacion_divisas (
    id_solicitud SERIAL PRIMARY KEY,
    id_transaccion VARCHAR(50) UNIQUE NOT NULL,
    id_cuenta INTEGER NOT NULL REFERENCES cuentas(id_cuenta),
    id_usuario_solicitante INTEGER NOT NULL REFERENCES usuarios(id_usuario),
    id_divisa INTEGER NOT NULL REFERENCES divisas(id_divisa),
    descripcion TEXT,
    monto_mxn DECIMAL(15, 2) NOT NULL,
    tasa_cambio DECIMAL(10, 4) NOT NULL,
    monto_divisa DECIMAL(15, 2) NOT NULL,
    estado VARCHAR(20) DEFAULT 'Pendiente' CHECK (estado IN ('Pendiente', 'En Revisión', 'Autorizada', 'Rechazada', 'Expirada')),
    fecha_solicitud TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP,
    id_usuario_autorizador INTEGER REFERENCES usuarios(id_usuario),
    fecha_autorizacion TIMESTAMP,
    comentarios_autorizacion TEXT,
    motivo_rechazo TEXT
);

CREATE TABLE roles_autorizadores_divisas (
    id_config SERIAL PRIMARY KEY,
    id_divisa INTEGER NOT NULL REFERENCES divisas(id_divisa),
    rol VARCHAR(50) NOT NULL,
    monto_minimo DECIMAL(15, 2) DEFAULT 0,
    monto_maximo DECIMAL(15, 2),
    activo BOOLEAN DEFAULT TRUE
);

-- ============================================
-- 5. ÍNDICES
-- ============================================

CREATE INDEX idx_usuarios_usuario ON usuarios(usuario);
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_rol ON usuarios(rol);
CREATE INDEX idx_cuentas_usuario ON cuentas(id_usuario);
CREATE INDEX idx_cuentas_numero ON cuentas(numero_cuenta);
CREATE INDEX idx_movimientos_cuenta ON movimientos(id_cuenta);
CREATE INDEX idx_movimientos_fecha ON movimientos(fecha DESC);
CREATE INDEX idx_movfin_folio ON movimientos_financieros(folio);
CREATE INDEX idx_movfin_fecha ON movimientos_financieros(fecha_hora DESC);
CREATE INDEX idx_movfin_estado ON movimientos_financieros(estado);
CREATE INDEX idx_auditoria_usuario ON auditoria(id_usuario);
CREATE INDEX idx_auditoria_fecha ON auditoria(fecha_hora DESC);
CREATE INDEX idx_solicitudes_estado ON solicitudes_autorizacion_divisas(estado);
CREATE INDEX idx_solicitudes_fecha ON solicitudes_autorizacion_divisas(fecha_solicitud);

-- ============================================
-- 6. DATOS INICIALES
-- ============================================

-- Usuarios de prueba
INSERT INTO usuarios (usuario, contraseña, nombre_completo, email, rol, pregunta_seguridad_1, respuesta_seguridad_1, pregunta_seguridad_2, respuesta_seguridad_2, pregunta_seguridad_3, respuesta_seguridad_3) VALUES
('admin', 'Admin123!', 'Administrador del Sistema', 'admin@banco.com', 'Administrador', '¿Cuál es tu color favorito?', 'azul', '¿En qué ciudad naciste?', 'mexico', '¿Cuál es tu comida favorita?', 'pizza'),
('jperez', 'Pass123!', 'Juan Pérez García', 'jperez@email.com', 'Cliente', '¿Cuál es tu color favorito?', 'verde', '¿En qué ciudad naciste?', 'guadalajara', '¿Cuál es tu comida favorita?', 'tacos'),
('mlopez', 'Pass123!', 'María López Hernández', 'mlopez@email.com', 'Cliente', '¿Cuál es tu color favorito?', 'rosa', '¿En qué ciudad naciste?', 'monterrey', '¿Cuál es tu comida favorita?', 'sushi'),
('cgarcia', 'Pass123!', 'Carlos García Martínez', 'cgarcia@email.com', 'Ejecutivo', '¿Cuál es tu color favorito?', 'negro', '¿En qué ciudad naciste?', 'puebla', '¿Cuál es tu comida favorita?', 'pasta'),
('arodriguez', 'Pass123!', 'Ana Rodríguez Sánchez', 'arodriguez@email.com', 'Cajero', '¿Cuál es tu color favorito?', 'morado', '¿En qué ciudad naciste?', 'queretaro', '¿Cuál es tu comida favorita?', 'ensalada');

-- Cuentas
INSERT INTO cuentas (id_usuario, numero_cuenta, tipo_cuenta, saldo) VALUES
(1, '1001234567', 'CORRIENTE', 50000.00),
(2, '1001234568', 'AHORRO', 25000.00),
(3, '1001234569', 'AHORRO', 15000.00),
(4, '1001234570', 'CORRIENTE', 75000.00),
(5, '1001234571', 'AHORRO', 10000.00);

-- Divisas
INSERT INTO divisas (codigo, nombre, simbolo, tasa_cambio) VALUES
('USD', 'Dólar Estadounidense', '$', 17.50),
('EUR', 'Euro', '€', 19.20),
('GBP', 'Libra Esterlina', '£', 22.30),
('CAD', 'Dólar Canadiense', 'C$', 13.10),
('JPY', 'Yen Japonés', '¥', 0.12);

-- Configuración de roles autorizadores
INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Ejecutivo', 0, 50000 FROM divisas;

INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Gerente', 0, 200000 FROM divisas;

INSERT INTO roles_autorizadores_divisas (id_divisa, rol, monto_minimo, monto_maximo) 
SELECT id_divisa, 'Administrador', 0, NULL FROM divisas;

-- ============================================
-- 7. TRIGGERS
-- ============================================

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

-- ============================================
-- VERIFICACIÓN
-- ============================================

SELECT 'Base de datos configurada exitosamente' as mensaje;
SELECT 'Usuarios creados: ' || COUNT(*) FROM usuarios;
SELECT 'Cuentas creadas: ' || COUNT(*) FROM cuentas;
SELECT 'Divisas configuradas: ' || COUNT(*) FROM divisas;

-- ============================================
-- BANCO PREMIER - SCRIPT DE BASE DE DATOS
-- Sistema Bancario Profesional
-- ============================================

-- Crear la base de datos (ejecutar como superusuario)
-- CREATE DATABASE banco_db;
-- \c banco_db;

-- ============================================
-- TABLA: USUARIOS
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
    bloqueado_hasta TIMESTAMP,
    CONSTRAINT chk_usuario_length CHECK (LENGTH(usuario) <= 20)
);

-- Tabla para tokens de recuperación de contraseña
CREATE TABLE tokens_recuperacion (
    id_token SERIAL PRIMARY KEY,
    id_usuario INTEGER NOT NULL REFERENCES usuarios(id_usuario) ON DELETE CASCADE,
    token VARCHAR(100) UNIQUE NOT NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_expiracion TIMESTAMP NOT NULL,
    usado BOOLEAN DEFAULT FALSE
);

-- Índices para optimización
CREATE INDEX idx_usuarios_usuario ON usuarios(usuario);
CREATE INDEX idx_usuarios_email ON usuarios(email);
CREATE INDEX idx_usuarios_estatus ON usuarios(estatus);
CREATE INDEX idx_tokens_token ON tokens_recuperacion(token);
CREATE INDEX idx_tokens_usuario ON tokens_recuperacion(id_usuario);

-- ============================================
-- TABLA: CUENTAS
-- ============================================
CREATE TABLE cuentas (
    id_cuenta SERIAL PRIMARY KEY,
    id_usuario INTEGER NOT NULL REFERENCES usuarios(id_usuario) ON DELETE CASCADE,
    numero_cuenta VARCHAR(20) UNIQUE NOT NULL,
    tipo_cuenta VARCHAR(20) DEFAULT 'AHORRO' CHECK (tipo_cuenta IN ('AHORRO', 'CORRIENTE', 'INVERSION')),
    saldo DECIMAL(15,2) DEFAULT 0.00 CHECK (saldo >= 0),
    limite_retiro_diario DECIMAL(15,2) DEFAULT 10000.00,
    estatus BOOLEAN DEFAULT TRUE,
    fecha_apertura TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_cierre TIMESTAMP
);

-- Índices para optimización
CREATE INDEX idx_cuentas_usuario ON cuentas(id_usuario);
CREATE INDEX idx_cuentas_numero ON cuentas(numero_cuenta);
CREATE INDEX idx_cuentas_estatus ON cuentas(estatus);

-- ============================================
-- TABLA: MOVIMIENTOS
-- ============================================
CREATE TABLE movimientos (
    id_movimiento SERIAL PRIMARY KEY,
    id_cuenta INTEGER NOT NULL REFERENCES cuentas(id_cuenta) ON DELETE CASCADE,
    tipo VARCHAR(50) NOT NULL CHECK (tipo IN (
        'DEPOSITO', 
        'RETIRO', 
        'CARGO', 
        'ABONO', 
        'TRANSFERENCIA ENVIADA', 
        'TRANSFERENCIA RECIBIDA'
    )),
    monto DECIMAL(15,2) NOT NULL CHECK (monto > 0),
    concepto TEXT,
    saldo_anterior DECIMAL(15,2) NOT NULL,
    saldo_nuevo DECIMAL(15,2) NOT NULL,
    referencia VARCHAR(50),
    fecha TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Índices para optimización
CREATE INDEX idx_movimientos_cuenta ON movimientos(id_cuenta);
CREATE INDEX idx_movimientos_fecha ON movimientos(fecha DESC);
CREATE INDEX idx_movimientos_tipo ON movimientos(tipo);

-- ============================================
-- DATOS DE PRUEBA
-- ============================================

-- Usuarios de prueba (contraseñas en texto plano para desarrollo - en producción usar bcrypt)
INSERT INTO usuarios (usuario, contraseña, nombre_completo, email, telefono, 
                     pregunta_seguridad_1, respuesta_seguridad_1,
                     pregunta_seguridad_2, respuesta_seguridad_2,
                     pregunta_seguridad_3, respuesta_seguridad_3) VALUES
('admin', 'Admin123!', 'Administrador del Sistema', 'admin@bancopremier.com', '555-0001',
 '¿Cuál es el nombre de tu primera mascota?', 'firulais',
 '¿En qué ciudad naciste?', 'mexico',
 '¿Cuál es tu color favorito?', 'azul'),
('jperez', 'Pass123!', 'Juan Pérez García', 'jperez@email.com', '555-1234',
 '¿Cuál es el nombre de tu primera mascota?', 'max',
 '¿En qué ciudad naciste?', 'guadalajara',
 '¿Cuál es tu color favorito?', 'verde'),
('mlopez', 'Pass123!', 'María López Hernández', 'mlopez@email.com', '555-5678',
 '¿Cuál es el nombre de tu primera mascota?', 'luna',
 '¿En qué ciudad naciste?', 'monterrey',
 '¿Cuál es tu color favorito?', 'rosa'),
('cgarcia', 'Pass123!', 'Carlos García Martínez', 'cgarcia@email.com', '555-9012',
 '¿Cuál es el nombre de tu primera mascota?', 'rocky',
 '¿En qué ciudad naciste?', 'puebla',
 '¿Cuál es tu color favorito?', 'negro'),
('arodriguez', 'Pass123!', 'Ana Rodríguez Sánchez', 'arodriguez@email.com', '555-3456',
 '¿Cuál es el nombre de tu primera mascota?', 'mimi',
 '¿En qué ciudad naciste?', 'queretaro',
 '¿Cuál es tu color favorito?', 'morado');

-- Cuentas de prueba
INSERT INTO cuentas (id_usuario, numero_cuenta, tipo_cuenta, saldo, limite_retiro_diario) VALUES
(1, '1001234567', 'CORRIENTE', 50000.00, 20000.00),
(2, '1001234568', 'AHORRO', 25000.00, 10000.00),
(3, '1001234569', 'AHORRO', 15000.00, 10000.00),
(4, '1001234570', 'CORRIENTE', 75000.00, 30000.00),
(5, '1001234571', 'INVERSION', 100000.00, 50000.00);

-- Movimientos de prueba (historial)
INSERT INTO movimientos (id_cuenta, tipo, monto, concepto, saldo_anterior, saldo_nuevo, fecha) VALUES
-- Cuenta 1 (admin)
(1, 'DEPOSITO', 10000.00, 'Depósito inicial', 40000.00, 50000.00, CURRENT_TIMESTAMP - INTERVAL '30 days'),
(1, 'RETIRO', 5000.00, 'Retiro en cajero automático', 50000.00, 45000.00, CURRENT_TIMESTAMP - INTERVAL '25 days'),
(1, 'DEPOSITO', 10000.00, 'Depósito en ventanilla', 45000.00, 55000.00, CURRENT_TIMESTAMP - INTERVAL '20 days'),
(1, 'CARGO', 5000.00, 'Pago de servicios', 55000.00, 50000.00, CURRENT_TIMESTAMP - INTERVAL '15 days'),

-- Cuenta 2 (jperez)
(2, 'DEPOSITO', 5000.00, 'Depósito inicial', 20000.00, 25000.00, CURRENT_TIMESTAMP - INTERVAL '28 days'),
(2, 'RETIRO', 2000.00, 'Retiro en cajero', 25000.00, 23000.00, CURRENT_TIMESTAMP - INTERVAL '22 days'),
(2, 'DEPOSITO', 4000.00, 'Nómina', 23000.00, 27000.00, CURRENT_TIMESTAMP - INTERVAL '18 days'),
(2, 'CARGO', 2000.00, 'Pago de tarjeta', 27000.00, 25000.00, CURRENT_TIMESTAMP - INTERVAL '10 days'),

-- Cuenta 3 (mlopez)
(3, 'DEPOSITO', 15000.00, 'Apertura de cuenta', 0.00, 15000.00, CURRENT_TIMESTAMP - INTERVAL '35 days'),
(3, 'RETIRO', 3000.00, 'Retiro en ventanilla', 15000.00, 12000.00, CURRENT_TIMESTAMP - INTERVAL '20 days'),
(3, 'DEPOSITO', 3000.00, 'Depósito', 12000.00, 15000.00, CURRENT_TIMESTAMP - INTERVAL '12 days'),

-- Cuenta 4 (cgarcia)
(4, 'DEPOSITO', 50000.00, 'Apertura de cuenta empresarial', 25000.00, 75000.00, CURRENT_TIMESTAMP - INTERVAL '40 days'),
(4, 'CARGO', 15000.00, 'Pago a proveedores', 75000.00, 60000.00, CURRENT_TIMESTAMP - INTERVAL '30 days'),
(4, 'DEPOSITO', 30000.00, 'Ingreso por ventas', 60000.00, 90000.00, CURRENT_TIMESTAMP - INTERVAL '20 days'),
(4, 'RETIRO', 15000.00, 'Retiro para nómina', 90000.00, 75000.00, CURRENT_TIMESTAMP - INTERVAL '10 days'),

-- Cuenta 5 (arodriguez)
(5, 'DEPOSITO', 100000.00, 'Inversión inicial', 0.00, 100000.00, CURRENT_TIMESTAMP - INTERVAL '45 days'),
(5, 'ABONO', 5000.00, 'Rendimientos de inversión', 100000.00, 105000.00, CURRENT_TIMESTAMP - INTERVAL '30 days'),
(5, 'RETIRO', 5000.00, 'Retiro parcial', 105000.00, 100000.00, CURRENT_TIMESTAMP - INTERVAL '15 days');

-- ============================================
-- VISTAS ÚTILES
-- ============================================

-- Vista de resumen de cuentas
CREATE OR REPLACE VIEW vista_resumen_cuentas AS
SELECT 
    u.nombre_completo,
    u.usuario,
    c.numero_cuenta,
    c.tipo_cuenta,
    c.saldo,
    c.limite_retiro_diario,
    c.fecha_apertura,
    COUNT(m.id_movimiento) as total_movimientos
FROM usuarios u
INNER JOIN cuentas c ON u.id_usuario = c.id_usuario
LEFT JOIN movimientos m ON c.id_cuenta = m.id_cuenta
WHERE u.estatus = TRUE AND c.estatus = TRUE
GROUP BY u.nombre_completo, u.usuario, c.numero_cuenta, c.tipo_cuenta, 
         c.saldo, c.limite_retiro_diario, c.fecha_apertura
ORDER BY c.saldo DESC;

-- Vista de movimientos recientes
CREATE OR REPLACE VIEW vista_movimientos_recientes AS
SELECT 
    u.nombre_completo,
    c.numero_cuenta,
    m.tipo,
    m.monto,
    m.concepto,
    m.saldo_nuevo,
    m.fecha
FROM movimientos m
INNER JOIN cuentas c ON m.id_cuenta = c.id_cuenta
INNER JOIN usuarios u ON c.id_usuario = u.id_usuario
WHERE m.fecha >= CURRENT_DATE - INTERVAL '30 days'
ORDER BY m.fecha DESC;

-- ============================================
-- FUNCIONES ÚTILES
-- ============================================

-- Función para obtener el saldo de una cuenta
CREATE OR REPLACE FUNCTION obtener_saldo(p_numero_cuenta VARCHAR)
RETURNS DECIMAL(15,2) AS $$
DECLARE
    v_saldo DECIMAL(15,2);
BEGIN
    SELECT saldo INTO v_saldo
    FROM cuentas
    WHERE numero_cuenta = p_numero_cuenta AND estatus = TRUE;
    
    RETURN COALESCE(v_saldo, 0);
END;
$$ LANGUAGE plpgsql;

-- Función para validar límite de retiro diario
CREATE OR REPLACE FUNCTION validar_limite_retiro(
    p_id_cuenta INTEGER,
    p_monto DECIMAL
)
RETURNS BOOLEAN AS $$
DECLARE
    v_limite DECIMAL(15,2);
    v_total_retiros DECIMAL(15,2);
BEGIN
    -- Obtener límite de retiro
    SELECT limite_retiro_diario INTO v_limite
    FROM cuentas
    WHERE id_cuenta = p_id_cuenta;
    
    -- Calcular total de retiros del día
    SELECT COALESCE(SUM(monto), 0) INTO v_total_retiros
    FROM movimientos
    WHERE id_cuenta = p_id_cuenta
    AND tipo IN ('RETIRO', 'TRANSFERENCIA ENVIADA')
    AND DATE(fecha) = CURRENT_DATE;
    
    -- Validar si el nuevo retiro excede el límite
    RETURN (v_total_retiros + p_monto) <= v_limite;
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- TRIGGERS
-- ============================================

-- Trigger para actualizar última sesión
CREATE OR REPLACE FUNCTION actualizar_ultima_sesion()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE usuarios
    SET ultima_sesion = CURRENT_TIMESTAMP
    WHERE id_usuario = NEW.id_usuario;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- ============================================
-- CONSULTAS ÚTILES PARA ADMINISTRACIÓN
-- ============================================

-- Ver resumen de todas las cuentas
-- SELECT * FROM vista_resumen_cuentas;

-- Ver movimientos recientes
-- SELECT * FROM vista_movimientos_recientes LIMIT 20;

-- Total de dinero en el banco
-- SELECT SUM(saldo) as total_banco FROM cuentas WHERE estatus = TRUE;

-- Cuentas con mayor saldo
-- SELECT u.nombre_completo, c.numero_cuenta, c.saldo 
-- FROM cuentas c
-- INNER JOIN usuarios u ON c.id_usuario = u.id_usuario
-- WHERE c.estatus = TRUE
-- ORDER BY c.saldo DESC
-- LIMIT 10;

-- Movimientos por tipo en el último mes
-- SELECT tipo, COUNT(*) as cantidad, SUM(monto) as total
-- FROM movimientos
-- WHERE fecha >= CURRENT_DATE - INTERVAL '30 days'
-- GROUP BY tipo
-- ORDER BY total DESC;

-- ============================================
-- PERMISOS (Opcional - ajustar según necesidad)
-- ============================================

-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO banco_user;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO banco_user;

-- ============================================
-- FIN DEL SCRIPT
-- ============================================

SELECT 'Base de datos configurada exitosamente!' as mensaje;
SELECT 'Total de usuarios: ' || COUNT(*) FROM usuarios;
SELECT 'Total de cuentas: ' || COUNT(*) FROM cuentas;
SELECT 'Total de movimientos: ' || COUNT(*) FROM movimientos;

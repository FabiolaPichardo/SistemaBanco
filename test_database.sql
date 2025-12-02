-- Script para verificar la base de datos
-- Ejecuta esto en tu cliente PostgreSQL para verificar

-- 1. Verificar que la tabla usuarios tenga las columnas de preguntas de seguridad
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'usuarios' 
ORDER BY ordinal_position;

-- 2. Ver los usuarios existentes
SELECT usuario, email, 
       CASE WHEN pregunta_seguridad_1 IS NULL THEN 'NO' ELSE 'SI' END as tiene_preguntas
FROM usuarios;

-- 3. Ver un usuario específico con sus preguntas
SELECT usuario, contraseña, email, 
       pregunta_seguridad_1, respuesta_seguridad_1,
       pregunta_seguridad_2, respuesta_seguridad_2,
       pregunta_seguridad_3, respuesta_seguridad_3
FROM usuarios 
WHERE usuario = 'admin';

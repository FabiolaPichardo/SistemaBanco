# 🧪 GUÍA DE PRUEBAS - REQUERIMIENTOS BAN-41 A BAN-50

## ✅ CHECKLIST DE PRUEBAS

### Preparación
- [ ] Scripts SQL ejecutados en Supabase
- [ ] Proyecto compilado sin errores
- [ ] Usuario con rol "Gerente" o "Administrador" creado
- [ ] Al menos 25 movimientos de prueba en la BD

---

## 🔍 BAN-41: DETALLES EXPANDIBLES

### Pasos de Prueba:
1. Abrir módulo "Revisión de Movimientos"
2. Hacer **doble clic** en cualquier fila
3. Verificar que se abre modal con detalles

### ✅ Criterios de Aceptación:
- [ ] Modal se abre correctamente
- [ ] Muestra todos los campos: folio, fecha, tipo, cuentas, beneficiario, importe, moneda, concepto, referencia, cuenta contable, estado
- [ ] Diseño profesional con encabezado azul
- [ ] Botón "Cerrar" funciona correctamente

---

## 📄 BAN-42: COMPROBANTE PDF

### Pasos de Prueba:
1. Abrir detalles de un movimiento (doble clic)
2. Hacer clic en botón "📄 Descargar Comprobante PDF"
3. Verificar que se genera el archivo

### ✅ Criterios de Aceptación:
- [ ] Archivo .txt se genera en la carpeta del proyecto
- [ ] Se abre automáticamente en Notepad
- [ ] Contiene todos los datos del movimiento
- [ ] Formato profesional con separadores
- [ ] Incluye fecha de generación y usuario

---

## ✏️ BAN-43: EDICIÓN DE MOVIMIENTOS

### Pasos de Prueba:
1. Iniciar sesión como Gerente o Administrador
2. Abrir detalles de un movimiento
3. Hacer clic en botón "✏️ Editar"
4. Modificar concepto, referencia o estado
5. Agregar comentarios
6. Guardar cambios

### ✅ Criterios de Aceptación:
- [ ] Botón "Editar" habilitado solo para usuarios autorizados
- [ ] Formulario de edición se abre correctamente
- [ ] Campos editables: concepto, referencia, estado, comentarios
- [ ] Cambios se guardan en la BD
- [ ] Tabla se actualiza automáticamente
- [ ] Mensaje de éxito se muestra

### Prueba Negativa:
- [ ] Iniciar sesión como "Cajero" o "Analista"
- [ ] Verificar que botón "Editar" está deshabilitado

---

## 🗑️ BAN-44: ELIMINACIÓN CON AUDITORÍA

### Pasos de Prueba:
1. Iniciar sesión como Gerente o Administrador
2. Abrir detalles de un movimiento
3. Hacer clic en botón "🗑️ Eliminar"
4. Confirmar eliminación
5. Verificar en BD que estado cambió a "ELIMINADO"

### ✅ Criterios de Aceptación:
- [ ] Botón "Eliminar" habilitado solo para usuarios autorizados
- [ ] Mensaje de confirmación se muestra
- [ ] Movimiento marcado como "ELIMINADO" (no borrado físicamente)
- [ ] Comentarios incluyen usuario y fecha
- [ ] Registro en tabla historial_movimientos
- [ ] Modal se cierra automáticamente
- [ ] Tabla se actualiza

### Verificación en BD:
```sql
-- Verificar soft delete
SELECT folio, estado, comentarios_autorizacion 
FROM movimientos_financieros 
WHERE estado = 'ELIMINADO';

-- Verificar auditoría
SELECT * FROM historial_movimientos 
ORDER BY fecha_accion DESC 
LIMIT 10;
```

---

## 📑 BAN-45: PAGINACIÓN

### Pasos de Prueba:
1. Verificar que hay más de 20 movimientos en la BD
2. Abrir módulo "Revisión de Movimientos"
3. Verificar indicador "Página 1 de X"
4. Hacer clic en "Siguiente ▶"
5. Hacer clic en "◀ Anterior"

### ✅ Criterios de Aceptación:
- [ ] Muestra máximo 20 registros por página
- [ ] Indicador "Página X de Y" correcto
- [ ] Botón "Anterior" deshabilitado en página 1
- [ ] Botón "Siguiente" deshabilitado en última página
- [ ] Navegación funciona correctamente
- [ ] Resumen ejecutivo muestra totales de TODOS los datos (no solo página actual)
- [ ] Filtros se mantienen al cambiar de página

---

## 📤 BAN-46: EXPORTACIÓN PDF/WORD/EXCEL

### Pasos de Prueba:

#### Exportar PDF:
1. Aplicar algunos filtros (opcional)
2. Hacer clic en botón "📄 PDF"
3. Confirmar en vista previa
4. Verificar archivo generado

#### Exportar Word:
1. Hacer clic en botón "📝 Word"
2. Confirmar en vista previa
3. Verificar archivo .doc generado

#### Exportar Excel:
1. Hacer clic en botón "📊 Excel"
2. Confirmar en vista previa
3. Verificar archivo .csv generado
4. Abrir con Excel

### ✅ Criterios de Aceptación:
- [ ] Tres botones visibles en barra superior
- [ ] PDF genera archivo .txt con formato profesional
- [ ] Word genera archivo .doc
- [ ] Excel genera archivo .csv compatible
- [ ] Todos respetan filtros aplicados
- [ ] Archivos contienen todos los datos filtrados
- [ ] Nombres de archivo incluyen timestamp

---

## 👁️ BAN-47: VISTA PREVIA DE EXPORTACIÓN

### Pasos de Prueba:
1. Hacer clic en cualquier botón de exportación
2. Verificar modal de vista previa
3. Revisar primeras 20 filas
4. Probar "Confirmar" y "Cancelar"

### ✅ Criterios de Aceptación:
- [ ] Modal de vista previa se abre antes de exportar
- [ ] Muestra primeras 20 filas en DataGridView
- [ ] Información clara sobre cuántos registros se exportarán
- [ ] Botón "Confirmar" procede con exportación
- [ ] Botón "Cancelar" cierra modal sin exportar
- [ ] Vista previa muestra datos correctos

---

## 🔄 BAN-48: ACTUALIZACIÓN AUTOMÁTICA

### Pasos de Prueba:
1. Abrir módulo "Revisión de Movimientos"
2. Anotar timestamp de "Última actualización"
3. Esperar 30 segundos
4. Verificar que timestamp se actualiza
5. Agregar un movimiento desde otro cliente/navegador
6. Esperar 30 segundos
7. Verificar que aparece el nuevo movimiento

### ✅ Criterios de Aceptación:
- [ ] Timer configurado a 30 segundos
- [ ] Timestamp se actualiza automáticamente
- [ ] Datos se recargan sin intervención del usuario
- [ ] No hay errores en consola
- [ ] Si falla conexión, muestra mensaje de error
- [ ] Timer se detiene al cerrar formulario

---

## 🎨 BAN-49: DISEÑO VISUAL OPTIMIZADO

### Pasos de Prueba:
1. Abrir módulo "Revisión de Movimientos"
2. Verificar colores de tipos de operación
3. Verificar colores de estados
4. Verificar legibilidad de texto

### ✅ Criterios de Aceptación:
- [ ] Cargos en color rojo (#DC3545)
- [ ] Abonos en color verde (#28A745)
- [ ] Estados con colores de fondo:
  - PENDIENTE: amarillo
  - PROCESADO: verde
  - RECHAZADO: rojo
- [ ] Tipografía Segoe UI legible
- [ ] Contraste adecuado en todos los elementos
- [ ] Diseño profesional y consistente

---

## 🔄 BAN-50: BOTÓN REFRESCAR MANUAL

### Pasos de Prueba:
1. Abrir módulo "Revisión de Movimientos"
2. Hacer clic en botón "🔄 Refrescar"
3. Verificar actualización

### ✅ Criterios de Aceptación:
- [ ] Botón visible en esquina inferior derecha
- [ ] Color verde (#28A745)
- [ ] Al hacer clic, datos se actualizan inmediatamente
- [ ] Mensaje de confirmación se muestra
- [ ] Timestamp se actualiza con "✅ Actualizado manualmente"
- [ ] No interfiere con actualización automática

---

## 🔐 PRUEBAS DE SEGURIDAD

### Permisos de Usuario:
- [ ] Gerente puede editar y eliminar
- [ ] Administrador puede editar y eliminar
- [ ] Cajero NO puede editar ni eliminar (botones deshabilitados)
- [ ] Analista NO puede editar ni eliminar (botones deshabilitados)

### Auditoría:
```sql
-- Verificar que se registran cambios
SELECT 
    h.folio,
    h.accion,
    h.campo_modificado,
    h.valor_anterior,
    h.valor_nuevo,
    h.usuario,
    h.fecha_accion
FROM historial_movimientos h
ORDER BY h.fecha_accion DESC
LIMIT 20;
```

---

## 📊 PRUEBAS DE INTEGRACIÓN

### Filtros + Paginación:
1. Aplicar filtro de tipo "CARGO"
2. Verificar paginación con datos filtrados
3. Cambiar a página 2
4. Verificar que filtro se mantiene

### Filtros + Exportación:
1. Aplicar filtro de beneficiario
2. Exportar a Excel
3. Verificar que archivo solo contiene datos filtrados

### Búsqueda + Paginación + Exportación:
1. Buscar "Pago"
2. Verificar resultados paginados
3. Exportar a PDF
4. Verificar que PDF contiene solo resultados de búsqueda

---

## 🐛 CASOS DE PRUEBA NEGATIVOS

### Sin Datos:
- [ ] Aplicar filtros que no devuelven resultados
- [ ] Verificar mensaje "No se encontraron movimientos"
- [ ] Verificar que exportación muestra advertencia

### Conexión Fallida:
- [ ] Desconectar internet
- [ ] Esperar actualización automática
- [ ] Verificar mensaje de error
- [ ] Reconectar y verificar recuperación

### Permisos Insuficientes:
- [ ] Iniciar sesión como Cajero
- [ ] Intentar editar (botón deshabilitado)
- [ ] Intentar eliminar (botón deshabilitado)

---

## 📝 REPORTE DE PRUEBAS

### Resumen:
- Total de pruebas: ___
- Pruebas exitosas: ___
- Pruebas fallidas: ___
- Bugs encontrados: ___

### Bugs Encontrados:
1. 
2. 
3. 

### Observaciones:


### Aprobación:
- [ ] Todas las funcionalidades BAN-41 a BAN-50 funcionan correctamente
- [ ] Sistema listo para producción

---

**Fecha de pruebas:** _______________
**Probado por:** _______________
**Firma:** _______________

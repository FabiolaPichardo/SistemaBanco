# 📖 Guía de Usuario - Banco Premier

## Bienvenido al Sistema Bancario Profesional

Esta guía te ayudará a utilizar todas las funcionalidades del sistema Banco Premier.

---

## 🔐 1. Inicio de Sesión

### Pantalla de Login
Al iniciar la aplicación, verás una elegante pantalla de inicio de sesión con:

- **Logo del banco** (🏦)
- **Título**: "BANCO PREMIER"
- **Subtítulo**: "Banca Digital Segura"
- **Campos de entrada**:
  - Usuario
  - Contraseña
- **Botones**:
  - INGRESAR (azul corporativo)
  - Salir (gris)

### Credenciales de Prueba

| Usuario | Contraseña | Perfil |
|---------|------------|--------|
| admin | admin123 | Administrador |
| jperez | pass123 | Cliente |
| mlopez | pass123 | Cliente |

### Pasos para Ingresar:
1. Escribe tu nombre de usuario
2. Escribe tu contraseña
3. Presiona ENTER o haz clic en "INGRESAR"
4. Si las credenciales son correctas, accederás al menú principal

### Mensajes de Error:
- "Ingrese usuario y contraseña" - Si dejas campos vacíos
- "Usuario o contraseña incorrectos" - Si las credenciales no coinciden

---

## 🏠 2. Menú Principal

### Diseño del Menú
El menú principal presenta un diseño moderno tipo dashboard con:

**Header Superior (Azul Corporativo)**
- Logo: "🏦 BANCO PREMIER"
- Mensaje de bienvenida: "Bienvenido, [Tu Nombre]"
- Fecha actual

**Panel de Control**
Seis tarjetas interactivas organizadas en dos filas:

#### Primera Fila:
1. **💰 Consultar Saldo**
   - Ver el saldo actual de tu cuenta
   
2. **💳 Nuevo Movimiento**
   - Registrar depósitos y retiros
   
3. **🔄 Transferencias**
   - Transferir entre cuentas

#### Segunda Fila:
4. **📊 Historial**
   - Ver movimientos realizados
   
5. **📄 Estado de Cuenta**
   - Generar reporte detallado
   
6. **👤 Mi Perfil**
   - Configuración de cuenta (en desarrollo)

**Botón Inferior**
- 🚪 CERRAR SESIÓN (gris)

### Interacción:
- **Hover**: Las tarjetas cambian de color al pasar el mouse
- **Click**: Haz clic en cualquier parte de la tarjeta para acceder

---

## 💰 3. Consultar Saldo

### Información Mostrada:
- **Número de Cuenta**: Tu número de cuenta bancaria
- **Saldo Disponible**: Monto actual en formato de moneda ($XX,XXX.XX)
- **Fecha de Actualización**: Timestamp de la consulta

### Características:
- Saldo mostrado en **verde** con fuente grande
- Diseño limpio tipo tarjeta bancaria
- Actualización en tiempo real

### Botones:
- **CERRAR**: Volver al menú principal

---

## 💳 4. Registrar Movimiento

### Tipos de Movimiento:
1. **DEPOSITO**: Agregar dinero a tu cuenta
2. **RETIRO**: Sacar dinero de tu cuenta
3. **CARGO**: Registrar un cargo (pago de servicios, etc.)
4. **ABONO**: Registrar un abono

### Campos del Formulario:

#### Tipo de Movimiento
- Lista desplegable con las 4 opciones
- Selección obligatoria

#### Monto ($)
- Campo numérico
- Solo acepta números y punto decimal
- Debe ser mayor a 0
- Formato: 1000.50

#### Concepto / Descripción
- Campo de texto multilínea
- Opcional pero recomendado
- Describe el motivo del movimiento

### Validaciones:
- ✓ Monto debe ser mayor a 0
- ✓ Para RETIRO/CARGO: verifica saldo suficiente
- ✓ Solo números en el campo de monto

### Proceso:
1. Selecciona el tipo de movimiento
2. Ingresa el monto
3. Escribe un concepto (opcional)
4. Haz clic en "✓ GUARDAR"
5. Confirma la operación
6. Verás un mensaje de éxito con el nuevo saldo

### Botones:
- **✓ GUARDAR** (azul): Procesar el movimiento
- **✗ CANCELAR** (gris): Cancelar y volver

### Mensajes:
- **Éxito**: "Movimiento registrado exitosamente. Nuevo saldo: $XX,XXX.XX"
- **Error**: "Saldo insuficiente" (para retiros/cargos)

---

## 🔄 5. Transferencias

### Funcionalidad Avanzada
Sistema de transferencias entre cuentas con validación en tiempo real.

### Campos del Formulario:

#### Cuenta Destino
- Número de cuenta del beneficiario
- **Validación automática**: Al salir del campo, verifica si la cuenta existe
- Muestra el nombre del titular si es válida
- Indicadores:
  - ✓ Verde: Cuenta válida
  - ✗ Rojo: Cuenta no encontrada

#### Monto a Transferir ($)
- Campo numérico
- Solo acepta números y punto decimal
- Debe ser mayor a 0

#### Concepto / Referencia
- Descripción de la transferencia
- Campo multilínea
- Opcional

### Panel de Advertencia:
⚠️ **"Verifique los datos antes de confirmar. Las transferencias son irreversibles."**

### Validaciones:
- ✓ Cuenta destino debe existir
- ✓ Cuenta destino debe ser diferente a la tuya
- ✓ Monto debe ser mayor a 0
- ✓ Debes tener saldo suficiente
- ✓ Confirmación antes de procesar

### Proceso:
1. Ingresa el número de cuenta destino
2. Espera la validación automática
3. Verifica que aparezca el nombre del beneficiario
4. Ingresa el monto
5. Escribe un concepto (opcional)
6. Haz clic en "✓ TRANSFERIR"
7. Confirma en el diálogo de confirmación
8. La transferencia se procesa

### Registro de Movimientos:
La transferencia crea **dos movimientos**:
- En tu cuenta: "TRANSFERENCIA ENVIADA" (resta)
- En cuenta destino: "TRANSFERENCIA RECIBIDA" (suma)

### Botones:
- **✓ TRANSFERIR** (azul): Procesar transferencia
- **✗ CANCELAR** (gris): Cancelar operación

---

## 📊 6. Historial de Movimientos

### Vista de Tabla
Muestra todos tus movimientos en una tabla profesional con:

### Columnas:
1. **Fecha**: Fecha del movimiento
2. **Tipo**: Tipo de operación
3. **Monto**: Cantidad en formato de moneda
4. **Concepto**: Descripción del movimiento
5. **Saldo Anterior**: Saldo antes del movimiento
6. **Saldo Nuevo**: Saldo después del movimiento

### Características:
- **Ordenamiento**: Por fecha descendente (más recientes primero)
- **Colores alternados**: Filas con fondo alternado para mejor lectura
- **Formato de moneda**: Todos los montos con formato $XX,XXX.XX
- **Selección completa**: Al hacer clic, selecciona toda la fila
- **Scroll**: Si hay muchos movimientos, aparece barra de desplazamiento

### Tipos de Movimiento:
- DEPOSITO
- RETIRO
- CARGO
- ABONO
- TRANSFERENCIA ENVIADA
- TRANSFERENCIA RECIBIDA

### Botones:
- **CERRAR**: Volver al menú principal

---

## 📄 7. Estado de Cuenta

### Funcionalidad Completa
Genera un estado de cuenta detallado con filtros por fecha.

### Panel de Filtros:

#### Fecha Inicio
- Selector de fecha (DatePicker)
- Por defecto: Hace 1 mes

#### Fecha Fin
- Selector de fecha (DatePicker)
- Por defecto: Hoy

#### Botón Filtrar
- 🔍 FILTRAR: Aplica el rango de fechas seleccionado

### Panel de Resumen:
Muestra 4 indicadores clave:

1. **Saldo Inicial**
   - Saldo al inicio del período
   - Color: Negro

2. **Total Ingresos**
   - Suma de: DEPOSITO + ABONO + TRANSFERENCIA RECIBIDA
   - Color: Verde
   - Formato: +$XX,XXX.XX

3. **Total Egresos**
   - Suma de: RETIRO + CARGO + TRANSFERENCIA ENVIADA
   - Color: Rojo
   - Formato: -$XX,XXX.XX

4. **Saldo Final**
   - Saldo al final del período
   - Color: Azul corporativo
   - Fuente más grande

### Tabla de Movimientos:
Similar al historial, pero filtrada por el rango de fechas seleccionado.

### Cálculo del Resumen:
```
Saldo Inicial = Saldo Final - Ingresos + Egresos
Saldo Final = Saldo Inicial + Ingresos - Egresos
```

### Botones:
- **📥 EXPORTAR PDF** (azul): Exportar a PDF (en desarrollo)
- **CERRAR** (gris): Volver al menú principal

### Casos de Uso:
- Ver movimientos del último mes
- Generar reporte trimestral
- Revisar movimientos de un período específico
- Análisis de ingresos y gastos

---

## 🎨 8. Características de Diseño

### Paleta de Colores:
- **Azul Corporativo**: #003366 (Headers, títulos)
- **Azul Secundario**: #0066CC (Hover en botones)
- **Dorado Elegante**: #D4AF37 (Acentos, subtítulos)
- **Gris Claro**: #F5F5F5 (Fondos)
- **Verde Éxito**: #28A745 (Saldos, confirmaciones)
- **Rojo Peligro**: #DC3545 (Errores, egresos)

### Tipografía:
- **Fuente**: Segoe UI (moderna y profesional)
- **Títulos**: 18pt Bold
- **Subtítulos**: 14pt Bold
- **Headers**: 12pt Bold
- **Cuerpo**: 10pt Regular
- **Montos**: 24pt Bold

### Efectos Visuales:
- **Tarjetas**: Bordes sutiles, fondo blanco
- **Hover**: Cambio de color al pasar el mouse
- **Botones**: Flat design con colores corporativos
- **Inputs**: Bordes simples, fondo blanco

---

## ⚠️ 9. Mensajes y Validaciones

### Mensajes de Éxito:
- ✓ "Movimiento registrado exitosamente"
- ✓ "Transferencia realizada exitosamente"
- ✓ Incluyen el nuevo saldo

### Mensajes de Error:
- ✗ "Ingrese usuario y contraseña"
- ✗ "Usuario o contraseña incorrectos"
- ✗ "Ingrese el monto"
- ✗ "Ingrese un monto válido mayor a 0"
- ✗ "Saldo insuficiente"
- ✗ "La cuenta destino no es válida"

### Mensajes de Confirmación:
- ❓ "¿Confirma la transferencia de $X,XXX.XX?"
- Incluyen detalles de la operación
- Requieren confirmación explícita

---

## 🔒 10. Seguridad

### Características de Seguridad:
1. **Autenticación**: Usuario y contraseña requeridos
2. **Sesiones**: Control de sesión activa
3. **Validaciones**: Todas las operaciones son validadas
4. **Confirmaciones**: Operaciones críticas requieren confirmación
5. **Auditoría**: Todos los movimientos quedan registrados con:
   - Fecha y hora exacta
   - Saldo anterior y nuevo
   - Concepto de la operación

### Buenas Prácticas:
- ✓ Cierra sesión al terminar
- ✓ No compartas tus credenciales
- ✓ Verifica los datos antes de confirmar transferencias
- ✓ Revisa tu historial regularmente
- ✓ Reporta cualquier movimiento no reconocido

---

## 🆘 11. Solución de Problemas

### No puedo iniciar sesión
- Verifica que el usuario y contraseña sean correctos
- Asegúrate de que la base de datos esté corriendo
- Revisa la conexión a PostgreSQL

### No veo mis movimientos
- Verifica que estés usando la cuenta correcta
- Revisa el rango de fechas en Estado de Cuenta
- Asegúrate de que los movimientos estén registrados

### Error al hacer transferencia
- Verifica que la cuenta destino exista
- Asegúrate de tener saldo suficiente
- Confirma que el monto sea válido

### La aplicación no responde
- Verifica la conexión a la base de datos
- Revisa que PostgreSQL esté corriendo
- Reinicia la aplicación

---

## 📞 12. Soporte

### Información de Contacto:
- **Email**: soporte@bancopremier.com
- **Teléfono**: 555-0000
- **Horario**: Lunes a Viernes, 9:00 AM - 6:00 PM

### Recursos Adicionales:
- Manual técnico: Ver README.md
- Script de base de datos: database_setup.sql
- Código fuente: Disponible en el repositorio

---

## ✨ 13. Consejos y Trucos

### Atajos de Teclado:
- **ENTER** en login: Inicia sesión
- **ESC**: Cierra diálogos (en algunos casos)

### Navegación Rápida:
- Las tarjetas del menú responden al click en cualquier parte
- Los botones cambian de color al pasar el mouse

### Mejores Prácticas:
1. Siempre escribe un concepto en tus movimientos
2. Revisa el historial semanalmente
3. Usa el estado de cuenta para análisis mensual
4. Verifica dos veces antes de transferir

---

**¡Gracias por usar Banco Premier!** 🏦

*Banca Digital Segura - Tu confianza es nuestro compromiso*
